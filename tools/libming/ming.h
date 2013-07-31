#ifndef SWF_MING_H_INCLUDED
#define SWF_MING_H_INCLUDED

typedef unsigned char byte;

#ifdef __MINGW32__
#include <stdarg.h>
#include <stdio.h>
#include <stdlib.h>

static inline void err(int eval, const char *fmt, ...)
{
    va_list ap;
    va_start(ap, fmt);
    vfprintf(stderr, fmt, ap);
    va_end(ap);
    exit(eval);
}

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

#define SWF_error(...) err(1, __VA_ARGS__)
#define SWF_warn warn

#endif
