/*
 * Copyright (C) 2013 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
/*
 * Changes by IG Dev:
 * 1. Added gettimeofday from sys/time.h which is missing in win32.
 * 2. Added win32 implementation for FreeBSD specific extension: strcasestr.
 */

#ifndef LWF_COMPAT_H
#define LWF_COMPAT_H

#if (__cplusplus > 199711L) || (_MSC_VER >= 1800)
# define scoped_ptr unique_ptr
#endif

#if !defined(_MSC_VER)

// for gettimeofday
# include <sys/time.h>
// for strcasestr
# include <cstring>

#else

# include <time.h>
# include <winsock2.h>

#define strncasecmp _strnicmp

// WIN32 implementation for missing stuff
int gettimeofday(struct timeval *tp, struct timezone *tzp);
char *strcasestr(const char *strA, const char *strB);

#endif // _MSC_VER

#endif
