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

class Property
  constructor:(@lwf) ->
    @matrix = new Matrix
    @colorTransform = new ColorTransform
    @scaleX = 1
    @scaleY = 1
    @rotation = 0
    @hasMatrix = false
    @hasColorTransform = false
    @clearRenderingOffset()

  clear: ->
    @scaleX = 1
    @scaleY = 1
    @rotation = 0
    @matrix.clear()
    @colorTransform.clear()
    if @hasMatrix or @hasColorTransform
      @lwf.setPropertyDirty()
    @hasMatrix = false
    @hasColorTransform = false
    @clearRenderingOffset()
    return

  move:(x, y) ->
    @matrix.translateX += x
    @matrix.translateY += y
    @hasMatrix = true
    @lwf.setPropertyDirty()
    return

  moveTo:(x, y) ->
    @matrix.translateX = x
    @matrix.translateY = y
    @hasMatrix = true
    @lwf.setPropertyDirty()
    return

  rotate:(degree) ->
    @rotateTo(@rotation + degree)
    return

  rotateTo:(degree) ->
    @rotation = degree
    @setScaleAndRotation()
    return

  scale:(x, y) ->
    @scaleX *= x
    @scaleY *= y
    @setScaleAndRotation()
    return

  scaleTo:(x, y) ->
    @scaleX = x
    @scaleY = y
    @setScaleAndRotation()
    return

  setScaleAndRotation: ->
    radian = @rotation * Math.PI / 180
    if radian is 0
      c = 1
      s = 0
    else
      c = Math.cos(radian)
      s = Math.sin(radian)
    @matrix.scaleX = @scaleX * c
    @matrix.skew0 = @scaleY * -s
    @matrix.skew1 = @scaleX * s
    @matrix.scaleY = @scaleY * c
    @hasMatrix = true
    @lwf.setPropertyDirty()
    return

  setMatrix:(m, @scaleX = 1, @scaleY = 1, @rotation = 0) ->
    @matrix.set(m)
    @hasMatrix = true
    @lwf.setPropertyDirty()
    return

  setAlpha:(alpha) ->
    @colorTransform.multi.alpha = alpha
    @hasColorTransform = true
    @lwf.setPropertyDirty()
    return

  setColorTransform:(c) ->
    @colorTransform.set(c)
    @hasColorTransform = true
    @lwf.setPropertyDirty()
    return

  setRenderingOffset:(rOffset) ->
    @renderingOffset = rOffset
    @hasRenderingOffset = true
    return

  clearRenderingOffset: ->
    @renderingOffset = Number.MIN_VALUE
    @hasRenderingOffset = false
    return
