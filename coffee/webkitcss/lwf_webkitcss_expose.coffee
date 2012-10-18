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
  global["LWF"]["WebkitCSSRendererFactory"] = WebkitCSSRendererFactory
  global["LWF"]["WebkitCSSResourceCache"] = WebkitCSSResourceCache

  global["LWF"]["useWebkitCSSRenderer"] = ->
    global["LWF"]["ResourceCache"] = WebkitCSSResourceCache

WebkitCSSRendererFactory.prototype["convertColor"] =
  WebkitCSSRendererFactory.prototype.convertColor
WebkitCSSRendererFactory.prototype["fitForHeight"] =
  WebkitCSSRendererFactory.prototype.fitForHeight
WebkitCSSRendererFactory.prototype["fitForWidth"] =
  WebkitCSSRendererFactory.prototype.fitForWidth
WebkitCSSRendererFactory.prototype["scaleForHeight"] =
  WebkitCSSRendererFactory.prototype.scaleForHeight
WebkitCSSRendererFactory.prototype["scaleForWidth"] =
  WebkitCSSRendererFactory.prototype.scaleForWidth
WebkitCSSRendererFactory.prototype["setBackgroundColor"] =
  WebkitCSSRendererFactory.prototype.setBackgroundColor

WebkitCSSResourceCache.prototype["clear"] =
  WebkitCSSResourceCache.prototype.clear
WebkitCSSResourceCache.prototype["getCache"] =
  WebkitCSSResourceCache.prototype.getCache
WebkitCSSResourceCache.prototype["loadLWF"] =
  WebkitCSSResourceCache.prototype.loadLWF
WebkitCSSResourceCache.prototype["loadLWFs"] =
  WebkitCSSResourceCache.prototype.loadLWFs
WebkitCSSResourceCache.prototype["unloadLWF"] =
  WebkitCSSResourceCache.prototype.unloadLWF
WebkitCSSResourceCache.prototype["setParticleConstructor"] =
  WebkitCSSResourceCache.prototype.setParticleConstructor
WebkitCSSResourceCache.prototype["setDOMElementConstructor"] =
  WebkitCSSResourceCache.prototype.setDOMElementConstructor
