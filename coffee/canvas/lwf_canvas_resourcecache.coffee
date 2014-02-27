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

class CanvasResourceCache extends WebkitCSSResourceCache
  getRendererName: ->
    return "Canvas"

  newFactory:(settings, cache, data) ->
    return new CanvasRendererFactory(data, @, cache, settings.stage,
    settings["textInSubpixel"] ? false, settings["needsClear"] ? true,
    settings["quirkyClearRect"] ? false)

  generateImages:(settings, imageCache, texture, image) ->
    m = texture.filename.match(/_withpadding/)
    if m?
      w = image.width + 2
      h = image.height + 2
      canvas = document.createElement('canvas')
      canvas.width = w
      canvas.height = h
      canvas.name = @getCanvasName()
      ctx = canvas.getContext('2d')
      canvas.withPadding = true
      ctx.drawImage(image,
        0, 0, image.width, image.height,
        1, 1, image.width, image.height)
      imageCache[texture.filename] = canvas

    super(settings, imageCache, texture, image)
    return
