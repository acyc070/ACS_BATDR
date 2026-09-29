using System;
using UnityEngine;

[Serializable]
public class DataDirectories : DataCategory
{
	[SerializeField]
	private SectionDataDirectory m_SectionDirectory = new SectionDataDirectory();

	[SerializeField]
	private TaskDataDirectory m_TaskDirectory = new TaskDataDirectory();

	[SerializeField]
	private CollectableDataDirectory m_CollectableDirectory = new CollectableDataDirectory();

	public SectionDataDirectory SectionDirectory => this.m_SectionDirectory;

	public TaskDataDirectory TaskDirectory => this.m_TaskDirectory;

	public CollectableDataDirectory CollectableDirectory => this.m_CollectableDirectory;

	public MemoDataDirectory MemoDirectory => this.m_CollectableDirectory.MemoDirectory;

	public AudioLogDataDirectory AudioLogDirectory => this.m_CollectableDirectory.AudioLogDirectory;

	public MemoryDataDirectory MemoryDirectory => this.m_CollectableDirectory.MemoryDirectory;

	public UpgradeDataDirectory UpgradeDirectory => this.m_CollectableDirectory.UpgradeDirectory;

	public MeatlyDataDirectory MeatlyDirectory => this.m_CollectableDirectory.MeatlyDirectory;

	public IllusionDataDirectory IllusionDirectory => this.m_CollectableDirectory.IllusionDirectory;

	public GentSchematicDataDirectory GentSchematicDirectory => this.m_CollectableDirectory.GentSchematicDirectory;

	public GentLockDataDirectory GentLockDirectory => this.m_CollectableDirectory.GentLockDataDirectory;

	public GentPowerDataDirectory GentPowerDirectory => this.m_CollectableDirectory.GentPowerDataDirectory;

	public JackatoyDataDirectory JackatoyDirectory => this.m_CollectableDirectory.JackatoyDataDirectory;

	public DataDirectories()
	{
		base.AddData(this.SectionDirectory);
		base.AddData(this.TaskDirectory);
		base.AddData(this.MemoDirectory);
		base.AddData(this.AudioLogDirectory);
		base.AddData(this.MemoryDirectory);
		base.AddData(this.UpgradeDirectory);
		base.AddData(this.MeatlyDirectory);
		base.AddData(this.IllusionDirectory);
		base.AddData(this.GentSchematicDirectory);
		base.AddData(this.GentLockDirectory);
		base.AddData(this.GentPowerDirectory);
		base.AddData(this.JackatoyDirectory);
	}
}
