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

class CanvasBitmapContext
  constructor:(@factory, @data, bitmapEx) ->
    @fragment = @data.textureFragments[bitmapEx.textureFragmentId]
    texture = @data.textures[@fragment.textureId]
    @image = @factory.cache[texture.filename]
    imageWidth = @image.width
    withPadding = if texture.filename.match(/_withpadding/) then true else false
    imageWidth -= 2 if withPadding
    imageScale = imageWidth / texture.width
    @scale = 1 / (texture.scale * imageScale)

    repeat = null
    if (bitmapEx.attribute & Format.BitmapEx.Attribute.REPEAT_S) isnt 0
      repeat = "repeat-x"
    if (bitmapEx.attribute & Format.BitmapEx.Attribute.REPEAT_T) isnt 0
      repeat = if repeat? then "repeat" else "repeat-y"
    @pattern = if repeat? then \
      @factory.stageContext.createPattern(@image, repeat) else null

    x = @fragment.x
    y = @fragment.y
    u = @fragment.u
    v = @fragment.v
    w = @fragment.w
    h = @fragment.h

    if withPadding
      x -= 1
      y -= 1
      w += 2
      h += 2

    bu = bitmapEx.u * w
    bv = bitmapEx.v * h
    bw = bitmapEx.w
    bh = bitmapEx.h

    x += bu
    y += bv
    u += bu
    v += bv
    w *= bw
    h *= bh

    @x = Math.round(x * imageScale)
    @y = Math.round(y * imageScale)
    @u = Math.round(u * imageScale)
    @v = Math.round(v * imageScale)
    if @fragment.rotated
      @w = Math.round(h * imageScale)
      @h = Math.round(w * imageScale)
    else
      @w = Math.round(w * imageScale)
      @h = Math.round(h * imageScale)
    @w = @image.width - @u if @u + @w > @image.width
    @h = @image.height - @v if @v + @h > @image.height
    @imageHeight = h * imageScale

  destruct: ->

class CanvasBitmapRenderer
  constructor:(@context) ->
    fragment = @context.fragment
    @matrix = new Matrix(0, 0, 0, 0, 0, 0)
    @matrixForAtlas = new Matrix() if fragment.rotated or
      @context.x isnt 0 or @context.y isnt 0 or @context.scale isnt 1
    @cmd = {}

  destruct: ->

  render:(m, c, renderingIndex, renderingCount, visible) ->
    return if !visible or c.multi.alpha is 0

    if @matrix.setWithComparing(m)
      m = @matrix
      fragment = @context.fragment
      x = @context.x
      y = @context.y
      scale = @context.scale
      if fragment.rotated
        m = Utility.rotateMatrix(
          @matrixForAtlas, m, scale, x, y + @context.imageHeight)
      else if scale isnt 1 or x isnt 0 or y isnt 0
        m = Utility.scaleMatrix(@matrixForAtlas, m, scale, x, y)
    else
      m = @matrixForAtlas if @matrixForAtlas?

    @alpha = c.multi.alpha

    fragment = @context.fragment
    cmd = @cmd
    cmd.alpha = @alpha
    cmd.blendMode = @context.factory.blendMode
    cmd.maskMode = @context.factory.maskMode
    cmd.matrix = m
    cmd.image = @context.image
    cmd.pattern = @context.pattern
    cmd.u = @context.u
    cmd.v = @context.v
    cmd.w = @context.w
    cmd.h = @context.h
    @context.factory.addCommand(renderingIndex, cmd)
    return

