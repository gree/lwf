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

class Movie extends IObject
  constructor:(lwf, parent, objId, instId, matrixId = null,
      colorTransformId = null, attached = false, handler = null, n = null) ->
    type = if attached then Type.ATTACHEDMOVIE else Type.MOVIE
    super(lwf, parent, type, objId, instId)
    @name = n if n?
    @matrixId = matrixId
    @colorTransformId = colorTransformId
    @data = lwf.data.movies[objId]
    @totalFrames = @data.frames
    @instanceHead = null
    @instanceTail = null
    @currentFrameInternal = -1
    @execedFrame = -1
    @animationPlayedFrame = -1
    @lastControlOffset = -1
    @lastControls = -1
    @lastHasButton = false
    @lastControlAnimationOffset = -1
    @skipped = false
    @postLoaded = false
    @active = true
    @visible = true
    @playing = true
    @jumped = false
    @overriding = false
    @attachMovieExeced = false
    @attachMoviePostExeced = false
    @movieExecCount = -1
    @postExecCount = -1
    @eventHandlers = {}
    @requestedCalculateBounds = false
    @calculateBoundsCallback = null

    @property = new Property(lwf)

    @matrix0 = new Matrix
    @matrix1 = new Matrix
    @colorTransform0 = new ColorTransform
    @colorTransform1 = new ColorTransform
    @blendMode = "normal"

    @displayList = []
    @attachName = null
    @depth = null
    @hasButton = false

    @getMovieFunctions()

    if objId is @lwf.data.header.rootMovieId
      func = @lwf.functions?['init']
      func.call(@) if func?

    @loadFunc.call(@) if @loadFunc?
    @playAnimation(ClipEvent.LOAD)

    @handler = new MovieEventHandlers
    @handler.concat(lwf.getMovieEventHandlers(@))
    @handler.concat(handler)
    @handler.call("load", @) unless @handler.empty
    lwf.execMovieCommand()

  getMovieFunctions: ->
    [@loadFunc, @postLoadFunc, @unloadFunc, @enterFrameFunc] =
      @lwf.getMovieFunctions(@objectId)
    return

  setHandlers:(handler) ->
    @handler.concat(handler)
    return

  play: ->
    @playing = true
    return @

  stop: ->
    @playing = false
    return @

  nextFrame: ->
    @jumped = true
    @stop()
    ++@currentFrameInternal if @currentFrameInternal < @totalFrames - 1
    return @

  prevFrame: ->
    @jumped = true
    @stop()
    --@currentFrameInternal if @currentFrameInternal > 0
    return @

  gotoFrame:(frameNo) ->
    return @gotoFrameInternal(frameNo - 1)

  gotoFrameInternal:(frameNo) ->
    @jumped = true
    @stop()
    @currentFrameInternal = frameNo
    return @

  setVisible:(visible) ->
    @visible = visible
    return @

  globalToLocal:(point) ->
    if @property.hasMatrix
      m = new Matrix()
      m = Utility.calcMatrix(m, @matrix, @property.matrix)
    else
      m = @matrix
    invert = new Matrix()
    Utility.invertMatrix(invert, m)
    [x, y] = Utility.calcMatrixToPoint(point.x, point.y, invert)
    return new Point(x, y)

  localToGlobal:(point) ->
    if @property.hasMatrix
      m = new Matrix()
      m = Utility.calcMatrix(m, @matrix, @property.matrix)
    else
      m = @matrix
    [x, y] = Utility.calcMatrixToPoint(point.x, point.y, m)
    return new Point(x, y)

  getDepth:(keys) ->
    depth = if keys.length is 0 then 0 else keys[keys.length - 1] + 1
    return depth

  reorderList:(reorder, keys, list, index, object, op) ->
    Utility.insertIntArray(keys, index)
    list[index] = object
    if reorder
      i = 0
      newlist = {}
      for k, v of list
        op(v, i)
        keys[i] = i
        newlist[i] = v
        ++i
      list = newlist
    return list

  deleteAttachedMovie:( \
      parent, movie, destroy = true, deleteFromDetachedMovies = true) ->
    attachName = movie.attachName
    depth = movie.depth
    delete parent.attachedMovies[attachName]
    delete parent.attachedMovieList[depth]
    Utility.deleteIntArray(parent.attachedMovieListKeys, depth)
    delete parent.detachedMovies[attachName] if deleteFromDetachedMovies
    delete parent[attachName]
    movie.destroy() if destroy

  attachMovie:(linkageName, attachName, options = null) ->
    if linkageName instanceof LWF
      return @attachLWF(linkageName, attachName, options)

    options ?= {}
    depth = options["depth"]
    reorder = options["reorder"] ? false

    if linkageName instanceof Movie and linkageName.lwf is @lwf
      movie = linkageName
      @deleteAttachedMovie(movie.parent, movie, false)
    else if typeof(linkageName) is "string"
      movieId = @lwf.searchMovieLinkage(@lwf.getStringId(linkageName))
      return null if movieId is -1
    else
      return null

    unless @attachedMovies?
      @attachedMovies = {}
      @detachedMovies = {}
      @attachedMovieList = {}
      @attachedMovieListKeys = Utility.newIntArray()

    attachedMovie = @attachedMovies[attachName]
    @deleteAttachedMovie(@, attachedMovie) if attachedMovie?

    if !reorder and depth?
      attachedMovie = @attachedMovieList[depth]
      @deleteAttachedMovie(@, attachedMovie) if attachedMovie?

    handlers = new MovieEventHandlers()
    handlers.add(options)
    if movie?
      movie.parent = @
      movie.setHandlers(handlers)
    else
      movie = new Movie(@lwf, @, movieId, -1, 0, 0, true, handlers, attachName)
      movie.exec() if @attachMovieExeced
      movie.postExec(true) if @attachMoviePostExeced
    movie.attachName = attachName
    depth = @getDepth(@attachedMovieListKeys) unless depth?
    movie.depth = depth
    @attachedMovies[attachName] = movie
    @attachedMovieList = @reorderList(reorder, @attachedMovieListKeys,
      @attachedMovieList, movie.depth, movie, (o, i) -> o.depth = i)
    @[attachName] = movie
    return movie

  attachEmptyMovie:(attachName, options = null) ->
    return @attachMovie("_empty", attachName, options)

  swapAttachedMovieDepth:(depth0, depth1) ->
    return if !@attachedMovies? or depth0 is depth1
    attachedMovie0 = @attachedMovieList[depth0]
    attachedMovie1 = @attachedMovieList[depth1]
    if attachedMovie0?
      attachedMovie0.depth = depth1
      @attachedMovieList[depth1] = attachedMovie0
      Utility.insertIntArray(@attachedMovieListKeys, depth1)
    else
      delete @attachedMovieList[depth1]
      Utility.deleteIntArray(@attachedMovieListKeys, depth1)
    if attachedMovie1?
      attachedMovie1.depth = depth0
      @attachedMovieList[depth0] = attachedMovie1
      Utility.insertIntArray(@attachedMovieListKeys, depth0)
    else
      delete @attachedMovieList[depth0]
      Utility.deleteIntArray(@attachedMovieListKeys, depth0)
    return

  getAttachedMovie:(attachName) ->
    return null unless @attachedMovies?
    switch typeof(attachName)
      when "string"
        return @attachedMovies[attachName]
      when "number"
        depth = attachName
        return @attachedMovieList[depth]

  searchAttachedMovie:(attachName, recursive = true) ->
    movie = @getAttachedMovie(attachName)
    return movie if movie?

    return null unless recursive

    instance = @instanceHead
    while instance isnt null
      if instance.isMovie
        i = instance.searchAttachedMovie(attachName, recursive)
        return i if i isnt null
      instance = instance.linkInstance
    return null

  detachMovie:(arg) ->
    if @detachedMovies?
      if arg instanceof Movie
        @detachedMovies[arg.attachName] = true if arg?.attachName?
      else
        switch typeof(arg)
          when "string"
            @detachedMovies[arg] = true
          when "number"
            attachedMovie = @attachedMovieList?[arg]
            if attachedMovie?.attachName?
              @detachedMovies[attachedMovie.attachName] = true
    return

  detachFromParent: ->
    return if @type isnt Type.ATTACHEDMOVIE

    @active = false
    @parent.detachMovie(@) if @parent?
    return

  removeMovieClip: ->
    if @attachName?
      @parent.detachMovie(@) if @parent?
    else if @lwf.attachName?
      @lwf.parent.detachLWF(@lwf) if @lwf.parent?
    return

  attachBitmap:(linkageName, depth) ->
    bitmapId = @lwf.data.bitmapMap[linkageName]
    return null unless bitmapId?
    bitmap = new BitmapClip(@lwf, @, bitmapId)
    if @bitmapClips?
      @detachBitmap(depth)
    else
      @bitmapClips = []
    @bitmapClips[depth] = bitmap
    bitmap.depth = depth
    bitmap.name = linkageName
    return bitmap

  getAttachedBitmaps: ->
    return @bitmapClips

  getAttachedBitmap:(depth) ->
    return null unless @bitmapClips?
    return @bitmapClips[depth]

  detachBitmap:(depth) ->
    return unless @bitmapClips?
    bitmapClip = @bitmapClips[depth]
    return unless bitmapClip?
    bitmapClip.destroy()
    @bitmapClips[depth] = null
    return

  execDetachHandler:(lwfContainer) ->
    lwf = lwfContainer.child
    if lwf.detachHandler?
      if lwf.detachHandler(lwf)
        lwf.destroy()
      else
        lwf.setAttachVisible(false)
        lwf.render()
    else
      lwf.destroy()
    lwf.parent = null
    lwf.detachHandler = null
    lwf.attachName = null
    return

  deleteAttachedLWF:( \
      parent, lwfContainer, destroy = true, deleteFromDetachedLWFs = true) ->
    attachName = lwfContainer.child.attachName
    depth = lwfContainer.child.depth
    delete parent.attachedLWFs[attachName]
    delete parent.attachedLWFList[depth]
    Utility.deleteIntArray(parent.attachedLWFListKeys, depth)
    delete parent.detachedLWFs[attachName] if deleteFromDetachedLWFs
    delete parent[attachName]
    @execDetachHandler(lwfContainer) if destroy

  attachLWF:(attachLWF, attachName, options = null) ->
    options ?= {}
    depth = options["depth"]
    reorder = options["reorder"] ? false
    detachHandler = options["detach"]

    unless @attachedLWFs?
      @attachedLWFs = {}
      @detachedLWFs = {}
      @attachedLWFList = {}
      @attachedLWFListKeys = Utility.newIntArray()

    if attachLWF.parent?
      lwfContainer = attachLWF.parent.attachedLWFs[attachLWF.attachName]
      @deleteAttachedLWF(attachLWF.parent, lwfContainer, false)

    lwfContainer = @attachedLWFs[attachName]
    @deleteAttachedLWF(@, lwfContainer) if lwfContainer?

    if !reorder and depth?
      lwfContainer = @attachedLWFList[depth]
      @deleteAttachedLWF(@, lwfContainer) if lwfContainer?

    lwfContainer = new LWFContainer(@, attachLWF)

    @lwf.setInteractive() if attachLWF.interactive
    attachLWF.setParent(@)
    attachLWF.rootMovie.parent = @
    attachLWF.detachHandler = detachHandler
    attachLWF.attachName = attachName
    depth = @getDepth(@attachedLWFListKeys) unless depth?
    attachLWF.depth = depth
    @attachedLWFs[attachName] = lwfContainer
    @attachedLWFList = @reorderList(reorder, @attachedLWFListKeys,
      @attachedLWFList, attachLWF.depth, lwfContainer,
      (o, i) -> o.child.depth = i)
    @[attachName] = attachLWF.rootMovie
    delete @lwf.loadedLWFs[attachLWF.lwfInstanceId] if attachLWF.lwfInstanceId?

    @lwf.isLWFAttached = true
    return

  swapAttachedLWFDepth:(depth0, depth1) ->
    return if !@attachedLWFs? or depth0 is depth1
    attachedLWF0 = @attachedLWFList[depth0]
    attachedLWF1 = @attachedLWFList[depth1]
    if attachedLWF0?
      attachedLWF0.child.depth = depth1
      @attachedLWFList[depth1] = attachedLWF0
      Utility.insertIntArray(@attachedLWFListKeys, depth1)
    else
      delete @attachedLWFList[depth1]
      Utility.deleteIntArray(@attachedLWFListKeys, depth1)
    if attachedLWF1?
      attachedLWF1.child.depth = depth0
      @attachedLWFList[depth0] = attachedLWF1
      Utility.insertIntArray(@attachedLWFListKeys, depth0)
    else
      delete @attachedLWFList[depth0]
      Utility.deleteIntArray(@attachedLWFListKeys, depth0)
    return

  swapDepths:(depth) ->
    if depth instanceof Movie
      movie = depth
      if @attachName?
        @parent.swapAttachedMovieDepth(@depth, movie.depth)
      else if @lwf is movie.lwf and @lwf.attachName?
        @parent.swapAttachedLWFDepth(@lwf.depth, movie.lwf.depth)
    else
      if @attachName?
        @parent.swapAttachedMovieDepth(@depth, depth)
      else if @lwf.attachName?
        @parent.swapAttachedLWFDepth(@lwf.depth, depth)
    return

  getAttachedLWF:(attachName) ->
    return null unless @attachedLWFs?
    switch typeof(attachName)
      when "string"
        return @attachedLWFs[attachName]?.child
      when "number"
        depth = attachName
        return @attachedLWFList[depth]?.child

  searchAttachedLWF:(attachName, recursive = true) ->
    attachedLWF = @getAttachedLWF(attachName)
    return attachedLWF if attachedLWF?

    return null unless recursive

    instance = @instanceHead
    while instance isnt null
      if instance.isMovie
        i = instance.searchAttachedLWF(attachName, recursive)
        return i if i isnt null
      instance = instance.linkInstance
    return null

  detachLWF:(arg) ->
    if @detachedLWFs?
      if arg instanceof LWF
        @detachedLWFs[arg.attachName] = true if arg?.attachName?
      else
        switch typeof(arg)
          when "string"
            @detachedLWFs[arg] = true
          when "number"
            attachedLWF = @attachedLWFList?[arg]
            if attachedLWF?.child?.attachName?
              @detachedLWFs[attachedLWF.child.attachName] = true
    return

  detachAllLWFs: ->
    if @detachedLWFs?
      for k, lwfContainer of @attachedLWFs
        @detachedLWFs[lwfContainer.child.attachName] = true
    return

  execObject:(depth, objId, matrixId, colorTransformId, instId, blendMode) ->
    return if objId is -1

    data = @lwf.data
    dataObject = data.objects[objId]
    dataObjectId = dataObject.objectId
    obj = @displayList[depth]

    if obj? and (obj.type isnt dataObject.objectType or
        obj.objectId isnt dataObjectId or
        (obj.isMovie and obj.instanceId isnt instId))
      obj.destroy()
      obj = null

    unless obj?
      switch dataObject.objectType
        when Type.BUTTON
          obj = new Button(@lwf,
            @, dataObjectId, instId, matrixId, colorTransformId)
        when Type.GRAPHIC
          obj = new Graphic(@lwf, @, dataObjectId)
        when Type.MOVIE
          obj = new Movie(@lwf,
            @, dataObjectId, instId, matrixId, colorTransformId)
          switch blendMode
            when Format.Constant.BLEND_MODE_ADD
              obj.blendMode = "add"
            when Format.Constant.BLEND_MODE_ERASE
              obj.blendMode = "erase"
            when Format.Constant.BLEND_MODE_LAYER
              obj.blendMode = "layer"
            when Format.Constant.BLEND_MODE_MASK
              obj.blendMode = "mask"
            when Format.Constant.BLEND_MODE_MULTIPLY
              obj.blendMode = "multiply"
            when Format.Constant.BLEND_MODE_SCREEN
              obj.blendMode = "screen"
        when Type.BITMAP
          obj = new Bitmap(@lwf, @, dataObjectId)
        when Type.BITMAPEX
          obj = new BitmapEx(@lwf, @, dataObjectId)
        when Type.TEXT
          obj = new Text(@lwf, @, dataObjectId, instId)
        when Type.PARTICLE
          obj = new Particle(@lwf, @, dataObjectId)
        when Type.PROGRAMOBJECT
          obj = new ProgramObject(@lwf, @, dataObjectId)

    if obj.isMovie or obj.isButton
      obj.linkInstance = null
      if @instanceHead is null
        @instanceHead = obj
      else
        @instanceTail.linkInstance = obj
      @instanceTail = obj
      if obj.isButton
        @hasButton = true

    @displayList[depth] = obj
    obj.execCount = @movieExecCount
    obj.exec(matrixId, colorTransformId)
    return

  override:(m, c) ->
    @overriding = true
    Utility.copyMatrix(@matrix, m)
    Utility.copyColorTransform(@colorTransform, c)
    @lwf.isPropertyDirty = true
    return

  exec:(matrixId = 0, colorTransformId = 0) ->
    @attachMovieExeced = false
    @attachMoviePostExeced = false
    super(matrixId, colorTransformId)
    return

  postExec:(progressing) ->
    @hasButton = false
    return unless @active

    @execedFrame = -1
    postExeced = @postExecCount is @lwf.execCount
    if progressing and @playing and !@jumped and !postExeced
      ++@currentFrameInternal
    loop
      @currentFrameInternal = 0 if \
        @currentFrameInternal < 0 or @currentFrameInternal >= @totalFrames
      break if @currentFrameInternal is @execedFrame

      @currentFrameCurrent = @currentFrameInternal
      @execedFrame = @currentFrameCurrent
      data = @lwf.data
      frame = data.frames[@data.frameOffset + @currentFrameCurrent]

      if @lastControlOffset is frame.controlOffset and
          @lastControls is frame.controls

        controlAnimationOffset = @lastControlAnimationOffset

        if @skipped
          instance = @instanceHead
          while instance isnt null
            if instance.isMovie
              instance.attachMovieExeced = false
              instance.attachMoviePostExeced = false
            else if instance.isButton
              instance.enterFrame()
            instance = instance.linkInstance
          @hasButton = @lastHasButton
        else
          for depth in [0...@data.depths]
            obj = @displayList[depth]
            if obj?
              unless postExeced
                obj.matrixIdChanged = false
                obj.colorTransformIdChanged = false
              if obj.isMovie
                obj.attachMovieExeced = false
                obj.attachMoviePostExeced = false
              else if obj.isButton
                obj.enterFrame()
                @hasButton = true
          @lastHasButton = @hasButton
          @skipped = true

      else
        ++@movieExecCount
        @instanceHead = null
        @instanceTail = null
        @lastControlOffset = frame.controlOffset
        @lastControls = frame.controls
        controlAnimationOffset = -1
        for i in [0...frame.controls]
          control = data.controls[frame.controlOffset + i]
  
          switch control.controlType
            when ControlType.MOVE
              p = data.places[control.controlId]
              @execObject(p.depth, p.objectId, p.matrixId, 0,
                p.instanceId, p.blendMode)
  
            when ControlType.MOVEM
              ctrl = data.controlMoveMs[control.controlId]
              p = data.places[ctrl.placeId]
              @execObject(p.depth, p.objectId, ctrl.matrixId,
                0, p.instanceId, p.blendMode)
  
            when ControlType.MOVEC
              ctrl = data.controlMoveCs[control.controlId]
              p = data.places[ctrl.placeId]
              @execObject(p.depth, p.objectId, p.matrixId,
                ctrl.colorTransformId, p.instanceId, p.blendMode)
  
            when ControlType.MOVEMC
              ctrl = data.controlMoveMCs[control.controlId]
              p = data.places[ctrl.placeId]
              @execObject(p.depth, p.objectId,
                ctrl.matrixId, ctrl.colorTransformId, p.instanceId, p.blendMode)
  
            when ControlType.ANIMATION
              controlAnimationOffset = i if controlAnimationOffset is -1

        @lastControlAnimationOffset = controlAnimationOffset
        @lastHasButton = @hasButton
  
        for depth in [0...@data.depths]
          obj = @displayList[depth]
          if obj? and obj.execCount isnt @movieExecCount
            obj.destroy()
            @displayList[depth] = null

      @attachMovieExeced = true
      if @attachedMovies?
        for k in @attachedMovieListKeys
          movie = @attachedMovieList[k]
          movie.exec()

      instance = @instanceHead
      while instance isnt null
        if instance.isMovie
          movie = instance
          movie.postExec(progressing)
          if !@hasButton and movie.hasButton
            @hasButton = true
        instance = instance.linkInstance

      @attachMoviePostExeced = true
      if @attachedMovies?
        for attachName, v of @detachedMovies
          movie = @attachedMovies[attachName]
          @deleteAttachedMovie(@, movie, true, false) if movie?
        @detachedMovies = {}
        for k in @attachedMovieListKeys
          movie = @attachedMovieList[k]
          movie.postExec(progressing)
          @hasButton = true if !@hasButton and movie.hasButton

      @hasButton = true if @attachedLWFs?

      unless @postLoaded
        @postLoaded = true
        @postLoadFunc.call(@) if @postLoadFunc?
        @handler.call("postLoad", @) unless @handler.empty

      if @nextEnterFrameFunctions?
        funcs = @nextEnterFrameFunctions
        @nextEnterFrameFunctions = null
        func.call(@) for func in funcs

      if controlAnimationOffset isnt -1 and
          @execedFrame is @currentFrameInternal
        animationPlayed =
          @animationPlayedFrame is @currentFrameCurrent and !@jumped
        unless animationPlayed
          for i in [controlAnimationOffset...frame.controls]
            control = data.controls[frame.controlOffset + i]
            @lwf.playAnimation(control.controlId, @)

      @animationPlayedFrame = @currentFrameCurrent
      @jumped = false if @currentFrameCurrent is @currentFrameInternal

    if @postExecCount isnt @lwf.execCount
      @enterFrameFunc.call(@) if @enterFrameFunc?
      @playAnimation(ClipEvent.ENTERFRAME)
      @handler.call("enterFrame", @) unless @handler.empty
    @postExecCount = @lwf.execCount
    return

  updateObject:(obj, m, c, matrixChanged, colorTransformChanged) ->
    if obj.isMovie and obj.property.hasMatrix
      objm = m
    else if matrixChanged or !obj.updated or obj.matrixIdChanged
      objm = Utility.calcMatrixId(@lwf, @matrix1, m, obj.matrixId)
    else
      objm = null

    if obj.isMovie and obj.property.hasColorTransform
      objc = c
    else if colorTransformChanged or !obj.updated or obj.colorTransformIdChanged
      objc = Utility.calcColorTransformId(
        @lwf, @colorTransform1, c, obj.colorTransformId)
    else
      objc = null

    obj.update(objm, objc)

  update:(m, c) ->
    return unless @active

    if @overriding
      matrixChanged = true
      colorTransformChanged = true
    else
      matrixChanged = @matrix.setWithComparing(m)
      colorTransformChanged = @colorTransform.setWithComparing(c)

    if @property.hasMatrix
      matrixChanged = true
      m = Utility.calcMatrix(@matrix0, @matrix, @property.matrix)
    else
      m = @matrix

    if @property.hasColorTransform
      colorTransformChanged = true
      c = Utility.calcColorTransform(
        @colorTransform0, @colorTransform, @property.colorTransform)
    else
      c = @colorTransform

    for depth in [0...@data.depths]
      obj = @displayList[depth]
      @updateObject(obj, m, c, matrixChanged, colorTransformChanged) if obj?

    if @bitmapClips?
      for bitmapClip in @bitmapClips
        bitmapClip.update(m, c) if bitmapClip?

    if @attachedMovies? or @attachedLWFs?
      if @attachedMovies?
        for k in @attachedMovieListKeys
          movie = @attachedMovieList[k]
          @updateObject(movie, m, c, matrixChanged, colorTransformChanged)

      if @attachedLWFs?
        for attachName, v of @detachedLWFs
          lwfContainer = @attachedLWFs[attachName]
          @deleteAttachedLWF(@, lwfContainer, true, false) if lwfContainer?

        @detachedLWFs = {}
        for k in @attachedLWFListKeys
          lwfContainer = @attachedLWFList[k]
          @lwf.renderObject(lwfContainer.child.exec(@lwf.thisTick, m, c))

    if @requestedCalculateBounds
      @xMin = Number.MAX_VALUE
      @xMax = -Number.MAX_VALUE
      @yMin = Number.MAX_VALUE
      @yMax = -Number.MAX_VALUE
      inspector = (o, h, d, r) => @calculateBounds(o)
      @inspect(inspector, 0, 0)
      if @lwf.property.hasMatrix
        invert = new Matrix()
        Utility.invertMatrix(invert, @lwf.property.matrix)
        p = Utility.calcMatrixToPoint(@xMin, @yMin, invert)
        @xMin = p[0]
        @yMin = p[1]
        p = Utility.calcMatrixToPoint(@xMax, @yMax, invert)
        @xMax = p[0]
        @yMax = p[1]
      @bounds =
        "xMin":@xMin
        "xMax":@xMax
        "yMin":@yMin
        "yMax":@yMax
      @requestedCalculateBounds = false
      if @calculateBoundsCallback?
        @calculateBoundsCallback.call(@)
        @calculateBoundsCallback = null

    @handler.call("update", @) unless @handler.empty

    return

  calculateBounds:(o) ->
    tfId = null
    switch o.type
      when Type.GRAPHIC
        @calculateBounds(obj) for obj in o.displayList
      when Type.BITMAP, Type.BITMAPEX
        if o.type is Type.BITMAP
          tfId = o.lwf.data.bitmaps[o.objectId].textureFragmentId
        else
          tfId = o.lwf.data.bitmapExs[o.objectId].textureFragmentId
        if tfId? and tfId >= 0
          tf = o.lwf.data.textureFragments[tfId]
          @updateBounds(o.matrix, tf.x, tf.x + tf.w, tf.y, tf.y + tf.h)
      when Type.BUTTON
        @updateBounds(o.matrix, 0, o.width, 0, o.height)
      when Type.TEXT
        text = o.lwf.data.texts[o.objectId]
        @updateBounds(o.matrix, 0, text.width, 0, text.height)
      when Type.PROGRAMOBJECT
        pobj = o.lwf.data.programObjects[o.objectId]
        @updateBounds(o.matrix, 0, pobj.width, 0, pobj.height)
    return

  updateBounds:(matrix, xmin, xmax, ymin, ymax) ->
    for p in [[xmin, ymin], [xmin, ymax], [xmax, ymin], [xmax, ymax]]
      [x, y] = Utility.calcMatrixToPoint(p[0], p[1], matrix)
      if x < @xMin
        @xMin = x
      else if x > @xMax
        @xMax = x
      if y < @yMin
        @yMin = y
      else if y > @yMax
        @yMax = y
    return

  linkButton:() ->
    return if !@visible or !@active or !@hasButton

    for depth in [0...@data.depths]
      obj = @displayList[depth]
      if obj?
        if obj.isButton
          obj.linkButton()
        else if obj.isMovie
          obj.linkButton() if obj.hasButton

    if @attachedMovies?
      for k in @attachedMovieListKeys
        movie = @attachedMovieList[k]
        movie.linkButton() if movie.hasButton

    if @attachedLWFs?
      for k in @attachedLWFListKeys
        lwfContainer = @attachedLWFList[k]
        lwfContainer.linkButton()
    return

  render:(v, rOffset) ->
    return if !@active and !@lwf.rendererFactory.needsRenderForInactive?
    v = false if !@visible or !@active

    useBlendMode = false
    useMaskMode = false
    if @blendMode isnt "normal"
      switch @blendMode
        when "add"
          @lwf.beginBlendMode(@blendMode)
          useBlendMode = true
        when "erase", "layer", "mask"
          @lwf.beginMaskMode(@blendMode)
          useMaskMode = true

    @handler.call("render", @) unless @handler.empty

    if @property.hasRenderingOffset
      @lwf.renderOffset()
      rOffset = @property.renderingOffset
    if rOffset is Number.MIN_VALUE
      @lwf.clearRenderOffset()

    for depth in [0...@data.depths]
      obj = @displayList[depth]
      obj.render(v, rOffset) if obj?

    if @bitmapClips?
      for bitmapClip in @bitmapClips
        bitmapClip.render(v and bitmapClip.visible, rOffset) if bitmapClip?

    if @attachedMovies?
      for k in @attachedMovieListKeys
        attachedMovie = @attachedMovieList[k]
        attachedMovie.render(v, rOffset)

    if @attachedLWFs?
      for k in @attachedLWFListKeys
        lwfContainer = @attachedLWFList[k]
        child = lwfContainer.child
        child.setAttachVisible(v)
        @lwf.renderObject(child.render(
          @lwf.renderingIndex, @lwf.renderingCount, rOffset))

    @lwf.endBlendMode() if useBlendMode
    @lwf.endMaskMode() if useMaskMode
    return

  inspect:(inspector, hierarchy, depth) ->
    if @property.hasRenderingOffset
      @lwf.renderOffset()
      rOffset = @property.renderingOffset
    if rOffset is Number.MIN_VALUE
      @lwf.clearRenderOffset()

    inspector(@, hierarchy, depth, rOffset)

    ++hierarchy

    for d in [0...@data.depths]
      obj = @displayList[d]
      obj.inspect(inspector, hierarchy, d, rOffset) if obj?

    if @bitmapClips?
      for bitmapClip in @bitmapClips
        bitmapClip.inspect(inspector, hierarchy, d++, rOffset) if bitmapClip?

    if @attachedMovies?
      for k in @attachedMovieListKeys
        attachedMovie = @attachedMovieList[k]
        attachedMovie.inspect(inspector, hierarchy, d++, rOffset)

    if @attachedLWFs?
      for k in @attachedLWFListKeys
        lwfContainer = @attachedLWFList[k]
        child = lwfContainer.child
        @lwf.renderObject(child.inspect(inspector, hierarchy, d++, rOffset))
    return

  destroy: ->
    for obj in @displayList
      obj.destroy() if obj?

    if @bitmapClips?
      for bitmapClip in @bitmapClips
        bitmapClip.destroy() if bitmapClip?
      @bitmapClips = null

    if @attachedMovies?
      movie.destroy() for k, movie of @attachedMovies
      @attachedMovies = null
      @detachedMovies = null
      @attachedMovieList = null
      @attachedMovieListKeys = null

    if @attachedLWFs?
      @execDetachHandler(lwfContainer) for k, lwfContainer of @attachedLWFs
      @attachedLWFs = null
      @detachedLWFs = null
      @attachedLWFList = null
      @attachedLWFListKeys = null

    @unloadFunc.call(@) if @unloadFunc?
    @playAnimation(ClipEvent.UNLOAD)

    @handler.call("unload", @) unless @handler.empty

    @instanceHead = null
    @instanceTail = null
    @displayList = null
    @property = null

    super()
    return

  playAnimation:(clipEvent) ->
    clipEvents = @lwf.data.movieClipEvents
    for i in [0...@data.clipEvents]
      c = clipEvents[@data.clipEventId + i]
      if (c.clipEvent & clipEvent) isnt 0
        @lwf.playAnimation(c.animationId, @)
    return

  dispatchEvent:(e) ->
    if typeof e is "object"
      param = e["param"]
      e = e["type"]
    else
      param = null
    switch e
      when "load", "postLoad", "unload", "enterFrame", "update", "render"
        @handler.call(e, @) unless @handler.empty
      else
        handlers = @eventHandlers[e]
        return false unless handlers?
        handlers = (handler for handler in handlers)
        for handler in handlers
          handler.call(@, {"type":e, "param":param}) if handler?
    return true

  addEventHandler:(e, eventHandler) ->
    switch e
      when "load", "postLoad", "unload", "enterFrame", "update", "render"
        @lwf.enableExec()
        @handler.addHandler(e, eventHandler)
      else
        @eventHandlers[e] ?= []
        @eventHandlers[e].push(eventHandler)
    return

  removeEventHandler:(e, eventHandler) ->
    switch e
      when "load", "postLoad", "unload", "enterFrame", "update", "render"
        @handler.removeHandler(e, eventHandler)
      else
        handlers = @eventHandlers[e]
        return unless handlers?
        i = 0
        while i < handlers.length
          if handlers[i] is eventHandler
            handlers.splice(i, 1)
          else
            ++i
        delete @eventHandlers[e] if handlers.length is 0
    return

  clearEventHandler:(e = null) ->
    switch e
      when null
        @handler.clear()
        @eventHandlers = {}
      when "load", "postLoad", "unload", "enterFrame", "update", "render"
        @handler.clear(e)
      else
        delete @eventHandlers[e]
    return

  setEventHandler:(e, eventHandler) ->
    @clearEventHandler(e)
    @addEventHandler(e, eventHandler)
    return

  nextEnterFrame:(func) ->
    @nextEnterFrameFunctions ?= []
    @nextEnterFrameFunctions.push(func)
    return

  searchFrame:(label) ->
    return @lwf.searchFrame(@, label)

  gotoLabel:(label) ->
    label = @lwf.getStringId(label) if typeof label is "string"
    @gotoFrame(@lwf.searchFrame(@, label))
    return @

  gotoAndStop:(n) ->
    if typeof n is "string"
      @gotoFrame(@lwf.searchFrame(@, @lwf.getStringId(n)))
    else
      @gotoFrame(n)
    @stop()
    return @

  gotoAndPlay:(n) ->
    if typeof n is "string"
      @gotoFrame(@lwf.searchFrame(@, @lwf.getStringId(n)))
    else
      @gotoFrame(n)
    @play()
    return @

  searchMovieInstance:(stringId, recursive = true) ->
    stringId = @lwf.getStringId(stringId) if typeof stringId is "string"
    instance = @instanceHead
    while instance isnt null
      if instance.isMovie and
          @lwf.getInstanceNameStringId(instance.instanceId) == stringId
        return instance
      else if recursive and instance.isMovie
        i = instance.searchMovieInstance(stringId, recursive)
        return i if i isnt null
      instance = instance.linkInstance

    return null

  searchMovieInstanceByInstanceId:(instId, recursive) ->
    instance = @instanceHead
    while instance isnt null
      if instance.isMovie and instance.instanceId is instId
        return instance
      else if recursive and instance.isMovie
        i = instance.searchMovieInstanceByInstanceId(instId, recursive)
        return i if i isnt null
      instance = instance.linkInstance
    return null

  searchButtonInstance:(stringId, recursive = true) ->
    stringId = @lwf.getStringId(stringId) if typeof stringId is "string"
    instance = @instanceHead
    while instance isnt null
      if instance.isButton and
          @lwf.getInstanceNameStringId(instance.instanceId) == stringId
        return instance
      else if recursive and instance.isMovie
        i = instance.searchButtonInstance(stringId, recursive)
        return i if i isnt null
      instance = instance.linkInstance

    return null

  searchButtonInstanceByInstanceId:(instId, recursive) ->
    instance = @instanceHead
    while instance isnt null
      if instance.isButton and instance.instanceId is instId
        return instance
      else if recursive and instance.isMovie
        i = instance.searchMovieInstanceByInstanceId(instId, recursive)
        return i if i isnt null
      instance = instance.linkInstance
    return null

  move:(x, y) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.move(x, y)
    return @

  moveTo:(x, y) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.moveTo(x, y)
    return @

  rotate:(degree) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.rotate(degree)
    return @

  rotateTo:(degree) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.rotateTo(degree)
    return @

  scale:(x, y) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.scale(x, y)
    return @

  scaleTo:(x, y) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.scaleTo(x, y)
    return @

  setMatrix:(m, scaleX = 1, scaleY = 1, rotation = 0) ->
    @property.setMatrix(m, scaleX, scaleY, rotation)
    return @

  setAlpha:(alpha) ->
    Utility.syncColorTransform(@) unless @property.hasColorTransform
    @property.setAlpha(alpha)
    return @

  setColorTransform:(c) ->
    @property.setColorTransform(c)
    return @

  setRenderingOffset:(rOffset) ->
    @property.setRenderingOffset(rOffset)
    return @

  getX: ->
    if @property.hasMatrix
      return @property.matrix.translateX
    else
      return Utility.getX(@)

  setX:(v) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.moveTo(v, @property.matrix.translateY)
    return

  getY: ->
    if @property.hasMatrix
      return @property.matrix.translateY
    else
      return Utility.getY(@)

  setY:(v) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.moveTo(@property.matrix.translateX, v)
    return

  getScaleX: ->
    if @property.hasMatrix
      return @property.scaleX
    else
      return Utility.getScaleX(@)

  setScaleX:(v) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.scaleTo(v, @property.scaleY)
    return

  getScaleY: ->
    if @property.hasMatrix
      return @property.scaleY
    else
      return Utility.getScaleY(@)

  setScaleY:(v) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.scaleTo(@property.scaleX, v)
    return

  getRotation: ->
    if @property.hasMatrix
      return @property.rotation
    else
      return Utility.getRotation(@)

  setRotation:(v) ->
    Utility.syncMatrix(@) unless @property.hasMatrix
    @property.rotateTo(v)
    return

  getAlphaProperty: ->
    if @property.hasColorTransform
      return @property.colorTransform.multi.alpha
    else
      return Utility.getAlpha(@)

  setAlphaProperty:(v) ->
    Utility.syncColorTransform(@) unless @property.hasColorTransform
    @property.setAlpha(v)
    return

  requestCalculateBounds:(callback = null) ->
    @requestedCalculateBounds = true
    @calculateBoundsCallback = callback
    @bounds = undefined
    return

  getBounds: ->
    return @bounds

  setFrameRate:(frameRate) ->
    if @attachedMovies?
      for k in @attachedMovieListKeys
        movie = @attachedMovieList[k]
        movie.setFrameRate(frameRate)

    if @attachedLWFs?
      for k in @attachedLWFListKeys
        lwfContainer = @attachedLWFList[k]
        lwfContainer.child.setFrameRate(frameRate)

    instance = @instanceHead
    while instance isnt null
      if instance.isMovie
        instance.setFrameRate(frameRate)
      instance = instance.linkInstance
    return

if typeof(Movie.prototype.__defineGetter__) isnt "undefined"
  Movie.prototype.__defineGetter__("x", -> @getX())
  Movie.prototype.__defineSetter__("x", (v) -> @setX(v))
  Movie.prototype.__defineGetter__("y", -> @getY())
  Movie.prototype.__defineSetter__("y", (v) -> @setY(v))
  Movie.prototype.__defineGetter__("scaleX", -> @getScaleX())
  Movie.prototype.__defineSetter__("scaleX", (v) -> @setScaleX(v))
  Movie.prototype.__defineGetter__("scaleY", -> @getScaleY())
  Movie.prototype.__defineSetter__("scaleY", (v) -> @setScaleY(v))
  Movie.prototype.__defineGetter__("rotation", -> @getRotation())
  Movie.prototype.__defineSetter__("rotation", (v) -> @setRotation(v))
  Movie.prototype.__defineGetter__("alpha", -> @getAlphaProperty())
  Movie.prototype.__defineSetter__("alpha", (v) -> @setAlphaProperty(v))
  Movie.prototype.__defineGetter__("currentFrame", -> @currentFrameInternal + 1)
else if typeof(Object.defineProperty) isnt "undefined"
  Object.defineProperty(Movie.prototype, "x",
    get: -> @getX()
    set: (v) -> @setX(v))
  Object.defineProperty(Movie.prototype, "y",
    get: -> @getY()
    set: (v) -> @setY(v))
  Object.defineProperty(Movie.prototype, "scaleX",
    get: -> @getScaleX()
    set: (v) -> @setScaleX(v))
  Object.defineProperty(Movie.prototype, "scaleY",
    get: -> @getScaleY()
    set: (v) -> @setScaleY(v))
  Object.defineProperty(Movie.prototype, "rotation",
    get: -> @getRotation()
    set: (v) -> @setRotation(v))
  Object.defineProperty(Movie.prototype, "alpha",
    get: -> @getAlphaProperty()
    set: (v) -> @setAlphaProperty(v))
  Object.defineProperty(Movie.prototype, "currentFrame",
    get: -> @currentFrameInternal + 1)

