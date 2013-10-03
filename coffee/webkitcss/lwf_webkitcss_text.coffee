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

class WebkitCSSTextContext extends HTML5TextContext

class WebkitCSSTextRenderer extends HTML5TextRenderer
  constructor:(@lwf, @context, @textObject) ->
    super

    @alpha = -1
    @zIndex = -1
    @visible = false
    @cmd = {}

  destructor: ->
    @node.parentNode.removeChild(@node) if @node?
    return

  destruct: ->
    @context.factory.destructRenderer(@)
    return

  render:(m, c, renderingIndex, renderingCount, visible) ->
    if @visible is visible
      return if visible is false
    else
      @visible = visible
      if visible is false
        @node.style.visibility = "hidden" if @node?
        return
      else
        @node.style.visibility = "visible" if @node?

    super

    nodeChanged = false
    if @node? and @node isnt @canvas
      @node.parentNode.removeChild(@node)
      @node = null
    unless @node?
      nodeChanged = true
      @node = @canvas
      @node.style.display = "block"
      @node.style.pointerEvents = "none"
      @node.style.position = "absolute"
      @node.style.webkitTransformOrigin = "0px 0px"
      @node.style.visibility = "visible"
      @context.factory.stage.appendChild(@node)

    maskMode = @context.factory.maskMode

    return if !nodeChanged and !@matrixChanged and
      @alpha is c.multi.alpha and
      @zIndex is renderingIndex and
      maskMode is "normal" and
      @node.parentNode is @context.factory.stage

    @alpha = c.multi.alpha
    @zIndex = renderingIndex

    cmd = @cmd
    cmd.isBitmap = false
    cmd.renderer = @
    cmd.matrix = @matrix
    cmd.maskMode = maskMode
    @context.factory.addCommand(renderingIndex, cmd)
    return

