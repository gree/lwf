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

if typeof global isnt "undefined"
  global["LWF"]["Cocos2dRendererFactory"] = Cocos2dRendererFactory
  global["LWF"]["Cocos2dResourceCache"] = Cocos2dResourceCache
  global["LWF"]["ResourceCache"] = Cocos2dResourceCache

  if typeof cc isnt "undefined"
    cc["LWF"] = global["LWF"]

Cocos2dRendererFactory.prototype["convertColor"] =
  Cocos2dRendererFactory.prototype.convertColor
Cocos2dRendererFactory.prototype["fitForHeight"] =
  Cocos2dRendererFactory.prototype.fitForHeight
Cocos2dRendererFactory.prototype["fitForWidth"] =
  Cocos2dRendererFactory.prototype.fitForWidth
Cocos2dRendererFactory.prototype["scaleForHeight"] =
  Cocos2dRendererFactory.prototype.scaleForWidth
Cocos2dRendererFactory.prototype["scaleForStage"] =
  Cocos2dRendererFactory.prototype.scaleForWidth
Cocos2dRendererFactory.prototype["setBackgroundColor"] =
  Cocos2dRendererFactory.prototype.setBackgroundColor

Cocos2dResourceCache.prototype["clear"] =
  Cocos2dResourceCache.prototype.clear
Cocos2dResourceCache.prototype["getCache"] =
  Cocos2dResourceCache.prototype.getCache
Cocos2dResourceCache.prototype["loadLWF"] =
  Cocos2dResourceCache.prototype.loadLWF
Cocos2dResourceCache.prototype["loadLWFs"] =
  Cocos2dResourceCache.prototype.loadLWFs
Cocos2dResourceCache.prototype["unloadLWF"] =
  Cocos2dResourceCache.prototype.unloadLWF
Cocos2dResourceCache.prototype["setParticleConstructor"] =
  Cocos2dResourceCache.prototype.setParticleConstructor
Cocos2dResourceCache.prototype["setDOMElementConstructor"] =
  Cocos2dResourceCache.prototype.setDOMElementConstructor
