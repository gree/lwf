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

class Game
  constructor:(@stage, @cache, @textNode = null) ->
    @requests = []

  requestLWF:(lwfName, onload) ->
    if lwfName.match(/(.*\/)([^\/]+)/)
      prefix = RegExp.$1
      lwfName = RegExp.$2
    else
      prefix = ""
    @requests.push({
      lwf:lwfName,
      prefix:prefix,
      stage:@stage,
      onload:onload,
      useBackgroundColor:true,
      #worker:false,
    })

  loadLWFs:(onloadall) ->
    @cache.loadLWFs(@requests, onloadall)
    @requests = []

  load:(lwfName) ->
    @requestLWF(lwfName, (lwf) => @lwf = lwf)
    @loadLWFs((errors) => @init() unless errors?)

  getTime: ->
    Date.now() / 1000.0

  inputPoint:(e) ->
    x = e.clientX + document.body.scrollLeft +
      document.documentElement.scrollLeft - @stage.offsetLeft
    y = e.clientY + document.body.scrollTop +
      document.documentElement.scrollTop - @stage.offsetTop
    @lwf.inputPoint(x, y)

  inputPress:(e) ->
    @inputPoint(e)
    @lwf.inputPress()

  inputRelease:(e) ->
    @inputPoint(e)
    @lwf.inputRelease()

  onmove:(e) =>
    do (e) =>
      @inputQueue.push(() => @inputPoint(e))
  onpress:(e) =>
    do (e) =>
      @inputQueue.push(() => @inputPress(e))
  onrelease:(e) =>
    do (e) =>
      @inputQueue.push(() => @inputRelease(e))

  init: ->
    @inputQueue = []
    @lwf.rendererFactory.fitForHeight(@lwf)
    @from = @getTime()
    @exec()

    @stage.addEventListener("mousedown", @onpress, false)
    @stage.addEventListener("mousemove", @onmove, false)
    @stage.addEventListener("mouseup", @onrelease, false)
    @stage.addEventListener("touchstart", @onpress, false)
    @stage.addEventListener("touchmove", @onmove, false)
    @stage.addEventListener("touchend", @onrelease, false)

  exec: ->
    if @destroyed?
      if @lwf?
        @stage.removeEventListener("mousedown", @onpress, false)
        @stage.removeEventListener("mousemove", @onmove, false)
        @stage.removeEventListener("mouseup", @onrelease, false)
        @stage.removeEventListener("touchstart", @onpress, false)
        @stage.removeEventListener("touchmove", @onmove, false)
        @stage.removeEventListener("touchend", @onrelease, false)
        @cache = null
        @lwf.destroy()
        @lwf = null
      requestAnimationFrame(=> @exec())
      return

    time = @getTime()
    tick = time - @from
    @from = time
    input() for input in @inputQueue
    @inputQueue = []
    @lwf.exec(tick)
    @lwf.render()

    if @textNode?
      fps = Math.round(1.0 / tick)
      @textNode.textContent = "#{fps}fps"

    requestAnimationFrame(=> @exec())

  destroy: ->
    @destroyed = true

window.onload = ->
  loadScript = (url, onload) ->
    script = document.createElement("script")
    script.type = "text/javascript"
    script.onload = ->
      onload()
    script.src = url
    head = document.getElementsByTagName('head')[0]
    head.appendChild(script)

  startGame = (renderer) ->
    if renderer is "webgl"
      LWF.useWebGLRenderer()
    else if renderer is "canvas"
      LWF.useCanvasRenderer()
    else
      LWF.useWebkitCSSRenderer()
    cache = LWF.ResourceCache.get()
    game = new Game(stage, cache, textNode)
    window.game = game
    game.load(lwfName)

  unless window.requestAnimationFrame?
    for vendor in ['webkit', 'moz']
      window.requestAnimationFrame = window[vendor+'RequestAnimationFrame']
      break if window.requestAnimationFrame?
  unless window.requestAnimationFrame?
    lastTime = 0
    window.requestAnimationFrame = (callback, element) ->
      currTime = new Date().getTime()
      lastTime = currTime if lastTime is 0
      timeToCall = Math.max(0, 16 - (currTime - lastTime))
      timeoutCallback = ->
        callback(currTime + timeToCall)
      id = window.setTimeout(timeoutCallback, timeToCall)
      lastTime = currTime + timeToCall
      return id

  if window.location.search.match(/lwf=([^=&]+)/)
    lwfName = RegExp.$1
  else
    lwfName = "testlwf.lwf"

  if window.location.search.match(/renderer=(canvas|webgl)/)
    renderer = RegExp.$1
    if renderer == "webgl"
      loadScript("lwf_webgl.js", -> startGame("webgl"))
    else
      loadScript("lwf.js", -> startGame("canvas"))
    stage = document.createElement("canvas")
    stage.style.position = "absolute"
    stage.width = 0
    stage.height = 0
    if window.location.search.match(/size=(\d+)x(\d+)/)
      stage.width = parseInt(RegExp.$1, 10)
      stage.height = parseInt(RegExp.$2, 10)
  else
    loadScript("lwf.js", -> startGame("css"))
    stage = document.createElement("div")
    if window.location.search.match(/size=(\d+)x(\d+)/)
      stage.style.width = RegExp.$1 + "px"
      stage.style.height = RegExp.$2 + "px"

  textNode = document.createTextNode("0fps")
  document.body.appendChild(textNode)
  document.body.appendChild(document.createElement('br'))
  div = document.createElement("div")
  document.body.appendChild(div)
  div.appendChild(stage)
