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

class Point
  constructor:(@x = 0, @y = 0) ->

class Translate
  constructor:(@translateX = 0, @translateY = 0) ->

class Matrix
  constructor:(@scaleX, @scaleY, @skew0, @skew1, @translateX, @translateY) ->
    @clear() unless @scaleX?

  clear: ->
    @scaleX = 1
    @scaleY = 1
    @skew0 = 0
    @skew1 = 0
    @translateX = 0
    @translateY = 0
    return

  set:(m) ->
    @scaleX = m.scaleX
    @scaleY = m.scaleY
    @skew0 = m.skew0
    @skew1 = m.skew1
    @translateX = m.translateX
    @translateY = m.translateY
    return @

  setWithComparing:(m) ->
    return false if m is null

    scaleX = m.scaleX
    scaleY = m.scaleY
    skew0 = m.skew0
    skew1 = m.skew1
    translateX = m.translateX
    translateY = m.translateY
    changed = false
    if @scaleX isnt scaleX
      @scaleX = scaleX
      changed = true
    if @scaleY isnt scaleY
      @scaleY = scaleY
      changed = true
    if @skew0 isnt skew0
      @skew0 = skew0
      changed = true
    if @skew1 isnt skew1
      @skew1 = skew1
      changed = true
    if @translateX isnt translateX
      @translateX = translateX
      changed = true
    if @translateY isnt translateY
      @translateY = translateY
      changed = true
    return changed

class Color
  constructor:(@red, @green, @blue, @alpha) ->
    unless @red?
      @red = 0
      @green = 0
      @blue = 0
      @alpha = 0

  set:(r, g, b, a) ->
    if typeof r is "object"
      c = r
      @red = c.red
      @green = c.green
      @blue = c.blue
      @alpha = c.alpha
    else
      @red = r
      @green = g
      @blue = b
      @alpha = a
    return

class AlphaTransform
  constructor:(@alpha) ->

class ColorTransform
  constructor:(mr, mg, mb, ma, ar, ag, ab, aa) ->
    @multi = new Color(mr, mg, mb, ma)
    #@add = new Color(ar, ag, ab, aa)
    @clear() unless mr?

  clear: ->
    @multi.set(1, 1, 1, 1)
    #@add.set(0, 0, 0, 0)
    return

  set:(c) ->
    @multi.set(c.multi)
    #@add.set(c.add)
    return @

  setWithComparing:(c) ->
    return false if c is null

    cm = c.multi
    red = cm.red
    green = cm.green
    blue = cm.blue
    alpha = cm.alpha
    changed = false
    m = @multi
    if m.red isnt red
      m.red = red
      changed = true
    if m.green isnt green
      m.green = green
      changed = true
    if m.blue isnt blue
      m.blue = blue
      changed = true
    if m.alpha isnt alpha
      m.alpha = alpha
      changed = true
    return changed

