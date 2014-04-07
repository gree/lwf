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

class Format
  class @Constant
    @HEADER_SIZE = 324

    @FORMAT_VERSION_0 = 0x13
    @FORMAT_VERSION_1 = 0x12
    @FORMAT_VERSION_2 = 0x11

    @FORMAT_VERSION_COMPAT_0 = 0x12
    @FORMAT_VERSION_COMPAT_1 = 0x10
    @FORMAT_VERSION_COMPAT_2 = 0x10

    @FORMAT_TYPE = 0

    @OPTION_OFFSET = 7

    @OPTION_USE_SCRIPT = (1 << 0)
    @OPTION_USE_TEXTUREATLAS = (1 << 1)
    @OPTION_COMPRESSED = (1 << 2)

    @MATRIX_FLAG = (1 << 31)
    @COLORTRANSFORM_FLAG = (1 << 31)

    @TEXTUREFORMAT_NORMAL = 0
    @TEXTUREFORMAT_PREMULTIPLIEDALPHA = 1

    @BLEND_MODE_NORMAL = 0
    @BLEND_MODE_ADD = 1
    @BLEND_MODE_LAYER = 2
    @BLEND_MODE_ERASE = 3
    @BLEND_MODE_MASK = 4
    @BLEND_MODE_MULTIPLY = 5
    @BLEND_MODE_SCREEN = 6

  class @StringBase
    constructor:(@stringId) ->

  class @Texture
    constructor:(@stringId, @format, @width, @height, @scale) ->

    setFilename:(data) ->
      @filename = data.strings[@stringId]
      return

  class @TextureReplacement
    constructor:(@filename, @format, @width, @height, @scale) ->

  class @TextureFragment
    constructor:(@stringId, @textureId, @rotated, @x, @y, @u, @v, @w, @h) ->

    setFilename:(data) ->
      @filename = data.strings[@stringId]
      return

  class @TextureFragmentReplacement
    constructor:(@filename, @textureId, @rotated, @x, @y, @u, @v, @w, @h) ->

  class @Bitmap
    constructor:(@matrixId, @textureFragmentId) ->

  class @BitmapEx
    class @Attribute
      @REPEAT_S = (1 << 0)
      @REPEAT_T = (1 << 1)

    constructor:(@matrixId, @textureFragmentId, @attribute, @u, @v, @w, @h) ->

  class @Font
    constructor:(@stringId, @letterSpacing) ->

  class @TextProperty
    class @Align
      @LEFT = 0
      @RIGHT = 1
      @CENTER = 2
      @ALIGN_MASK = 0x3
      @VERTICAL_BOTTOM = (1 << 2)
      @VERTICAL_MIDDLE = (2 << 2)
      @VERTICAL_MASK = 0xc

    constructor:(@maxLength, @fontId, @fontHeight, @align, \
        @leftMargin, @rightMargin, @letterSpacing, @leading, \
        @strokeColorId, @strokeWidth, \
        @shadowColorId, @shadowOffsetX, @shadowOffsetY, @shadowBlur) ->

  class @Text
    constructor:(@matrixId, \
        @nameStringId, @textPropertyId, @stringId, @colorId, @width, @height) ->

  class @ParticleData
    constructor:(@stringId) ->

  class @Particle
    constructor:(@matrixId, @colorTransformId, @particleDataId) ->

  class @ProgramObject extends @StringBase
    constructor:(stringId, @width, @height, @matrixId, @colorTransformId) ->
      super(stringId)

  class @GraphicObject
    class @Type
      @BITMAP = 0
      @BITMAPEX = 1
      @TEXT = 2
      @GRAPHIC_OBJECT_MAX = 3

    constructor:(@graphicObjectType, @graphicObjectId) ->

  class @Graphic
    constructor:(@graphicObjectId, @graphicObjects) ->

  class @LObject
    class @Type
      @BUTTON = 0
      @GRAPHIC = 1
      @MOVIE = 2
      @BITMAP = 3
      @BITMAPEX = 4
      @TEXT = 5
      @PARTICLE = 6
      @PROGRAMOBJECT = 7
      @ATTACHEDMOVIE = 8
      @OBJECT_MAX = 9

    constructor:(@objectType, @objectId) ->

  class @Animation
    constructor:(@animationOffset, @animationLength) ->

  class @ButtonCondition
    class @Condition
      @ROLLOVER       = (1 << 0)
      @ROLLOUT        = (1 << 1)
      @PRESS          = (1 << 2)
      @RELEASE        = (1 << 3)
      @DRAGOUT        = (1 << 4)
      @DRAGOVER       = (1 << 5)
      @RELEASEOUTSIDE = (1 << 6)
      @KEYPRESS       = (1 << 7)

    constructor:(@condition, @keyCode, @animationId) ->

  class @Button
    constructor:(@width, @height, \
      @matrixId, @colorTransformId, @conditionId, @conditions) ->

  class @Label extends @StringBase
    constructor:(stringId, @frameNo) ->
      super(stringId)

  class @InstanceName extends @StringBase
    constructor:(stringId) ->
      super(stringId)

  class @Event extends @StringBase
    constructor:(stringId) ->
      super(stringId)

  class @String
    constructor:(@stringOffset, @stringLength) ->

  class @Place
    constructor:(@depth, @objectId, @instanceId, @matrixId, @blendMode) ->

  class @ControlMoveM
    constructor:(@placeId, @matrixId) ->

  class @ControlMoveC
    constructor:(@placeId, @colorTransformId) ->

  class @ControlMoveMC
    constructor:(@placeId, @matrixId, @colorTransformId) ->

  class @Control
    class @Type
      @MOVE = 0
      @MOVEM = 1
      @MOVEC = 2
      @MOVEMC = 3
      @ANIMATION = 4
      @CONTROL_MAX = 5

    constructor:(@controlType, @controlId) ->

  class @Frame
    constructor:(@controlOffset, @controls) ->

  class @MovieClipEvent
    class @ClipEvent
      @LOAD = (1 << 0)
      @UNLOAD = (1 << 1)
      @ENTERFRAME = (1 << 2)

    constructor:(@clipEvent, @animationId) ->

  class @Movie
    constructor:(@depths, @labelOffset, \
      @labels, @frameOffset, @frames, @clipEventId, @clipEvents) ->

  class @MovieLinkage extends @StringBase
    constructor:(stringId, @movieId) ->
      super(stringId)

  class @ItemArray
    constructor:(@offset, @length) ->

  class @Header
    constructor:( \
      @id0, \
      @id1, \
      @id2, \
      @id3, \
      @formatVersion0, \
      @formatVersion1, \
      @formatVersion2, \
      @option, \
      @width, \
      @height, \
      @frameRate, \
      @rootMovieId, \
      @nameStringId, \
      @backgroundColor, \
      @stringBytes, \
      @animationBytes, \
      @translate, \
      @matrix, \
      @color, \
      @alphaTransform, \
      @colorTransform, \
      @objectData, \
      @texture, \
      @textureFragment, \
      @bitmap, \
      @bitmapEx, \
      @font, \
      @textProperty, \
      @text, \
      @particleData, \
      @particle, \
      @programObject, \
      @graphicObject, \
      @graphic, \
      @animation, \
      @buttonCondition, \
      @button, \
      @label, \
      @instanceName, \
      @eventData, \
      @place, \
      @controlMoveM, \
      @controlMoveC, \
      @controlMoveMC, \
      @control, \
      @frame, \
      @movieClipEvent, \
      @movie, \
      @movieLinkage, \
      @stringData, \
      @lwfLength) ->

Align = Format.TextProperty.Align
ClipEvent = Format.MovieClipEvent.ClipEvent
Condition = Format.ButtonCondition.Condition
Constant = Format.Constant
Type = Format.LObject.Type
ControlType = Format.Control.Type
GObjType = Format.GraphicObject.Type
