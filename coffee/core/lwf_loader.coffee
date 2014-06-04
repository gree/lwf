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

class LWFLoader
  readByte: ->
    return @d.charCodeAt(@index++) & 0xff

  readChar: ->
    return String.fromCharCode(@readByte())

  readBytes:(length) ->
    bytes = @d.substr(@index, length)
    @index += length
    return bytes

  readInt32: ->
    return \
      (@readByte() <<  0) +
      (@readByte() <<  8) +
      (@readByte() << 16) +
      (@readByte() << 24)

  readSingle: ->
    b3 = @readByte()
    b2 = @readByte()
    b1 = @readByte()
    b0 = @readByte()
    sign = 1 - (2 * (b0 >> 7))
    exp = (((b0 << 1) & 0xff)|(b1 >> 7)) - 127
    sig = ((b1 & 0x7f) << 16)|(b2 << 8)|b3
    return 0.0 if sig is 0 and exp is -127
    return sign * (1 + sig * Math.pow(2, -23)) * Math.pow(2, exp)

  loadTranslate: ->
    return new Translate(
      @readSingle(),
      @readSingle())

  loadMatrix: ->
    return new Matrix(
      @readSingle(),
      @readSingle(),
      @readSingle(),
      @readSingle(),
      @readSingle(),
      @readSingle())

  loadColor: ->
    return new Color(
      @readSingle(),
      @readSingle(),
      @readSingle(),
      @readSingle())

  loadAlphaTransform: ->
    return new AlphaTransform(
      @readSingle())

  loadColorTransform: ->
    multi = @loadColor()
    add = @loadColor()
    return new ColorTransform(multi.red, multi.green, multi.blue, multi.alpha,
      add.red, add.green, add.blue, add.alpha)

  loadTexture: ->
    return new Format.Texture(
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readSingle())

  loadTextureFragment: ->
    return new Format.TextureFragment(
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32())

  loadBitmap: ->
    return new Format.Bitmap(
      @readInt32(),
      @readInt32())

  loadBitmapEx: ->
    return new Format.BitmapEx(
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readSingle(),
      @readSingle(),
      @readSingle(),
      @readSingle())

  loadFont: ->
    return new Format.Font(
      @readInt32(),
      @readSingle())

  loadTextProperty: ->
    return new Format.TextProperty(
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readSingle(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32())

  loadText: ->
    return new Format.Text(
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32())

  loadParticleData: ->
    return new Format.ParticleData(
      @readInt32())

  loadParticle: ->
    return new Format.Particle(
      @readInt32(),
      @readInt32(),
      @readInt32())

  loadProgramObject: ->
    return new Format.ProgramObject(
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32())

  loadGraphicObject: ->
    return new Format.GraphicObject(
      @readInt32(),
      @readInt32())

  loadGraphic: ->
    return new Format.Graphic(
      @readInt32(),
      @readInt32())

  loadObject: ->
    return new Format.LObject(
      @readInt32(),
      @readInt32())

  loadAnimation: ->
    return new Format.Animation(
      @readInt32(),
      @readInt32())

  loadButtonCondition: ->
    return new Format.ButtonCondition(
      @readInt32(),
      @readInt32(),
      @readInt32())

  loadButton: ->
    return new Format.Button(
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32())

  loadLabel: ->
    return new Format.Label(
      @readInt32(),
      @readInt32())

  loadInstanceName: ->
    return new Format.InstanceName(
      @readInt32())

  loadEvent: ->
    return new Format.Event(
      @readInt32())

  loadString: ->
    return new Format.String(
      @readInt32(),
      @readInt32())

  loadPlace: ->
    v0 = @readInt32()
    v1 = @readInt32()
    v2 = @readInt32()
    v3 = @readInt32()
    return new Format.Place(
      v0 & 0xffffff,
      v1,
      v2,
      v3,
      v0 >> 24)

  loadControlMoveM: ->
    return new Format.ControlMoveM(
      @readInt32(),
      @readInt32())

  loadControlMoveC: ->
    return new Format.ControlMoveC(
      @readInt32(),
      @readInt32())

  loadControlMoveMC: ->
    return new Format.ControlMoveMC(
      @readInt32(),
      @readInt32(),
      @readInt32())

  loadControl: ->
    return new Format.Control(
      @readInt32(),
      @readInt32())

  loadFrame: ->
    return new Format.Frame(
      @readInt32(),
      @readInt32())

  loadMovieClipEvent: ->
    return new Format.MovieClipEvent(
      @readInt32(),
      @readInt32())

  loadMovie: ->
    return new Format.Movie(
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32(),
      @readInt32())

  loadMovieLinkage: ->
    return new Format.MovieLinkage(
      @readInt32(),
      @readInt32())

  loadItemArray: ->
    return new Format.ItemArray(
      @readInt32(),
      @readInt32())

  loadHeader: ->
    id0 = @readChar()
    id1 = @readChar()
    id2 = @readChar()
    id3 = @readChar()
    formatVersion0 = @readByte()
    formatVersion1 = @readByte()
    formatVersion2 = @readByte()
    option = @readByte()
    width = @readInt32()
    height = @readInt32()
    frameRate = @readInt32()
    rootMovieId = @readInt32()
    nameStringId = @readInt32()
    backgroundColor = @readInt32()
    stringBytes = @loadItemArray()
    animationBytes = @loadItemArray()
    translate = @loadItemArray()
    matrix = @loadItemArray()
    color = @loadItemArray()
    alphaTransform = @loadItemArray()
    colorTransform = @loadItemArray()
    objectData = @loadItemArray()
    texture = @loadItemArray()
    textureFragment = @loadItemArray()
    bitmap = @loadItemArray()
    bitmapEx = @loadItemArray()
    font = @loadItemArray()
    textProperty = @loadItemArray()
    text = @loadItemArray()
    particleData = @loadItemArray()
    particle = @loadItemArray()
    programObject = @loadItemArray()
    graphicObject = @loadItemArray()
    graphic = @loadItemArray()
    animation = @loadItemArray()
    buttonCondition = @loadItemArray()
    button = @loadItemArray()
    label = @loadItemArray()
    instanceName = @loadItemArray()
    eventData = @loadItemArray()
    place = @loadItemArray()
    controlMoveM = @loadItemArray()
    controlMoveC = @loadItemArray()
    controlMoveMC = @loadItemArray()
    control = @loadItemArray()
    frame = @loadItemArray()
    movieClipEvent = @loadItemArray()
    movie = @loadItemArray()
    movieLinkage = @loadItemArray()
    stringData = @loadItemArray()
    lwfLength = @readInt32()

    return new Format.Header(
      id0,
      id1,
      id2,
      id3,
      formatVersion0,
      formatVersion1,
      formatVersion2,
      option,
      width,
      height,
      frameRate,
      rootMovieId,
      nameStringId,
      backgroundColor,
      stringBytes,
      animationBytes,
      translate,
      matrix,
      color,
      alphaTransform,
      colorTransform,
      objectData,
      texture,
      textureFragment,
      bitmap,
      bitmapEx,
      font,
      textProperty,
      text,
      particleData,
      particle,
      programObject,
      graphicObject,
      graphic,
      animation,
      buttonCondition,
      button,
      label,
      instanceName,
      eventData,
      place,
      controlMoveM,
      controlMoveC,
      controlMoveMC,
      control,
      frame,
      movieClipEvent,
      movie,
      movieLinkage,
      stringData,
      lwfLength)

  load:(@d) ->
    @index = 0

    header = @loadHeader()
    data = new Data(header)
    return null unless data.check()

    stringBytes = @readBytes(header.stringBytes.length)
    animationBytes = @readBytes(header.animationBytes.length)

    data.translates = (@loadTranslate() for i in [0...header.translate.length])
    data.matrices = (@loadMatrix() for i in [0...header.matrix.length])
    data.colors = (@loadColor() for i in [0...header.color.length])
    data.alphaTransforms =
      (@loadAlphaTransform() for i in [0...header.alphaTransform.length])
    data.colorTransforms =
      (@loadColorTransform() for i in [0...header.colorTransform.length])
    data.objects = (@loadObject() for i in [0...header.objectData.length])
    data.textures = (@loadTexture() for i in [0...header.texture.length])
    data.textureFragments =
      (@loadTextureFragment() for i in [0...header.textureFragment.length])
    data.bitmaps = (@loadBitmap() for i in [0...header.bitmap.length])
    data.bitmapExs = (@loadBitmapEx() for i in [0...header.bitmapEx.length])
    data.fonts = (@loadFont() for i in [0...header.font.length])
    data.textProperties =
      (@loadTextProperty() for i in [0...header.textProperty.length])
    data.texts = (@loadText() for i in [0...header.text.length])
    data.particleDatas =
      (@loadParticleData() for i in [0...header.particleData.length])
    data.particles = (@loadParticle() for i in [0...header.particle.length])
    data.programObjects =
      (@loadProgramObject() for i in [0...header.programObject.length])
    data.graphicObjects =
      (@loadGraphicObject() for i in [0...header.graphicObject.length])
    data.graphics = (@loadGraphic() for i in [0...header.graphic.length])
    animations = (@loadAnimation() for i in [0...header.animation.length])
    data.buttonConditions =
      (@loadButtonCondition() for i in [0...header.buttonCondition.length])
    data.buttons = (@loadButton() for i in [0...header.button.length])
    data.labels = (@loadLabel() for i in [0...header.label.length])
    data.instanceNames =
      (@loadInstanceName() for i in [0...header.instanceName.length])
    data.events = (@loadEvent() for i in [0...header.eventData.length])
    data.places = (@loadPlace() for i in [0...header.place.length])
    data.controlMoveMs =
      (@loadControlMoveM() for i in [0...header.controlMoveM.length])
    data.controlMoveCs =
      (@loadControlMoveC() for i in [0...header.controlMoveC.length])
    data.controlMoveMCs =
      (@loadControlMoveMC() for i in [0...header.controlMoveMC.length])
    data.controls = (@loadControl() for i in [0...header.control.length])
    data.frames = (@loadFrame() for i in [0...header.frame.length])
    data.movieClipEvents =
      (@loadMovieClipEvent() for i in [0...header.movieClipEvent.length])
    data.movies = (@loadMovie() for i in [0...header.movie.length])
    data.movieLinkages =
      (@loadMovieLinkage() for i in [0...header.movieLinkage.length])
    stringDatas = (@loadString() for i in [0...header.stringData.length])

    data.animations = []
    for a in animations
      o = a.animationOffset
      data.animations.push @readAnimation(
        animationBytes.slice(o, o + a.animationLength))

    data.strings = []
    stringMap = data.stringMap
    for a in stringDatas
      o = a.stringOffset
      s = stringBytes.slice(o, o + a.stringLength)
      str = ""
      i = 0
      while i < s.length
        c = s.charCodeAt(i) & 255
        if c < 128
          str += String.fromCharCode(c)
          ++i
        else if (c >> 5) == 6
          c2 = s.charCodeAt(i + 1)
          str += String.fromCharCode(((c & 31) << 6)|(c2 & 63))
          i += 2
        else if (c >> 4) == 14
          c2 = s.charCodeAt(i + 1)
          c3 = s.charCodeAt(i + 2)
          code = ((c & 15) << 12)|((c2 & 63) << 6)|(c3 & 63)
          str += String.fromCharCode(code)
          i += 3
        else
          c2 = s.charCodeAt(i + 1)
          c3 = s.charCodeAt(i + 2)
          c4 = s.charCodeAt(i + 3)
          code = ((c & 7) << 18)|((c2 & 63) << 12)|((c3 & 63) << 6)|(c4 << 63)
          str += String.fromCharCode(code)
          i += 4
      stringMap[str] = data.strings.length
      data.strings.push str

    instanceNameMap = data.instanceNameMap
    for i in [0...data.instanceNames.length]
      instanceNameMap[data.instanceNames[i].stringId] = i

    eventMap = data.eventMap
    for i in [0...data.events.length]
      eventMap[data.events[i].stringId] = i

    movieLinkageMap = data.movieLinkageMap
    for i in [0...data.movieLinkages.length]
      movieLinkageMap[data.movieLinkages[i].stringId] = i

    movieLinkageNameMap = data.movieLinkageNameMap
    for i in [0...data.movieLinkages.length]
      movieLinkageNameMap[data.movieLinkages[i].movieId] =
        data.movieLinkages[i].stringId

    programObjectMap = data.programObjectMap
    for i in [0...data.programObjects.length]
      programObjectMap[data.programObjects[i].stringId] = i

    labelMap = data.labelMap
    for m in data.movies
      o = m.labelOffset
      map = {}
      for l in data.labels[o...(o + m.labels)]
        map[l.stringId] = l.frameNo
      labelMap.push map

    t.setFilename(data) for t in data.textures

    bitmapMap = data.bitmapMap
    for i in [0...data.textureFragments.length]
      t = data.textureFragments[i]
      t.setFilename(data)
      filename = t.filename
      m = filename.match(/(.+)_atlas_.+_info_.+(_.+){6}/)
      filename = m[1] if m?
      bitmapMap[filename] = data.bitmaps.length
      data.bitmaps.push(new Format.Bitmap(0, i))

    return data

  readByteFromBytes:(bytes, index) ->
    b = bytes.charCodeAt(index++) & 0xff
    return [index, b]

  readInt32FromBytes:(bytes, index) ->
    [index, b0] = @readByteFromBytes(bytes, index)
    [index, b1] = @readByteFromBytes(bytes, index)
    [index, b2] = @readByteFromBytes(bytes, index)
    [index, b3] = @readByteFromBytes(bytes, index)
    i = (b0 <<  0) +
        (b1 <<  8) +
        (b2 << 16) +
        (b3 << 24)
    return [index, i]

  readAnimation:(bytes) ->
    array = []
    index = 0
    loop
      [index, code] = @readByteFromBytes(bytes, index)
      array.push code

      switch code
        when Animation.GOTOFRAME, \
            Animation.GOTOLABEL, Animation.EVENT, Animation.CALL
          [index, i] = @readInt32FromBytes(bytes, index)
          array.push i

        when Animation.SETTARGET
          [index, count] = @readInt32FromBytes(bytes, index)
          array.push count
          for i in [0...count]
            [index, target] = @readInt32FromBytes(bytes, index)
            array.push target

        when Animation.END
          return array

class LWFLoaderWithArray extends LWFLoader
  readByte: ->
    return @d[@index++]

  readBytes:(length) ->
    bytes = ""
    for i in [0...length]
      bytes += String.fromCharCode(@d[@index++])
    return bytes

class LWFLoaderWithArrayBuffer extends LWFLoader
  constructor: ->
    @int32Array = new Int32Array()
    @float32Array = new Float32Array()

  readByte: ->
    return @d[@index++]

  readBytes:(length) ->
    bytes = ""
    for i in [0...length]
      bytes += String.fromCharCode(@d[@index++])
    return bytes

  readInt32: ->
    i = @dInt32[@index / 4]
    @index += 4
    return i

  readSingle: ->
    f = @dFloat32[@index / 4]
    @index += 4
    return f

  load:(d) ->
    @d = new Uint8Array(d)
    @dInt32 = new Int32Array(d)
    @dFloat32 = new Float32Array(d)
    super(@d)

class Loader
  @load:(d) ->
    return if !d? or typeof d isnt "string"
    lwfLoader = new LWFLoader
    return lwfLoader.load(d)

  @loadArray:(d) ->
    return if !d?
    lwfLoader = new LWFLoaderWithArray
    return lwfLoader.load(d)

  @loadArrayBuffer:(d) ->
    return if !d?
    lwfLoader = new LWFLoaderWithArrayBuffer
    return lwfLoader.load(d)
