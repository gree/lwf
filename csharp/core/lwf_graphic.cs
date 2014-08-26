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

using Type = Format.GraphicObject.Type;

public class Graphic : Object
{
	private Object[] m_displayList;

	public Object[] displayList {get {return m_displayList;}}

	public Graphic(LWF lwf, Movie parent, int objId)
		: base(lwf, parent, Format.Object.Type.GRAPHIC, objId)
	{
		Format.Graphic data = lwf.data.graphics[objId];
		int n = data.graphicObjects;
		m_displayList = new Object[n];

		Format.GraphicObject[] graphicObjects = lwf.data.graphicObjects;
		for (int i = 0; i < n; ++i) {
			Format.GraphicObject gobj =
				graphicObjects[data.graphicObjectId + i];
			Object obj = null;
			int graphicObjectId = gobj.graphicObjectId;

			// Ignore error
			if (graphicObjectId == -1)
				continue;

			switch ((Type)gobj.graphicObjectType) {
			case Type.BITMAP:
				obj = new Bitmap(lwf, parent, graphicObjectId);
				break;

			case Type.BITMAPEX:
				obj = new BitmapEx(lwf, parent, graphicObjectId);
				break;

			case Type.TEXT:
				obj = new Text(lwf, parent, graphicObjectId);
				break;
			}

			obj.Exec();
			m_displayList[i] = obj;
		}
	}

	public override void Update(Matrix m, ColorTransform c)
	{
		int n = m_displayList.Length;
		for (int i = 0; i < n; ++i)
			m_displayList[i].Update(m, c);
	}

	public override void Render(bool v, int rOffset)
	{
		if (!v)
			return;
		int n = m_displayList.Length;
		for (int i = 0; i < n; ++i)
			m_displayList[i].Render(v, rOffset);
	}

#if UNITY_EDITOR
	public override void RenderNow()
	{
		int n = m_displayList.Length;
		for (int i = 0; i < n; ++i)
			m_displayList[i].RenderNow();
	}
#endif

	public override void Destroy()
	{
		int n = m_displayList.Length;
		for (int i = 0; i < n; ++i)
			m_displayList[i].Destroy();
		m_displayList = null;
	}
}

}	// namespace LWF
