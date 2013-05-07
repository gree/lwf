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

class EventHandlers
  constructor: ->
    @clear()

  clear:(type = null) ->
    if type?
      @[type] = [] if @[type]?
    else
      @[type] = [] for type in @types
    @empty = true
    return

  add:(handlers) ->
    return unless handlers?
    for type in @types
      handler = handlers[type]
      @[type].push(handler) if handler?
    @updateEmpty()
    return

  concat:(handlers) ->
    return unless handlers?
    return if handlers.empty
    for type in @types
      handler = handlers[type]
      @[type].push(h) for h in handler
    @updateEmpty()
    return

  addHandler:(type, handler) ->
    if handler?
      @[type].push(handler)
      @empty = false
    return

  removeInternal:(array, handler) ->
    i = 0
    while i < array.length
      if array[i] is handler
        array.splice(i, 1)
      else
        ++i
    return

  remove:(handlers) ->
    for type in @types
      handler = handlers[type]
      @removeInternal(@[type], handler) if handler?
    @updateEmpty()
    return

  removeHandler:(type, handler) ->
    @removeInternal(@[type], handler) if handler?
    @updateEmpty()
    return

  call:(type, target) ->
    handlers = @[type]
    if handlers? and handlers.length > 0
      handlers = (handler for handler in handlers)
      handler.call(target) for handler in handlers
    return

  updateEmpty: ->
    for type in @types
      if @[type]?.length > 0
        @empty = false
        return
    @empty = true
    return

