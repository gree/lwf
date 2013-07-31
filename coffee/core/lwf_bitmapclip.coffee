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
    data = lwf.data.bitmaps[objId]
    fragment = lwf.data.textureFragments[data.textureFragmentId]
    @width = fragment.w
    @height = fragment.h
    @dataMatrixId = data.matrixId
    @renderer = lwf.rendererFactory.constructBitmap(lwf, objId, @)

    @_regX = 0
    @_regY = 0
    @_x = 0
    @_y = 0
    @_scaleX = 1
    @_scaleY = 1
    @_rotation = 0
    @_alpha = 1
    @depth = -1

    @dirtyMatrix = true
    @dirtyMatrixSR = false
    @dirtyColorTransform = true
    @mScaleX = 1
    @mScaleY = 1
    @mSkew0 = 0
    @mSkew1 = 0

  updateMatrix:(m) ->
    if @dirtyMatrixSR
      radian = @_rotation * Math.PI / 180
      if radian is 0
        c = 1
        s = 0
      else
        c = Math.cos(radian)
        s = Math.sin(radian)
      @mScaleX = @_scaleX * c
      @mSkew0 = @_scaleY * -s
      @mSkew1 = @_scaleX * s
      @mScaleY = @_scaleY * c
      @dirtyMatrixSR = false

    dst = @matrix
    dst.scaleX =
      m.scaleX * @mScaleX +
      m.skew0  * @mSkew1
    dst.skew0 =
      m.scaleX * @mSkew0 +
      m.skew0  * @mScaleY
    dst.translateX =
      m.scaleX * @_x +
      m.skew0  * @_y +
      m.translateX +
        m.scaleX * @_regX + m.skew0 * @_regY +
        dst.scaleX * -@_regX + dst.skew0 * -@_regY
    dst.skew1 =
      m.skew1  * @mScaleX +
      m.scaleY * @mSkew1
    dst.scaleY =
      m.skew1  * @mSkew0 +
      m.scaleY * @mScaleY
    dst.translateY =
      m.skew1  * @_x +
      m.scaleY * @_y +
      m.translateY +
        m.skew1 * @_regX + m.scaleY * @_regY +
        dst.skew1 * -@_regX + dst.scaleY * -@_regY
    @dirtyMatrix = false
    return

  updateColorTransform:(c) ->
    m = @colorTransform.multi
    cm = c.multi
    m.red = cm.red
    m.green = cm.green
    m.blue = cm.blue
    m.alpha = @_alpha * cm.alpha
    @dirtyColorTransform = false
    return

  dirty:(dirtyMatrixSR = false) ->
    @lwf.setPropertyDirty()
    @dirtyMatrix = true
    @dirtyMatrixSR = true if dirtyMatrixSR
    return

  getRegX: ->
    return @_regX

  setRegX:(v) ->
    @dirty() if @_regX isnt v
    @_regX = v
    return

  getRegY: ->
    return @_regY

  setRegY:(v) ->
    @dirty() if @_regY isnt v
    @_regY = v
    return

  getX: ->
    return @_x

  setX:(v) ->
    @dirty() if @_x isnt v
    @_x = v
    return

  getY: ->
    return @_y

  setY:(v) ->
    @dirty() if @_y isnt v
    @_y = v
    return

  getScaleX: ->
    return @_scaleX

  setScaleX:(v) ->
    @dirty(true) if @_scaleX isnt v
    @_scaleX = v
    return

  getScaleY: ->
    return @_scaleY

  setScaleY:(v) ->
    @dirty(true) if @_scaleY isnt v
    @_scaleY = v
    return

  getRotation: ->
    return @_rotation

  setRotation:(v) ->
    @dirty(true) if @_rotation isnt v
    @_rotation = v
    return

  getAlphaProperty: ->
    return @_alpha

  setAlphaProperty:(v) ->
    if @_alpha isnt v
      @lwf.setPropertyDirty()
      @dirtyColorTransform = true
    @_alpha = v
    return

if typeof(BitmapClip.prototype.__defineGetter__) isnt "undefined"
  BitmapClip.prototype.__defineGetter__("regX", -> @getRegX())
  BitmapClip.prototype.__defineSetter__("regX", (v) -> @setRegX(v))
  BitmapClip.prototype.__defineGetter__("regY", -> @getRegY())
  BitmapClip.prototype.__defineSetter__("regY", (v) -> @setRegY(v))
  BitmapClip.prototype.__defineGetter__("x", -> @getX())
  BitmapClip.prototype.__defineSetter__("x", (v) -> @setX(v))
  BitmapClip.prototype.__defineGetter__("y", -> @getY())
  BitmapClip.prototype.__defineSetter__("y", (v) -> @setY(v))
  BitmapClip.prototype.__defineGetter__("scaleX", -> @getScaleX())
  BitmapClip.prototype.__defineSetter__("scaleX", (v) -> @setScaleX(v))
  BitmapClip.prototype.__defineGetter__("scaleY", -> @getScaleY())
  BitmapClip.prototype.__defineSetter__("scaleY", (v) -> @setScaleY(v))
  BitmapClip.prototype.__defineGetter__("rotation", -> @getRotation())
  BitmapClip.prototype.__defineSetter__("rotation", (v) -> @setRotation(v))
  BitmapClip.prototype.__defineGetter__("alpha", -> @getAlphaProperty())
  BitmapClip.prototype.__defineSetter__("alpha", (v) -> @setAlphaProperty(v))
else if typeof(Object.defineProperty) isnt "undefined"
  Object.defineProperty(BitmapClip.prototype, "regX",
    get: -> @getRegX()
    set: (v) -> @setRegX(v))
  Object.defineProperty(BitmapClip.prototype, "regY",
    get: -> @getRegY()
    set: (v) -> @setRegY(v))
  Object.defineProperty(BitmapClip.prototype, "x",
    get: -> @getX()
    set: (v) -> @setX(v))
  Object.defineProperty(BitmapClip.prototype, "y",
    get: -> @getY()
    set: (v) -> @setY(v))
  Object.defineProperty(BitmapClip.prototype, "scaleX",
    get: -> @getScaleX()
    set: (v) -> @setScaleX(v))
  Object.defineProperty(BitmapClip.prototype, "scaleY",
    get: -> @getScaleY()
    set: (v) -> @setScaleY(v))
  Object.defineProperty(BitmapClip.prototype, "rotation",
    get: -> @getRotation()
    set: (v) -> @setRotation(v))
  Object.defineProperty(BitmapClip.prototype, "alpha",
    get: -> @getAlphaProperty()
    set: (v) -> @setAlphaProperty(v))

