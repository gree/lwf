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

#include "lwf_core.h"
#include "lwf_data.h"
#include "lwf_movie.h"

namespace LWF {

int LWF::GetInstanceNameStringId(int instId) const
{
	if (instId < 0 || instId >= (int)data->instanceNames.size())
		return -1;
	return data->instanceNames[instId].stringId;
}

int LWF::GetStringId(string str) const
{
	map<string, int>::const_iterator it(data->stringMap.find(str));
	if (it != data->stringMap.end())
		return it->second;
	else
		return -1;
}

int LWF::SearchInstanceId(int stringId) const
{
	if (stringId < 0 || stringId >= (int)data->strings.size())
		return -1;

	map<int, int>::const_iterator it(data->instanceNameMap.find(stringId));
	if (it != data->instanceNameMap.end())
		return it->second;
	else
		return -1;
}

int LWF::SearchFrame(const Movie *movie, string label) const
{
	return SearchFrame(movie, GetStringId(label));
}

int LWF::SearchFrame(const Movie *movie, int stringId) const
{
	if (stringId < 0 || stringId >= (int)data->strings.size())
		return -1;

	const map<int, int> &m = data->labelMap[movie->objectId];
	map<int, int>::const_iterator it = m.find(stringId);
	if (it != m.end())
		return it->second + 1;
	else
		return -1;
}

const map<int, int> *LWF::GetMovieLabels(string linkageName) const
{
	int objectId = SearchMovieLinkage(GetStringId(linkageName));
	if (objectId < 0)
		return 0;
	return &data->labelMap[objectId];
}

const map<int, int> *LWF::GetMovieLabels(const Movie *movie) const
{
	if (!movie)
		return 0;
	return &data->labelMap[movie->objectId];
}

int LWF::SearchMovieLinkage(int stringId) const
{
	if (stringId < 0 || stringId >= (int)data->strings.size())
		return -1;

	map<int, int>::const_iterator it = data->movieLinkageMap.find(stringId);
	if (it != data->movieLinkageMap.end())
		return data->movieLinkages[it->second].movieId;
	else
		return -1;
}

string LWF::GetMovieLinkageName(int movieId) const
{
	map<int, int>::const_iterator it = data->movieLinkageNameMap.find(movieId);
	if (it != data->movieLinkageNameMap.end())
		return data->strings[it->second];
	else
		return string();
}

int LWF::SearchEventId(string eventName) const
{
	return SearchEventId(GetStringId(eventName));
}

int LWF::SearchEventId(int stringId) const
{
	if (stringId < 0 || stringId >= (int)data->strings.size())
		return -1;

	map<int, int>::const_iterator it = data->eventMap.find(stringId);
	if (it != data->eventMap.end())
		return it->second;
	else
		return -1;
}

int LWF::SearchProgramObjectId(string programObjectName) const
{
	return SearchProgramObjectId(GetStringId(programObjectName));
}

int LWF::SearchProgramObjectId(int stringId) const
{
	if (stringId < 0 || stringId >= (int)data->strings.size())
		return -1;

	map<int, int>::const_iterator it = data->programObjectMap.find(stringId);
	if (it != data->programObjectMap.end())
		return it->second;
	else
		return -1;
}

}	// namespace LWF
