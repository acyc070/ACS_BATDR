using System;

[Serializable]
public class TaskEntryDataVO : JDisposable
{
	public TaskEntryID ID;

	public bool IsComplete;

	public static TaskEntryDataVO Create(TaskEntryID id, bool isComplete)
	{
		return new TaskEntryDataVO
		{
			ID = id,
			IsComplete = isComplete
		};
	}
}
