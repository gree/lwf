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
      newUrl = imageMap.call(settings, url)
      url = newUrl if newUrl?
    else if typeof imageMap is 'object'
      newUrl = imageMap[url]
      url = newUrl if newUrl?
    url = prefix + url unless url.match(/^\//)
    url = url.replace(/(\.png|\.jpg)/i, suffix + "$1")
    return url

  checkTextures:(settings, data) ->
    settings._alphaMap = {}
    settings._rgbMap = {}
    settings._textures = []

    re = new RegExp("_atlas_(.*)_info_" +
      "([0-9])_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)", "i")

    for texture in data.textures
      m = texture.filename.match(/^(.*)_rgb_([0-9a-f]{6})(.*)$/i)
      if m?
        ma = texture.filename.match(re)
        if ma?
          orig = ma[1]
          rotated = if ma[2] is "1" then true else false
          u = parseInt(ma[3], 10)
          v = parseInt(ma[4], 10)
          w = parseInt(ma[5], 10)
          h = parseInt(ma[6], 10)
          x = parseInt(ma[7], 10)
          y = parseInt(ma[8], 10)
        else
          orig = m[1] + m[3]
          rotated = false
          u = 0
          v = 0
          w = null
          h = null
          x = 0
          y = 0
        rgb = m[2]
        settings._rgbMap[orig] ?= []
        settings._rgbMap[orig].push(
          filename: texture.filename
          rgb: rgb
          rotated:rotated
          u:u
          v:v
          w:w
          h:h
          x:x
          y:y
        )
        continue

      settings._textures.push(texture)
      url = @getTextureURL(settings, data, texture)
      m = url.match(/^(.*)_withalpha(.*\.)jpg/i)
      if m?
        pngURL = "#{m[1]}_alpha#{m[2]}png"
        pm = pngURL.match(/\/([^\/]+)$/)
        pngFilename = if pm? then pm[1] else pngURL
        t = new Format.TextureReplacement(pngFilename)
        t.url = pngURL
        settings._textures.push(t)
        settings._alphaMap[texture.filename] = [texture, t]
        settings._alphaMap[t.filename] = [texture, t]
    return

  onloaddata:(settings, data, url) ->
    unless data? and data.check()
      settings.error.push({url:url, reason:"dataError"})
      settings["onload"].call(settings, null)
      return

    settings["name"] = data.name()

    @checkTextures(settings, data)

    lwfUrl = settings["lwf"]
    @cache[lwfUrl] = {}
    @cache[lwfUrl].data = data
    settings.total = settings._textures.length + 1
    settings.total++ if data.useScript
    settings.loadedCount = 1
    settings["onprogress"].call(settings,
      settings.loadedCount, settings.total) if settings["onprogress"]?

    if data.useScript
      @loadJS(settings, data)
    else
      @loadImages(settings, data)
    return

  loadLWF:(settings) ->
    lwfUrl = settings["lwf"]
    url = lwfUrl
    url = (settings["prefix"] ? "") + url unless url.match(/^\//)
    settings.error = []

    if @cache[lwfUrl]?
      data = @cache[lwfUrl].data
      if data?
        settings["name"] = data.name()
        @checkTextures(settings, data)
        settings.total = settings._textures.length + 1
        settings.loadedCount = 1
        onprogress.call(settings,
          settings.loadedCount, settings.total) if onprogress?
        @loadImages(settings, data)
        return

    @loadLWFData(settings, url)
    return

  dispatchOnloaddata:(settings, \
      url, useWorker, useArrayBuffer, useWorkerWithArrayBuffer, data) ->
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
            worker = worker.onmessage = worker.onerror = null
            @onloaddata(settings, data, url)
          worker.onerror = (e) =>
            settings.error.push({url:workerJS, reason:"error"})
            worker = worker.onmessage = worker.onerror = null
            settings["onload"].call(settings, null)
          if useWorkerWithArrayBuffer and data.type isnt "base64"
            worker.webkitPostMessage(data.data)
          else
            worker.postMessage(data.data)

    unless workerJS?
      if data.type is "base64"
        data = (new Zlib.Inflate(atob(data.data))).decompress()
        if typeof Uint8Array isnt 'undefined' and
            typeof Uint16Array isnt 'undefined' and
            typeof Uint32Array isnt 'undefined'
          data = Loader.loadArrayBuffer(data)
        else
          data = Loader.loadArray(data)
      else if useArrayBuffer
        data = Loader.loadArrayBuffer(data.data)
      else
        data = Loader.load(data.data)
      @onloaddata(settings, data, url)

    return

  loadLWFData:(settings, url) ->
    onload = settings["onload"]
    useWorker = false
    useWorkerWithArrayBuffer = false
    if typeof Worker isnt 'undefined' and
        (!settings["worker"]? or settings["worker"])
      useWorker = true
      if typeof Worker.prototype.webkitPostMessage isnt "undefined"
        useWorkerWithArrayBuffer = true

    m = url.match(/([^\/]+)\.lwf\.js/i)
    if m?
      name = m[1].toLowerCase()
      head = document.getElementsByTagName('head')[0]
      script = document.createElement("script")
      script.type = "text/javascript"
      script.charset = "UTF-8"
      script.onabort = =>
        settings.error.push({url:url, reason:"abort"})
        head.removeChild(script)
        script = script.onload = script.onabort = script.onerror = null
        onload.call(settings, null)
      script.onerror = =>
        settings.error.push({url:url, reason:"error"})
        head.removeChild(script)
        script = script.onload = script.onabort = script.onerror = null
        onload.call(settings, null)
      script.onload = =>
        str = global["LWF"]?["DataScript"]?[name]
        head.removeChild(script)
        script = script.onload = script.onabort = script.onerror = null
        if str?
          data = type:"base64", data:str
          @dispatchOnloaddata(settings,
            url, useWorker, useArrayBuffer, useWorkerWithArrayBuffer, data)
        else
          settings.error.push({url:url, reason:"error"})
          onload.call(settings, null)

      script.src = url
      head.appendChild(script)
      return

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

      if useArrayBuffer
        data = type:"arraybuffer", data:xhr.response
      else
        data = type:"text", data:xhr.responseText
      @dispatchOnloaddata(settings, url,
        useWorker, useArrayBuffer, useWorkerWithArrayBuffer, data)

      xhr = xhr.onabort = xhr.onerror = xhr.onreadystatechange = null

    xhr.send null
    return

  loadJS:(settings, data) ->
    lwfUrl = settings["lwf"]
    url = settings["js"] ? lwfUrl.replace(/\.lwf(\.js)?/i, ".js")
    url = (settings["prefix"] ? "") + url unless url.match(/^\//)
    onload = settings["onload"]
    onprogress = settings["onprogress"]

    script = document.createElement("script")
    script.type = "text/javascript"
    script.charset = "UTF-8"
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
      @cache[lwfUrl].script = script
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
        settings["onload"].call(settings, null)
      else
        @newLWF(settings, imageCache, data)
    return

  generateImages:(settings, imageCache, texture, image) ->
    d = settings._rgbMap[texture.filename]
    if d?
      for o in d
        w = o.w ? image.width
        h = o.h ? image.height
        if @.constructor is WebkitCSSResourceCache
          name = "canvas_" + o.filename.replace(/[\.-]/g, "_")
          ctx = document.getCSSCanvasContext(
            "2d", name, w, h)
          canvas = ctx.canvas
          canvas.name = name
        else
          canvas = document.createElement('canvas')
          canvas.width = w
          canvas.height = h
          ctx = canvas.getContext('2d')
        ctx.fillStyle = "##{o.rgb}"
        ctx.fillRect(0, 0, w, h)
        ctx.globalCompositeOperation = 'destination-in'
        if o.rotated
          m = new Matrix()
          Utility.rotateMatrix(m, new Matrix(), 1, o.x, o.y + h)
          ctx.setTransform(
            m.scaleX, m.skew1, m.skew0, m.scaleY, m.translateX, m.translateY)
        else if o.x isnt 0 or o.y isnt 0
          m = new Matrix()
          Utility.scaleMatrix(m, new Matrix(), 1, o.x, o.yy)
          ctx.setTransform(
            m.scaleX, m.skew1, m.skew0, m.scaleY, m.translateX, m.translateY)
        ctx.drawImage(image, o.u, o.v, w, h, 0, 0, w, h)
        imageCache[o.filename] = canvas
    return

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
                name = "canvas_" + jpg.filename.replace(/[\.-]/g, "_")
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
      @, cache, settings["stage"], settings["textInSubpixel"] ? false,
         settings["use3D"] ? true)

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
    cache.instances ?= 0
    cache.instances++
    factory = @newFactory(settings, imageCache, data)
    embeddedScript = global["LWF"]?["Script"]?[data.name()] if data.useScript
    lwf = new LWF(data, factory, embeddedScript, settings["privateData"])
    lwf.url = settings["lwf"]
    if settings["preferredFrameRate"]?
      if settings["execLimit"]?
        lwf.setPreferredFrameRate(
          settings["preferredFrameRate"], settings["execLimit"])
      else
        lwf.setPreferredFrameRate(settings["preferredFrameRate"])
    @onloadLWF(settings, lwf)
    return

  unloadLWF:(lwf) ->
    cache = @cache[lwf.url]
    if cache? and --cache.instances <= 0
      head = document.getElementsByTagName('head')[0]
      head.removeChild(cache.script)
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
            errors = errors.concat(settings.error)
          ++loadedCount
          onloadall(errors) if loadTotal is loadedCount
      @loadLWF(settings)

  getCache: ->
    return @cache

  setParticleConstructor:(ctor) ->
    @particleConstructor = ctor
    return

  setDOMElementConstructor:(ctor) ->
    @domElementConstructor = ctor
    return
