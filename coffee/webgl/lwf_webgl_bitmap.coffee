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
    fragment = data.textureFragments[bitmapEx.textureFragmentId]
    texdata = data.textures[fragment.textureId]
    @preMultipliedAlpha =
      texdata.format == Format.Constant.TEXTUREFORMAT_PREMULTIPLIEDALPHA
    filename = texdata.filename
    image = @factory.cache[filename]
    key = image.src ? image.name
    repeat_s = (bitmapEx.attribute & Format.BitmapEx.Attribute.REPEAT_S) isnt 0
    key += "_REPEAT_S" if repeat_s
    repeat_t = (bitmapEx.attribute & Format.BitmapEx.Attribute.REPEAT_T) isnt 0
    key += "_REPEAT_T" if repeat_s
    d = @factory.textures[key]
    if d?
      [@texture, scale, attribute] = d
    else
      scale = 1 / texdata.scale
      gl = @factory.glContext
      @texture = gl.createTexture()
      gl.bindTexture(gl.TEXTURE_2D, @texture)
      gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image)
      @factory.setTexParameter(gl, repeat_s, repeat_t)
      @factory.textures[key] = [@texture, scale, bitmapEx.attribute]

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

    x0 = x * scale
    y0 = y * scale
    x1 = (x + w) * scale
    y1 = (y + h) * scale

    ###
    @vertexData = [
      {x:x1, y:y1},
      {x:x1, y:y0},
      {x:x0, y:y1},
      {x:x0, y:y0}
    ]
    ###
    @vertexData = new Float32Array(2 * 4)
    @vertexData[0] = x1; @vertexData[1] = y1
    @vertexData[2] = x1; @vertexData[3] = y0
    @vertexData[4] = x0; @vertexData[5] = y1
    @vertexData[6] = x0; @vertexData[7] = y0

    @uv = new Float32Array(2 * 4)
    if fragment.rotated is 0
      u0 = u / tw
      v0 = v / th
      u1 = (u + w) / tw
      v1 = (v + h) / th
      ###
      @uv = [
        {u:u1, v:v1},
        {u:u1, v:v0},
        {u:u0, v:v1},
        {u:u0, v:v0}
      ]
      ###
      @uv[0] = u1; @uv[1] = v1
      @uv[2] = u1; @uv[3] = v0
      @uv[4] = u0; @uv[5] = v1
      @uv[6] = u0; @uv[7] = v0
    else
      u0 = u / tw
      v0 = v / th
      u1 = (u + h) / tw
      v1 = (v + w) / th
      ###
      @uv = [
        {u:u0, v:v1},
        {u:u1, v:v1},
        {u:u0, v:v0},
        {u:u1, v:v0}
      ]
      ###
      @uv[0] = u0; @uv[1] = v1
      @uv[2] = u1; @uv[3] = v1
      @uv[4] = u0; @uv[5] = v0
      @uv[6] = u1; @uv[7] = v0

  destruct: ->

class WebGLBitmapRenderer
  constructor:(@context) ->
    @cmd = new WebGLRenderCommand(@context, @context.texture, null)

  destruct: ->

  render:(m, c, renderingIndex, renderingCount, visible) ->
    return if !visible or c.multi.alpha is 0

    factory = @context.factory
    cmd = @cmd
    cmd.matrix = m
    cmd.colorTransform = c
    cmd.blendMode = factory.blendMode
    cmd.maskMode = factory.maskMode
    factory.addCommand(renderingIndex, cmd)
    return

