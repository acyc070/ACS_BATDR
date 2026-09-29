using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SectionDataObject : DataObject<SectionID, SectionDataObject>, IDataObject<SectionID>, IDataObject
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private bool m_IsComplete;

	[SerializeField]
	private ObjectiveDataGroup m_ObjectiveData = new ObjectiveDataGroup();

	[SerializeField]
	private CutsceneDataGroup m_CutsceneData = new CutsceneDataGroup();

	[SerializeField]
	private DoorDataGroup m_DoorData = new DoorDataGroup();

	[SerializeField]
	private MovableDataGroup m_MovableData = new MovableDataGroup();

	[SerializeField]
	private LootDataGroup m_LootData = new LootDataGroup();

	[SerializeField]
	private LootableEnemyDataGroup m_LootableEnemyData = new LootableEnemyDataGroup();

	[SerializeField]
	private BreakableDataGroup m_BreakableData = new BreakableDataGroup();

	[SerializeField]
	private FoodDataGroup m_FoodData = new FoodDataGroup();

	[SerializeField]
	private GentRechargerDataGroup m_GentRechargeData = new GentRechargerDataGroup();

	[SerializeField]
	private GentBatteryDataGroup m_GentBatteryData = new GentBatteryDataGroup();

	[SerializeField]
	private GentPartsDataGroup m_GentPartsData = new GentPartsDataGroup();

	[SerializeField]
	private SlugDataGroup m_SlugData = new SlugDataGroup();

	[SerializeField]
	private GentToolkitDataGroup m_GentToolkitData = new GentToolkitDataGroup();

	[SerializeField]
	private GentBatteryCasingDataGroup m_GentBatteryCasingData = new GentBatteryCasingDataGroup();

	[SerializeField]
	private GentCardDataGroup m_GentCardData = new GentCardDataGroup();

	[SerializeField]
	private PictureDataGroup m_PictureData = new PictureDataGroup();

	[SerializeField]
	private EnemyDataGroup m_EnemyData = new EnemyDataGroup();

	[SerializeField]
	private CharacterDirectorDataGroup m_CharacterDirectorData = new CharacterDirectorDataGroup();

	[SerializeField]
	private HideDataGroup m_HideData = new HideDataGroup();

	[SerializeField]
	private LadderDataGroup m_LadderData = new LadderDataGroup();

	[SerializeField]
	private ProjectorDataGroup m_ProjectorData = new ProjectorDataGroup();

	[SerializeField]
	private ComboLockDataGroup m_ComboLockData = new ComboLockDataGroup();

	[SerializeField]
	private PeekDataGroup m_PeekData = new PeekDataGroup();

	[SerializeField]
	private CompanionDirectorDataGroup m_CompanionDirectorData = new CompanionDirectorDataGroup();

	private List<Data> m_Groups = new List<Data>();

	public override SectionID ID => this.m_SectionID;

	public bool IsComplete => this.m_IsComplete;

	public ObjectiveDataGroup ObjectiveData => this.m_ObjectiveData;

	public CutsceneDataGroup CutsceneData => this.m_CutsceneData;

	public DoorDataGroup DoorData => this.m_DoorData;

	public MovableDataGroup MovableData => this.m_MovableData;

	public LootDataGroup LootData => this.m_LootData;

	public LootableEnemyDataGroup LootableEnemyData => this.m_LootableEnemyData;

	public BreakableDataGroup BreakableData => this.m_BreakableData;

	public FoodDataGroup FoodData => this.m_FoodData;

	public GentRechargerDataGroup GentRechargeData => this.m_GentRechargeData;

	public GentBatteryDataGroup GentBatteryData => this.m_GentBatteryData;

	public GentPartsDataGroup GentPartsData => this.m_GentPartsData;

	public SlugDataGroup SlugData => this.m_SlugData;

	public GentToolkitDataGroup GentToolkitData => this.m_GentToolkitData;

	public GentBatteryCasingDataGroup GentBatteryCasingData => this.m_GentBatteryCasingData;

	public GentCardDataGroup GentCardData => this.m_GentCardData;

	public PictureDataGroup PictureData => this.m_PictureData;

	public EnemyDataGroup EnemyData => this.m_EnemyData;

	public CharacterDirectorDataGroup CharacterDirectorData => this.m_CharacterDirectorData;

	public HideDataGroup HideData => this.m_HideData;

	public LadderDataGroup LadderData => this.m_LadderData;

	public ProjectorDataGroup ProjectorData => this.m_ProjectorData;

	public ComboLockDataGroup ComboLockData => this.m_ComboLockData;

	public PeekDataGroup PeekData => this.m_PeekData;

	public CompanionDirectorDataGroup CompanionDirectorData => this.m_CompanionDirectorData;

	public SectionDataObject()
	{
		this.m_Groups.Add(this.ObjectiveData);
		this.m_Groups.Add(this.CutsceneData);
		this.m_Groups.Add(this.DoorData);
		this.m_Groups.Add(this.MovableData);
		this.m_Groups.Add(this.LootData);
		this.m_Groups.Add(this.LootableEnemyData);
		this.m_Groups.Add(this.BreakableData);
		this.m_Groups.Add(this.FoodData);
		this.m_Groups.Add(this.GentRechargeData);
		this.m_Groups.Add(this.GentBatteryData);
		this.m_Groups.Add(this.GentPartsData);
		this.m_Groups.Add(this.SlugData);
		this.m_Groups.Add(this.GentToolkitData);
		this.m_Groups.Add(this.GentBatteryCasingData);
		this.m_Groups.Add(this.GentCardData);
		this.m_Groups.Add(this.PictureData);
		this.m_Groups.Add(this.EnemyData);
		this.m_Groups.Add(this.CharacterDirectorData);
		this.m_Groups.Add(this.HideData);
		this.m_Groups.Add(this.LadderData);
		this.m_Groups.Add(this.ProjectorData);
		this.m_Groups.Add(this.ComboLockData);
		this.m_Groups.Add(this.PeekData);
		this.m_Groups.Add(this.CompanionDirectorData);
	}

	public void SetComplete()
	{
		this.m_IsComplete = true;
	}

	public bool GetGroup(string name, out Data group)
	{
		bool result;
		result = false;
		group = null;
		for (int i = 0; i < this.m_Groups.Count; i = unchecked(i + 1))
		{
			group = this.m_Groups[i];
			if (group.Name == name)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	protected override void Deserialize()
	{
		this.m_SectionID = base.m_ID;
	}
}
