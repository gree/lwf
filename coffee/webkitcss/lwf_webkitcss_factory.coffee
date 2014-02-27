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

class WebKitCSSRenderCommand
  constructor: ->
    @renderCount = 0
    @renderingIndex = 0
    @isBitmap = false
    @renderer = null
    @matrix = null
    @maskMode = 0

class WebkitCSSRendererFactory
  constructor:(data, @resourceCache, @cache, \
      @stage, @textInSubpixel, @use3D, @recycleTextCanvas, @quirkyClearRect) ->
    @needsRenderForInactive = true
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
      bitmapEx.attribute = 0
      @bitmapContexts.push new WebkitCSSBitmapContext(@, data, bitmapEx)

    @bitmapExContexts = []
    for bitmapEx in data.bitmapExs
      continue if bitmapEx.textureFragmentId is -1
      @bitmapExContexts.push new WebkitCSSBitmapContext(@, data, bitmapEx)

    @textContexts = []
    for text in data.texts
      @textContexts.push new WebkitCSSTextContext(@, data, text)

    style = @stage.style
    style.display = "block"
    style.overflow = "hidden"
    style.webkitUserSelect = "none"
    if @use3D
      style.webkitTransform = "translateZ(0)"
      style.webkitTransformStyle = "preserve-3d"

    [w, h] = @getStageSize()
    if w is 0 and h is 0
      style.width = "#{data.header.width}px"
      style.height = "#{data.header.height}px"

    @initCommands()
    @destructedRenderers = []

  initCommands: ->
    if !@commands? or @commandsCount < @commands.length * 0.75
      @commands = []
    @commandsCount = 0
    @subCommands = null
    return

  isMask:(cmd) ->
    switch cmd.maskMode
      when "erase", "mask"
        return true
    return false

  isLayer:(cmd) ->
    cmd.maskMode is "layer"

  addCommand:(rIndex, cmd) ->
    cmd.renderCount = @lwf.renderCount
    cmd.renderingIndex = rIndex
    if @isMask(cmd)
      if @subCommands?
        @subCommands[rIndex] = cmd
    else
      if @isLayer(cmd) and @commandMaskMode isnt cmd.maskMode
        cmd.subCommands = []
        @subCommands = cmd.subCommands
      @commands[rIndex] = cmd
      @commandsCount++
    @commandMaskMode = cmd.maskMode
    return

  addCommandToParent:(lwf) ->
    parent = lwf.parent
    parent = parent.parent while parent.parent?
    f = parent.lwf.rendererFactory
    renderCount = lwf.renderCount
    for rIndex in [0...@commands.length]
      cmd = @commands[rIndex]
      continue if !cmd? or cmd.renderingIndex isnt rIndex or
        cmd.renderCount isnt renderCount
      subCommands = cmd.subCommands
      cmd.subCommands = null
      f.addCommand(rIndex, cmd)
      if subCommands?
        for srIndex in [0...subCommands.length]
          scmd = subCommands[srIndex]
          continue if !scmd? or scmd.renderingIndex isnt srIndex or
            scmd.renderCount isnt renderCount
          f.addCommand(srIndex, scmd)
    @initCommands()
    return

  destruct: ->
    @callRendererDestructor() if @destructedRenderers?
    context.destruct() for context in @bitmapContexts
    context.destruct() for context in @bitmapExContexts
    context.destruct() for context in @textContexts
    return

  init:(lwf) ->
    @lwf = lwf
    lwf.stage = @stage
    lwf.resourceCache = @resourceCache
    return if @setupedDomElementConstructor
    @setupedDomElementConstructor = true
    for progObj in lwf.data.programObjects
      name = lwf.data.strings[progObj.stringId]
      m = name.match(/^DOM_(.*)/)
      if m?
        domName = m[1]
        do (domName) =>
          lwf.setProgramObjectConstructor(name, (lwf_, objId, w, h) =>
            ctor = @resourceCache.domElementConstructor
            return null unless ctor?
            domElement = ctor(lwf_, domName, w, h)
            return null unless domElement?
            new WebkitCSSDomElementRenderer(@, domElement)
          )

  destructRenderer:(renderer) ->
    @destructedRenderers.push(renderer)
    return

  callRendererDestructor: ->
    for renderer in @destructedRenderers
      renderer.destructor()
    @destructedRenderers = []
    return

  beginRender:(lwf) ->
    if lwf.parent?
      f = lwf.parent.lwf.rendererFactory
      @blendMode = f.blendMode if f.blendMode?
      @maskMode = f.maskMode

    @callRendererDestructor() if @destructedRenderers?
    return

  render:(cmd) ->
    renderer = cmd.renderer
    node = renderer.node
    style = node.style
    style.zIndex = renderer.zIndex
    m = cmd.matrix

    switch cmd.maskMode
      when "mask"
        @renderMasked = true
        style.opacity = 0
        if @renderMaskMode isnt "mask"
          if node.mask?
            @mask = node.mask
            style = @mask.style
          else
            @mask = node.mask = document.createElement("div")
            style = @mask.style
            style.display = "block"
            style.position = "absolute"
            style.overflow = "hidden"
            style.webkitUserSelect = "none"
            style.webkitTransformOrigin = "0px 0px"
            @stage.appendChild(@mask)
          style.width = node.style.width
          style.height = node.style.height
          unless @maskMatrix?
            @maskMatrix = new Matrix()
            @maskedMatrix = new Matrix()
          Utility.invertMatrix(@maskMatrix, m)
        else
          return

      when "layer"
        if @renderMasked
          if @renderMaskMode isnt cmd.maskMode
            @mask.style.zIndex = renderer.zIndex
          if node.parentNode isnt @mask
            node.parentNode.removeChild(node)
            @mask.appendChild(node)
          m = Utility.calcMatrix(@maskedMatrix, @maskMatrix, m)
        else
          if node.parentNode isnt @stage
            node.parentNode.removeChild(node)
            @stage.appendChild(node)

      else
        if node.parentNode isnt @stage
          node.parentNode.removeChild(node)
          @stage.appendChild(node)
    @renderMaskMode = cmd.maskMode

    style.opacity = renderer.alpha
    scaleX = m.scaleX.toFixed(12)
    scaleY = m.scaleY.toFixed(12)
    skew1 = m.skew1.toFixed(12)
    skew0 = m.skew0.toFixed(12)
    translateX = m.translateX.toFixed(12)
    translateY = m.translateY.toFixed(12)
    if @use3D
      style.webkitTransform = "matrix3d(" +
        "#{scaleX},#{skew1},0,0," +
        "#{skew0},#{scaleY},0,0," +
        "0,0,1,0," +
        "#{translateX},#{translateY},0,1)"
    else
      style.webkitTransform = "matrix(" +
        "#{scaleX},#{skew1},#{skew0},#{scaleY},#{translateX},#{translateY})"
    return

  endRender:(lwf) ->
    if lwf.parent?
      @addCommandToParent(lwf)
      @callRendererDestructor() if @destructedRenderers?
      return

    @renderMaskMode = "normal"
    @renderMasked = false
    renderCount = lwf.renderCount
    for rIndex in [0...@commands.length]
      cmd = @commands[rIndex]
      continue if !cmd? or cmd.renderingIndex isnt rIndex or
        cmd.renderCount isnt renderCount
      if cmd.subCommands?
        for srIndex in [0...cmd.subCommands.length]
          scmd = cmd.subCommands[srIndex]
          continue if !scmd? or scmd.renderingIndex isnt srIndex or
            scmd.renderCount isnt renderCount
          @render(scmd)
      @render(cmd)

    @initCommands()

    @callRendererDestructor() if @destructedRenderers?
    return

  setBlendMode:(blendMode) ->

  setMaskMode:(@maskMode) ->

  constructBitmap:(lwf, objectId, bitmap) ->
    context = @bitmapContexts[objectId]
    new WebkitCSSBitmapRenderer(context) if context

  constructBitmapEx:(lwf, objectId, bitmapEx) ->
    context = @bitmapExContexts[objectId]
    new WebkitCSSBitmapRenderer(context) if context

  constructText:(lwf, objectId, text) ->
    context = @textContexts[objectId]
    new WebkitCSSTextRenderer(lwf, context, text) if context

  constructParticle:(lwf, objectId, particle) ->
    ctor = @resourceCache.particleConstructor
    particleData = lwf.data.particleDatas[particle.particleDataId]
    ctor(lwf, lwf.data.strings[particleData.stringId]) if ctor?

  convertColor:(d, c, t) ->
    Utility.calcColor(d, c, t)
    d.red = Math.round(d.red * 255)
    d.green = Math.round(d.green * 255)
    d.blue = Math.round(d.blue * 255)
    return

  convertRGB:(c) ->
    r = Math.round(c.red * 255)
    g = Math.round(c.green * 255)
    b = Math.round(c.blue * 255)
    return "rgb(#{r},#{g},#{b})"

  getStageSize: ->
    r = @stage.getBoundingClientRect()
    return [r.width, r.height]

  fitForHeight:(lwf) ->
    [w, h] = @getStageSize()
    lwf.fitForHeight(w, h) if h isnt 0 and h isnt lwf.data.header.height
    return

  fitForWidth:(lwf) ->
    [w, h] = @getStageSize()
    lwf.fitForWidth(w, h) if w isnt 0 and w isnt lwf.data.header.width
    return

  scaleForHeight:(lwf) ->
    [w, h] = @getStageSize()
    lwf.scaleForHeight(w, h) if h isnt 0 and h isnt lwf.data.header.height
    return

  scaleForWidth:(lwf) ->
    [w, h] = @getStageSize()
    lwf.scaleForWidth(w, h) if w isnt 0 and w isnt lwf.data.header.width
    return

  parseBackgroundColor:(v) ->
    if typeof v is "number"
      bgColor = v
    else if typeof v is "string"
      bgColor = parseInt(v, 16)
    else if v instanceof LWF
      lwf = v
      bgColor = lwf.data.header.backgroundColor
      bgColor |= 0xff << 24
    else
      return [255, 255, 255, 255]
    a = ((bgColor >> 24) & 0xff)
    r = ((bgColor >> 16) & 0xff)
    g = ((bgColor >>  8) & 0xff)
    b = ((bgColor >>  0) & 0xff)
    return [r, g, b, a]

  setBackgroundColor:(v) ->
    [r, g, b, a] = @parseBackgroundColor(v)
    @stage.style.backgroundColor = "rgba(#{r},#{g},#{b},#{a / 255})"
    return

  clearCanvasRect:(canvas, ctx) ->
    ctx.clearRect(0, 0, canvas.width + 1, canvas.height + 1)
    canvas.width = canvas.width if @quirkyClearRect
    return

