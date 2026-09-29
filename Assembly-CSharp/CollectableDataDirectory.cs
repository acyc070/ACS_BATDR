using System;
using UnityEngine;

[Serializable]
public class CollectableDataDirectory
{
	[SerializeField]
	private MemoDataDirectory m_MemoDirectory = new MemoDataDirectory();

	[SerializeField]
	private AudioLogDataDirectory m_AudioLogDirectory = new AudioLogDataDirectory();

	[SerializeField]
	private MemoryDataDirectory m_MemoryDirectory = new MemoryDataDirectory();

	[SerializeField]
	private UpgradeDataDirectory m_UpgradeDirectory = new UpgradeDataDirectory();

	[SerializeField]
	private MeatlyDataDirectory m_MeatlyDirectory = new MeatlyDataDirectory();

	[SerializeField]
	private IllusionDataDirectory m_IllusionDirectory = new IllusionDataDirectory();

	[SerializeField]
	private GentSchematicDataDirectory m_GentSchematicDirectory = new GentSchematicDataDirectory();

	[SerializeField]
	private GentLockDataDirectory m_GentLockDataDirectory = new GentLockDataDirectory();

	[SerializeField]
	private GentPowerDataDirectory m_GentPowerDataDirectory = new GentPowerDataDirectory();

	[SerializeField]
	private JackatoyDataDirectory m_JackatoyDataDirectory = new JackatoyDataDirectory();

	public MemoDataDirectory MemoDirectory => this.m_MemoDirectory;

	public AudioLogDataDirectory AudioLogDirectory => this.m_AudioLogDirectory;

	public MemoryDataDirectory MemoryDirectory => this.m_MemoryDirectory;

	public UpgradeDataDirectory UpgradeDirectory => this.m_UpgradeDirectory;

	public MeatlyDataDirectory MeatlyDirectory => this.m_MeatlyDirectory;

	public IllusionDataDirectory IllusionDirectory => this.m_IllusionDirectory;

	public GentSchematicDataDirectory GentSchematicDirectory => this.m_GentSchematicDirectory;

	public GentLockDataDirectory GentLockDataDirectory => this.m_GentLockDataDirectory;

	public GentPowerDataDirectory GentPowerDataDirectory => this.m_GentPowerDataDirectory;

	public JackatoyDataDirectory JackatoyDataDirectory => this.m_JackatoyDataDirectory;
}
