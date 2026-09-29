using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TaskDataObject : DataObject<TaskID, TaskDataObject>, IDataObject<TaskID>, IDataObject
{
	[SerializeField]
	private TaskID m_DataID;

	[SerializeField]
	private bool m_IsComplete;

	[SerializeField]
	private List<TaskEntryDataObject> m_Tasks = new List<TaskEntryDataObject>();

	public override TaskID ID => this.m_DataID;

	public bool IsComplete => this.m_IsComplete;

	public List<TaskEntryDataObject> Tasks => this.m_Tasks;

	public void Complete()
	{
		this.m_IsComplete = true;
	}

	public TaskEntryDataObject GetEntry(TaskEntryID taskEntryID)
	{
		TaskEntryDataObject taskEntryDataObject;
		taskEntryDataObject = null;
		for (int i = 0; i < this.m_Tasks.Count; i = unchecked(i + 1))
		{
			taskEntryDataObject = this.m_Tasks[i];
			if (taskEntryDataObject.ID == taskEntryID)
			{
				break;
			}
			taskEntryDataObject = null;
		}
		return taskEntryDataObject;
	}

	public bool CheckComplete()
	{
		bool result;
		result = true;
		for (int i = 0; i < this.m_Tasks.Count; i = unchecked(i + 1))
		{
			if (!this.m_Tasks[i].IsComplete)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
