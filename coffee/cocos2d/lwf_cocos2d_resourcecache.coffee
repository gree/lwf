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

class Cocos2dResourceCache extends WebkitCSSResourceCache
  constructor: ->
    super
    @particlePrefix = "res/"
    @particleSuffix = ".plist"

  loadLWFNode:(settings) ->
    settings._loadLWF = true
    @loadLWF(settings)
    return

  loadLWFNodes:(settingsArray, onloadall) ->
    for settings in settingsArray
      settings._loadLWF = true
    @loadLWFs(settingsArray, onloadall)
    return

  newFactory:(settings, cache, data) ->
    return new Cocos2dRendererFactory(
      data, @, cache, settings["textInSubpixel"] ? false)

  loadLWFData:(settings, url) ->
    data = cc.FileUtils.getInstance().getByteArrayFromFile(url)
    data = Loader.loadArray(data)
    @onloaddata(settings, data, url)
    return

  onloadLWF:(settings, lwf) ->
    if settings._loadLWF?
      lwfNode = cc.LWFNode.lwfNodeWithLWF(lwf)
      s = settings["contentSize"]
      if s?
        lwfNode.setContentSize(s)
      factory = lwf.rendererFactory
      if settings["fitForHeight"]
        factory.fitForHeight(lwf)
      else if settings["fitForWidth"]
        factory.fitForWidth(lwf)
      delete settings._loadLWF
      settings["onload"].call(settings, lwfNode)
    else
      settings["onload"].call(settings, lwf)
    return

  loadJS:(settings, data) ->
    # TODO
    settings.loadedCount++
    @loadImages(settings, data)
    return

  onLoadImage: ->
    # @ is settings
    @loadedCount++
    @onprogress.call(@, @loadedCount, @total) if @onprogress?
    if @loadedCount is @total
      cache = @_cache
      data = @_data
      imageCache = @_imageCache
      delete @_cache
      delete @_data
      delete @_imageCache
      cache.newLWF(@, imageCache, data)
    return

  loadImages:(settings, data) ->
    prefix = settings["imagePrefix"] ? settings["prefix"] ? ""
    suffix = settings["imageSuffix"] ? ""
    imageCache = {}
    settings._cache = @
    settings._data = data
    settings._imageCache = imageCache

    for texture in data.textures
      name = texture.filename
      url = settings["imageMap"]?[name] ? name
      url = prefix + url unless url.match(/^\//)
      url = url.replace(/(\.png|\.jpg)$/i, suffix + "$1")
      imageCache[name] = url
      tc = cc.TextureCache.getInstance()
      tc.addImageAsync(url, settings, @onLoadImage)
    return

  unloadLWF:(lwf) ->
    super(lwf)
    unless @cache[lwf.url]?
      for name, url of lwf.renderFactory.cache
        if url.match(/\.(png|jpg)$/i)
          cc.TextureCache.getInstance().removeTextureForKey(url)
    return

  setParticlePath:(prefix, suffix) ->
    @particlePrefix = prefix
    @particleSuffix = suffix
    return
