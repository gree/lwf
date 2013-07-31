require 'mkmf'
name = 'actioncompiler'
case RUBY_PLATFORM
when /darwin/
  CONFIG['CC'] = "MACOSX_DEPLOYMENT_TARGET=10.7 " + CONFIG['CC']
end
$objs = %W|
  assembler.o
  lex.swf4.o
  swf4compiler.tab.o
  compile.o
  main.o
|
dir_config(name)
create_makefile(name)

f = File.open('Makefile', 'a')
f.write <<EOL

LEX = flex
YACC = bison -y
CLEANFILES += lex.swf4.c swf4compiler.tab.c swf4compiler.tab.h Makefile

lex.swf4.c: swf4compiler.flex swf4compiler.tab.h
	$(LEX) -Pswf4 swf4compiler.flex

swf4compiler.tab.c: swf4compiler.y
	$(YACC) -p swf4 -b swf4compiler swf4compiler.y

swf4compiler.tab.h: swf4compiler.y
	$(YACC) --defines -p swf4 -b swf4compiler swf4compiler.y
EOL
f.close
