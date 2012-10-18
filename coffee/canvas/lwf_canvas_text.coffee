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

Align = Format.TextProperty.Align

class CanvasTextContext
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

    switch (@textProperty.align & Align.ALIGN_MASK)
      when Align.RIGHT
        @align = "right"
        @offsetX = @text.width
      when Align.CENTER
        @align = "center"
        @offsetX = @text.width / 2
      else # Align.LEFT
        @align = "left"
        @offsetX = 0

  destruct: ->

class CanvasTextRenderer
  constructor:(@lwf, @context, @textObject) ->
    @str = @textObject.parent[@context.name] ? @context.str
    @str = String(@str) if @str?
    @matrixForScale = new Matrix()
    @color = new Color

    scale = @lwf.scaleByStage
    canvas = document.createElement("canvas")
    canvas.width = @context.text.width * scale
    canvas.height = @context.text.height * scale
    ctx = canvas.getContext("2d")
    fontHeight = @context.textProperty.fontHeight * scale
    ctx.font = "#{fontHeight}px #{@context.fontName}"
    ctx.textAlign = @context.align
    @canvas = canvas
    @canvasContext = ctx
    @textRendered = false

  destruct: ->

  render:(m, c, renderingIndex, renderingCount, visible) ->
    return unless visible

    m = Utility.scaleMatrix(@matrixForScale, m, 1 / @lwf.scaleByStage, 0, 0)
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

    strChanged = false
    str = @textObject.parent[@context.name]
    str = String(str) if str?
    if str? and str isnt @str
      strChanged = true
      @str = str

    @renderText(c) if !@textRendered or colorChanged or strChanged

    @context.factory.commands[renderingIndex] = {
      alpha:c.alpha,
      matrix:m,
      image:@canvas,
      u:0,
      v:0,
      w:@canvas.width,
      h:@canvas.height
    }
    return

  renderText:(c) ->
    @textRendered = true
    canvas = @canvas
    ctx = @canvasContext
    lines = @str.split("\n")
    scale = @lwf.scaleByStage
    property = @context.textProperty
    fontHeight = property.fontHeight * scale
    leading = property.leading * scale
    h = (fontHeight * lines.length + leading * (lines.length - 1)) * 96 / 72
    switch (property.align & Align.VERTICAL_MASK)
      when Align.VERTICAL_BOTTOM
        offsetY = canvas.height - h
      when Align.VERTICAL_MIDDLE
        offsetY = (canvas.height - h) / 2
      else
        offsetY = 0
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    ctx.fillStyle = "rgb(#{c.red},#{c.green},#{c.blue})"

    useStroke = false
    if @context.strokeColor?
      ctx.strokeStyle = @context.factory.convertRGB(@context.strokeColor)
      ctx.lineWidth = property.strokeWidth
      useStroke = true

    if @context.shadowColor?
      shadowColor = @context.factory.convertRGB(@context.shadowColor)
      ctx.shadowOffsetX = property.shadowOffsetX
      ctx.shadowOffsetY = property.shadowOffsetY
      ctx.shadowBlur = property.shadowBlur

    for i in [0...lines.length]
      line = lines[i]
      x = @context.offsetX * scale
      y = fontHeight + offsetY
      y += (fontHeight + leading) * i * 96 / 72 if i > 0
      ctx.shadowColor = shadowColor if @context.shadowColor?
      ctx.fillText(line, x, y)
      if useStroke
        ctx.shadowColor = "rgba(0, 0, 0, 0)" if @context.shadowColor?
        ctx.strokeText(line, x, y)
    return
