using System;
using UnityEngine;

[Serializable]
public class BreakableDataObject : DataObject<int, BreakableDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_BreakableID;

	public override int ID => this.m_BreakableID;

	protected override void Deserialize()
	{
		this.m_BreakableID = base.m_ID;
	}
}
