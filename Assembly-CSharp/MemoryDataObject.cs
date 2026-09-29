using System;
using UnityEngine;

[Serializable]
public class MemoryDataObject : DataObject<MemoryID, MemoryDataObject>, IDataObject<MemoryID>, IDataObject
{
	[SerializeField]
	private MemoryID m_DataID;

	public override MemoryID ID => this.m_DataID;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
