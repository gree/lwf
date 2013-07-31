#include "compile.h"
#include <unistd.h>
#include <stdlib.h>
#include <fcntl.h>
#include "ruby.h"

static VALUE rb_mActionCompiler;

static VALUE version(VALUE self)
{
    return rb_str_new2("1.0.0");
}

static VALUE compile(VALUE self, VALUE string)
{
    Check_Type(string, T_STRING);

    Buffer b;
    swf4ParseInit(RSTRING_PTR(string), 0, 7);
    int parserError = swf4parse((void *)&b);
    VALUE result = rb_str_new(b->buffer, bufferLength(b));

    return result;
}

void Init_actioncompiler()
{
    rb_mActionCompiler = rb_define_module("ActionCompiler");
    rb_define_module_function(rb_mActionCompiler, "version", version, 0);
    rb_define_module_function(rb_mActionCompiler, "compile", compile, 1);
}
