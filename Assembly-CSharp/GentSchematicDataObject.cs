using System;
using UnityEngine;

[Serializable]
public class GentSchematicDataObject : DataObject<GentSchematicID, GentSchematicDataObject>, IDataObject<GentSchematicID>, IDataObject
{
	[SerializeField]
	private GentSchematicID m_DataID;

	public override GentSchematicID ID => this.m_DataID;

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
