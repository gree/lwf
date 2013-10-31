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

  affineTransformMake = (m) ->
    if cc.renderContextType is cc.WEBGL
      skew0 = m.skew0
      skew1 = m.skew1
    else
      skew0 = m.skew1
      skew1 = m.skew0
    return cc.AffineTransformMake(
      m.scaleX, -skew1, skew0, -m.scaleY, m.translateX,
        cc.Director.getInstance().getWinSizeInPixels().height - m.translateY)

  cc.LWFBitmap = cc.Sprite.extend(
    initWithTexture:(texture, rect) ->
      result = @_super(texture, rect)
      @setFlippedY(true)
      return result

    setMatrix:(m) ->
      @_transform = affineTransformMake(m)
      @setNodeDirty(true)
      return

    nodeToParentTransform: ->
      return @_transform
  )

  cc.LWFBitmap.lwfBitmapWithTexture = (texture, rect) ->
    pLWFBitmap = new cc.LWFBitmap()
    if pLWFBitmap.initWithTexture(texture, rect)
      return pLWFBitmap
    else
      return null

  cc.LWFLabel = cc.LabelTTF.extend(
    initWithString:(label, fontName, fontSize, dimensions, hA, vA) ->
      result = @_super(label, fontName, fontSize, dimensions, hA, vA)
      @setFlippedY(true)
      return result

    setMatrix:(m) ->
      @_transform = affineTransformMake(m)
      @setNodeDirty(true)
      return

    nodeToParentTransform: ->
      return @_transform
  )

  cc.LWFLabel.create = (label, fontName, fontSize, dimensions, hA, vA) ->
    pLWFLabel = new cc.LWFLabel()
    if pLWFLabel.initWithString(label, fontName, fontSize, dimensions, hA, vA)
      return pLWFLabel
    else
      return null

  cc.LWFParticle = cc.ParticleSystem.extend(
    setMatrix:(m) ->
      @_transform = affineTransformMake(m)
      @setNodeDirty(true)
      return

    nodeToParentTransform: ->
      return @_transform
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

    initWithLWF:(lwf) ->
      @init()
      lwf.rendererFactory.lwfNode = @
      @_m_lwf = lwf
      @scheduleUpdate()
      return true

    cleanup: ->
      @_m_lwf.destroy()
      @_super()
      return

    getLWF: ->
      return @_m_lwf

    update:(dt) ->
      @_super(dt)
      @_m_lwf.exec(dt)
      @_m_lwf.render()
      return

    onEnter: ->
      if @_m_lwf.interactive
        cc.registerTargetedDelegate(0, true, @)
        @_m_targetedDelegateAdded = true
      @_super()
      return

    onExit: ->
      if @_m_targetedDelegateAdded
        cc.unregisterTouchDelegate(@)
      @_super()
      return

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

