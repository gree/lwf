var OutWindow = function(){
  this._windowSize = 0;
};

OutWindow.prototype.create = function(windowSize){
  if ( (!this._buffer) || (this._windowSize !== windowSize) ){
    this._buffer = makeBuffer(windowSize);
  }
  this._windowSize = windowSize;
  this._pos = 0;
  this._streamPos = 0;
};

OutWindow.prototype.flush = function(){
  var size = this._pos - this._streamPos;
  if (size !== 0){
    while(size --){
      this._stream.writeByte(this._buffer[this._streamPos ++]);
    }
    if (this._pos >= this._windowSize){
      this._pos = 0;
    }
    this._streamPos = this._pos;
  }
};

OutWindow.prototype.releaseStream = function(){
  this.flush();
  this._stream = null;
};

OutWindow.prototype.setStream = function(stream){
  this.releaseStream();
  this._stream = stream;
};

OutWindow.prototype.init = function(solid){
  if (!solid){
    this._streamPos = 0;
    this._pos = 0;
  }
};

OutWindow.prototype.copyBlock = function(distance, len){
  var pos = this._pos - distance - 1;
  if (pos < 0){
    pos += this._windowSize;
  }
  while(len --){
    if (pos >= this._windowSize){
      pos = 0;
    }
    this._buffer[this._pos ++] = this._buffer[pos ++];
    if (this._pos >= this._windowSize){
      this.flush();
    }
  }
};

OutWindow.prototype.putByte = function(b){
  this._buffer[this._pos ++] = b;
  if (this._pos >= this._windowSize){
    this.flush();
  }
};

OutWindow.prototype.getByte = function(distance){
  var pos = this._pos - distance - 1;
  if (pos < 0){
    pos += this._windowSize;
  }
  return this._buffer[pos];
};

LZ.OutWindow = OutWindow;
