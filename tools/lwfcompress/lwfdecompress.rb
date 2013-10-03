#!/usr/bin/env ruby
require 'fileutils'
begin
  require 'lzma'
rescue
  puts "can't find lzma. gem install ruby-lzma"
end

src = ARGV[0]
dst = ARGV[1]

f = File.open(src, "rb")
d = f.read
d.force_encoding("ASCII-8BIT")
f.close

HEADER_LENGTH = 324

if (d[7].ord & (1 << 2)) == 0
  FileUtils.cp src, dst
  exit 0
end

d[7] = (d[7].ord & ~(1 << 2)).chr
length = d[HEADER_LENGTH - 4, 4].unpack("i").pop
compressed = d[HEADER_LENGTH, d.length - HEADER_LENGTH]

if compressed[5, 4].unpack("i").pop != length - HEADER_LENGTH
  puts "ERROR #{compressed[5, 4].unpack("i").pop} #{length - HEADER_LENGTH}"
  exit 1
end

f = File.open(dst, "wb")
f.write d[0, HEADER_LENGTH]
f.write LZMA.decompress(compressed)
f.close

if File.size(dst) != length
  puts "ERROR"
  exit 1
end
