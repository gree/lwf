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
  global["LWF"]["WebGLRendererFactory"] = WebGLRendererFactory
  global["LWF"]["WebGLResourceCache"] = WebGLResourceCache

  global["LWF"]["useWebGLRenderer"] = ->
    global["LWF"]["ResourceCache"] = WebGLResourceCache
  global["LWF"]["LWF"]["useWebGLRenderer"] = global["LWF"]["useWebGLRenderer"]

WebGLRendererFactory.prototype["convertColor"] =
  WebGLRendererFactory.prototype.convertColor
WebGLRendererFactory.prototype["fitForHeight"] =
  WebGLRendererFactory.prototype.fitForHeight
WebGLRendererFactory.prototype["fitForWidth"] =
  WebGLRendererFactory.prototype.fitForWidth
WebGLRendererFactory.prototype["scaleForHeight"] =
  WebGLRendererFactory.prototype.scaleForHeight
WebGLRendererFactory.prototype["scaleForWidth"] =
  WebGLRendererFactory.prototype.scaleForWidth
WebGLRendererFactory.prototype["setBackgroundColor"] =
  WebGLRendererFactory.prototype.setBackgroundColor

WebGLResourceCache.prototype["clear"] =
  WebGLResourceCache.prototype.clear
WebGLResourceCache.prototype["getCache"] =
  WebGLResourceCache.prototype.getCache
WebGLResourceCache.prototype["loadLWF"] =
  WebGLResourceCache.prototype.loadLWF
WebGLResourceCache.prototype["loadLWFs"] =
  WebGLResourceCache.prototype.loadLWFs
WebGLResourceCache.prototype["unloadLWF"] =
  WebGLResourceCache.prototype.unloadLWF
WebGLResourceCache.prototype["setParticleConstructor"] =
  WebGLResourceCache.prototype.setParticleConstructor
WebGLResourceCache.prototype["setDOMElementConstructor"] =
  WebGLResourceCache.prototype.setDOMElementConstructor
