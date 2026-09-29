using System;
using UnityEngine;

[Serializable]
public class TaskEntryDataObject : DataObject<TaskEntryID, TaskEntryDataObject>, IDataObject<TaskEntryID>, IDataObject
{
	[SerializeField]
	private TaskEntryID m_DataID;

	[SerializeField]
	private bool m_IsComplete;

	public override TaskEntryID ID => this.m_DataID;

	public bool IsComplete => this.m_IsComplete;

	public void Complete()
	{
		this.m_IsComplete = true;
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
