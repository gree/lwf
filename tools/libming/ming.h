#ifndef SWF_MING_H_INCLUDED
#define SWF_MING_H_INCLUDED

typedef unsigned char byte;

#include <stdarg.h>
#include <stdio.h>
#ifdef __MINGW32__
#include <stdlib.h>

static inline void warn(const char *fmt, ...)
{
    va_list ap;
    va_start(ap, fmt);
    vfprintf(stderr, fmt, ap);
    va_end(ap);
}
#else
#include <err.h>
#endif

extern char swf4ErrorBuffer[1024];
static inline void SWF_error(const char *fmt, ...)
{
    va_list ap;
    va_start(ap, fmt);
    vsnprintf(&swf4ErrorBuffer[7], sizeof(swf4ErrorBuffer) - 7, fmt, ap);
    va_end(ap);
    memcpy(&swf4ErrorBuffer[0], "ERROR: ", 7);
}

#define SWF_warn warn

#endif
