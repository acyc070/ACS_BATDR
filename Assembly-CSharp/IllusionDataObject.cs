using System;
using UnityEngine;

[Serializable]
public class IllusionDataObject : DataObject<IllusionID, IllusionDataObject>, IDataObject<IllusionID>, IDataObject
{
	[SerializeField]
	private IllusionID m_DataID;

	public override IllusionID ID => this.m_DataID;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
