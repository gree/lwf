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
  @shaderProgram = null

  constructor:(data, @resourceCache, @cache, @stage, @textInSubpixel) ->
    params = {premultipliedAlpha:false}
    @stage.style.webkitUserSelect = "none"
    @stageContext = @stage.getContext("webgl", params) ?
      @stage.getContext("experimental-webgl", params)
    if @stage.width is 0 and @stage.height is 0
      @stage.width = data.header.width
      @stage.height = data.header.height
    @blendMode = "normal"
    @maskMode = "normal"

    gl = @stageContext
    gl.enable(gl.BLEND)
    gl.disable(gl.DEPTH_TEST)
    gl.disable(gl.DITHER)
    gl.disable(gl.SCISSOR_TEST)
    gl.activeTexture(gl.TEXTURE0)
    gl.clearColor(0.0, 0.0, 0.0, 1.0)

    r = @stage.getBoundingClientRect()
    dpr = devicePixelRatio
    @w = Math.round(r.width * dpr)
    @h = Math.round(r.height * dpr)
    gl.viewport(0, 0, @w, @h)

    if WebGLRendererFactory.shaderProgram?
      shaderProgram = WebGLRendererFactory.shaderProgram
    else
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
      WebGLRendererFactory.shaderProgram = shaderProgram

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
      continue if bitmap.textureFragmentId is -1
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
      continue if bitmapEx.textureFragmentId is -1
      @bitmapExContexts.push new WebGLBitmapContext(@, data, bitmapEx)

    @textContexts = []
    for text in data.texts
      @textContexts.push new WebGLTextContext(@, data, text)

    @initCommands()

  loadShader:(gl, type, program) ->
    shader = gl.createShader(type)
    gl.shaderSource(shader, program)
    gl.compileShader(shader)
    unless gl.getShaderParameter(shader, gl.COMPILE_STATUS)
      alert("An error occurred compiling the shaders: " +
        gl.getShaderInfoLog(shader))
    return shader

  destruct: ->
    @deleteMask()
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
    gl.clear(gl.COLOR_BUFFER_BIT)
    return

  render:(gl, cmd, rIndex) ->
    if @renderMaskMode isnt cmd.maskMode
      @generateMask()
      switch cmd.maskMode
        when "erase", "mask"
          @renderMasked = true
          @srcFactor = if cmd.maskMode is "erase" then \
            gl.ONE_MINUS_DST_ALPHA else gl.DST_ALPHA
          gl.bindFramebuffer(gl.FRAMEBUFFER, @maskFrameBuffer)
          gl.clearColor(0, 0, 0, 0)
          gl.clear(gl.COLOR_BUFFER_BIT)
        when "layer"
          if @renderMasked
            gl.bindFramebuffer(gl.FRAMEBUFFER, @layerFrameBuffer)
            gl.clearColor(0, 0, 0, 0)
            gl.clear(gl.COLOR_BUFFER_BIT)
          else
            gl.bindFramebuffer(gl.FRAMEBUFFER, null)
        else
          if @renderMaskMode is "layer" and @renderMasked
            @renderMask()
          else
            gl.bindFramebuffer(gl.FRAMEBUFFER, null)
      @renderMaskMode = cmd.maskMode

    cmd.renderer.renderCommand(
      cmd.matrix, cmd.colorTransform, rIndex, cmd.blendMode)
    return

  endRender:(lwf) ->
    if lwf.parent?
      f = lwf.parent.lwf.rendererFactory
      f.addCommand(parseInt(rIndex, 10), cmd) for rIndex, cmd of @commands
      @initCommands()
      return

    @renderMaskMode = "normal"
    @renderMasked = false
    gl = @stageContext
    for rIndex in @commandsKeys
      cmd = @commands[rIndex]
      if cmd.subCommandsKeys?
        for srIndex in cmd.subCommandsKeys
          scmd = cmd.subCommands[srIndex]
          @render(gl, scmd, srIndex)
      @render(gl, cmd, rIndex)

    if @renderMaskMode isnt "normal"
      if @renderMaskMode is "layer" and @renderMasked
        @renderMask()
      else
        gl.bindFramebuffer(gl.FRAMEBUFFER, null)

    @initCommands()
    return

  setBlendMode:(@blendMode) ->

  setMaskMode:(@maskMode) ->

  generateMask: ->
    return if @maskTexture?

    @maskMatrix = new Float32Array([1,0,0,0,0,-1,0,0,0,0,1,0,0,@h,0,1])
    @maskColor = new Float32Array([1,1,1,1])

    gl = @stageContext
    @maskTexture = gl.createTexture()
    @layerTexture = gl.createTexture()
    textures = [@maskTexture, @layerTexture]
    @maskFrameBuffer = gl.createFramebuffer()
    @layerFrameBuffer = gl.createFramebuffer()
    framebuffers = [@maskFrameBuffer, @layerFrameBuffer]

    for i in [0...2]
      texture = textures[i]
      gl.bindTexture(gl.TEXTURE_2D, texture)
      gl.texImage2D(gl.TEXTURE_2D, 0,
        gl.RGBA, @w, @h, 0, gl.RGBA, gl.UNSIGNED_BYTE, null)
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR)
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR)
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
      gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)
      gl.bindTexture(gl.TEXTURE_2D, null)

      framebuffer = framebuffers[i]
      gl.bindFramebuffer(gl.FRAMEBUFFER, framebuffer)
      gl.framebufferTexture2D(gl.FRAMEBUFFER,
        gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, texture, 0)
      gl.bindFramebuffer(gl.FRAMEBUFFER, null)

    vertices = new Float32Array([
      @w, @h, 0,
      @w,  0, 0,
       0, @h, 0,
       0,  0, 0,
    ])
    @maskVertexBuffer = gl.createBuffer()
    gl.bindBuffer(gl.ARRAY_BUFFER, @maskVertexBuffer)
    gl.bufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW)

    uv = new Float32Array([
      1, 1,
      1, 0,
      0, 1,
      0, 0,
    ])
    @maskUVBuffer = gl.createBuffer()
    gl.bindBuffer(gl.ARRAY_BUFFER, @maskUVBuffer)
    gl.bufferData(gl.ARRAY_BUFFER, uv, gl.STATIC_DRAW)

    triangles = new Uint8Array([
      0, 1, 2,
      2, 1, 3,
    ])
    @maskTrianglesBuffer = gl.createBuffer()
    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, @maskTrianglesBuffer)
    gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, triangles, gl.STATIC_DRAW)
    return

  deleteMask: ->
    return unless @maskTexture?

    gl = @stageContext
    gl.deleteBuffer(@maskVertexBuffer)
    gl.deleteBuffer(@maskUVBuffer)
    gl.deleteBuffer(@maskTrianglesBuffer)

    gl.deleteFramebuffer(@maskFrameBuffer)
    gl.deleteFramebuffer(@layerFrameBuffer)
    @maskFrameBuffer = null
    @layerFrameBuffer = null

    gl.deleteTexture(@maskTexture)
    gl.deleteTexture(@layerTexture)
    @maskTexture = null
    @layerTexture = null
    return

  renderMask: ->
    gl = @stageContext

    gl.bindBuffer(gl.ARRAY_BUFFER, @maskVertexBuffer)
    gl.vertexAttribPointer(@aVertexPosition, 3, gl.FLOAT, false, 0, 0)
    gl.bindBuffer(gl.ARRAY_BUFFER, @maskUVBuffer)
    gl.vertexAttribPointer(@aTextureCoord, 2, gl.FLOAT, false, 0, 0)

    gl.uniformMatrix4fv(@uMatrix, false, @maskMatrix)
    gl.uniform4fv(@uColor, @maskColor)

    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, @maskTrianglesBuffer)
  
    gl.bindFramebuffer(gl.FRAMEBUFFER, @maskFrameBuffer)
    gl.blendFunc(@srcFactor, gl.ZERO)
    gl.bindTexture(gl.TEXTURE_2D, @layerTexture)
    gl.drawElements(gl.TRIANGLES, 6, gl.UNSIGNED_BYTE, 0)
  
    gl.bindFramebuffer(gl.FRAMEBUFFER, null)
    gl.blendFunc(gl.ONE, gl.ONE_MINUS_SRC_ALPHA)
    gl.bindTexture(gl.TEXTURE_2D, @maskTexture)
    gl.drawElements(gl.TRIANGLES, 6, gl.UNSIGNED_BYTE, 0)
    return

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

  getStageSize: ->
    r = @stage.getBoundingClientRect()
    dpr = devicePixelRatio
    return [r.width * dpr, r.height * dpr]

  setBackgroundColor:(v) ->
    [r, g, b, a] = @parseBackgroundColor(v)
    gl = @stageContext
    gl.clearColor(r / 255, g / 255, b / 255, a / 255)
    return
