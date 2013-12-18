#!/usr/bin/env ruby
# -*- coding: utf-8 -*-
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
if RUBY_VERSION < "1.9.3"
  puts "ERROR: Requires Ruby 1.9.3 or later"
  exit
end
libdir = File.dirname(__FILE__) + '/lib'
$:.unshift libdir
require 'yaml'
require 'zlib'
require 'kconv'
require 'uri'
require 'pp'
require 'optparse'
require 'fileutils'
require 'find'
require 'htmlparser'
require 'chunky_png'
require 'json'
require 'rkelly'
require 'zip/zip'
ACTIONCOMPILER_VERSION = "1.0.0"
begin
  require 'actioncompiler'
rescue LoadError
  puts "ERROR: Cannot load 'actioncompiler' extension. Please 'gem install gems/actioncompiler*.gem'."
  exit
end
begin
  require 'rubygems'
  require 'libxml'
rescue LoadError
  require 'rexml/document'
end
begin
  require 'rb-img'
rescue LoadError
  puts "ERROR: Cannot load 'rb-img' extension. Please 'gem install gems/rb-img*.gem'."
  exit
end

LWF_HEADER_SIZE = 324

LWF_FORMAT_VERSION_0 = 0x13
LWF_FORMAT_VERSION_1 = 0x12
LWF_FORMAT_VERSION_2 = 0x11
LWF_FORMAT_VERSION = (LWF_FORMAT_VERSION_0 << 16) +
  (LWF_FORMAT_VERSION_1 << 8) + LWF_FORMAT_VERSION_2

LWF_FORMAT_VERSION_COMPAT_0 = 0x12
LWF_FORMAT_VERSION_COMPAT_1 = 0x10
LWF_FORMAT_VERSION_COMPAT_2 = 0x10
LWF_FORMAT_VERSION_COMPAT = (LWF_FORMAT_VERSION_COMPAT_0 << 16) +
  (LWF_FORMAT_VERSION_COMPAT_1 << 8) + LWF_FORMAT_VERSION_COMPAT_2

MATRIX_FLAG = (1 << 31)
COLORTRANSFORM_FLAG = (1 << 31)

OPTION_USE_SCRIPT = (1 << 0)
OPTION_USE_TEXTUREATLAS = (1 << 1)

TEXTUREFORMAT_NORMAL = 0
TEXTUREFORMAT_PREMULTIPLIEDALPHA = 1

LOAD           = (1 << 0)
UNLOAD         = (1 << 1)
ENTERFRAME     = (1 << 2)

ROLLOVER       = (1 << 0)
ROLLOUT        = (1 << 1)
PRESS          = (1 << 2)
RELEASE        = (1 << 3)
KEYPRESS       = (1 << 7)

BLEND_NORMAL = 0
BLEND_ADD = 1
BLEND_LAYER = 2
BLEND_ERASE = 3
BLEND_MASK = 4

BLEND_MODE = {
   0 => {:type => "normal",     :supported => true, :value => BLEND_NORMAL},
   1 => {:type => "normal",     :supported => true, :value => BLEND_NORMAL},
   2 => {:type => "layer",      :supported => true, :value => BLEND_LAYER},
   3 => {:type => "multiply",   :supported => false},
   4 => {:type => "screen",     :supported => false},
   5 => {:type => "lighten",    :supported => false},
   6 => {:type => "darken",     :supported => false},
   7 => {:type => "difference", :supported => false},
   8 => {:type => "add",        :supported => true, :value => BLEND_ADD},
   9 => {:type => "subtract",   :supported => false},
  10 => {:type => "invert",     :supported => false},
  11 => {:type => "alpha",      :supported => true, :value => BLEND_MASK},
  12 => {:type => "erase",      :supported => true, :value => BLEND_ERASE},
  13 => {:type => "overlay",    :supported => false},
  14 => {:type => "hardlight",  :supported => false},
}

class Colormap
  attr_accessor :r, :g, :b, :a
  def initialize(r, g, b, a)
    @r = r
    @g = g
    @b = b
    @a = a
  end
end

class LosslessData
  def initialize(bitmap_type,
      width, height, table_size, has_alpha, startpos, endpos)
    @bitmap_type = bitmap_type
    @width = width
    @height = height
    @table_size = table_size
    @has_alpha = has_alpha
    @startpos = startpos
    @endpos = endpos
  end

  def export_png(swf, filename)
    data = ""
    element_size = @has_alpha ? 4 : 3
    zstream = Zlib::Inflate.new()

    case @bitmap_type
    when 3
      width = (@width + 3) / 4 * 4
      bpl = width
      depth = 1

      color_table_size = @table_size * element_size
      color_table = ""
      while color_table_size > color_table.size
        data = zstream.inflate(swf[@startpos].chr)
        @startpos += 1
        size = [color_table_size - color_table.size, data.size].min
        color_table += data[0, size]
        data = data[size, data.size - size]
      end

      colormap = []
      @table_size.times do |i|
        c = Colormap.new(
          color_table[i * element_size + 0].ord,
          color_table[i * element_size + 1].ord,
          color_table[i * element_size + 2].ord,
          @has_alpha ? color_table[i * element_size + 3].ord : 255)
        colormap.push c
      end

    when 4
      colormap = nil
      depth = 2
      width = (@width + 1) / 2 * 2
      bpl = width * depth

    when 5
      colormap = nil
      depth = 4
      width = @width
      bpl = width * depth
    end

    data_size = depth * width * @height
    while data_size != data.size
      data += zstream.inflate(swf[@startpos, @endpos - @startpos])
      if zstream.finished?
        data += zstream.finish
        break
      end
    end
    zstream.close

    unless defined? Img
      pixels = []
      case @bitmap_type
      when 3
        @height.times do |y|
          @width.times do |x|
            data_index = y * width + x
            c = colormap[data[data_index].ord]
            pixels.push ChunkyPNG::Color.rgba(c.r, c.g, c.b, c.a)
          end
        end
      when 4
        @height.times do |y|
          @width.times do |x|
            data_index = y * width + x
            d0 = data[data_index].ord
            d1 = data[data_index + 1].ord
            pixels.push ChunkyPNG::Color.rgba(
              (d0 & 0x78) * 255 / 0x78,
              (((d0 & 0x03) << 4) | ((d1 & 0xc0) >> 4)) * 255 / 0x3f,
              (d1 & 0x1e) * 255 / 0x1e,
              (d1 & 1) ? 255 : 0)
          end
        end
      when 5
        (@width * @height).times do |i|
          data_index = i * 4
          pixels.push ChunkyPNG::Color.rgba(
            data[data_index + 1].ord,
            data[data_index + 2].ord,
            data[data_index + 3].ord,
            data[data_index + 0].ord)
        end
      end
      ChunkyPNG::Image.new(@width, @height, pixels).save(filename)
    else
      fmt = 0
      bytes = nil
      case @bitmap_type
      when 3
        fmt = Img::RGBA8888
        bytes = []
        @height.times do |y|
          @width.times do |x|
            data_index = y * width + x
            c = colormap[data[data_index].ord]
            bytes.push(c.r)
            bytes.push(c.g)
            bytes.push(c.b)
            bytes.push(c.a)
          end
        end
        bytes = bytes.pack('C*')
      when 4
        fmt = Img::RGBA5551
        bytes = data
      when 5
        fmt = Img::ARGB8888
        bytes = data
      end
      img = Img::save(filename, @width, @height, fmt, bytes)
    end
  end
end

def to_color(href, name, str)
  if str =~ /^([0-9a-f][0-9a-f])([0-9a-f][0-9a-f])([0-9a-f][0-9a-f])$/i
    color = Color.new
    color.red = $1.to_i(16) / 255.0
    color.green = $2.to_i(16) / 255.0
    color.blue = $3.to_i(16) / 255.0
    color
  else
    error "INVALID #{name}: #{str} in #{href}"
    nil
  end
end

def to_num(href, name, str, minusable = true)
  regexp = minusable ? /^[\d-]+$/ : /^\d+$/
  if str =~ regexp
    str.to_i
  else
    error "INVALID #{name}: #{str} in #{href}"
    0
  end
end

def to_float(href, name, str)
  regexp = /^[\.\d-]+$/
  if str =~ regexp
    str.to_f
  else
    error "INVALID #{name}: #{str} in #{href}"
    0
  end
end

def to_u8(v)
  error "to_u8 range" if v < 0 or v > 255
  [v].pack('C')
end

def to_s32(v)
  [v].pack('V')
end

def to_u32(v)
  [v].pack('V')
end

def swf2lwf_setfuncs
  if @use_fixed_point

    def to_float32(v)
      [(v * (1 << 12) + (v > 0.0 ? 0.5 : -0.5)).to_i].pack('V')
    end

  else

    def to_float32(v)
      [v].pack('e')
    end

  end
end

def escape(str)
  return str if str.nil?
  str = URI.encode(str)
  str = str.gsub(/(%[\da-fA-F][\da-fA-F])+/) {|m| '__' + m.gsub(/%/, '') + '__'}
  return str
end

def compile_as(script, funcname)
  src = @lwfpath + ".as"
  dst = @lwfpath + ".asbin"

  re = Regexp.new("(?<t>tellTarget\s*\([^\(]+\)\s*)(?<p>\{(?:[^{}]|\g<p>)*\})/")
  as = script.gsub(/fscommand(\s*\(\s*")/, 'geturl1\1FSCommand:').gsub(re, '\k<t>\k<p>;') + "\n"

  info funcname
  info "==========================================================="
  info script
  info "-----------------------------------------------------------"
  info as
  info "-----------------------------------------------------------"

  begin
    raise if ActionCompiler::version() != ACTIONCOMPILER_VERSION
    @swf = ActionCompiler::compile(as)
  rescue
    unless @actioncompiler_error
      @actioncompiler_error = true
      error(<<EOL)
can't load actioncompiler module.
Please install actioncompiler gem like the following.
--
cd lwf/tools/libming
gem build actioncompiler.gemspec
gem install actioncompiler-#{ACTIONCOMPILER_VERSION}.gem

EOL
    end
    @swf = ""
  end
  @swf += [].pack('x')
  @swf.force_encoding("ASCII-8BIT")
  @pos = 0
  info "  size: #{@swf.size}"

  actions = parse_action
  info actions.to_s
  return actions
end

class LWFData
  def to_a
    instance_variables.map{|v| eval v.to_s}
  end
end

class LWFTranslate < LWFData
  attr_reader :translateX, :translateY
  def initialize(matrix)
    @translateX = matrix.translate_x
    @translateY = matrix.translate_y
  end
  def to_bytes
    to_float32(@translateX) +
    to_float32(@translateY)
  end
end

class LWFMatrix < LWFData
  attr_reader :scaleX, :scaleY, :skew0, :skew1, :translateX, :translateY
  def initialize(matrix)
    @scaleX = matrix.scale_x
    @scaleY = matrix.scale_y
    @skew0 = matrix.rotate_skew0
    @skew1 = matrix.rotate_skew1
    @translateX = matrix.translate_x
    @translateY = matrix.translate_y
  end
  def to_bytes
    to_float32(@scaleX) +
    to_float32(@scaleY) +
    to_float32(@skew0) +
    to_float32(@skew1) +
    to_float32(@translateX) +
    to_float32(@translateY)
  end
end

class LWFColor < LWFData
  def initialize(color)
    @red = color.red
    @green = color.green
    @blue = color.blue
    @alpha = color.alpha
  end
  def to_bytes
    to_float32(@red) +
    to_float32(@green) +
    to_float32(@blue) +
    to_float32(@alpha)
  end
end

class LWFAlphaTransform < LWFData
  def initialize(colorTransform)
    @alpha = colorTransform.multi.alpha
  end
  def to_bytes
    to_float32(@alpha)
  end
end

class LWFColorTransform < LWFData
  def initialize(colorTransform)
    @multi_red = colorTransform.multi.red
    @multi_green = colorTransform.multi.green
    @multi_blue = colorTransform.multi.blue
    @multi_alpha = colorTransform.multi.alpha
    @add_red = colorTransform.add.red
    @add_green = colorTransform.add.green
    @add_blue = colorTransform.add.blue
    @add_alpha = colorTransform.add.alpha
  end
  def to_bytes
    to_float32(@multi_red) +
    to_float32(@multi_green) +
    to_float32(@multi_blue) +
    to_float32(@multi_alpha) +
    to_float32(@add_red) +
    to_float32(@add_green) +
    to_float32(@add_blue) +
    to_float32(@add_alpha)
  end
end

class LWFAction
  def initialize(actionOffset, actionLength)
    @actionOffset = actionOffset
    @actionLength = actionLength
  end
  def to_bytes
    to_u32(@actionOffset) +
    to_u32(@actionLength)
  end
end

LWF_OBJECT_BUTTON = 0
LWF_OBJECT_GRAPHIC = 1
LWF_OBJECT_MOVIE = 2
LWF_OBJECT_BITMAP = 3
LWF_OBJECT_BITMAPEX = 4
LWF_OBJECT_TEXT = 5
LWF_OBJECT_PARTICLE = 6
LWF_OBJECT_PROGRAMOBJECT = 7

class LWFObject < LWFData
  def initialize(objectType, objectId)
    @objectType = objectType
    @objectId = objectId
  end
  def to_bytes
    to_u32(@objectType) +
    to_u32(@objectId)
  end
end

class LWFButtonCondition
  def initialize(condition, keycode, actionId)
    @condition = condition
    @keycode = keycode
    @actionId = actionId
  end
  def to_bytes
    to_u32(@condition) +
    to_u32(@keycode) +
    to_u32(@actionId)
  end
end

class LWFButton < LWFData
  def initialize(width, height,
      matrixId, colorTransformId, conditionId, conditions)
    @width = width
    @height = height
    @matrixId = matrixId
    @colorTransformId = colorTransformId
    @conditionId = conditionId
    @conditions = conditions
  end
  def to_bytes
    to_u32(@width) +
    to_u32(@height) +
    to_u32(@matrixId) +
    to_u32(@colorTransformId) +
    to_u32(@conditionId) +
    to_u32(@conditions)
  end
end

class LWFTexture
  attr_reader :name, :width, :height
  def initialize(name, stringId, format, width, height, scale)
    @name = name
    @stringId = stringId
    @format = format
    @width = width
    @height = height
    @scale = scale
  end
  def to_bytes
    to_u32(@stringId) +
    to_u32(@format) +
    to_u32(@width) +
    to_u32(@height) +
    to_float32(@scale)
  end
end

class LWFTextureFragment
  attr_reader :texture,
    :stringId, :textureId, :x, :y, :u, :v, :w, :h, :textureAtlas
  def initialize(texture)
    @texture = texture
  end

  def set(stringId,
      textureId, rotated, x, y, u, v, w, h, textureAtlas = nil)
    @stringId = stringId
    @textureId = textureId
    @rotated = rotated ? 1 : 0
    @x = x
    @y = y
    @u = u
    @v = v
    @w = w
    @h = h
    @textureAtlas = textureAtlas
  end

  def to_bytes
    to_u32(@stringId) +
    to_u32(@textureId) +
    to_u32(@rotated) +
    to_u32(@x) +
    to_u32(@y) +
    to_u32(@u) +
    to_u32(@v) +
    to_u32(@w) +
    to_u32(@h)
  end

  def dump
    "#{textureAtlas.nil? ? "--------" : textureAtlas.name} #{texture.name} " +
      "(x:#{x},y:#{y}) (u:#{u},v:#{v}) (w:#{w},h:#{h})"
  end
end

class LWFBitmap < LWFData
  def initialize(matrixId, textureFragmentId)
    @matrixId = matrixId
    @textureFragmentId = textureFragmentId
  end
  def to_bytes
    to_u32(@matrixId) +
    to_u32(@textureFragmentId)
  end
end

class LWFBitmapEx < LWFData
  attr_reader :textureFragmentId
  attr_accessor :u, :v, :w, :h
  def initialize(matrixId, textureFragmentId, attribute, u, v, w, h)
    @matrixId = matrixId
    @textureFragmentId = textureFragmentId
    @attribute = attribute
    @u = u
    @v = v
    @w = w
    @h = h
  end
  def to_bytes
    to_u32(@matrixId) +
    to_u32(@textureFragmentId) +
    to_u32(@attribute) +
    to_float32(@u) +
    to_float32(@v) +
    to_float32(@w) +
    to_float32(@h)
  end
end

class LWFFont < LWFData
  def initialize(nameStringId, font)
    @nameStringId = nameStringId
    @letterspacing = font.letterspacing
  end
  def to_bytes
    to_u32(@nameStringId) +
    to_float32(@letterspacing)
  end
end

LWF_TEXTPROPERTY_ALIGN_LEFT = 0
LWF_TEXTPROPERTY_ALIGN_RIGHT = 1
LWF_TEXTPROPERTY_ALIGN_CENTER = 2
LWF_TEXTPROPERTY_ALIGN_MASK = 0x3
LWF_TEXTPROPERTY_VALIGN_BOTTOM = (1 << 2)
LWF_TEXTPROPERTY_VALIGN_MIDDLE = (2 << 2)
LWF_TEXTPROPERTY_VALIGN_MASK = 0xc

class LWFTextProperty < LWFData
  def initialize(text, fontId, strokeColorId, shadowColorId)
    @maxLength = text.max_length 
    @fontId = fontId
    @fontHeight = text.font_height
    @align = text.align
    @leftMargin = text.left_margin
    @rightMargin = text.right_margin
    @letterSpacing = text.letter_spacing
    @leading = text.leading
    @strokeColorId = strokeColorId
    @strokeWidth = text.stroke_width
    @shadowColorId = shadowColorId
    @shadowOffsetX = text.shadow_offset_x
    @shadowOffsetY = text.shadow_offset_y
    @shadowBlur = text.shadow_blur
  end
  def to_bytes
    to_u32(@maxLength) +
    to_u32(@fontId) +
    to_u32(@fontHeight) +
    to_u32(@align) +
    to_u32(@leftMargin) +
    to_u32(@rightMargin) +
    to_float32(@letterSpacing) +
    to_s32(@leading) +
    to_s32(@strokeColorId) +
    to_s32(@strokeWidth) +
    to_s32(@shadowColorId) +
    to_s32(@shadowOffsetX) +
    to_s32(@shadowOffsetY) +
    to_s32(@shadowBlur)
  end
end

class LWFText < LWFData
  def initialize(matrixId,
      nameStringId, textPropertyId, stringId, colorId, width, height)
    @matrixId = matrixId
    @nameStringId = nameStringId
    @textPropertyId = textPropertyId
    @stringId = stringId
    @colorId = colorId
    @width = width
    @height = height
  end
  def to_bytes
    to_u32(@matrixId) +
    to_u32(@nameStringId) +
    to_u32(@textPropertyId) +
    to_u32(@stringId) +
    to_u32(@colorId) +
    to_u32(@width) +
    to_u32(@height)
  end
end

class LWFParticleData < LWFData
  def initialize(nameStringId)
    @nameStringId = nameStringId
  end
  def to_bytes
    to_u32(@nameStringId)
  end
end

class LWFParticle < LWFData
  def initialize(matrixId, colorTransformId, particleDataId)
    @matrixId = matrixId
    @colorTransformId = colorTransformId
    @particleDataId = particleDataId
  end
  def to_bytes
    to_u32(@matrixId) +
    to_u32(@colorTransformId) +
    to_u32(@particleDataId)
  end
end

class LWFProgramObject < LWFData
  def initialize(width, height, matrixId, colorTransformId, nameStringId)
    @width = width
    @height = height
    @matrixId = matrixId
    @colorTransformId = colorTransformId
    @nameStringId = nameStringId
  end
  def to_bytes
    to_u32(@nameStringId) +
    to_u32(@width) +
    to_u32(@height) +
    to_u32(@matrixId) +
    to_u32(@colorTransformId)
  end
end

LWF_GRAPHICOBJECT_BITMAP = 0
LWF_GRAPHICOBJECT_BITMAPEX = 1
LWF_GRAPHICOBJECT_TEXT = 2

GRAPHICOBJECT_CONVTABLE = {
	LWF_GRAPHICOBJECT_BITMAP => LWF_OBJECT_BITMAP,
	LWF_GRAPHICOBJECT_BITMAPEX => LWF_OBJECT_BITMAPEX,
	LWF_GRAPHICOBJECT_TEXT => LWF_OBJECT_TEXT,
}

class LWFGraphicObject < LWFData
  def initialize(graphicObjectType, graphicObjectId)
    @graphicObjectType = graphicObjectType
    @graphicObjectId = graphicObjectId
  end
  def to_bytes
    to_u32(@graphicObjectType) +
    to_u32(@graphicObjectId)
  end
end

class LWFGraphic < LWFData
  def initialize(graphicObjectId, graphicObjects)
    @graphicObjectId = graphicObjectId
    @graphicObjects = graphicObjects
  end
  def to_bytes
    to_u32(@graphicObjectId) +
    to_u32(@graphicObjects)
  end
end

LWF_CONTROL_MOVE = 0
LWF_CONTROL_MOVEM = 1
LWF_CONTROL_MOVEC = 2
LWF_CONTROL_MOVEMC = 3
LWF_CONTROL_ACTION = 4

class LWFControlPLACE < LWFData
  def initialize(depth, objectId, instanceId, matrixId, blend_mode)
    @depth = depth
    @objectId = objectId
    @instanceId = instanceId
    @matrixId = matrixId
    @blend_mode = blend_mode
    if @depth < 0 or @depth > 0xffffff
      error("place depth #{@depth} is out of range")
    end
  end
  def to_bytes
    to_u32(@depth + (@blend_mode << 24)) +
    to_u32(@objectId) +
    to_u32(@instanceId) +
    to_u32(@matrixId)
  end
end

class LWFControlMOVEM < LWFData
  def initialize(placeId, matrixId)
    @placeId = placeId
    @matrixId = matrixId
  end
  def to_bytes
    to_u32(@placeId) +
    to_u32(@matrixId)
  end
end

class LWFControlMOVEC < LWFData
  def initialize(placeId, colorTransformId)
    @placeId = placeId
    @colorTransformId = colorTransformId
  end
  def to_bytes
    to_u32(@placeId) +
    to_u32(@colorTransformId)
  end
end

class LWFControlMOVEMC < LWFData
  def initialize(placeId, matrixId, colorTransformId)
    @placeId = placeId
    @matrixId = matrixId
    @colorTransformId = colorTransformId
  end
  def to_bytes
    to_u32(@placeId) +
    to_u32(@matrixId) +
    to_u32(@colorTransformId)
  end
end

class LWFControl < LWFData
  def initialize(controlType, controlId)
    @controlType = controlType
    @controlId = controlId
  end
  def to_bytes
    to_u32(@controlType) +
    to_u32(@controlId)
  end
end

class LWFFrame < LWFData
  attr_reader :controlOffset
  def initialize(controlOffset, controls)
    @controlOffset = controlOffset
    @controls = controls
  end
  def to_bytes
    to_u32(@controlOffset) +
    to_u32(@controls)
  end
end

class LWFMovieClipEvent < LWFData
  def initialize(clipEvent, actionId)
    @clipEvent = clipEvent
    @actionId = actionId
  end
  def to_bytes
    to_u32(@clipEvent) +
    to_u32(@actionId)
  end
end

class LWFMovie < LWFData
  attr_reader :linkage_name
  def initialize(depths, labelOffset, labels, frameOffset, frames, linkage_name, clipEventId, clipEvents)
    @depths = depths
    @labelOffset = labelOffset
    @labels = labels
    @frameOffset = frameOffset
    @frames = frames
    @linkage_name = linkage_name
    @clipEventId = clipEventId
    @clipEvents = clipEvents
  end
  def to_bytes
    to_u32(@depths) +
    to_u32(@labelOffset) +
    to_u32(@labels) +
    to_u32(@frameOffset) +
    to_u32(@frames) +
    to_u32(@clipEventId) +
    to_u32(@clipEvents)
  end
end

class LWFLabel
  def initialize(stringId, frameNo)
    @stringId = stringId
    @frameNo = frameNo
  end
  def to_bytes
    to_u32(@stringId) +
    to_u32(@frameNo)
  end
end

class LWFMovieLinkage
  attr_reader :stringId, :lwfMovieId
  def initialize(stringId, lwfMovieId)
    @stringId = stringId
    @lwfMovieId = lwfMovieId
  end
  def to_bytes
    to_u32(@stringId) +
    to_u32(@lwfMovieId)
  end
end

class LWFInstanceName
  def initialize(stringId)
    @stringId = stringId
  end
  def to_bytes
    to_u32(@stringId)
  end
end

class LWFEvent
  def initialize(stringId)
    @stringId = stringId
  end
  def to_bytes
    to_u32(@stringId)
  end
end

class LWFString
  attr_reader :stringOffset, :stringLength
  def initialize(stringOffset, stringLength)
    @stringOffset = stringOffset
    @stringLength = stringLength
  end
  def to_bytes
    to_u32(@stringOffset) +
    to_u32(@stringLength)
  end
end

def info(str)
  @logfile.puts str if $DEBUG and @logfile
  puts str if $DEBUG
end

def warn(str)
  @logfile.puts "WARN: #{str}" if @logfile
  puts "WARN: #{str}"
end

def error(str)
  @logfile.puts "ERROR: #{str}" if @logfile
  puts "ERROR: #{str}"
end

class Stage
  attr_accessor :x_min, :x_max, :y_min, :y_max
end

class Rect
  attr_accessor :width, :height
  def initialize(width, height)
    @width = width
    @height = height
  end
end

class Color
  attr_accessor :red, :green, :blue, :alpha
  def initialize
    @red = 0.0
    @green = 0.0
    @blue = 0.0
    @alpha = 1.0
  end
  def dump
    "(#{red},#{green},#{blue},#{alpha})"
  end
end

class Matrix
  attr_accessor :scale_x, :scale_y
  attr_accessor :rotate_skew0, :rotate_skew1
  attr_accessor :translate_x, :translate_y
  def initialize
    @scale_x = 1.0
    @scale_y = 1.0
    @rotate_skew0 = 0.0
    @rotate_skew1 = 0.0
    @translate_x = 0.0
    @translate_y = 0.0
  end
  def is_translate_only?
    @scale_x == 1.0 and @scale_y == 1.0 and
      @rotate_skew0 == 0.0 and @rotate_skew1 == 0.0
  end
  def dump
    "(#{@scale_x},#{@scale_y},#{@rotate_skew0},#{@rotate_skew1}," +
      "#{@translate_x},#{@translate_y})"
  end
end

class ColorTransform
  attr_accessor :multi, :add
  def initialize
    @multi = Color.new
    @multi.red = 1.0
    @multi.green = 1.0
    @multi.blue = 1.0
    @add = Color.new
    @add.alpha = 0.0
  end
  def is_multi_alpha_only?
    @multi.red == 1.0 and @multi.green == 1.0 and @multi.blue == 1.0 and
      @add.red == 0.0 and @add.green == 0.0 and @add.blue == 0.0 and
        @add.alpha == 0.0
  end
  def is_default?
    @multi.red == 1.0 and
      @multi.green == 1.0 and
      @multi.blue == 1.0 and
      @multi.alpha == 1.0 and
      @add.red == 0.0 and
      @add.green == 0.0 and
      @add.blue == 0.0 and
      @add.alpha == 0.0
  end
  def dump
    "multi(r:#{@multi.red},g:#{@multi.green},b:#{@multi.blue}," +
      "a:#{@multi.alpha}) add(r:#{@add.red},g:#{@add.green}," +
        "b:#{@add.blue},a:#{@add.alpha})"
  end
end

class FillStyleBitmap
  attr_reader :object, :matrix
  def initialize(object, matrix)
    @object = object
    @matrix = matrix
  end
end

class Vertex
  attr_accessor :x, :y
  def initialize(x, y)
    @x = x
    @y = y
  end
end

class SWFObject
  attr_reader :ref
  def initialize
    clear_reference
  end
  def reference
    @ref += 1
  end
  def clear_reference
    @ref = 0
  end
end

class Texture < SWFObject
  attr_accessor :filename, :name,
    :width, :height, :format, :scale,
    :textureatlas, :losslessdata, :need_to_export
  def initialize(width, height, losslessdata)
    super()
    @width = width
    @height = height
    @losslessdata = losslessdata
    @need_to_export = false
    @format = TEXTUREFORMAT_NORMAL
    @scale = 1.0
    @textureatlas = false
  end

  def export_png(swf)
    if @need_to_export and @ref > 0
      @losslessdata.export_png(swf, @filename)
    end
  end
end

class Bitmap < SWFObject
  attr_accessor :texture, :u, :v, :width, :height, :matrix
  def initialize(texture, u, v, width, height, matrix)
    super()
    @texture = texture
    @u = u
    @v = v
    @width = width
    @height = height
    @matrix = matrix
  end
  def reference
    @texture.reference unless @texture.nil?
    super()
  end
end

class Font < SWFObject
  attr_reader :name, :letterspacing, :orgname
  attr_accessor :font_id
  def initialize(fontinfo, orgname)
    super()
    if fontinfo.nil?
      @name = orgname
      @letterspacing = 0
    else
      @name = fontinfo['name']
      @letterspacing = (fontinfo['letterspacing'] || 0).to_f
    end
    @orgname = orgname
  end
end

class Text < SWFObject
  attr_reader :max_length, :font, :font_height, :align,
    :left_margin, :right_margin, :letter_spacing, :leading,
      :stroke_color, :stroke_width, :shadow_color, :shadow_offset_x,
      :shadow_offset_y, :shadow_blur, :color, :name, :text, :matrix
  def initialize(stage, max_length, font, font_height, align,
      left_margin, right_margin, letter_spacing, leading, stroke_color,
      stroke_width, shadow_color, shadow_offset_x, shadow_offset_y, shadow_blur,
      color, name, text)
    super()
    @stage = stage
    @max_length = max_length
    @font = font
    @font_height = font_height
    @align = align
    @left_margin = left_margin
    @right_margin = right_margin
    @letter_spacing = letter_spacing
    @leading = leading
    @stroke_color = stroke_color
    @stroke_width = stroke_width
    @shadow_color = shadow_color
    @shadow_offset_x = shadow_offset_x
    @shadow_offset_y = shadow_offset_y
    @shadow_blur = shadow_blur
    @color = color
    @name = name
    @text = text
    @matrix = Matrix.new
    @matrix.translate_x = @stage.x_min
    @matrix.translate_y = @stage.y_min
  end

  def width
    @stage.x_max - @stage.x_min
  end

  def height
    @stage.y_max - @stage.y_min
  end

  def reference
    super()
    font.reference
  end
end

class Particle < SWFObject
  attr_reader :name, :matrix
  def initialize(name, matrix)
    super()
    @name = name
    @matrix = matrix
  end
end

class ProgramObject < SWFObject
  attr_reader :name, :width, :height, :matrix
  def initialize(name, width, height, matrix)
    super()
    @name = name
    @width = width
    @height = height
    @matrix = matrix
  end
end

class Graphic < SWFObject
  attr_reader :graphic_objects
  def initialize
    super()
    @graphic_objects = Array.new
  end
  def add_graphic_object(object)
    @graphic_objects.push object
  end
  def reference
    graphic_objects.each{|o| o.reference}
    super()
  end
end

class Place
  attr_reader :depth, :object, :instance_name, :matrix, :blend_mode
  def set(depth, object, instance_name, matrix, blend_mode)
    @depth = depth
    @object = object
    @instance_name = instance_name
    @matrix = matrix
    @blend_mode = blend_mode
  end
end

class ControlMOVE
  attr_reader :place, :matrix, :color_transform
  def initialize(place, matrix, color_transform)
    @place = place
    @matrix = matrix
    @color_transform = color_transform
  end
  def get_matrix
    @matrix || @place.matrix
  end
  def get_color_transform
    @color_transform
  end
  def get_depth
    @place.depth
  end
end

class ControlACTION
  attr_reader :actions
  def initialize(actions)
    @actions = actions
  end
  def get_depth
    0
  end
end

class Movie < SWFObject
  attr_accessor :frames, :labels, :depth_max,
    :display_list_prev, :display_list, :parent_movie, :linkage_name, :actions,
    :linkage_name_lambdas
  def initialize(movie)
    super()
    @frames = Array.new
    @labels = Hash.new
    @display_list_prev = Array.new
    @display_list = Array.new
    @controls = nil
    @parent_movie = movie
    @depth_max = 0
    @ref = 0
    @linkage_name = nil
    @linkage_name_lambdas = []
    @actions = Array.new
  end

  def clear
    @display_list_prev = nil
    @display_list = nil
  end

  def add_control(control)
    depth = control.get_depth
    @depth_max = depth if depth > @depth_max
    @controls ||= Array.new
    @controls.push control
  end

  def add_frame
    size = @display_list_prev.size
    size = @display_list.size if @display_list.size > size
    for i in 0...size do
      if !@display_list[i].nil?
        @display_list_prev[i] = @display_list[i]
      elsif !@display_list_prev[i].nil?
        control = @display_list_prev[i]
        add_control control
        @display_list_prev[i] = control
      end
    end
    @display_list = Array.new
    @frames.push(@controls || Array.new)
    @controls = nil
  end

  def set_label(label)
    @labels[label] = @frames.size
  end

  def add_action action
    @actions.push action
  end
end

class Button < SWFObject
  attr_accessor :width, :height,
    :matrix, :color_transform, :actions, :name, :type, :script_actions,
    :linkage_name_lambdas
  def initialize(width, height, matrix, color_transform)
    super()
    @width = width
    @height = height
    @matrix = matrix
    @color_transform = color_transform
    @actions = Array.new
    @script_actions = Hash.new
    @name = nil
    @linkage_name_lambdas = []
  end

  def add_action(action)
    @actions.push action
  end

  def add_script_action(key, action)
    actions = @script_actions[key]
    if actions.nil?
      actions = @actions.dup
      @script_actions[key] = actions
    end
    actions.push action
  end

  def dup_by_key(key)
    b = dup()
    b.actions = @script_actions[key]
    b.script_actions = Hash.new
    b
  end
end

def get_byte
  error "SWF data error" if @pos >= @swf.size
  data = @swf[@pos].ord
  @pos += 1
  data
end

def get_word
  get_byte + (get_byte << 8)
end

def get_sword
  v = get_byte + (get_byte << 8)
  if v > 32767
    v -= 65536
  end
  v
end

def get_dword
  get_byte + (get_byte << 8) + (get_byte << 16) + (get_byte << 24)
end

def init_bits
  @bit_pos = 0
  @bit_buf = 0
end

def get_bits(n)
  v = 0
  while true
    s = n - @bit_pos
    if (s > 0)
      v |= @bit_buf << s
      n -= @bit_pos
      @bit_buf = get_byte
      @bit_pos = 8
    else
      v |= @bit_buf >> -s
      @bit_pos -= n
      @bit_buf &= 0xff >> (8 - @bit_pos)
      return v
    end
  end
end

def get_sbits(n)
  v = get_bits(n)
  v |= -1 << n if (v & (1 << (n - 1))) != 0
  v
end

def get_string
  str = ""
  while true
    c = get_byte
    break if c == 0
    str += c.chr
  end
  str.force_encoding("UTF-8")
end

def get_data(length)
  error "SWF data error" if @pos + length >= @swf.size
  data = @swf[@pos, length]
  @pos += length
  data
end

def get_stage
  init_bits
  bits = get_bits(5)
  stage = Stage.new
  stage.x_min = get_sbits(bits) / 20.0
  stage.x_max = get_sbits(bits) / 20.0
  stage.y_min = get_sbits(bits) / 20.0
  stage.y_max = get_sbits(bits) / 20.0
  stage
end

def get_rgb
  color = Color.new
  color.red = get_byte / 255.0
  color.green = get_byte / 255.0
  color.blue = get_byte / 255.0
  color
end

def get_rgba
  color = Color.new
  color.red = get_byte / 255.0
  color.green = get_byte / 255.0
  color.blue = get_byte / 255.0
  color.alpha = get_byte / 255.0
  color
end

def get_xrgb
  color = Color.new
  color.red = get_byte / 255.0
  color.green = get_byte / 255.0
  color.blue = get_byte / 255.0
  color
end

def get_argb
  color = Color.new
  color.alpha = get_byte / 255.0
  color.red = get_byte / 255.0
  color.green = get_byte / 255.0
  color.blue = get_byte / 255.0
  color
end

def get_matrix
  matrix = Matrix.new
  init_bits
  if get_bits(1) != 0
    bits = get_bits(5)
    matrix.scale_x = get_sbits(bits) / 65536.0
    matrix.scale_y = get_sbits(bits) / 65536.0
  end
  if get_bits(1) != 0
    bits = get_bits(5)
    matrix.rotate_skew1 = get_sbits(bits) / 65536.0
    matrix.rotate_skew0 = get_sbits(bits) / 65536.0
  end
  bits = get_bits(5)
  matrix.translate_x = get_sbits(bits) / 20.0
  matrix.translate_y = get_sbits(bits) / 20.0
  matrix
end

def get_flags(n)
  flags = Array.new
  init_bits
  d = get_bits(n)
  for i in 0...n
    flags.push((d & ((1 << (n - 1)) >> i)) != 0 ? true : false)
  end
  flags
end

def get_color_transform(with_alpha)
  color_transform = ColorTransform.new
  has_add, has_multi = get_flags(2)
  bits = get_bits(4)
  if has_multi
    color_transform.multi.red = get_sbits(bits) / 256.0
    color_transform.multi.green = get_sbits(bits) / 256.0
    color_transform.multi.blue = get_sbits(bits) / 256.0
    color_transform.multi.alpha = get_sbits(bits) / 256.0 if with_alpha
  end
  if has_add
    color_transform.add.red = get_sbits(bits) / 256.0
    color_transform.add.green = get_sbits(bits) / 256.0
    color_transform.add.blue = get_sbits(bits) / 256.0
    color_transform.add.alpha = get_sbits(bits) / 256.0 if with_alpha
  end
  color_transform
end

def get_action_word
  if @version >= 6
    d = get_dword
  else
    d = get_word
  end
  d
end

def get_tag
  code = get_word
  length = code & 0x3f
  code >>= 6
  length = get_dword if length == 0x3f
  @pos_next = @pos + length
  code
end

def parse_show_frame
  @current_movie.add_frame
end

def parse_fill_styles(has_alpha)
  count = get_byte
  count = get_word if count == 255
  @fill_styles = Array.new
  for i in 0...count
    type = get_byte
    if (type & 0x10) != 0
      matrix = get_matrix
      gradients = get_byte
      for j in 0...gradients
        get_byte
        get_byte
        get_byte
        get_byte
        get_byte if has_alpha
      end
    elsif (type & 0x40) != 0
      obj_id = get_word
      matrix = get_matrix
      matrix.scale_x /= 20.0
      matrix.rotate_skew0 /= 20.0
      matrix.rotate_skew1 /= 20.0
      matrix.scale_y /= 20.0
      if obj_id != 0xffff
        @fill_styles[i] = FillStyleBitmap.new(@objects[obj_id], matrix)
      end
    else
      get_byte
      get_byte
      get_byte
      get_byte if has_alpha
    end
  end
end

def parse_line_styles(has_alpha)
  line_styles = get_byte
  line_styles = get_word if line_styles == 255
  for i in 0...line_styles
    twips = get_word
    if has_alpha
      color = get_rgba
    else
      color = get_rgb
    end
  end
end

def parse_define_shape
  has_alpha = (@tag_parser == :parse_define_shape3 ? true : false)

  obj_id = get_word
  stage = get_stage

  info "  shape #{obj_id}"

  @current_graphic = Graphic.new
  @objects[obj_id] = @current_graphic

  parse_fill_styles(has_alpha)
  parse_line_styles(has_alpha)

  init_bits
  fill_bits = get_bits(4)
  line_bits = get_bits(4)

  info "  decode rectangles."
  
  vertices = Array.new
  u = 0.0
  v = 0.0

  while true
    if get_bits(1) == 0
      flags = get_bits(5)
      break if flags == 0

      if (flags & (1 << 0)) != 0
        bits = get_bits(5)
        u = get_sbits(bits) / 20.0
        v = get_sbits(bits) / 20.0
        info "  MOVE: (#{u}, #{v})"
      end

      if (flags & (1 << 1)) != 0
        fill_style0 = @fill_styles[get_bits(fill_bits) - 1]
      end
      if (flags & (1 << 2)) != 0
        fill_style1 = @fill_styles[get_bits(fill_bits) - 1]
      end
      if (flags & (1 << 3)) != 0
        line_style = get_bits(line_bits) - 1
      end

      if (flags & (1 << 4)) != 0
        parse_fill_styles(has_alpha)
        parse_line_styles(has_alpha)

        init_bits
        fill_bits = get_bits(4)
        line_bits = get_bits(4)
      end
    else
      if get_bits(1) != 0
        bits = get_bits(4) + 2
        if get_bits(1) != 0
          delta_x = get_sbits(bits) / 20.0
          delta_y = get_sbits(bits) / 20.0
        else
          if get_bits(1) != 0
            delta_x = 0.0
            delta_y = get_sbits(bits) / 20.0
          else
            delta_x = get_sbits(bits) / 20.0
            delta_y = 0.0
          end
        end
        vertices.push Vertex.new(delta_x, delta_y)
        info "  LINE #{vertices.size - 1}: (#{delta_x}, #{delta_y})"

        if vertices.size == 4
          if (vertices[0].x * vertices[1].y -
              vertices[0].y * vertices[1].x) < 0.0
            style = fill_style0
          else
            style = fill_style1
          end
          texture = style.is_a?(FillStyleBitmap) ? style.object : nil

          if !style.nil?
            m = style.matrix
          else
            m = Matrix.new
          end
          dt = m.scale_x * m.scale_y - m.rotate_skew0 * m.rotate_skew1
          im = Matrix.new
          im.scale_x = m.scale_y / dt
          im.rotate_skew0 = -m.rotate_skew0 / dt
          im.translate_x =
            (m.rotate_skew0 * m.translate_y - m.translate_x * m.scale_y) / dt
          im.rotate_skew1 = -m.rotate_skew1 / dt
          im.scale_y = m.scale_x / dt
          im.translate_y =
            (m.translate_x * m.rotate_skew1 - m.scale_x * m.translate_y) / dt

          x = u
          y = v
          x_min = 0x7fffffff
          y_min = 0x7fffffff
          x_max = -0x7fffffff
          y_max = -0x7fffffff
          vertices.each do |vertex|
            x += vertex.x
            y += vertex.y
            dx = im.scale_x * x + im.rotate_skew0 * y + im.translate_x
            dy = im.rotate_skew1 * x + im.scale_y * y + im.translate_y
            dx = (dx * 20.0).round / 20.0
            dy = (dy * 20.0).round / 20.0
            x_min = dx if dx < x_min
            y_min = dy if dy < y_min
            x_max = dx if dx > x_max
            y_max = dy if dy > y_max
          end
          w = x_max - x_min
          h = y_max - y_min
          if !texture.nil?
            w = texture.width if (w - texture.width).abs < 1
            h = texture.height if (h - texture.height).abs < 1
          end
          u = x_min
          v = y_min
          rect = Rect.new(w, h)
          info "  RECT: #{w} x #{h}"

          if !texture.nil?
            if w > texture.width
              case texture.filename
              when /_REPEAT_S_/, /_REPEAT_ST_/, /_REPEAT_TS_/
                if (texture.width & (texture.width - 1)) != 0
                  error "Using _REPEAT_S_ Bitmap with Non-power-of-two Bitmap " +
                    "[#{texture.filename}]."
                end
              else
                error "Using BitmapFill with non-_REPEAT_S_ Bitmap " +
                  "[#{texture.filename}]."
              end
            end
            if h > texture.height
              case texture.filename
              when /_REPEAT_T_/, /_REPEAT_ST_/, /_REPEAT_TS_/
                if (texture.height & (texture.height - 1)) != 0
                  error "Using _REPEAT_T_ Bitmap with Non-power-of-two Bitmap " +
                    "[#{texture.filename}]."
                end
              else
                error "Using BitmapFill with non-_REPEAT_T_ Bitmap " +
                  "[#{texture.filename}]."
              end
            end
          end

          @current_graphic.add_graphic_object(
            Bitmap.new(texture, u, v, rect.width, rect.height,
              style.nil? ? Matrix.new : style.matrix))

          vertices = Array.new
        end
      else
        bits = get_bits(4) + 2
        get_sbits(bits)
        get_sbits(bits)
        get_sbits(bits)
        get_sbits(bits)
      end
    end
  end
  #warn "Shape(#{obj_id}) has no-rect shapes." unless vertices.empty?
end
alias parse_define_shape2 parse_define_shape
alias parse_define_shape3 parse_define_shape

def calc_float(imax, v)
  i = (imax * v).round
  i = imax if i > imax
  i
end

def parse_set_background_color
  color = get_rgb
  @background_color =
    (calc_float(0xff, color.red) << 16) |
    (calc_float(0xff, color.green) << 8) |
    (calc_float(0xff, color.blue) << 0)
  info sprintf("  background color: %08x\n", @background_color)
end

def parse_frame_label
  label = get_string
  @current_movie.set_label label
  @label_map[label] = true
end

SWFACTION_END                     = 0x00
# v3 actions
SWFACTION_NEXTFRAME               = 0x04
SWFACTION_PREVFRAME               = 0x05
SWFACTION_PLAY                    = 0x06
SWFACTION_STOP                    = 0x07
SWFACTION_GOTOFRAME               = 0x81
SWFACTION_GETURL                  = 0x83
SWFACTION_SETTARGET               = 0x8B
SWFACTION_GOTOLABEL               = 0x8C

NONVMINSTS = {
  SWFACTION_NEXTFRAME => true,
  SWFACTION_PREVFRAME => true,
  SWFACTION_PLAY => true,
  SWFACTION_STOP => true,
  SWFACTION_GOTOFRAME => true,
  SWFACTION_GETURL => true,
  SWFACTION_SETTARGET => true,
  SWFACTION_GOTOLABEL => true,
}

class DefineFuncionArgument
  attr_accessor :function, :args, :names, :next
  def initialize
    @names = Array.new
  end
end

class DefineFuncion2Argument
  attr_accessor :function, :args, :useregs, :flag, :names, :regs, :next
  def initialize
    @names = Array.new
    @regs = Array.new
  end
end

def parse_action
  info "  parse_action"
  actions = Array.new
  instmap = Hash.new
  branchmap = Hash.new
  functionmap = Hash.new
  functionends = Array.new
  while true
    instpos = @pos
    instmap[instpos] = actions.size
    functionends.delete_if do |f|
      if f == instpos
        actions.push [:ENDFUNCTION]
        true
      else
        false
      end
    end
    action = get_byte
    if action == 0
      actions.push [:END]
      branchmap.each do |k, v|
        error sprintf("data error: branch %d %d\n", k, v) if instmap[k].nil?
        actions[v][1] = instmap[k]
        info sprintf("  BRANCH %3d > %d\n", v, actions[v][1])
      end
      functionmap.each do |k, v|
        error sprintf("data error: function %d %d\n", k, v) if instmap[k].nil?
        actions[v][1].next = instmap[k] + 1
        info sprintf("  FUNCTION %3d > %d\n", v, actions[v][1].next)
      end
      return actions 
    end

    if (action & 0x80) != 0
      length = get_word
    else
      length = 0
    end

    info sprintf("  %3d: 0x%02x > %d", instpos, action, actions.size)

    if NONVMINSTS[action].nil?
        warn sprintf("SWF uses Actionscript [code=0x%x]", action)
    end

    case action
    when SWFACTION_NEXTFRAME
      info "    GOTONEXTFRAME"
      actions.push [:GOTONEXTFRAME]

    when SWFACTION_PREVFRAME
      info "    GOTOPREVFRAME"
      actions.push [:GOTOPREVFRAME]

    when SWFACTION_PLAY
      info "    PLAY"
      actions.push [:PLAY]

    when SWFACTION_STOP
      info "    STOP"
      actions.push [:STOP]

    when SWFACTION_GOTOFRAME
      frame = get_word
      info sprintf("    GOTOFRAME %d", frame)
      actions.push [:GOTOFRAME, frame]

    when SWFACTION_SETTARGET
      targets = Array.new
      target = get_string
      unless target.empty?
        targets.push :ROOT if target[0, 1] == '/'
        target.split(/\//).each do |t|
          unless t.empty?
            targets.push(t == ".." ? :PARENT : t)
          end
        end
      end
      info "    SETTARGET " + target
      actions.push [:SETTARGET, targets]

    when SWFACTION_GOTOLABEL
      label = get_string
      info "    GOTOLABEL " + label
      actions.push [:GOTOLABEL, label]

    when SWFACTION_GETURL
      url = get_string
      target = get_string
      if url =~ /^FSCommand:event$/i
        info "    EVENT " + target
        error "Invalid event name: #{target}" unless target =~ /^[a-zA-Z0-9_]+$/
        actions.push [:EVENT, target]
        @event_map[target] = true
      elsif url =~ /^FSCommand:skip$/i
        info "    SKIP"
        return actions
      else
        warn sprintf("SWF uses unknown fscommand url[#{url}] target[#{target}]")
      end

    else
      info "    UNKNOWN"
      unless @ignore_unknownaction
        warn sprintf("SWF uses unknown Actionscript [code=0x%x]", action)
      end
      data = get_data(length)
    end
  end
end

def parse_do_action
  sprite = get_word if @tag_parser == :parse_do_init_action
  actions = parse_action
  @current_movie.add_control ControlACTION.new(actions) unless actions.empty?
end
alias parse_do_init_action parse_do_action

def parse_define_bits_jpeg
  error "Bitmap is as JPEG. Bitmap should be as 'Loss-less'."
end
alias parse_define_bits_jpeg2 parse_define_bits_jpeg
alias parse_define_bits_jpeg3 parse_define_bits_jpeg
alias parse_define_bits_jpeg4 parse_define_bits_jpeg

def parse_define_bits_lossless
  obj_id = get_word
  bitmap_type = get_byte
  width = get_word
  height = get_word
  table_size = (bitmap_type == 3 ? get_byte : 0) + 1
  has_alpha = @tag_parser == :parse_define_bits_lossless2

  losslessdata = LosslessData.new(bitmap_type,
    width, height, table_size, has_alpha, @pos, @pos_next)
  texture = Texture.new(width, height, losslessdata)
  @textures.push texture
  @objects[obj_id] = texture
  info "  texture #{obj_id} #{width} #{height}"
end
alias parse_define_bits_lossless2 parse_define_bits_lossless

def parse_define_button2
  button = nil
  button_id = get_word
  menu = get_byte
  action_offset = get_word

  found_hit_frame = false
  while true
    state = get_byte
    break if state == 0
    obj_id = get_word
    layer = get_word
    matrix = get_matrix
    color_transform = get_color_transform(true)
    if (state & (1 << 3)) != 0
      error "There are some Hit frames of the button." if found_hit_frame
      found_hit_frame = true
      graphic = @objects[obj_id]
      unless graphic.is_a?(Graphic) or graphic.graphic_objects.empty?
        warn "The Hit frame of the button has some no-rect shapes."
      end
      if graphic.graphic_objects.size > 1
        warn "The Hit frame of the button has some rects not only one."
      end

      gobj = graphic.graphic_objects.first
      if gobj.nil?
        button_width = 0
        button_height = 0
      else
        gmatrix = gobj.matrix.dup
        matrix.translate_x += gobj.u + gmatrix.translate_x
        matrix.translate_y += gobj.v + gmatrix.translate_y
        button_width = gobj.width * gmatrix.scale_x
        button_height = gobj.height * gmatrix.scale_y
      end

      info sprintf("  button hit (%f,%f)", button_width, button_height)
      button = Button.new(button_width, button_height, matrix, color_transform)
      @objects[button_id] = button
    end
  end
  unless found_hit_frame
    error "The Hit frame of the button doesn't have any rect."
  end
  return if button.nil?

  if action_offset != 0
    while true
      length = get_word
      event = get_word
      info sprintf("  button event 0x%04x", event)
      actions = parse_action
      condition = 0
      keycode = 0
      if (event & (0x7f << 9)) != 0
        condition |= KEYPRESS
        keycode = ((event >> 9) & 0x7f)
      end
      if (event & (1 << 0)) != 0
        condition |= ROLLOVER
      end
      if (event & (1 << 1)) != 0
        condition |= ROLLOUT
      end
      if (event & (1 << 2)) != 0
        condition |= PRESS
      end
      if (event & (1 << 3)) != 0
        condition |= RELEASE
      end
      button.add_action [condition, actions, keycode] unless actions.empty?
      break if length == 0
    end
  end
  info "  button #{button_id} #{button_width}x#{button_height}"
end

def parse_define_font2
  obj_id = get_word
  has_layout, is_shiftjis, is_small_text, is_ansi,
    is_wide_offsets, is_wide_codes, is_italic, is_bold = get_flags(8)
  langcode = get_byte
  name_length = get_byte
  name = get_data(name_length - 1)
  get_byte

  fontinfo = nil
  @font_table.each do |t|
    if Regexp.new(t["regexp"]) =~ name
      fontinfo = t
      break
    end
  end
  font = Font.new(fontinfo, name)

  info "  font #{name}"

  @objects[obj_id] = font
  @fontname_map[font.name] = font
end
alias parse_define_font3 parse_define_font2

def parse_define_text
  error "Text should be 'Dynamic Text'."
end
alias parse_define_text2 parse_define_text

def parse_define_edit_text
  obj_id = get_word
  stage = get_stage

  has_text, is_word_wrap, is_multiline, is_password,
    is_readonly, has_color, has_max_length, has_font,
      reserved, use_auto_size, has_layout, is_no_select,
        use_border, reserved, use_html, use_outlines = get_flags(16)

  font_id = 0
  font_height = 0
  if has_font
    font_id = get_word
    font_height = (get_word / 20.0).round
  end

  color = has_color ? get_rgba : Color.new
  max_length = has_max_length ? get_word : 0

  align = LWF_TEXTPROPERTY_ALIGN_LEFT
  left_margin = 0
  right_margin = 0
  letter_spacing = 0
  leading = 0
  if has_layout
    align = get_byte
    align = LWF_TEXTPROPERTY_ALIGN_LEFT if align < 0 or align > 2
    left_margin = (get_word / 20.0).round
    right_margin = (get_word / 20.0).round
    get_sword
    leading = (get_sword / 20.0).round
  end
  name = get_string
  text = has_text ? get_string.gsub(/\r/, "\n") : ""

  stroke_color = nil
  stroke_width = 0
  shadow_color = nil
  shadow_offset_x = 0
  shadow_offset_y = 0
  shadow_blur = 0

  if use_html
    @html_text = ""
    @align = align
    @stroke_color = nil
    @stroke_width = 0
    @shadow_color = nil
    @shadow_offset_x = 0
    @shadow_offset_y = 0
    @shadow_blur = 0
    @use_stroke = false
    @use_shadow = false
    @letter_spacing = 0
    def check_node(node)
      if node.class == HTML::Text
        if @html_text == ""
          @html_text += node.content
        else
          @html_text += "\n" + node.content
        end
      elsif node.class == HTML::Tag
        href = node.attributes['href']
        unless href.nil?
          href.split(/,/).each do |t|
            case t
            when /^\s*valign\s*=\s*(.*)\s*$/
              case $1
              when "top"
                # NOTHING TO DO
              when "bottom"
                @align |= LWF_TEXTPROPERTY_VALIGN_BOTTOM
                if (@align & LWF_TEXTPROPERTY_VALIGN_MIDDLE) != 0
                  warn "DUPLICATE VALIGN SETTING: in #{href}"
                end
              when "middle"
                @align |= LWF_TEXTPROPERTY_VALIGN_MIDDLE
                if (@align & LWF_TEXTPROPERTY_VALIGN_BOTTOM) != 0
                  warn "DUPLICATE VALIGN SETTING: in #{href}"
                end
              else
                error "UNKNOWN VALIGN ARGUMENT: #{t} in #{href}"
              end
            when /^\s*strokeColor\s*=\s*(.*)\s*$/
              @use_stroke = true
              @stroke_color = to_color(href, "strokeColor", $1)
            when /^\s*strokeWidth\s*=\s*(.*)\s*$/
              @use_stroke = true
              @stroke_width = to_num(href, "strokeWidth", $1, false)
            when /^\s*shadowColor\s*=\s*(.*)\s*$/
              @use_shadow = true
              @shadow_color = to_color(href, "shadowColor", $1)
            when /^\s*shadowOffsetX\s*=\s*(.*)\s*$/
              @use_shadow = true
              @shadow_offset_x = to_num(href, "shadowOffsetX", $1)
            when /^\s*shadowOffsetY\s*=\s*(.*)\s*$/
              @use_shadow = true
              @shadow_offset_y = to_num(href, "shadowOffsetY", $1)
            when /^\s*shadowBlur\s*=\s*(.*)\s*$/
              @use_shadow = true
              @shadow_blur = to_num(href, "shadowBlur", $1)
            when /^\s*letterSpacing\s*=\s*(.*)\s*$/
              @letter_spacing = to_float(href, "letterSpacing", $1)
            else
              error "UNKNOWN ATTRIBUTE: #{t} in #{href}"
            end
          end
        end
        if @use_stroke and @stroke_color.nil?
          error "NEED strokeColor: in #{href}"
        end
        if @use_shadow and @shadow_color.nil?
          error "NEED shadowColor: in #{href}"
        end
      end
      node.children.each do |child|
        check_node(child)
      end
    end
    check_node(HTML::HTMLParser.parse(text).root)
    text = @html_text
    align = @align
    stroke_color = @stroke_color
    stroke_width = @stroke_width
    shadow_color = @shadow_color
    shadow_offset_x = @shadow_offset_x
    shadow_offset_y = @shadow_offset_y
    shadow_blur = @shadow_blur
    letter_spacing = @letter_spacing
  end

  graphic = Graphic.new
  gobj = Text.new(stage, max_length, @objects[font_id], font_height, align,
    left_margin, right_margin, letter_spacing, leading, stroke_color,
    stroke_width, shadow_color, shadow_offset_x, shadow_offset_y, shadow_blur,
    color, name, text)
  graphic.add_graphic_object(gobj)
  info "  text #{obj_id} name=#{name} align=#{align} " +
    "letter_spacing=#{letter_spacing}"
  unless stroke_color.nil?
    info "    stroke:color=#{stroke_color.dump} w=#{stroke_width} "
  end
  unless shadow_color.nil?
    info "    shadow:color=#{shadow_color.dump} x=#{shadow_offset_x} " +
      "y=#{shadow_offset_y} b=#{shadow_blur}"
  end
  @string_map[name] = true
  #warn("Text('#{name}') is already defined.") unless @text_name_map[name].nil?
  @text_name_map[name] = true
  @string_map[text] = true
  @objects[obj_id] = graphic
end

def parse_define_sprite
  obj_id = get_word
  frames = get_word
  @current_movie = Movie.new(@current_movie)
  @objects[obj_id] = @current_movie
  parse_tags
  @current_movie.clear
  @current_movie = @current_movie.parent_movie
  info "  movie frames:#{frames}"
end

def parse_place_object2
  if @tag_parser == :parse_place_object3
    has_actions, has_clipping_depth, has_name, has_morph_position,
      has_color_transform, has_matrix, has_obj_id, has_move, reserved,
      has_opaque_background, has_visible, has_image, has_classname,
      has_cache_as_bitmap, has_blend_mode, has_filter_list = get_flags(16)
  else
    has_actions, has_clipping_depth, has_name, has_morph_position,
      has_color_transform, has_matrix, has_obj_id, has_move = get_flags(8)
    reserved = has_opaque_background = has_visible = has_image = has_classname =
      has_cache_as_bitmap = has_blend_mode = has_filter_list = false
  end
  if $DEBUG
    info "  actions=#{has_actions ? 1 : 0}" +
      " clipping_depth=#{has_clipping_depth ? 1 : 0}" +
      " name=#{has_name ? 1 : 0}" +
      " morph_position=#{has_morph_position ? 1 : 0}" +
      " color_transform=#{has_color_transform ? 1 : 0}" +
      " matrix=#{has_matrix ? 1 : 0}"
    info "  obj_id=#{has_obj_id ? 1 : 0}" +
      " move=#{has_move ? 1 : 0}" +
      " reserved=#{reserved ? 1 : 0}" +
      " opaque_background=#{has_opaque_background ? 1 : 0}" +
      " visible=#{has_visible ? 1 : 0}" +
      " image=#{has_image ? 1 : 0}" +
      " classname=#{has_classname ? 1 : 0}"
    info "  cache_as_bitmap=#{has_cache_as_bitmap ? 1 : 0}" +
      " blend_mode=#{has_blend_mode ? 1 : 0}" +
      " filter_list=#{has_filter_list ? 1 : 0}"
  end
  depth = get_word
  prev_control = @current_movie.display_list_prev[depth]
  class_name = get_string if has_classname
  obj_id = get_word if has_obj_id
  matrix = nil
  if has_matrix
    matrix = get_matrix
  elsif prev_control
    matrix = prev_control.get_matrix
  end
  error "matrix error" if matrix.nil?
  color_transform = nil
  if has_color_transform
    color_transform = get_color_transform(true)
  elsif prev_control
    color_transform = prev_control.get_color_transform
  end
  color_transform = nil if color_transform and color_transform.is_default?

  morph_position = get_word if has_morph_position
  name = get_string if has_name
  clipping_depth = get_word if has_clipping_depth

  if has_filter_list
    n = get_byte
    n.times do
      filter_type = get_byte
      case filter_type
      when 0 # DropShadow
        get_rgba
        get_dword
        get_dword
        get_dword
        get_dword
        get_word
        get_flags(8)
      when 1 # Blur
        get_dword
        get_dword
        get_flags(8)
      when 2 # Glow
        get_rgba
        get_dword
        get_dword
        get_word
        get_flags(8)
      when 3 # Bevel
        get_rgba
        get_rgba
        get_dword
        get_dword
        get_dword
        get_dword
        get_word
        get_flags(8)
      when 4 # Gradient Glow
        n = get_byte
      when 5 # Convolution
        x = get_byte
        y = get_byte
        get_dword
        get_dword
        (x * y).times do
          get_dword
        end
        get_rgba
        get_flags(8)
      when 6 # Color Matrix
        20.times do
          get_dword
        end
      when 7 # Gradient Bevel
        nc = get_byte
        nc.times do
          get_rgba
        end
        nc.times do
          get_byte
        end
        get_dword
        get_dword
        get_dword
        get_dword
        get_word
        get_flags(8)
        nc = get_byte
        nc.times do
          get_rgba
        end
        nc.times do
          get_byte
        end
        get_dword
        get_dword
        get_dword
        get_dword
        get_word
        get_flags(8)
      end
    end
  end
  if has_blend_mode
    blend_mode = get_byte
    blend_mode = 0 if blend_mode == 1
    if blend_mode != 0
      if @format_version == LWF_FORMAT_VERSION
        if BLEND_MODE[blend_mode][:supported]
          @blend_mode_used = true
        else
          warn "blend mode \"#{BLEND_MODE[blend_mode][:type]}\" " +
            "is not supported"
          blend_mode = 0
        end
      else
        warn "blend mode is not normal with format:" +
          "#{sprintf("0x%06x", @format_version)}"
        blend_mode = 0
      end
    end
  else
    blend_mode = 0
  end
  bitmap_cache = get_byte if has_cache_as_bitmap
  get_byte if has_visible
  get_rgba if has_visible

  if has_obj_id
    place = Place.new

    instance_name = name ||
      create_instance_name(matrix.translate_x, matrix.translate_y)
    if @objects[obj_id].class == Button
      button = @objects[obj_id]

      current_movie = @current_movie
      frame_no = @current_movie.frames.size
      l = lambda {
        script_name = "#{current_movie.linkage_name}_" +
          "#{frame_no}_#{button.name}_#{instance_name}"
        m = @instance_script_map[script_name]
        if m
          m.each do |condition, funcname|
            if @script_funcname_map[funcname]
              case condition
              when "press"
                condition = PRESS
              when "release"
                condition = RELEASE
              when "rollOver"
                condition = ROLLOVER
              when "rollOut"
                condition = ROLLOUT
              end
              as = @script_funcname_map[funcname][:ActionScript]
              unless as.nil?
                button.add_script_action(place, [condition, as, 0])
                @script_funcname_map[funcname].delete(:ActionScript)
              end
              if @script_funcname_map[funcname].empty?
                @script_funcname_map.delete(funcname)
              else
                button.add_script_action(
                  place, [condition, [[:CALL, funcname]], 0])
                @using_script_funcname_map[funcname] =
                  @script_funcname_map[funcname]
              end
            end
          end
        end
      }
      if @version >= 20
        if button.name.nil?
          button.linkage_name_lambdas.push(l)
        else
          l.call
        end
      else
        if @current_movie.linkage_name.nil?
          @current_movie.linkage_name_lambdas.push(l)
        else
          l.call
        end
      end
    end
    @objects[obj_id].reference
    place.set(depth,
      @objects[obj_id], name, matrix, BLEND_MODE[blend_mode][:value])
    info "  PLACE depth:#{depth} obj:#{obj_id} name:[#{name}] " +
      "blend_mode:#{BLEND_MODE[blend_mode][:type]}"
    control = ControlMOVE.new(place, matrix, color_transform)
    @instance_name_map[name] = true unless name.nil?
  else
    info "  MOVE depth:#{depth}"
    control = ControlMOVE.new(prev_control.place, matrix, color_transform)
  end
  info "    matrix:" + matrix.dump unless matrix.nil?
  info "    color:" + color_transform.dump unless color_transform.nil?
  @current_movie.add_control control
  @current_movie.display_list[depth] = control

  if @version >= 5 and has_actions
    movie = @objects[obj_id]
    get_word
    get_action_word
    while true
      event = get_action_word
      break if event == 0
      size = get_dword
      get_byte if (event & 0x00400000) != 0
      actions = parse_action
      if actions.size >= 1
        e = 0
        e |= LOAD if (event & 0x1) != 0
        e |= ENTERFRAME if (event & 0x2) != 0
        e |= UNLOAD if (event & 0x4) != 0
        movie.add_action [e, actions] if e != 0
      end
    end

    current_movie = @current_movie
    frame_no = @current_movie.frames.size
    l = lambda {
      script_name = "#{current_movie.linkage_name}_" +
        "#{frame_no}_#{movie.linkage_name}_#{instance_name}"
      m = @instance_script_map[script_name]
      if m
        m.each do |event, funcname|
          if @script_funcname_map[funcname]
            case event
            when "load"
              event = LOAD
            when "enterFrame"
              event = ENTERFRAME
            when "unload"
              event = UNLOAD
            end
            movie.add_action [event, [[:CALL, funcname]]]
            @using_script_funcname_map[funcname] =
              @script_funcname_map[funcname]
          end
        end
      end
    }
    if @current_movie.linkage_name.nil?
      @current_movie.linkage_name_lambdas.push(l)
    else
      l.call
    end
  end
end
alias parse_place_object3 parse_place_object2

def parse_remove_object
  obj_id = get_word if @tag_parser == :parse_remove_object
  depth = get_word
  info "  depth #{depth}"
  @current_movie.display_list_prev[depth] = nil
  @current_movie.display_list[depth] = nil
end
alias parse_remove_object2 parse_remove_object

def parse_export
  externals = get_word
  for i in 0...externals
    obj_id = get_word
    name = get_string
    info "  export #{obj_id} -> '#{name}'"
    object = @objects[obj_id]
    if object.is_a?(Texture)
      @objects[obj_id].filename = name
    elsif object.is_a?(Button)
      case name
      when /_PARTICLE_(.*)/
        name = $1
        @particle_name_map[name] = true
        @objects[obj_id].name = name
        @objects[obj_id].type = :PARTICLE
      when /_PROG_([a-zA-Z0-9_]+)/
        name = $1
        @program_object_name_map[name] = true
        @objects[obj_id].name = name
        @objects[obj_id].type = :PROGRAMOBJECT
      else
        @objects[obj_id].name = name
        unless object.linkage_name_lambdas.nil?
          object.linkage_name_lambdas.each do |l|
            l.call
          end
          object.linkage_name_lambdas = nil
        end
      end
    elsif object.is_a?(Movie)
      @movie_linkage_name_map[name] = true
      object.reference
      object.linkage_name = name
      unless object.linkage_name_lambdas.nil?
        object.linkage_name_lambdas.each do |l|
          l.call
        end
        object.linkage_name_lambdas = nil
      end
    end
  end
end
alias parse_symbol_class parse_export

Tags = {
   1 => :parse_show_frame,
   2 => :parse_define_shape,
   3 => :parse_free_character,
   4 => :parse_place_object,
   5 => :parse_remove_object,
   6 => :parse_define_bits_jpeg,
   7 => :parse_define_button,
   8 => :parse_jpeg_tables,
   9 => :parse_set_background_color,
  10 => :parse_define_font,
  11 => :parse_define_text,
  12 => :parse_do_action,
  13 => :parse_define_font_info,
  14 => :parse_define_sound,
  15 => :parse_start_sound,
  16 => :parse_stop_sound,
  17 => :parse_define_button_sound,
  18 => :parse_sound_stream_head,
  19 => :parse_sound_stream_block,
  20 => :parse_define_bits_lossless,
  21 => :parse_define_bits_jpeg2,
  22 => :parse_define_shape2,
  23 => :parse_define_button_cxform,
  24 => :parse_protect,
  25 => :parse_paths_are_postaction,
  26 => :parse_place_object2,
  28 => :parse_remove_object2,
  29 => :parse_sync_frame,
  31 => :parse_free_all,
  32 => :parse_define_shape3,
  33 => :parse_define_text2,
  34 => :parse_define_button2,
  35 => :parse_define_bits_jpeg3,
  36 => :parse_define_bits_lossless2,
  37 => :parse_define_edit_text,
  38 => :parse_define_video,
  39 => :parse_define_sprite,
  40 => :parse_name_character,
  41 => :parse_serial_number,
  42 => :parse_define_text_format,
  43 => :parse_frame_label,
  45 => :parse_sound_stream_head2,
  46 => :parse_define_morph_shape,
  47 => :parse_generate_frame,
  48 => :parse_define_font2,
  49 => :parse_generator_command,
  50 => :parse_define_command_object,
  51 => :parse_character_set,
  52 => :parse_external_font,
  56 => :parse_export,
  57 => :parse_import,
  58 => :parse_protect_debug,
  59 => :parse_do_init_action,
  60 => :parse_define_video_stream,
  61 => :parse_video_frame,
  62 => :parse_define_font_info2,
  64 => :parse_protect_debug2,
  65 => :parse_action_limits,
  66 => :parse_set_tab_index,
  69 => :parse_file_attributes,
  70 => :parse_place_object3,
  71 => :parse_import2,
  73 => :parse_define_font_align_zones,
  74 => :parse_csm_text_settings,
  75 => :parse_define_font3,
  76 => :parse_symbol_class,
  77 => :parse_metadata,
  78 => :parse_define_scaling_grid,
  82 => :parse_do_abc,
  83 => :parse_define_shape4,
  84 => :parse_define_morph_shape2,
  86 => :parse_define_scene_and_frame_label_data,
  87 => :parse_define_binary_data,
  88 => :parse_define_font_name,
  89 => :parse_start_sound2,
  90 => :parse_define_bits_jpeg4,
  91 => :parse_define_font4,
}

def parse_tags
  while true
    @tag = get_tag
    break if @tag == 0
    @tag_parser = Tags[@tag]
    if @tag_parser.nil?
      info "TAG: #{@tag} unknown."
    else
      info @tag_parser.to_s.sub(/parse_/, 'TAG: ')
      if @tag_parser.nil? or !respond_to?(@tag_parser, true)
        info "  skip"
      else
        self.send @tag_parser
      end
    end
    @pos = @pos_next
  end
end

def load_swf(filename)
  @filename = filename
  f = File.open(@filename, 'rb')
  swf = f.read
  f.close
  swf.force_encoding("ASCII-8BIT")

  magic, @version, length = swf.unpack('a3cV')
  case @version
  when 7
  when 8
  when 20
  else
    warn "SWF Format Version #{@version} is not supported"
  end

  case magic
  when 'FWS'
  when 'CWS'
    begin
      data = Zlib::Inflate.inflate(swf[8, swf.size - 8])
      swf = ['FWS', @version, data.length].pack('a3cV') + data
    rescue
      error "Failed to extract."
    end
  else
    error "It is not SWF."
  end
  @swf_data = swf
end

def parse_swf()
  @swf = @swf_data
  @pos = 8
  @stage = get_stage
  @frame_rate = get_byte / 256.0 + get_byte
  @frames = get_word
  @root_movie = Movie.new(nil)
  @root_movie.linkage_name = "_root"
  @root_movie.reference
  @current_movie = @root_movie

  parse_tags

  @current_movie.clear
  if @objects.empty? 
    root_movie_id = 0
  else
    root_movie_id = @objects.keys.sort.last + 1
  end
  @objects[root_movie_id] = @root_movie

  empty_movie = Movie.new(nil)
  empty_movie.add_frame
  empty_movie.linkage_name = "_empty"
  empty_movie.reference
  empty_movie_id = root_movie_id + 1
  @objects[empty_movie_id] = empty_movie
end

def add_matrix(matrix)
  if matrix.is_translate_only?
    lwfTranslate = LWFTranslate.new(matrix)
    translateId = @map_translate[lwfTranslate.to_a]
    if translateId.nil?
      translateId = @data_translate.size
      @data_translate.push lwfTranslate
      @map_translate[lwfTranslate.to_a] = translateId
    end
    translateId
  else
    lwfMatrix = LWFMatrix.new(matrix)
    matrixId = @map_matrix[lwfMatrix.to_a]
    if matrixId.nil?
      matrixId = @data_matrix.size + MATRIX_FLAG
      @data_matrix.push lwfMatrix
      @map_matrix[lwfMatrix.to_a] = matrixId
    end
    matrixId
  end
end

def add_colorTransform(colorTransform)
  if colorTransform.is_multi_alpha_only?
    lwfAlphaTransform = LWFAlphaTransform.new(colorTransform)
    alphaTransformId = @map_alphaTransform[lwfAlphaTransform.to_a]
    if alphaTransformId.nil?
      alphaTransformId = @data_alphaTransform.size
      @data_alphaTransform.push lwfAlphaTransform
      @map_alphaTransform[lwfAlphaTransform.to_a] = alphaTransformId
    end
    alphaTransformId
  else
    lwfColorTransform = LWFColorTransform.new(colorTransform)
    colorTransformId = @map_colorTransform[lwfColorTransform.to_a]
    if colorTransformId.nil?
      colorTransformId = @data_colorTransform.size + COLORTRANSFORM_FLAG
      @data_colorTransform.push lwfColorTransform
      @map_colorTransform[lwfColorTransform.to_a] = colorTransformId
    end
    colorTransformId
  end
end

LWF_ACTION_END = 0
LWF_ACTION_PLAY = 1
LWF_ACTION_STOP = 2
LWF_ACTION_GOTONEXTFRAME = 3
LWF_ACTION_GOTOPREVFRAME = 4
LWF_ACTION_GOTOFRAME = 5
LWF_ACTION_GOTOLABEL = 6
LWF_ACTION_SETTARGET = 7
LWF_ACTION_EVENT = 8
LWF_ACTION_CALL = 9

LWF_INSTANCE_TARGET_ROOT = 0xFFFFFFFF
LWF_INSTANCE_TARGET_PARENT = 0xFFFFFFFE

def add_actions(actions)
  actionId = @map_action[actions]
  if actionId.nil?
    action_offset = @actionBytes.size
    actions.each_with_index do |action, i|
      info sprintf("  %3d: %s", i, action[0].to_s)
      instruction = eval("LWF_ACTION_" + action[0].to_s)
      @actionBytes += to_u8(instruction)
      @actions.push instruction
      case action[0]
      when :GOTOFRAME
        @actionBytes += to_u32(action[1])
        @actions += [action[1], 0, 0, 0]
      when :SETTARGET
        @actionBytes += to_u32(action[1].size)
        @actions += [action[1].size, 0, 0, 0]
        action[1].each do |target|
          info sprintf("      [%s]", target)
          case target
          when :ROOT
            @actionBytes += to_u32(LWF_INSTANCE_TARGET_ROOT)
            @actions += [LWF_INSTANCE_TARGET_ROOT, 0, 0, 0]
          when :PARENT
            @actionBytes += to_u32(LWF_INSTANCE_TARGET_PARENT)
            @actions += [LWF_INSTANCE_TARGET_PARENT, 0, 0, 0]
          else
            instanceNameId = @map_instanceName[target]
            if instanceNameId.nil?
              error("Instance(#{target}) not found")
              instanceNameId = 0
            end
            info sprintf("      %d\n", instanceNameId)
            @actionBytes += to_u32(instanceNameId)
            @actions += [instanceNameId, 0, 0, 0]
          end
        end
      when :GOTOLABEL
        stringId = @strings[action[1]]
        if stringId.nil?
          error("Label(#{action[1]}) not found")
          stringId = -1
        end
        @actionBytes += to_u32(stringId)
        @actions += [stringId, 0, 0, 0]
      when :EVENT
        eventId = @map_event[action[1]]
        info sprintf("      [%d]", eventId)
        @actionBytes += to_u32(eventId)
        @actions += [eventId, 0, 0, 0]
      when :CALL
        info sprintf("      [%s]", action[1])
        stringId = @strings[action[1]]
        @actionBytes += to_u32(stringId)
        @actions += [stringId, 0, 0, 0]
      end
    end
    @actionBytes += to_u8(LWF_ACTION_END)
    @actions.push LWF_ACTION_END
    lwfAction = LWFAction.new(
      action_offset, @actionBytes.size - action_offset)

    actionId = @data_action.size
    @data_action.push lwfAction
    @map_action[actions] = actionId
  end
  info sprintf("action %d", actionId)
  actionId
end

def add_object(obj, objectType, objectId)
  lwfObject = LWFObject.new(objectType, objectId)
  lwfObjectId = @map_object[lwfObject.to_a]
  if lwfObjectId.nil?
    lwfObjectId = @data_object.size
    @data_object.push lwfObject
    @map_object[lwfObject.to_a] = lwfObjectId
  end
  @map_objectId[obj] = lwfObjectId
  lwfObjectId
end

LWFObjects = [
  :translate,
  :matrix,
  :color,
  :alphaTransform,
  :colorTransform,
  :object,
  :texture,
  :textureFragment,
  :bitmap,
  :bitmapEx,
  :font,
  :textProperty,
  :text,
  :particleData,
  :particle,
  :programObject,
  :graphicObject,
  :graphic,
  :action,
  :buttonCondition,
  :button,
  :label,
  :instanceName,
  :event,
  :place,
  :controlMoveM,
  :controlMoveC,
  :controlMoveMC,
  :control,
  :frame,
  :movieClipEvent,
  :movie,
  :movieLinkage,
  :string,
]

def create_instance_name(x, y)
  "x" + x.to_s.sub(/\.0$/, '').sub(/\./, "_").gsub(/-/, "_") +
    "_y" + y.to_s.sub(/\.0$/, '').sub(/\./, "_").gsub(/-/, "_")
end

def parse_xflxml(xml, isRootMovie = false)
  return unless xml =~ /Actionscript/
  if defined?(LibXML)
    doc = LibXML::XML::Document.string(xml)
    entries = doc.find('//xfl:script', 'xfl'=>'http://ns.adobe.com/xfl/2008/')
    elementsMsg = 'children'
    textMsg = 'content'
  else
    doc = REXML::Document.new(xml)
    entries = REXML::XPath.match(doc, "//script")
    elementsMsg = 'elements'
    textMsg = 'text'
  end
  entries.each do |e|
    case e.parent.parent.name
    when "DOMFrame"
      frame = e.parent.parent
      layer = frame.parent.parent
      layers = layer.parent
      timeline = layer.parent.parent
      item = timeline.parent.parent

      instance_linkage_name = nil
      instance_name = nil
      frame.each do |ee|
        begin
          if ee.name == "elements"
            ee.each do |eee|
              begin
                if eee.name == "DOMSymbolInstance"
                  instance_linkage_name =
                    @library_name_map[eee.attributes["libraryItemName"]]
                  instance_linkage_name =
                    escape(instance_linkage_name).gsub(/(\/|%2F)/, '_')
                  instance_name = eee.attributes["name"]
                  instance_name = escape(instance_name)
                end
              rescue NoMethodError
              end
            end
          end
        rescue NoMethodError
        end
      end

      linkageName = timeline.attributes["name"]
      if item.name == "DOMSymbolItem"
        lid = item.attributes["linkageIdentifier"]
        linkageName = lid unless lid.nil?
      end
      linkageName = escape(linkageName)
  
      name = isRootMovie ? "_root" : linkageName
      layerElements = layers.send(elementsMsg)
      layerElements = [nil] +
        layerElements.delete_if{|l| l.name != "DOMLayer"} if defined?(LibXML)
      depth = layerElements.index(layer) # 1 origin
      index = frame.attributes["index"].to_i
  
      scripts = {}
      script_index = nil
      script_nest = 0
      i = 0
      nest = 0
      text = e.send(textMsg).strip.gsub(/\n/, "\001")
      while i < text.length
        s = text[i, text.length - i]
        pos = (/(\/\*\s*|\*\/\s*)/ =~ s)
        if pos
          i += pos
        else
          if @version >= 20
            scripts[:ActionScript] ||= {}
            scripts[:ActionScript]["frame"] ||= ""
            s.sub!(/^[\s\001]*\001/, '')
            s.sub!(/[\s\001]+$/, '')
            s.gsub!(/\001/, "\n")
            scripts[:ActionScript]["frame"] += s
          end
          break
        end
        s = text[i, text.length - i]
        skip = 0
        case s
        when /^(\/\*\s*)/
          skip += $1.length
          if script_index.nil?
            case s
            when /^(\/\*\s*)(as)(\s*\001|\s+)/i
              type = "frame"
              lang = :ActionScript
              script_index = i + $1.length + $2.length + $3.length
              skip += $2.length + $3.length
              script_nest = nest
            when /^(\/\*\s*)(js|js_load|js_postLoad|js_enterFrame|js_unload)(\s*\001|\s+)/i
              case $2.downcase
              when "js"
                type = "frame"
              when "js_load"
                type = "load"
              when "js_postload"
                type = "postLoad"
              when "js_enterframe"
                type = "enterFrame"
              when "js_unload"
                type = "unload"
              end
              if type != "frame" and index != 0
                error "#{name}:frame #{index+1}: " +
                  "js_#{type} should be in the first frame of the movie"
              else
                lang = :JavaScript
                script_index = i + $1.length + $2.length + $3.length
                skip += $2.length + $3.length
                script_nest = nest
              end
            when /^(\/\*\s*)(lua|lua_load|lua_postLoad|lua_enterFrame|lua_unload)(\s*\001|\s+)/i
              case $2.downcase
              when "lua"
                type = "frame"
              when "lua_load"
                type = "load"
              when "lua_postload"
                type = "postLoad"
              when "lua_enterframe"
                type = "enterFrame"
              when "lua_unload"
                type = "unload"
              end
              if type != "frame" and index != 0
                error "#{name}:frame #{index+1}: " +
                  "lua_#{type} should be in the first frame of the movie"
              else
                lang = :Lua
                script_index = i + $1.length + $2.length + $3.length
                skip += $2.length + $3.length
                script_nest = nest
              end
            end
          end
          nest += 1
        when /^(\*\/\s*)/
          skip += $1.length
          nest -= 1
          if !script_index.nil? and nest == script_nest
            scripts[lang] ||= {}
            scripts[lang][type] ||= ""
            tmp = text[script_index, i - script_index]
            tmp.sub!(/^[\s\001]*\001/, '')
            tmp.sub!(/[\s\001]+$/, '')
            tmp.gsub!(/\001/, "\n")
            scripts[lang][type] += tmp
            script_index = nil
          end
        end
        i += skip
      end

      re = Regexp.new("on\s*\(\s*(?<c>press|release|rollOver|rollOut)\s*\)\s*(?<p>\{(?:[^{}]|\g<p>)*\})")
      scripts.each do |lang, types|
        types.each do |type, script|
          if script =~ /\W/
            if type == "frame"
              funcname = "#{name}_#{index}_#{depth}"
              frame_action = true
              if lang == :ActionScript
                if instance_linkage_name
                  script.gsub(re) do |m|
                    event = $~[:c]
                    btnscript = $~[:p]
                    btnscript = btnscript.slice(1, btnscript.length - 2)
                    script_name = "#{name}_#{index}_" +
                      "#{instance_linkage_name}_#{instance_name}"
                    funcname = "#{script_name}_#{event}"
                    btnscript = compile_as(btnscript, funcname)
                    @instance_script_map[script_name] ||= Hash.new
                    @instance_script_map[script_name][event] ||= Hash.new
                    @instance_script_map[script_name][event] = funcname
                    @script_funcname_map[funcname] ||= {}
                    @script_funcname_map[funcname][lang] = btnscript
                    frame_action = false
                  end
                end
                script = compile_as(script, funcname) if frame_action
              end
              if frame_action
                @script_map[name] ||= Hash.new
                @script_map[name][index] ||= Hash.new
                @script_map[name][index][depth] = funcname
                @script_funcname_map[funcname] ||= {}
                @script_funcname_map[funcname][lang] = script
              end
            else
              funcname = "#{name}_#{type}"
              @using_script_funcname_map[funcname] ||= {}
              @using_script_funcname_map[funcname][lang] = script
            end
          end
        end
      end

    when "DOMSymbolInstance"
      symbol_instance = e.parent.parent
      frame = symbol_instance.parent.parent
      layer = frame.parent.parent
      layers = layer.parent
      item = layer.parent.parent.parent.parent

      instance_linkage_name =
        @library_name_map[symbol_instance.attributes["libraryItemName"]]
      instance_linkage_name =
        escape(instance_linkage_name).gsub(/(\/|%2F)/, '_')
      instance_name = symbol_instance.attributes["name"]
      instance_name = escape(instance_name)
      if instance_name.nil? or instance_name.empty?
        em =
          symbol_instance.send(elementsMsg).find(){|elm| elm.name == "matrix"}
        if em.nil?
          instance_name = create_instance_name(0, 0)
        else
          m = em.send(elementsMsg).find(){|elm| elm.name == "Matrix"}
          instance_name =
            create_instance_name(m.attributes["tx"], m.attributes["ty"])
        end
      end
      name = isRootMovie ? "_root" :
        escape(item.attributes["linkageIdentifier"])
      layerElements = layers.send(elementsMsg)
      layerElements = [nil] +
        layerElements.delete_if{|l| l.name != "DOMLayer"} if defined?(LibXML)
      depth = layerElements.index(layer) # 1 origin
      index = frame.attributes["index"].to_i

      event = nil
      event_nest = 0
      scripts = {}
      script_index = nil
      script_nest = 0
      i = 0
      nest = 0
      text = e.send(textMsg).strip.gsub(/\n/, "\001")
      while i < text.length
        s = text[i, text.length - i]
        pos = (/((on|onClipEvent)([\s\001]*)(\([\s\001]*)([a-zA-Z]+)([\s\001]*\))|on[\s\001]*\([\s\001]*keyPress[\s\001]*".*"[\s\001]*\)|\{\s*|\}\s*|\/\*\s*|\*\/\s*)/ =~ s)
        if pos
          i += pos
        else
          break
        end
        s = text[i, text.length - i]
        skip = 0
        case s
        when /^(on|onClipEvent)([\s\001]*)(\([\s\001]*)([a-zA-Z]+)([\s\001]*\))/
          skip += $1.length + $2.length + $3.length + $4.length + $5.length
          event = $4 if event_nest == 0
        when /^(on[\s\001]*\([\s\001]*keyPress[\s\001]*".*"[\s\001]*\))/
          skip += $1.length
          event = "keyPress"
        when /^(\{\s*)/
          skip += $1.length
          event_nest += 1
        when /^(\}\s*)/
          skip += $1.length
          event_nest -= 1
          if event_nest == 0
            scripts.each do |lang, script|
              if script =~ /\W/
                if event == "keyPress"
                  error "doesn't support script in keyPress event"
                else
                  script_name =
                    "#{name}_#{index}_#{instance_linkage_name}_#{instance_name}"
                  funcname = "#{script_name}_#{event}"
                  @instance_script_map[script_name] ||= Hash.new
                  @instance_script_map[script_name][event] ||= Hash.new
                  @instance_script_map[script_name][event] = funcname
                  @script_funcname_map[funcname] ||= {}
                  @script_funcname_map[funcname][lang] = script
                end
              end
            end
            event = nil
            scripts = {}
          end
        when /^(\/\*\s*)/
          skip += $1.length
          if script_index.nil?
            case s
            when /^(\/\*\s*)(js)(\s*\001|\s+)/i
              lang = :JavaScript
              script_index = i + $1.length + $2.length + $3.length
              skip += $2.length + $3.length
              script_nest = nest
            when /^(\/\*\s*)(lua)(\s*\001|\s+)/i
              lang = :Lua
              script_index = i + $1.length + $2.length + $3.length
              skip += $2.length + $3.length
              script_nest = nest
            end
          end
          nest += 1
        when /^(\*\/\s*)/
          skip += $1.length
          nest -= 1
          if !script_index.nil? and nest == script_nest
            tmp = text[script_index, i - script_index].sub(/\s*$/, '')
            tmp.sub!(/^[\s\001]*\001/, '')
            tmp.sub!(/[\s\001]+$/, '')
            tmp.gsub!(/\001/, "\n")
            scripts[lang] ||= ""
            scripts[lang] += tmp
            script_index = nil
          end
        end
        i += skip
      end
    end
  end
end

def decode_linkageName(body)
  line = body.split(/\n/)[0]
  line.force_encoding("UTF-8")
  if line =~ /DOMSymbolItem/
    line =~ /\s+name="([^"]+)"/
    name = $1
    line =~ /\s+linkageIdentifier="([^"]+)"/
    linkageIdentifier = $1
    @library_name_map[name] = linkageIdentifier if name and linkageIdentifier
  end
end

def parse_fla(lwfbasedir)
  if File.file?(@fla)
    root_xmls = []
    other_xmls = []

    Zip::ZipFile.foreach(@fla) do |entry|
      if entry.file?
        if entry.name == 'DOMDocument.xml'
          entry.get_input_stream do |io|
            root_xmls.push(io.read)
          end
        elsif entry.name =~ /^LIBRARY\/.*\.xml$/
          entry.get_input_stream do |io|
            body = io.read
            decode_linkageName(body)
            other_xmls.push(body)
          end
        end
      end
    end

    root_xmls.each do |xml|
      parse_xflxml(xml, true)
    end
    other_xmls.each do |xml|
      parse_xflxml(xml)
    end
  else
    xfldir = @fla

    xml = xfldir + "/DOMDocument.xml"
    unless File.file? xml
      error "can't read the fla"
      return
    end

    other_xmls = []
    Find.find(xfldir + "/LIBRARY") do |f|
      next unless f =~ /\.xml$/
      body = File.read(f)
      decode_linkageName(body)
      other_xmls.push(body)
    end

    parse_xflxml(File.read(xml), true)
    other_xmls.each do |xml|
      parse_xflxml(xml)
    end
  end
end

def check_texturename(name)
  if name =~ /(.*)_rgb_[0-9a-f]{6}(.*)/ or
      name =~ /(.*)_rgb_\d+,\d+,\d+(.*)/ or
      name =~ /(.*)_rgba_[0-9a-f]{8}(.*)/ or
      name =~ /(.*)_rgba_\d+,\d+,\d+,\d+(.*)/ or
      name =~ /(.*)_add_[0-9a-f]{6}(.*)/ or
      name =~ /(.*)_add_\d+,\d+,\d+(.*)/
    return $1 + $2
  else
    return nil
  end
end

def add_lwfbutton(button)
  conditionId = nil
  conditions = 0
  button.actions.each do |action|
    next if action[1].empty?
    conditionId ||= @data_buttonCondition.size
    @data_buttonCondition.push LWFButtonCondition.new(
      action[0], action[2], add_actions(action[1]))
    conditions += 1
  end

  lwfButton = LWFButton.new(
    button.width,
    button.height,
    add_matrix(button.matrix),
    add_colorTransform(button.color_transform),
    conditionId || 0,
    conditions)
  lwfButtonId = @map_button[lwfButton.to_a]
  if lwfButtonId.nil?
    lwfButtonId = @data_button.size
    @data_button.push lwfButton
    @map_button[lwfButton.to_a] = lwfButtonId
  end
  add_object(button, LWF_OBJECT_BUTTON, lwfButtonId)
end

def add_bitmap(texture, matrixId, u = nil, v = nil, width = nil, height = nil)
  unless texture.nil?
    if u.nil?
      u = 0
      v = 0
      width = texture.width
      height = texture.height
      matrixId = 0
    end
    textureFragmentId = @map_textureFragment[texture]
    if textureFragmentId.nil?
      textureFragmentId = @data_textureFragment.size
      @map_textureFragment[texture] = textureFragmentId
      @data_textureFragment.push LWFTextureFragment.new(texture)
    end
  end
  textureFragmentId = -1 if textureFragmentId.nil?

  if u != 0 or v != 0 or texture.nil? or
      width != texture.width or height != texture.height
    # unless texture.nil? or texture.filename.nil?
    #   warn "bitmap(#{texture.filename} " +
    #     "#{texture.width}x#{texture.height}) is specified UVWH" +
    #       "(#{u},#{v})-(#{width},#{height})."
    # end
    attribute = 0
    if texture
      case texture.filename
      when /_REPEAT_S_/
        attribute = 1
      when /_REPEAT_T_/
        attribute = 2
      when /_REPEAT_ST_/, /_REPEAT_TS_/
        attribute = 3
      end
      u = u.to_f / texture.width.to_f
      v = v.to_f / texture.height.to_f
      w = width.to_f / texture.width.to_f
      h = height.to_f / texture.height.to_f
    else
      u = 0
      v = 0
      w = 0
      h = 0
    end
    lwfBitmapEx = LWFBitmapEx.new(matrixId,
      textureFragmentId, attribute, u, v, w, h)
    lwfBitmapExId = @map_bitmapEx[lwfBitmapEx.to_a]
    if lwfBitmapExId.nil?
      lwfBitmapExId = @data_bitmapEx.size
      @data_bitmapEx.push lwfBitmapEx
      @map_bitmapEx[lwfBitmapEx.to_a] = lwfBitmapExId
    end
    info "  bitmapEx #{lwfBitmapExId} " +
      "textureFragmentId=#{textureFragmentId} " +
      "attribute=#{attribute} #{u},#{v},#{w},#{h}"
    return [LWF_GRAPHICOBJECT_BITMAPEX, lwfBitmapExId]
  else
    lwfBitmap = LWFBitmap.new(matrixId, textureFragmentId)
    lwfBitmapId = @map_bitmap[lwfBitmap.to_a]
    if lwfBitmapId.nil?
      lwfBitmapId = @data_bitmap.size
      @data_bitmap.push lwfBitmap
      @map_bitmap[lwfBitmap.to_a] = lwfBitmapId
    end
    info "  bitmap #{lwfBitmapId} textureFragmentId=#{textureFragmentId}"
    return [LWF_GRAPHICOBJECT_BITMAP, lwfBitmapId]
  end
end

def swf2lwf(*args)
  args.each do |arg|
    unless File.file?(arg)
      error "can't read #{arg}"
      return
    end
  end
  swffile = args.shift
  textureatlasfiles = args
  @lwfpath = swffile.sub(/\.swf$/i, '')
  lwfbasedir = @lwfpath + ".lwfdata/"
  lwfname = File.basename(@lwfpath)
  FileUtils.rm_rf lwfbasedir if File.directory?(lwfbasedir)
  FileUtils.mkdir_p lwfbasedir
  @script_map = Hash.new
  @instance_script_map = Hash.new
  @script_funcname_map = Hash.new
  @using_script_funcname_map = Hash.new
  @event_map = Hash.new
  @lwfpath = lwfbasedir + lwfname
  @logfile = File.open(@lwfpath + '.txt', 'wb')

  begin  # @logfile

  @logfile.sync = true
  @logfile.puts Time.now.ctime
  @option = 0
  @objects = Hash.new
  @button_map = Hash.new
  @instance_name_map = Hash.new
  @instance_name_map["_root"] = true
  @label_map = Hash.new
  @particle_name_map = Hash.new
  @program_object_name_map = Hash.new
  @movie_linkage_name_map = Hash.new
  @movie_linkage_name_map["_empty"] = true
  @fontname_map = Hash.new
  @constantpool_map = Hash.new
  @textures = Array.new
  @texture_filename_map = Hash.new
  @text_name_map = Hash.new
  @string_map = Hash.new
  @background_color = 0
  @library_name_map = Hash.new

  load_swf(swffile)
  parse_fla(lwfbasedir) unless @fla.nil?
  parse_swf()

  LWFObjects.each do |s|
    eval <<-EOF
      @data_#{s.to_s} = Array.new
      @map_#{s.to_s} = Hash.new
      @bytes_#{s} = String.new
  EOF
  end
  @actionBytes = ""
  @actions = Array.new
  @fonts = Hash.new
  @particles = Array.new
  @progs = Array.new
  @movie_linkages = Array.new

  @textureatlasdicts = Array.new
  textureatlasfiles.each do |textureatlasfile|
    # check TexturePacker JSON
    textureatlasdict = JSON.parse(File.read(textureatlasfile))
    if textureatlasdict.nil? or textureatlasdict["meta"].nil?
      error "can't read #{textureatlasfile}"
      next
    end

    frames = textureatlasdict["frames"]
    if frames.instance_of?(Array)
      newFrames = {}
      textureatlasdict["frames"] = newFrames
      frames.each do |frame|
        newFrames[frame["filename"]] = frame
      end
    end

    meta = textureatlasdict["meta"]
    size = meta["size"]
    textureatlas = Texture.new(size["w"].to_i, size["h"].to_i, nil)
    textureatlas.textureatlas = true
    textureatlas.filename = meta["image"]
    textureatlas.format =
      TEXTUREFORMAT_PREMULTIPLIEDALPHA if meta["preMultipliedAlpha"]
    textureatlas.scale = meta["scale"] ? meta["scale"].to_f : 1.0
    @textures.push textureatlas
    textureatlasdict["meta"]["texture"] = textureatlas
    @textureatlasdicts.push textureatlasdict
  end

  names = {}
  names[File.basename(@lwfpath, '.*')] = @lwfpath + '.lwf'
  texturenamemap = {}
  @textures.each_with_index do |texture, i|
    if texture.textureatlas == false and
        (texture.filename.nil? or @use_internal_png)
      texture.filename = @lwfpath + "_#{i}.png"
      texture.format = TEXTUREFORMAT_PREMULTIPLIEDALPHA
      texture.need_to_export = true
    end
    texture.name = File.basename(texture.filename)

    name = File.basename(texture.filename, '.*')
    warn "bitmap #{texture.filename} conflicts #{names[name]}" if names[name]
    names[name] = texture.filename

    origName = check_texturename(texture.name)
    unless origName.nil?
      tinfo = nil
      @textureatlasdicts.each do |textureatlasdict|
        tinfo = textureatlasdict["frames"][origName]
        if tinfo
          r = tinfo["rotated"]
          u = tinfo["frame"]["x"].to_i
          v = tinfo["frame"]["y"].to_i
          w = tinfo["frame"]["w"].to_i
          h = tinfo["frame"]["h"].to_i
          x = tinfo["spriteSourceSize"]["x"].to_i
          y = tinfo["spriteSourceSize"]["y"].to_i
          filename = textureatlasdict["meta"]["texture"].filename
          if w != texture.width or h != texture.height
            warn "Texture [#{texture.name}] size(#{texture.width}x#{texture.height}) is not same as TextureAtlas [#{filename}/#{origName}] size(#{w}x#{h})"
          end
          texture.name += "_atlas_#{filename}" +
            "_info_#{r ? 1 : 0}_#{u}_#{v}_#{w}_#{h}_#{x}_#{y}"
          break
        end
      end
    end
    @texture_filename_map[texture.name] = true
    texturenamemap[File.basename(texture.name, '.*').downcase] = true
  end

  @textures.each do |texture|
    basename = check_texturename(texture.name)
    unless basename.nil?
      unless basename =~ /\.[^\.]+_atlas_.+_info_.+(_.+){6}/
        name = File.basename(basename, '.*')
        if texturenamemap[name.downcase].nil?
          error "Texture [#{name}] not found for [#{texture.name}]"
        end
      end
    end
  end

  @strings = Hash.new
  @strings[lwfname] = true
  [@instance_name_map, @label_map, @event_map,
      @constantpool_map, @string_map, @fontname_map,
        @particle_name_map, @program_object_name_map,
          @movie_linkage_name_map, @texture_filename_map,
            @script_funcname_map].each do |m|
    m.keys.each do |key|
      @strings[key] = true
    end
  end
  i = 0
  @strings.keys.sort.each do |key|
    @strings[key] = i
    i += 1
  end
  @instance_name_map.keys.sort{|a,b| @strings[a] <=> @strings[b]}.each do |n|
    @map_instanceName[n] = @data_instanceName.size
    @data_instanceName.push LWFInstanceName.new(@strings[n])
  end
  @map_instanceName[nil] = -1
  @event_map.keys.sort.each do |n|
    @map_event[n] = @data_event.size
    @data_event.push LWFEvent.new(@strings[n])
  end

  add_matrix(Matrix.new)
  add_colorTransform(ColorTransform.new)
  @map_objectId = Hash.new
  @objects.keys.sort.each do |obj_id|
    obj = @objects[obj_id]
    next if obj.ref == 0
    case obj
    when Button
      button = obj

      case button.type
      when :PARTICLE
        unless button.actions.empty?
          error sprintf(
            "[_PARTICLE_%s] particle object is attached " +
              "a button script.", button.name)
        end

        particleDataId = @map_particleData[button.name]
        if particleDataId.nil?
          particleDataId = @data_particleData.size
          @map_particleData[button.name] = particleDataId
          @data_particleData.push LWFParticleData.new(@strings[button.name])
          @particles.push button.name
        end

        lwfParticle = LWFParticle.new(
          add_matrix(button.matrix),
          add_colorTransform(button.color_transform),
          particleDataId)
        lwfParticleId = @map_particle[lwfParticle.to_a]
        if lwfParticleId.nil?
          lwfParticleId = @data_particle.size
          @data_particle.push lwfParticle
          @map_particle[lwfParticle.to_a] = lwfParticleId
        end
        add_object(obj, LWF_OBJECT_PARTICLE, lwfParticleId)

      when :PROGRAMOBJECT
        unless button.actions.empty?
          error sprintf(
            "[_PROG_%s] program object is attached " +
              "a button script.", button.name)
        end

        nameStringId = @strings[button.name]
        lwfProgramObject = LWFProgramObject.new(
          button.width,
          button.height,
          add_matrix(button.matrix),
          add_colorTransform(button.color_transform),
          nameStringId)
        lwfProgramObjectId = @map_programObject[lwfProgramObject.to_a]
        if lwfProgramObjectId.nil?
          lwfProgramObjectId = @data_programObject.size
          @data_programObject.push lwfProgramObject
          @map_programObject[lwfProgramObject.to_a] = lwfProgramObjectId
          @progs.push button.name
        end
        add_object(obj, LWF_OBJECT_PROGRAMOBJECT, lwfProgramObjectId)

      else
        add_lwfbutton(button)
        unless button.script_actions.empty?
          button.script_actions.keys.each do |place|
            b = button.dup_by_key(place)
            @button_map[place] = b
            add_lwfbutton(b)
          end
        end
      end

    when Graphic
      gobjs = Array.new
      is_single_object = (obj.graphic_objects.size == 1)

      obj.graphic_objects.each do |gobj|
        type = nil
        gobjId = nil
        matrixId = add_matrix(gobj.matrix)

        case gobj
        when Bitmap
          texture = gobj.texture
          type, gobjId = add_bitmap(
            texture, matrixId, gobj.u, gobj.v, gobj.width, gobj.height)

        when Text
          if gobj.stroke_color.nil?
            strokeColorId = -1
          else
            strokeColor = LWFColor.new(gobj.stroke_color)
            strokeColorId = @map_color[strokeColor.to_a]
            if strokeColorId.nil?
              strokeColorId = @data_color.size
              @data_color.push strokeColor
              @map_color[strokeColor.to_a] = strokeColorId
            end
          end

          if gobj.shadow_color.nil?
            shadowColorId = -1
          else
            shadowColor = LWFColor.new(gobj.shadow_color)
            shadowColorId = @map_color[shadowColor.to_a]
            if shadowColorId.nil?
              shadowColorId = @data_color.size
              @data_color.push shadowColor
              @map_color[shadowColor.to_a] = shadowColorId
            end
          end

          textProperty = LWFTextProperty.new(gobj,
            gobj.font.font_id, strokeColorId, shadowColorId)

          textPropertyId = @map_textProperty[textProperty.to_a]
          if textPropertyId.nil?
            textPropertyId = @data_textProperty.size
            @data_textProperty.push textProperty
            @map_textProperty[textProperty.to_a] = textPropertyId
          end

          color = LWFColor.new(gobj.color)
          colorId = @map_color[color.to_a]
          if colorId.nil?
            colorId = @data_color.size
            @data_color.push color
            @map_color[color.to_a] = colorId
          end

          nameStringId = gobj.name.empty? ? -1 : @strings[gobj.name]
          stringId = @strings[gobj.text]

          lwfText = LWFText.new(matrixId, nameStringId,
            textPropertyId, stringId, colorId, gobj.width, gobj.height)
          lwfTextId = @map_text[lwfText.to_a]
          if lwfTextId.nil?
            lwfTextId = @data_text.size
            @data_text.push lwfText
            @map_text[lwfText.to_a] = lwfTextId
          end

          type = LWF_GRAPHICOBJECT_TEXT
          gobjId = lwfTextId
        end

        if is_single_object
          add_object(obj, GRAPHICOBJECT_CONVTABLE[type], gobjId)
        else
          gobjs.push LWFGraphicObject.new(type, gobjId)
        end
      end

      if !is_single_object
        gobjs_array = gobjs.map{|g| g.to_a}
        lwfGraphicId = @map_graphic[gobjs_array]
        if lwfGraphicId.nil?
          lwfGraphicId = @data_graphic.size
          @data_graphic.push LWFGraphic.new(
            @data_graphicObject.size, gobjs.size)
          @map_graphic[gobjs_array] = lwfGraphicId

          @data_graphicObject += gobjs
        end
        add_object(obj, LWF_OBJECT_GRAPHIC, lwfGraphicId)
      end

    when Movie
      label_offset = @data_label.size
      obj.labels.keys.sort{|a,b| @strings[a] <=> @strings[b]}.each do |label|
        @data_label.push LWFLabel.new(@strings[label], obj.labels[label])
      end

      frame_offset = @data_frame.size
      obj.frames.each_with_index do |controls, frame_index|
        lwfControls = Array.new
        lwfControlSrcs = Array.new
        lwfControlActions = Array.new
        lwfControlActionSrcs = Array.new
        controls.each do |control|
          case control
          when ControlMOVE
            place = control.place
            error "object error" if place.object.nil?

            button = @button_map[place]
            unless button.nil?
              lwfObjectId = @map_objectId[button]
            else
              lwfObjectId = @map_objectId[place.object]
            end
            if lwfObjectId.nil?
              if place.object.class == Texture
                type, gobjId =
                  add_bitmap(place.object, add_matrix(Matrix.new))
                lwfObjectId = add_object(
                  place.object, GRAPHICOBJECT_CONVTABLE[type], gobjId)
              else
                lwfObjectId = -1
                error "object error"
              end
            end

            lwfPlace = LWFControlPLACE.new(place.depth - 1, lwfObjectId,
              @map_instanceName[place.instance_name],
                add_matrix(place.matrix), place.blend_mode)

            lwfPlaceId = @map_place[lwfPlace.to_a]
            if lwfPlaceId.nil?
              lwfPlaceId = @data_place.size
              @data_place.push lwfPlace
              @map_place[lwfPlace.to_a] = lwfPlaceId
            end

            if control.matrix == place.matrix
              if control.color_transform.nil?
                lwfControlSrcs.push control
                lwfControls.push LWFControl.new(
                  LWF_CONTROL_MOVE, lwfPlaceId)
              else
                lwfControlMoveC = LWFControlMOVEC.new(
                  lwfPlaceId, add_colorTransform(control.color_transform))
                lwfControlMoveCId = @map_controlMoveC[lwfControlMoveC.to_a]
                if lwfControlMoveCId.nil?
                  lwfControlMoveCId = @data_controlMoveC.size
                  @data_controlMoveC.push lwfControlMoveC
                  @map_controlMoveC[lwfControlMoveC.to_a] = lwfControlMoveCId
                end
                lwfControlSrcs.push control
                lwfControls.push LWFControl.new(
                  LWF_CONTROL_MOVEC, lwfControlMoveCId)
              end
            else
              if control.color_transform.nil?
                lwfControlMoveM = LWFControlMOVEM.new(
                  lwfPlaceId, add_matrix(control.matrix))
                lwfControlMoveMId = @map_controlMoveM[lwfControlMoveM.to_a]
                if lwfControlMoveMId.nil?
                  lwfControlMoveMId = @data_controlMoveM.size
                  @data_controlMoveM.push lwfControlMoveM
                  @map_controlMoveM[lwfControlMoveM.to_a] = lwfControlMoveMId
                end
                lwfControlSrcs.push control
                lwfControls.push LWFControl.new(
                  LWF_CONTROL_MOVEM, lwfControlMoveMId)
              else
                lwfControlMoveMC = LWFControlMOVEMC.new(
                  lwfPlaceId, add_matrix(control.matrix),
                  add_colorTransform(control.color_transform))
                lwfControlMoveMCId = @map_controlMoveMC[lwfControlMoveMC.to_a]
                if lwfControlMoveMCId.nil?
                  lwfControlMoveMCId = @data_controlMoveMC.size
                  @data_controlMoveMC.push lwfControlMoveMC
                  @map_controlMoveMC[
                    lwfControlMoveMC.to_a] = lwfControlMoveMCId
                end
                lwfControlSrcs.push control
                lwfControls.push LWFControl.new(
                  LWF_CONTROL_MOVEMC, lwfControlMoveMCId)
              end
            end

          when ControlACTION
            unless control.actions.empty?
              lwfControlActionSrcs.push control
              lwfControlActions.push LWFControl.new(
                  LWF_CONTROL_ACTION, add_actions(control.actions))
            end
          end
        end

        if @script_map[obj.linkage_name]
          scripts = @script_map[obj.linkage_name][frame_index]
          if scripts
            scripts.sort{|a,b| a[0] <=> b[0]}.each do |o|
              funcname = o[1]
              as = @script_funcname_map[funcname][:ActionScript]
              unless as.nil?
                control = ControlACTION.new(as)
                lwfControlActionSrcs.push control
                lwfControlActions.push LWFControl.new(
                    LWF_CONTROL_ACTION, add_actions(control.actions))
                @script_funcname_map[funcname].delete(:ActionScript)
              end
              if @script_funcname_map[funcname].empty?
                @script_funcname_map.delete(funcname)
              else
                @using_script_funcname_map[funcname] =
                  @script_funcname_map[funcname]
                control = ControlACTION.new([[:CALL, funcname]])
                lwfControlActionSrcs.push control
                lwfControlActions.push LWFControl.new(
                    LWF_CONTROL_ACTION, add_actions(control.actions))
              end
            end
          end
        end

        lwfControlSrcs += lwfControlActionSrcs
        lwfControls += lwfControlActions

        lwfControls_array = lwfControls.map{|c| c.to_a}
        lwfFrame = @map_frame[lwfControls_array]
        if lwfFrame.nil?
          control_offset = @data_control.size
          lwfControls.each_index do |i|
            control = lwfControlSrcs[i]
            lwfControl = lwfControls[i]
            @map_control[control] = @data_control.size
            @data_control.push lwfControl
          end
          lwfFrame = LWFFrame.new(control_offset, lwfControls.size)
          @map_frame[lwfControls_array] = lwfFrame
        else
          lwfControls.each_index do |i|
            control = lwfControlSrcs[i]
            @map_control[control] = lwfFrame.controlOffset + i
          end
        end
        @data_frame.push lwfFrame
      end

      clipEventId = nil
      clipEvents = 0
      obj.actions.each do |action|
        next if action[1].empty?
        clipEventId ||= @data_movieClipEvent.size
        @data_movieClipEvent.push LWFMovieClipEvent.new(
          action[0], add_actions(action[1]))
        clipEvents += 1
      end

      lwfMovie = LWFMovie.new(obj.depth_max, label_offset,
        obj.labels.size, frame_offset, obj.frames.size, obj.linkage_name,
        clipEventId || 0, clipEvents)
      lwfMovieId = @map_movie[lwfMovie.to_a]
      if lwfMovieId.nil?
        lwfMovieId = @data_movie.size
        @data_movie.push lwfMovie
        @map_movie[lwfMovie.to_a] = lwfMovieId
      end
      add_object(obj, LWF_OBJECT_MOVIE, lwfMovieId)
      @root_movie_id = lwfMovieId if obj == @root_movie

    when Font
      lwfFont = LWFFont.new(@strings[obj.name], obj)
      lwfFontId = @map_font[lwfFont.to_a]
      if lwfFontId.nil?
        lwfFontId = @data_font.size
        @data_font.push lwfFont
        @map_font[lwfFont.to_a] = lwfFontId
      end
      obj.font_id = lwfFontId
      @fonts[lwfFontId] ||= obj
    end
  end

  movie_linkages_tmp = Hash.new
  @data_movie.each_with_index do |lwfMovie, i|
    unless lwfMovie.linkage_name.nil?
      stringId = @strings[lwfMovie.linkage_name]
      movie_linkages_tmp[stringId] = [lwfMovie, i]
    end
  end
  movie_linkages_tmp.keys.sort.each do |stringId|
    v = movie_linkages_tmp[stringId]
    lwfMovie = v[0]
    lwfMovieId = v[1]
    @data_movieLinkage.push LWFMovieLinkage.new(stringId, lwfMovieId)
    @movie_linkages.push(lwfMovie.linkage_name + ": " + lwfMovieId.to_s)
  end

  @data_textureFragment.each do |lwfTextureFragment|
    texture = lwfTextureFragment.texture
    filename = check_texturename(texture.filename)
    if filename
      coloredFragment = true
    else
      filename = texture.filename
      coloredFragment = false
    end

    tinfo = nil
    textureatlas = nil
    @textureatlasdicts.each do |textureatlasdict|
      tinfo = textureatlasdict["frames"][File.basename(filename)]
      textureatlas = textureatlasdict["meta"]["texture"]
      if tinfo and !coloredFragment
        textureatlasId = textureatlasdict["meta"]["textureId"]
        if textureatlasId.nil?
          textureatlas.ref
          textureatlasId = @data_texture.size
          @data_texture.push LWFTexture.new(textureatlas.name,
            @strings[textureatlas.name], textureatlas.format,
            textureatlas.width, textureatlas.height, textureatlas.scale)
          textureatlasdict["meta"]["textureId"] = textureatlasId
        end

        texture.clear_reference
        u = tinfo["frame"]["x"].to_i
        v = tinfo["frame"]["y"].to_i
        w = tinfo["frame"]["w"].to_i
        h = tinfo["frame"]["h"].to_i
        x = tinfo["spriteSourceSize"]["x"].to_i
        y = tinfo["spriteSourceSize"]["y"].to_i
        lwfTextureFragment.set(@strings[texture.name],
          textureatlasId, tinfo["rotated"], x, y, u, v, w, h, textureatlas)
        @option |= OPTION_USE_TEXTUREATLAS
        break
      end
    end
    next if tinfo and !coloredFragment

    if tinfo
      w = tinfo["frame"]["w"].to_i
      h = tinfo["frame"]["h"].to_i
      x = tinfo["spriteSourceSize"]["x"].to_i
      y = tinfo["spriteSourceSize"]["y"].to_i
      texture.width = w
      texture.height = h
      texture.scale = textureatlas.scale
    else
      w = texture.width
      h = texture.height
      x = 0
      y = 0
    end
    textureId = @map_texture[texture]
    if textureId.nil?
      textureId = @data_texture.size
      @map_texture[texture] = textureId
      @data_texture.push LWFTexture.new(texture.name, @strings[texture.name],
        texture.format, w, h, texture.scale)
    end
    lwfTextureFragment.set(@strings[texture.name],
      textureId, false, x, y, 0, 0, w, h)
  end

  @option |= OPTION_USE_SCRIPT unless @using_script_funcname_map.empty?
  @using_script_funcname_map.keys.each do |key|
    @script_funcname_map.delete(key)
  end
  @script_funcname_map.keys.each do |key|
    warn("script not used [#{key}]")
  end

  @offset = LWF_HEADER_SIZE

  @stringBytesOffset = @offset
  @stringBytes = ""
  @strings.keys.sort{|a,b| @strings[a] <=> @strings[b]}.each do |str|
    @data_string.push LWFString.new(@stringBytes.bytesize, str.bytesize)
    @stringBytes += [str, 0].pack('a*C')
  end
  psize = ((@stringBytes.size + 3) & ~3) - @stringBytes.size
  @stringBytes += [].pack("x#{psize}") if psize != 0
  @offset += @stringBytes.size

  @actionBytesOffset = @offset
  psize = ((@actionBytes.size + 3) & ~3) - @actionBytes.size
  @actionBytes += [].pack("x#{psize}") if psize != 0
  @offset += @actionBytes.size

  lwf_format_type = 0

  @header = to_u8('L'[0].ord) + to_u8('W'[0].ord) + to_u8('F'[0].ord) +
    to_u8(lwf_format_type)
  if @blend_mode_used
    @header += to_u8(LWF_FORMAT_VERSION_0) +
      to_u8(LWF_FORMAT_VERSION_1) + to_u8(LWF_FORMAT_VERSION_2)
  else
    @header += to_u8(LWF_FORMAT_VERSION_COMPAT_0) +
      to_u8(LWF_FORMAT_VERSION_COMPAT_1) + to_u8(LWF_FORMAT_VERSION_COMPAT_2)
  end
  @header += to_u8(@option)

  [
    @stage.x_max - @stage.x_min,
    @stage.y_max - @stage.y_min,
    @frame_rate,
    @root_movie_id,
    @strings[lwfname],
    @background_color,
    @stringBytesOffset,
    @stringBytes.size,
    @actionBytesOffset,
    @actionBytes.size,
  ].each{|v| @header += to_u32(v)}

  @stats = ""
  @stats += sprintf("option: %x\n", @option)
  @stats += sprintf("stage: %dx%d\n",
    @stage.x_max - @stage.x_min, @stage.y_max - @stage.y_min)
  @stats += sprintf("frame_rate: %d\n", @frame_rate)
  @stats += sprintf("background_color: %08x\n", @background_color)
  @stats += sprintf("root: %d\n", @root_movie_id)

  @data = @stringBytes + @actionBytes
  LWFObjects.each do |s|
    eval <<-EOF
      @header += to_u32(@offset) + to_u32(@data_#{s}.size)
      arr = Array.new
      @data_#{s}.each{|o| arr.push(o.to_bytes)}
      @bytes_#{s} = arr.join('')
      @offset += @bytes_#{s}.size
      @data += @bytes_#{s}
      @stats += sprintf("#{s}: %d\n", @data_#{s}.size)
  EOF
  end

  error "LWF_HEADER_SIZE ERROR" if @header.size + 4 != LWF_HEADER_SIZE

  f = File.open(@lwfpath + ".lwf", "wb")
  f.write @header
  f.write to_u32(LWF_HEADER_SIZE + @data.size)
  f.write @data
  f.close

  f = File.open(@lwfpath + ".textures", "wb")
  @data_texture.each do |lwfTexture|
    f.puts lwfTexture.name
  end
  f.close

  f = File.open(@lwfpath + ".fragments", "wb")
  @data_textureFragment.each do |lwfTextureFragment|
    f.puts lwfTextureFragment.dump
  end
  f.close

  f = File.open(@lwfpath + ".fonts", "wb")
  @fonts.keys.sort.each do |key|
    font = @fonts[key]
    f.puts "#{key}: [#{font.name}, #{font.orgname}, #{font.letterspacing}]"
  end
  f.close

  f = File.open(@lwfpath + ".events", "wb")
  @event_map.keys.sort.each do |key|
    f.puts "#{key}"
  end
  f.close

  f = File.open(@lwfpath + ".particles", "wb")
  @particles.each do |entry|
    f.puts "#{entry}"
  end
  f.close

  f = File.open(@lwfpath + ".progs", "wb")
  @progs.each do |entry|
    f.puts "#{entry}"
  end
  f.close

  f = File.open(@lwfpath + ".texts", "wb")
  @text_name_map.keys.sort.each do |key|
    f.puts "#{key}"
  end
  f.close

  f = File.open(@lwfpath + ".linkages", "wb")
  @movie_linkages.each do |entry|
    f.puts "#{entry}"
  end
  f.close

  f = File.open(@lwfpath + ".instances", "wb")
  @instance_name_map.keys.sort.each do |key|
    f.puts "#{key}"
  end
  f.close

  f = File.open(@lwfpath + ".labels", "wb")
  @label_map.keys.sort.each do |key|
    f.puts "#{key}"
  end
  f.close

  f = File.open(@lwfpath + ".stats", "wb")
  f.write @stats
  f.close

  unless @using_script_funcname_map.empty?
    js = File.open(@lwfpath + ".js", "wb")
    js.write <<-EOL
global.LWF.Script = global.LWF.Script || {};
global.LWF.Script["#{lwfname}"] = function() {
	var LWF = global.LWF.LWF;
	var Loader = global.LWF.Loader;
	var Movie = global.LWF.Movie;
	var Property = global.LWF.Property;
	var Point = global.LWF.Point;
	var Matrix = global.LWF.Matrix;
	var Color = global.LWF.Color;
	var ColorTransform = global.LWF.ColorTransform;
	var Tween = global.LWF.Tween;
	var _root;

	var fscommand = function(type, arg) {
		if (type === "event") {
			_root.lwf.dispatchEvent(arg, this);
		} else {
			throw Error("unknown fscommand");
		}
	};

	var trace = function(msg) {
		console.log(msg);
	};

	var Script = (function() {function Script() {}

	Script.prototype["init"] = function() {
		var movie = this;
		while (movie.parent !== null)
			movie = movie.parent.lwf.rootMovie;
		_root = movie;
	};

	Script.prototype["destroy"] = function() {
		_root = null;
	};
    EOL

    lua = File.open(@lwfpath + ".lua", "wb")
    lua.write <<-EOL
if not LWF then LWF={} end
if not LWF.Script then LWF.Script={} end
if not LWF.Script.#{lwfname} then LWF.Script.#{lwfname}={} end
local _root
    EOL

    @using_script_funcname_map["_root_load"] ||= {}
    @using_script_funcname_map["_root_load"][:Lua] ||= ""

    @using_script_funcname_map.sort{|a,b| a <=> b}.each do |k, scripts|
      scripts.each do |lang, script|
        case lang
        when :JavaScript
          js.write <<-EOL

	Script.prototype["#{k}"] = function() {
          EOL
          offset = 0
          RKelly::Tokenizer.new.raw_tokens(script).each do |token|
            next if token.name != :RAW_IDENT and token.name != :SINGLE_CHAR
            case token.value
            when /^@([a-zA-Z_])/
              script[token.offset + offset, 2] = "this.#{$1}"
              offset += 4
            when /^@\[/
              script[token.offset + offset, 2] = "this["
              offset += 3
            when /^@$/
              script[token.offset + offset, 1] = "this"
              offset += 3
            when /^\$([a-zA-Z_])/
              script[token.offset + offset, 2] = "_root.#{$1}"
              offset += 5
            when /^\$\[/
              script[token.offset + offset, 2] = "_root["
              offset += 4
            when /^\$$/
              script[token.offset + offset, 1] = "_root"
              offset += 4
            end
          end
          js.write script.gsub(/^/, "\t\t")
          js.write "\n"
          js.write <<-EOL
	};
        EOL

        when :Lua
          lua.write <<-EOL

LWF.Script.#{lwfname}.#{k} = function(self)
          EOL
          if k == "_root_load"
            lua.write <<-EOL
	_root = self
            EOL
          end
          lua.write script.gsub(/^/, "\t")
          lua.write "\n"
          lua.write <<-EOL
end
        EOL
        end
      end
    end
  
    js.write <<-EOL

	return Script;

	})();

	return new Script();
};
    EOL
    js.close
  
    lua.write <<-EOL
    EOL
    lua.close
  end

  unless @disable_exporting_png
    @textures.map{|texture| texture.export_png(@swf)}
  end

  ensure  # @logfile
  @logfile.close
  end  # @logfile

end

def swf2lwf_optparse(args)
  use_conf = false
  conf_path = File.dirname(__FILE__) +
    '/' + File.basename(__FILE__, '.*') + '.conf'
  @ignore_unknownaction = false
  @use_fixed_point = false
  @use_internal_png = false
  @disable_exporting_png = false
  @fla = nil

  OptionParser.new do |opt|
    opt.banner += ' SWF [TexturePackerJSONFiles]'
    opt.on('-c CONF', desc = 'specify a CONF file.') do |conf|
      use_conf = true
      conf_path = conf
    end
    opt.on('-i', desc = 'suppress warnings for unknown actions.') {@ignore_unknownaction = true}
    opt.on('-p', desc = 'extract image data from the SWF file.') {@use_internal_png = true}
    opt.on('-k', desc = 'disable extracting image data.') {@disable_exporting_png = true}
    opt.on('-f FLA', desc = 'specify an FLA file corresponding to the SWF file.') do |fla|
      @fla = fla
    end
    opt.parse!(args.empty? ? ['--help'] : args)
  end

  @font_table = Hash.new
  @format_version = LWF_FORMAT_VERSION
  @blend_mode_used = false
  begin
    config = YAML::load(File.read(conf_path))
    @font_table = config['font']
    format_version = config['format']
    case format_version
    when nil, LWF_FORMAT_VERSION, sprintf("0x%06x", LWF_FORMAT_VERSION)
      @format_version = LWF_FORMAT_VERSION
    when LWF_FORMAT_VERSION_COMPAT, sprintf("0x%06x", LWF_FORMAT_VERSION_COMPAT)
      @format_version = LWF_FORMAT_VERSION_COMPAT
    else
      error "invalid format \"#{format_version}\" in conf file #{conf_path}"
    end
  rescue
    error "can't load conf file #{conf_path}" if use_conf
  end

  swf2lwf_setfuncs
end

if $0 == __FILE__
  swf2lwf_optparse(ARGV)
  swf2lwf(*ARGV)
end
