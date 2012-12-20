#
# This Base64 class is placed in the public domain.
# It is derived from cdecoder.c.
#
# cdecoder.c - c source to a base64 decoding algorithm implementation
# This is part of the libb64 project, and has been placed in the public domain.
# For details, see http://sourceforge.net/projects/libb64
#

class Base64
  @table = [
    62, -1, -1, -1, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1,
    -1, -1, -2, -1, -1, -1,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9,
    10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
    -1, -1, -1, -1, -1, -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35,
    36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51
  ]

  @decode:(str) ->
    table_length = Base64.table.length
    decoded = new Array((((table_length + 2) / 3) | 0) * 4)
    c = 0
    n = 0
    j = 0
    for i in [0...str.length]
      v = (str.charCodeAt(i) & 0xff) - 43
      continue if v < 0 or v >= table_length
      fragment = Base64.table[v]
      continue if fragment < 0
      switch n
        when 0
          c = (fragment & 0x03f) << 2
          ++n
        when 1
          c |= (fragment & 0x030) >> 4
          decoded[j++] = c
          c = (fragment & 0x00f) << 4
          ++n
        when 2
          c |= (fragment & 0x03c) >> 2
          decoded[j++] = c
          c = (fragment & 0x003) << 6
          ++n
        when 3
          c |= (fragment & 0x03f)
          decoded[j++] = c
          n = 0
    return decoded

