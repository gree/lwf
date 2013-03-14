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
      continue if bitmap.textureFragmentId is -1
      bitmapEx = new Format.BitmapEx()
      bitmapEx.matrixId = bitmap.matrixId
      bitmapEx.textureFragmentId = bitmap.textureFragmentId
      bitmapEx.u = 0
      bitmapEx.v = 0
      bitmapEx.w = 1
      bitmapEx.h = 1
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
    style.position = "absolute"
    style.overflow = "hidden"
    style.webkitUserSelect = "none"
    if @use3D
      style.webkitTransform = "translateZ(0)"
      style.webkitTransformStyle = "preserve-3d"

    [w, h] = @getStageSize()
    if w is 0 and h is 0
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
      style.zIndex = renderer.zIndex
      style.opacity = renderer.alpha
      m = command.matrix
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

    @commands = []
    return

  setBlendMode:(blendMode) ->

  setMaskMode:(maskMode) ->

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

  fitText:(ctx, line, words, lineStart, imin, imax) ->
    return if imax < imin
    imid = ((imin + imax) / 2) >> 0
    start = if lineStart is 0 then 0 else words[lineStart - 1]
    str = line.slice(start, words[imid])
    w = ctx.measureText(str).width
    if w <= @maxWidth
      if w > @lineWidth
        @index = imid
        @lineWidth = w
      @fitText(ctx, line, words, lineStart, imid + 1, imax)
    if w >= @lineWidth
      @fitText(ctx, line, words, lineStart, imin, imid - 1)

  adjustText:(lines, ctx, @maxWidth) ->
    newlines = []
    for line in lines
      words = line.split(" ")
      line = ""
      for word in words
        if word.length > 0
          line += " " if line.length > 0
          line += word

      if ctx.measureText(line).width > @maxWidth
        words = []
        prev = 0
        for i in [1...line.length]
          c = line.charCodeAt(i)
          words.push(i) if c is 0x20 or c >= 0x80 or prev >= 0x80
          prev = c
        words.push(line.length)

        imin = 0
        imax = words.length - 1
        loop
          @index = null
          @lineWidth = 0
          @fitText(ctx, line, words, imin, imin, imax)
          break if @index is null
          start = if imin is 0 then 0 else words[imin - 1]
          ++start if line.charCodeAt(start) is 0x20
          to = words[@index]
          str = line.slice(start, to)
          if @index is imax
            line = str
            break
          newlines.push(str)
          start = to + if line.charCodeAt(to) is 0x20 then 1 else 0
          str = line.slice(start)
          if ctx.measureText(str).width <= @maxWidth
            line = str
            break
          imin = @index + 1

      newlines.push(line)
    return newlines

  renderText:(canvas, \
      ctx, str, maxWidth, scale, context, fontHeight, offsetX, textColor) ->
    lines = @adjustText(str.split("\n"), ctx, maxWidth)

    property = context.textProperty
    leading = property.leading * scale

    switch (property.align & Align.VERTICAL_MASK)
      when Align.VERTICAL_BOTTOM
        len = lines.length
        h = (fontHeight * len + leading * (len - 1)) * 96 / 72
        offsetY = canvas.height - h
      when Align.VERTICAL_MIDDLE
        len = lines.length + 1
        h = (fontHeight * len + leading * (len - 1)) * 96 / 72
        offsetY = (canvas.height - h) / 2
      else
        offsetY = 0
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    ctx.fillStyle = "rgb(#{textColor.red},#{textColor.green},#{textColor.blue})"

    useStroke = false
    if context.strokeColor?
      ctx.strokeStyle = context.factory.convertRGB(context.strokeColor)
      ctx.lineWidth = property.strokeWidth * scale
      useStroke = true

    if context.shadowColor?
      shadowColor = context.factory.convertRGB(context.shadowColor)
      ctx.shadowOffsetX = property.shadowOffsetX * scale
      ctx.shadowOffsetY = property.shadowOffsetY * scale
      ctx.shadowBlur = property.shadowBlur * scale

    for i in [0...lines.length]
      line = lines[i]
      x = offsetX * scale
      y = fontHeight + offsetY
      y += (fontHeight + leading) * i * 96 / 72 if i > 0
      ctx.shadowColor = shadowColor if context.shadowColor?
      ctx.fillText(line, x, y)
      if useStroke
        ctx.shadowColor = "rgba(0, 0, 0, 0)" if context.shadowColor?
        ctx.strokeText(line, x, y)

