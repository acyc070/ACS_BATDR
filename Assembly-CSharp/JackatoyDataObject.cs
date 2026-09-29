using System;
using UnityEngine;

[Serializable]
public class JackatoyDataObject : DataObject<JackatoyID, JackatoyDataObject>, IDataObject<JackatoyID>, IDataObject
{
	[SerializeField]
	private JackatoyID m_DataID;

	public override JackatoyID ID => this.m_DataID;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
