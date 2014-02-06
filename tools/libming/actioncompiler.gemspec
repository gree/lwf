Gem::Specification.new do |s|
  s.name        = 'actioncompiler'
  s.version     = '1.0.2'
  s.date        = '2014-01-28'
  s.summary     = "SWF Version 3 Action Compiler"
  s.description = s.summary
  s.authors     = ["libming Authors"]
  s.email       = 'sakamoto@splhack.org'
  s.files       = Dir.glob('*.{c,h,flex,y}')
  s.extensions  = ['extconf.rb']
  s.homepage    = 'http://www.libming.org/'
  s.license     = 'LGPL-2.1'
  s.files = [
    'LICENSE',
    'README',
    'actioncompiler.gemspec',
    'actiontypes.h',
    'assembler.c',
    'assembler.h',
    'compile.c',
    'compile.h',
    'extconf.rb',
    'main.c',
    'ming.h',
    'swf4compiler.flex',
    'swf4compiler.y'
  ]
end
