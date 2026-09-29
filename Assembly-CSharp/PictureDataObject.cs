using System;
using UnityEngine;

[Serializable]
public class PictureDataObject : DataObject<int, PictureDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	public override int ID => this.m_DataID;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
