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

if cc?

  cc.LWFBitmap = cc.Sprite.extend(
    _m_lwf_matrix:null

    ctor: ->
      cc.associateWithNative(@, cc.Sprite)

    initWithTexture:(texture, rect) ->
      result = @_super(texture, rect)
      @setAnchorPoint(cc.p(0, 1))
      return result

    setMatrix:(m) ->
      if typeof @_super isnt "undefined"
        @_super({a:m.scaleX, \
          b:m.skew1, c:m.skew0, d:m.scaleY, tx:m.translateX, ty:m.translateY})
      else
        @_m_lwf_matrix = m
      return

    transform:(ctx) ->
      context = ctx ? cc.renderContext
      m = @_m_lwf_matrix
      context.transform(m.scaleX, m.skew1, m.skew0, m.scaleY,
        m.translateX, m.translateY - cc.canvas.height)
      return
  )

  cc.LWFBitmap.lwfBitmapWithTexture = (texture, rect) ->
    pLWFBitmap = new cc.LWFBitmap()
    if pLWFBitmap.initWithTexture(texture, rect)
      return pLWFBitmap
    else
      return null

  cc.LWFLabel = cc.LabelTTF.extend(
    _m_lwf_matrix:null

    ctor: ->
      cc.associateWithNative(@, cc.LabelTTF)

    initWithString:() -> # Multi arguments
      if typeof @_super isnt "undefined"
        result = @_super(arguments[0])
      else
        a = arguments[0]
        result = @initWithStringFontNameFontSize(a[0], a[1], a[2])
      @setAnchorPoint(cc.p(0, 1))
      return result

    setMatrix:(m) ->
      if typeof @_super isnt "undefined"
        @_super({a:m.scaleX, \
          b:m.skew1, c:m.skew0, d:m.scaleY, tx:m.translateX, ty:m.translateY})
      else
        @_m_lwf_matrix = m
      return

    transform:(ctx) ->
      context = ctx ? cc.renderContext
      m = @_m_lwf_matrix
      context.transform(m.scaleX, m.skew1, m.skew0, m.scaleY,
        m.translateX, m.translateY - cc.canvas.height)
      return
  )

  cc.LWFLabel.create = () -> # Multi arguments
    pLWFLabel = new cc.LWFLabel()
    if pLWFLabel.initWithString(arguments)
      return pLWFLabel
    else
      return null

  cc.LWFParticle = cc.ParticleSystem.extend(
    _m_lwf_matrix:null

    ctor: ->
      cc.associateWithNative(@, cc.ParticleSystem)

    setMatrix:(m) ->
      if typeof @_super isnt "undefined"
        @_super({a:m.scaleX, \
          b:m.skew1, c:m.skew0, d:m.scaleY, tx:m.translateX, ty:m.translateY})
      else
        @_m_lwf_matrix = m
      return

    transform:(ctx) ->
      context = ctx ? cc.renderContext
      m = @_m_lwf_matrix
      context.transform(m.scaleX, m.skew1, m.skew0, m.scaleY,
        m.translateX, m.translateY - cc.canvas.height)
      return
  )

  cc.LWFParticle.create = (plist) ->
    pLWFParticle = new cc.LWFParticle()
    if pLWFParticle.initWithFile(plist)
      return pLWFParticle
    else
      return null

  cc.LWFNode = cc.Node.extend(
    _m_lwf:null
    _m_targetedDelegateAdded:false

    ctor: ->
      cc.associateWithNative(@, cc.Node)

    initWithLWF:(lwf) ->
      @init() if typeof @init isnt "undefined"
      lwf.rendererFactory.lwfNode = @
      @_m_lwf = lwf
      @scheduleUpdate()
      return true

    cleanup: ->
      @_super()
      @_m_lwf.destroy()
      return

    getLWF: ->
      return @_m_lwf

    update:(dt) ->
      @_super(dt)
      @_m_lwf.exec(dt)
      @_m_lwf.render()
      return

    ###
    draw:(ctx) ->
      @_super(ctx)
      @_m_lwf.render()
      return
    ###

    onEnter: ->
      if @_m_lwf.interactive
        d = cc.Director.getInstance()
        if typeof d.getTouchDispatcher isnt "undefined"
          d.getTouchDispatcher().addTargetedDelegate(@, 0, true)
          @_m_targetedDelegateAdded = true
      return @_super()

    onExit: ->
      if @_m_targetedDelegateAdded
        cc.Director.getInstance().getTouchDispatcher().removeDelegate(@)
      return @_super()

    onTouchBegan:(touch, event) ->
      @onTouchMoved(touch, event)
      @_m_lwf.inputPress()
      return true

    onTouchMoved:(touch, event) ->
      p = @convertTouchToNodeSpace(touch)
      p = cc.Director.getInstance().convertToGL(p)
      @_m_lwf.inputPoint(p.x, p.y)
      return

    onTouchEnded:(touch, event) ->
      @onTouchMoved(touch, event)
      @_m_lwf.inputRelease()
      return
  )

  cc.LWFNode.lwfNodeWithLWF = (lwf) ->
    pLWFNode = new cc.LWFNode()
    if pLWFNode.initWithLWF(lwf)
      return pLWFNode
    else
      return null

