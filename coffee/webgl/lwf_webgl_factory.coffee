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

class WebGLRendererFactory extends WebkitCSSRendererFactory
  constructor:(data, @resourceCache, @cache, @stage, @textInSubpixel) ->
    params = {premultipliedAlpha:false}
    @stage.style.webkitUserSelect = "none"
    @stageContext = @stage.getContext("webgl", params) ?
      @stage.getContext("experimental-webgl", params)
    if @stage.width is 0 and @stage.height is 0
      @stage.width = data.header.width
      @stage.height = data.header.height
    @w = @stage.width
    @h = @stage.height

    gl = @stageContext
    gl.viewport(0, 0, @w, @h)
    gl.clearColor(0.0, 0.0, 0.0, 1.0)
    gl.enable(gl.DEPTH_TEST)
    gl.depthFunc(gl.LEQUAL)
    gl.enable(gl.BLEND)

    vertexShader = @loadShader(gl, gl.VERTEX_SHADER, """
attribute vec3 aVertexPosition;
attribute vec2 aTextureCoord;
uniform mat4 uPMatrix;
uniform mat4 uMatrix;
varying mediump vec2 vTextureCoord;
void main() {
  vTextureCoord = aTextureCoord;
  gl_Position = uPMatrix * uMatrix * vec4(aVertexPosition, 1.0);
}
""")

    fragmentShader = @loadShader(gl, gl.FRAGMENT_SHADER, """
varying mediump vec2 vTextureCoord;
uniform lowp vec4 uColor;
uniform sampler2D uTexture;
void main() {
  gl_FragColor = texture2D(uTexture, vTextureCoord) * uColor;
}
""")

    shaderProgram = gl.createProgram()
    gl.attachShader(shaderProgram, vertexShader)
    gl.attachShader(shaderProgram, fragmentShader)
    gl.linkProgram(shaderProgram)
    unless gl.getProgramParameter(shaderProgram, gl.LINK_STATUS)
      alert("Unable to initialize the shader program.")
    gl.useProgram(shaderProgram)

    @aVertexPosition = gl.getAttribLocation(shaderProgram, "aVertexPosition")
    gl.enableVertexAttribArray(@aVertexPosition)
    @aTextureCoord = gl.getAttribLocation(shaderProgram, "aTextureCoord")
    gl.enableVertexAttribArray(@aTextureCoord)
    @uPMatrix = gl.getUniformLocation(shaderProgram, "uPMatrix")
    @uMatrix = gl.getUniformLocation(shaderProgram, "uMatrix")
    @uColor = gl.getUniformLocation(shaderProgram, "uColor")
    @uTexture = gl.getUniformLocation(shaderProgram, "uTexture")

    depth = 1024
    @farZ = -(depth - 1)
    rl = @w
    tb = -@h
    fn = depth * 2
    pmatrix = [
      2 / rl, 0, 0, 0,
      0, 2 / tb, 0, 0,
      0, 0, -2 / fn, 0,
      -1, 1, 0, 1
    ]
    gl.uniformMatrix4fv(@uPMatrix, false, new Float32Array(pmatrix))

    @textures = {}
    @bitmapContexts = []
    for bitmap in data.bitmaps
      continue if bitmap.textureFragmentId == -1
      bitmapEx = new Format.BitmapEx()
      bitmapEx.matrixId = bitmap.matrixId
      bitmapEx.textureFragmentId = bitmap.textureFragmentId
      bitmapEx.u = 0
      bitmapEx.v = 0
      bitmapEx.w = 1
      bitmapEx.h = 1
      @bitmapContexts.push new WebGLBitmapContext(@, data, bitmapEx)

    @bitmapExContexts = []
    for bitmapEx in data.bitmapExs
      continue if bitmapEx.textureFragmentId == -1
      @bitmapExContexts.push new WebGLBitmapContext(@, data, bitmapEx)

    @textContexts = []
    for text in data.texts
      @textContexts.push new WebGLTextContext(@, data, text)

  loadShader:(gl, type, program) ->
    shader = gl.createShader(type)
    gl.shaderSource(shader, program)
    gl.compileShader(shader)
    unless gl.getShaderParameter(shader, gl.COMPILE_STATUS)
      alert("An error occurred compiling the shaders: " +
        gl.getShaderInfoLog(shader))
    return shader

  destruct: ->
    gl = @stageContext
    gl.bindTexture(gl.TEXTURE_2D, null)
    gl.bindBuffer(gl.ARRAY_BUFFER, null)
    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, null)
    context.destruct() for context in @bitmapContexts
    context.destruct() for context in @bitmapExContexts
    context.destruct() for context in @textContexts
    return

  beginRender:(lwf) ->
    gl = @stageContext
    gl.clear(gl.COLOR_BUFFER_BIT|gl.DEPTH_BUFFER_BIT)
    return

  endRender:(lwf) ->

  constructBitmap:(lwf, objectId, bitmap) ->
    context = @bitmapContexts[objectId]
    new WebGLBitmapRenderer(context) if context

  constructBitmapEx:(lwf, objectId, bitmapEx) ->
    context = @bitmapExContexts[objectId]
    new WebGLBitmapRenderer(context) if context

  constructText:(lwf, objectId, text) ->
    context = @textContexts[objectId]
    new WebGLTextRenderer(lwf, context, text) if context

  constructParticle:(lwf, objectId, particle) ->
    ctor = @resourceCache.particleConstructor
    particleData = lwf.data.particleDatas[particle.particleDataId]
    ctor(lwf, lwf.data.strings[particleData.stringId]) if ctor?

  setBackgroundColor:(lwf) ->
    bgColor = lwf.data.header.backgroundColor
    r = ((bgColor >> 16) & 0xff) / 255.0
    g = ((bgColor >>  8) & 0xff) / 255.0
    b = ((bgColor >>  0) & 0xff) / 255.0
    gl = @stageContext
    gl.clearColor(r, g, b, 1.0)
    return
