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

class LWF
  ROUND_OFF_TICK_RATE = 0.05

  constructor:(lwfData, \
      rendererFactory = null, embeddedScript = null, @privateData = null) ->
    @data = lwfData
    @functions = embeddedScript() if embeddedScript?
    @width = @data.header.width
    @height = @data.header.height
    @backgroundColor = @data.header.backgroundColor
    @name = @data.strings[@data.header.nameStringId]
    @interactive = @data.buttonConditions.length > 0
    @url = null
    @frameRate = @data.header.frameRate
    @execLimit = 3
    @tick = 1.0 / @frameRate
    @roundOffTick = @tick * ROUND_OFF_TICK_RATE
    @time = 0
    @thisTick = 0
    @attachVisible = true
    @execCount = 0
    @isExecDisabled = false
    @isPropertyDirty = false
    @isLWFAttached = false
    @interceptByNotAllowOrDenyButtons = true
    @intercepted = false
    @scaleByStage = 1
    @pointX = Number.MIN_VALUE
    @pointY = Number.MIN_VALUE
    @pressing = false
    @buttonHead = null

    @disableExec() if !@interactive and @data.frames.length is 1

    @property = new Property(@)
    @instances = []
    @eventHandlers = []
    @movieEventHandlers = []
    @buttonEventHandlers = []
    @movieCommands = {}
    @programObjectConstructors = []

    @parent = null
    @attachName = null
    @depth = null

    @matrix = new Matrix
    @matrixIdentity = new Matrix
    @colorTransform = new ColorTransform
    @colorTransformIdentity = new ColorTransform

    @init()

    @setRendererFactory(rendererFactory)

  setRendererFactory:(rendererFactory = null) ->
    rendererFactory = new NullRendererFactory() unless rendererFactory?
    @rendererFactory = rendererFactory
    @rendererFactory.init(@)
    return

  setFrameRate:(frameRate) ->
    return if frameRate is 0
    @frameRate = frameRate
    @tick = 1.0 / @frameRate
    return

  setPreferredFrameRate:(preferredFrameRate, execLimit = 2) ->
    return if frameRate is 0
    @execLimit = Math.ceil(@frameRate / preferredFrameRate) + execLimit
    return

  fitForHeight:(stageWidth, stageHeight) ->
    Utility.fitForHeight(@, stageWidth, stageHeight)
    return

  fitForWidth:(stageWidth, stageHeight) ->
    Utility.fitForWidth(@, stageWidth, stageHeight)
    return

  scaleForHeight:(stageHeight) ->
    Utility.scaleForHeight(@, stageHeight)
    return

  scaleForWidth:(stageWidth) ->
    Utility.scaleForWidth(@, stageWidth)
    return

  renderOffset: ->
    @renderingIndexOffsetted = 0
    return

  clearRenderOffset: ->
    @renderingIndexOffsetted = @renderingIndex
    return

  renderObject:(count = 1) ->
    @renderingIndex += count
    @renderingIndexOffsetted += count
    return @renderingIndex

  setAttachVisible:(visible) ->
    @attachVisible = visible
    return

  clearFocus:(button) ->
    @focus = null if @focus is button
    return

  clearIntercepted: ->
    @intercepted = false
    return

  init: ->
    @time = 0
    @progress = 0

    @instances = []
    @focus = null

    @movieCommands = {}

    @rootMovieStringId = @getStringId("_root")
    @rootMovie.destroy() if @rootMovie?
    @rootMovie = new Movie(@, null,
      @data.header.rootMovieId, @searchInstanceId(@rootMovieStringId))
    return

  calcMatrix:(matrix) ->
    p = @property
    if p.hasMatrix
      if matrix?
        m = Utility.calcMatrix(@matrix, matrix, p.matrix)
      else
        m = p.matrix
    else
      m = matrix ? @matrixIdentity
    return m

  calcColorTransform:(colorTransform) ->
    p = @property
    if p.hasColorTransform
      if colorTransform?
        c = Utility.calcColorTransform(
          @colorTransform, colorTransform, p.colorTransform)
      else
        c = p.colorTransform
    else
      c = colorTransform ? @colorTransformIdentity
    return c

  exec:(tick = 0, matrix = null, colorTransform = null) ->
    execed = false
    currentProgress = @progress

    if @isExecDisabled
      unless @executedForExecDisabled
        ++@execCount
        @rootMovie.exec()
        @rootMovie.postExec(true)
        @executedForExecDisabled = true
        execed = true
    else
      progressing = true
      @thisTick = tick
      if tick is 0
        @progress = @tick
      else if tick < 0
        @progress = @tick
        progressing = false
      else
        if @time is 0
          @time += @tick
          @progress += @tick
        else
          @time += tick
          @progress += tick

      execLimit = @execLimit
      while @progress >= @tick - @roundOffTick
        if --execLimit < 0
          @progress = 0
          break
        @progress -= @tick
        ++@execCount
        @rootMovie.exec()
        @rootMovie.postExec(progressing)
        execed = true

      if @progress < @roundOffTick
        @progress = 0

      @buttonHead = null
      @rootMovie.linkButton() if @interactive and @rootMovie.hasButton

    if execed or @isLWFAttached or @isPropertyDirty or
        matrix? or colorTransform?
      @update(matrix, colorTransform)

    unless @execDisabled
      if tick < 0
        @progress = currentProgress

    return @renderingCount

  forceExec:(matrix = null, colorTransform = null) ->
    return @exec(0, matrix, colorTransform)

  forceExecWithoutProgress:(matrix = null, colorTransform = null) ->
    return @exec(-1, matrix, colorTransform)

  update:(matrix = null, colorTransform = null) ->
    m = @calcMatrix(matrix)
    c = @calcColorTransform(colorTransform)
    @renderingIndex = 0
    @renderingIndexOffsetted = 0
    @rootMovie.update(m, c)
    @renderingCount = @renderingIndex
    @thisTick = 0
    @isPropertyDirty = false
    return

  render:(rIndex = 0, rCount = 0, rOffset = Number.MIN_VALUE) ->
    renderingCountBackup = @renderingCount
    @renderingCount = rCount if rCount > 0
    @renderingIndex = rIndex
    @renderingIndexOffsetted = rIndex
    if @property.hasRenderingOffset
      @renderOffset()
      rOffset = @property.renderingOffset
    @rendererFactory.beginRender(@)
    @rootMovie.render(@attachVisible, rOffset)
    @rendererFactory.endRender(@)
    @renderingCount = renderingCountBackup
    return @renderingCount

  inspect:(inspector, hierarchy = 0, \
      depth = 0, rIndex = 0, rCount = 0, rOffset = Number.MIN_VALUE) ->
    renderingCountBackup = @renderingCount
    @renderingCount = rCount if rCount > 0
    @renderingIndex = rIndex
    @renderingIndexOffsetted = rIndex
    if @property.hasRenderingOffset
      @renderOffset()
      rOffset = @property.renderingOffset

    @rootMovie.inspect(inspector, hierarchy, depth, rOffset)
    @renderingCount = renderingCountBackup
    return @renderingCount

  destroy: ->
    @stopTweens() if @stopTweens?
    @rootMovie.destroy()
    @rootMovie = null
    func = @functions?['destroy']
    func.call(@) if func?
    @functions = null
    if @rendererFactory?
      resourceCache = @rendererFactory.resourceCache
      resourceCache.unloadLWF(@) if resourceCache?
      @rendererFactory.destruct()
      @rendererFactory = null
    @property = null
    @buttonHead = null
    @instances = null
    @eventHandlers = null
    @movieEventHandlers = null
    @buttonEventHandlers = null
    @movieCommands = null
    @programObjectConstructors = null
    return

  getInstanceNameStringId:(instId) ->
    if instId < 0 or instId >= @data.instanceNames.length
      return -1
    else
      return @data.instanceNames[instId].stringId

  getStringId:(str) ->
    i = @data.stringMap[str]
    return if i? then i else -1

  searchInstanceId:(stringId) ->
    return -1 if stringId < 0 or stringId >= @data.strings.length
    i = @data.instanceNameMap[stringId]
    return if i? then i else -1

  searchFrame:(movie, stringId) ->
    stringId = @getStringId(stringId) if typeof stringId is "string"
    return -1 if stringId < 0 or stringId >= @data.strings.length
    frameNo = @data.labelMap[movie.objectId][stringId]
    return if frameNo? then frameNo + 1 else -1

  getMovieLabels:(movie) ->
    return null unless movie?
    return @data.labelMap[movie.objectId]

  searchMovieLinkage:(stringId) ->
    return -1 if stringId < 0 or stringId >= @data.strings.length
    i = @data.movieLinkageMap[stringId]
    return if i? then @data.movieLinkages[i].movieId else -1

  getMovieLinkageName:(movieId) ->
    i = @data.movieLinkageNameMap[movieId]
    return if i? then @data.strings[i] else null

  searchMovieInstance:(stringId) ->
    if typeof stringId is "string"
      instanceName = stringId
      if instanceName.indexOf(".") isnt -1
        names = instanceName.split(".")
        return null if names[0] isnt @data.strings[@rootMovieStringId]
        m = @rootMovie
        for name in names
          m = m.searchMovieInstance(name, false)
          return null unless m?
        return m
      stringId = @getStringId(stringId)
    return @searchMovieInstanceByInstanceId(@searchInstanceId(stringId))

  searchMovieInstanceByInstanceId:(instId) ->
    if typeof instId is "string"
      instId = @searchInstanceId(@getStringId(instId))
    return null if instId < 0 or instId >= @data.instanceNames.length
    obj = @instances[instId]
    while obj?
      return obj if obj.isMovie
      obj = obj.nextInstance
    return null

  searchButtonInstance:(stringId) ->
    if typeof stringId is "string"
      instanceName = stringId
      if instanceName.indexOf(".") isnt -1
        names = instanceName.split(".")
        return null if names[0] isnt @data.strings[@rootMovieStringId]
        m = @rootMovie
        for i in [1...names.length]
          if i is names.length - 1
            return m.searchButtonInstance(names[i], false)
          else
            m = m.searchButtonInstance(names[i], false)
            return null unless m?
        return null
      stringId = @getStringId(stringId)
    return @searchButtonInstanceByInstanceId(@searchInstanceId(stringId))

  searchButtonInstanceByInstanceId:(instId) ->
    if typeof instId is "string"
      instId = @searchInstanceId(@getStringId(instId))
    return null if instId < 0 or instId >= @data.instanceNames.length
    obj = @instances[instId]
    while obj?
      return obj if obj.isButton
      obj = obj.nextInstance
    return null

  searchEventId:(stringId) ->
    stringId = @getStringId(stringId) if typeof stringId is "string"
    return -1 if stringId < 0 or stringId >= @data.strings.length
    i = @data.eventMap[stringId]
    return if i? then i else -1

  searchProgramObjectId:(stringId) ->
    stringId = @getStringId(stringId) if typeof stringId is "string"
    return -1 if stringId < 0 or stringId >= @data.strings.length
    i = @data.programObjectMap[stringId]
    return if i? then i else -1

  getInstance:(instId) ->
    return @instances[instId]

  setInstance:(instId, instance) ->
    @instances[instId] = instance
    return

  addEventHandler:(eventId, eventHandler) ->
    eventId = @searchEventId(eventId) if typeof eventId is "string"
    return if eventId < 0 or eventId >= @data.events.length
    @eventHandlers[eventId] ?= []
    @eventHandlers[eventId].push(eventHandler)
    return

  removeEventHandler:(eventId, eventHandler) ->
    eventId = @searchEventId(eventId) if typeof eventId is "string"
    return if eventId < 0 or eventId >= @data.events.length
    handlers = @eventHandlers[eventId]
    return unless handlers?
    i = 0
    while i < handlers.length
      if handlers[i] is eventHandler
        handlers.splice(i, 1)
      else
        ++i
    return

  clearEventHandler:(eventId) ->
    eventId = @searchEventId(eventId) if typeof eventId is "string"
    return if eventId < 0 or eventId >= @data.events.length
    @eventHandlers[eventId] = null
    return

  setEventHandler:(eventId, eventHandler) ->
    @clearEventHandler(eventId)
    @addEventHandler(eventId, eventHandler)
    return

  getProgramObjectConstructor:(programObjectId) ->
    if typeof programObjectId is "string"
      programObjectId = @searchProgramObjectId(@getStringId(programObjectId))
    return null if programObjectId < 0 or
      programObjectId >= @data.programObjects.length
    return @programObjectConstructors[programObjectId]

  setProgramObjectConstructor:(programObjectId, programObjectConstructor) ->
    if typeof programObjectId is "string"
      programObjectId = @searchProgramObjectId(@getStringId(programObjectId))
    return if programObjectId < 0 or
      programObjectId >= @data.programObjects.length
    @programObjectConstructors[programObjectId] = programObjectConstructor
    return

  getMovieEventHandlers:(m) ->
    if typeof m is "string"
      instanceName = m
      instId = @searchInstanceId(@getStringId(instanceName))
      if instId >= 0 and instId < @data.instanceNames.length
        return @movieEventHandlers[instId]
      else
        return null unless @movieEventHandlersByFullName?
        return @movieEventHandlersByFullName[instanceName]

    if @movieEventHandlersByFullName?
      fullName = m.getFullName()
      if fullName?
        handlers = @movieEventHandlersByFullName[fullName]
        return handlers if handlers?
    return @movieEventHandlers[m.instanceId]

  addMovieEventHandler:(instanceName, handlers) ->
    instId = @searchInstanceId(@getStringId(instanceName))
    if instId >= 0 and instId < @data.instanceNames.length
      h = @movieEventHandlers[instId]
      unless h?
        h = new MovieEventHandlers()
        @movieEventHandlers[instId] = h
      movie = @searchMovieInstanceByInstanceId(instId)
      movie.setHandlers(h) if movie?
    else
      return if instanceName.indexOf(".") is -1
      @movieEventHandlersByFullName ?= []
      h = @movieEventHandlersByFullName[instanceName]
      unless h?
        h = new MovieEventHandlers()
        @movieEventHandlersByFullName[instanceName] = h
      movie = @searchMovieInstance(instanceName)
      movie.setHandlers(h) if movie?
    h.add(handlers)
    return

  removeMovieEventHandler:(instanceName, handlers) ->
    h = @getMovieEventHandlers(instanceName)
    h.remove(handlers) if h?
    return

  clearMovieEventHandler:(instanceName, type = null) ->
    h = @getMovieEventHandlers(instanceName)
    h.clear(type) if h?
    return

  setMovieEventHandler:(instanceName, handlers) ->
    @clearMovieEventHandler(instanceName)
    @addMovieEventHandler(instanceName, handlers)
    return

  getButtonEventHandlers:(m) ->
    if typeof m is "string"
      instanceName = m
      instId = @searchInstanceId(@getStringId(instanceName))
      if instId >= 0 and instId < @data.instanceNames.length
        return @buttonEventHandlers[instId]
      else
        return null unless @buttonEventHandlersByFullName?
        return @buttonEventHandlersByFullName[instanceName]

    if @buttonEventHandlersByFullName?
      fullName = m.getFullName()
      if fullName?
        handlers = @buttonEventHandlersByFullName[fullName]
        return handlers if handlers?
    return @buttonEventHandlers[m.instanceId]

  addButtonEventHandler:(instanceName, handlers) ->
    @interactive = true
    instId = @searchInstanceId(@getStringId(instanceName))
    if instId >= 0 and instId < @data.instanceNames.length
      h = @buttonEventHandlers[instId]
      unless h?
        h = new ButtonEventHandlers()
        @buttonEventHandlers[instId] = h
      button = @searchButtonInstanceByInstanceId(instId)
      button.setHandlers(h) if button?
    else
      return if instanceName.indexOf(".") is -1
      @buttonEventHandlersByFullName ?= []
      h = @buttonEventHandlersByFullName[instanceName]
      unless h?
        h = new ButtonEventHandlers()
        @buttonEventHandlersByFullName[instanceName] = h
      button = @searchButtonInstance(instanceName)
      button.setHandlers(h) if movie?
    h.add(handlers)
    return

  removeButtonEventHandler:(instanceName, handlers) ->
    h = @getButtonEventHandlers(instanceName)
    h.remove(handlers) if h?
    return

  clearButtonEventHandler:(instanceName, type = null) ->
    h = @getButtonEventHandlers(instanceName)
    h.clear(type) if h?
    return

  setButtonEventHandler:(instanceName, handlers) ->
    @clearButtonEventHandler(instanceName)
    @addButtonEventHandler(instanceName, handlers)
    return

  execMovieCommand: ->
    deletes = []
    for k, v of @movieCommands
      available = true
      movie = @rootMovie
      for name in k
        movie = movie.searchMovieInstance(name)
        unless movie?
          available = false
          break
      if available
        v(movie)
        deletes.push k

    for k in deletes
      delete @movieCommands[k]
    return

  setMovieCommand:(instanceNames, cmd) ->
    names = instanceNames.slice(0)
    @movieCommands[names] = cmd
    @execMovieCommand()
    return

  searchAttachedMovie:(attachName) ->
    return @rootMovie.searchAttachedMovie(attachName)

  searchAttachedLWF:(attachName) ->
    return @rootMovie.searchAttachedLWF(attachName)

  addAllowButton:(buttonName) ->
    instId = @searchInstanceId(@getStringId(buttonName))
    return false if instId < 0

    @allowButtonList = {} unless @allowButtonList?
    @allowButtonList[instId] = true
    return true

  removeAllowButton:(buttonName) ->
    return false unless @allowButtonList?

    instId = @searchInstanceId(@getStringId(buttonName))
    return false if instId < 0

    delete @allowButtonList[instId]
    return true

  clearAllowButton: ->
    @allowButtonList = null
    return

  addDenyButton:(buttonName) ->
    instId = @searchInstanceId(@getStringId(buttonName))
    return false if instId < 0

    @denyButtonList = {} unless @denyButtonList?
    @denyButtonList[instId] = true
    return true

  denyAllButtons: ->
    @denyButtonList = {} unless @denyButtonList?
    for instId in [0...@data.instanceNames.length]
      @denyButtonList[instId] = true
    return

  removeDenyButton:(buttonName) ->
    return false unless @denyButtonList?

    instId = @searchInstanceId(@getStringId(buttonName))
    return false if instId < 0

    delete @denyButtonList[instId]
    return true

  clearDenyButton: ->
    @denyButtonList = null
    return

  disableExec: ->
    @isExecDisabled = true
    @executedForExecDisabled = false
    return

  enableExec: ->
    @isExecDisabled = false
    return

  setPropertyDirty: ->
    @isPropertyDirty = true
    @parent.lwf.setPropertyDirty() if @parent?
    return

  getMovieFunctions:(movieId) ->
    linkageName = @getMovieLinkageName(movieId)
    loadFunc = @functions?[linkageName + "_load"]
    postLoadFunc = @functions?[linkageName + "_postLoad"]
    unloadFunc = @functions?[linkageName + "_unload"]
    enterFrameFunc = @functions?[linkageName + "_enterFrame"]
    return [loadFunc, postLoadFunc, unloadFunc, enterFrameFunc]

  playAnimation:(animationId, movie, button) ->
    i = 0
    animations = @data.animations[animationId]
    target = movie

    loop
      a = animations[i++]
      switch a
        when Animation.END
          return
  
        when Animation.PLAY
          target.play()
  
        when Animation.STOP
          target.stop()
  
        when Animation.GOTONEXTFRAME
          target.gotoNextFrame()
  
        when Animation.GOTOPREVFRAME
          target.gotoPrevFrame()
  
        when Animation.GOTOFRAME
          target.gotoFrameInternal(animations[i++])
  
        when Animation.GOTOLABEL
          target.gotoFrame(@searchFrame(target, animations[i++]))
  
        when Animation.SETTARGET
          target = movie

          count = animations[i++]
          if count isnt 0
            for j in [0...count]
              instId = animations[i++]

              switch instId
                when Animation.INSTANCE_TARGET_ROOT
                  target = @rootMovie

                when Animation.INSTANCE_TARGET_PARENT
                  target = target.parent
                  target = @rootMovie unless @target?

                else
                  target =
                    target.searchMovieInstanceByInstanceId(instId, false)
                  target = movie unless target?
  
        when Animation.EVENT
          eventId = animations[i++]
          handlers = @eventHandlers[eventId]
          handler(movie, button) for handler in handlers if handlers?

        when Animation.CALL
          stringId = animations[i++]
          func = @functions?[@data.strings[stringId]]
          func.call(movie) if func?
    return

  inputPoint:(x, y) ->
    @intercepted = false
    return null unless @interactive

    @pointX = x
    @pointY = y

    found = false
    button = @buttonHead
    while button?
      if button.checkHit(x, y)
        if @allowButtonList?
          unless @allowButtonList[button.instanceId]?
            if @interceptByNotAllowOrDenyButtons
              @intercepted = true
              break
            else
              button = button.buttonLink
              continue

        else if @denyButtonList?
          if @denyButtonList[button.instanceId]?
            if @interceptByNotAllowOrDenyButtons
              @intercepted = true
              break
            else
              button = button.buttonLink
              continue

        found = true
        if @focus isnt button
          @focus.rollOut() if @focus?
          @focus = button
          @focus.rollOver()
        break
      button = button.buttonLink

    if !found and @focus?
      @focus.rollOut()
      @focus = null

    return @focus

  inputPress: ->
    return unless @interactive

    @pressing = true

    @focus.press() if @focus?
    return

  inputRelease: ->
    return unless @interactive

    @pressing = false

    @focus.release() if @focus?
    return

  inputKeyPress:(code) ->
    return unless @interactive

    button = @buttonHead
    while button?
      button.keyPress(code)
      button = button.buttonLink
    return
