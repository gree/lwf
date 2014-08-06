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

class IObject extends LObject
  constructor:(lwf, parent, type, objId, instId) ->
    super(lwf, parent, type, objId)

    @prevInstance = null
    @nextInstance = null
    @linkInstance = null

    @instanceId = if instId >= lwf.data.instanceNames.length then -1 else instId

    if @instanceId >= 0
      stringId = lwf.getInstanceNameStringId(@instanceId)
      if stringId isnt -1
        @name = lwf.data.strings[stringId]
        parent[@name] = @ if parent?

      head = @lwf.getInstance(@instanceId)
      head.prevInstance = @ if head?
      @nextInstance = head
      @lwf.setInstance(@instanceId, @)

  destroy: ->
    if @type isnt Type.ATTACHEDMOVIE and @instanceId >= 0
      head = @lwf.getInstance(@instanceId)
      @lwf.setInstance(@instanceId, @nextInstance) if head is @
      @nextInstance.prevInstance = @prevInstance if @nextInstance?
      @prevInstance.nextInstance = @nextInstance if @prevInstance?

    delete parent[@name] if @name? and parent?

    @prevInstance = null
    @nextInstance = null
    @linkInstance = null

    super()
    return

  linkButton: ->
 
  getFullName: ->
    fullPath = ""
    splitter = ""
    o = @
    while o?
      return null unless o.name?
      if (o.parent? and o.objectId is o.lwf.data.header.rootMovieId)
        name = o.lwf.attachName
      else
        name = o.name
      fullPath = name + splitter + fullPath
      splitter = "."
      o = o.parent
    return fullPath

