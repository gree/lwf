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

class LObject
  constructor:(@lwf, @parent, @type, @objectId) ->
    @matrixId = null
    @colorTransformId = null
    @matrixIdChanged = true
    @colorTransformIdChanged = true
    @matrix = new Matrix(0, 0, 0, 0, 0, 0)
    @colorTransform = new ColorTransform(0, 0, 0, 0)
    @execCount = 0
    @updated = false

    @isButton = @type is Type.BUTTON
    @isMovie = @type is Type.MOVIE or @type is Type.ATTACHEDMOVIE
    @isParticle = @type is Type.PARTICLE
    @isProgramObject = @type is Type.PROGRAMOBJECT
    @isText = @type is Type.TEXT

  exec:(matrixId = 0, colorTransformId = 0) ->
    if @matrixId isnt matrixId
      @matrixIdChanged = true
      @matrixId = matrixId
    if @colorTransformId isnt colorTransformId
      @colorTransformIdChanged = true
      @colorTransformId = colorTransformId
    return

  update:(m, c) ->
    @updated = true
    if m isnt null
      Utility.calcMatrixId(@lwf, @matrix, m, @dataMatrixId)
      @matrixIdChanged = false
    if c isnt null
      Utility.copyColorTransform(@colorTransform, c)
      @colorTransformIdChanged = false
    @lwf.renderObject()
    return

  render:(v, rOffset) ->
    if @renderer?
      rIndex = @lwf.renderingIndex
      rIndexOffsetted = @lwf.renderingIndexOffsetted
      rCount = @lwf.renderingCount
      if rOffset isnt Number.MIN_VALUE
        rIndex = rIndexOffsetted - rOffset + rCount
      @renderer.render(@matrix, @colorTransform, rIndex, rCount, v)
    @lwf.renderObject()
    return

  inspect:(inspector, hierarchy, depth, rOffset) ->
    rIndex = @lwf.renderingIndex
    rIndexOffsetted = @lwf.renderingIndexOffsetted
    rCount = @lwf.renderingCount
    if rOffset isnt Number.MIN_VALUE
      rIndex = rIndexOffsetted + rOffset + rCount
    inspector(@, hierarchy, depth, rIndex)
    @lwf.renderObject()
    return

  destroy: ->
    if @renderer
      @renderer.destruct()
      @renderer = null
    @parent = null
    @lwf = null
    return
