using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class Section : DataMonoBehaviour<SectionID, SectionDataObject>
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("Navigation Settings")]
	[SerializeField]
	private bool m_HasNavigation = true;

	[Header("Seasonal")]
	[SerializeField]
	private SeasonalSectionData m_SeasonalSectionData;

	private SectionController[] m_SectionControllers;

	protected override SectionID m_ID => this.m_SectionID;

	public SectionID SectionID => this.m_ID;

	public bool HasNavigation => this.m_HasNavigation;

	public SectionController[] SectionControllers => this.m_SectionControllers;

	public bool IsActive { get; private set; }

	public bool IsReady { get; private set; }

	public void SetActive(bool active)
	{
		this.IsActive = active;
	}

	protected override void InternalInitialize()
	{
		JDebug.Log("Section :: InternalInitialize", this, JDebug.JDebugType.SectionController);
		this.SetActive(active: true);
		base.StartCoroutine(this.InitializeControllers());
	}

	private IEnumerator InitializeControllers()
	{
		this.m_SectionControllers = base.GetComponentsInChildren<SectionController>(includeInactive: false);
		SectionLootController sectionLootController;
		sectionLootController = null;
		SectionBreakableController sectionBreakableController;
		sectionBreakableController = null;
		SectionHideController sectionHideController;
		sectionHideController = null;
		SectionCompanionDirectorController sectionCompanionDirectorController;
		sectionCompanionDirectorController = null;
		SewerDrainController sewerDrainController;
		sewerDrainController = null;
		for (int i = 0; i < this.m_SectionControllers.Length; i = unchecked(i + 1))
		{
			SectionController sectionController;
			sectionController = this.m_SectionControllers[i];
			sectionController.SetSection(this);
			if (sectionController is SectionLootController)
			{
				sectionLootController = sectionController as SectionLootController;
			}
			else if (sectionController is SectionBreakableController)
			{
				sectionBreakableController = sectionController as SectionBreakableController;
			}
			else if (sectionController is SectionHideController)
			{
				sectionHideController = sectionController as SectionHideController;
			}
			else if (sectionController is SectionCompanionDirectorController)
			{
				sectionCompanionDirectorController = sectionController as SectionCompanionDirectorController;
			}
			else if (sectionController is SewerDrainController)
			{
				sewerDrainController = sectionController as SewerDrainController;
			}
			else
			{
				yield return sectionController.Initialize();
			}
		}
		if (sewerDrainController != null)
		{
			yield return sewerDrainController.Initialize();
		}
		if (sectionLootController != null)
		{
			yield return sectionLootController.Initialize();
		}
		if (sectionBreakableController != null)
		{
			yield return sectionBreakableController.Initialize();
		}
		if (sectionHideController != null)
		{
			yield return sectionHideController.Initialize();
		}
		if (sectionCompanionDirectorController != null)
		{
			yield return sectionCompanionDirectorController.Initialize();
		}
		if (this.m_SeasonalSectionData != null && SeasonalCheck.IsSeasonal(out var seasonalType) && seasonalType != SeasonalType.None && GameManager.Instance.PlayerSettings.Seasonal)
		{
			SeasonalData seasonalData;
			seasonalData = this.m_SeasonalSectionData.SeasonalDatas.Find((SeasonalData x) => x.SeasonalType == seasonalType);
			if (seasonalData != null && seasonalData.SeasonalObject != null)
			{
				GameObject gameObject;
				gameObject = Object.Instantiate(seasonalData.SeasonalObject);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localEulerAngles = Vector3.zero;
			}
		}
		this.IsReady = true;
	}

	protected override void OnDisposed()
	{
		this.m_SectionControllers = null;
		base.OnDisposed();
	}
}
