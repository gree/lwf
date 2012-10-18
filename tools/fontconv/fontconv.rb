#!/usr/local/bin/macruby
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
framework 'Cocoa'

font_path = ARGV[0]
font_name = ARGV[1]
font_size = ARGV[2].to_i * 96 / 72
font_list = ARGV[3]
sheet_width = ARGV[4].to_i
sheet_height = ARGV[5].to_i
out_name = ARGV[6]

list = {}
begin
  f = File.open(font_list, "rb")
  while f.gets
    $_.each_char do |c|
      list[c] = true
    end
  end
rescue
end

url = NSURL.fileURLWithPath(font_path)
CTFontManagerRegisterFontsForURL(url, KCTFontManagerScopeProcess, nil)
attr = NSDictionary.dictionaryWithObjectsAndKeys(
  NSArray.alloc.init, NSFontCascadeListAttribute,
  font_name, NSFontNameAttribute,
  nil)
desc = NSFontDescriptor.fontDescriptorWithFontAttributes(attr)
font = NSFont.fontWithDescriptor(desc, size:font_size)
font_ascent = font.ascender.round
cmap = font.coveredCharacterSet

attr = NSDictionary.dictionaryWithObjectsAndKeys(
  font, NSFontAttributeName,
  NSColor.whiteColor, NSForegroundColorAttributeName,
  NSColor.clearColor, NSBackgroundColorAttributeName,
  nil)

img = NSImage.alloc.initWithSize(NSSize.new(sheet_width, sheet_height))
img.lockFocus
img.unlockFocus
sheet = NSBitmapImageRep.imageRepWithData(img.TIFFRepresentation)
sheet_x = 1
sheet_y = 1
max_height = 0
w = font_size * 2
h = font_size * 2

class Metric
  attr_accessor :character, :first, :second,
    :u, :v, :width, :height, :bearingX, :bearingY, :advance,
    :prevNum, :nextNum
  def initialize(character,
      first, second, u, v, w, h, bearingX, bearingY, advance)
    @character = character
    @first = first
    @second = second
    @u = u
    @v = v
    @width = w
    @height = h
    @bearingX = bearingX
    @bearingY = bearingY
    @advance = advance
  end

  def pack
    [@advance, @u, @v, @bearingX, @bearingY,
     @width, @height, @first, @second, @prevNum, @nextNum].pack("ev2c2C6")
  end
end

indecies = []
metrics = []

256.times do |first|
  indecies[first] = metrics.size
  first_entry = -1

  metrics_with_second = []
  256.times do |second|
    c = (first << 8) | second
    next unless cmap.characterIsMember(c)

    string = NSString.alloc.initWithCharacters([c], length:1)
    next if string.empty?
    next if !list.empty? and !list[string]

    textStorage = NSTextStorage.alloc.initWithString(string, attributes:attr)

    layoutManager = NSLayoutManager.alloc.init
    textStorage.addLayoutManager(layoutManager)
    numberOfGlyphs = layoutManager.numberOfGlyphs
    glyphs = NSMutableData.dataWithLength(4 * numberOfGlyphs)
    layoutManager.getGlyphs(glyphs.mutableBytes, range:NSMakeRange(0, numberOfGlyphs))
    glyph = (glyphs.bytes[3] << 24) | 
      (glyphs.bytes[2] << 16) |
      (glyphs.bytes[1] <<  8) |
      (glyphs.bytes[0] <<  0)
    advance = font.advancementForGlyph(glyph).width / font_size

    img = NSImage.alloc.initWithSize(NSSize.new(w, h))
    img.lockFocus
    textStorage.drawWithRect(
      NSRect.new(NSPoint.new(0, font_size), NSZeroSize), options:0)
    img.unlockFocus
    rep = NSBitmapImageRep.imageRepWithData(img.TIFFRepresentation)

    available = false
    bottom = 0
    top = h
    right = 0
    left = w
    h.times do |y|
      w.times do |x|
        a = rep.colorAtX(x, y:y).alphaComponent
        if a != 0
          available = true
          top = y if y < top
          bottom = y if y > bottom
          left = x if x < left
          right = x if x > right
        end
      end
    end
    next unless available 

    tw = right - left + 2
    th = bottom - top + 2
    if sheet_x + tw >= sheet_width
      sheet_x = 1
      sheet_y += max_height
      max_height = 0
    end
    if th > max_height
      max_height = th
    end
    if sheet_y + th >= sheet_height
      puts "ERROR: exceeded"
      exit 1
    end

    u = sheet_x
    v = sheet_y

    sy = 0
    (top..bottom).each do |y|
      sx = 0
      (left..right).each do |x|
        a = rep.colorAtX(x, y:y).alphaComponent
        color = NSColor.colorWithCalibratedRed(1, green:1, blue:1, alpha:a)
        sheet.setColor(color, atX:sheet_x + sx, y:sheet_y + sy)
        sx += 1
      end
      sy += 1
    end

    sheet_x += tw

    first_entry = second if first_entry == -1

    metric = Metric.new(
      string,
      first,
      second,
      u,
      v,
      right - left + 1,
      bottom - top + 1,
      left,
      font_size - top,
      advance)
    metrics_with_second.push metrics.size
    metrics.push metric
  end

  if first_entry == -1
    indecies[first] = -0x7fff
  else
    indecies[first] -= first_entry
  end
  puts sprintf("%02x: %d", first, indecies[first])
  metrics_with_second.each_with_index do |n, i|
    index = metrics[n]
    index.prevNum = i
    index.nextNum = metrics_with_second.size - i - 1
    puts sprintf(" %02x: %02x: [%s] prev=%d next=%d u=%f v=%f w=%d h=%d x=%d y=%d",
      i, index.second, index.character, index.prevNum, index.nextNum,
      index.u, index.v, index.width, index.height,
      index.bearingX, index.bearingY)
  end
end

rep = NSBitmapImageRep.imageRepWithData(sheet.TIFFRepresentation)
data = rep.representationUsingType(NSPNGFileType, properties:nil)
texture_name = out_name + "_texture"
data.writeToFile(texture_name + ".png", atomically:false)

f = File.open(out_name + ".bytes", "wb")
f.write([font_size, font_ascent, metrics.size, sheet_width, sheet_height].pack("v*"))
indecies.each do |index|
  f.write([index].pack("v"))
end
metrics.each do |metric|
  f.write(metric.pack)
end
f.write([texture_name].pack("a*x"))
f.close
