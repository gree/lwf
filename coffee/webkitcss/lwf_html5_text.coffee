#
# Copyright (C) 2013 GREE, Inc.
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

class HTML5TextContext
  constructor:(@factory, @data, @text) ->
    @str = @data.strings[@text.stringId]
    @textProperty = @data.textProperties[@text.textPropertyId]
    font = @data.fonts[@textProperty.fontId]
    @textColor = @data.colors[@text.colorId]

    if @textProperty.strokeColorId isnt -1
      @strokeColor = @data.colors[@textProperty.strokeColorId]
    if @textProperty.shadowColorId isnt -1
      @shadowColor = @data.colors[@textProperty.shadowColorId]

    @fontName = "#{@data.strings[font.stringId]},sans-serif"
    @fontChanged = true
    @letterSpacing = font.letterSpacing + @textProperty.letterSpacing

  destruct: ->
    return

class HTML5TextRenderer
  constructor:(@lwf, @context, @textObject) ->
    @str = @textObject.parent[@textObject.name] ? @context.str
    @str = String(@str) if @str?
    @matrixForCheck = new Matrix(0, 0, 0, 0, 0, 0)
    @matrix = new Matrix()
    @color = new Color
    @textRendered = false
    @textScale = @lwf.textScale
    @currentShadowMarginY = 0
    @initCanvas()

  destruct: ->
    return

  measureText:(str) ->
    swidth = if str.length <= 1 then 0 else (str.length - 1) * @letterSpacing
    return @canvasContext.measureText(str).width + swidth

  fitText:(line, words, lineStart, imin, imax) ->
    return if imax < imin
    imid = ((imin + imax) / 2) >> 0
    start = if lineStart is 0 then 0 else words[lineStart - 1]
    str = line.slice(start, words[imid])
    w = @measureText(str)
    if w <= @maxWidth
      if w > @lineWidth
        @index = imid
        @lineWidth = w
      @fitText(line, words, lineStart, imid + 1, imax)
    if w >= @lineWidth
      @fitText(line, words, lineStart, imin, imid - 1)
    return

  adjustText:(lines) ->
    newlines = []
    for line in lines
      words = line.split(" ")
      line = ""
      for word in words
        if word.length > 0
          line += " " if line.length > 0
          line += word

      if @measureText(line) > @maxWidth
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
          @fitText(line, words, imin, imin, imax)
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
          if @measureText(str) <= @maxWidth
            line = str
            break
          imin = @index + 1

      newlines.push(line)
    return newlines

  renderLines:(ctx, lines, useStroke, shadowColor, offsetY) ->
    for i in [0...lines.length]
      line = lines[i]
      x = @offsetX * @lwf.textScale
      if @letterSpacing isnt 0
        switch (@context.textProperty.align & Align.ALIGN_MASK)
          when Align.RIGHT
            x -= @measureText(line)
          when Align.CENTER
            x -= @measureText(line) / 2
      y = offsetY + (@fontHeight + @leading) * i * 96 / 72
      if useStroke
        ctx.shadowColor = "rgba(0, 0, 0, 0)" if shadowColor?
        if @letterSpacing is 0
          ctx.strokeText(line, x, y)
        else
          offset = 0
          for j in [0...line.length]
            c = line[j]
            ctx.strokeText(c, x + offset, y)
            offset += @canvasContext.measureText(c).width + @letterSpacing
      ctx.shadowColor = shadowColor if shadowColor?
      if @letterSpacing is 0
        ctx.fillText(line, x, y)
      else
        offset = 0
        for j in [0...line.length]
          c = line[j]
          ctx.fillText(c, x + offset, y)
          offset += @canvasContext.measureText(c).width + @letterSpacing
    return

  renderText:(textColor) ->
    @textRendered = true

    context = @context
    canvas = @canvas
    ctx = @canvasContext
    lines = @adjustText(@str.split("\n"))

    property = context.textProperty
    useStroke = context.strokeColor?
    shadowColor = context.factory.convertRGB(context.shadowColor) if context.shadowColor?

    context.factory.clearCanvasRect(canvas, ctx)
    @initCanvasContext(ctx, textColor)

    offsetY = @fontHeight * 1.2
    switch (property.align & Align.VERTICAL_MASK)
      when Align.VERTICAL_BOTTOM, Align.VERTICAL_MIDDLE
        @renderLines(ctx, lines, useStroke, "rgba(0, 0, 0, 0)", offsetY)
        img = ctx.getImageData(0, 0, canvas.width, canvas.height)
        width = canvas.width
        height = canvas.height
        first = null
        last = null
        r = 0
        while !first? && r < height
          for c in [0...width]
            if img.data[r * width * 4 + c * 4 + 3] isnt 0
              first = r
              break
          r++
        r = height
        while !last? && r > 0
          r--
          for c in [0...width]
            if img.data[r * width * 4 + c * 4 + 3] isnt 0
              last = r
              break
        if first? and last?
          h = last - first + 1
          switch (property.align & Align.VERTICAL_MASK)
            when Align.VERTICAL_BOTTOM
              offsetY += height - h - first - @currentShadowMarginY
            when Align.VERTICAL_MIDDLE
              offsetY += (height - h) / 2 - first - @currentShadowMarginY
        context.factory.clearCanvasRect(canvas, ctx)
        if context.factory.quirkyClearRect?
          @initCanvasContext(ctx, textColor)

    @renderLines(ctx, lines, useStroke, shadowColor, offsetY)
    return

  needsScale: ->
    return true

  render:(m, c, renderingIndex, renderingCount, visible) ->
    @matrixChanged = @matrixForCheck.setWithComparing(m)
    if @matrixChanged
      if @needsScale()
        m = Utility.scaleMatrix(@matrix, m, 1 / @lwf.textScale, 0, 0)
      else
        m = Utility.copyMatrix(@matrix, m)
      if @context.shadowColor?
        property = @context.textProperty
        scale = @lwf.textScale
        @currentShadowMarginY = 0
        switch (property.align & Align.VERTICAL_MASK)
          when Align.VERTICAL_BOTTOM
            if property.shadowOffsetY > 0
              @currentShadowMarginY = property.shadowOffsetY * scale
            @currentShadowMarginY += property.shadowBlur * scale
          when Align.VERTICAL_MIDDLE
          else
            if property.shadowOffsetY < 0
              @currentShadowMarginY = property.shadowOffsetY * scale
            @currentShadowMarginY -= property.shadowBlur * scale
        m.translateY += m.scaleY * @currentShadowMarginY
      unless @context.factory.textInSubpixel
        m.translateX = Math.round(m.translateX)
        m.translateY = Math.round(m.translateY)

    red = @color.red
    green = @color.green
    blue = @color.blue
    @context.factory.convertColor(@color, @context.textColor, c)
    c = @color
    colorChanged = false
    if red isnt c.red or green isnt c.green or blue isnt c.blue
      colorChanged = true

    fontChanged = false
    if @context.fontChanged
      fontChanged = true
      @context.fontChanged = false

    strChanged = false
    str = @textObject.parent[@textObject.name]
    str = String(str) if str?
    if str? and str isnt @str
      strChanged = true
      @str = str
      @initCanvas() unless @context.factory.recycleTextCanvas

    scaleChanged = false
    if @textScale isnt @lwf.textScale
      scaleChanged = true
      @initCanvas()
      @textScale = @lwf.textScale

    @renderText(c) if !@textRendered or
      colorChanged or fontChanged or strChanged or scaleChanged
    return

  initCanvas: ->
    scale = @lwf.textScale
    property = @context.textProperty
    @leading = property.leading * scale
    @fontHeight = property.fontHeight * scale
    leftMargin = property.leftMargin / @fontHeight
    rightMargin = property.rightMargin / @fontHeight
    lm = 0
    rm = 0
    if @context.strokeColor?
      lm = rm = property.strokeWidth / 2 * scale
    if @context.shadowColor?
      sw = (property.shadowBlur - property.shadowOffsetX) * scale
      lm = sw if sw > lm
      sw = (property.shadowOffsetX + property.shadowBlur) * scale
      rm = sw if sw > rm
    leftMargin += lm
    rightMargin += rm

    text = @context.text
    switch (property.align & Align.ALIGN_MASK)
      when Align.RIGHT
        @align = "right"
        @offsetX = text.width - rightMargin
      when Align.CENTER
        @align = "center"
        @offsetX = text.width / 2
      else # Align.LEFT
        @align = "left"
        @offsetX = leftMargin

    [canvas, ctx] = @context.factory.resourceCache.createCanvas(
      @context.text.width * scale, @context.text.height * scale)
    @maxWidth = canvas.width - (leftMargin + rightMargin)
    @initCanvasContext(ctx)
    @canvas = canvas
    @canvasContext = ctx
    @letterSpacing = ctx.measureText('M').width * @context.letterSpacing
    return

  initCanvasContext:(ctx, textColor) ->
    ctx.font = "#{@fontHeight}px #{@context.fontName}"
    ctx.textAlign = @align
    ctx.textBaseline = "bottom"
    return unless textColor?

    context = @context;
    property = context.textProperty
    scale = @lwf.textScale

    ctx.fillStyle = "rgb(#{textColor.red},#{textColor.green},#{textColor.blue})"
    ctx.lineCap = "round"
    ctx.lineJoin = "round"

    if context.strokeColor?
      ctx.strokeStyle = context.factory.convertRGB(context.strokeColor)
      ctx.lineWidth = property.strokeWidth * scale

    if context.shadowColor?
      ctx.shadowOffsetX = property.shadowOffsetX * scale
      ctx.shadowOffsetY = property.shadowOffsetY * scale
      ctx.shadowBlur = property.shadowBlur * scale

    return
