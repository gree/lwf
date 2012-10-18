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

class WebGLTextContext
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

    gl = @factory.stageContext
    tw = @text.width
    th = @text.height

    x = 0
    y = 0
    u = 0
    v = 0
    w = tw
    h = th

    x0 = x
    y0 = y
    x1 = x + w
    y1 = y + h

    vertices = new Float32Array([
      x1, y1, 0,
      x1, y0, 0,
      x0, y1, 0,
      x0, y0, 0,
    ])
    @verticesBuffer = gl.createBuffer()
    gl.bindBuffer(gl.ARRAY_BUFFER, @verticesBuffer)
    gl.bufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW)

    dw = 2.0 * tw
    dh = 2.0 * th
    u0 = (2 * u + 1) / dw
    v0 = (2 * v + 1) / dh
    u1 = u0 + (w * 2 - 2) / dw
    v1 = v0 + (h * 2 - 1) / dh
    uv = new Float32Array([
      u1, v1,
      u1, v0,
      u0, v1,
      u0, v0,
    ])
    @uvBuffer = gl.createBuffer()
    gl.bindBuffer(gl.ARRAY_BUFFER, @uvBuffer)
    gl.bufferData(gl.ARRAY_BUFFER, uv, gl.STATIC_DRAW)

    triangles = new Uint16Array([
      0, 1, 2,
      2, 1, 3
    ])
    @trianglesBuffer = gl.createBuffer()
    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, @trianglesBuffer)
    gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, triangles, gl.STATIC_DRAW)

  destruct: ->
    gl.bindBuffer(gl.ARRAY_BUFFER, null)
    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, null)
    gl.deleteBuffer(@verticesBuffer)
    gl.deleteBuffer(@uvBuffer)
    gl.deleteBuffer(@trianglesBuffer)
    return

class WebGLTextRenderer
  constructor:(@lwf, @context, @textObject) ->
    @str = @textObject.parent[@context.name] ? @context.str
    @str = String(@str) if @str?

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

    @matrixForScale = new Matrix()
    @matrix = new Float32Array([0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1])
    @color = new Float32Array([0,0,0,0])

  destruct: ->
    gl = @context.factory.stageContext
    gl.deleteTexture(@texture) if @texture
    return

  render:(m, c, renderingIndex, renderingCount, visible) ->
    return unless visible

    strChanged = false
    str = @textObject.parent[@context.name]
    str = String(str) if str?
    if str? and str isnt @str
      strChanged = true
      @str = str
    @renderText() if !@textRendered or strChanged

    factory = @context.factory
    gl = factory.stageContext

    gl.bindBuffer(gl.ARRAY_BUFFER, @context.verticesBuffer)
    gl.vertexAttribPointer(factory.aVertexPosition, 3, gl.FLOAT, false, 0, 0)
    gl.bindBuffer(gl.ARRAY_BUFFER, @context.uvBuffer)
    gl.vertexAttribPointer(factory.aTextureCoord, 2, gl.FLOAT, false, 0, 0)

    textInSubpixel = @context.factory.textInSubpixel
    gm = @matrix
    gm[0] = m.scaleX
    gm[1] = m.skew1
    gm[4] = m.skew0
    gm[5] = m.scaleY
    gm[12] = if textInSubpixel then m.translateX else Math.round(m.translateX)
    gm[13] = if textInSubpixel then m.translateY else Math.round(m.translateY)
    gm[14] = factory.farZ + renderingIndex
    gl.uniformMatrix4fv(factory.uMatrix, false, gm)

    gc = @color
    src = gl.SRC_ALPHA
    gc[0] = c.multi.red
    gc[1] = c.multi.green
    gc[2] = c.multi.blue
    gc[3] = c.multi.alpha
    gl.blendFunc(src, gl.ONE_MINUS_SRC_ALPHA)
    gl.uniform4fv(factory.uColor, gc)

    gl.activeTexture(gl.TEXTURE0)
    gl.bindTexture(gl.TEXTURE_2D, @texture)
    gl.uniform1i(factory.uTexture, 0)

    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, @context.trianglesBuffer)
    gl.drawElements(gl.TRIANGLES, 6, gl.UNSIGNED_SHORT, 0)
    return

  renderText: ->
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
    ctx.fillStyle = @context.factory.convertRGB(@context.textColor)

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

    gl = @context.factory.stageContext
    if @texture
      gl.bindTexture(gl.TEXTURE_2D, null)
      gl.deleteTexture(@texture)
    @texture = gl.createTexture()
    gl.bindTexture(gl.TEXTURE_2D, @texture)
    gl.texImage2D(gl.TEXTURE_2D,
      0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, canvas)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)
    return

