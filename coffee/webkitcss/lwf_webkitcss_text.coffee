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

class WebkitCSSTextContext
  constructor:(@factory, @data, @text) ->
    @str = @data.strings[text.stringId]
    @textProperty = @data.textProperties[text.textPropertyId]
    font = @data.fonts[@textProperty.fontId]
    @fontName = "\"#{@data.strings[font.stringId]}\",sans-serif"
    @textColor = @data.colors[text.colorId]
    @name = @data.strings[text.nameStringId]

    if @textProperty.strokeColorId isnt -1
      @strokeColor = @data.colors[@textProperty.strokeColorId]
    if @textProperty.shadowColorId isnt -1
      @shadowColor = @data.colors[@textProperty.shadowColorId]

    switch (@textProperty.align & Align.ALIGN_MASK)
      when Align.RIGHT
        @align = "right"
      when Align.CENTER
        @align = "center"
      else # Format.TextProperty.Align.LEFT
        @align = "left"

  destruct: ->

class WebkitCSSTextRenderer
  constructor:(@lwf, @context, @textObject) ->
    @str = @textObject.parent[@context.name] ? @context.str
    @str = String(@str) if @str?
    @textNodes = []
    @matrixForScale = new Matrix()
    @color = new Color

    scale = @lwf.scaleByStage
    text = @context.text
    textProperty = @context.textProperty
    width = (text.width -
      textProperty.leftMargin - textProperty.rightMargin) * scale

    @node = document.createElement('div')
    @node.style.width = "#{width}px"
    @node.style.height = "#{text.height * scale}px"
    @node.style.position = "absolute"
    @node.style.textAlign = @context.align
    @node.style.textIndent = "#{textProperty.indent * scale}px"
    @node.style.lineHeight =
      "#{(textProperty.fontHeight + textProperty.leading) * scale}pt"
    @node.style.marginLeft = "#{textProperty.leftMargin * scale}px"
    @node.style.fontSize = "#{textProperty.fontHeight * scale}px"
    @node.style.fontFamily = @context.fontName
    @node.style.display = "block"
    @node.style.pointerEvents = "none"
    @node.style.webkitTransformOrigin = "0px 0px"
    @node.style.webkitUserSelect = "none"

    if @context.strokeColor?
      @node.style.webkitTextStrokeColor =
        @context.factory.convertRGB(@context.strokeColor)
      @node.style.webkitTextStrokeWidth = "#{textProperty.strokeWidth}px"

    if @context.shadowColor?
      @node.style.textShadow = "#{textProperty.shadowOffsetX}px " +
        "#{textProperty.shadowOffsetY}px " +
        "#{textProperty.shadowBlur}px " +
        @context.factory.convertRGB(@context.shadowColor)

    @context.factory.stage.appendChild(@node)

    @matrix = new Matrix()
    @alpha = -1
    @zIndex = -1
    @visible = true
    @textRendered = false

  destruct: ->
    @context.factory.stage.removeChild(@node)
    return

  render:(m, c, renderingIndex, renderingCount, visible) ->
    if @visible is visible
      return if visible is false
    else
      @visible = visible
      if visible is false
        @node.style.visible = "hidden"
        return
      else
        @node.style.visible = "visible"

    strChanged = false
    str = @textObject.parent[@context.name]
    str = String(str) if str?
    if str? and str isnt @str
      strChanged = true
      @str = str
    @renderText() if !@textRendered or strChanged

    matrixChanged = @matrix.setWithComparing(m)

    return if !matrixChanged and
      @alpha is c.multi.alpha and @zIndex is renderingIndex

    m = Utility.scaleMatrix(
      @matrixForScale, @matrix, 1 / @lwf.scaleByStage, 0, 0)
    unless @context.factory.textInSubpixel
      m.translateX = Math.round(m.translateX)
      m.translateY = Math.round(m.translateY)

    @alpha = c.multi.alpha
    @zIndex = renderingIndex
    @context.factory.convertColor(@color, @context.textColor, c)

    @context.factory.commands.push({isBitmap:false, renderer:@, matrix:m})
    return

  clearTextNodes: ->
    @textNodes = []
    @node.removeChild(@node.firstChild) while @node.firstChild
    return

  renderText: ->
    @textRendered = true
    lines = @str.split("\n")
    if lines.length is 0
      @clearTextNodes()
      return

    if lines.length is @textNodes.length
        for i in [0...lines.length]
          @textNodes[i].textContent = lines[i]
    else
      @clearTextNodes()
      if lines.length > 1
        for i in [0...lines.length]
          @node.appendChild(document.createElement('br')) if i > 0
          textNode = document.createTextNode(lines[i])
          @node.appendChild(textNode)
          @textNodes.push(textNode)
      else
        textNode = document.createTextNode(@str)
        @node.appendChild(textNode)
        @textNodes.push(textNode)

    scale = @lwf.scaleByStage
    property = @context.textProperty
    fontHeight = property.fontHeight * scale
    leading = property.leading * scale
    h = fontHeight * lines.length + leading * (lines.length - 1)
    switch (property.align & Align.VERTICAL_MASK)
      when Align.VERTICAL_BOTTOM
        offsetY = @context.text.height - h
      when Align.VERTICAL_MIDDLE
        offsetY = (@context.text.height - h) / 2
      else
        offsetY = 0
    @node.style.marginTop = "#{offsetY}px"
    return

