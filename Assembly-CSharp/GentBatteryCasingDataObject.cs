using System;
using UnityEngine;

[Serializable]
public class GentBatteryCasingDataObject : DataObject<int, GentBatteryCasingDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	public override int ID => this.m_DataID;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
