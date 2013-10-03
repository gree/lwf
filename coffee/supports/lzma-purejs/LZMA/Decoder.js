/*
Copyright (c) 2011 Juan Mellado

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

/*
References:
- "LZMA SDK" by Igor Pavlov
  http://www.7-zip.org/sdk.html
*/
/* Original source found at: http://code.google.com/p/js-lzma/ */

// shortcuts
var initBitModels = RangeCoder.Encoder.initBitModels;

var LenDecoder = function(){
  this._choice = initBitModels(null, 2);
  this._lowCoder = [];
  this._midCoder = [];
  this._highCoder = new RangeCoder.BitTreeDecoder(8);
  this._numPosStates = 0;
};

LenDecoder.prototype.create = function(numPosStates){
  for (; this._numPosStates < numPosStates; ++ this._numPosStates){
    this._lowCoder[this._numPosStates] = new RangeCoder.BitTreeDecoder(3);
    this._midCoder[this._numPosStates] = new RangeCoder.BitTreeDecoder(3);
  }
};

LenDecoder.prototype.init = function(){
  var i = this._numPosStates;
  initBitModels(this._choice);
  while(i --){
    this._lowCoder[i].init();
    this._midCoder[i].init();
  }
  this._highCoder.init();
};

LenDecoder.prototype.decode = function(rangeDecoder, posState){
  if (rangeDecoder.decodeBit(this._choice, 0) === 0){
    return this._lowCoder[posState].decode(rangeDecoder);
  }
  if (rangeDecoder.decodeBit(this._choice, 1) === 0){
    return 8 + this._midCoder[posState].decode(rangeDecoder);
  }
  return 16 + this._highCoder.decode(rangeDecoder);
};

var LiteralDecoder = function(){
};

LiteralDecoder.Decoder2 = function(){
  this._decoders = initBitModels(null, 0x300);
};

LiteralDecoder.Decoder2.prototype.init = function(){
  initBitModels(this._decoders);
};

LiteralDecoder.Decoder2.prototype.decodeNormal = function(rangeDecoder){
  var symbol = 1;

  do{
    symbol = (symbol << 1) | rangeDecoder.decodeBit(this._decoders, symbol);
  }while(symbol < 0x100);

  return symbol & 0xff;
};

LiteralDecoder.Decoder2.prototype.decodeWithMatchByte = function(rangeDecoder, matchByte){
  var symbol = 1, matchBit, bit;

  do{
    matchBit = (matchByte >> 7) & 1;
    matchByte <<= 1;
    bit = rangeDecoder.decodeBit(this._decoders, ( (1 + matchBit) << 8) + symbol);
    symbol = (symbol << 1) | bit;
    if (matchBit !== bit){
      while(symbol < 0x100){
        symbol = (symbol << 1) | rangeDecoder.decodeBit(this._decoders, symbol);
      }
      break;
    }
  }while(symbol < 0x100);

  return symbol & 0xff;
};

LiteralDecoder.prototype.create = function(numPosBits, numPrevBits){
  var i;

  if (this._coders &&
      (this._numPrevBits === numPrevBits) &&
      (this._numPosBits === numPosBits) ){
    return;
  }
  this._numPosBits = numPosBits;
  this._posMask = (1 << numPosBits) - 1;
  this._numPrevBits = numPrevBits;

  this._coders = [];

  i = 1 << (this._numPrevBits + this._numPosBits);
  while(i --){
    this._coders[i] = new LiteralDecoder.Decoder2();
  }
};

LiteralDecoder.prototype.init = function(){
  var i = 1 << (this._numPrevBits + this._numPosBits);
  while(i --){
    this._coders[i].init();
  }
};

LiteralDecoder.prototype.getDecoder = function(pos, prevByte){
  return this._coders[( (pos & this._posMask) << this._numPrevBits) +
                      ( (prevByte & 0xff) >>> (8 - this._numPrevBits) )];
};

var Decoder = function(){
  var i;
  this._outWindow = new LZ.OutWindow();
  this._rangeDecoder = new RangeCoder.Decoder();

  this._isMatchDecoders =
    initBitModels(null, Base.kNumStates << Base.kNumPosStatesBitsMax);
  this._isRepDecoders = initBitModels(null, Base.kNumStates);
  this._isRepG0Decoders = initBitModels(null, Base.kNumStates);
  this._isRepG1Decoders = initBitModels(null, Base.kNumStates);
  this._isRepG2Decoders = initBitModels(null, Base.kNumStates);
  this._isRep0LongDecoders =
    initBitModels(null, Base.kNumStates << Base.kNumPosStatesBitsMax);
  this._posSlotDecoder = [];
  this._posDecoders = initBitModels(null, Base.kNumFullDistances - Base.kEndPosModelIndex);
  this._posAlignDecoder = new RangeCoder.BitTreeDecoder(Base.kNumAlignBits);
  this._lenDecoder = new LenDecoder();
  this._repLenDecoder = new LenDecoder();
  this._literalDecoder = new LiteralDecoder();
  this._dictionarySize = -1;
  this._dictionarySizeCheck = -1;
  this._posStateMask = 0;

  for (i=0; i<Base.kNumLenToPosStates; i++) {
    this._posSlotDecoder[i] = new RangeCoder.BitTreeDecoder(Base.kNumPosSlotBits);
  }
};

Decoder.prototype.setDictionarySize = function(dictionarySize){
  if (dictionarySize < 0){
    return false;
  }
  if (this._dictionarySize !== dictionarySize){
    this._dictionarySize = dictionarySize;
    this._dictionarySizeCheck = Math.max(this._dictionarySize, 1);
    this._outWindow.create( Math.max(this._dictionarySizeCheck, (1 << 12)) );
  }
  return true;
};

Decoder.prototype.setLcLpPb = function(lc, lp, pb){
  var numPosStates = 1 << pb;

  if (lc > Base.kNumLitContextBitsMax || lp > 4 || pb > Base.kNumPosStatesBitsMax){
    return false;
  }

  this._literalDecoder.create(lp, lc);

  this._lenDecoder.create(numPosStates);
  this._repLenDecoder.create(numPosStates);
  this._posStateMask = numPosStates - 1;

  return true;
};

Decoder.prototype.init = function(){
  var i = Base.kNumLenToPosStates;

  this._outWindow.init(false);

  initBitModels(this._isMatchDecoders);
  initBitModels(this._isRepDecoders);
  initBitModels(this._isRepG0Decoders);
  initBitModels(this._isRepG1Decoders);
  initBitModels(this._isRepG2Decoders);
  initBitModels(this._isRep0LongDecoders);
  initBitModels(this._posDecoders);

  this._literalDecoder.init();

  while(i --){
    this._posSlotDecoder[i].init();
  }

  this._lenDecoder.init();
  this._repLenDecoder.init();
  this._posAlignDecoder.init();
  this._rangeDecoder.init();
};

Decoder.prototype.code = function(inStream, outStream, outSize){
  // note that nowPos64 is actually only 53 bits long; that sets a limit
  // on the amount of data we can decode
  var state, rep0 = 0, rep1 = 0, rep2 = 0, rep3 = 0, nowPos64 = 0, prevByte = 0,
      posState, decoder2, len, distance, posSlot, numDirectBits;

  this._rangeDecoder.setStream(inStream);
  this._outWindow.setStream(outStream);

  this.init();

  state = Base.stateInit();
  while(outSize < 0 || nowPos64 < outSize){
    posState = nowPos64 & this._posStateMask;

    if (this._rangeDecoder.decodeBit(this._isMatchDecoders, (state << Base.kNumPosStatesBitsMax) + posState) === 0){
      decoder2 = this._literalDecoder.getDecoder(nowPos64, prevByte);

      if (!Base.stateIsCharState(state)){
        prevByte = decoder2.decodeWithMatchByte(this._rangeDecoder, this._outWindow.getByte(rep0) );
      }else{
        prevByte = decoder2.decodeNormal(this._rangeDecoder);
      }
      this._outWindow.putByte(prevByte);
      state = Base.stateUpdateChar(state);
      nowPos64++;

    }else{

      if (this._rangeDecoder.decodeBit(this._isRepDecoders, state) === 1){
        len = 0;
        if (this._rangeDecoder.decodeBit(this._isRepG0Decoders, state) === 0){
          if (this._rangeDecoder.decodeBit(this._isRep0LongDecoders, (state << Base.kNumPosStatesBitsMax) + posState) === 0){
            state = Base.stateUpdateShortRep(state);
            len = 1;
          }
        }else{
          if (this._rangeDecoder.decodeBit(this._isRepG1Decoders, state) === 0){
            distance = rep1;
          }else{
            if (this._rangeDecoder.decodeBit(this._isRepG2Decoders, state) === 0){
              distance = rep2;
            }else{
              distance = rep3;
              rep3 = rep2;
            }
            rep2 = rep1;
          }
          rep1 = rep0;
          rep0 = distance;
        }
        if (len === 0){
          len = Base.kMatchMinLen + this._repLenDecoder.decode(this._rangeDecoder, posState);
          state = Base.stateUpdateRep(state);
        }
      }else{
        rep3 = rep2;
        rep2 = rep1;
        rep1 = rep0;

        len = Base.kMatchMinLen + this._lenDecoder.decode(this._rangeDecoder, posState);
        state = Base.stateUpdateMatch(state);

        posSlot = this._posSlotDecoder[Base.getLenToPosState(len)].decode(this._rangeDecoder);
        if (posSlot >= Base.kStartPosModelIndex){

          numDirectBits = (posSlot >> 1) - 1;
          rep0 = (2 | (posSlot & 1) ) << numDirectBits;

          if (posSlot < Base.kEndPosModelIndex){
            rep0 += RangeCoder.BitTreeDecoder.reverseDecode(this._posDecoders,
                rep0 - posSlot - 1, this._rangeDecoder, numDirectBits);
          }else{
            rep0 += this._rangeDecoder.decodeDirectBits(numDirectBits - Base.kNumAlignBits) << Base.kNumAlignBits;
            rep0 += this._posAlignDecoder.reverseDecode(this._rangeDecoder);
            if (rep0 < 0){
              if (rep0 === -1){
                break;
              }
              return false;
            }
          }
        }else{
          rep0 = posSlot;
        }
      }

      if (rep0 >= nowPos64 || rep0 >= this._dictionarySizeCheck){
        return false;
      }

      this._outWindow.copyBlock(rep0, len);
      nowPos64 += len;
      prevByte = this._outWindow.getByte(0);
    }
  }

  this._outWindow.flush();
  this._outWindow.releaseStream();
  this._rangeDecoder.releaseStream();

  return true;
};

Decoder.prototype.setDecoderProperties = function(properties){
  var value, lc, lp, pb, dictionarySize, i, shift;

  if (properties.length < 5){
    return false;
  }

  value = properties[0] & 0xFF;
  lc = value % 9;
  value = ~~(value / 9);
  lp = value % 5;
  pb = ~~(value / 5);

  if ( !this.setLcLpPb(lc, lp, pb) ){
    return false;
  }

  dictionarySize = 0;
  for (i=0, shift=1; i<4; i++, shift*=256)
    dictionarySize += (properties[1+i] & 0xFF) * shift;

  return this.setDictionarySize(dictionarySize);
};
Decoder.prototype.setDecoderPropertiesFromStream = function(stream) {
  var buffer = [], i;
  for (i=0; i<5; i++) {
    buffer[i] = stream.readByte();
  }
  return this.setDecoderProperties(buffer);
};

LZMA.Decoder = Decoder;
}).call(this);
