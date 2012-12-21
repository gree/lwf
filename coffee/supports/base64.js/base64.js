(function() {

var Base64 = {};

/**
 * @const
 * @type {string}
 */
Base64.Character =  'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';

/**
 * @const
 * @type {string}
 */
Base64.RangeError = "INVALID_CHARACTER_ERR";

Base64.btoaArray = {};

/**
 * @const
 * @type {(Uint8Array|Array.<number>)}
 */
Base64.btoaArray.CharacterTable = (
/**
 * @param {string} chars
 * @return {(Uint8Array|Array.<number>)}
 */
function(chars) {
  /** @type {(Uint8Array|Array.<number>)} */
  var array =
    new (typeof Uint8Array !== 'undefined' ? Uint8Array : Array)(chars.length);
  /** @type {number} */
  var i;
  /** @type {number} */
  var il;

  for (i = 0, il = array.length; i < il; ++i) {
    array[i] = chars.charCodeAt(i);
  }

  return array;
})(Base64.Character);

/**
 * @param {string} str base64 encoded string.
 * @return {Array.<number>|Uint8Array} decoded byte-array.
 */
Base64.atobArray = function(str) {
  /** @type {number} */
  var buffer = 0;
  /** @type {number} */
  var pos = 0;
  /** @type {number} */
  var length = str.length;
  /** @type {(Uint8Array|Array.<number>)} */
  var out;
  /** @type {number} */
  var outpos = 0;
  /** @type {number} */
  var bitlen = 0;
  /** @type {Array.<number>|Int16Array} */
  var decode = Base64.atobArray.DecodeTable;
  /** @type {number} */
  var decoded;
  /** @type {number} */
  var tmp;
  /** @type {number} */
  var mod;

  // remove padding
  while (str.charAt(length-1) === '=') {
    --length;
  }
  mod = length % 4;

  // create output buffer
  out = new (typeof Uint8Array !== 'undefined' ? Uint8Array : Array)(
    ((length + 3) / 4 | 0) * 3 - [0, 0, 2, 1][mod]
  );

  // check range
  if (length % 4 === 1 || (str.length > 0 && length === 0)) {
    throw new Error(Base64.RangeError);
  }

  while (pos < length) {
    tmp = str.charCodeAt(pos++);
    decoded = tmp < 256 ? decode[tmp] : -1;

    // check character range
    if (decoded === -1) {
      throw new Error(Base64.RangeError);
    }

    // add buffer (6bit)
    buffer = (buffer << 6) + decoded;
    bitlen += 6;

    // decode byte
    if (bitlen >= 8) {
      bitlen -= 8;

      // extract byte
      tmp = buffer >> bitlen;

      // decode character
      out[outpos++] = tmp;

      // remove character bits
      buffer ^= tmp << bitlen;
    }
  }

  return out;
};

/**
 * @const
 * @type {(Array.<number>|Int16Array)}
 */
Base64.atobArray.DecodeTable = (
/**
 * @param {Uint8Array|Array.<number>} encodeTable character table.
 * @return {Int16Array|Array.<number>} decode table.
 */
function(encodeTable) {
  /** @type {(Int16Array|Array.<number>)} */
  var table = new (typeof Int16Array !== 'undefined' ? Int16Array : Array)(256);
  /** @type {number} */
  var i;
  /** @type {Array.<number>} */
  var array = encodeTable instanceof Array ?
    encodeTable : Array.prototype.slice.call(encodeTable);

  for (i = 0; i < 0xff; ++i) {
    table[i] = array.indexOf(i);
  }

  return table;
})(Base64.btoaArray.CharacterTable);

global["LWF"].Base64 = Base64;

}).call(this);
