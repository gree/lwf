(function() {

var InWindow = function(keepSizeBefore, keepSizeAfter, keepSizeReserve, stream){
  if (arguments.length >= 4) {
    // typical initialization sequence
    var args = Array.prototype.slice.call(arguments, 0);
    var _stream = args.pop();
    this.create.apply(this, args);
    this.setStream(_stream);
    this.init();
  }
};

InWindow.prototype.moveBlock = function() {
  var i;
  var offset = this._bufferOffset + this._pos + this._keepSizeBefore;
  // we need one additional byte, since MovePos moves on 1 byte.
  if (offset > 0) {
    offset--;
  }

  var numBytes = this._bufferOffset + this._streamPos - offset;
  for (i = 0; i < numBytes; i++) {
    this._bufferBase[i] = this._bufferBase[offset + i];
  }
  this._bufferOffset -= offset;
};

InWindow.prototype.readBlock = function() {
  if (this._streamEndWasReached) {
    return;
  }
  while (true) {
    var size = -this._bufferOffset + this._blockSize - this._streamPos;
    if (size === 0) {
      return;
    }
    var numReadBytes =
      this._stream.read(this._bufferBase,
                        this._bufferOffset + this._streamPos,
                        size);
    if (numReadBytes <= 0) {
      this._posLimit = this._streamPos;
      var pointerToPosition = this._bufferOffset + this._posLimit;
      if (pointerToPosition > this._pointerToLastSafePosition) {
        this._posLimit = this._pointerToLastSafePosition - this._bufferOffset;
      }

      this._streamEndWasReached = true;
      return;
    }
    this._streamPos += numReadBytes;
    if (this._streamPos >= this._pos + this._keepSizeAfter) {
      this._posLimit = this._streamPos - this._keepSizeAfter;
    }
  }
};

InWindow.prototype.free = function() {
  this._bufferBase = null;
};

InWindow.prototype.create = function(keepSizeBefore, keepSizeAfter, keepSizeReserve) {
  this._keepSizeBefore = keepSizeBefore;
  this._keepSizeAfter = keepSizeAfter;
  var blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserve;
  if ((!this._bufferBase) || this._blockSize !== blockSize) {
    this.free();
    this._blockSize = blockSize;
    this._bufferBase = makeBuffer(this._blockSize);
  }
  this._pointerToLastSafePosition = this._blockSize - keepSizeAfter;
};

InWindow.prototype.setStream = function(stream) {
  this._stream = stream;
};

InWindow.prototype.releaseStream = function() {
  this._stream = null;
};

InWindow.prototype.init = function() {
  this._bufferOffset = 0;
  this._pos = 0;
  this._streamPos = 0;
  this._streamEndWasReached = false;
  this.readBlock();
};

InWindow.prototype.movePos = function() {
  this._pos++;
  if (this._pos > this._posLimit) {
    var pointerToPosition = this._bufferOffset + this._pos;
    if (pointerToPosition > this._pointerToLastSafePosition) {
      this.moveBlock();
    }
    this.readBlock();
  }
};

InWindow.prototype.getIndexByte = function(index) {
  return this._bufferBase[this._bufferOffset + this._pos + index];
};

// index + limit have not to exceed _keepSizeAfter
InWindow.prototype.getMatchLen = function(index, distance, limit) {
  var pby, i;
  if (this._streamEndWasReached) {
    if (this._pos + index + limit > this._streamPos) {
      limit = this._streamPos - (this._pos + index);
    }
  }
  distance++;
  pby = this._bufferOffset + this._pos + index;
  for (i=0; i < limit && this._bufferBase[pby + i] === this._bufferBase[pby + i - distance]; ) {
    i++;
  }
  return i;
};

InWindow.prototype.getNumAvailableBytes = function() {
  return this._streamPos - this._pos;
};

InWindow.prototype.reduceOffsets = function(subValue) {
  this._bufferOffset += subValue;
  this._posLimit -= subValue;
  this._pos -= subValue;
  this._streamPos -= subValue;
};

LZ.InWindow = InWindow;
