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

class WebkitCSSBitmapContext
  constructor:(@factory, @data, bitmapEx) ->
    @fragment = data.textureFragments[bitmapEx.textureFragmentId]
    texture = data.textures[@fragment.textureId]
    image = @factory.cache[texture.filename]
    imageScale = image.width / texture.width
    @scale = 1 / (texture.scale * imageScale)

    x = @fragment.x
    y = @fragment.y
    u = @fragment.u
    v = @fragment.v
    w = @fragment.w
    h = @fragment.h

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
    su = Math.round(u * imageScale)
    sv = Math.round(v * imageScale)
    if @fragment.rotated
      sw = Math.round(h * imageScale)
      sh = Math.round(w * imageScale)
    else
      sw = Math.round(w * imageScale)
      sh = Math.round(h * imageScale)
    @h = Math.round(h * imageScale)
    @node = document.createElement("div")
    if image.src?
      @node.style.background = "url(#{image.src}) transparent"
    else
      @node.style.background = "-webkit-canvas(#{image.name}) transparent"
    @node.style.backgroundPosition = "-#{su}px -#{sv}px"
    @node.style.width = "#{sw}px"
    @node.style.height = "#{sh}px"
    @node.style.display = "block"
    @node.style.pointerEvents = "none"
    @node.style.position = "absolute"
    @node.style.webkitTransformOrigin = "0px 0px"
    @width = sw
    @height = sh
    @cache = []

  destruct: ->
    for node in @cache
      if node.mask?
        node.mask.parentNode.removeChild(node.mask)
      node.parentNode.removeChild(node)
    return

class WebkitCSSBitmapRenderer
  constructor:(@context) ->
    @matrix = new Matrix(0, 0, 0, 0, 0, 0)
    @alpha = -1
    @zIndex = -1
    @visible = true

    fragment = @context.fragment
    @matrixForAtlas = new Matrix() if fragment.rotated or
      @context.x isnt 0 or @context.y isnt 0 or @context.scale isnt 1
    @cmd = {}

  destructor: ->
    if @node?
      @node.style.visibility = "hidden"
      @context.cache.push(@node)
    return

  destruct: ->
    @context.factory.destructRenderer(@)
    return

  render:(m, c, renderingIndex, renderingCount, visible) ->
    if @visible is visible
      return if visible is false
    else
      @visible = visible
      if visible is false
        @node.style.visibility = "hidden" if @node?
        return
      else
        @node.style.visibility = "visible" if @node?

    unless @node?
      if @context.cache.length > 0
        @node = @context.cache.pop()
        @node.style.visibility = "visible"
      else
        @node = @context.node.cloneNode(true)
        @context.factory.stage.appendChild(@node)

    matrixChanged = @matrix.setWithComparing(m)
    if matrixChanged
      m = @matrix
      fragment = @context.fragment
      x = @context.x
      y = @context.y
      scale = @context.scale
      if fragment.rotated
        m = Utility.rotateMatrix(@matrixForAtlas, m, scale, x, y + @context.h)
      else if scale isnt 1 or x isnt 0 or y isnt 0
        m = Utility.scaleMatrix(@matrixForAtlas, m, scale, x, y)
    else
      if @matrixForAtlas?
        m = @matrixForAtlas
      else
        m = @matrix

    maskMode = @context.factory.maskMode

    return if !matrixChanged and
      @alpha is c.multi.alpha and
      @zIndex is renderingIndex and
      maskMode is "normal" and
      @node.parentNode is @context.factory.stage

    @alpha = c.multi.alpha
    @zIndex = renderingIndex
    cmd = @cmd
    cmd.isBitmap = true
    cmd.renderer = @
    cmd.matrix = m
    cmd.maskMode = maskMode
    @context.factory.addCommand(renderingIndex, cmd)
    return

