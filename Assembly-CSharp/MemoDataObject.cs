using System;
using UnityEngine;

[Serializable]
public class MemoDataObject : DataObject<MemoID, MemoDataObject>, IDataObject<MemoID>, IDataObject
{
	[SerializeField]
	private MemoID m_DataID;

	public override MemoID ID => this.m_DataID;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
