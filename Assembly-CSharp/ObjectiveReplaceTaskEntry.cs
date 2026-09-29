using UnityEngine;

public class ObjectiveReplaceTaskEntry : Objective
{
	[SerializeField]
	private TaskID m_TaskID;

	[SerializeField]
	private TaskEntryID m_CurrentTaskEntryID;

	[SerializeField]
	private TaskEntryID m_ReplacementTaskEntryID;

	[SerializeField]
	private bool m_ShowPrompt;

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
		TaskEntryDataObject entry;
		entry = taskDataObject.GetEntry(this.m_CurrentTaskEntryID);
		if (entry != null)
		{
			taskDataObject.Tasks.Remove(entry);
		}
		if (taskDataObject.GetEntry(this.m_ReplacementTaskEntryID) == null)
		{
			taskDataObject.Tasks.Add(DataObject<TaskEntryID, TaskEntryDataObject>.Create(this.m_ReplacementTaskEntryID));
		}
		if (this.m_ShowPrompt)
		{
			string key;
			key = this.m_TaskID.ToString();
			if (this.m_DisplayEntryText)
			{
				key = this.m_ReplacementTaskEntryID.ToString();
			}
			GameManager.Instance.ShowObjective(TextUtility.GetKey("Objective_Update").ToUpper(), TextUtility.GetKey(key).ToUpper(), this.m_IsReal);
		}
		base.SendOnComplete();
	}
}
