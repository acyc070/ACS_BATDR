using System;
using UnityEngine;

[Serializable]
public class ObjectiveDataObject : DataObject<int, ObjectiveDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private ObjectiveStatus m_Status;

	public override int ID => this.m_DataID;

	public ObjectiveStatus Status => this.m_Status;

	public void SetStatus(ObjectiveStatus status)
	{
		this.m_Status = status;
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
