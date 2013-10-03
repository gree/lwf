var BitTreeDecoder = function(numBitLevels){
  this._numBitLevels = numBitLevels;
  this.init();
};

BitTreeDecoder.prototype.init = function(){
  this._models = Encoder.initBitModels(null, 1 << this._numBitLevels);
};

BitTreeDecoder.prototype.decode = function(rangeDecoder){
  var m = 1, i = this._numBitLevels;

  while(i --){
    m = (m << 1) | rangeDecoder.decodeBit(this._models, m);
  }
  return m - (1 << this._numBitLevels);
};

BitTreeDecoder.prototype.reverseDecode = function(rangeDecoder){
  var m = 1, symbol = 0, i = 0, bit;

  for (; i < this._numBitLevels; ++ i){
    bit = rangeDecoder.decodeBit(this._models, m);
    m = (m << 1) | bit;
    symbol |= (bit << i);
  }
  return symbol;
};

BitTreeDecoder.reverseDecode = function(models, startIndex, rangeDecoder,
                                        numBitLevels) {
  var m = 1, symbol = 0, i = 0, bit;

  for (; i < numBitLevels; ++ i){
    bit = rangeDecoder.decodeBit(models, startIndex + m);
    m = (m << 1) | bit;
    symbol |= (bit << i);
  }
  return symbol;
};

RangeCoder.BitTreeDecoder = BitTreeDecoder;
}).call(this);
