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

namespace LWF {

public class Renderer
{
	protected LWF m_lwf;

	public LWF lwf {get {return m_lwf;}}

	public Renderer(LWF lwf) {m_lwf = lwf;}
	public virtual void Destruct() {}
	public virtual void Update(Matrix matrix, ColorTransform colorTransform) {}
	public virtual void Render(Matrix matrix, ColorTransform colorTransform,
		int renderingIndex, int renderingCount, bool visible) {}
#if UNITY_EDITOR
	public virtual void RenderNow() {}
#endif
}

public class TextRenderer : Renderer
{
	public TextRenderer(LWF lwf) : base(lwf) {}
	public virtual void SetText(string text) {}
}

public interface IRendererFactory
{
	Renderer ConstructBitmap(LWF lwf, int objId, Bitmap bitmap);
	Renderer ConstructBitmapEx(LWF lwf, int objId, BitmapEx bitmapEx);
	TextRenderer ConstructText(LWF lwf, int objId, Text text);
	Renderer ConstructParticle(LWF lwf, int objId, Particle particle);
	void Init(LWF lwf);
	void BeginRender(LWF lwf);
	void EndRender(LWF lwf);
	void Destruct();
	void SetBlendMode(int blendMode);
	void SetMaskMode(int maskMode);
}

public class NullRendererFactory : IRendererFactory
{
	public Renderer ConstructBitmap(LWF lwf, int objId, Bitmap bitmap)
		{return null;}
	public Renderer ConstructBitmapEx(LWF lwf, int objId, BitmapEx bitmapEx)
		{return null;}
	public TextRenderer ConstructText(LWF lwf, int objId, Text text)
		{return null;}
	public Renderer ConstructParticle(LWF lwf, int objId, Particle particle)
		{return null;}
	public void Init(LWF lwf) {}
	public void BeginRender(LWF lwf) {}
	public void EndRender(LWF lwf) {}
	public void Destruct() {}
	public void SetBlendMode(int blendMode) {}
	public void SetMaskMode(int maskMode) {}
}

}	// namespace LWF
