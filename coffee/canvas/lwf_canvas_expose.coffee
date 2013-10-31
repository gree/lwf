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
  global["LWF"]["CanvasRendererFactory"] = CanvasRendererFactory
  global["LWF"]["CanvasResourceCache"] = CanvasResourceCache

  global["LWF"]["useCanvasRenderer"] = ->
    global["LWF"]["ResourceCache"] = CanvasResourceCache
  global["LWF"]["LWF"]["useCanvasRenderer"] = global["LWF"]["useCanvasRenderer"]

CanvasRendererFactory.prototype["convertColor"] =
  CanvasRendererFactory.prototype.convertColor
CanvasRendererFactory.prototype["fitForHeight"] =
  CanvasRendererFactory.prototype.fitForHeight
CanvasRendererFactory.prototype["fitForWidth"] =
  CanvasRendererFactory.prototype.fitForWidth
CanvasRendererFactory.prototype["scaleForHeight"] =
  CanvasRendererFactory.prototype.scaleForHeight
CanvasRendererFactory.prototype["scaleForWidth"] =
  CanvasRendererFactory.prototype.scaleForWidth
CanvasRendererFactory.prototype["setBackgroundColor"] =
  CanvasRendererFactory.prototype.setBackgroundColor

CanvasResourceCache.prototype["clear"] =
  CanvasResourceCache.prototype.clear
CanvasResourceCache.prototype["getCache"] =
  CanvasResourceCache.prototype.getCache
CanvasResourceCache.prototype["getRendererName"] =
  CanvasResourceCache.prototype.getRendererName
CanvasResourceCache.prototype["loadLWF"] =
  CanvasResourceCache.prototype.loadLWF
CanvasResourceCache.prototype["loadLWFs"] =
  CanvasResourceCache.prototype.loadLWFs
CanvasResourceCache.prototype["unloadLWF"] =
  CanvasResourceCache.prototype.unloadLWF
CanvasResourceCache.prototype["setParticleConstructor"] =
  CanvasResourceCache.prototype.setParticleConstructor
CanvasResourceCache.prototype["setDOMElementConstructor"] =
  CanvasResourceCache.prototype.setDOMElementConstructor
