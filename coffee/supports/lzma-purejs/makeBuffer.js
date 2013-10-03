  // typed array / Buffer compatibility.
  var makeBuffer = function(len) {
      var b = [], i;
      for (i=0; i<len; i++) { b[i] = 0; }
      return b;
  };
  if (typeof(Uint8Array) !== 'undefined') {
    makeBuffer = function(len) { return new Uint8Array(len); };
  }
