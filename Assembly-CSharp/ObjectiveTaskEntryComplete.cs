using UnityEngine;

public class ObjectiveTaskEntryComplete : Objective
{
	[SerializeField]
	private TaskID m_TaskID;

	[SerializeField]
	private TaskEntryID m_TaskEntryID;

	protected override void InternalInitialize()
	{
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.TaskDirectory.GetValue(this.m_TaskID) is TaskDataObject taskDataObject)
		{
			taskDataObject.GetEntry(this.m_TaskEntryID)?.Complete();
			if (taskDataObject.CheckComplete())
			{
				taskDataObject.Complete();
			}
		}
		base.SendOnComplete();
	}
}
