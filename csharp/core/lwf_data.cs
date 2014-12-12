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

using System.Collections.Generic;

namespace LWF {

using Constant = Format.Constant;

public partial class Data
{
	public Format.Header header;
	public Translate[] translates;
	public Matrix[] matrices;
	public Color[] colors;
	public AlphaTransform[] alphaTransforms;
	public ColorTransform[] colorTransforms;
	public Format.Object[] objects;
	public Format.Texture[] textures;
	public Format.TextureFragment[] textureFragments;
	public Format.Bitmap[] bitmaps;
	public Format.BitmapEx[] bitmapExs;
	public Format.Font[] fonts;
	public Format.TextProperty[] textProperties;
	public Format.Text[] texts;
	public Format.ParticleData[] particleDatas;
	public Format.Particle[] particles;
	public Format.ProgramObject[] programObjects;
	public Format.GraphicObject[] graphicObjects;
	public Format.Graphic[] graphics;
	public int[][] animations;
	public Format.ButtonCondition[] buttonConditions;
	public Format.Button[] buttons;
	public Format.Label[] labels;
	public Format.InstanceName[] instanceNames;
	public Format.Event[] events;
	public Format.Place[] places;
	public Format.ControlMoveM[] controlMoveMs;
	public Format.ControlMoveC[] controlMoveCs;
	public Format.ControlMoveMC[] controlMoveMCs;
	public Format.ControlMoveMCB[] controlMoveMCBs;
	public Format.Control[] controls;
	public Format.Frame[] frames;
	public Format.MovieClipEvent[] movieClipEvents;
	public Format.Movie[] movies;
	public Format.MovieLinkage[] movieLinkages;
	public string[] strings;

	public Dictionary<string, int> stringMap;
	public Dictionary<int, int> instanceNameMap;
	public Dictionary<int, int> eventMap;
	public Dictionary<int, int> movieLinkageMap;
	public Dictionary<int, int> movieLinkageNameMap;
	public Dictionary<int, int> programObjectMap;
	public Dictionary<int, int>[] labelMap;
	public Dictionary<string, int> bitmapMap;

	public string name {get {return strings[header.nameStringId];}}
	public bool useScript {get {return
		(header.option & (int)Format.Constant.OPTION_USE_LUASCRIPT) != 0;}}
	public bool useTextureAtlas {get {return
		(header.option & (int)Format.Constant.OPTION_USE_TEXTUREATLAS) != 0;}}

	public bool Check()
	{
		byte v0 = header.formatVersion0;
		byte v1 = header.formatVersion1;
		byte v2 = header.formatVersion2;

		if (header != null &&
				header.id0 == 'L' &&
				header.id1 == 'W' &&
				header.id2 == 'F' &&
				header.id3 == (byte)Constant.FORMAT_TYPE &&
				((
					v0 == (byte)Constant.FORMAT_VERSION_0 &&
					v1 == (byte)Constant.FORMAT_VERSION_1 &&
					v2 == (byte)Constant.FORMAT_VERSION_2
				) || (
					v0 == (byte)Constant.FORMAT_VERSION_COMPAT0_0 &&
					v1 == (byte)Constant.FORMAT_VERSION_COMPAT0_1 &&
					v2 == (byte)Constant.FORMAT_VERSION_COMPAT0_2
				) || (
					v0 == (byte)Constant.FORMAT_VERSION_COMPAT1_0 &&
					v1 == (byte)Constant.FORMAT_VERSION_COMPAT1_1 &&
					v2 == (byte)Constant.FORMAT_VERSION_COMPAT1_2
				)) &&
				(header.option & (int)Format.Constant.OPTION_COMPRESSED) == 0) {
			return true;
		}
		return false;
	}

	public bool ReplaceTexture(
		int index, Format.TextureReplacement textureReplacement)
	{
		if (index < 0 || index >= textures.Length)
			return false;

		textures[index] = textureReplacement;
		return true;
	}

	public bool ReplaceTextureFragment(
		int index, Format.TextureFragmentReplacement textureFragmentReplacement)
	{
		if (index < 0 || index >= textureFragments.Length)
			return false;

		textureFragments[index] = textureFragmentReplacement;
		return true;
	}
}

}	// namespace LWF
