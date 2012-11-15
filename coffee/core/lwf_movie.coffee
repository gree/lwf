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

Type = Format.LObject.Type
ClipEvent = Format.MovieClipEvent.ClipEvent

class Movie extends IObject
  constructor:(lwf, parent, objId, instId, \
      @matrixId = 0, @colorTransformId = 0, attached = false, handler = null) ->
    type = if attached then Type.ATTACHEDMOVIE else Type.MOVIE
    super(lwf, parent, type, objId, instId)

    @data = lwf.data.movies[objId]
    @totalFrames = @data.frames
    @instanceHead = null
    @instanceTail = null
    @currentFrameInternal = -1
    @execedFrame = -1
    @animationPlayedFrame = -1
    @postLoaded = false
    @active = true
    @visible = true
    @playing = true
    @jumped = false
    @overriding = false
    @attachMovieExeced = false
    @attachMoviePostExeced = false

    @property = new Property(lwf)

    if typeof(@.__defineGetter__) isnt "undefined"
      @.__defineGetter__("x", -> @getX())
      @.__defineSetter__("x", (v) -> @setX(v))
      @.__defineGetter__("y", -> @getY())
      @.__defineSetter__("y", (v) -> @setY(v))
      @.__defineGetter__("scaleX", -> @getScaleX())
      @.__defineSetter__("scaleX", (v) -> @setScaleX(v))
      @.__defineGetter__("scaleY", -> @getScaleY())
      @.__defineSetter__("scaleY", (v) -> @setScaleY(v))
      @.__defineGetter__("rotation", -> @getRotation())
      @.__defineSetter__("rotation", (v) -> @setRotation(v))
      @.__defineGetter__("alpha", -> @getAlphaProperty())
      @.__defineSetter__("alpha", (v) -> @setAlphaProperty(v))
      @.__defineGetter__("currentFrame", -> @currentFrameInternal + 1)
    else if typeof(Object.defineProperty) isnt "undefined"
      Object.defineProperty(@, "x",
        get: -> @getX()
        set: (v) -> @setX(v))
      Object.defineProperty(@, "y",
        get: -> @getY()
        set: (v) -> @setY(v))
      Object.defineProperty(@, "scaleX",
        get: -> @getScaleX()
        set: (v) -> @setScaleX(v))
      Object.defineProperty(@, "scaleY",
        get: -> @getScaleY()
        set: (v) -> @setScaleY(v))
      Object.defineProperty(@, "rotation",
        get: -> @getRotation()
        set: (v) -> @setRotation(v))
      Object.defineProperty(@, "alpha",
        get: -> @getAlphaProperty()
        set: (v) -> @setAlphaProperty(v))
      Object.defineProperty(@, "currentFrame",
        get: -> @currentFrameInternal + 1)

    @matrix0 = new Matrix
    @matrix1 = new Matrix
    @colorTransform0 = new ColorTransform
    @colorTransform1 = new ColorTransform

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

    @handler = if handler? then handler else lwf.getMovieEventHandlers(@)
    @handler.call("load", @) if @handler?
    lwf.execMovieCommand()

  getMovieFunctions: ->
    [@loadFunc, @postLoadFunc, @unloadFunc, @enterFrameFunc] =
      @lwf.getMovieFunctions(@objectId)
    return

  setHandlers:(handler) ->
    @handler = handler
    return

  play: ->
    @playing = true
    return @

  stop: ->
    @playing = false
    return @

  gotoNextFrame: ->
    @jumped = true
    @stop()
    ++@currentFrameInternal
    return @

  gotoPrevFrame: ->
    @jumped = true
    @stop()
    --@currentFrameInternal
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
    invert = new Matrix()
    Utility.invertMatrix(invert, @matrix)
    [x, y] = Utility.calcMatrixToPoint(point.x, point.y, invert)
    return new Point(x, y)

  localToGlobal:(point) ->
    [x, y] = Utility.calcMatrixToPoint(point.x, point.y, @matrix)
    return new Point(x, y)

  shrinkList:(list) ->
    for i in [(list.length - 1)..0]
      if list[i]?
        if i is list.length - 1
          return list
        else
          return list[0..i]
    return []

  reorderList:(reorder, list, index, object, op) ->
    if !reorder or index >= list.length
      list[index] = object
    else
      list.splice(index, 0, object)
    if reorder
      i = 0
      while i < list.length
        if list[i]?
          op(list[i], i)
          i += 1
        else
          list.splice(i, 1)

  deleteAttachedMovie:( \
      parent, movie, destroy = true, deleteFromDetachedMovies = true) ->
    attachName = movie.attachName
    depth = movie.depth
    delete parent.attachedMovies[attachName]
    delete parent.attachedMovieList[depth]
    delete parent.detachedMovies[attachName] if deleteFromDetachedMovies
    delete parent[attachName]
    parent.attachedMovieList = @shrinkList(parent.attachedMovieList)
    movie.destroy() if destroy

  attachMovie:(linkageName, attachName, options = null) ->
    options ?= {}
    depth = options["depth"]
    reorder = options["reorder"] ? false
    load = options["load"]
    postLoad = options["postLoad"]
    unload = options["unload"]
    enterFrame = options["enterFrame"]
    update = options["update"]
    render = options["render"]

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
      @attachedMovieList = []

    attachedMovie = @attachedMovies[attachName]
    @deleteAttachedMovie(@, attachedMovie) if attachedMovie?

    unless reorder
      attachedMovie = @attachedMovieList[depth]
      @deleteAttachedMovie(@, attachedMovie) if attachedMovie?

    handlers = new MovieEventHandlers(
      load, postLoad, unload, enterFrame, update, render)
    if movie?
      movie.setHandlers(handler)
    else
      movie = new Movie(@lwf, @, movieId, -1, 0, 0, true, handlers)
      movie.exec() if @attachMovieExeced
      movie.postExec(true) if @attachMoviePostExeced
    movie.attachName = attachName
    movie.depth = depth ? @attachedMovieList.length
    movie.name = attachName
    @attachedMovies[attachName] = movie
    @reorderList(reorder,
      @attachedMovieList, movie.depth, movie, (o, i) -> o.depth = i)
    @[attachName] = movie
    return movie

  swapAttachedMovieDepth:(depth0, depth1) ->
    return unless @attachedMovies?
    attachedMovie0 = @attachedMovieList[depth0]
    attachedMovie1 = @attachedMovieList[depth1]
    attachedMovie0.depth = depth1 if attachedMovie0?
    attachedMovie1.depth = depth0 if attachedMovie1?
    @attachedMovieList[depth0] = attachedMovie1
    @attachedMovieList[depth1] = attachedMovie0
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
    while instance?
      if instance.isMovie()
        i = instance.searchAttachedMovie(attachName, recursive)
        return i if i?
      instance = instance.linkInstance
    return null

  detachMovie:(arg) ->
    if @detachedMovies?
      switch typeof(arg)
        when "string"
          @detachedMovies[arg] = true
        when "number"
          attachedMovie = @attachedMovieList?[arg]
          if attachedMovie?.attachName?
            @detachedMovies[attachedMovie.attachName] = true
        when typeof(Movie)
          @detachedMovies[arg.attachName] = true if arg?.attachName?
    return

  detachFromParent: ->
    return if @type isnt Type.ATTACHEDMOVIE

    @active = false
    @parent.detachMovie(@) if @parent?
    return

  execDetachHandler:(lwfContainer) ->
    lwf = lwfContainer.child
    if lwf.detachHandler?
      lwf.destroy() if lwf.detachHandler(lwf)
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
    delete parent.detachedLWFs[attachName] if deleteFromDetachedLWFs
    delete parent[attachName]
    parent.attachedLWFList = @shrinkList(parent.attachedLWFList)
    @execDetachHandler(lwfContainer) if destroy

  attachLWF:(attachLWF, attachName, options = null) ->
    options ?= {}
    depth = options["depth"]
    reorder = options["reorder"] ? false
    detachHandler = options["detach"]

    unless @attachedLWFs?
      @attachedLWFs = {}
      @detachedLWFs = {}
      @attachedLWFList = []

    if attachLWF.parent?
      lwfContainer = attachLWF.parent.attachedLWFs[attachLWF.attachName]
      @deleteAttachedLWF(attachLWF.parent, lwfContainer, false)

    lwfContainer = @attachedLWFs[attachName]
    @deleteAttachedLWF(@, lwfContainer) if lwfContainer?

    unless reorder
      lwfContainer = @attachedLWFList[depth]
      @deleteAttachedLWF(@, lwfContainer) if lwfContainer?

    lwfContainer = new LWFContainer(@, attachLWF)

    @lwf.interactive = true if attachLWF.interactive
    attachLWF.parent = @
    attachLWF.detachHandler = detachHandler
    attachLWF.attachName = attachName
    attachLWF.depth = depth ? @attachedLWFList.length
    @attachedLWFs[attachName] = lwfContainer
    @reorderList(reorder, @attachedLWFList,
      attachLWF.depth, lwfContainer, (o, i) -> o.child.depth = i)
    @[attachName] = attachLWF.rootMovie

    @lwf.isLWFAttached = true
    return

  swapAttachedLWFDepth:(depth0, depth1) ->
    return unless @attachedLWFs?
    attachedLWF0 = @attachedLWFList[depth0]
    attachedLWF1 = @attachedLWFList[depth1]
    attachedLWF0.child.depth = depth1 if attachedLWF0?
    attachedLWF1.child.depth = depth0 if attachedLWF1?
    @attachedLWFList[depth0] = attachedLWF1
    @attachedLWFList[depth1] = attachedLWF0
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
    while instance?
      if instance.isMovie()
        i = instance.searchAttachedLWF(attachName, recursive)
        return i if i?
      instance = instance.linkInstance
    return null

  detachLWF:(arg) ->
    if @detachedLWFs?
      switch typeof(arg)
        when "string"
          @detachedLWFs[arg] = true
        when "number"
          attachedLWF = @attachedLWFList?[arg]
          if attachedLWF?.child?.attachName?
            @detachedLWFs[attachedLWF.child.attachName] = true
        when typeof(LWF)
          @detachedLWFs[arg.attachName] = true if arg?.attachName?
    return

  detachAllLWFs: ->
    if @detachedLWFs?
      for k, lwfContainer of @attachedLWFs
        @detachedLWFs[lwfContainer.child.attachName] = true
    return

  execObject:(depth, objId, matrixId, colorTransformId, instId) ->
    return if objId is -1

    data = @lwf.data
    dataObject = data.objects[objId]
    dataObjectId = dataObject.objectId
    obj = @displayList[depth]

    if obj? and (obj.type isnt dataObject.objectType or
        obj.objectId != dataObjectId or (obj.isMovie() and
        obj.instanceId != instId))
      obj.destroy()
      obj = null

    unless obj?
      switch dataObject.objectType
        when Type.BUTTON
          obj = new Button(@lwf, @, dataObjectId, instId)
        when Type.GRAPHIC
          obj = new Graphic(@lwf, @, dataObjectId)
        when Type.MOVIE
          obj = new Movie(
            @lwf, @, dataObjectId, instId, matrixId, colorTransformId)
        when Type.BITMAP
          obj = new Bitmap(@lwf, @, dataObjectId)
        when Type.BITMAPEX
          obj = new BitmapEx(@lwf, @, dataObjectId)
        when Type.TEXT
          obj = new Text(@lwf, @, dataObjectId)
        when Type.PARTICLE
          obj = new Particle(@lwf, @, dataObjectId)
        when Type.PROGRAMOBJECT
          obj = new ProgramObject(@lwf, @, dataObjectId)

    if obj.type == Type.MOVIE
      obj.linkInstance = null
      unless @instanceHead?
        @instanceHead = obj
      else
        @instanceTail.linkInstance = obj
      @instanceTail = obj
    else if obj.type == Type.BUTTON
      @hasButton = true

    @displayList[depth] = obj
    obj.execCount = @lwf.execCount

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

    @instanceHead = null
    @instanceTail = null
    @execedFrame = -1
    ++@currentFrameInternal if progressing and @playing and !@jumped
    loop
      @currentFrameInternal = 0 if \
        @currentFrameInternal < 0 or @currentFrameInternal >= @totalFrames
      break if @currentFrameInternal is @execedFrame

      @instanceHead = null
      @instanceTail = null

      @currentFrameCurrent = @currentFrameInternal
      @execedFrame = @currentFrameCurrent
      data = @lwf.data
      frame = data.frames[@data.frameOffset + @currentFrameCurrent]

      controlAnimationOffset = -1
      for i in [0...frame.controls]
        control = data.controls[frame.controlOffset + i]

        switch control.controlType
          when Format.Control.Type.MOVE
            p = data.places[control.controlId]
            @execObject(p.depth, p.objectId, p.matrixId, 0, p.instanceId)

          when Format.Control.Type.MOVEM
            ctrl = data.controlMoveMs[control.controlId]
            p = data.places[ctrl.placeId]
            @execObject(p.depth, p.objectId, ctrl.matrixId, 0, p.instanceId)

          when Format.Control.Type.MOVEC
            ctrl = data.controlMoveCs[control.controlId]
            p = data.places[ctrl.placeId]
            @execObject(p.depth, p.objectId, p.matrixId,
              ctrl.colorTransformId, p.instanceId)

          when Format.Control.Type.MOVEMC
            ctrl = data.controlMoveMCs[control.controlId]
            p = data.places[ctrl.placeId]
            @execObject(p.depth, p.objectId,
              ctrl.matrixId, ctrl.colorTransformId, p.instanceId)

          when Format.Control.Type.ANIMATION
            controlAnimationOffset = i if controlAnimationOffset is -1

      for depth in [0...@data.depths]
        obj = @displayList[depth]
        if obj?
          if obj.execCount isnt @lwf.execCount
            obj.destroy()
            @displayList[depth] = null

      @attachMovieExeced = true
      if @attachedMovies?
        for movie in @attachedMovieList
          movie.exec() if movie?

      instance = @instanceHead
      while instance?
        if instance.isMovie()
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
        for movie in @attachedMovieList
          if movie?
            movie.postExec(progressing)
            if !@hasButton and movie.hasButton
              @hasButton = true

      unless @postLoaded
        @postLoaded = true
        @postLoadFunc.call(@) if @postLoadFunc?
        @handler.call("postLoad", @) if @handler?

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

    @enterFrameFunc.call(@) if @enterFrameFunc?
    @playAnimation(ClipEvent.ENTERFRAME)
    @handler.call("enterFrame", @) if @handler?
    return

  update:(m, c) ->
    return unless @active

    unless @overriding
      Utility.copyMatrix(@matrix, m)
      Utility.copyColorTransform(@colorTransform, c)

    @handler.call("update", @) if @handler?

    for depth in [0...@data.depths]
      obj = @displayList[depth]
      if obj?
        objm = @matrix0
        objHasOwnMatrix = obj.type is Type.MOVIE and obj.property.hasMatrix
        if @property.hasMatrix
          if objHasOwnMatrix
            Utility.calcMatrix(objm, @matrix, @property.matrix)
          else
            Utility.calcMatrix(@matrix1, @matrix, @property.matrix)
            Utility.calcMatrixId(@lwf, objm, @matrix1, obj.matrixId)
        else
          if objHasOwnMatrix
            Utility.copyMatrix(objm, @matrix)
          else
            Utility.calcMatrixId(@lwf, objm, @matrix, obj.matrixId)

        objc = @colorTransform0
        objHasOwnColorTransform =
          obj.type is Type.MOVIE and obj.property.hasColorTransform
        if @property.hasColorTransform
          if objHasOwnColorTransform
            Utility.calcColorTransform(objc,
              @colorTransform, @property.colorTransform)
          else
            Utility.calcColorTransform(@colorTransform1,
              @colorTransform, @property.colorTransform)
            Utility.calcColorTransformId(@lwf,
              objc, @colorTransform1, obj.colorTransformId)
        else
          if objHasOwnColorTransform
            Utility.copyColorTransform(objc, @colorTransform)
          else
            Utility.calcColorTransformId(@lwf,
              objc, @colorTransform, obj.colorTransformId)

        obj.update(objm, objc)

    if @attachedMovies? or @attachedLWFs?
      m = @matrix
      if @property.hasMatrix
        m1 = @matrix1.set(m)
        Utility.calcMatrix(m, m1, @property.matrix)

      c = @colorTransform
      if @property.hasColorTransform
        c1 = @colorTransform1.set(c)
        Utility.calcColorTransform(c, c1, @property.colorTransform)

      if @attachedMovies?
        for movie in @attachedMovieList
          movie.update(m, c) if movie?

      if @attachedLWFs?
        for attachName, v of @detachedLWFs
          lwfContainer = @attachedLWFs[attachName]
          @deleteAttachedLWF(@, lwfContainer, true, false) if lwfContainer?

        @detachedLWFs = {}
        for lwfContainer in @attachedLWFList
          if lwfContainer?
            @lwf.renderObject(lwfContainer.child.exec(@lwf.thisTick, m, c))
    return

  linkButton:() ->
    return if !@visible or !@active

    if @attachedLWFs?
      for lwfContainer in @attachedLWFList
        lwfContainer.linkButton() if lwfContainer?

    if @attachedMovies?
      for movie in @attachedMovieList
        movie.linkButton() if movie?.hasButton

    for depth in [0...@data.depths]
      obj = @displayList[depth]
      if obj?
        if obj.type is Type.BUTTON
          obj.linkButton()
        else if obj.type is Type.MOVIE
          movie = obj
          if movie.hasButton
            movie.linkButton()
    return

  render:(v, rOffset) ->
    v = false if !@visible or !@active

    @handler.call("render", @) if @handler?

    if @property.hasRenderingOffset
      @lwf.renderOffset()
      rOffset = @property.renderingOffset
    if rOffset is Number.MIN_VALUE
      @lwf.clearRenderOffset()

    for depth in [0...@data.depths]
      obj = @displayList[depth]
      obj.render(v, rOffset) if obj?

    if @attachedMovies?
      for attachedMovie in @attachedMovieList
        attachedMovie.render(v, rOffset) if attachedMovie?

    if @attachedLWFs?
      for lwfContainer in @attachedLWFList
        if lwfContainer?
          child = lwfContainer.child
          child.setAttachVisible(v)
          @lwf.renderObject(child.render(
            @lwf.renderingIndex, @lwf.renderingCount, rOffset))
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

    if @attachedMovies?
      for attachedMovie in @attachedMovieList
        if attachedMovie?
          attachedMovie.inspect(inspector, hierarchy, d++, rOffset)

    if @attachedLWFs?
      for lwfContainer in @attachedLWFList
        if lwfContainer?
          child = lwfContainer.child
          @lwf.renderObject(child.inspect(inspector, hierarchy, d++, rOffset))
    return

  destroy: ->
    for obj in @displayList
      obj.destroy() if obj?

    if @attachedMovies?
      movie.destroy() for k, movie of @attachedMovies
      @attachedMovies = null
      @detachedMovies = null
      @attachedMovieList = null

    if @attachedLWFs?
      @execDetachHandler(lwfContainer) for k, lwfContainer of @attachedLWFs
      @attachedLWFs = null
      @detachedLWFs = null
      @attachedLWFList = null

    @unloadFunc.call(@) if @unloadFunc?
    @playAnimation(ClipEvent.UNLOAD)

    @handler.call("unload", @) if @handler?

    @instanceHead = null
    @instanceTail = null
    @displayList = null
    @property = null

    super
    return

  playAnimation:(clipEvent) ->
    clipEvents = @lwf.data.movieClipEvents
    for i in [0...@data.clipEvents]
      c = clipEvents[@data.clipEventId + i]
      if (c.clipEvent & clipEvent) isnt 0
        @lwf.playAnimation(c.animationId, @)
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
    while instance?
      if instance.isMovie() and
          @lwf.getInstanceNameStringId(instance.instanceId) == stringId
        return instance
      else if recursive and instance.isMovie()
        i = instance.searchMovieInstance(stringId, recursive)
        return i if i?
      instance = instance.linkInstance

    return null

  searchMovieInstanceByInstanceId:(instId, recursive) ->
    instance = @instanceHead
    while instance?
      if instance.isMovie() and instance.instanceId == instId
        return instance
      else if recursive and instance.isMovie()
        i = instance.searchMovieInstanceByInstanceId(instId, recursive)
        return i if i?
      instance = instance.linkInstance
    return null

  searchButtonInstance:(stringId, recursive = true) ->
    stringId = @lwf.getStringId(stringId) if typeof stringId is "string"
    instance = @instanceHead
    while instance?
      if instance.isButton() and
          @lwf.getInstanceNameStringId(instance.instanceId) == stringId
        return instance
      else if recursive and instance.isMovie()
        i = instance.searchButtonInstance(stringId, recursive)
        return i if i?
      instance = instance.linkInstance

    return null

  searchButtonInstanceByInstanceId:(instId, recursive) ->
    instance = @instanceHead
    while instance?
      if instance.isButton() and instance.instanceId == instId
        return instance
      else if recursive and instance.isMovie()
        i = instance.searchMovieInstanceByInstanceId(instId, recursive)
        return i if i?
      instance = instance.linkInstance
    return null

  move:(x, y) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.move(x, y)
    return @

  moveTo:(x, y) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.moveTo(x, y)
    return @

  rotate:(degree) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.rotate(degree)
    return @

  rotateTo:(degree) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.rotateTo(degree)
    return @

  scale:(x, y) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.scale(x, y)
    return @

  scaleTo:(x, y) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.scaleTo(x, y)
    return @

  setMatrix:(m, scaleX = 1, scaleY = 1, rotation = 0) ->
    @property.setMatrix(m, scalex, scaleY, rotation)
    return @

  setAlpha:(alpha) ->
    Utility.getColorTransform(@) unless @property.hasColorTransform
    @property.setAlpha(alpha)
    return @

  setColorTransform:(c) ->
    @property.setColorTransform(c)
    return @

  setRenderingOffset:(rOffset) ->
    @property.setRenderingOffset(rOffset)
    return @

  getX: ->
    Utility.getMatrix(@) unless @property.hasMatrix
    return @property.matrix.translateX

  setX:(v) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.moveTo(v, @property.matrix.translateY)
    return

  getY: ->
    Utility.getMatrix(@) unless @property.hasMatrix
    return @property.matrix.translateY

  setY:(v) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.moveTo(@property.matrix.translateX, v)
    return

  getScaleX: ->
    Utility.getMatrix(@) unless @property.hasMatrix
    return @property.scaleX

  setScaleX:(v) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.scaleTo(v, @property.scaleY)
    return

  getScaleY: ->
    Utility.getMatrix(@) unless @property.hasMatrix
    return @property.scaleY

  setScaleY:(v) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.scaleTo(@property.scaleX, v)
    return

  getRotation: ->
    Utility.getMatrix(@) unless @property.hasMatrix
    return @property.rotation

  setRotation:(v) ->
    Utility.getMatrix(@) unless @property.hasMatrix
    @property.rotateTo(v)
    return

  getAlphaProperty: ->
    Utility.getColorTransform(@) unless @property.hasColorTransform
    return @property.colorTransform.multi.alpha

  setAlphaProperty:(v) ->
    Utility.getColorTransform(@) unless @property.hasColorTransform
    @property.setAlpha(v)
    return
