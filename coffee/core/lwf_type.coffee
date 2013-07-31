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

class TypedArray
  @available: typeof Float32Array isnt 'undefined'

class Point
  constructor:(@x = 0, @y = 0) ->

class Translate
  constructor:(translateX = 0, translateY = 0) ->
    @_ = if TypedArray.available then new Float32Array(2) else []
    @_[0] = translateX
    @_[1] = translateY

  getTranslateX: ->
    return @_[0]

  setTranslateX:(v) ->
    @_[0] = v
    return

  getTranslateY: ->
    return @_[1]

  setTranslateY:(v) ->
    @_[1] = v
    return

if typeof(Translate.prototype.__defineGetter__) isnt "undefined"
  Translate.prototype.__defineGetter__("translateX", -> @getTranslateX())
  Translate.prototype.__defineSetter__("translateX", (v) -> @setTranslateX(v))
  Translate.prototype.__defineGetter__("translateY", -> @getTranslateY())
  Translate.prototype.__defineSetter__("translateY", (v) -> @setTranslateY(v))
else if typeof(Object.defineProperty) isnt "undefined"
  Object.defineProperty(Translate.prototype, "translateX",
    get: -> @getTranslateX()
    set: (v) -> @setTranslateX(v))
  Object.defineProperty(Translate.prototype, "translateY",
    get: -> @getTranslateY()
    set: (v) -> @setTranslateY(v))

class Matrix
  constructor:(scaleX, scaleY, skew0, skew1, translateX, translateY) ->
    @_ = if TypedArray.available then new Float32Array(6) else []
    if scaleX?
      @_[0] = scaleX
      @_[1] = skew1
      @_[2] = skew0
      @_[3] = scaleY
      @_[4] = translateX
      @_[5] = translateY
    else
      @clear()

  clear: ->
    @_[0] = 1 # scaleX
    @_[1] = 0 # skew1
    @_[2] = 0 # skew0
    @_[3] = 1 # scaleY
    @_[4] = 0 # translateX
    @_[5] = 0 # translateX
    return

  set:(m) ->
    for i in [0...6]
      @_[i] = m._[i]
    return @

  setWithComparing:(m) ->
    return false if m is null

    changed = false
    for i in [0...6]
      if @_[i] isnt m._[i]
        @_[i] = m._[i]
        changed = true
    return changed

  getScaleX: ->
    return @_[0]

  setScaleX:(v) ->
    @_[0] = v
    return

  getSkew1: ->
    return @_[1]

  setSkew1:(v) ->
    @_[1] = v
    return

  getSkew0: ->
    return @_[2]

  setSkew0:(v) ->
    @_[2] = v
    return

  getScaleY: ->
    return @_[3]

  setScaleY:(v) ->
    @_[3] = v
    return

  getTranslateX: ->
    return @_[4]

  setTranslateX:(v) ->
    @_[4] = v
    return

  getTranslateY: ->
    return @_[5]

  setTranslateY:(v) ->
    @_[5] = v
    return

if typeof(Matrix.prototype.__defineGetter__) isnt "undefined"
  Matrix.prototype.__defineGetter__("scaleX", -> @getScaleX())
  Matrix.prototype.__defineSetter__("scaleX", (v) -> @setScaleX(v))
  Matrix.prototype.__defineGetter__("scaleY", -> @getScaleY())
  Matrix.prototype.__defineSetter__("scaleY", (v) -> @setScaleY(v))
  Matrix.prototype.__defineGetter__("skew0", -> @getSkew0())
  Matrix.prototype.__defineSetter__("skew0", (v) -> @setSkew0(v))
  Matrix.prototype.__defineGetter__("skew1", -> @getSkew1())
  Matrix.prototype.__defineSetter__("skew1", (v) -> @setSkew1(v))
  Matrix.prototype.__defineGetter__("translateX", -> @getTranslateX())
  Matrix.prototype.__defineSetter__("translateX", (v) -> @setTranslateX(v))
  Matrix.prototype.__defineGetter__("translateY", -> @getTranslateY())
  Matrix.prototype.__defineSetter__("translateY", (v) -> @setTranslateY(v))
else if typeof(Object.defineProperty) isnt "undefined"
  Object.defineProperty(Matrix.prototype, "scaleX",
    get: -> @getScaleX()
    set: (v) -> @setScaleX(v))
  Object.defineProperty(Matrix.prototype, "scaleY",
    get: -> @getScaleY()
    set: (v) -> @setScaleY(v))
  Object.defineProperty(Matrix.prototype, "skew0",
    get: -> @getSkew0()
    set: (v) -> @setSkew0(v))
  Object.defineProperty(Matrix.prototype, "skew1",
    get: -> @getSkew1()
    set: (v) -> @setSkew1(v))
  Object.defineProperty(Matrix.prototype, "translateX",
    get: -> @getTranslateX()
    set: (v) -> @setTranslateX(v))
  Object.defineProperty(Matrix.prototype, "translateY",
    get: -> @getTranslateY()
    set: (v) -> @setTranslateY(v))

class Color
  constructor:(red, green, blue, alpha) ->
    @_ = if TypedArray.available then new Float32Array(4) else []
    if red?
      @_[0] = red
      @_[1] = green
      @_[2] = blue
      @_[3] = alpha
    else
      @_[0] = 0
      @_[1] = 0
      @_[2] = 0
      @_[3] = 0

  set:(r, g, b, a) ->
    if typeof r is "object"
      c = r
      for i in [0...4]
        @_[i] = c._[i]
    else
      @_[0] = r
      @_[1] = g
      @_[2] = b
      @_[3] = a
    return

  getRed: ->
    return @_[0]

  setRed:(v) ->
    @_[0] = v
    return

  getGreen: ->
    return @_[1]

  setGreen:(v) ->
    @_[1] = v
    return

  getBlue: ->
    return @_[2]

  setBlue:(v) ->
    @_[2] = v
    return

  getAlpha: ->
    return @_[3]

  setAlpha:(v) ->
    @_[3] = v
    return

if typeof(Color.prototype.__defineGetter__) isnt "undefined"
  Color.prototype.__defineGetter__("red", -> @getRed())
  Color.prototype.__defineSetter__("red", (v) -> @setRed(v))
  Color.prototype.__defineGetter__("green", -> @getGreen())
  Color.prototype.__defineSetter__("green", (v) -> @setGreen(v))
  Color.prototype.__defineGetter__("blue", -> @getBlue())
  Color.prototype.__defineSetter__("blue", (v) -> @setBlue(v))
  Color.prototype.__defineGetter__("alpha", -> @getAlpha())
  Color.prototype.__defineSetter__("alpha", (v) -> @setAlpha(v))
else if typeof(Object.defineProperty) isnt "undefined"
  Object.defineProperty(Color.prototype, "red",
    get: -> @getRed()
    set: (v) -> @setRed(v))
  Object.defineProperty(Color.prototype, "green",
    get: -> @getGreen()
    set: (v) -> @setGreen(v))
  Object.defineProperty(Color.prototype, "blue",
    get: -> @getBlue()
    set: (v) -> @setBlue(v))
  Object.defineProperty(Color.prototype, "alpha",
    get: -> @getAlpha()
    set: (v) -> @setAlpha(v))

class AlphaTransform
  constructor:(alpha) ->
    @_ = if TypedArray.available then new Float32Array(1) else []
    @_[0] = alpha

  getAlpha: ->
    return @_[0]

  setAlpha:(v) ->
    @_[0] = v
    return

if typeof(AlphaTransform.prototype.__defineGetter__) isnt "undefined"
  AlphaTransform.prototype.__defineGetter__("alpha", -> @getAlpha())
  AlphaTransform.prototype.__defineSetter__("alpha", (v) -> @setAlpha(v))
else if typeof(Object.defineProperty) isnt "undefined"
  Object.defineProperty(AlphaTransform.prototype, "alpha",
    get: -> @getAlpha()
    set: (v) -> @setAlpha(v))

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

    changed = false
    cm = c.multi
    m = @multi
    for i in [0...4]
      if m._[i] isnt cm._[i]
        m._[i] = cm._[i]
        changed = true
    return changed

