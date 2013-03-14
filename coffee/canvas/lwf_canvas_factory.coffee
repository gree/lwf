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
    @blendMode = "normal"
    @maskMode = "normal"
    @bitmapContexts = []
    for bitmap in data.bitmaps
      continue if bitmap.textureFragmentId is -1
      bitmapEx = new Format.BitmapEx()
      bitmapEx.matrixId = bitmap.matrixId
      bitmapEx.textureFragmentId = bitmap.textureFragmentId
      bitmapEx.u = 0
      bitmapEx.v = 0
      bitmapEx.w = 1
      bitmapEx.h = 1
      @bitmapContexts.push new CanvasBitmapContext(@, data, bitmapEx)

    @bitmapExContexts = []
    for bitmapEx in data.bitmapExs
      continue if bitmapEx.textureFragmentId is -1
      @bitmapExContexts.push new CanvasBitmapContext(@, data, bitmapEx)

    @textContexts = []
    for text in data.texts
      @textContexts.push new CanvasTextContext(@, data, text)

    @stage.style.webkitUserSelect = "none"
    @stage.style.webkitTransform = "translateZ(0)"
    @stageContext = @stage.getContext("2d")
    if @stage.width is 0 and @stage.height is 0
      @stage.width = data.header.width
      @stage.height = data.header.height

    @initCommands()

  initCommands: ->
    @commands = {}
    @commandsKeys = Utility.newIntArray()

  addCommand:(rIndex, cmd) ->
    @commands[rIndex] = cmd
    Utility.insertIntArray(@commandsKeys, rIndex)
    return

  destruct: ->
    context.destruct() for context in @bitmapContexts
    context.destruct() for context in @bitmapExContexts
    context.destruct() for context in @textContexts
    return

  beginRender:(lwf) ->

  renderMask: ->
    ctx = @eraseCanvas.getContext('2d')
    ctx.globalAlpha = 1
    ctx.globalCompositeOperation = "source-out"
    ctx.setTransform(1, 0, 0, 1, 0, 0)
    ctx.drawImage(@layerCanvas, 0, 0)

    ctx = @stageContext
    ctx.globalAlpha = 1
    ctx.globalCompositeOperation = "source-over"
    ctx.setTransform(1, 0, 0, 1, 0, 0)
    ctx.drawImage(@eraseCanvas, 0, 0)

  endRender:(lwf) ->
    ctx = @stageContext
    if lwf.parent?
      f = lwf.parent.lwf.rendererFactory
      f.addCommand(parseInt(rIndex, 10), cmd) for rIndex, cmd of @commands
      @initCommands()
      return

    if @needsClear
      ctx.setTransform(1, 0, 0, 1, 0, 0)
      if @clearColor?
        if @clearColor[3] is 'a'
          ctx.clearRect(0, 0, @stage.width, @stage.height)
        ctx.fillStyle = @clearColor
        ctx.fillRect(0, 0, @stage.width, @stage.height)
      else
        ctx.clearRect(0, 0, @stage.width, @stage.height)

    blendMode = "normal"
    maskMode = "normal"
    for rIndex in @commandsKeys
      cmd = @commands[rIndex]
      ctx.globalAlpha = cmd.alpha
      if maskMode isnt cmd.maskMode
        switch cmd.maskMode
          when "erase"
            unless @eraseCanvas?
              @eraseCanvas = document.createElement('canvas')
              @eraseCanvas.width = @stage.width
              @eraseCanvas.height = @stage.height
              cleared = true
            else
              cleared = false
            ctx = @eraseCanvas.getContext('2d')
            ctx.globalAlpha = 1
            ctx.globalCompositeOperation = "source-over"
            blendMode = "normal"
            unless cleared
              ctx.setTransform(1, 0, 0, 1, 0, 0)
              ctx.clearRect(0, 0, @stage.width, @stage.height)
          when "layer"
            unless @layerCanvas?
              @layerCanvas = document.createElement('canvas')
              @layerCanvas.width = @stage.width
              @layerCanvas.height = @stage.height
              cleared = true
            else
              cleared = false
            ctx = @layerCanvas.getContext('2d')
            ctx.globalAlpha = 1
            ctx.globalCompositeOperation = "source-over"
            blendMode = "normal"
            unless cleared
              ctx.setTransform(1, 0, 0, 1, 0, 0)
              ctx.clearRect(0, 0, @stage.width, @stage.height)
          when "normal"
            ctx = @stageContext
            ctx.globalAlpha = 1
            ctx.globalCompositeOperation = "source-over"
            blendMode = "normal"
            @renderMask() if maskMode is "layer"
        maskMode = cmd.maskMode
      if blendMode isnt cmd.blendMode
        blendMode = cmd.blendMode
        switch blendMode
          when "add"
            ctx.globalCompositeOperation = "lighter"
          when "normal"
            ctx.globalCompositeOperation = "source-over"
      m = cmd.matrix
      ctx.setTransform(
        m.scaleX, m.skew1, m.skew0, m.scaleY, m.translateX, m.translateY)
      u = cmd.u
      v = cmd.v
      w = cmd.w
      h = cmd.h
      ctx.drawImage(cmd.image, u, v, w, h, 0, 0, w, h)
    ctx.globalAlpha = 1
    ctx.globalCompositeOperation = "source-over"
    @renderMask() if maskMode is "layer"
    @initCommands()
    return

  setBlendMode:(@blendMode) ->

  setMaskMode:(@maskMode) ->

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

  setBackgroundColor:(v) ->
    [r, g, b, a] = @parseBackgroundColor(v)
    @clearColor = "rgba(#{r},#{g},#{b},#{a / 255})"
    return

