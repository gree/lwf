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

#include "lwf_data.h"

namespace LWF {
namespace Format {

static string ConvertFilename(const Data *data, int stringId)
{
	const string &s = data->strings[stringId];
	string::size_type pos = s.find_last_of('.');
	if (pos != string::npos)
		return s.substr(0, pos);
	else
		return s;
}

void Texture::SetFilename(const Data *data)
{
	filename = ConvertFilename(data, stringId);
}

const string &Texture::GetFilename(const Data *data) const
{
	return data->strings[stringId];
}

void TextureFragment::SetFilename(const Data *data)
{
	filename = ConvertFilename(data, stringId);
}

const string &TextureFragment::GetFilename(const Data *data) const
{
	return data->strings[stringId];
}

}	// namespace Format
}	// namespace LWF
