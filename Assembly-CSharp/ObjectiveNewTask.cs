using UnityEngine;

public class ObjectiveNewTask : Objective
{
	[SerializeField]
	private TaskID m_TaskID;

	[SerializeField]
	private TaskEntryID m_TaskEntryID;

	[SerializeField]
	private bool m_ShowPrompt = true;

	[SerializeField]
	private bool m_DisplayEntryText;

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
		if (taskDataObject.GetEntry(this.m_TaskEntryID) == null)
		{
			TaskEntryDataObject item;
			item = DataObject<TaskEntryID, TaskEntryDataObject>.Create(this.m_TaskEntryID);
			taskDataObject.Tasks.Add(item);
		}
		if (this.m_ShowPrompt)
		{
			string message;
			message = TextUtility.GetKey(this.m_TaskID.ToString()).ToUpper();
			if (this.m_DisplayEntryText)
			{
				message = TextUtility.GetKey(this.m_TaskEntryID.ToString()).ToUpper();
			}
			GameManager.Instance.ShowObjective(TextUtility.GetKey("Objective_New").ToUpper(), message, this.m_IsReal);
		}
		base.SendOnComplete();
	}
}
