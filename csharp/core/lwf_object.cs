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

using Type = Format.Object.Type;
using Inspector = System.Action<Object, int, int, int>;

public class Object
{
	protected LWF m_lwf;
	protected Movie m_parent;
	protected Type m_type;
	protected int m_execCount;
	protected int m_objectId;
	protected int m_matrixId;
	protected int m_colorTransformId;
	protected Matrix m_matrix;
	protected int m_dataMatrixId;
	protected ColorTransform m_colorTransform;
	protected Renderer m_renderer;
	protected bool m_matrixIdChanged;
	protected bool m_colorTransformIdChanged;
	protected bool m_updated;

	public LWF lwf {get {return m_lwf;}}
	public Movie parent {get {return m_parent;}}
	public Type type {get {return m_type;}}
	public int objectId {get {return m_objectId;}}
	public int matrixId {get {return m_matrixId;}}
	public int colorTransformId {get {return m_colorTransformId;}}
	public Matrix matrix {get {return m_matrix;}}
	public ColorTransform colorTransform {get {return m_colorTransform;}}
	public bool matrixIdChanged {
		get {return m_matrixIdChanged;}
		set {m_matrixIdChanged = value;}
	}
	public bool colorTransformIdChanged {
		get {return m_colorTransformIdChanged;}
		set {m_colorTransformIdChanged = value;}
	}
	public bool updated {get {return m_updated;}}
	public int execCount {
		get {return m_execCount;}
		set {m_execCount = value;}
	}

	public Object() {}

	public Object(LWF lwf, Movie parent, int type, int objId)
		: this(lwf, parent, (Type)type, objId) {}

	public Object(LWF lwf, Movie parent, Type type, int objId)
	{
		m_lwf = lwf;
		m_parent = parent;
		m_type = type;
		m_objectId = objId;
		m_matrixId = -1;
		m_colorTransformId = -1;
		m_matrixIdChanged = true;
		m_colorTransformIdChanged = true;
		m_matrix = new Matrix(0, 0, 0, 0, 0, 0);
		m_colorTransform = new ColorTransform(0, 0, 0, 0);
		m_execCount = 0;
		m_updated = false;
	}

	public virtual void Exec(int matrixId = 0, int colorTransformId = 0)
	{
		if (m_matrixId != matrixId) {
			m_matrixIdChanged = true;
			m_matrixId = matrixId;
		}
		if (m_colorTransformId != colorTransformId) {
			m_colorTransformIdChanged = true;
			m_colorTransformId = colorTransformId;
		}
	}

	public virtual void Update(Matrix m, ColorTransform c)
	{
		m_updated = true;
		if (m != null) {
			Utility.CalcMatrix(m_lwf, m_matrix, m, m_dataMatrixId);
			m_matrixIdChanged = false;
		}
		if (c != null) {
			Utility.CopyColorTransform(m_colorTransform, c);
			m_colorTransformIdChanged = false;
		}
		m_lwf.RenderObject();
	}

	public virtual void Render(bool v, int rOffset)
	{
		if (m_renderer != null) {
			int rIndex = m_lwf.renderingIndex;
			int rIndexOffsetted = m_lwf.renderingIndexOffsetted;
			int rCount = m_lwf.renderingCount;
			if (rOffset != System.Int32.MinValue)
				rIndex = rIndexOffsetted - rOffset + rCount;
			m_renderer.Render(m_matrix, m_colorTransform, rIndex, rCount, v);
		}
		m_lwf.RenderObject();
	}

#if UNITY_EDITOR
	public virtual void RenderNow()
	{
		if (m_renderer != null)
			m_renderer.RenderNow();
	}
#endif

	public virtual void Inspect(
		Inspector inspector, int hierarchy, int depth, int rOffset)
	{
		int rIndex = m_lwf.renderingIndex;
		int rIndexOffsetted = m_lwf.renderingIndexOffsetted;
		int rCount = m_lwf.renderingCount;
		if (rOffset != System.Int32.MinValue)
			rIndex = rIndexOffsetted + rOffset + rCount;
		inspector(this, hierarchy, depth, rIndex);
		m_lwf.RenderObject();
	}

	public virtual void Destroy()
	{
		if (m_renderer != null) {
			m_renderer.Destruct();
			m_renderer = null;
		}
	}

	public bool IsButton() {return m_type == Type.BUTTON ? true : false;}
	public bool IsMovie() {return (m_type == Type.MOVIE ||
		m_type == Type.ATTACHEDMOVIE) ? true : false;}
	public bool IsParticle() {return m_type == Type.PARTICLE ? true : false;}
	public bool IsProgramObject()
		{return m_type == Type.PROGRAMOBJECT ? true : false;}
	public bool IsText() {return m_type == Type.TEXT ? true : false;}
	public virtual bool IsBitmapClip() {return false;}
}

}	// namespace LWF
