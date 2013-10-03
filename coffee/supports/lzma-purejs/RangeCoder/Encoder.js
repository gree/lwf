(function() {
// largest 32/24/16/8-bit numbers.
var MAX32 = 0xFFFFFFFF;
var MAX24 = 0x00FFFFFF;
var MAX16 = 0x0000FFFF;
var MAX8  = 0x000000FF;
var MASK24= 0xFF000000; // 32-bit inverse of MAX24

var kNumBitModelTotalBits = 11;
var kBitModelTotal = (1 << kNumBitModelTotalBits);
var kNumMoveBits = 5;

var kNumMoveReducingBits = 2;
var kNumBitPriceShiftBits = 6;

var Encoder = function(stream){
  this.init();
  if (stream) { this.setStream(stream); }
};
Encoder.prototype.setStream = function(stream) {
  this._stream = stream;
};
Encoder.prototype.releaseStream = function() {
  this._stream = null;
};
Encoder.prototype.init = function() {
  // The cache/cache_size variables are used to properly handle
  // carries, and represent a number defined by a big-endian sequence
  // starting with the cache value, and followed by cache_size 0xff
  // bytes, which has been shifted out of the low register, but hasn't
  // been written yet, because it could be incremented by one due to a
  // carry.

  // Note that the first byte output will always be 0 due to the fact
  // that cache and low are initialized to 0, and the encoder
  // implementation; the decoder ignores this byte.
  this._position = 0;
  this.low = 0; // unsigned 33-bit integer
  this.range = MAX32; // unsigned 32-bit integer
  // cacheSize needs to be large enough to store the full uncompressed
  // size; javascript's 2^53 limit should be enough
  this._cacheSize = 1;
  this._cache = 0; // unsigned 8-bit
};
Encoder.prototype.flushData = function() {
  var i;
  for (i=0; i<5; i++) {
    this.shiftLow();
  }
};
Encoder.prototype.flushStream = function() {
  if (this._stream.flush) {
    this._stream.flush();
  }
};
Encoder.prototype.shiftLow = function() {
  // "normalization"
  var overflow = (this.low > MAX32) ? 1 : 0;
  if (this.low < MASK24 || overflow) {
    this._position += this._cacheSize;
    var temp = this._cache;
    do {
      this._stream.writeByte((temp + overflow) & MAX8);
      temp = MAX8;
    } while (--this._cacheSize !== 0);
    // set cache to bits 24-31 of 'low'
    this._cache = this.low >>> 24; // this truncates correctly
    // set cache_size to 0 (do/while loop did this)
  }
  this._cacheSize++;
  // 'lowest 24 bits of low, shifted left by 8'
  // careful, '<< 8' can flip sign of 'low'
  this.low = (this.low & MAX24) * 256;
};

Encoder.prototype.encodeDirectBits = function(v, numTotalBits) {
  var i, mask;
  mask = 1 << (numTotalBits-1);
  for (i = numTotalBits - 1; i >= 0; i--, mask>>>=1) {
    this.range >>>= 1; // range is unsigned 32-bit int
    if (v & mask) {
      this.low += this.range;
    }
    if (this.range <= MAX24) {
      this.range *= 256; // careful not to flip sign
      this.shiftLow();
    }
  }
};

Encoder.prototype.getProcessedSizeAdd = function() {
  return this._cacheSize + this._position + 4;
};

Encoder.initBitModels = function(probs, len) {
  var i;
  if (len && !probs) {
    if (typeof(Uint16Array)!=='undefined') {
      probs = new Uint16Array(len);
    } else {
      probs = [];
      probs.length = len;
    }
  }
  for (i=0; i < probs.length; i++)
    probs[i] = (kBitModelTotal >>> 1); // 0.5 probability
  return probs;
};

Encoder.prototype.encode = function(probs, index, symbol) {
  var prob = probs[index];
  var newBound = (this.range >>> kNumBitModelTotalBits) * prob;
  if (symbol === 0) {
    this.range = newBound;
    probs[index] = prob + ((kBitModelTotal - prob) >>> kNumMoveBits);
  } else {
    this.low += newBound;
    this.range -= newBound;
    probs[index] = prob - (prob >>> kNumMoveBits);
  }
  if (this.range <= MAX24) {
    this.range *= 256; // careful not to flip sign
    this.shiftLow();
  }
};

var ProbPrices = [];
if (typeof(Uint32Array)!=='undefined') {
  ProbPrices = new Uint32Array(kBitModelTotal >>> kNumMoveReducingBits);
}
(function() {
  var kNumBits = (kNumBitModelTotalBits - kNumMoveReducingBits);
  var i, j;
  for (i = kNumBits - 1; i >= 0; i--) {
    var start = 1 << (kNumBits - i - 1);
    var end = 1 << (kNumBits - i);
    for (j = start; j < end; j++) {
      ProbPrices[j] = (i << kNumBitPriceShiftBits) +
        (((end - j) << kNumBitPriceShiftBits) >>> (kNumBits - i - 1));
    }
  }
})();

Encoder.getPrice = function(prob, symbol) {
  return ProbPrices[(((prob - symbol) ^ ((-symbol))) & (kBitModelTotal - 1)) >>> kNumMoveReducingBits];
};
Encoder.getPrice0 = function(prob) {
  return ProbPrices[prob >>> kNumMoveReducingBits];
};
Encoder.getPrice1 = function(prob) {
  return ProbPrices[(kBitModelTotal - prob) >>> kNumMoveReducingBits];
};

// export constants for use in Encoder.
Encoder.kNumBitPriceShiftBits = kNumBitPriceShiftBits;

RangeCoder.Encoder = Encoder;
