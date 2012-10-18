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

class CanvasRendererFactory extends WebkitCSSRendererFactory
  constructor:(data, \
      @resourceCache, @cache, @stage, @textInSubpixel, @needsClear) ->
    @bitmapContexts = []
    for bitmap in data.bitmaps
      continue if bitmap.textureFragmentId == -1
      bitmapEx = new Format.BitmapEx()
      bitmapEx.matrixId = bitmap.matrixId
      bitmapEx.textureFragmentId = bitmap.textureFragmentId
      bitmapEx.u = 0
      bitmapEx.v = 0
      bitmapEx.w = 1
      bitmapEx.h = 1
      @bitmapContexts.push new CanvasBitmapContext(this, data, bitmapEx)

    @bitmapExContexts = []
    for bitmapEx in data.bitmapExs
      continue if bitmapEx.textureFragmentId == -1
      @bitmapExContexts.push new CanvasBitmapContext(this, data, bitmapEx)

    @textContexts = []
    for text in data.texts
      @textContexts.push new CanvasTextContext(this, data, text)

    @stage.style.webkitUserSelect = "none"
    @stageContext = @stage.getContext("2d")
    if @stage.width is 0 and @stage.height is 0
      @stage.width = data.header.width
      @stage.height = data.header.height

    @commands = {}

  destruct: ->
    context.destruct() for context in @bitmapContexts
    context.destruct() for context in @bitmapExContexts
    context.destruct() for context in @textContexts
    return

  beginRender:(lwf) ->

  endRender:(lwf) ->
    ctx = @stageContext
    if lwf.parent?
      commands = lwf.parent.lwf.rendererFactory.commands
      for rIndex, cmd of @commands
        commands[rIndex] = cmd
      @commands = {}
      return

    if @needsClear
      ctx.globalAlpha = 1
      ctx.setTransform(1, 0, 0, 1, 0, 0)
      if @clearColor?
        ctx.fillStyle = @clearColor
        ctx.fillRect(0, 0, @stage.width, @stage.height)
      else
        ctx.clearRect(0, 0, @stage.width, @stage.height)

    for rIndex, cmd of @commands
      ctx.globalAlpha = cmd.alpha
      m = cmd.matrix
      ctx.setTransform(
        m.scaleX, m.skew1, m.skew0, m.scaleY, m.translateX, m.translateY)
      u = cmd.u
      v = cmd.v
      w = cmd.w
      h = cmd.h
      ctx.drawImage(cmd.image, u, v, w, h, 0, 0, w, h)
    @commands = {}
    return

  constructBitmap:(lwf, objectId, bitmap) ->
    context = @bitmapContexts[objectId]
    new CanvasBitmapRenderer(context) if context

  constructBitmapEx:(lwf, objectId, bitmapEx) ->
    context = @bitmapExContexts[objectId]
    new CanvasBitmapRenderer(context) if context

  constructText:(lwf, objectId, text) ->
    context = @textContexts[objectId]
    new CanvasTextRenderer(lwf, context, text) if context

  constructParticle:(lwf, objectId, particle) ->
    ctor = @resourceCache.particleConstructor
    particleData = lwf.data.particleDatas[particle.particleDataId]
    ctor(lwf, lwf.data.strings[particleData.stringId]) if ctor?

  getStageSize: ->
    return [@stage.width, @stage.height]

  setBackgroundColor:(lwf) ->
    bgColor = lwf.data.header.backgroundColor
    r = (bgColor >> 16) & 0xff
    g = (bgColor >>  8) & 0xff
    b = (bgColor >>  0) & 0xff
    @clearColor = "rgb(#{r}, #{g}, #{b})"
    return
