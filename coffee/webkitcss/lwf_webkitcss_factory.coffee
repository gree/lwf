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

class WebkitCSSRendererFactory
  constructor:(data, @resourceCache, @cache, @stage, @textInSubpixel, @use3D) ->
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
      @bitmapContexts.push new WebkitCSSBitmapContext(this, data, bitmapEx)

    @bitmapExContexts = []
    for bitmapEx in data.bitmapExs
      continue if bitmapEx.textureFragmentId == -1
      @bitmapExContexts.push new WebkitCSSBitmapContext(this, data, bitmapEx)

    @textContexts = []
    for text in data.texts
      @textContexts.push new WebkitCSSTextContext(this, data, text)

    style = @stage.style
    style.display = "block"
    style.position = "absolute"
    style.overflow = "hidden"
    style.webkitUserSelect = "none"
    if @use3D
      style.webkitTransform = "translateZ(0)"
      style.webkitTransformStyle = "preserve-3d"

    computedStyle = window.getComputedStyle(@stage, "")
    h = computedStyle.getPropertyValue("height")
    w = computedStyle.getPropertyValue("width")
    if h is "0px" and w is "0px"
      style.width = "#{data.header.width}px"
      style.height = "#{data.header.height}px"

    @commands = []

  destruct: ->
    context.destruct() for context in @bitmapContexts
    context.destruct() for context in @bitmapExContexts
    context.destruct() for context in @textContexts
    return

  init:(lwf) ->
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

  beginRender:(lwf) ->

  endRender:(lwf) ->
    for command in @commands
      renderer = command.renderer
      style = renderer.node.style
      unless command.isBitmap
        c = renderer.color
        style.color = "rgb(#{c.red},#{c.green},#{c.blue})"
      style.zIndex = renderer.zIndex
      style.opacity = renderer.alpha
      m = command.matrix
      if @use3D
        style.webkitTransform = "matrix3d(" +
          "#{m.scaleX},#{m.skew1},0,0," +
          "#{m.skew0},#{m.scaleY},0,0," +
          "0,0,1,0," +
          "#{m.translateX},#{m.translateY},0,1)"
      else
        style.webkitTransform = "matrix(" +
          "#{m.scaleX},#{m.skew1},#{m.skew0},#{m.scaleY}," +
          "#{m.translateX},#{m.translateY})"

    @commands = []
    return

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
    computedStyle = window.getComputedStyle(@stage, "")
    w = parseInt(computedStyle.getPropertyValue("width").replace("px", ""), 10)
    h = parseInt(computedStyle.getPropertyValue("height").replace("px", ""), 10)
    [w, h]

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

  setBackgroundColor:(lwf) ->
    bgColor = lwf.backgroundColor
    r = (bgColor >> 16) & 0xff
    g = (bgColor >>  8) & 0xff
    b = (bgColor >>  0) & 0xff
    @stage.style.backgroundColor = "rgb(#{r},#{g},#{b})"
    return
