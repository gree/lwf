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

class BitmapClip extends LObject
  constructor:(lwf, parent, objId) ->
    super(lwf, parent, Type.BITMAP, objId)
    @isBitmapClip = true
    data = lwf.data.bitmaps[objId]
    fragment = lwf.data.textureFragments[data.textureFragmentId]
    texdata = lwf.data.textures[fragment.textureId]
    @width = fragment.w / texdata.scale
    @height = fragment.h / texdata.scale
    @dataMatrixId = data.matrixId
    @renderer = lwf.rendererFactory.constructBitmap(lwf, objId, @)

    @depth = -1
    @visible = true

    @regX = 0
    @regY = 0
    @x = 0
    @y = 0
    @scaleX = 1
    @scaleY = 1
    @rotation = 0
    @alpha = 1

    @_scaleX = @scaleX
    @_scaleY = @scaleY
    @_rotation = @rotation
    @_cos = 1
    @_sin = 0

    @_ = if TypedArray.available then new Float32Array(8) else []
    @_[0] = 1 # mScaleX
    @_[1] = 0 # mSkew1
    @_[2] = 0 # mSkew0
    @_[3] = 1 # mScaleY
    @_[4] = 0 # x
    @_[5] = 0 # y
    @_[6] = 0 # regX
    @_[7] = 0 # regY

  update:(m, c) ->
    dst = @matrix._
    mm = m._
    my = @_

    if @rotation isnt @_rotation
      @_rotation = @rotation
      radian = @_rotation * Math.PI / 180
      if radian is 0
        @_cos = 1
        @_sin = 0
      else
        @_cos = Math.cos(radian)
        @_sin = Math.sin(radian)
      dirty = true
    else
      dirty = false
    if dirty or @_scaleX isnt @scaleX or @_scaleY isnt @scaleY
      @_scaleX = @scaleX
      @_scaleY = @scaleY
      #@mScaleX = @_scaleX * c
      my[0] = @_scaleX * @_cos
      #@mSkew1 = @_scaleX * s
      my[1] = @_scaleX * @_sin
      #@mSkew0 = @_scaleY * -s
      my[2] = @_scaleY * -@_sin
      #@mScaleY = @_scaleY * c
      my[3] = @_scaleY * @_cos

    #x = @_x - @_regX
    my[4] = @x
    my[6] = @regX
    my[4] -= my[6]
    #y = @_y - @_regY
    my[5] = @y
    my[7] = @regY
    my[5] -= my[7]

    #dst = @matrix
    #dst.scaleX =
    #  m.scaleX * @mScaleX +
    #  m.skew0  * @mSkew1
    dst[0] = mm[0] * my[0] + mm[2] * my[1]
    #dst.skew0 =
    #  m.scaleX * @mSkew0 +
    #  m.skew0  * @mScaleY
    dst[2] = mm[0] * my[2] + mm[2] * my[3]
    #dst.translateX =
    #  m.scaleX * x +
    #  m.skew0  * y +
    #  m.translateX +
    #    m.scaleX * @_regX + m.skew0 * @_regY +
    #    dst.scaleX * -@_regX + dst.skew0 * -@_regY
    dst[4] = mm[0] * my[4] + mm[2] * my[5] +
      mm[4] + mm[0] * my[6] + mm[2] * my[7] +
      dst[0] * -my[6] + dst[2] * -my[7]
    #dst.skew1 =
    #  m.skew1  * @mScaleX +
    #  m.scaleY * @mSkew1
    dst[1] = mm[1] * my[0] + mm[3] * my[1]
    #dst.scaleY =
    #  m.skew1  * @mSkew0 +
    #  m.scaleY * @mScaleY
    dst[3] = mm[1] * my[2] + mm[3] * my[3]
    #dst.translateY =
    #  m.skew1  * x +
    #  m.scaleY * y +
    #  m.translateY +
    #    m.skew1 * @_regX + m.scaleY * @_regY +
    #    dst.skew1 * -@_regX + dst.scaleY * -@_regY
    dst[5] = mm[1] * my[4] + mm[3] * my[5] +
      mm[5] + mm[1] * my[6] + mm[3] * my[7] +
      dst[1] * -my[6] + dst[3] * -my[7]

    dst = @colorTransform.multi._
    cm = c.multi._
    if @lwf.useVertexColor
      #m.red = cm.red
      #m.green = cm.green
      #m.blue = cm.blue
      #m.alpha = @_alpha * cm.alpha
      if @alpha is 1
        for i in [0...4]
          dst[i] = cm[i]
      else
        for i in [0...3]
          dst[i] = cm[i]
        dst[3] = @alpha * cm[3]
    else
      dst[3] = @alpha * cm[3]
    @lwf.renderObject()
    return

  setMatrix:(m) ->
    #@mScaleX = m.scaleX
    #@mScaleY = m.scaleY
    #@mSkew0 = m.skew0
    #@mSkew1 = m.skew1
    #@x = m.translateX
    #@y = m.translateY
    for i in [0...6]
      @_[i] = m._[i]
    return

  detachFromParent: ->
    if @parent?
      @parent.detachBitmap(@depth)
      @parent = null
    return

