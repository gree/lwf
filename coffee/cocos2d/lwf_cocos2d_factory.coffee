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

class Cocos2dRendererFactory
  constructor:(data, @resourceCache, @cache, @textInSubpixel) ->
    @bitmapContexts = []
    for bitmap in data.bitmaps
      continue if bitmap.textureFragmentId is -1
      bitmapEx = new Format.BitmapEx()
      bitmapEx.matrixId = bitmap.matrixId
      bitmapEx.textureFragmentId = bitmap.textureFragmentId
      bitmapEx.u = 0
      bitmapEx.v = 0
      bitmapEx.w = 1
      bitmapEx.h = 1
      @bitmapContexts.push new Cocos2dBitmapContext(@, data, bitmapEx)

    @bitmapExContexts = []
    for bitmapEx in data.bitmapExs
      continue if bitmapEx.textureFragmentId is -1
      @bitmapExContexts.push new Cocos2dBitmapContext(@, data, bitmapEx)

    @textContexts = []
    for text in data.texts
      @textContexts.push new Cocos2dTextContext(@, data, text)

    @particleContexts = []
    for particle in data.particles
      @particleContexts.push new Cocos2dParticleContext(@, data, particle)

    @lwfNode = null

  destruct: ->
    context.destruct() for context in @bitmapContexts
    context.destruct() for context in @bitmapExContexts
    context.destruct() for context in @textContexts
    return

  init:(lwf) ->
    return if @setupedDomElementConstructor
    @setupedDomElementConstructor = true
    for progObj in lwf.data.programObjects
      name = lwf.data.strings[progObj.stringId]
      m = name.match(/^DOM_(.*)/)
      if m?
        domName = m[1]
        do (domName) =>
          lwf.setProgramObjectConstructor(name, (lwf_, objId, w, h) =>
            ctor = @resourceCache.domElementConstructor
            return null unless ctor?
            domElement = ctor(lwf_, domName, w, h)
            return null unless domElement?
            new WebkitCSSDomElementRenderer(@, domElement)
          )

  beginRender:(lwf) ->

  endRender:(lwf) ->

  setBlendMode:(blendMode) ->

  setMaskMode:(maskMode) ->

  constructBitmap:(lwf, objectId, bitmap) ->
    context = @bitmapContexts[objectId]
    new Cocos2dBitmapRenderer(context) if context

  constructBitmapEx:(lwf, objectId, bitmapEx) ->
    context = @bitmapExContexts[objectId]
    new Cocos2dBitmapRenderer(context) if context

  constructText:(lwf, objectId, text) ->
    context = @textContexts[objectId]
    new Cocos2dTextRenderer(lwf, context, text) if context

  constructParticle:(lwf, objectId, particle) ->
    context = @particleContexts[objectId]
    new Cocos2dParticleRenderer(context) if context

  convertColor:(lwf, d, c, t) ->
    Utility.calcColor(lwf, d, c, t)
    d.red = Math.round(d.red * 255)
    d.green = Math.round(d.green * 255)
    d.blue = Math.round(d.blue * 255)
    d.alpha = Math.round(d.alpha * 255)
    return

  getSize: ->
    if @lwfNode?
      s = @lwfNode.getContentSize()
      w = s.width
      h = s.height
    if w is 0 and h is 0
      s = cc.Director.getInstance().getWinSizeInPixels()
      w = s.width
      h = s.height
    return [w, h]

  fitForHeight:(lwf) ->
    [w, h] = @getSize()
    lwf.fitForHeight(w, h) if h isnt 0 and h isnt lwf.data.header.height
    return

  fitForWidth:(lwf) ->
    [w, h] = @getSize()
    lwf.fitForWidth(w, h) if w isnt 0 and w isnt lwf.data.header.width
    return

  scaleForHeight:(lwf) ->
    [w, h] = @getSize()
    lwf.scaleForHeight(w, h) if h isnt 0 and h isnt lwf.data.header.height
    return

  scaleForWidth:(lwf) ->
    [w, h] = @getSize()
    lwf.scaleForWidth(w, h) if w isnt 0 and w isnt lwf.data.header.width
    return

  setBackgroundColor:(v) ->
    if typeof v is "number"
      bgColor = v
    else if typeof v is "string"
      bgColor = parseInt(v, 16)
    else if v instanceof LWF
      lwf = v
      bgColor = lwf.data.header.backgroundColor
      bgColor |= 0xff << 24
    else
      return
    a = ((bgColor >> 24) & 0xff) / 255.0
    r = ((bgColor >> 16) & 0xff) / 255.0
    g = ((bgColor >>  8) & 0xff) / 255.0
    b = ((bgColor >>  0) & 0xff) / 255.0
    #TODO
    return
