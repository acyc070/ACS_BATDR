using System;
using UnityEngine;

[Serializable]
public class MeatlyDataObject : DataObject<MeatlyID, MeatlyDataObject>, IDataObject<MeatlyID>, IDataObject
{
	[SerializeField]
	private MeatlyID m_DataID;

	public override MeatlyID ID => this.m_DataID;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
