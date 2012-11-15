/*
 * Copyright (C) 2012 GREE, Inc.
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

using System;
using System.Collections.Generic;

namespace LWF {

public partial class LWF
{
	public int GetInstanceNameStringId(int instId)
	{
		if (instId < 0 || instId >= m_data.instanceNames.Length)
			return -1;
		return m_data.instanceNames[instId].stringId;
	}

	public int GetStringId(string str)
	{
		int i;
		if (m_data.stringMap.TryGetValue(str, out i))
			return i;
		else
			return -1;
	}

	public int SearchInstanceId(int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int i;
		if (m_data.instanceNameMap.TryGetValue(stringId, out i))
			return i;
		else
			return -1;
	}

	public int SearchFrame(Movie movie, string label)
	{
		return SearchFrame(movie, GetStringId(label));
	}

	public int SearchFrame(Movie movie, int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int frameNo;
		Dictionary<int, int> labelMap = m_data.labelMap[movie.objectId];
		if (labelMap.TryGetValue(stringId, out frameNo))
			return frameNo + 1;
		else
			return -1;
	}

	public Dictionary<int, int> GetMovieLabels(Movie movie)
	{
		if (movie == null)
			return null;
		return m_data.labelMap[movie.objectId];
	}

	public int SearchMovieLinkage(int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int i;
		if (m_data.movieLinkageMap.TryGetValue(stringId, out i))
			return m_data.movieLinkages[i].movieId;
		else
			return -1;
	}

	public string GetMovieLinkageName(int movieId)
	{
		int i;
		if (m_data.movieLinkageNameMap.TryGetValue(movieId, out i))
			return m_data.strings[i];
		else
			return null;
	}

	public int SearchEventId(string eventName)
	{
		return SearchEventId(GetStringId(eventName));
	}

	public int SearchEventId(int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int i;
		if (m_data.eventMap.TryGetValue(stringId, out i))
			return i;
		else
			return -1;
	}

	public int SearchProgramObjectId(string programObjectName)
	{
		return SearchProgramObjectId(GetStringId(programObjectName));
	}

	public int SearchProgramObjectId(int stringId)
	{
		if (stringId < 0 || stringId >= m_data.strings.Length)
			return -1;

		int i;
		if (m_data.programObjectMap.TryGetValue(stringId, out i))
			return i;
		else
			return -1;
	}
}

}	// namespace LWF
