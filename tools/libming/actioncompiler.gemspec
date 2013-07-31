Gem::Specification.new do |s|
  s.name        = 'actioncompiler'
  s.version     = '1.0.0'
  s.date        = '2013-07-30'
  s.summary     = "SWF Version 3 Action Compiler"
  s.description = s.summary
  s.authors     = ["libming Authors"]
  s.email       = 'sakamoto@splhack.org'
  s.files       = Dir.glob('*.{c,h,flex,y}')
  s.extensions  = ['extconf.rb']
  s.homepage    = 'http://www.libming.org/'
end
