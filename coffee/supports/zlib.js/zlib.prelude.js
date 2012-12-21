(function() {

var Zlib = {};

var USE_TYPEDARRAY = false;

if (typeof Uint8Array !== 'undefined' &&
		typeof Uint16Array !== 'undefined' &&
		typeof Uint32Array !== 'undefined') {
	USE_TYPEDARRAY = true;
}

