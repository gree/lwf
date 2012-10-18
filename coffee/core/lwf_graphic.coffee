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

class Graphic extends LObject
  constructor:(lwf, parent, objId) ->
    super(lwf, parent, Format.LObject.Type.GRAPHIC, objId)

    data = lwf.data.graphics[objId]
    n = data.graphicObjects
    @displayList = []

    graphicObjects = lwf.data.graphicObjects
    for i in [0...n]
      gobj = graphicObjects[data.graphicObjectId + i]
      graphicObjectId = gobj.graphicObjectId

      continue if graphicObjectId == -1

      switch gobj.graphicObjectType
        when Format.GraphicObject.Type.BITMAP
          obj = new Bitmap(lwf, parent, graphicObjectId)
        when Format.GraphicObject.Type.BITMAPEX
          obj = new BitmapEx(lwf, parent, graphicObjectId)
        when Format.GraphicObject.Type.TEXT
          obj = new Text(lwf, parent, graphicObjectId)

      @displayList[i] = obj

  update:(m, c) ->
    obj.update(m, c) for obj in @displayList
    return

  render:(v, rOffset) ->
    return unless v
    obj.render(v, rOffset) for obj in @displayList
    return

  destroy: ->
    obj.destroy() for obj in @displayList
    @displayList = null
    return
