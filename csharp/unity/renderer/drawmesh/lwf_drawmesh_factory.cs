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

using UnityEngine;

using TextureLoader = System.Func<string, UnityEngine.Texture2D>;
using TextureUnloader = System.Action<UnityEngine.Texture2D>;

namespace LWF {
namespace DrawMeshRenderer {

public partial class Factory : UnityRenderer.Factory
{
	public Factory(Data d, GameObject gObj,
			float zOff = 0, float zR = 1, int rQOff = 0,
			string sLayerName = null, int sOrder = 0, bool uAC = false,
			Camera renderCam = null, Camera inputCam = null,
			string texturePrfx = "", string fontPrfx = "",
			TextureLoader textureLdr = null,
			TextureUnloader textureUnldr = null,
			string shaderName = "LWF")
		: base(d, gObj, zOff, zR, rQOff, sLayerName, sOrder, uAC, renderCam,
			inputCam, texturePrfx, fontPrfx, textureLdr, textureUnldr, shaderName)
	{
		CreateBitmapContexts();
		CreateTextContexts();
	}

	public override void Destruct()
	{
		DestructBitmapContexts();
		DestructTextContexts();

		base.Destruct();
	}

	public override Renderer ConstructBitmap(LWF lwf,
		int objectId, Bitmap bitmap)
	{
		return new BitmapRenderer(lwf, m_bitmapContexts[objectId]);
	}

	public override Renderer ConstructBitmapEx(LWF lwf,
		int objectId, BitmapEx bitmapEx)
	{
		return new BitmapRenderer(lwf, m_bitmapExContexts[objectId]);
	}

	public override TextRenderer ConstructText(LWF lwf, int objectId, Text text)
	{
		return new TextMeshRenderer(lwf, m_textContexts[objectId]);
	}
}

}	// namespace DrawMeshRenderer
}	// namespace LWF
