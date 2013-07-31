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

class WebGLTextContext extends HTML5TextContext
  constructor:(@factory, @data, @text) ->
    super

    @preMultipliedAlpha = false

    tw = @text.width
    th = @text.height

    x0 = 0
    y0 = 0
    x1 = tw
    y1 = th

    @vertexData = [
      {x:x1, y:y1},
      {x:x1, y:y0},
      {x:x0, y:y1},
      {x:x0, y:y0}
    ]

    dw = 2.0 * tw
    dh = 2.0 * th
    u0 = 1 / dw
    v0 = 1 / dh
    u1 = u0 + (tw * 2 - 2) / dw
    v1 = v0 + (th * 2 - 1) / dh

    @uv = [
      {u:u1, v:v1},
      {u:u1, v:v0},
      {u:u0, v:v1},
      {u:u0, v:v0}
    ]

class WebGLTextRenderer extends HTML5TextRenderer
  constructor:(@lwf, @context, @textObject) ->
    super
    @meshCache = new Float32Array(4 * 9)
    @cmd = {}

  destruct: ->
    gl = @context.factory.stageContext
    gl.deleteTexture(@texture) if @texture
    return

  needsScale: ->
    return false

  render:(m, c, renderingIndex, @renderingCount, visible) ->
    return if !visible or c.multi.alpha is 0

    super

    factory = @context.factory
    cmd = @cmd
    cmd.renderer = null
    cmd.context = @context
    cmd.texture = @texture
    cmd.matrix = @matrix
    cmd.colorTransform = c
    cmd.blendMode = factory.blendMode
    cmd.maskMode = factory.maskMode
    cmd.meshCache = @meshCache
    cmd.useMeshCache = !@matrixChanged
    factory.addCommand(renderingIndex, cmd)
    return

  renderText:(textColor) ->
    super

    gl = @context.factory.stageContext
    @texture = gl.createTexture() unless @texture?
    gl.bindTexture(gl.TEXTURE_2D, @texture)
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, @canvas)
    @context.factory.setTexParameter(gl)
    return

