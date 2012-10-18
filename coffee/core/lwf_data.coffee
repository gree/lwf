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

class Data
  constructor:(@header, @translates, @matrices, @colors, @alphaTransforms, \
      @colorTransforms, @objects, @textures, @textureFragments, @bitmaps, \
      @bitmapExs, @fonts, @textProperties, @texts, @particleDatas, @particles, \
      @programObjects, @graphicObjects, @graphics, @animations, \
      @buttonConditions, @buttons, @labels, @instanceNames, @events, @places, \
      @controlMoveMs, @controlMoveCs, @controlMoveMCs, @controls, @frames, \
      @movieClipEvents, @movies, @movieLinkages, @strings) ->

    if @header.header?

      d = @header
      @header = d.header
      @translates = d.translates
      @matrices = d.matrices
      @colors = d.colors
      @alphaTransforms = d.alphaTransforms
      @colorTransforms = d.colorTransforms
      @objects = d.objects
      @textures = d.textures
      @textureFragments = d.textureFragments
      @bitmaps = d.bitmaps
      @bitmapExs = d.bitmapExs
      @fonts = d.fonts
      @textProperties = d.textProperties
      @texts = d.texts
      @particleDatas = d.particleDatas
      @particles = d.particles
      @programObjects = d.programObjects
      @graphicObjects = d.graphicObjects
      @graphics = d.graphics
      @animations = d.animations
      @buttonConditions = d.buttonConditions
      @buttons = d.buttons
      @labels = d.labels
      @instanceNames = d.instanceNames
      @events = d.events
      @places = d.places
      @controlMoveMs = d.controlMoveMs
      @controlMoveCs = d.controlMoveCs
      @controlMoveMCs = d.controlMoveMCs
      @controls = d.controls
      @frames = d.frames
      @movieClipEvents = d.movieClipEvents
      @movies = d.movies
      @movieLinkages = d.movieLinkages
      @strings = d.strings
      @stringMap = d.stringMap
      @eventMap = d.eventMap
      @instanceNameMap = d.instanceNameMap
      @movieLinkageMap = d.movieLinkageMap
      @movieLinkageNameMap = d.movieLinkageNameMap
      @programObjectMap = d.programObjectMap
      @labelMap = d.labelMap
      @useScript = d.useScript
      @useTextureAtlas = d.useTextureAtlas

    else

      @stringMap = {}
      @eventMap = {}
      @instanceNameMap = {}
      @movieLinkageMap = {}
      @movieLinkageNameMap = {}
      @programObjectMap = {}
      @labelMap = []
      if @header?
        @useScript = (@header.option & Format.Constant.OPTION_USE_SCRIPT) != 0
        @useTextureAtlas =
          (@header.option & Format.Constant.OPTION_USE_TEXTUREATLAS) != 0
      else
        @useScript = false
        @useTextureAtlas = false

  check: ->
    if @header? and
        @header.id0 is 'L' and
        @header.id1 is 'W' and
        @header.id2 is 'F' and
        @header.formatVersion0 is Format.Constant.FORMAT_VERSION_0 and
        @header.formatVersion1 is Format.Constant.FORMAT_VERSION_1 and
        @header.formatVersion2 is Format.Constant.FORMAT_VERSION_2
      return true
    else
      return false

  name: ->
    return @strings[@header.nameStringId]

  replaceTexture:(index, textureReplacement) ->
    return false if index < 0 or index >= textures.length
    textures[index] = textureReplacement
    return true

  replaceTextureFragment:(index, textureFragmentReplacement) ->
    return false if index < 0 or index >= textureFragments.length
    textureFragments[index] = textureFragmentReplacement
    return true
