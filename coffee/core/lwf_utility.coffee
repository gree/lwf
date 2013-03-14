#
# Copyright (C) 2012 GREE, Inc.
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

class Utility
  @calcMatrixToPoint:(sx, sy, m) ->
    dx = m.scaleX * sx + m.skew0  * sy + m.translateX
    dy = m.skew1  * sx + m.scaleY * sy + m.translateY
    return [dx, dy]

  @getMatrixDeterminant:(matrix) ->
    return matrix.scaleX * matrix.scaleY - matrix.skew0 * matrix.skew1 < 0

  @syncMatrix:(movie) ->
    matrixId = movie.matrixId ? 0
    if (matrixId & Constant.MATRIX_FLAG) is 0
      translate = movie.lwf.data.translates[matrixId]
      scaleX = 1
      scaleY = 1
      rotation = 0
      matrix =
        scaleX: scaleX
        scaleY: scaleY
        skew0: 0
        skew1: 0
        translateX: translate.translateX
        translateY: translate.translateY
    else
      matrixId &= ~Constant.MATRIX_FLAG
      matrix = movie.lwf.data.matrices[matrixId]
      md = @getMatrixDeterminant(matrix)
      scaleX = Math.sqrt(
        matrix.scaleX * matrix.scaleX + matrix.skew1 * matrix.skew1)
      scaleX = -scaleX if md
      scaleY = Math.sqrt(
        matrix.scaleY * matrix.scaleY + matrix.skew0 * matrix.skew0)
      if md
        rotation = Math.atan2(matrix.skew1, -matrix.scaleX)
      else
        rotation = Math.atan2(matrix.skew1, matrix.scaleX)
      rotation = rotation / Math.PI * 180

    movie.property.setMatrix(matrix, scaleX, scaleY, rotation)
    return

  @getX:(movie) ->
    matrixId = movie.matrixId ? 0
    if (matrixId & Constant.MATRIX_FLAG) is 0
      translate = movie.lwf.data.translates[matrixId]
      return translate.translateX
    else
      matrixId &= ~Constant.MATRIX_FLAG
      matrix = movie.lwf.data.matrices[matrixId]
      return matrix.translateX

  @getY:(movie) ->
    matrixId = movie.matrixId ? 0
    if (matrixId & Constant.MATRIX_FLAG) is 0
      translate = movie.lwf.data.translates[matrixId]
      return translate.translateY
    else
      matrixId &= ~Constant.MATRIX_FLAG
      matrix = movie.lwf.data.matrices[matrixId]
      return matrix.translateY

  @getScaleX:(movie) ->
    matrixId = movie.matrixId ? 0
    if (matrixId & Constant.MATRIX_FLAG) is 0
      return 1
    else
      matrixId &= ~Constant.MATRIX_FLAG
      matrix = movie.lwf.data.matrices[matrixId]
      md = @getMatrixDeterminant(matrix)
      scaleX = Math.sqrt(
        matrix.scaleX * matrix.scaleX + matrix.skew1 * matrix.skew1)
      scaleX = -scaleX if md
      return scaleX

  @getScaleY:(movie) ->
    matrixId = movie.matrixId ? 0
    if (matrixId & Constant.MATRIX_FLAG) is 0
      return 1
    else
      matrixId &= ~Constant.MATRIX_FLAG
      matrix = movie.lwf.data.matrices[matrixId]
      scaleY = Math.sqrt(
        matrix.scaleY * matrix.scaleY + matrix.skew0 * matrix.skew0)
      return scaleY

  @getRotation:(movie) ->
    matrixId = movie.matrixId ? 0
    if (matrixId & Constant.MATRIX_FLAG) is 0
      return 0
    else
      matrixId &= ~Constant.MATRIX_FLAG
      matrix = movie.lwf.data.matrices[matrixId]
      md = @getMatrixDeterminant(matrix)
      if md
        rotation = Math.atan2(matrix.skew1, -matrix.scaleX)
      else
        rotation = Math.atan2(matrix.skew1, matrix.scaleX)
      rotation = rotation / Math.PI * 180
      return rotation

  @syncColorTransform:(movie) ->
    colorTransformId = movie.colorTransformId ? 0
    if (colorTransformId & Constant.COLORTRANSFORM_FLAG) is 0
      alphaTransform = movie.lwf.data.alphaTransforms[colorTransformId]
      colorTransform =
        multi:
          red: 1
          green: 1
          blue: 1
          alpha: alphaTransform.alpha
    else
      colorTransformId = colorTransformId & ~Constant.COLORTRANSFORM_FLAG
      colorTransform = movie.lwf.data.colorTransforms[colorTransformId]

    movie.property.setColorTransform(colorTransform)
    return

  @getAlpha:(movie) ->
    colorTransformId = movie.colorTransformId ? 0
    if (colorTransformId & Constant.COLORTRANSFORM_FLAG) is 0
      alphaTransform = movie.lwf.data.alphaTransforms[colorTransformId]
      return alphaTransform.alpha
    else
      colorTransformId = colorTransformId & ~Constant.COLORTRANSFORM_FLAG
      colorTransform = movie.lwf.data.colorTransforms[colorTransformId]
      return colorTransform.alpha

  @calcMatrixId:(lwf, dst, src0, src1Id) ->
    if src1Id is 0
      dst.set(src0)
    else if (src1Id & Constant.MATRIX_FLAG) is 0
      translate = lwf.data.translates[src1Id]
      dst.scaleX = src0.scaleX
      dst.skew0  = src0.skew0
      dst.translateX =
        src0.scaleX * translate.translateX +
        src0.skew0  * translate.translateY +
        src0.translateX
      dst.skew1  = src0.skew1
      dst.scaleY = src0.scaleY
      dst.translateY =
        src0.skew1  * translate.translateX +
        src0.scaleY * translate.translateY +
        src0.translateY
    else
      matrixId = src1Id & ~Constant.MATRIX_FLAG
      src1 = lwf.data.matrices[matrixId]
      @calcMatrix(dst, src0, src1)
    return dst

  @calcMatrix:(dst, src0, src1) ->
    dst.scaleX =
      src0.scaleX * src1.scaleX +
      src0.skew0  * src1.skew1
    dst.skew0 =
      src0.scaleX * src1.skew0 +
      src0.skew0  * src1.scaleY
    dst.translateX =
      src0.scaleX * src1.translateX +
      src0.skew0  * src1.translateY +
      src0.translateX
    dst.skew1 =
      src0.skew1  * src1.scaleX +
      src0.scaleY * src1.skew1
    dst.scaleY =
      src0.skew1  * src1.skew0 +
      src0.scaleY * src1.scaleY
    dst.translateY =
      src0.skew1  * src1.translateX +
      src0.scaleY * src1.translateY +
      src0.translateY
    return dst

  @rotateMatrix:(dst, src, scale, offsetX, offsetY) ->
    offsetX *= scale
    offsetY *= scale
    dst.scaleX = -src.skew0 * scale
    dst.skew0 = src.scaleX * scale
    dst.translateX = src.scaleX * offsetX + src.skew0 * offsetY + src.translateX
    dst.skew1 = -src.scaleY * scale
    dst.scaleY = src.skew1 * scale
    dst.translateY = src.skew1 * offsetX + src.scaleY * offsetY + src.translateY
    return dst

  @scaleMatrix:(dst, src, scale, offsetX, offsetY) ->
    offsetX *= scale
    offsetY *= scale
    dst.scaleX = src.scaleX * scale
    dst.skew0 = src.skew0 * scale
    dst.translateX = src.scaleX * offsetX + src.skew0 * offsetY + src.translateX
    dst.skew1 = src.skew1 * scale
    dst.scaleY = src.scaleY * scale
    dst.translateY = src.skew1 * offsetX + src.scaleY * offsetY + src.translateY
    return dst

  @fitForHeight:(lwf, stageWidth, stageHeight) ->
    scale = stageHeight / lwf.height
    lwf.scaleByStage = scale
    lwf.property.scale(scale, scale)
    lwf.property.move((stageWidth - lwf.width * scale) / 2, 0)
    return

  @fitForWidth:(lwf, stageWidth, stageHeight) ->
    scale = stageWidth / lwf.width
    lwf.scaleByStage = scale
    lwf.property.scale(scale, scale)
    lwf.property.move(0, (stageHeight - lwf.height * scale) / 2, 0)
    return

  @scaleForHeight:(lwf, stageHeight) ->
    scale = stageHeight / lwf.height
    lwf.scaleByStage = scale
    lwf.property.scale(scale, scale)
    return

  @scaleForWidth:(lwf, stageWidth) ->
    scale = stageWidth / lwf.width
    lwf.scaleByStage = scale
    lwf.property.scale(scale, scale)
    return

  @copyMatrix:(dst, src) ->
    if src isnt null then dst.set(src) else dst.clear()
    return dst

  @invertMatrix:(dst, src) ->
    dt = src.scaleX * src.scaleY - src.skew0 * src.skew1
    if dt isnt 0
      dst.scaleX = src.scaleY / dt
      dst.skew0 = -src.skew0 / dt
      dst.translateX = (src.skew0 * src.translateY -
        src.translateX * src.scaleY) / dt
      dst.skew1 = -src.skew1 / dt
      dst.scaleY = src.scaleX / dt
      dst.translateY = (src.translateX * src.skew1 -
        src.scaleX * src.translateY) / dt
    else
      dst.clear()
    return

  @calcColorTransformId:(lwf, dst, src0, src1Id) ->
    if src1Id is 0
      dst.set(src0)
    else if (src1Id & Constant.COLORTRANSFORM_FLAG) is 0
      alphaTransform = lwf.data.alphaTransforms[src1Id]
      dst.multi.red   = src0.multi.red
      dst.multi.green = src0.multi.green
      dst.multi.blue  = src0.multi.blue
      dst.multi.alpha = src0.multi.alpha * alphaTransform.alpha
      #dst.add.set(src0.add)
    else
      colorTransformId = src1Id & ~Constant.COLORTRANSFORM_FLAG
      src1 = lwf.data.colorTransforms[colorTransformId]
      @calcColorTransform(dst, src0, src1)
    return dst

  @calcColorTransform:(dst, src0, src1) ->
    dst.multi.red   = src0.multi.red   * src1.multi.red
    dst.multi.green = src0.multi.green * src1.multi.green
    dst.multi.blue  = src0.multi.blue  * src1.multi.blue
    dst.multi.alpha = src0.multi.alpha * src1.multi.alpha
    #dst.add.red   = src0.add.red   * src1.multi.red   + src1.add.red
    #dst.add.green = src0.add.green * src1.multi.green + src1.add.green
    #dst.add.blue  = src0.add.blue  * src1.multi.blue  + src1.add.blue
    #dst.add.alpha = src0.add.alpha * src1.multi.alpha + src1.add.alpha
    return dst

  @copyColorTransform:(dst, src) ->
    if src isnt null then dst.set(src) else dst.clear()
    return dst

  @calcColor:(dst, c, t) ->
    dst.red   = c.red   * t.multi.red
    dst.green = c.green * t.multi.green
    dst.blue  = c.blue  * t.multi.blue
    dst.alpha = c.alpha * t.multi.alpha
    #dst.red   = c.red   * t.multi.red   + t.add.red
    #dst.green = c.green * t.multi.green + t.add.green
    #dst.blue  = c.blue  * t.multi.blue  + t.add.blue
    #dst.alpha = c.alpha * t.multi.alpha + t.add.alpha
    return

  @newIntArray:() ->
    return []

  @insertIntArray:(array, v) ->
    if array.length is 0 or v > array[array.length - 1]
      array.push(v)
      return
    i = @locationOfIntArray(array, v, 0, array.length - 1)
    array.splice(i, 0, v) if array[i] isnt v
    return

  @deleteIntArray:(array, v) ->
    i = @locationOfIntArray(array, v, 0, array.length - 1)
    array.splice(i, 1) if array[i] is v
    return

  @locationOfIntArray:(array, v, first, last) ->
    while first <= last
      mid = ((first + last) / 2) >> 0
      if v > array[mid]
        first = mid + 1
      else if v < array[mid]
        last = mid - 1
      else
        return mid
    return first

