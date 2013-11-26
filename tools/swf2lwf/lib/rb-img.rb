libdir = File.dirname(__FILE__)
$:.unshift(libdir) unless
  $:.include?(libdir) || $:.include?(File.expand_path(libdir))

begin
  require 'img.so'
rescue LoadError
  RUBY_VERSION =~ /^(\d+\.\d+)\./
  version = $1
  platform = RUBY_PLATFORM
  platform = $1 if platform =~ /(.*darwin).*/
  platformdir = libdir + '/' + version + '/' + platform + '/'
  begin
    require platformdir + 'img.so'
  rescue LoadError
  end
end

module Img
  VERSION = '0.0.6'
end
