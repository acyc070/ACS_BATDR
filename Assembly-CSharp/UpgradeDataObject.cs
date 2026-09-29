using System;
using UnityEngine;

[Serializable]
public class UpgradeDataObject : DataObject<UpgradeID, UpgradeDataObject>, IDataObject<UpgradeID>, IDataObject
{
	[SerializeField]
	private UpgradeID m_DataID;

	public override UpgradeID ID => this.m_DataID;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
