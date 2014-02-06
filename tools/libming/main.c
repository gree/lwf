#include "compile.h"
#include <unistd.h>
#include <stdlib.h>
#include <fcntl.h>
#include "ruby.h"

char swf4ErrorBuffer[1024];

static VALUE rb_mActionCompiler;

static VALUE version(VALUE self)
{
    return rb_str_new2("1.0.2");
}

static VALUE compile(VALUE self, VALUE string)
{
    Check_Type(string, T_STRING);

    Buffer b;
    swf4ParseInit(RSTRING_PTR(string), 0, 7);
    swf4ErrorBuffer[0] = '\0';
    int parserError = swf4parse((void *)&b);
    VALUE result;
    if (parserError == 0)
        result = rb_str_new(b->buffer, bufferLength(b));
    else
        result = rb_str_new(swf4ErrorBuffer, strlen(swf4ErrorBuffer));

    return result;
}

void Init_actioncompiler()
{
    rb_mActionCompiler = rb_define_module("ActionCompiler");
    rb_define_module_function(rb_mActionCompiler, "version", version, 0);
    rb_define_module_function(rb_mActionCompiler, "compile", compile, 1);
}
