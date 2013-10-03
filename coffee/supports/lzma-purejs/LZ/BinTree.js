var CrcTable = (function() {
  var table = [];
  if (typeof(Uint32Array)!=='undefined') {
    table = new Uint32Array(256);
  }

  var kPoly = 0xEDB88320, i, j, r;
  for (i = 0; i < 256; i++) {
    r = i;
    for (j = 0; j < 8; j++) {
      if ((r & 1) !== 0) {
        r = (r >>> 1) ^ kPoly;
      } else {
        r >>>= 1;
      }
    }
    table[i] = r;
  }

  return table;
})();
//console.assert(CrcTable.length === 256);

// constants
var kHash2Size = 1 << 10, kHash3Size = 1 << 16, kBT2HashSize = 1 << 16;
var kStartMaxLen = 1, kHash3Offset = kHash2Size, kEmptyHashValue = 0;
var kMaxValForNormalize = (1 << 30) - 1;

function BinTree() {
  InWindow.call(this);
  this._cyclicBufferSize = 0;

  this._son = [];
  this._hash = [];

  this._cutValue = 0xFF;
  this._hashSizeSum = 0;

  this.HASH_ARRAY = true;

  this.kNumHashDirectBytes = 0;
  this.kMinMatchCheck = 4;
  this.kFixHashSize = kHash2Size + kHash3Size;

  if (arguments.length >= 6) {
    var args = Array.prototype.slice.call(arguments, 0);
    this.setType(args.shift());
    var stream = args.pop();
    this.create.apply(this, args);
    this.setStream(stream);
    this.init();
  }
}
// a little bit of sugar for super-method invocations.
var _super_ = InWindow.prototype;
BinTree.prototype = Object.create(_super_);

BinTree.prototype.setType = function(numHashBytes) {
  this.HASH_ARRAY = numHashBytes > 2;
  if (this.HASH_ARRAY) {
    this.kNumHashDirectBytes = 0;
    this.kMinMatchCheck = 4;
    this.kFixHashSize = kHash2Size + kHash3Size;
  } else {
    this.kNumHashDirectBytes = 2;
    this.kMinMatchCheck = 2 + 1;
    this.kFixHashSize = 0;
  }
};

BinTree.prototype.init = function() {
  var i;
  _super_.init.call(this);
  for (i = 0; i < this._hashSizeSum; i++) {
    this._hash[i] = kEmptyHashValue;
  }
  this._cyclicBufferPos = 0;
  this.reduceOffsets(-1);
};

BinTree.prototype.movePos = function() {
  if (++this._cyclicBufferPos >= this._cyclicBufferSize) {
    this._cyclicBufferPos = 0;
  }
  _super_.movePos.call(this);
  if (this._pos === kMaxValForNormalize) {
    this.normalize();
  }
};

BinTree.prototype.create = function(historySize, keepAddBufferBefore, matchMaxLen, keepAddBufferAfter) {
  var windowReservSize, cyclicBufferSize, hs;

  if (historySize > kMaxValForNormalize - 256) {
    //console.assert(false, 'Unsupported historySize');
    return false;
  }
  this._cutValue = 16 + (matchMaxLen >>> 1);

  windowReservSize = (historySize + keepAddBufferBefore +
                       matchMaxLen + keepAddBufferAfter) / 2 + 256;

  _super_.create.call(this, historySize + keepAddBufferBefore,
                      matchMaxLen + keepAddBufferAfter,
                      windowReservSize);

  this._matchMaxLen = matchMaxLen;

  cyclicBufferSize = historySize + 1;
  if (this._cyclicBufferSize !== cyclicBufferSize) {
    this._cyclicBufferSize = cyclicBufferSize;
    this._son = [];
    this._son.length = cyclicBufferSize * 2;
  }

  hs = kBT2HashSize;

  if (this.HASH_ARRAY) {
    hs = historySize - 1;
    hs |= hs >>> 1;
    hs |= hs >>> 2;
    hs |= hs >>> 4;
    hs |= hs >>> 8;
    hs >>>= 1;
    hs |= 0xFFFF;
    if (hs > (1 << 24)) {
      hs >>>= 1;
    }
    this._hashMask = hs;
    hs++;
    hs += this.kFixHashSize;
  }
  if (hs !== this._hashSizeSum) {
    this._hashSizeSum = hs;
    this._hash = [];
    this._hash.length = this._hashSizeSum;
  }
  return true;
};

BinTree.prototype.getMatches = function(distances) {
  var lenLimit;
  if (this._pos + this._matchMaxLen <= this._streamPos) {
    lenLimit = this._matchMaxLen;
  } else {
    lenLimit = this._streamPos - this._pos;
    if (lenLimit < this.kMinMatchCheck) {
      this.movePos();
      return 0;
    }
  }

  var offset = 0;
  var matchMinPos = (this._pos > this._cyclicBufferSize) ? (this._pos - this._cyclicBufferSize) : 0;
  var cur = this._bufferOffset + this._pos;
  var maxLen = kStartMaxLen; // to avoid items for len < hashSize
  var hashValue = 0, hash2Value = 0, hash3Value = 0;

  if (this.HASH_ARRAY) {
    var temp = CrcTable[this._bufferBase[cur]] ^ this._bufferBase[cur + 1];
    hash2Value = temp & (kHash2Size - 1);
    temp ^= this._bufferBase[cur + 2] << 8;
    hash3Value = temp & (kHash3Size - 1);
    hashValue = (temp ^ (CrcTable[this._bufferBase[cur + 3]] << 5)) & this._hashMask;
  } else {
    hashValue = this._bufferBase[cur] ^ (this._bufferBase[cur + 1] << 8);
  }

  var curMatch = this._hash[this.kFixHashSize + hashValue];
  if (this.HASH_ARRAY) {
    var curMatch2 = this._hash[hash2Value];
    var curMatch3 = this._hash[kHash3Offset + hash3Value];
    this._hash[hash2Value] = this._pos;
    this._hash[kHash3Offset + hash3Value] = this._pos;
    if (curMatch2 > matchMinPos) {
      if (this._bufferBase[this._bufferOffset + curMatch2] === this._bufferBase[cur]) {
        distances[offset++] = maxLen = 2;
        distances[offset++] = this._pos - curMatch2 - 1;
      }
    }
    if (curMatch3 > matchMinPos) {
      if (this._bufferBase[this._bufferOffset + curMatch3] === this._bufferBase[cur]) {
        if (curMatch3 === curMatch2) {
          offset -= 2;
        }
        distances[offset++] = maxLen = 3;
        distances[offset++] = this._pos - curMatch3 - 1;
        curMatch2 = curMatch3;
      }
    }
    if (offset !== 0 && curMatch2 === curMatch) {
      offset -= 2;
      maxLen = kStartMaxLen;
    }
  }

  this._hash[this.kFixHashSize + hashValue] = this._pos;

  var ptr0 = (this._cyclicBufferPos << 1) + 1;
  var ptr1 = (this._cyclicBufferPos << 1);

  var len0, len1;
  len0 = len1 = this.kNumHashDirectBytes;

  if (this.kNumHashDirectBytes !== 0) {
    if (curMatch > matchMinPos) {
      if (this._bufferBase[this._bufferOffset + curMatch + this.kNumHashDirectBytes] !==
          this._bufferBase[cur + this.kNumHashDirectBytes]) {
        distances[offset++] = maxLen = this.kNumHashDirectBytes;
        distances[offset++] = this._pos - curMatch - 1;
      }
    }
  }

  var count = this._cutValue;

  while (true) {
    if (curMatch <= matchMinPos || count-- === 0) {
      this._son[ptr0] = this._son[ptr1] = kEmptyHashValue;
      break;
    }

    var delta = this._pos - curMatch;
    var cyclicPos = ((delta <= this._cyclicBufferPos) ?
                 (this._cyclicBufferPos - delta) :
                 (this._cyclicBufferPos - delta + this._cyclicBufferSize)) << 1;

    var pby1 = this._bufferOffset + curMatch;
    var len = Math.min(len0, len1);
    if (this._bufferBase[pby1 + len] === this._bufferBase[cur + len]) {
      while (++len !== lenLimit) {
        if (this._bufferBase[pby1 + len] !== this._bufferBase[cur + len]) {
          break;
        }
      }
      if (maxLen < len) {
        distances[offset++] = maxLen = len;
        distances[offset++] = delta - 1;
        if (len === lenLimit) {
          this._son[ptr1] = this._son[cyclicPos];
          this._son[ptr0] = this._son[cyclicPos + 1];
          break;
        }
      }
    }
    if (this._bufferBase[pby1 + len] < this._bufferBase[cur + len]) {
      this._son[ptr1] = curMatch;
      ptr1 = cyclicPos + 1;
      curMatch = this._son[ptr1];
      len1 = len;
    } else {
      this._son[ptr0] = curMatch;
      ptr0 = cyclicPos;
      curMatch = this._son[ptr0];
      len0 = len;
    }
  }

  this.movePos();
  return offset;
};

BinTree.prototype.skip = function(num) {
  var lenLimit, matchMinPos, cur, curMatch, hashValue, hash2Value, hash3Value, temp;
  var ptr0, ptr1, len0, len1, count, delta, cyclicPos, pby1, len;
  do {
    if (this._pos + this._matchMaxLen <= this._streamPos) {
      lenLimit = this._matchMaxLen;
    } else {
      lenLimit = this._streamPos - this._pos;
      if (lenLimit < this.kMinMatchCheck) {
        this.movePos();
        continue;
      }
    }

    matchMinPos = this._pos > this._cyclicBufferSize ? (this._pos - this._cyclicBufferSize) : 0;
    cur = this._bufferOffset + this._pos;

    if (this.HASH_ARRAY) {
      temp = CrcTable[this._bufferBase[cur]] ^ this._bufferBase[cur + 1];
      hash2Value = temp & (kHash2Size - 1);
      this._hash[hash2Value] = this._pos;
      temp ^= this._bufferBase[cur + 2] << 8;
      hash3Value = temp & (kHash3Size - 1);
      this._hash[kHash3Offset + hash3Value] = this._pos;
      hashValue = (temp ^ (CrcTable[this._bufferBase[cur + 3]] << 5)) & this._hashMask;
    } else {
      hashValue = this._bufferBase[cur] ^ (this._bufferBase[cur + 1] << 8);
    }

    curMatch = this._hash[this.kFixHashSize + hashValue];
    this._hash[this.kFixHashSize + hashValue] = this._pos;

    ptr0 = (this._cyclicBufferPos << 1) + 1;
    ptr1 = (this._cyclicBufferPos << 1);

    len0 = len1 = this.kNumHashDirectBytes;

    count = this._cutValue;
    while (true) {
      if (curMatch <= matchMinPos || count-- === 0) {
        this._son[ptr0] = this._son[ptr1] = kEmptyHashValue;
        break;
      }

      delta = this._pos - curMatch;
      cyclicPos = (delta <= this._cyclicBufferPos ?
                   (this._cyclicBufferPos - delta) :
                   (this._cyclicBufferPos - delta + this._cyclicBufferSize)) << 1;

      pby1 = this._bufferOffset + curMatch;
      len = (len0 < len1) ? len0 : len1;
      if (this._bufferBase[pby1 + len] === this._bufferBase[cur + len]) {
        while (++len !== lenLimit) {
          if (this._bufferBase[pby1 + len] !== this._bufferBase[cur + len]) {
            break;
          }
        }
        if (len === lenLimit) {
          this._son[ptr1] = this._son[cyclicPos];
          this._son[ptr0] = this._son[cyclicPos + 1];
          break;
        }
      }
      if (this._bufferBase[pby1 + len] < this._bufferBase[cur + len]) {
        this._son[ptr1] = curMatch;
        ptr1 = cyclicPos + 1;
        curMatch = this._son[ptr1];
        len1 = len;
      } else {
        this._son[ptr0] = curMatch;
        ptr0 = cyclicPos;
        curMatch = this._son[ptr0];
        len0 = len;
      }
    }
    this.movePos();
  } while (--num !== 0);
};

BinTree.prototype.normalizeLinks = function(items, numItems, subValue) {
  var i, value;
  for (i = 0; i < numItems; i++) {
    value = items[i];
    if (value <= subValue) {
      value = kEmptyHashValue;
    } else {
      value -= subValue;
    }
    items[i] = value;
  }
};

BinTree.prototype.normalize = function() {
  var subValue = this._pos - this._cyclicBufferSize;
  this.normalizeLinks(this._son, this._cyclicBufferSize * 2, subValue);
  this.normalizeLinks(this._hash, this._hashSizeSum, subValue);
  this.reduceOffsets(subValue);
};

BinTree.prototype.setCutValue = function(cutValue) {
  this._cutValue = cutValue;
};

LZ.BinTree = BinTree;
}).call(this);
