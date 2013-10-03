// shortcuts
var initBitModels = RangeCoder.Encoder.initBitModels;

// constants
var EMatchFinderTypeBT2 = 0;
var EMatchFinderTypeBT4 = 1;

var kInfinityPrice = 0xFFFFFFF;
var kDefaultDictionaryLogSize = 22;
var kNumFastBytesDefault = 0x20;

var kNumOpts = 1 << 12;

var kPropSize = 5;

var g_FastPos = (function() {
  var g_FastPos = makeBuffer(1 << 11);
  var kFastSlots = 22;
  var c = 2;
  var slotFast;
  g_FastPos[0] = 0;
  g_FastPos[1] = 1;
  for (slotFast = 2; slotFast < kFastSlots; slotFast++) {
    var j, k = 1 << ((slotFast >> 1) - 1);
    for (j = 0; j < k; j++,c++) {
      g_FastPos[c] = slotFast;
    }
  }
  return g_FastPos;
})();

var getPosSlot = function(pos) {
  if (pos < (1 << 11)) {
    return g_FastPos[pos];
  }
  if (pos < (1 << 21)) {
    return g_FastPos[pos >>> 10] + 20;
  }
  return g_FastPos[pos >>> 20] + 40;
};

var getPosSlot2 = function(pos) {
  if (pos < (1 << 17)) {
    return g_FastPos[pos >>> 6] + 12;
  }
  if (pos < (1 << 27)) {
    return g_FastPos[pos >>> 16] + 32;
  }
  return g_FastPos[pos >>> 26] + 52;
};

var Encoder = function() {
  var i;
  this._state = Base.stateInit();
  this._previousByte = 0;
  this._repDistances = []; // XXX use Uint32Array?
  this._repDistances.length = Base.kNumRepDistances;

  // these fields are defined much lower in the original Java source file
  this._optimum = [];
  this._matchFinder = null;
  this._rangeEncoder = new RangeCoder.Encoder();

  this._isMatch = initBitModels(null, Base.kNumStates << Base.kNumPosStatesBitsMax);
  this._isRep = initBitModels(null, Base.kNumStates);
  this._isRepG0 = initBitModels(null, Base.kNumStates);
  this._isRepG1 = initBitModels(null, Base.kNumStates);
  this._isRepG2 = initBitModels(null, Base.kNumStates);
  this._isRep0Long = initBitModels(null, Base.kNumStates << Base.kNumPosStatesBitsMax);

  this._posSlotEncoder = [];

  this._posEncoders = initBitModels(null, Base.kNumFullDistances-Base.kEndPosModelIndex);

  this._posAlignEncoder = new RangeCoder.BitTreeEncoder(Base.kNumAlignBits);

  this._lenEncoder = new Encoder.LenPriceTableEncoder();
  this._repMatchLenEncoder = new Encoder.LenPriceTableEncoder();

  this._literalEncoder = new Encoder.LiteralEncoder();

  this._matchDistances = [];
  this._matchDistances.length = Base.kMatchMaxLen*2 + 2;
  for (i=0; i < this._matchDistances.length; i++) {
      this._matchDistances[i] = 0;
  }

  this._numFastBytes = kNumFastBytesDefault;
  this._longestMatchLength = 0;
  this._numDistancePairs = 0;

  this._additionalOffset = 0;
  this._optimumEndIndex = 0;
  this._optimumCurrentIndex = 0;

  this._longestMatchWasFound = false;

  this._posSlotPrices = [];
  this._distancesPrices = [];
  this._alignPrices = [];
  this._alignPriceCount = 0;

  this._distTableSize = kDefaultDictionaryLogSize * 2;

  this._posStateBits = 2;
  this._posStateMask = 4 - 1;
  this._numLiteralPosStateBits = 0;
  this._numLiteralContextBits = 3;

  this._dictionarySize = (1 << kDefaultDictionaryLogSize);
  this._dictionarySizePrev = 0xFFFFFFFF;
  this._numFastBytesPrev = 0xFFFFFFFF;

  // note that this is a 53-bit variable, not 64-bit.  This sets the maximum
  // encoded file size.
  this.nowPos64 = 0;
  this._finished = false;
  this._inStream = null;

  this._matchFinderType = EMatchFinderTypeBT4;
  this._writeEndMark = false;
  this._needReleaseMFStream = false;

  // ...and even further down we find these:
  this.reps = [];
  this.repLens = [];
  this.backRes = 0;

  // ...keep going, eventually we find the constructor:
  for (i = 0; i < kNumOpts; i++) {
    this._optimum[i] = new Encoder.Optimal();
  }
  for (i = 0; i < Base.kNumLenToPosStates; i++) {
    this._posSlotEncoder[i] = new RangeCoder.BitTreeEncoder(Base.kNumPosSlotBits);
  }

  this._matchPriceCount = 0;

  // ...and just above the 'Code' method, we find:
  this.processedInSize = [0];
  this.processedOutSize = [0];
  this.finished = [false];
};
Encoder.prototype.baseInit = function() {
  var i;
  this._state = Base.stateInit();
  this._previousByte = 0;
  for (i = 0; i < Base.kNumRepDistances; i++) {
    this._repDistances[i] = 0;
  }
};

var LiteralEncoder = Encoder.LiteralEncoder = function() {
  this._coders = null;
  this._numPrevBits = -1;
  this._numPosBits = -1;
  this._posMask = 0;
};

LiteralEncoder.Encoder2 = function() {
  this._encoders = initBitModels(null, 0x300);
};

LiteralEncoder.Encoder2.prototype.init = function() {
  initBitModels(this._encoders);
};

LiteralEncoder.Encoder2.prototype.encode = function(rangeEncoder, symbol) {
  var context = 1, i;
  for (i = 7; i >= 0; i--) {
    var bit = (symbol >>> i) & 1;
    rangeEncoder.encode(this._encoders, context, bit);
    context = (context << 1) | bit;
  }
};

LiteralEncoder.Encoder2.prototype.encodeMatched = function(rangeEncoder, matchByte, symbol) {
  var context = 1, same = true, i;
  for (i = 7; i>= 0; i--) {
    var bit = (symbol >> i) & 1;
    var state = context;
    if (same) {
      var matchBit = (matchByte >>> i) & 1;
      state += (1 + matchBit) << 8;
      same = (matchBit === bit);
    }
    rangeEncoder.encode(this._encoders, state, bit);
    context = (context << 1) | bit;
  }
};

LiteralEncoder.Encoder2.prototype.getPrice = function(matchMode, matchByte, symbol) {
  var price = 0;
  var context = 1;
  var i = 7;
  var bit, matchBit;
  if (matchMode) {
    for (; i >= 0; i--) {
      matchBit = (matchByte >>> i) & 1;
      bit = (symbol >>> i) & 1;
      price += RangeCoder.Encoder.getPrice(this._encoders[((1 + matchBit) << 8) + context], bit);
      context = (context << 1) | bit;
      if (matchBit !== bit) {
        i--;
        break;
      }
    }
  }
  for (; i >= 0; i--) {
    bit = (symbol >>> i) & 1;
    price += RangeCoder.Encoder.getPrice(this._encoders[context], bit);
    context = (context << 1) | bit;
  }
  return price;
};

LiteralEncoder.prototype.create = function(numPosBits, numPrevBits) {
  var i;
  if (this._coders &&
      this._numPrevBits === numPrevBits &&
      this._numPosBits === numPosBits) {
    return;
  }

  this._numPosBits = numPosBits;
  this._posMask = (1 << numPosBits) - 1;
  this._numPrevBits = numPrevBits;
  var numStates = 1 << (this._numPrevBits + this._numPosBits);
  this._coders = [];
  for (i = 0; i < numStates; i++) {
    this._coders[i] = new LiteralEncoder.Encoder2();
  }
};

LiteralEncoder.prototype.init = function() {
  var numStates = 1 << (this._numPrevBits + this._numPosBits), i;
  for (i = 0; i < numStates; i++) {
    this._coders[i].init();
  }
};

LiteralEncoder.prototype.getSubCoder = function(pos, prevByte) {
  return this._coders[((pos & this._posMask) << this._numPrevBits) + (prevByte >> (8 - this._numPrevBits))];
};

var LenEncoder = Encoder.LenEncoder = function() {
  var posState;
  this._choice = initBitModels(null, 2);
  this._lowCoder = [];
  this._midCoder = [];
  this._highCoder = new RangeCoder.BitTreeEncoder(Base.kNumHighLenBits);

  for (posState = 0; posState < Base.kNumPosStatesEncodingMax; posState++) {
    this._lowCoder[posState] = new RangeCoder.BitTreeEncoder(Base.kNumLowLenBits);
    this._midCoder[posState] = new RangeCoder.BitTreeEncoder(Base.kNumMidLenBits);
  }
};

LenEncoder.prototype.init = function(numPosStates) {
  var posState;
  initBitModels(this._choice);
  for (posState = 0; posState < numPosStates; posState++) {
    this._lowCoder[posState].init();
    this._midCoder[posState].init();
  }
  this._highCoder.init();
};

LenEncoder.prototype.encode = function(rangeEncoder, symbol, posState) {
  if (symbol < Base.kNumLowLenSymbols) {
    rangeEncoder.encode(this._choice, 0, 0);
    this._lowCoder[posState].encode(rangeEncoder, symbol);
  } else {
    symbol -= Base.kNumLowLenSymbols;
    rangeEncoder.encode(this._choice, 0, 1);
    if (symbol < Base.kNumMidLenSymbols) {
      rangeEncoder.encode(this._choice, 1, 0);
      this._midCoder[posState].encode(rangeEncoder, symbol);
    } else {
      rangeEncoder.encode(this._choice, 1, 1);
      this._highCoder.encode(rangeEncoder, symbol - Base.kNumMidLenSymbols);
    }
  }
};

LenEncoder.prototype.setPrices = function(posState, numSymbols, prices, st) {
  var a0 = RangeCoder.Encoder.getPrice0(this._choice[0]);
  var a1 = RangeCoder.Encoder.getPrice1(this._choice[0]);
  var b0 = a1 + RangeCoder.Encoder.getPrice0(this._choice[1]);
  var b1 = a1 + RangeCoder.Encoder.getPrice1(this._choice[1]);
  var i;
  for (i = 0; i < Base.kNumLowLenSymbols; i++) {
    if (i >= numSymbols) {
      return;
    }
    prices[st + i] = a0 + this._lowCoder[posState].getPrice(i);
  }
  for (; i < Base.kNumLowLenSymbols + Base.kNumMidLenSymbols; i++) {
    if (i >= numSymbols) {
      return;
    }
    prices[st + i] = b0 + this._midCoder[posState].getPrice(i - Base.kNumLowLenSymbols);
  }
  for (; i < numSymbols; i++) {
    prices[st + i] = b1 + this._highCoder.getPrice(i - Base.kNumLowLenSymbols - Base.kNumMidLenSymbols);
  }
};

var kNumLenSpecSymbols = Base.kNumLowLenSymbols + Base.kNumMidLenSymbols;

var LenPriceTableEncoder = Encoder.LenPriceTableEncoder = function() {
  LenEncoder.call(this); // superclass constructor
  this._prices = [];
  this._counters = [];
  this._tableSize = 0;
};
LenPriceTableEncoder.prototype = Object.create(LenEncoder.prototype);
LenPriceTableEncoder.prototype.setTableSize = function(tableSize) {
  this._tableSize = tableSize;
};
LenPriceTableEncoder.prototype.getPrice = function(symbol, posState) {
  return this._prices[posState * Base.kNumLenSymbols + symbol];
};
LenPriceTableEncoder.prototype.updateTable = function(posState) {
  this.setPrices(posState, this._tableSize, this._prices,
                 posState * Base.kNumLenSymbols);
  this._counters[posState] = this._tableSize;
};
LenPriceTableEncoder.prototype.updateTables = function(numPosStates) {
  var posState;
  for (posState = 0; posState < numPosStates; posState++) {
    this.updateTable(posState);
  }
};
LenPriceTableEncoder.prototype.encode = (function(superEncode) {
  return function(rangeEncoder, symbol, posState) {
    superEncode.call(this, rangeEncoder, symbol, posState);
    if (--this._counters[posState] === 0) {
      this.updateTable(posState);
    }
  };
})(LenPriceTableEncoder.prototype.encode);

var Optimal = Encoder.Optimal = function() {
  this.state = 0;

  this.prev1IsChar = false;
  this.prev2 = false;

  this.posPrev2 = 0;
  this.backPrev2 = 0;

  this.price = 0;
  this.posPrev = 0;
  this.backPrev = 0;

  this.backs0 = 0;
  this.backs1 = 0;
  this.backs2 = 0;
  this.backs3 = 0;
};
Optimal.prototype.makeAsChar = function() {
  this.backPrev = -1;
  this.prev1IsChar = false;
};
Optimal.prototype.makeAsShortRep = function() {
  this.backPrev = 0;
  this.prev1IsChar= false;
};
Optimal.prototype.isShortRep = function() {
  return this.backPrev === 0;
};

// back to the Encoder class!
Encoder.prototype.create = function() {
  var numHashBytes;
  if (!this._matchFinder) {
    var bt = new LZ.BinTree();
    numHashBytes = 4;
    if (this._matchFinderType === EMatchFinderTypeBT2) {
      numHashBytes = 2;
    }
    bt.setType(numHashBytes);
    this._matchFinder = bt;
  }
  this._literalEncoder.create(this._numLiteralPosStateBits,
                              this._numLiteralContextBits);

  if (this._dictionarySize === this._dictionarySizePrev &&
      this._numFastBytesPrev === this._numFastBytes) {
    return;
  }
  this._matchFinder.create(this._dictionarySize, kNumOpts, this._numFastBytes,
                           Base.kMatchMaxLen + 1);
  this._dictionarySizePrev = this._dictionarySize;
  this._numFastBytesPrev = this._numFastBytes;
};

Encoder.prototype.setWriteEndMarkerMode = function(writeEndMarker) {
  this._writeEndMark = writeEndMarker;
};

Encoder.prototype.init = function() {
  var i;
  this.baseInit();
  this._rangeEncoder.init();

  initBitModels(this._isMatch);
  initBitModels(this._isRep0Long);
  initBitModels(this._isRep);
  initBitModels(this._isRepG0);
  initBitModels(this._isRepG1);
  initBitModels(this._isRepG2);
  initBitModels(this._posEncoders);

  this._literalEncoder.init();
  for (i = 0; i < Base.kNumLenToPosStates; i++) {
    this._posSlotEncoder[i].init();
  }

  this._lenEncoder.init(1 << this._posStateBits);
  this._repMatchLenEncoder.init(1 << this._posStateBits);

  this._posAlignEncoder.init();

  this._longestMatchWasFound = false;
  this._optimumEndIndex = 0;
  this._optimumCurrentIndex = 0;
  this._additionalOffset = 0;
};

Encoder.prototype.readMatchDistances = function() {
  var lenRes = 0;
  this._numDistancePairs = this._matchFinder.getMatches(this._matchDistances);
  if (this._numDistancePairs > 0) {
    lenRes = this._matchDistances[this._numDistancePairs - 2];
    if (lenRes === this._numFastBytes) {
      lenRes += this._matchFinder.getMatchLen(lenRes - 1, this._matchDistances[this._numDistancePairs - 1], Base.kMatchMaxLen - lenRes);
    }
  }
  this._additionalOffset++;
  // [csa] Gary Linscott thinks that numDistancePairs should be a retval here.
  return lenRes;
};

Encoder.prototype.movePos = function(num) {
  if (num > 0) {
    this._matchFinder.skip(num);
    this._additionalOffset += num;
  }
};

Encoder.prototype.getRepLen1Price = function(state, posState) {
  return RangeCoder.Encoder.getPrice0(this._isRepG0[state]) +
    RangeCoder.Encoder.getPrice0(this._isRep0Long[(state << Base.kNumPosStatesBitsMax) + posState]);
};

Encoder.prototype.getPureRepPrice = function(repIndex, state, posState) {
  var price;
  if (repIndex === 0) {
    price = RangeCoder.Encoder.getPrice0(this._isRepG0[state]);
    price += RangeCoder.Encoder.getPrice1(this._isRep0Long[(state << Base.kNumPosStatesBitsMax) + posState]);
  } else {
    price = RangeCoder.Encoder.getPrice1(this._isRepG0[state]);
    if (repIndex === 1) {
      price += RangeCoder.Encoder.getPrice0(this._isRepG1[state]);
    } else {
      price += RangeCoder.Encoder.getPrice1(this._isRepG1[state]);
      price += RangeCoder.Encoder.getPrice(this._isRepG2[state], repIndex - 2);
    }
  }
  return price;
};

Encoder.prototype.getRepPrice = function(repIndex, len, state, posState) {
  var price = this._repMatchLenEncoder.getPrice(len - Base.kMatchMinLen, posState);
  return price + this.getPureRepPrice(repIndex, state, posState);
};

Encoder.prototype.getPosLenPrice = function(pos, len, posState) {
  var price;
  var lenToPosState = Base.getLenToPosState(len);
  if (pos < Base.kNumFullDistances) {
    price = this._distancesPrices[(lenToPosState * Base.kNumFullDistances) + pos];
  } else {
    price = this._posSlotPrices[(lenToPosState << Base.kNumPosSlotBits) + getPosSlot2(pos)] + this._alignPrices[pos & Base.kAlignMask];
  }
  return price + this._lenEncoder.getPrice(len - Base.kMatchMinLen, posState);
};

Encoder.prototype.backward = function(cur) {
  this._optimumEndIndex = cur;
  var posMem = this._optimum[cur].posPrev;
  var backMem = this._optimum[cur].backPrev;
  do {
    if (this._optimum[cur].prev1IsChar) {
      this._optimum[posMem].makeAsChar();
      this._optimum[posMem].posPrev = posMem - 1;
      if (this._optimum[cur].prev2) {
        this._optimum[posMem - 1].prev1IsChar = false;
        this._optimum[posMem - 1].posPrev = this._optimum[cur].posPrev2;
        this._optimum[posMem - 1].backPrev = this._optimum[cur].backPrev2;
      }
    }
    var posPrev = posMem;
    var backCur = backMem;

    backMem = this._optimum[posPrev].backPrev;
    posMem = this._optimum[posPrev].posPrev;

    this._optimum[posPrev].backPrev = backCur;
    this._optimum[posPrev].posPrev = cur;
    cur = posPrev;
  } while (cur > 0);

  this.backRes = this._optimum[0].backPrev;
  this._optimumCurrentIndex = this._optimum[0].posPrev;
  // [csa] Gary Linscott thinks that backRes should be a retval here.
  return this._optimumCurrentIndex;
};

Encoder.prototype.getOptimum = function(position) {

  if (this._optimumEndIndex !== this._optimumCurrentIndex) {
    var lenRes = this._optimum[this._optimumCurrentIndex].posPrev - this._optimumCurrentIndex;
    this.backRes = this._optimum[this._optimumCurrentIndex].backPrev;
    this._optimumCurrentIndex = this._optimum[this._optimumCurrentIndex].posPrev;
    // [csa] Gary Linscott thinks that backRes should be a retval here.
    return lenRes;
  }
  this._optimumCurrentIndex = this._optimumEndIndex = 0;

  var lenMain;
  if (!this._longestMatchWasFound) {
    lenMain = this.readMatchDistances();
  } else {
    lenMain = this._longestMatchLength;
    this._longestMatchWasFound = false;
  }
  var numDistancePairs = this._numDistancePairs;

  var numAvailableBytes = this._matchFinder.getNumAvailableBytes() + 1;
  if (numAvailableBytes < 2) {
    this.backRes = -1;
    // [csa] Gary Linscott thinks that backRes should be a retval here.
    return 1;
  }
  if (numAvailableBytes > Base.kMatchMaxLen) {
    numAvailableBytes = Base.kMatchMaxLen;
  }

  var repMaxIndex = 0, i;
  for (i = 0; i < Base.kNumRepDistances; i++) {
    this.reps[i] = this._repDistances[i];
    this.repLens[i] = this._matchFinder.getMatchLen(-1, this.reps[i], Base.kMatchMaxLen);
    if (this.repLens[i] > this.repLens[repMaxIndex]) {
      repMaxIndex = i;
    }
  }
  if (this.repLens[repMaxIndex] >= this._numFastBytes) {
    this.backRes = repMaxIndex;
    var lenRes2 = this.repLens[repMaxIndex];
    this.movePos(lenRes2 - 1);
    // [csa] Gary Linscott thinks that backRes should be a retval here.
    return lenRes2;
  }

  if (lenMain >= this._numFastBytes) {
    // [csa] Gary Linscott thinks that backRes should be a retval here.
    this.backRes = this._matchDistances[numDistancePairs - 1] + Base.kNumRepDistances;
    this.movePos(lenMain - 1);
    return lenMain;
  }

  var currentByte = this._matchFinder.getIndexByte(-1);
  var matchByte = this._matchFinder.getIndexByte(-this._repDistances[0] - 2);

  if (lenMain < 2 && currentByte !== matchByte && this.repLens[repMaxIndex] < 2) {
    // [csa] Gary Linscott thinks that backRes should be a retval here.
    this.backRes = -1;
    return 1;
  }

  this._optimum[0].state = this._state;

  var posState = position & this._posStateMask;

  this._optimum[1].price = RangeCoder.Encoder.getPrice0(this._isMatch[(this._state << Base.kNumPosStatesBitsMax) + posState]) +
    this._literalEncoder.getSubCoder(position, this._previousByte).getPrice(!Base.stateIsCharState(this._state), matchByte, currentByte);
  this._optimum[1].makeAsChar();

  var matchPrice = RangeCoder.Encoder.getPrice1(this._isMatch[(this._state << Base.kNumPosStatesBitsMax) + posState]);
  var repMatchPrice = matchPrice + RangeCoder.Encoder.getPrice1(this._isRep[this._state]);

  if (matchByte === currentByte) {
    var shortRepPrice = repMatchPrice + this.getRepLen1Price(this._state, posState);
    if (shortRepPrice < this._optimum[1].price) {
      this._optimum[1].price = shortRepPrice;
      this._optimum[1].makeAsShortRep();
    }
  }

  var lenEnd = (lenMain >= this.repLens[repMaxIndex]) ?
    lenMain : this.repLens[repMaxIndex];
  if (lenEnd < 2) {
    // [csa] Gary Linscott thinks that backRes should be a retval here.
    this.backRes = this._optimum[1].backPrev;
    return 1;
  }

  this._optimum[1].posPrev = 0;

  this._optimum[0].backs0 = this.reps[0];
  this._optimum[0].backs1 = this.reps[1];
  this._optimum[0].backs2 = this.reps[2];
  this._optimum[0].backs3 = this.reps[3];

  var len = lenEnd;
  do {
    this._optimum[len--].price = kInfinityPrice;
  } while (len >= 2);

  for (i = 0; i < Base.kNumRepDistances; i++) {
    var repLen = this.repLens[i];
    if (repLen < 2) {
      continue;
    }
    var price = repMatchPrice + this.getPureRepPrice(i, this._state, posState);
    do {
      var curAndLenPrice = price +
        this._repMatchLenEncoder.getPrice(repLen - 2, posState);
      var optimum = this._optimum[repLen];
      if (curAndLenPrice < optimum.price) {
        optimum.price = curAndLenPrice;
        optimum.posPrev = 0;
        optimum.backPrev = i;
        optimum.prev1IsChar = false;
      }
    } while (--repLen >= 2);
  }

  var normalMatchPrice = matchPrice +
    RangeCoder.Encoder.getPrice0(this._isRep[this._state]);

  len = this.repLens[0] >= 2 ? this.repLens[0] + 1 : 2;
  if (len <= lenMain) {
    var offs = 0;
    while (len > this._matchDistances[offs]) {
      offs += 2;
    }
    for (;; len++) {
      var distance = this._matchDistances[offs + 1];
      var curAndLenPrice2 = normalMatchPrice +
        this.getPosLenPrice(distance, len, posState);
      var optimum2 = this._optimum[len];
      if (curAndLenPrice2 < optimum2.price) {
        optimum2.price = curAndLenPrice2;
        optimum2.posPrev = 0;
        optimum2.backPrev = distance + Base.kNumRepDistances;
        optimum2.prev1IsChar = false;
      }
      if (len === this._matchDistances[offs]) {
        offs += 2;
        if (offs === numDistancePairs) {
          break;
        }
      }
    }
  }

  var cur = 0;
  while (true) {
    cur++;
    if (cur === lenEnd) {
      return this.backward(cur);
    }
    var newLen = this.readMatchDistances();
    numDistancePairs = this._numDistancePairs;
    if (newLen >= this._numFastBytes) {
      this._longestMatchLength = newLen;
      this._longestMatchWasFound = true;
      return this.backward(cur);
    }
    position++;
    var posPrev = this._optimum[cur].posPrev;
    var state;
    if (this._optimum[cur].prev1IsChar) {
      posPrev--;
      if (this._optimum[cur].prev2) {
        state = this._optimum[this._optimum[cur].posPrev2].state;
        if (this._optimum[cur].backPrev2 < Base.kNumRepDistances) {
          state = Base.stateUpdateRep(state);
        } else {
          state = Base.stateUpdateMatch(state);
        }
      } else {
        state = this._optimum[posPrev].state;
      }
      state = Base.stateUpdateChar(state);
    } else {
      state = this._optimum[posPrev].state;
    }
    if (posPrev === cur - 1) {
      if (this._optimum[cur].isShortRep()) {
        state = Base.stateUpdateShortRep(state);
      } else {
        state = Base.stateUpdateChar(state);
      }
    } else {
      var pos;
      if (this._optimum[cur].prev1IsChar && this._optimum[cur].prev2) {
        posPrev = this._optimum[cur].posPrev2;
        pos = this._optimum[cur].backPrev2;
        state = Base.stateUpdateRep(state);
      } else {
        pos = this._optimum[cur].backPrev;
        if (pos < Base.kNumRepDistances) {
          state = Base.stateUpdateRep(state);
        } else {
          state = Base.stateUpdateMatch(state);
        }
      }

      var opt = this._optimum[posPrev];
      if (pos < Base.kNumRepDistances) {
        if (pos === 0) {
          this.reps[0] = opt.backs0;
          this.reps[1] = opt.backs1;
          this.reps[2] = opt.backs2;
          this.reps[3] = opt.backs3;
        } else if (pos === 1) {
          this.reps[0] = opt.backs1;
          this.reps[1] = opt.backs0;
          this.reps[2] = opt.backs2;
          this.reps[3] = opt.backs3;
        } else if (pos === 2) {
          this.reps[0] = opt.backs2;
          this.reps[1] = opt.backs0;
          this.reps[2] = opt.backs1;
          this.reps[3] = opt.backs3;
        } else {
          this.reps[0] = opt.backs3;
          this.reps[1] = opt.backs0;
          this.reps[2] = opt.backs1;
          this.reps[3] = opt.backs2;
        }
      } else {
        this.reps[0] = pos - Base.kNumRepDistances;
        this.reps[1] = opt.backs0;
        this.reps[2] = opt.backs1;
        this.reps[3] = opt.backs2;
      }
    }
    this._optimum[cur].state = state;
    this._optimum[cur].backs0 = this.reps[0];
    this._optimum[cur].backs1 = this.reps[1];
    this._optimum[cur].backs2 = this.reps[2];
    this._optimum[cur].backs3 = this.reps[3];
    var curPrice = this._optimum[cur].price;

    currentByte = this._matchFinder.getIndexByte(-1);
    matchByte = this._matchFinder.getIndexByte(-this.reps[0] - 2);
    posState = position & this._posStateMask;

    var curAnd1Price = curPrice +
      RangeCoder.Encoder.getPrice0(this._isMatch[(state << Base.kNumPosStatesBitsMax) + posState]) +
      this._literalEncoder.getSubCoder(position, this._matchFinder.getIndexByte(-2)).
      getPrice(!Base.stateIsCharState(state), matchByte, currentByte);

    var nextOptimum = this._optimum[cur + 1];

    var nextIsChar = false;
    if (curAnd1Price < nextOptimum.price) {
      nextOptimum.price = curAnd1Price;
      nextOptimum.posPrev = cur;
      nextOptimum.makeAsChar();
      nextIsChar = true;
    }

    matchPrice = curPrice + RangeCoder.Encoder.getPrice1(this._isMatch[(state << Base.kNumPosStatesBitsMax) + posState]);
    repMatchPrice = matchPrice + RangeCoder.Encoder.getPrice1(this._isRep[state]);

    if (matchByte === currentByte &&
        !(nextOptimum.posPrev < cur && nextOptimum.backPrev === 0)) {
      var shortRepPrice2 =
        repMatchPrice + this.getRepLen1Price(state, posState);
      if (shortRepPrice2 <= nextOptimum.price) {
        nextOptimum.price = shortRepPrice2;
        nextOptimum.posPrev = cur;
        nextOptimum.makeAsShortRep();
        nextIsChar = true;
      }
    }

    var numAvailableBytesFull = this._matchFinder.getNumAvailableBytes() + 1;
    numAvailableBytesFull = Math.min(kNumOpts - 1 - cur, numAvailableBytesFull);
    numAvailableBytes = numAvailableBytesFull;

    if (numAvailableBytes < 2) {
      continue;
    }
    if (numAvailableBytes > this._numFastBytes) {
      numAvailableBytes = this._numFastBytes;
    }
    if (!nextIsChar && matchByte !== currentByte) {
      // Try Literal + rep0
      var t = Math.min(numAvailableBytesFull - 1, this._numFastBytes);
      var lenTest2 = this._matchFinder.getMatchLen(0, this.reps[0], t);
      if (lenTest2 >= 2) {
        var state2 = Base.stateUpdateChar(state);

        var posStateNext = (position + 1) & this._posStateMask;
        var nextRepMatchPrice = curAnd1Price +
          RangeCoder.Encoder.getPrice1(this._isMatch[(state2 << Base.kNumPosStatesBitsMax) + posStateNext]) +
          RangeCoder.Encoder.getPrice1(this._isRep[state2]);

        var offset = cur + 1 + lenTest2;
        while (lenEnd < offset) {
          this._optimum[++lenEnd].price = kInfinityPrice;
        }
        var curAndLenPrice3 = nextRepMatchPrice + this.getRepPrice(0, lenTest2, state2, posStateNext);
        var optimum3 = this._optimum[offset];
        if (curAndLenPrice3 < optimum3.price) {
          optimum3.price = curAndLenPrice3;
          optimum3.posPrev = cur + 1;
          optimum3.backPrev = 0;
          optimum3.prev1IsChar = true;
          optimum3.prev2 = false;
        }
      }
    }

    var startLen = 2;	// Speed optimization

    var repIndex;
    for (repIndex = 0; repIndex < Base.kNumRepDistances; repIndex++) {
      var lenTest = this._matchFinder.getMatchLen(-1, this.reps[repIndex], numAvailableBytes);
      if (lenTest < 2) {
        continue;
      }
      var lenTestTemp = lenTest;
      do {
        while (lenEnd < cur + lenTest) {
          this._optimum[++lenEnd].price = kInfinityPrice;
        }
        var curAndLenPrice4 = repMatchPrice + this.getRepPrice(repIndex, lenTest, state, posState);
        var optimum4 = this._optimum[cur + lenTest];
        if (curAndLenPrice4 < optimum4.price) {
          optimum4.price = curAndLenPrice4;
          optimum4.posPrev = cur;
          optimum4.backPrev = repIndex;
          optimum4.prev1IsChar = false;
        }
      } while (--lenTest >= 2);
      lenTest = lenTestTemp;

      if (repIndex === 0) {
        startLen = lenTest + 1;
      }

      // if (_maxMode)
      if (lenTest < numAvailableBytesFull) {
        var t5 = Math.min(numAvailableBytesFull - 1 - lenTest, this._numFastBytes);
        var lenTest25 = this._matchFinder.getMatchLen(lenTest, this.reps[repIndex], t5);
        if (lenTest25 >= 2) {
          var state25 = Base.stateUpdateRep(state);
          var posStateNext5 = (position + lenTest) & this._posStateMask;
          var curAndLenCharPrice = repMatchPrice +
            this.getRepPrice(repIndex, lenTest, state, posState) +
            RangeCoder.Encoder.getPrice0(this._isMatch[(state25 << Base.kNumPosStatesBitsMax) + posStateNext5]) +
            this._literalEncoder.getSubCoder(position + lenTest,
                                             this._matchFinder.getIndexByte(lenTest - 2)).
            getPrice(true,
                     this._matchFinder.getIndexByte(lenTest - 1 - (this.reps[repIndex] + 1)),
                     this._matchFinder.getIndexByte(lenTest - 1));

          state25 = Base.stateUpdateChar(state25);
          posStateNext5 = (position + lenTest + 1) & this._posStateMask;
          var nextMatchPrice5 = curAndLenCharPrice +
            RangeCoder.Encoder.getPrice1(this._isMatch[(state25 << Base.kNumPosStatesBitsMax) + posStateNext5]);
          var nextRepMatchPrice5 = nextMatchPrice5 +
            RangeCoder.Encoder.getPrice1(this._isRep[state25]);

          // for(; lenTest2 >= 2; lenTest2--) {
          var offset5 = lenTest + 1 + lenTest25;
          while (lenEnd < cur + offset5) {
            this._optimum[++lenEnd].price = kInfinityPrice;
          }
          var curAndLenPrice5 = nextRepMatchPrice5 +
            this.getRepPrice(0, lenTest25, state25, posStateNext5);
          var optimum5 = this._optimum[cur + offset5];
          if (curAndLenPrice5 < optimum5.price) {
            optimum5.price = curAndLenPrice5;
            optimum5.posPrev = cur + lenTest + 1;
            optimum5.backPrev = 0;
            optimum5.prev1IsChar = true;
            optimum5.prev2 = true;
            optimum5.posPrev2 = cur;
            optimum5.backPrev2 = repIndex;
          }
        }
      }
    }

    if (newLen > numAvailableBytes) {
      newLen = numAvailableBytes;
      numDistancePairs = 0;
      while(newLen > this._matchDistances[numDistancePairs]) {
        numDistancePairs += 2;
      }
      this._matchDistances[numDistancePairs] = newLen;
      numDistancePairs += 2;
    }
    if (newLen >= startLen) {
      normalMatchPrice = matchPrice +
        RangeCoder.Encoder.getPrice0(this._isRep[state]);
      while (lenEnd < cur + newLen) {
        this._optimum[++lenEnd].price = kInfinityPrice;
      }

      var offs6 = 0;
      while (startLen > this._matchDistances[offs6]) {
        offs6 += 2;
      }

      var lenTest6;
      for (lenTest6 = startLen; ; lenTest6++) {
        var curBack = this._matchDistances[offs6 + 1];
        var curAndLenPrice6 = normalMatchPrice +
          this.getPosLenPrice(curBack, lenTest6, posState);
        var optimum6 = this._optimum[cur + lenTest6];
        if (curAndLenPrice6 < optimum6.price) {
          optimum6.price = curAndLenPrice6;
          optimum6.posPrev = cur;
          optimum6.backPrev = curBack + Base.kNumRepDistances;
          optimum6.prev1IsChar = false;
        }

        if (lenTest6 === this._matchDistances[offs6]) {
          if (lenTest6 < numAvailableBytesFull) {
            var t7 = Math.min(numAvailableBytesFull - 1 - lenTest6,
                              this._numFastBytes);
            var lenTest27 = this._matchFinder.getMatchLen(lenTest6, curBack, t7);
            if (lenTest27 >= 2) {
              var state27 = Base.stateUpdateMatch(state);

              var posStateNext7 = (position + lenTest6) & this._posStateMask;
              var curAndLenCharPrice7 = curAndLenPrice6 +
                RangeCoder.Encoder.getPrice0(this._isMatch[(state27 << Base.kNumPosStatesBitsMax) + posStateNext7]) +
                this._literalEncoder.getSubCoder(position + lenTest6,
                                                 this._matchFinder.getIndexByte(lenTest6 - 2)).
                getPrice(true,
                         this._matchFinder.getIndexByte(lenTest6 - (curBack + 1) -1),
                         this._matchFinder.getIndexByte(lenTest6 - 1));
              state27 = Base.stateUpdateChar(state27);
              posStateNext7 = (position + lenTest6 + 1) & this._posStateMask;
              var nextMatchPrice7 = curAndLenCharPrice7 +
                RangeCoder.Encoder.getPrice1(this._isMatch[(state27 << Base.kNumPosStatesBitsMax) + posStateNext7]);
              var nextRepMatchPrice7 = nextMatchPrice7 +
                RangeCoder.Encoder.getPrice1(this._isRep[state27]);

              var offset7 = lenTest6 + 1 + lenTest27;
              while (lenEnd < cur + offset7) {
                this._optimum[++lenEnd].price = kInfinityPrice;
              }
              var curAndLenPrice7 = nextRepMatchPrice7 +
                this.getRepPrice(0, lenTest27, state27, posStateNext7);
              var optimum7 = this._optimum[cur + offset7];
              if (curAndLenPrice7 < optimum7.price) {
                optimum7.price = curAndLenPrice7;
                optimum7.posPrev = cur + lenTest6 + 1;
                optimum7.backPrev = 0;
                optimum7.prev1IsChar = true;
                optimum7.prev2 = true;
                optimum7.posPrev2 = cur;
                optimum7.backPrev2 = curBack + Base.kNumRepDistances;
              }
            }
          }
          offs6 += 2;
          if (offs6 === numDistancePairs) {
            break;
          }
        }
      }
    }
  }
};

Encoder.prototype.changePair = function(smallDist, bigDist) {
  var kDif = 7;
  return (smallDist < (1 << (32 - kDif)) && bigDist >= (smallDist << kDif));
};

Encoder.prototype.writeEndMarker = function(posState) {
  if (!this._writeEndMark) {
    return;
  }

  this._rangeEncoder.encode(this._isMatch, (this._state << Base.kNumPosStatesBitsMax) + posState, 1);
  this._rangeEncoder.encode(this._isRep, this._state, 0);
  this._state = Base.stateUpdateMatch(this._state);
  var len = Base.kMatchMinLen;
  this._lenEncoder.encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
  var posSlot = (1 << Base.kNumPosSlotBits) - 1;
  var lenToPosState = Base.getLenToPosState(len);
  this._posSlotEncoder[lenToPosState].encode(this._rangeEncoder, posSlot);
  var footerBits = 30;
  var posReduced = (1 << footerBits) - 1;
  this._rangeEncoder.encodeDirectBits(posReduced >> Base.kNumAlignBits,
                                      footerBits - Base.kNumAlignBits);
  this._posAlignEncoder.reverseEncode(this._rangeEncoder,
                                      posReduced & Base.kAlignMask);
};

Encoder.prototype.flush = function(nowPos) {
  this.releaseMFStream();
  this.writeEndMarker(nowPos & this._posStateMask);
  this._rangeEncoder.flushData();
  this._rangeEncoder.flushStream();
};

Encoder.prototype.codeOneBlock = function(inSize, outSize, finished) {
  inSize[0] = 0;
  outSize[0] = 0;
  finished[0] = true;

  if (this._inStream) {
    this._matchFinder.setStream(this._inStream);
    this._matchFinder.init();
    this._needReleaseMFStream = true;
    this._inStream = null;
  }

  if (this._finished) {
    return;
  }
  this._finished = true;

  var progressPosValuePrev = this.nowPos64;
  var posState, curByte, i;

  if (this.nowPos64 === 0) {
    if (this._matchFinder.getNumAvailableBytes() === 0) {
      this.flush(this.nowPos64);
      return;
    }

    this.readMatchDistances();
    posState = this.nowPos64 & this._posStateMask;
    this._rangeEncoder.encode(this._isMatch, (this._state << Base.kNumPosStatesBitsMax) + posState, 0);
    this._state = Base.stateUpdateChar(this._state);
    curByte = this._matchFinder.getIndexByte(0 - this._additionalOffset);
    this._literalEncoder.getSubCoder(this.nowPos64, this._previousByte).
      encode(this._rangeEncoder, curByte);
    this._previousByte = curByte;
    this._additionalOffset--;
    this.nowPos64++;
  }
  if (this._matchFinder.getNumAvailableBytes() === 0) {
    this.flush(this.nowPos64);
    return;
  }
  while (true) {
    var len = this.getOptimum(this.nowPos64);
    var pos = this.backRes;
    posState = this.nowPos64 & this._posStateMask;
    var complexState = (this._state << Base.kNumPosStatesBitsMax) + posState;
    if (len === 1 && pos === -1) {
      this._rangeEncoder.encode(this._isMatch, complexState, 0);
      curByte = this._matchFinder.getIndexByte(- this._additionalOffset);
      var subCoder = this._literalEncoder.getSubCoder(this.nowPos64,
                                                      this._previousByte);
      if (!Base.stateIsCharState(this._state)) {
        var matchByte = this._matchFinder.getIndexByte(- this._repDistances[0] - 1 - this._additionalOffset);
        subCoder.encodeMatched(this._rangeEncoder, matchByte, curByte);
      } else {
        subCoder.encode(this._rangeEncoder, curByte);
      }
      this._previousByte = curByte;
      this._state = Base.stateUpdateChar(this._state);
    } else {
      this._rangeEncoder.encode(this._isMatch, complexState, 1);
      if (pos < Base.kNumRepDistances) {
        this._rangeEncoder.encode(this._isRep, this._state, 1);
        if (pos === 0) {
          this._rangeEncoder.encode(this._isRepG0, this._state, 0);
          if (len === 1) {
            this._rangeEncoder.encode(this._isRep0Long, complexState, 0);
          } else {
            this._rangeEncoder.encode(this._isRep0Long, complexState, 1);
          }
        } else {
          this._rangeEncoder.encode(this._isRepG0, this._state, 1);
          if (pos === 1) {
            this._rangeEncoder.encode(this._isRepG1, this._state, 0);
          } else {
            this._rangeEncoder.encode(this._isRepG1, this._state, 1);
            this._rangeEncoder.encode(this._isRepG2, this._state, pos - 2);
          }
        }
        if (len === 1) {
          this._state = Base.stateUpdateShortRep(this._state);
        } else {
          this._repMatchLenEncoder.encode(this._rangeEncoder,
                                          len - Base.kMatchMinLen, posState);
          this._state = Base.stateUpdateRep(this._state);
        }
        var distance = this._repDistances[pos];
        if (pos !== 0) {
          for (i = pos; i >= 1; i--) {
            this._repDistances[i] = this._repDistances[i - 1];
          }
          this._repDistances[0] = distance;
        }
      } else {
        this._rangeEncoder.encode(this._isRep, this._state, 0);
        this._state = Base.stateUpdateMatch(this._state);
        this._lenEncoder.encode(this._rangeEncoder, len - Base.kMatchMinLen,
                                posState);
        pos -= Base.kNumRepDistances;
        var posSlot = getPosSlot(pos);
        var lenToPosState = Base.getLenToPosState(len);
        this._posSlotEncoder[lenToPosState].encode(this._rangeEncoder, posSlot);

        if (posSlot >= Base.kStartPosModelIndex) {
          var footerBits = ((posSlot >>> 1) - 1);
          var baseVal = ((2 | (posSlot & 1)) << footerBits);
          var posReduced = pos - baseVal;

          if (posSlot < Base.kEndPosModelIndex) {
            RangeCoder.BitTreeEncoder.reverseEncode(this._posEncoders,
                                                    baseVal - posSlot - 1,
                                                    this._rangeEncoder,
                                                    footerBits, posReduced);
          } else {
            this._rangeEncoder.encodeDirectBits(posReduced >> Base.kNumAlignBits, footerBits - Base.kNumAlignBits);
            this._posAlignEncoder.reverseEncode(this._rangeEncoder,
                                                posReduced & Base.kAlignMask);
            this._alignPriceCount++;
          }
        }
        var distance2 = pos;
        for (i = Base.kNumRepDistances - 1; i >= 1; i--) {
          this._repDistances[i] = this._repDistances[i - 1];
        }
        this._repDistances[0] = distance2;
        this._matchPriceCount++;
      }
      this._previousByte =
        this._matchFinder.getIndexByte(len - 1 - this._additionalOffset);
    }
    this._additionalOffset -= len;
    this.nowPos64 += len;
    if (this._additionalOffset === 0) {
      // if (!_fastMode)
      if (this._matchPriceCount >= (1 << 7)) {
        this.fillDistancesPrices();
      }
      if (this._alignPriceCount >= Base.kAlignTableSize) {
        this.fillAlignPrices();
      }
      inSize[0] = this.nowPos64;
      outSize[0] = this._rangeEncoder.getProcessedSizeAdd();

      if (this._matchFinder.getNumAvailableBytes() === 0) {
        this.flush(this.nowPos64);
        return;
      }

      if (this.nowPos64 - progressPosValuePrev >= (1 << 12)) {
        this._finished = false;
        finished[0] = false;
        return;
      }
    }
  }
};

Encoder.prototype.releaseMFStream = function() {
  if (this._matchFinder && this._needReleaseMFStream) {
    this._matchFinder.releaseStream();
    this._needReleaseMFStream = false;
  }
};

Encoder.prototype.setOutStream = function(outStream) {
  this._rangeEncoder.setStream(outStream);
};
Encoder.prototype.releaseOutStream = function() {
  this._rangeEncoder.releaseStream();
};

Encoder.prototype.releaseStreams = function() {
  this.releaseMFStream();
  this.releaseOutStream();
};

Encoder.prototype.setStreams = function(inStream, outStream, inSize, outSize) {
  this._inStream = inStream;
  this._finished = false;
  this.create();
  this.setOutStream(outStream);
  this.init();

  // if (!_fastMode)
  if (true) {
    this.fillDistancesPrices();
    this.fillAlignPrices();
  }

  this._lenEncoder.setTableSize(this._numFastBytes + 1 - Base.kMatchMinLen);
  this._lenEncoder.updateTables(1 << this._posStateBits);
  this._repMatchLenEncoder.setTableSize(this._numFastBytes +
                                        1 - Base.kMatchMinLen);
  this._repMatchLenEncoder.updateTables(1 << this._posStateBits);

  this.nowPos64 = 0;
};

Encoder.prototype.code = function(inStream, outStream, inSize, outSize, progress) {
  this._needReleaseMFStream = false;
  try {
    this.setStreams(inStream, outStream, inSize, outSize);
    while (true) {

      this.codeOneBlock(this.processedInSize, this.processedOutSize,
                        this.finished);
      if (this.finished[0]) {
        return;
      }
      if (progress) {
        progress.setProgress(this.processedInSize[0], this.processedOutSize[0]);
      }
    }
  } finally {
    this.releaseStreams();
  }
};

Encoder.prototype.writeCoderProperties = function(outStream) {
  var properties = makeBuffer(kPropSize), i;
  properties[0] = ((this._posStateBits * 5 + this._numLiteralPosStateBits) * 9+
                   this._numLiteralContextBits);
  for (i = 0; i < 4; i++) {
    properties[1 + i] = (this._dictionarySize >>> (8 * i));
  }
  for (i = 0; i< kPropSize; i++) {
    outStream.writeByte(properties[i]);
  }
};

Encoder.prototype.fillDistancesPrices = function() {
  var tempPrices = [];
  tempPrices.length = Base.kNumFullDistances;
  var i, posSlot;
  for (i = Base.kStartPosModelIndex; i < Base.kNumFullDistances; i++) {
    posSlot = getPosSlot(i);
    var footerBits = ((posSlot >>> 1) - 1);
    var baseVal = ((2 | (posSlot & 1)) << footerBits);
    tempPrices[i] =
      RangeCoder.BitTreeEncoder.reverseGetPrice(this._posEncoders,
                                                baseVal - posSlot - 1,
                                                footerBits, i - baseVal);
  }

  var lenToPosState = 0;
  for ( ; lenToPosState < Base.kNumLenToPosStates; lenToPosState++) {
    var encoder = this._posSlotEncoder[lenToPosState];

    var st = (lenToPosState << Base.kNumPosSlotBits);
    for (posSlot = 0; posSlot < this._distTableSize; posSlot++) {
      this._posSlotPrices[st + posSlot] = encoder.getPrice(posSlot);
    }
    for (posSlot = Base.kEndPosModelIndex; posSlot < this._distTableSize; posSlot++) {
      this._posSlotPrices[st + posSlot] += ((((posSlot >>> 1) - 1) - Base.kNumAlignBits) << RangeCoder.Encoder.kNumBitPriceShiftBits);
    }

    var st2 = lenToPosState * Base.kNumFullDistances;
    for (i = 0; i < Base.kStartPosModelIndex; i++) {
      this._distancesPrices[st2 + i] = this._posSlotPrices[st + i];
    }
    for (; i < Base.kNumFullDistances; i++) {
      this._distancesPrices[st2 + i] =
        this._posSlotPrices[st + getPosSlot(i)] + tempPrices[i];
    }
  }
  this._matchPriceCount = 0;
};

Encoder.prototype.fillAlignPrices = function() {
  var i;
  for (i = 0; i < Base.kAlignTableSize; i++) {
    this._alignPrices[i] = this._posAlignEncoder.reverseGetPrice(i);
  }
  this._alignPriceCount = 0;
};

Encoder.prototype.setAlgorithm = function(algorithm) {
  /*
    _fastMode = (algorithm == 0);
    _maxMode = (algorithm >= 2);
  */
  return true;
};

Encoder.prototype.setDictionarySize = function(dictionarySize) {
  var kDicLogSizeMaxCompress = 29;
  if (dictionarySize < (1 << Base.kDicLogSizeMin) ||
      dictionarySize > (1 << kDicLogSizeMaxCompress)) {
    return false;
  }
  this._dictionarySize = dictionarySize;
  var dicLogSize = 0;
  while (dictionarySize > (1 << dicLogSize)) {
    dicLogSize++;
  }
  this._distTableSize = dicLogSize * 2;
  return true;
};

Encoder.prototype.setNumFastBytes = function(numFastBytes) {
  if (numFastBytes < 5 || numFastBytes > Base.kMatchMaxLen) {
    return false;
  }
  this._numFastBytes = numFastBytes;
  return true;
};

Encoder.prototype.setMatchFinder = function(matchFinderIndex) {
  if (matchFinderIndex < 0 || matchFinderIndex > 2) {
    return false;
  }
  var matchFinderIndexPrev = this._matchFinderType;
  this._matchFinderType = matchFinderIndex;
  if (this._matchFinder && matchFinderIndexPrev != this._matchFinderType) {
    this._dictionarySizePrev = -1;
    this._matchFinder = null;
  }
  return true;
};

Encoder.prototype.setLcLpPb = function(lc, lp, pb) {
  if (lp < 0 || lp > Base.kNumLitPosStatesBitsEncodingMax ||
      lc < 0 || lc > Base.kNumLitContextBitsMax ||
      pb < 0 || pb > Base.kNumPosStatesBitsEncodingMax) {
    return false;
  }
  this._numLiteralPosStateBits = lp;
  this._numLiteralContextBits = lc;
  this._posStateBits = pb;
  this._posStateMask = ((1) << this._posStateBits) - 1;
  return true;
};

Encoder.prototype.setEndMarkerMode = function(endMarkerMode) {
  this._writeEndMark = endMarkerMode;
};

Encoder.EMatchFinderTypeBT2 = EMatchFinderTypeBT2;
Encoder.EMatchFinderTypeBT4 = EMatchFinderTypeBT4;

LZMA.Encoder = Encoder;
