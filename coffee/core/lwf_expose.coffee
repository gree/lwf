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
  global["LWF"] ?= {}
  global["LWF"]["Color"] = Color
  global["LWF"]["ColorTransform"] = ColorTransform
  global["LWF"]["Format"] = Format
  global["LWF"]["LWF"] = LWF
  global["LWF"]["Loader"] = Loader
  global["LWF"]["Matrix"] = Matrix
  global["LWF"]["Movie"] = Movie
  global["LWF"]["Point"] = Point
  global["LWF"]["Property"] = Property
  # global["LWF"]["Script"] for per data JavaScript

LWF.prototype["addAllowButton"] = LWF.prototype.addAllowButton
LWF.prototype["addButtonEventHandler"] = LWF.prototype.addButtonEventHandler
LWF.prototype["addButtonEventListener"] = LWF.prototype.addButtonEventHandler
LWF.prototype["addDenyButton"] = LWF.prototype.addDenyButton
LWF.prototype["addEventHandler"] = LWF.prototype.addEventHandler
LWF.prototype["addEventListener"] = LWF.prototype.addEventHandler
LWF.prototype["addMovieEventHandler"] = LWF.prototype.addMovieEventHandler
LWF.prototype["addMovieEventListener"] = LWF.prototype.addMovieEventHandler
LWF.prototype["clearAllowButton"] = LWF.prototype.clearAllowButton
LWF.prototype["clearButtonEventHandler"] = LWF.prototype.clearButtonEventHandler
LWF.prototype["clearButtonEventListener"] =
  LWF.prototype.clearButtonEventHandler
LWF.prototype["clearDenyButton"] = LWF.prototype.clearDenyButton
LWF.prototype["clearEventHandler"] = LWF.prototype.clearEventHandler
LWF.prototype["clearEventListener"] = LWF.prototype.clearEventHandler
LWF.prototype["clearMovieEventHandler"] = LWF.prototype.clearMovieEventHandler
LWF.prototype["clearMovieEventListener"] = LWF.prototype.clearMovieEventHandler
LWF.prototype["denyAllButtons"] = LWF.prototype.denyAllButtons
LWF.prototype["destroy"] = LWF.prototype.destroy
LWF.prototype["dispatchEvent"] = LWF.prototype.dispatchEvent
LWF.prototype["exec"] = LWF.prototype.exec
LWF.prototype["fitForHeight"] = LWF.prototype.fitForHeight
LWF.prototype["fitForWidth"] = LWF.prototype.fitForWidth
LWF.prototype["forceExec"] = LWF.prototype.forceExec
LWF.prototype["forceExecWithoutProgress"] =
  LWF.prototype.forceExecWithoutProgress
LWF.prototype["getStageSize"] = LWF.prototype.getStageSize
LWF.prototype["getStringId"] = LWF.prototype.getStringId
LWF.prototype["init"] = LWF.prototype.init
LWF.prototype["inputKeyPress"] = LWF.prototype.inputKeyPress
LWF.prototype["inputPoint"] = LWF.prototype.inputPoint
LWF.prototype["inputPress"] = LWF.prototype.inputPress
LWF.prototype["inputRelease"] = LWF.prototype.inputRelease
LWF.prototype["inspect"] = LWF.prototype.inspect
LWF.prototype["removeAllowButton"] = LWF.prototype.removeAllowButton
LWF.prototype["removeButtonEventHandler"] =
  LWF.prototype.removeButtonEventHandler
LWF.prototype["removeButtonEventListener"] =
  LWF.prototype.removeButtonEventHandler
LWF.prototype["removeDenyButton"] = LWF.prototype.removeDenyButton
LWF.prototype["removeEventHandler"] = LWF.prototype.removeEventHandler
LWF.prototype["removeEventListener"] = LWF.prototype.removeEventHandler
LWF.prototype["removeMovieEventHandler"] = LWF.prototype.removeMovieEventHandler
LWF.prototype["removeMovieEventListener"] =
  LWF.prototype.removeMovieEventHandler
LWF.prototype["render"] = LWF.prototype.render
LWF.prototype["scaleForHeight"] = LWF.prototype.scaleForHeight
LWF.prototype["scaleForWidth"] = LWF.prototype.scaleForWidth
LWF.prototype["searchAttachedLWF"] = LWF.prototype.searchAttachedLWF
LWF.prototype["searchAttachedMovie"] = LWF.prototype.searchAttachedMovie
LWF.prototype["searchEventId"] = LWF.prototype.searchEventId
LWF.prototype["searchFrame"] = LWF.prototype.searchFrame
LWF.prototype["searchProgramObjectId"] = LWF.prototype.searchProgramObjectId
LWF.prototype["setButtonEventHandler"] = LWF.prototype.setButtonEventHandler
LWF.prototype["setButtonEventListener"] = LWF.prototype.setButtonEventHandler
LWF.prototype["setEventHandler"] = LWF.prototype.setEventHandler
LWF.prototype["setEventListener"] = LWF.prototype.setEventHandler
LWF.prototype["setFrameRate"] = LWF.prototype.setFrameRate
LWF.prototype["setMovieCommand"] = LWF.prototype.setMovieCommand
LWF.prototype["setMovieEventHandler"] = LWF.prototype.setMovieEventHandler
LWF.prototype["setMovieEventListener"] = LWF.prototype.setMovieEventHandler
LWF.prototype["setProgramObjectConstructor"] =
  LWF.prototype.setProgramObjectConstructor
LWF.prototype["setRendererFactory"] = LWF.prototype.setRendererFactory

Loader["load"] = Loader.load

Data.prototype["check"] = Data.prototype.check
Data.prototype["name"] = Data.prototype.name

IObject.prototype["getFullName"] = IObject.prototype.getFullName

Button.prototype["addEventHandler"] = Button.prototype.addEventHandler
Button.prototype["addEventListener"] = Button.prototype.addEventHandler
Button.prototype["clearEventHandler"] = Button.prototype.clearEventHandler
Button.prototype["clearEventListener"] = Button.prototype.clearEventHandler
Button.prototype["dispatchEvent"] = Button.prototype.dispatchEvent
Button.prototype["removeEventHandler"] = Button.prototype.removeEventHandler
Button.prototype["removeEventListener"] = Button.prototype.removeEventHandler
Button.prototype["setEventHandler"] = Button.prototype.setEventHandler
Button.prototype["setEventListener"] = Button.prototype.setEventHandler

Movie.prototype["addEventHandler"] = Movie.prototype.addEventHandler
Movie.prototype["addEventListener"] = Movie.prototype.addEventHandler
Movie.prototype["attachLWF"] = Movie.prototype.attachLWF
Movie.prototype["attachMovie"] = Movie.prototype.attachMovie
Movie.prototype["clearEventHandler"] = Movie.prototype.clearEventHandler
Movie.prototype["clearEventListener"] = Movie.prototype.clearEventHandler
Movie.prototype["detachFromParent"] = Movie.prototype.detachFromParent
Movie.prototype["detachLWF"] = Movie.prototype.detachLWF
Movie.prototype["detachMovie"] = Movie.prototype.detachMovie
Movie.prototype["dispatchEvent"] = Movie.prototype.dispatchEvent
Movie.prototype["getAttachedLWF"] = Movie.prototype.getAttachedLWF
Movie.prototype["getAttachedMovie"] = Movie.prototype.getAttachedMovie
Movie.prototype["globalToLocal"] = Movie.prototype.globalToLocal
Movie.prototype["gotoAndPlay"] = Movie.prototype.gotoAndPlay
Movie.prototype["gotoAndStop"] = Movie.prototype.gotoAndStop
Movie.prototype["gotoFrame"] = Movie.prototype.gotoFrame
Movie.prototype["gotoLabel"] = Movie.prototype.gotoLabel
Movie.prototype["inspect"] = Movie.prototype.inspect
Movie.prototype["localToGlobal"] = Movie.prototype.localToGlobal
Movie.prototype["move"] = Movie.prototype.move
Movie.prototype["moveTo"] = Movie.prototype.moveTo
Movie.prototype["nextFrame"] = Movie.prototype.nextFrame
Movie.prototype["override"] = Movie.prototype.override
Movie.prototype["play"] = Movie.prototype.play
Movie.prototype["prevFrame"] = Movie.prototype.prevFrame
Movie.prototype["removeEventHandler"] = Movie.prototype.removeEventHandler
Movie.prototype["removeEventListener"] = Movie.prototype.removeEventHandler
Movie.prototype["removeMovieClip"] = Movie.prototype.removeMovieClip
Movie.prototype["rotate"] = Movie.prototype.rotate
Movie.prototype["rotateTo"] = Movie.prototype.rotateTo
Movie.prototype["scale"] = Movie.prototype.scale
Movie.prototype["scaleTo"] = Movie.prototype.scaleTo
Movie.prototype["searchAttachedLWF"] = Movie.prototype.searchAttachedLWF
Movie.prototype["searchAttachedMovie"] = Movie.prototype.searchAttachedMovie
Movie.prototype["searchMovieInstance"] = Movie.prototype.searchMovieInstance
Movie.prototype["searchMovieInstanceByInstanceId"] =
  Movie.prototype.searchMovieInstanceByInstanceId
Movie.prototype["setAlpha"] = Movie.prototype.setAlpha
Movie.prototype["setColorTransform"] = Movie.prototype.setColorTransform
Movie.prototype["setEventHandler"] = Movie.prototype.setEventHandler
Movie.prototype["setEventListener"] = Movie.prototype.setEventHandler
Movie.prototype["setMatrix"] = Movie.prototype.setMatrix
Movie.prototype["setRenderingOffset"] = Movie.prototype.setRenderingOffset
Movie.prototype["setVisible"] = Movie.prototype.setVisible
Movie.prototype["stop"] = Movie.prototype.stop
Movie.prototype["swapAttachedLWFDepth"] = Movie.prototype.swapAttachedLWFDepth
Movie.prototype["swapAttachedMovieDepth"] =
  Movie.prototype.swapAttachedMovieDepth
Movie.prototype["swapDepths"] = Movie.prototype.swapDepths

Property.prototype["clear"] = Property.prototype.clear
Property.prototype["move"] = Property.prototype.move
Property.prototype["moveTo"] = Property.prototype.moveTo
Property.prototype["rotate"] = Property.prototype.rotate
Property.prototype["rotateTo"] = Property.prototype.rotateTo
Property.prototype["scale"] = Property.prototype.scale
Property.prototype["scaleTo"] = Property.prototype.scaleTo
Property.prototype["setAlpha"] = Property.prototype.setAlpha
Property.prototype["setColorTransform"] = Property.prototype.setColorTransform
Property.prototype["setMatrix"] = Property.prototype.setMatrix

Matrix.prototype["clear"] = Matrix.prototype.clear
Matrix.prototype["set"] = Matrix.prototype.set

Color.prototype["set"] = Color.prototype.set

ColorTransform.prototype["clear"] = ColorTransform.prototype.clear
ColorTransform.prototype["set"] = ColorTransform.prototype.set
