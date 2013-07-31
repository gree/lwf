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

class Cocos2dTextContext
  constructor:(@factory, @data, @text) ->
    @str = @data.strings[@text.stringId]
    @textProperty = @data.textProperties[@text.textPropertyId]
    font = @data.fonts[@textProperty.fontId]
    @textColor = @data.colors[@text.colorId]
    @name = @data.strings[@text.nameStringId]

    @fontName = "\"#{@data.strings[font.stringId]}\",sans-serif"

  destruct: ->

class Cocos2dTextRenderer
  constructor:(@lwf, @context, @textObject) ->
    @str = @textObject.parent[@context.name] ? @context.str
    @str = String(@str) if @str?
    @matrix = new Matrix(0,0,0,0,0,0)
    @matrixForScale = new Matrix()
    @color = new Color()
    @color3B = cc.c3b()

    switch (@context.textProperty.align & Align.ALIGN_MASK)
      when Align.RIGHT
        align = cc.TEXT_ALIGNMENT_RIGHT
      when Align.CENTER
        align = cc.TEXT_ALIGNMENT_CENTER
      else # Align.LEFT
        align = cc.TEXT_ALIGNMENT_LEFT

    switch (@context.textProperty.align & Align.VERTICAL_MASK)
      when Align.VERTICAL_BOTTOM
        verticalAlign = cc.VERTICAL_TEXT_ALIGNMENT_BOTTOM
      when Align.VERTICAL_MIDDLE
        verticalAlign = cc.VERTICAL_TEXT_ALIGNMENT_CENTER
      else # Align.VERTICAL_TOP
        verticalAlign = cc.VERTICAL_TEXT_ALIGNMENT_TOP

    scale = @lwf.textScale
    @label = cc.LWFLabel.create("", @context.fontName,
      @context.textProperty.fontHeight * scale,
      cc.size(@context.text.width * scale, @context.text.height * scale),
      align, verticalAlign)
    @z = -1
    @textRendered = false

  destruct: ->
    @label.removeFromParentAndCleanup(true)
    return

  render:(m, c, renderingIndex, renderingCount, visible) ->
    unless visible
      @label.setOpacity(0)
      return

    z = renderingIndex
    if @z isnt z
      if @z is -1
        @context.factory.lwfNode.addChild(@label, z)
      else
        @label.getParent().reorderChild(@label, z)
      @z = z

    red = @color.red
    green = @color.green
    blue = @color.blue
    @context.factory.convertColor(@color, @context.textColor, c)
    c = @color3B
    c.r = @color.red
    c.g = @color.green
    c.b = @color.blue
    alpha = @color.alpha
    colorChanged = false
    if red isnt @color.red or green isnt @color.green or
        blue isnt @color.blue or alpha isnt @color.alpha
      @label.setOpacity(alpha)
      colorChanged = true

    strChanged = false
    str = @textObject.parent[@context.name]
    str = String(str) if str?
    if str? and str isnt @str
      strChanged = true
      @str = str

    @renderText(c) if !@textRendered or colorChanged or strChanged

    if @matrix.setWithComparing(m)
      m = Utility.scaleMatrix(
        @matrixForScale, @matrix, 1 / @lwf.textScale, 0, 0)
      unless @context.factory.textInSubpixel
        m.translateX = Math.round(m.translateX)
        m.translateY = Math.round(m.translateY)
      @label.setMatrix(m)
    return

  renderText:(c) ->
    @textRendered = true
    @label.setColor(c)
    @label.setString(@str)
    return
