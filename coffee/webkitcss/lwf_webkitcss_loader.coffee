#
# Copyright (C) 2013 GREE, Inc.
#
# This software is provided 'as-is', without any express or implied
# warranty.  In no event will the authors be held liable for any damages
# arising from the use of this software.
#
# Permission is granted to anyone to use this software for any purpose,
# including commercial applications, and to alter it and redistribute it
# freely, subject to the following restrictions:
#
# 1. The origin of this software must not be misrepresented; you must not
#    claim that you wrote the original software. If you use this software
#    in a product, an acknowledgment in the product documentation would be
#    appreciated but is not required.
# 2. Altered source versions must be plainly marked as such, and must not be
#    misrepresented as being the original software.
# 3. This notice may not be removed or altered from any source distribution.
#

class WebkitCSSLoader
  @load:(d) ->
    return null if !d? or typeof d isnt "string"
    option = d.charCodeAt(Format.Constant.OPTION_OFFSET) & 0xff
    return Loader.load(d) if (option & Format.Constant.OPTION_COMPRESSED) is 0

    if typeof Uint8Array isnt 'undefined'
      a = new ArrayBuffer(d.length)
      b = new Uint8Array(a)
      for i in [0...d.length]
        b[i] = d.charCodeAt(i) & 0xff
      return @loadArrayBuffer(a)
    else
      a = new Array(d.length)
      for i in [0...d.length]
        a[i] = d.charCodeAt(i) & 0xff
      return @loadArray(a)

  @loadArray:(d) ->
    return null if !d?
    option = d[Format.Constant.OPTION_OFFSET]
    if (option & Format.Constant.OPTION_COMPRESSED) is 0
      return Loader.loadArray(d)

    header = d.slice(0, Format.Constant.HEADER_SIZE)
    compressed = d.slice(Format.Constant.HEADER_SIZE)
    try
      decompressed = global["LWF"].LZMA.decompressFile(compressed)
    catch e
      return null
    d = header.concat(decompressed)
    return Loader.loadArray(d)

  @loadArrayBuffer:(d) ->
    return null if !d?
    o = new Uint8Array(d, Format.Constant.OPTION_OFFSET, 1)
    option = o[0]
    if (option & Format.Constant.OPTION_COMPRESSED) is 0
      return Loader.loadArrayBuffer(d)

    header = new Uint8Array(d.slice(0, Format.Constant.HEADER_SIZE))
    compressed = new Uint8Array(d.slice(Format.Constant.HEADER_SIZE))
    try
      decompressed = global["LWF"].LZMA.decompressFile(compressed)
      decompressed = new Uint8Array(decompressed)
    catch e
      return null
    d = new Uint8Array(header.length + decompressed.length)
    d.set(header, 0)
    d.set(decompressed, Format.Constant.HEADER_SIZE)
    return Loader.loadArrayBuffer(d.buffer)

