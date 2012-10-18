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

class WebkitCSSResourceCache
  _instance = null
  @get: ->
    _instance ?= new @()

  constructor: ->
    @cache = {}

  clear: ->
    @cache = {}

  getTextureURL:(settings, data, texture) ->
    prefix = settings["imagePrefix"] ? settings["prefix"] ? ""
    suffix = settings["imageSuffix"] ? ""
    imageMap = settings["imageMap"]
    url = texture.filename
    if typeof imageMap is 'function'
      newUrl = imageMap(url)
      url = newUrl if newUrl?
    else if typeof imageMap is 'object'
      newUrl = imageMap[url]
      url = newUrl if newUrl?
    url = prefix + url unless url.match(/^\//)
    url = url.replace(/(\.png|\.jpg)$/i, suffix + "$1")
    return url

  checkTextures:(settings, data) ->
    settings._alphaMap = {}
    settings._rgbMap = {}
    settings._textures = []

    for texture in data.textures
      m = texture.filename.match(/^(.*)_rgb_([0-9a-f]{6})(.*)$/i)
      if m?
        orig = m[1] + m[3]
        rgb = m[2]
        settings._rgbMap[orig] ?= []
        settings._rgbMap[orig].push(
          filename: texture.filename
          rgb: rgb
        )
        continue

      settings._textures.push(texture)
      url = @getTextureURL(settings, data, texture)
      m = url.match(/^(.*)_withalpha(.*\.)jpg$/i)
      if m?
        pngURL = "#{m[1]}_alpha#{m[2]}png"
        pm = pngURL.match(/\/([^\/]+)$/)
        pngFilename = if pm? then pm[1] else pngURL
        t = new Format.TextureReplacement(pngFilename)
        t.url = pngURL
        settings._textures.push(t)
        settings._alphaMap[texture.filename] = [texture, t]
        settings._alphaMap[t.filename] = [texture, t]

  onloaddata:(settings, data, url) ->
    unless data? and data.check()
      settings.error.push({url:url, reason:"dataError"})
      settings["onload"].call(settings, null)
      return

    @checkTextures(settings, data)

    lwfUrl = settings["lwf"]
    @cache[lwfUrl] = {}
    @cache[lwfUrl].__data__ = data
    settings.total = settings._textures.length + 1
    settings.total++ if data.useScript
    settings.loadedCount = 1
    settings["onprogress"].call(settings,
      settings.loadedCount, settings.total) if settings["onprogress"]?

    if data.useScript
      @loadJS(settings, data)
    else
      @loadImages(settings, data)

  loadLWF:(settings) ->
    lwfUrl = settings["lwf"]
    url = lwfUrl
    url = (settings["prefix"] ? "") + url unless url.match(/^\//)
    settings.error = []

    if @cache[lwfUrl]?
      data = @cache[lwfUrl].__data__
      if data?
        @checkTextures(settings, data)
        settings.total = settings._textures.length + 1
        settings.loadedCount = 1
        onprogress.call(settings,
          settings.loadedCount, settings.total) if onprogress?
        @loadImages(settings, data)
        return

    @loadLWFData(settings, url)

  loadLWFData:(settings, url) ->
    onload = settings["onload"]
    useWorker = false
    useWorkerWithArrayBuffer = false
    if typeof Worker isnt 'undefined' and
        (!settings["worker"]? or settings["worker"])
      useWorker = true
      if typeof Worker.prototype.webkitPostMessage isnt "undefined"
        useWorkerWithArrayBuffer = true

    xhr = new XMLHttpRequest
    xhr.open 'GET', url, true
    if typeof xhr.responseType is 'string' and
        typeof Uint8Array isnt 'undefined' and
        typeof Int32Array isnt 'undefined' and
        typeof Float32Array isnt 'undefined' and
        (!useWorker or useWorkerWithArrayBuffer)
      useArrayBuffer = true
      xhr.responseType = "arraybuffer"
    else
      useArrayBuffer = false
      xhr.overrideMimeType 'text/plain; charset=x-user-defined'
    xhr.onabort = =>
      settings.error.push({url:url, reason:"abort"})
      xhr = xhr.onabort = xhr.onerror = xhr.onreadystatechange = null
      onload.call(settings, null)
    xhr.onerror = =>
      settings.error.push({url:url, reason:"error"})
      xhr = xhr.onabort = xhr.onerror = xhr.onreadystatechange = null
      onload.call(settings, null)
    xhr.onreadystatechange = =>
      return if xhr.readyState isnt 4
      return if !(xhr.status in [0, 200])

      if useWorker
        workerJS = null
        scripts = document.getElementsByTagName("script")
        re = new RegExp("(^|.*\/#{__FILE__})$", "i")
        for i in [0...scripts.length]
          continue if scripts[i].src is ""
          m = scripts[i].src.match(re)
          if m?
            workerJS = m[1]
            break
        if workerJS?
          do (workerJS) =>
            worker = new Worker(workerJS)
            worker.onmessage = (e) =>
              data = new Data(e.data)
              @onloaddata(settings, data, url)
            worker.onerror = (e) =>
              settings.error.push({url:workerJS, reason:"error"})
              settings["onload"].call(settings, null)
            if useWorkerWithArrayBuffer
              worker.webkitPostMessage(xhr.response)
            else
              worker.postMessage(xhr.response)

      unless workerJS?
        if useArrayBuffer
          data = Loader.loadArrayBuffer(xhr.response)
          @onloaddata(settings, data, url)
        else
          data = Loader.load(xhr.responseText)
          @onloaddata(settings, data, url)

      xhr = xhr.onabort = xhr.onerror = xhr.onreadystatechange = null

    xhr.send null
    return

  loadJS:(settings, data) ->
    lwfUrl = settings["lwf"]
    url = settings["js"] ? lwfUrl.replace(/\.lwf$/i, ".js")
    url = (settings["prefix"] ? "") + url unless url.match(/^\//)
    onload = settings["onload"]
    onprogress = settings["onprogress"]

    script = document.createElement("script")
    script.type = "text/javascript"
    script.onabort = =>
      delete @cache[lwfUrl]
      settings.error.push({url:url, reason:"abort"})
      script = script.onload = script.onabort = script.onerror = null
      onload.call(settings, null)
    script.onerror = =>
      delete @cache[lwfUrl]
      settings.error.push({url:url, reason:"error"})
      script = script.onload = script.onabort = script.onerror = null
      onload.call(settings, null)
    script.onload = =>
      settings.loadedCount++
      onprogress.call(settings,
        settings.loadedCount, settings.total) if onprogress?
      script = script.onload = script.onabort = script.onerror = null
      @loadImages(settings, data)
    script.src = url
    head = document.getElementsByTagName('head')[0]
    head.appendChild(script)
    return

  loadImagesCallback:(settings, imageCache, data) ->
    settings.loadedCount++
    settings["onprogress"].call(settings,
      settings.loadedCount, settings.total) if settings["onprogress"]?
    if settings.loadedCount is settings.total
      delete settings._alphaMap
      delete settings._rgbMap
      delete settings._textures
      if settings.error.length > 0
        delete @cache[settings["lwf"]]
        onload.call(settings, null)
      else
        @newLWF(settings, imageCache, data)

  generateImages:(settings, imageCache, texture, image) ->
    d = settings._rgbMap[texture.filename]
    if d?
      for o in d
        if @.constructor is WebkitCSSResourceCache
          name = "canvas_" + o.filename.replace(/[\.-]/g, "_")
          ctx = document.getCSSCanvasContext(
            "2d", name, image.width, image.height)
          canvas = ctx.canvas
          canvas.name = name
        else
          canvas = document.createElement('canvas')
          canvas.width = image.width
          canvas.height = image.height
          ctx = canvas.getContext('2d')
        ctx.fillStyle = "##{o.rgb}"
        ctx.fillRect(0, 0, image.width, image.height)
        ctx.globalCompositeOperation = 'destination-in'
        ctx.drawImage(image, 0, 0, image.width, image.height)
        imageData = ctx.getImageData(0, 0, image.width, image.height)
        pixels = imageData.data
        i = 3
        n = pixels.length
        while i < n
          i += 4
          pixels[i] = 255 if pixels[i] > 0
        ctx.putImageData(imageData, 0, 0)
        imageCache[o.filename] = canvas

  loadImages:(settings, data) ->
    imageCache = {}

    if data.textures.length is 0
      @newLWF(settings, imageCache, data)
      return

    for texture in settings._textures
      if texture.url?
        url = texture.url
      else
        url = @getTextureURL(settings, data, texture)
      do (texture, url) =>
        image = new Image()
        image.onabort = =>
          settings.error.push({url:url, reason:"abort"})
          image = image.onload = image.onabort = image.onerror = null
          @loadImagesCallback(settings, imageCache, data)
        image.onerror = =>
          settings.error.push({url:url, reason:"error"})
          image = image.onload = image.onabort = image.onerror = null
          @loadImagesCallback(settings, imageCache, data)
        image.onload = =>
          imageCache[texture.filename] = image
          d = settings._alphaMap[texture.filename]
          if d?
            jpg = d[0]
            alpha = d[1]
            jpgImg = imageCache[jpg.filename]
            alphaImg = imageCache[alpha.filename]
            if jpgImg? and alphaImg?
              if @.constructor is WebkitCSSResourceCache
                name = "canvas_" + jpg.filename.replace(/[\.-]/, "_")
                ctx = document.getCSSCanvasContext(
                  "2d", name, jpgImg.width, jpgImg.height)
                canvas = ctx.canvas
                canvas.name = name
              else
                canvas = document.createElement('canvas')
                canvas.width = jpgImg.width
                canvas.height = jpgImg.height
                ctx = canvas.getContext('2d')
              ctx.drawImage(jpgImg, 0, 0, jpgImg.width, jpgImg.height)
              ctx.globalCompositeOperation = 'destination-in'
              ctx.drawImage(alphaImg, 0, 0, jpgImg.width, jpgImg.height)
              delete imageCache[jpg.filename]
              delete imageCache[alpha.filename]
              imageCache[jpg.filename] = canvas
              @generateImages(settings, imageCache, jpg, canvas)
          else
            @generateImages(settings, imageCache, texture, image)

          image = image.onload = image.onabort = image.onerror = null
          @loadImagesCallback(settings, imageCache, data)
        image.src = url
    return

  newFactory:(settings, cache, data) ->
    return new WebkitCSSRendererFactory(data,
      @, cache, settings["stage"], settings["textInSubpixel"] ? false, true)

  onloadLWF:(settings, lwf) ->
    factory = lwf.rendererFactory
    factory.setBackgroundColor(lwf) if settings["useBackgroundColor"]
    if settings["fitForHeight"]
      factory.fitForHeight(lwf)
    else if settings["fitForWidth"]
      factory.fitForWidth(lwf)
    settings["onload"].call(settings, lwf)
    return

  newLWF:(settings, imageCache, data) ->
    lwfUrl = settings["lwf"]
    cache = @cache[lwfUrl]
    cache.__instances__ ?= 0
    cache.__instances__++
    factory = @newFactory(settings, imageCache, data)
    embeddedScript = global["LWF"]?["Script"]?[data.name()] if data.useScript
    lwf = new LWF(data, factory, embeddedScript, settings["privateData"])
    lwf.url = settings["lwf"]
    @onloadLWF(settings, lwf)
    return

  unloadLWF:(lwf) ->
    if @cache[lwf.url]? and --@cache[lwf.url].__instances__ <= 0
      delete @cache[lwf.url]
    return

  loadLWFs:(settingsArray, onloadall) ->
    loadTotal = settingsArray.length
    loadedCount = 0
    errors = null
    for settings in settingsArray
      onload = settings["onload"]
      do (onload) =>
        settings["onload"] = (lwf) =>
          onload(lwf) if onload?
          if settings.error.length > 0
            errors ?= []
            errors.apply(settings.error)
          ++loadedCount
          onloadall(errors) if loadTotal is loadedCount
      @loadLWF(settings)

  getCache: ->
    return @cache

  setParticleConstructor:(ctor) ->
    @particleConstructor = ctor

  setDOMElementConstructor:(ctor) ->
    @domElementConstructor = ctor
