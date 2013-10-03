(function() {

  var Base = Object.create(null);
  Base.kNumRepDistances = 4;
  Base.kNumStates = 12;

  Base.stateInit = function() {
    return 0;
  };

  Base.stateUpdateChar = function(index) {
    if (index < 4) {
      return 0;
    }
    if (index < 10) {
      return index - 3;
    }
    return index - 6;
  };

  Base.stateUpdateMatch = function(index) {
    return (index < 7 ? 7 : 10);
  };

  Base.stateUpdateRep = function(index) {
    return (index < 7 ? 8 : 11);
  };

  Base.stateUpdateShortRep = function(index) {
    return (index < 7 ? 9 : 11);
  };

  Base.stateIsCharState = function(index) {
    return index < 7;
  };

  Base.kNumPosSlotBits = 6;
  Base.kDicLogSizeMin = 0;
  // Base.kDicLogSizeMax = 28;
  // Base.kDistTableSizeMax = Base.kDicLogSizeMax * 2;

  Base.kNumLenToPosStatesBits = 2; // it's for speed optimization
  Base.kNumLenToPosStates = 1 << Base.kNumLenToPosStatesBits;

  Base.kMatchMinLen = 2;

  Base.getLenToPosState = function(len) {
    len -= Base.kMatchMinLen;
    if (len < Base.kNumLenToPosStates) {
      return len;
    }
    return (Base.kNumLenToPosStates - 1);
  };

  Base.kNumAlignBits = 4;
  Base.kAlignTableSize = 1 << Base.kNumAlignBits;
  Base.kAlignMask = (Base.kAlignTableSize - 1);

  Base.kStartPosModelIndex = 4;
  Base.kEndPosModelIndex = 14;
  Base.kNumPosModels = Base.kEndPosModelIndex - Base.kStartPosModelIndex;

  Base.kNumFullDistances = 1 << (Base.kEndPosModelIndex / 2);

  Base.kNumLitPosStatesBitsEncodingMax = 4;
  Base.kNumLitContextBitsMax = 8;

  Base.kNumPosStatesBitsMax = 4;
  Base.kNumPosStatesMax = (1 << Base.kNumPosStatesBitsMax);
  Base.kNumPosStatesBitsEncodingMax = 4;
  Base.kNumPosStatesEncodingMax = (1 << Base.kNumPosStatesBitsEncodingMax);

  Base.kNumLowLenBits = 3;
  Base.kNumMidLenBits = 3;
  Base.kNumHighLenBits = 8;
  Base.kNumLowLenSymbols = 1 << Base.kNumLowLenBits;
  Base.kNumMidLenSymbols = 1 << Base.kNumMidLenBits;
  Base.kNumLenSymbols = Base.kNumLowLenSymbols + Base.kNumMidLenSymbols +
    (1 << Base.kNumHighLenBits);
  Base.kMatchMaxLen = Base.kMatchMinLen + Base.kNumLenSymbols - 1;

LZMA.Base = Base;
