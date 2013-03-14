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
    @name = @data.strings[@text.nameStringId]

    if @textProperty.strokeColorId isnt -1
      @strokeColor = @data.colors[@textProperty.strokeColorId]
    if @textProperty.shadowColorId isnt -1
      @shadowColor = @data.colors[@textProperty.shadowColorId]

    @fontName = "\"#{@data.strings[font.stringId]}\",sans-serif"

  destruct: ->

class HTML5TextRenderer
  constructor:(@lwf, @context, @textObject) ->
    @str = @textObject.parent[@context.name] ? @context.str
    @str = String(@str) if @str?
    @matrixForScale = new Matrix()
    @color = new Color
    scale = @lwf.scaleByStage
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
        align = "right"
        @offsetX = text.width - rightMargin
      when Align.CENTER
        align = "center"
        @offsetX = text.width / 2
      else # Align.LEFT
        align = "left"
        @offsetX = leftMargin

    canvas = document.createElement("canvas")
    canvas.width = @context.text.width * scale
    canvas.height = @context.text.height * scale
    @maxWidth = canvas.width - (leftMargin + rightMargin)
    ctx = canvas.getContext("2d")
    ctx.font = "#{@fontHeight}px #{@context.fontName}"
    ctx.textAlign = align
    ctx.textBaseline = "bottom"
    @canvas = canvas
    @canvasContext = ctx
    @textRendered = false

  destruct: ->

  fitText:(line, words, lineStart, imin, imax) ->
    return if imax < imin
    imid = ((imin + imax) / 2) >> 0
    start = if lineStart is 0 then 0 else words[lineStart - 1]
    str = line.slice(start, words[imid])
    w = @canvasContext.measureText(str).width
    if w <= @maxWidth
      if w > @lineWidth
        @index = imid
        @lineWidth = w
      @fitText(line, words, lineStart, imid + 1, imax)
    if w >= @lineWidth
      @fitText(line, words, lineStart, imin, imid - 1)

  adjustText:(lines) ->
    newlines = []
    for line in lines
      words = line.split(" ")
      line = ""
      for word in words
        if word.length > 0
          line += " " if line.length > 0
          line += word

      if @canvasContext.measureText(line).width > @maxWidth
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
          if @canvasContext.measureText(str).width <= @maxWidth
            line = str
            break
          imin = @index + 1

      newlines.push(line)
    return newlines

  renderText:(textColor) ->
    @textRendered = true

    context = @context
    canvas = @canvas
    ctx = @canvasContext
    scale = @lwf.scaleByStage
    lines = @adjustText(@str.split("\n"))

    property = context.textProperty

    switch (property.align & Align.VERTICAL_MASK)
      when Align.VERTICAL_BOTTOM
        len = lines.length
        h = (@fontHeight * len + @leading * (len - 1)) * 96 / 72
        offsetY = canvas.height - h
      when Align.VERTICAL_MIDDLE
        len = lines.length + 1
        h = (@fontHeight * len + @leading * (len - 1)) * 96 / 72
        offsetY = (canvas.height - h) / 2
      else
        offsetY = 0
    offsetY += @fontHeight * 1.2

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
      x = @offsetX * scale
      y = offsetY + (@fontHeight + @leading) * i * 96 / 72
      ctx.shadowColor = shadowColor if context.shadowColor?
      ctx.fillText(line, x, y)
      if useStroke
        ctx.shadowColor = "rgba(0, 0, 0, 0)" if context.shadowColor?
        ctx.strokeText(line, x, y)

    return

  render:(m, c, renderingIndex, renderingCount, visible) ->
    m = Utility.scaleMatrix(@matrixForScale, m, 1 / @lwf.scaleByStage, 0, 0)
    unless @context.factory.textInSubpixel
      m.translateX = Math.round(m.translateX)
      m.translateY = Math.round(m.translateY)
    @matrix = m

    red = @color.red
    green = @color.green
    blue = @color.blue
    @context.factory.convertColor(@color, @context.textColor, c)
    c = @color
    colorChanged = false
    if red isnt c.red or green isnt c.green or blue isnt c.blue
      colorChanged = true

    strChanged = false
    str = @textObject.parent[@context.name]
    str = String(str) if str?
    if str? and str isnt @str
      strChanged = true
      @str = str

    @renderText(c) if !@textRendered or colorChanged or strChanged
    return

