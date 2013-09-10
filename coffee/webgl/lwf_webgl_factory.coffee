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

  setupGL: ->
    gl = @stageContext
    gl.enable(gl.BLEND)
    gl.disable(gl.DEPTH_TEST)
    gl.disable(gl.DITHER)
    gl.disable(gl.SCISSOR_TEST)
    gl.activeTexture(gl.TEXTURE0)
    gl.useProgram(@shaderProgram)
    return

  setTexParameter:(gl) ->
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE)
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE)
    return

  setViewport:(gl, lwf) ->
    r = @stage.getBoundingClientRect()
    changed = @clientRect isnt r
    changed |= @propertyMatrix.setWithComparing(lwf.property.matrix)
    if changed
      @clientRect = r
      dpr = devicePixelRatio
      @w = Math.round(r.width * dpr)
      @h = Math.round(r.height * dpr)
      gl.viewport(0, 0, @w, @h)
      #gl.scissor(@propertyMatrix.translateX, @propertyMatrix.translateY,
      #  @data.header.width * @propertyMatrix.scaleX,
      #  @data.header.height * @propertyMatrix.scaleY)

      right = @w
      left = 0
      top = 0
      bottom = @h
      far = 1
      near = -1
      pmatrix = [
        2 / (right - left), 0, 0, 0,
        0, 2 / (top - bottom), 0, 0,
        0, 0, -2 / (far - near), 0,
        -(right + left) / (right - left), -(top + bottom) / (top - bottom),
          -(far + near) / (far - near), 1
      ]
      gl.uniformMatrix4fv(@uPMatrix, false, new Float32Array(pmatrix))

  constructor:(@data, @resourceCache,
      @cache, @stage, @textInSubpixel, @needsClear, @useVertexColor) ->
    params = {premultipliedAlpha:true, alpha:false}
    @drawCalls = 0
    @stage.style.webkitUserSelect = "none"
    @stageContext = @stage.getContext("webgl", params) ?
      @stage.getContext("experimental-webgl", params)
    if @stage.width is 0 and @stage.height is 0
      @stage.width = @data.header.width
      @stage.height = @data.header.height
    @blendMode = "normal"
    @maskMode = "normal"
    @clientRect = null
    @propertyMatrix = new Matrix
    @vertexData = new Float32Array(1)
    @indexData = new Uint16Array(1)

    gl = @stageContext
    @vertexBufferSize = 3 * 4 + 2 * 4 + 4 * 4
    @vertexBuffer = gl.createBuffer()
    @indexBuffer = gl.createBuffer()
    @matrix = new Float32Array([1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1])

    if WebGLRendererFactory.shaderProgram?
      @shaderProgram = WebGLRendererFactory.shaderProgram
    else
      vertexShader = @loadShader(gl, gl.VERTEX_SHADER, """
        attribute vec4 aVertexPosition;
        attribute vec2 aTextureCoord;
        attribute vec4 aColor;
        uniform mat4 uPMatrix;
        uniform mat4 uMatrix;
        varying lowp vec2 vTextureCoord;
        varying lowp vec4 vColor;
        void main() {
          gl_Position = uPMatrix * uMatrix * aVertexPosition;
          vTextureCoord = aTextureCoord;
          vColor = aColor;
        }
        """)

      fragmentShader = @loadShader(gl, gl.FRAGMENT_SHADER, """
        varying lowp vec2 vTextureCoord;
        varying lowp vec4 vColor;
        uniform sampler2D uTexture;
        void main() {
          gl_FragColor = vColor * texture2D(uTexture, vTextureCoord);
        }
        """)

      @shaderProgram = gl.createProgram()
      gl.attachShader(@shaderProgram, vertexShader)
      gl.attachShader(@shaderProgram, fragmentShader)
      gl.linkProgram(@shaderProgram)
      unless gl.getProgramParameter(@shaderProgram, gl.LINK_STATUS)
        alert("Unable to initialize the shader program.")
      WebGLRendererFactory.shaderProgram = @shaderProgram

    @aVertexPosition = gl.getAttribLocation(@shaderProgram, "aVertexPosition")
    @aTextureCoord = gl.getAttribLocation(@shaderProgram, "aTextureCoord")
    @aColor = gl.getAttribLocation(@shaderProgram, "aColor")
    @uPMatrix = gl.getUniformLocation(@shaderProgram, "uPMatrix")
    @uMatrix = gl.getUniformLocation(@shaderProgram, "uMatrix")
    @uTexture = gl.getUniformLocation(@shaderProgram, "uTexture")

    @setupGL()
    gl.clearColor(0.0, 0.0, 0.0, 1.0)

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
    gl = @stageContext
    @deleteMask(gl)
    gl.deleteBuffer(@indexBuffer)
    gl.deleteBuffer(@vertexBuffer)
    gl.deleteTexture(d[0]) for k, d in @textures
    context.destruct() for context in @bitmapContexts
    context.destruct() for context in @bitmapExContexts
    context.destruct() for context in @textContexts
    return

  beginRender:(lwf) ->
    super
    @lwf = lwf
    @drawCalls = 0
    return if lwf.parent?
    @currentTexture = null
    @currentBlendMode = "normal"
    @faces = 0
    gl = @stageContext
    @setViewport(gl, lwf)
    gl.clear(gl.COLOR_BUFFER_BIT) if @needsClear
    return

  addCommand:(rIndex, cmd) ->
    super
    return if @lwf.parent?
    ++@faces if cmd.renderer is null
    return

  endRender:(lwf) ->
    if lwf.parent?
      parent = lwf.parent
      parent = parent.parent while parent.parent?
      f = parent.lwf.rendererFactory
      f.addCommand(parseInt(rIndex, 10), cmd) for rIndex, cmd of @commands
      @initCommands()
      return

    gl = @stageContext

    vertices = @faces * 4 * 9
    @faces = 0
    if vertices > @vertexData.length #or vertices < @vertexData.length / 6
      vertices *= 2
      vertices = 65532 * 9 if vertices > 65532 * 9
      indices = vertices / (4 * 9) * 6
      if vertices isnt @vertexData.length
        @vertexData = new Float32Array(vertices)
        @indexData = new Uint16Array(indices)
        offset = 0
        indexOffset = 0
        for i in [0...indices / 6]
          @indexData[offset + 0] = indexOffset + 0
          @indexData[offset + 1] = indexOffset + 1
          @indexData[offset + 2] = indexOffset + 2
          @indexData[offset + 3] = indexOffset + 2
          @indexData[offset + 4] = indexOffset + 1
          @indexData[offset + 5] = indexOffset + 3
          offset += 6
          indexOffset += 4
        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, @indexBuffer)
        gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, @indexData, gl.STATIC_DRAW)

    @renderMaskMode = "normal"
    @renderMasked = false
    for rIndex in @commandsKeys
      cmd = @commands[rIndex]
      if cmd.subCommandsKeys?
        for srIndex in cmd.subCommandsKeys
          scmd = cmd.subCommands[srIndex]
          @render(gl, scmd, srIndex)
      @render(gl, cmd, rIndex)
    @renderMesh(gl)

    if @renderMaskMode isnt "normal"
      if @renderMaskMode is "layer" and @renderMasked
        @renderMask(gl)
      else
        gl.bindFramebuffer(gl.FRAMEBUFFER, null)

    @initCommands()
    return

  render:(gl, cmd, rIndex) ->
    if @renderMaskMode isnt cmd.maskMode
      @renderMesh(gl)
      @generateMask(gl)
      switch cmd.maskMode
        when "erase", "mask"
          @renderMask(gl) if @renderMaskMode is "layer" and @renderMasked
          @renderMasked = true
          @maskSrcFactor = if cmd.maskMode is "erase" then \
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
            @renderMask(gl)
          else
            gl.bindFramebuffer(gl.FRAMEBUFFER, null)
      @renderMaskMode = cmd.maskMode

    if cmd.renderer is null
      @updateMesh(gl, cmd.context, cmd.texture, cmd.matrix, cmd.colorTransform,
        rIndex, cmd.blendMode, cmd.meshCache, cmd.useMeshCache)
    else
      cmd.renderer.renderCommand(
        cmd.matrix, cmd.colorTransform, rIndex, cmd.blendMode)
    return

  updateMesh:(gl, context,
      texture, m, c, renderingIndex, blendMode, meshCache, useMeshCache) ->
    if texture isnt @currentTexture or blendMode isnt @currentBlendMode or
        @faces * 4 * 9 >= @vertexData.length
      @renderMesh(gl)
      @currentTexture = texture
      @currentBlendMode = blendMode
      @blendSrcFactor =
        if context.preMultipliedAlpha then gl.ONE else gl.SRC_ALPHA
      @blendDstFactor =
        if blendMode is "add" then gl.ONE else gl.ONE_MINUS_SRC_ALPHA
      @faces = 0

    alpha = c.multi.alpha
    if @useVertexColor
      red = c.multi.red
      green = c.multi.green
      blue = c.multi.blue
      if context.preMultipliedAlpha
        red *= alpha
        green *= alpha
        blue *= alpha
    else
      red = 1
      green = 1
      blue = 1

    if useMeshCache
      for i in [0...4]
        offset = i * 9
        meshCache[offset + 5] = red
        meshCache[offset + 6] = green
        meshCache[offset + 7] = blue
        meshCache[offset + 8] = alpha
    else
      scaleX = m.scaleX
      skew0 = m.skew0
      translateX = m.translateX

      skew1 = m.skew1
      scaleY = m.scaleY
      translateY = m.translateY

      translateZ = 0

      vertexData = context.vertexData
      uv = context.uv
      for i in [0...4]
        x = vertexData[i].x
        y = vertexData[i].y

        px = x * scaleX + y * skew0 + translateX
        py = x * skew1 + y * scaleY + translateY
        pz = translateZ

        offset = i * 9
        meshCache[offset + 0] = px
        meshCache[offset + 1] = py
        meshCache[offset + 2] = pz
        meshCache[offset + 3] = uv[i].u
        meshCache[offset + 4] = uv[i].v
        meshCache[offset + 5] = red
        meshCache[offset + 6] = green
        meshCache[offset + 7] = blue
        meshCache[offset + 8] = alpha

    offset = @faces++ * 4 * 9
    for i in [0...4 * 9]
      @vertexData[offset + i] = meshCache[i]
    return

  renderMesh:(gl) ->
    return if @currentTexture is null or @faces is 0

    gl.bindTexture(gl.TEXTURE_2D, @currentTexture)
    gl.blendFunc(@blendSrcFactor, @blendDstFactor)

    gl.bindBuffer(gl.ARRAY_BUFFER, @vertexBuffer)
    gl.bufferData(gl.ARRAY_BUFFER, @vertexData, gl.DYNAMIC_DRAW)

    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, @indexBuffer)

    gl.vertexAttribPointer(
      @aVertexPosition, 3, gl.FLOAT, false, @vertexBufferSize, 0)
    gl.vertexAttribPointer(
      @aTextureCoord, 2, gl.FLOAT, false, @vertexBufferSize, 12)
    gl.vertexAttribPointer(
      @aColor, 4, gl.FLOAT, false, @vertexBufferSize, 20)

    gl.enableVertexAttribArray(@aVertexPosition)
    gl.enableVertexAttribArray(@aTextureCoord)
    gl.enableVertexAttribArray(@aColor)

    gl.uniformMatrix4fv(@uMatrix, false, @matrix)

    gl.drawElements(gl.TRIANGLES, @faces * 6, gl.UNSIGNED_SHORT, 0)
    ++@drawCalls
    return

  setBlendMode:(@blendMode) ->

  setMaskMode:(@maskMode) ->

  generateMask:(gl) ->
    return if @maskTexture? and
      @maskTextureWidth is @w and @maskTextureHeight is @h

    @maskMatrix = new Float32Array([1,0,0,0,0,-1,0,0,0,0,1,0,0,@h,0,1])

    @maskTexture = gl.createTexture()
    @maskTextureWidth = @w
    @maskTextureHeight = @h
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
      @setTexParameter(gl)

      framebuffer = framebuffers[i]
      gl.bindFramebuffer(gl.FRAMEBUFFER, framebuffer)
      gl.framebufferTexture2D(gl.FRAMEBUFFER,
        gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, texture, 0)
      gl.bindFramebuffer(gl.FRAMEBUFFER, null)

    buffer = new Float32Array([
      #x,  y, z, u, v, r, g, b, a
      @w, @h, 0, 1, 1, 1, 1, 1, 1,
      @w,  0, 0, 1, 0, 1, 1, 1, 1,
       0, @h, 0, 0, 1, 1, 1, 1, 1,
       0,  0, 0, 0, 0, 1, 1, 1, 1,
    ])
    @maskVertexBuffer = gl.createBuffer()
    gl.bindBuffer(gl.ARRAY_BUFFER, @maskVertexBuffer)
    gl.bufferData(gl.ARRAY_BUFFER, buffer, gl.STATIC_DRAW)

    indexData = new Uint8Array([
      0, 1, 2,
      2, 1, 3,
    ])
    @maskIndexBuffer = gl.createBuffer()
    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, @maskIndexBuffer)
    gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, indexData, gl.STATIC_DRAW)
    return

  deleteMask:(gl) ->
    return unless @maskTexture?

    gl.deleteBuffer(@maskVertexBuffer)
    gl.deleteBuffer(@maskIndexBuffer)

    gl.deleteFramebuffer(@maskFrameBuffer)
    gl.deleteFramebuffer(@layerFrameBuffer)
    @maskFrameBuffer = null
    @layerFrameBuffer = null

    gl.deleteTexture(@maskTexture)
    gl.deleteTexture(@layerTexture)
    @maskTexture = null
    @layerTexture = null
    return

  renderMask:(gl) ->
    gl.bindBuffer(gl.ARRAY_BUFFER, @maskVertexBuffer)
    gl.vertexAttribPointer(
      @aVertexPosition, 3, gl.FLOAT, false, @vertexBufferSize, 0)
    gl.vertexAttribPointer(
      @aTextureCoord, 2, gl.FLOAT, false, @vertexBufferSize, 12)
    gl.vertexAttribPointer(
      @aColor, 4, gl.FLOAT, false, @vertexBufferSize, 20)

    gl.enableVertexAttribArray(@aVertexPosition)
    gl.enableVertexAttribArray(@aTextureCoord)
    gl.enableVertexAttribArray(@aColor)

    gl.uniformMatrix4fv(@uMatrix, false, @maskMatrix)

    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, @maskIndexBuffer)
  
    gl.bindFramebuffer(gl.FRAMEBUFFER, @maskFrameBuffer)
    gl.blendFunc(@maskSrcFactor, gl.ZERO)
    gl.bindTexture(gl.TEXTURE_2D, @layerTexture)
    gl.drawElements(gl.TRIANGLES, 6, gl.UNSIGNED_BYTE, 0)
    ++@drawCalls
  
    gl.bindFramebuffer(gl.FRAMEBUFFER, null)
    gl.blendFunc(gl.ONE, gl.ONE_MINUS_SRC_ALPHA)
    gl.bindTexture(gl.TEXTURE_2D, @maskTexture)
    gl.drawElements(gl.TRIANGLES, 6, gl.UNSIGNED_BYTE, 0)
    ++@drawCalls
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

