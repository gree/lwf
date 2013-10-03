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

var Util = Object.create(null);

Util.decompress = function(properties, inStream, outStream, outSize){
  var decoder = new LZMA.Decoder();

  if ( !decoder.setDecoderProperties(properties) ){
    throw "Incorrect stream properties";
  }

  if ( !decoder.code(inStream, outStream, outSize) ){
    throw "Error in data stream";
  }

  return true;
};

/* Also accepts a Uint8Array/Buffer/array as first argument, in which case
 * returns the decompressed file as a Uint8Array/Buffer/array. */
Util.decompressFile = function(inStream, outStream){
  var decoder = new LZMA.Decoder(), i, mult;
  var retval = true;

  if (!('readByte' in inStream)) {
    var inBuffer = inStream;
    inStream = {
      size: inBuffer.length,
      pos: 0,
      readByte: function() { return inBuffer[this.pos++]; }
    };
  }

  if ( !decoder.setDecoderPropertiesFromStream(inStream) ){
    throw "Incorrect stream properties";
  }

  // largest integer in javascript is 2^53 (unless we use typed arrays)
  // but we don't explicitly check for overflow here.  caveat user.
  var outSizeLo = 0;
  for (i=0, mult=1; i<4; i++, mult*=256) {
    outSizeLo += (inStream.readByte() * mult);
  }
  var outSizeHi = 0;
  for (i=0, mult=1; i<4; i++, mult*=256) {
    outSizeHi += (inStream.readByte() * mult);
  }
  var outSize = outSizeLo + (outSizeHi * 0x100000000);
  if (outSizeLo === 0xFFFFFFFF && outSizeHi === 0xFFFFFFFF) {
    outSize = -1;
  } else if (outSizeHi >= 0x200000) {
    outSize = -1; // force streaming
  }

  if (!(outStream && 'writeByte' in outStream)) {
    outStream = {
      buffer: (outSize>=0) ? makeBuffer(outSize) : [],
      pos: 0,
      writeByte: function(b) { this.buffer[this.pos++] = b; }
    };
    retval = outStream.buffer;
  }

  if ( !decoder.code(inStream, outStream, outSize) ){
    throw "Error in data stream";
  }

  return retval;
};

/* The following is a mapping from gzip/bzip2 style -1 .. -9 compression modes
 * to the corresponding LZMA compression modes. Thanks, Larhzu, for coining
 * these. */ /* [csa] lifted from lzmp.cpp in the LZMA SDK. */
var option_mapping = [
  { a:0, d: 0, fb: 0,  mf: null, lc:0, lp:0, pb:0 },// -0 (needed for indexing)
  { a:0, d:16, fb:64,  mf:"hc4", lc:3, lp:0, pb:2 },// -1
  { a:0, d:20, fb:64,  mf:"hc4", lc:3, lp:0, pb:2 },// -2
  { a:1, d:19, fb:64,  mf:"bt4", lc:3, lp:0, pb:2 },// -3
  { a:2, d:20, fb:64,  mf:"bt4", lc:3, lp:0, pb:2 },// -4
  { a:2, d:21, fb:128, mf:"bt4", lc:3, lp:0, pb:2 },// -5
  { a:2, d:22, fb:128, mf:"bt4", lc:3, lp:0, pb:2 },// -6
  { a:2, d:23, fb:128, mf:"bt4", lc:3, lp:0, pb:2 },// -7
  { a:2, d:24, fb:255, mf:"bt4", lc:3, lp:0, pb:2 },// -8
  { a:2, d:25, fb:255, mf:"bt4", lc:3, lp:0, pb:2 } // -9
];

/** Create and configure an Encoder, based on the given properties (which
 *  make be a simple number, for a compression level between 1 and 9. */
var makeEncoder = function(props) {
  var encoder = new LZMA.Encoder();
  var params = { // defaults!
    a: 1, /* algorithm */
    d: 23, /* dictionary */
    fb: 128, /* fast bytes */
    lc: 3, /* literal context */
    lp: 0, /* literal position */
    pb: 2, /* position bits */
    mf: "bt4", /* match finder (bt2/bt4) */
    eos: false /* write end of stream */
  };
  // override default params with props
  if (props) {
    if (typeof(props)==='number') { // -1 through -9 options
      props = option_mapping[props];
    }
    var p;
    for (p in props) {
      if (Object.prototype.hasOwnProperty.call(props, p)) {
        params[p] = props[p];
      }
    }
  }
  encoder.setAlgorithm(params.a);
  encoder.setDictionarySize( 1<< (+params.d));
  encoder.setNumFastBytes(+params.fb);
  encoder.setMatchFinder((params.mf === 'bt4') ?
                         LZMA.Encoder.EMatchFinderTypeBT4 :
                         LZMA.Encoder.EMatchFinderTypeBT2);
  encoder.setLcLpPb(+params.lc, +params.lp, +params.pb);
  encoder.setEndMarkerMode(!!params.eos);
  return encoder;
};

Util.compress = function(inStream, outStream, props, progress){
  var encoder = makeEncoder(props);

  encoder.writeCoderProperties(outStream);

  encoder.code(inStream, outStream, -1, -1, {
    setProgress: function(inSize, outSize) {
      if (progress) { progress(inSize, outSize); }
    }
  });

  return true;
};

/* Also accepts a Uint8Array/Buffer/array as first argument, in which case
 * returns the compressed file as a Uint8Array/Buffer/array. */
Util.compressFile = function(inStream, outStream, props, progress) {
  var encoder = makeEncoder(props);
  var retval = true, i;

  if (!('readByte' in inStream)) {
    var inBuffer = inStream;
    inStream = {
      size: inBuffer.length,
      pos: 0,
      readByte: function() { return inBuffer[this.pos++]; },
      read: function(buffer, bufOffset, length) {
        var bytesRead = 0;
        while (bytesRead < length && this.pos < inBuffer.length) {
          buffer[bufOffset++] = inBuffer[this.pos++];
          bytesRead++;
        }
        return bytesRead;
      }
    };
  }
  // if we know the size, write it; otherwise we need to use the 'eos' property
  var fileSize;
  if ('size' in inStream && inStream.size >= 0) {
    fileSize = inStream.size;
  } else {
    fileSize = -1;
    encoder.setEndMarkerMode(true);
  }

  if (!(outStream && 'writeByte' in outStream)) {
    outStream = {
      buffer: [],
      pos: 0,
      writeByte: function(b) { this.buffer[this.pos++] = b; },
      flush: function() { /* do nothing */ }
    };
    retval = outStream.buffer;
  }

  encoder.writeCoderProperties(outStream);

  var out64 = function(s) {
    // supports up to 53-bit integers
    var i;
    for (i=0;i<8;i++) {
      outStream.writeByte(s & 0xFF);
      s = Math.floor(s/256);
    }
  };
  out64(fileSize);

  encoder.code(inStream, outStream, fileSize, -1, {
    setProgress: function(inSize, outSize) {
      if (progress) { progress(inSize, outSize); }
    }
  });

  return retval;
};
