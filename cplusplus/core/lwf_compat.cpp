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

#include "lwf_compat.h"

#if defined(_MSC_VER)

# include <cstdint>
# include <cstring>
# include <cctype>

int gettimeofday(struct timeval *tp, struct timezone *tzp)
{
	// Note: some broken versions only have 8 trailing zero's,
	//       the correct epoch has 9 trailing zero's
	static const uint64_t EPOCH = ((uint64_t)116444736000000000ULL);

	SYSTEMTIME system_time;
	FILETIME file_time;
	uint64_t time;

	GetSystemTime(&system_time);
	SystemTimeToFileTime(&system_time, &file_time);
	time = ((uint64_t)file_time.dwLowDateTime);
	time += ((uint64_t)file_time.dwHighDateTime) << 32;

	tp->tv_sec = (long)((time - EPOCH) / 10000000L);
	tp->tv_usec = (long)(system_time.wMilliseconds * 1000);
	return 0;
}

char *strcasestr(const char *strA, const char *strB)
{
	size_t lenB = strlen(strB);
	size_t lenA = strlen(strA) - lenB + 1;

	bool found = true;

	for (int i = 0; i < lenA; i++) {
		for (int j = 0; j < lenB; j++) {
			unsigned char c1 = strA[i + j];
			unsigned char c2 = strB[j];
			if (toupper(c1) != toupper(c2))
				found = false;
		}

		if (found)
			return (char *)strA + i;
	}
	return NULL;
}

#endif // _MSC_VER
