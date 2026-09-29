using UnityEngine;

public class ObjectiveNewTasks : Objective
{
	[SerializeField]
	private TaskID m_TaskID;

	[SerializeField]
	private TaskEntryID[] m_TaskEntryID;

	[SerializeField]
	private bool m_ShowPrompt = true;

	[SerializeField]
	private bool m_IsReal;

	protected override void InternalInitialize()
	{
		TaskDataObject taskDataObject;
		taskDataObject = GameManager.Instance.GameData.CurrentSave.DataDirectories.TaskDirectory.GetValue(this.m_TaskID) as TaskDataObject;
		if (taskDataObject == null)
		{
			taskDataObject = DataObject<TaskID, TaskDataObject>.Create(this.m_TaskID);
			GameManager.Instance.GameData.CurrentSave.DataDirectories.TaskDirectory.Add(this.m_TaskID, taskDataObject);
		}
		for (int i = 0; i < this.m_TaskEntryID.Length; i = unchecked(i + 1))
		{
			TaskEntryID taskEntryID;
			taskEntryID = this.m_TaskEntryID[i];
			if (taskDataObject.GetEntry(taskEntryID) == null)
			{
				TaskEntryDataObject item;
				item = DataObject<TaskEntryID, TaskEntryDataObject>.Create(taskEntryID);
				taskDataObject.Tasks.Add(item);
			}
		}
		if (this.m_ShowPrompt)
		{
			GameManager.Instance.ShowObjective(TextUtility.GetKey("Objective_New").ToUpper(), TextUtility.GetKey(this.m_TaskID.ToString()).ToUpper(), this.m_IsReal);
		}
		base.SendOnComplete();
	}
}
