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
    return

  add:(handlers) ->
    for type in @types
      handler = handlers[type]
      @[type].push(handler) if handler?
    return

  removeHandler:(array, handler) ->
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
      @removeHandler(@[type], handler) if handler?
    return

  call:(type, target) ->
    handlers = @[type]
    if handlers?
      handler.call(target) for handler in handlers
    return

