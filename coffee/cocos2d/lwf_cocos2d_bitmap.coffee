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

class Cocos2dBitmapContext
  constructor:(@factory, @data, bitmapEx) ->
    @fragment = data.textureFragments[bitmapEx.textureFragmentId]
    texture = data.textures[@fragment.textureId]
    filename = @factory.cache[texture.filename]
    @texture2d = cc.TextureCache.getInstance().textureForKey(filename)
    if typeof @texture2d.getPixelsWide isnt "undefined"
      imageScale = @texture2d.getPixelsWide() / texture.width
    else
      imageScale = @texture2d.width / texture.width
    @scale = 1 / (texture.scale * imageScale)
    if @fragment.rotated
      w = @fragment.h
      h = @fragment.w
    else
      w = @fragment.w
      h = @fragment.h
    @imageHeight = @fragment.h * imageScale
    @rect = cc.rect(@fragment.u, @fragment.v, w, h)

  destruct: ->

class Cocos2dBitmapRenderer
  constructor:(@context) ->
    fragment = @context.fragment
    @matrix = new Matrix(0, 0, 0, 0, 0, 0)
    @matrixForAtlas = new Matrix() if fragment.rotated or
      fragment.x isnt 0 or fragment.y isnt 0 or @context.scale isnt 1

    @bitmap = cc.LWFBitmap.lwfBitmapWithTexture(
      @context.texture2d, @context.rect)
    @visible = true
    @alpha = 1
    @z = -1

  destruct: ->
    @bitmap.removeFromParentAndCleanup(true)

  render:(m, c, renderingIndex, renderingCount, visible) ->
    z = renderingIndex
    if @z isnt z
      if @z is -1
        @context.factory.lwfNode.addChild(@bitmap, z)
      else
        @bitmap.getParent().reorderChild(@bitmap, z)
      @z = z

    if @matrix.setWithComparing(m)
      m = @matrix
      fragment = @context.fragment
      x = fragment.x
      y = fragment.y
      scale = @context.scale
      if fragment.rotated
        m = Utility.rotateMatrix(@matrixForAtlas, m, scale, x, y + fragment.h)
      else if scale isnt 1 or x isnt 0 or y isnt 0
        m = Utility.scaleMatrix(@matrixForAtlas, m, scale, x, y)
      @bitmap.setMatrix(m)

    if @alpha isnt c.multi.alpha or @visible isnt visible
      @alpha = c.multi.alpha
      @visible = visible
      @bitmap.setOpacity(if @visible then @alpha * 255 else 0)

