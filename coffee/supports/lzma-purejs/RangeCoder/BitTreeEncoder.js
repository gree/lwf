var BitTreeEncoder = function(numBitLevels) {
  this._numBitLevels = numBitLevels;
  this.init();
};
BitTreeEncoder.prototype.init = function() {
  this._models = Encoder.initBitModels(null, 1 << this._numBitLevels);
};

BitTreeEncoder.prototype.encode = function(rangeEncoder, symbol) {
  var m = 1, bitIndex;
  for (bitIndex = this._numBitLevels; bitIndex > 0; ) {
    bitIndex--;
    var bit = (symbol >>> bitIndex) & 1;
    rangeEncoder.encode(this._models, m, bit);
    m = (m << 1) | bit;
  }
};

BitTreeEncoder.prototype.reverseEncode = function(rangeEncoder, symbol) {
  var m = 1, i;
  for (i = 0; i < this._numBitLevels; i++) {
    var bit = symbol & 1;
    rangeEncoder.encode(this._models, m, bit);
    m = (m << 1) | bit;
    symbol >>>= 1;
  }
};

BitTreeEncoder.reverseEncode = function(models, startIndex, rangeEncoder, numBitLevels, symbol) {
  var m = 1, i;
  for (i = 0; i < numBitLevels; i++) {
    var bit = symbol & 1;
    rangeEncoder.encode(models, startIndex + m, bit);
    m = (m << 1) | bit;
    symbol >>>= 1;
  }
};

BitTreeEncoder.prototype.getPrice = function(symbol) {
  var price = 0, m = 1, bitIndex;
  for (bitIndex = this._numBitLevels; bitIndex > 0; ) {
    bitIndex--;
    var bit = (symbol >>> bitIndex) & 1;
    price += Encoder.getPrice(this._models[m], bit);
    m = (m << 1) | bit;
  }
  return price;
};

BitTreeEncoder.prototype.reverseGetPrice = function(symbol) {
  var price = 0, m = 1, bitIndex;
  for (bitIndex = this._numBitLevels; bitIndex > 0; bitIndex--) {
    var bit = (symbol & 1);
    symbol >>>= 1;
    price += Encoder.getPrice(this._models[m], bit);
    m = (m << 1) | bit;
  }
  return price;
};

BitTreeEncoder.reverseGetPrice = function(models, startIndex,
                                          numBitLevels, symbol) {
  var price = 0, m = 1, bitIndex;
  for (bitIndex = numBitLevels; bitIndex > 0; bitIndex--) {
    var bit = (symbol & 1);
    symbol >>>= 1;
    price += Encoder.getPrice(models[startIndex + m], bit);
    m = (m << 1) | bit;
  }
  return price;
};

RangeCoder.BitTreeEncoder = BitTreeEncoder;
