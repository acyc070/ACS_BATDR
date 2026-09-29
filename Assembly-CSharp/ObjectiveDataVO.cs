public class ObjectiveDataVO : JDisposable
{
	public TaskID Title;

	public TaskEntryDataVO[] Tasks;

	public static ObjectiveDataVO Create(TaskID taskID, params TaskEntryDataVO[] taskEntryDataVOs)
	{
		return new ObjectiveDataVO
		{
			Title = taskID,
			Tasks = taskEntryDataVOs
		};
	}
}
