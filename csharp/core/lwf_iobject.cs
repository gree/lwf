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

public class IObject : Object {
	protected int m_instanceId;
	protected int m_iObjectId;
	protected string m_name;
	protected IObject m_prevInstance;
	protected IObject m_nextInstance;
	protected IObject m_linkInstance;
	protected bool m_alive;

	public IObject nextInstance {get {return m_nextInstance;}}
	public IObject linkInstance {
		get {return m_linkInstance;}
		set {m_linkInstance = value;}
	}
	public int instanceId {get {return m_instanceId;}}
	public int iObjectId {get {return m_iObjectId;}}
	public string name {get {return m_name;}}

	public IObject() {}

	public IObject(LWF lwf, Movie parent, int type, int objId, int instId)
		: this(lwf, parent, (Type)type, objId, instId) {}

	public IObject(LWF lwf,
			Movie parent, Type type, int objId, int instId)
		: base(lwf, parent, type, objId)
	{
		m_alive = true;
		m_prevInstance = null;
		m_nextInstance = null;
		m_linkInstance = null;

		m_instanceId =
			(instId >= lwf.data.instanceNames.Length) ? -1 : (int)instId;
		m_iObjectId = lwf.GetIObjectOffset();

		if (m_instanceId >= 0) {
			int stringId = lwf.GetInstanceNameStringId(m_instanceId);
			m_name = stringId == -1 ? null : lwf.data.strings[stringId];

			IObject head = m_lwf.GetInstance(m_instanceId);
			if (head != null)
				head.m_prevInstance = this;
			m_nextInstance = head;
			m_lwf.SetInstance(m_instanceId, this);
		}
	}

	public override void Destroy()
	{
		if (m_type != Type.ATTACHEDMOVIE && m_instanceId >= 0) {
			IObject head = m_lwf.GetInstance(m_instanceId);
			if (head == this)
				m_lwf.SetInstance(m_instanceId, m_nextInstance);
			if (m_nextInstance != null)
				m_nextInstance.m_prevInstance = m_prevInstance;
			if (m_prevInstance != null)
				m_prevInstance.m_nextInstance = m_nextInstance;
		}

		base.Destroy();
		m_alive = false;
	}

	public virtual void LinkButton()
	{
		// NOTHING TO DO
	}

	public string GetFullName()
	{
		string fullPath = "";
		string splitter = "";
		for (IObject o = this; o != null; o = o.parent) {
			if (o.name == null)
				return null;
			fullPath = o.name + splitter + fullPath;
			splitter = ".";
		}
		return fullPath;
	}
}

}	// namespace LWF
