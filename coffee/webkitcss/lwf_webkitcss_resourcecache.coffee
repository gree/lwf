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
    @lwfInstanceIndex = 0

  clear: ->
    for k, cache of @cache
      lwfInstance.destroy() for kk, lwfInstance of cache.instances
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
    if not /^(https?:)?\/\//.test(url)
      url = prefix + url unless url.match(/^\//)
      url = url.replace(/(\.png|\.jpg)/i, suffix + "$1")
    return url

  checkTextures:(settings, data) ->
    settings._alphaMap = {}
    settings._colorMap = {}
    settings._textures = []

    re = new RegExp("_atlas_(.*)_info_" +
      "([0-9])_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)_([0-9]+)", "i")

    re_rgb = new RegExp("(.*)_rgb_([0-9a-f]{6})(.*)$", "i")
    re_rgb10 = new RegExp("(.*)_rgb_([0-9]*),([0-9]*),([0-9]*)(.*)$", "i")
    re_rgba = new RegExp("(.*)_rgba_([0-9a-f]{8})(.*)$", "i")
    re_rgba10 = new RegExp(
      "(.*)_rgba_([0-9]*),([0-9]*),([0-9]*),([0-9]*)(.*)$", "i")
    re_add = new RegExp("(.*)_add_([0-9a-f]{6})(.*)$", "i")
    re_add10 = new RegExp("(.*)_add_([0-9]*),([0-9]*),([0-9]*)(.*)$", "i")

    for texture in data.textures
      orig = null
      if (m = texture.filename.match(re_rgb))?
        orig = m[1] + m[3]
        colorOp = "rgb"
        colorValue = m[2]
      else if (m = texture.filename.match(re_rgb10))?
        orig = m[1] + m[5]
        colorOp = "rgb"
        r = parseInt(m[2], 10).toString(16)
        g = parseInt(m[3], 10).toString(16)
        b = parseInt(m[4], 10).toString(16)
        colorValue =
          (if r.length is 1 then "0" else "") + r +
          (if g.length is 1 then "0" else "") + g +
          (if b.length is 1 then "0" else "") + b
      else if (m = texture.filename.match(re_rgba))?
        orig = m[1] + m[3]
        colorOp = "rgba"
        colorValue = m[2]
      else if (m = texture.filename.match(re_rgba10))?
        orig = m[1] + m[6]
        colorOp = "rgba"
        r = parseInt(m[2], 10).toString(16)
        g = parseInt(m[3], 10).toString(16)
        b = parseInt(m[4], 10).toString(16)
        a = parseInt(m[5], 10).toString(16)
        colorValue =
          (if r.length is 1 then "0" else "") + r +
          (if g.length is 1 then "0" else "") + g +
          (if b.length is 1 then "0" else "") + b +
          (if a.length is 1 then "0" else "") + a
      else if (m = texture.filename.match(re_add))?
        orig = m[1] + m[3]
        colorOp = "add"
        colorValue = m[2]
      else if (m = texture.filename.match(re_add10))?
        orig = m[1] + m[5]
        colorOp = "add"
        r = parseInt(m[2], 10).toString(16)
        g = parseInt(m[3], 10).toString(16)
        b = parseInt(m[4], 10).toString(16)
        colorValue =
          (if r.length is 1 then "0" else "") + r +
          (if g.length is 1 then "0" else "") + g +
          (if b.length is 1 then "0" else "") + b

      if orig?
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
          rotated = false
          u = 0
          v = 0
          w = null
          h = null
          x = 0
          y = 0
        settings._colorMap[orig] ?= []
        settings._colorMap[orig].push(
          filename: texture.filename
          colorOp: colorOp
          colorValue: colorValue
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
      m = texture.filename.match(/^(.*)_withalpha(.*\.)jpg(.*)$/i)
      if m?
        pngFilename = "#{m[1]}_alpha#{m[2]}png#{m[3]}"
        t = new Format.TextureReplacement(pngFilename)
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

    @cache[lwfUrl] = {}
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
        src = scripts[i].src.split('?')[0]
        m = src.match(re)
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
        data = global["LWF"].Base64.atobArray(data.data)
        data = (new global["LWF"].Zlib.Inflate(data)).decompress()
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
      lwfUrl = settings["lwf"]
      @cache[lwfUrl].scripts ?= []
      @cache[lwfUrl].scripts.push(script)
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
      if !(xhr.status is 0 or (xhr.status >= 200 and xhr.status < 300))
        settings.error.push({url:url, reason:"error"})
        xhr = xhr.onabort = xhr.onerror = xhr.onreadystatechange = null
        onload.call(settings, null)
        return

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
      script = script.onload = script.onabort = script.onerror = null
      @loadImages(settings, data)
    script.src = url
    head = document.getElementsByTagName('head')[0]
    head.appendChild(script)
    @cache[lwfUrl].scripts ?= []
    @cache[lwfUrl].scripts.push(script)
    return

  loadImagesCallback:(settings, imageCache, data) ->
    settings.loadedCount++
    settings["onprogress"].call(settings,
      settings.loadedCount, settings.total) if settings["onprogress"]?
    if settings.loadedCount is settings.total
      delete settings._alphaMap
      delete settings._colorMap
      delete settings._textures
      if settings.error.length > 0
        delete @cache[settings["lwf"]]
        settings["onload"].call(settings, null)
      else
        @newLWF(settings, imageCache, data)
    return

  drawImage:(ctx, image, o, x, y, u, v, h, iw, ih) ->
    if o.rotated
      m = new Matrix()
      Utility.rotateMatrix(m, new Matrix(), 1, x, y + h)
      ctx.setTransform(
        m.scaleX, m.skew1, m.skew0, m.scaleY, m.translateX, m.translateY)
    else if x isnt 0 or y isnt 0
      m = new Matrix()
      Utility.scaleMatrix(m, new Matrix(), 1, x, y)
      ctx.setTransform(
        m.scaleX, m.skew1, m.skew0, m.scaleY, m.translateX, m.translateY)
    ctx.drawImage(image, u, v, iw, ih, 0, 0, iw, ih)
    return

  createCanvas:(filename, w, h) ->
    if @.constructor is WebkitCSSResourceCache
      name = "canvas_" + filename.replace(/[\.,-]/g, "_")
      ctx = document.getCSSCanvasContext("2d", name, w, h)
      canvas = ctx.canvas
      canvas.name = name
    else
      canvas = document.createElement('canvas')
      canvas.width = w
      canvas.height = h
      ctx = canvas.getContext('2d')
    return [canvas, ctx]

  generateImages:(settings, imageCache, texture, image) ->
    d = settings._colorMap[texture.filename]
    if d?
      scale = image.width / texture.width
      for o in d
        x = Math.round(o.x * scale)
        y = Math.round(o.y * scale)
        u = Math.round(o.u * scale)
        v = Math.round(o.v * scale)
        w = Math.round((o.w ? texture.width) * scale)
        h = Math.round((o.h ? texture.height) * scale)
        if o.rotated
          iw = h
          ih = w
        else
          iw = w
          ih = h

        [canvas, ctx] = @createCanvas(o.filename, w, h)

        switch o.colorOp
          when "rgb"
            ctx.fillStyle = "##{o.colorValue}"
            ctx.fillRect(0, 0, w, h)
            ctx.globalCompositeOperation = 'destination-in'
            @drawImage(ctx, image, o, x, y, u, v, h, iw, ih)
          when "rgba"
            @drawImage(ctx, image, o, x, y, u, v, h, iw, ih)
            ctx.globalCompositeOperation = 'source-atop'
            val = o.colorValue
            r = parseInt(val.substr(0, 2), 16)
            g = parseInt(val.substr(2, 2), 16)
            b = parseInt(val.substr(4, 2), 16)
            a = parseInt(val.substr(6, 2), 16) / 255
            ctx.fillStyle = "rgba(#{r}, #{g}, #{b}, #{a})"
            ctx.fillRect(0, 0, w, h)
          when "add"
            canvasAdd = document.createElement('canvas')
            canvasAdd.width = w
            canvasAdd.height = h
            ctxAdd = canvasAdd.getContext('2d')
            ctxAdd.fillStyle = "##{o.colorValue}"
            ctxAdd.fillRect(0, 0, w, h)
            ctxAdd.globalCompositeOperation = 'destination-in'
            @drawImage(ctxAdd, image, o, x, y, u, v, h, iw, ih)
            @drawImage(ctx, image, o, x, y, u, v, h, iw, ih)
            ctx.globalCompositeOperation = 'lighter'
            ctx.drawImage(canvasAdd, 0, 0, canvasAdd.width, canvasAdd.height,
              0, 0, canvasAdd.width, canvasAdd.height)
        ctx.globalCompositeOperation = 'source-over'
        imageCache[o.filename] = canvas
    return

  loadImages:(settings, data) ->
    imageCache = {}

    if data.textures.length is 0
      @newLWF(settings, imageCache, data)
      return

    for texture in settings._textures
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
              [canvas, ctx] =
                @createCanvas(jpg.filename, jpgImg.width, jpgImg.height)
              ctx.drawImage(jpgImg,
                0, 0, jpgImg.width, jpgImg.height,
                0, 0, jpgImg.width, jpgImg.height)
              ctx.globalCompositeOperation = 'destination-in'
              ctx.drawImage(alphaImg,
                0, 0, alphaImg.width, alphaImg.height,
                0, 0, jpgImg.width, jpgImg.height)
              ctx.globalCompositeOperation = 'source-over'
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
    if settings["setBackgroundColor"]?
      factory.setBackgroundColor(settings["setBackgroundColor"])
    else if settings["useBackgroundColor"]
      factory.setBackgroundColor(lwf)
    if settings["fitForHeight"]
      factory.fitForHeight(lwf)
    else if settings["fitForWidth"]
      factory.fitForWidth(lwf)
    settings["onload"].call(settings, lwf)
    return

  newLWF:(settings, imageCache, data) ->
    lwfUrl = settings["lwf"]
    cache = @cache[lwfUrl]
    factory = @newFactory(settings, imageCache, data)
    embeddedScript = global["LWF"]?["Script"]?[data.name()] if data.useScript
    lwf = new LWF(data, factory, embeddedScript, settings["privateData"])
    lwf.url = settings["lwf"]
    lwf.lwfInstanceId = ++@lwfInstanceIndex
    cache.instances ?= {}
    cache.instances[lwf.lwfInstanceId] = lwf
    parentLWF = settings["parentLWF"]
    parentLWF.loadedLWFs[lwf.lwfInstanceId] = lwf if parentLWF?
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
    if cache?
      delete cache.instances[lwf.lwfInstanceId] if lwf.lwfInstanceId
      empty = true
      for k, v of cache.instances
        empty = false
        break
      if empty
        try
          head = document.getElementsByTagName('head')[0]
          head.removeChild(script) for script in cache.scripts
        catch e
          # ignore
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
