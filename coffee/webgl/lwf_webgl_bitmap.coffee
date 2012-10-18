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

class WebGLBitmapContext
  constructor:(@factory, @data, bitmapEx) ->
    gl = @factory.stageContext
    fragment = data.textureFragments[bitmapEx.textureFragmentId]
    texdata = data.textures[fragment.textureId]
    @preMultipliedAlpha =
      texdata.format == Format.Constant.TEXTUREFORMAT_PREMULTIPLIEDALPHA
    filename = texdata.filename
    d = @factory.textures[filename]
    if d?
      [@texture, scale] = d
    else
      image = @factory.cache[filename]
      scale = 1 / texdata.scale
      @texture = gl.createTexture()
      gl.bindTexture(gl.TEXTURE_2D, @texture)
      gl.texImage2D(gl.TEXTURE_2D,
        0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image)
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR)
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR)
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)
      gl.bindTexture(gl.TEXTURE_2D, null)
      @factory.textures[filename] = [@texture, scale]

    tw = texdata.width
    th = texdata.height

    x = fragment.x
    y = fragment.y
    u = fragment.u
    v = fragment.v
    w = fragment.w
    h = fragment.h

    bu = bitmapEx.u * w
    bv = bitmapEx.v * h
    bw = bitmapEx.w
    bh = bitmapEx.h

    x += bu
    y += bv
    u += bu
    v += bv
    w *= bw
    h *= bh

    height = h

    x0 = x * scale
    y0 = y * scale
    x1 = (x + w) * scale
    y1 = (y + h) * scale

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
    if fragment.rotated is 0
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
    else
      u0 = (2 * u + 1) / dw
      v0 = (2 * v + 1) / dh
      u1 = u0 + (h * 2 - 2) / dw
      v1 = v0 + (w * 2 - 1) / dh
      uv = new Float32Array([
        u0, v1,
        u1, v1,
        u0, v0,
        u1, v0,
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
    gl = @factory.stageContext
    gl.deleteTexture(@texture)
    gl.deleteBuffer(@verticesBuffer)
    gl.deleteBuffer(@uvBuffer)
    gl.deleteBuffer(@trianglesBuffer)
    return

class WebGLBitmapRenderer
  constructor:(@context) ->
    @matrix = new Float32Array([0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1])
    @color = new Float32Array([0,0,0,0])

  destruct: ->

  render:(m, c, renderingIndex, renderingCount, visible) ->
    return unless visible

    factory = @context.factory
    gl = factory.stageContext

    gl.bindBuffer(gl.ARRAY_BUFFER, @context.verticesBuffer)
    gl.vertexAttribPointer(factory.aVertexPosition, 3, gl.FLOAT, false, 0, 0)
    gl.bindBuffer(gl.ARRAY_BUFFER, @context.uvBuffer)
    gl.vertexAttribPointer(factory.aTextureCoord, 2, gl.FLOAT, false, 0, 0)

    gm = @matrix
    gm[0] = m.scaleX
    gm[1] = m.skew1
    gm[4] = m.skew0
    gm[5] = m.scaleY
    gm[12] = m.translateX
    gm[13] = m.translateY
    gm[14] = factory.farZ + renderingIndex
    gl.uniformMatrix4fv(factory.uMatrix, false, gm)

    gc = @color
    if @context.preMultipliedAlpha
      src = gl.ONE
      alpha = c.multi.alpha
      gc[0] = c.multi.red * alpha
      gc[1] = c.multi.green * alpha
      gc[2] = c.multi.blue * alpha
      gc[3] = alpha
    else
      src = gl.SRC_ALPHA
      gc[0] = c.multi.red
      gc[1] = c.multi.green
      gc[2] = c.multi.blue
      gc[3] = c.multi.alpha
    gl.blendFunc(src, gl.ONE_MINUS_SRC_ALPHA)
    gl.uniform4fv(factory.uColor, gc)

    gl.activeTexture(gl.TEXTURE0)
    gl.bindTexture(gl.TEXTURE_2D, @context.texture)
    gl.uniform1i(factory.uTexture, 0)

    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, @context.trianglesBuffer)
    gl.drawElements(gl.TRIANGLES, 6, gl.UNSIGNED_SHORT, 0)
    return

