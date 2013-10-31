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

class Cocos2dParticleContext
  constructor:(@factory, @data, particle) ->
    particleData = data.particleDatas[particle.particleDataId]
    @name = data.strings[particleData.stringId]
    @plist = @factory.resourceCache.particlePrefix +
      @name + @factory.resourceCache.particleSuffix

  destruct: ->

class Cocos2dParticleRenderer
  constructor:(@context) ->
    @matrix = new Matrix()
    @particle = cc.LWFParticle.create(@context.plist)
    @visible = true
    @alpha = 1
    @z = -1

  destruct: ->
    @particle.removeFromParent(true)

  update:(m, c) ->

  render:(m, c, renderingIndex, renderingCount, visible) ->
    z = renderingIndex
    if @z isnt z
      if @z is -1
        @context.factory.lwfNode.addChild(@particle, z)
      else
        @particle.getParent().reorderChild(@particle, z)
      @z = z

    if @matrix.setWithComparing(m)
      @particle.setMatrix(@matrix)

    if @alpha isnt c.multi.alpha or @visible isnt visible
      @alpha = c.multi.alpha
      @visible = visible
      @particle.setOpacity(if @visible then @alpha * 255 else 0)

