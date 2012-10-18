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

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BitmapFont {

public partial class Header
{
	public Header(BinaryReader br)
	{
		fontSize = br.ReadInt16();
		fontAscent = br.ReadInt16();
		metricCount = br.ReadInt16();
		sheetWidth = br.ReadInt16();
		sheetHeight = br.ReadInt16();
	}
}

public partial class Metric
{
	public Metric()
	{
	}

	public Metric(BinaryReader br)
	{
		advance = br.ReadSingle();
		u = br.ReadInt16();
		v = br.ReadInt16();
		bearingX = br.ReadSByte();
		bearingY = br.ReadSByte();
		width = br.ReadByte();
		height = br.ReadByte();
		first = br.ReadByte();
		second = br.ReadByte();
		prevNum = br.ReadByte();
		nextNum = br.ReadByte();
	}
}

public partial class Data
{
	public Data(byte[] bytes)
	{
		Stream s = new MemoryStream(bytes);
		BinaryReader br = new BinaryReader(s);

		header = new Header(br);
		indecies = new short[256];
		for (int i = 0; i < 256; ++i)
			indecies[i] = br.ReadInt16();

		metrics = new Metric[header.metricCount];
		for (int i = 0; i < header.metricCount; ++i)
			metrics[i] = new Metric(br);

		List<byte> bs = new List<byte>();
		while (true) {
			byte b = br.ReadByte();
			if (b == 0)
				break;
			bs.Add(b);
		}
		textureName = Encoding.UTF8.GetString(bs.ToArray());
	}
}

}	// namespace BitmapFont
