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

public class Text : Object
{
	protected string m_name;
	public string name {get {return m_name;}}

	public Text(LWF lwf, Movie p, int objId, int instId = -1)
		: base(lwf, p, Format.Object.Type.TEXT, objId)
	{
		Format.Text text = lwf.data.texts[objId];
		m_dataMatrixId = text.matrixId;

		if (text.nameStringId != -1) {
			m_name = lwf.data.strings[text.nameStringId];
		} else {
			if (instId >= 0 && instId < lwf.data.instanceNames.Length) {
				int stringId = lwf.GetInstanceNameStringId(instId);
				if (stringId != -1)
					m_name = lwf.data.strings[stringId];
			}
		}

		TextRenderer textRenderer =
			lwf.rendererFactory.ConstructText(lwf, objId, this);

		string t = null;
		if (text.stringId != -1)
			t = lwf.data.strings[text.stringId];

		if (text.nameStringId == -1 && string.IsNullOrEmpty(name)) {
			if (text.stringId != -1)
				textRenderer.SetText(t);
		} else {
#if LWF_USE_LUA
			string lt = lwf.GetTextLua(parent, name);
			if (!System.String.IsNullOrEmpty(lt))
				t = lt;
#endif
			lwf.SetTextRenderer(p.GetFullName(), name, t, textRenderer);
		}

		m_renderer = textRenderer;
	}
}

}	// namespace LWF
