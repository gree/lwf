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
  constructor:(data, @resourceCache, \
      @cache, @stage, @textInSubpixel, @needsClear, @quirkyClearRect) ->
    @blendMode = "normal"
    @maskMode = "normal"

    @stage.style.webkitUserSelect = "none"
    @stage.style.webkitTransform = "translateZ(0)"
    @stageContext = @stage.getContext("2d")
    if @stage.width is 0 and @stage.height is 0
      @stage.width = data.header.width
      @stage.height = data.header.height

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
      bitmapEx.attribute = 0
      @bitmapContexts.push new CanvasBitmapContext(@, data, bitmapEx)

    @bitmapExContexts = []
    for bitmapEx in data.bitmapExs
      continue if bitmapEx.textureFragmentId is -1
      @bitmapExContexts.push new CanvasBitmapContext(@, data, bitmapEx)

    @textContexts = []
    for text in data.texts
      @textContexts.push new CanvasTextContext(@, data, text)

    @initCommands()

  destruct: ->
    context.destruct() for context in @bitmapContexts
    context.destruct() for context in @bitmapExContexts
    context.destruct() for context in @textContexts
    return

  renderMask: ->
    ctx = @maskCanvas.getContext('2d')
    ctx.globalAlpha = 1
    ctx.globalCompositeOperation = @maskComposition
    ctx.setTransform(1, 0, 0, 1, 0, 0)
    ctx.drawImage(@layerCanvas,
      0, 0, @layerCanvas.width, @layerCanvas.height,
      0, 0, @layerCanvas.width, @layerCanvas.height)

    ctx = @stageContext
    ctx.globalAlpha = 1
    ctx.globalCompositeOperation = "source-over"
    ctx.setTransform(1, 0, 0, 1, 0, 0)
    ctx.drawImage(@maskCanvas,
      0, 0, @maskCanvas.width, @maskCanvas.height,
      0, 0, @maskCanvas.width, @maskCanvas.height)
    return

  render:(ctx, cmd) ->
    if @renderMaskMode isnt cmd.maskMode
      switch cmd.maskMode
        when "erase", "mask"
          @renderMask() if @renderMaskMode is "layer" and @renderMasked
          @renderMasked = true
          @maskComposition =
            if cmd.maskMode is "erase" then "source-out" else "source-in"
          unless @maskCanvas?
            @maskCanvas = document.createElement('canvas')
            @maskCanvas.width = @stage.width
            @maskCanvas.height = @stage.height
            cleared = true
          else
            cleared = false
          ctx = @maskCanvas.getContext('2d')
          ctx.globalAlpha = 1
          ctx.globalCompositeOperation = "source-over"
          @renderBlendMode = "normal"
          unless cleared
            ctx.setTransform(1, 0, 0, 1, 0, 0)
            @clearCanvasRect(@stage, ctx)
        when "layer"
          if @renderMasked
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
            @renderBlendMode = "normal"
            unless cleared
              ctx.setTransform(1, 0, 0, 1, 0, 0)
              @clearCanvasRect(@stage, ctx)
          else
            ctx = @stageContext
            ctx.globalAlpha = 1
            ctx.globalCompositeOperation = "source-over"
            @renderBlendMode = "normal"
        when "normal"
          ctx = @stageContext
          ctx.globalAlpha = 1
          ctx.globalCompositeOperation = "source-over"
          @renderBlendMode = "normal"
          @renderMask() if @renderMaskMode is "layer" and @renderMasked
      @renderMaskMode = cmd.maskMode
    if @renderBlendMode isnt cmd.blendMode
      @renderBlendMode = cmd.blendMode
      switch @renderBlendMode
        when "add"
          ctx.globalCompositeOperation = "lighter"
        when "normal"
          ctx.globalCompositeOperation = "source-over"
    ctx.globalAlpha = cmd.alpha
    m = cmd.matrix
    ctx.setTransform(
      m.scaleX, m.skew1, m.skew0, m.scaleY, m.translateX, m.translateY)
    u = cmd.u
    v = cmd.v
    w = cmd.w
    h = cmd.h
    if cmd.pattern? and (w > cmd.image.width or h > cmd.image.height)
      ctx.translate(-u, -v)
      ctx.rect(u, v, w, h)
      ctx.fillStyle = cmd.pattern
      ctx.fill()
    else
      ctx.drawImage(cmd.image, u, v, w, h, 0, 0, w, h)
    return ctx

  endRender:(lwf) ->
    ctx = @stageContext
    if lwf.parent?
      @addCommandToParent(lwf)
      return

    if @needsClear
      ctx.setTransform(1, 0, 0, 1, 0, 0)
      if @clearColor?
        if @clearColor[3] is 'a'
          @clearCanvasRect(@stage, ctx)
        ctx.fillStyle = @clearColor
        ctx.fillRect(0, 0, @stage.width, @stage.height)
      else
        @clearCanvasRect(@stage, ctx)

    @renderBlendMode = "normal"
    @renderMaskMode = "normal"
    @renderMasked = false
    for rIndex in @commandsKeys
      cmd = @commands[rIndex]
      if cmd.subCommandsKeys?
        for srIndex in cmd.subCommandsKeys
          scmd = cmd.subCommands[srIndex]
          ctx = @render(ctx, scmd)
      ctx = @render(ctx, cmd)

    if @renderMaskMode is "layer" and @renderMasked
      @renderMask()
    else
      ctx.globalAlpha = 1
      ctx.globalCompositeOperation = "source-over"

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

