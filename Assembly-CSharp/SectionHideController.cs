using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionHideController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private InteractableHideGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public InteractableHideGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		int num;
		num = -1;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			InteractableHideGroup interactableHideGroup;
			interactableHideGroup = this.m_Group[i];
			HideDataObject hideDataObject;
			hideDataObject = (HideDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, HideDataObject>(this.m_SectionID, interactableHideGroup.ID);
			if (hideDataObject == null)
			{
				hideDataObject = DataObject<int, HideDataObject>.Create(interactableHideGroup.ID);
				GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, hideDataObject);
			}
			else if (hideDataObject.HideState == 1)
			{
				num = hideDataObject.ID;
			}
			interactableHideGroup.Controller.OnEnter -= this.HandleInteractableHideOnEnter;
			interactableHideGroup.Controller.OnEnter += this.HandleInteractableHideOnEnter;
			interactableHideGroup.Controller.OnExit -= this.HandleInteractableHideOnExit;
			interactableHideGroup.Controller.OnExit += this.HandleInteractableHideOnExit;
			interactableHideGroup.Controller.Initialize();
			if (interactableHideGroup.ID == num && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.CombatStatus == CombatStatus.Hide)
			{
				interactableHideGroup.Controller.ForceEnter(hideDataObject);
			}
		}
		yield return null;
	}

	private void HandleInteractableHideOnEnter(object sender, EventArgs e)
	{
		InteractableHide interactableHide;
		interactableHide = sender as InteractableHide;
		HideDataObject hideDataObject;
		hideDataObject = (HideDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, HideDataObject>(this.m_SectionID, interactableHide.ID);
		if (hideDataObject == null)
		{
			hideDataObject = DataObject<int, HideDataObject>.Create(interactableHide.ID);
			GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, hideDataObject);
		}
		hideDataObject.SetHideState(isHiding: true);
	}

	private void HandleInteractableHideOnExit(object sender, EventArgs e)
	{
		((HideDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, HideDataObject>(this.m_SectionID, (sender as InteractableHide).ID))?.SetHideState(isHiding: false);
	}

	protected override void RemoveListeners()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			InteractableHideGroup obj;
			obj = this.m_Group[i];
			obj.Controller.OnEnter -= this.HandleInteractableHideOnEnter;
			obj.Controller.OnExit -= this.HandleInteractableHideOnExit;
		}
	}

	public override void SaveData()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			InteractableHideGroup interactableHideGroup;
			interactableHideGroup = this.m_Group[i];
			HideDataObject hideDataObject;
			hideDataObject = (HideDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, HideDataObject>(this.m_SectionID, interactableHideGroup.ID);
			if (hideDataObject != null && hideDataObject.HideState == 1)
			{
				hideDataObject.SetRotationX(GameManager.Instance.Player.AnimationContainer.localRotation);
				hideDataObject.SetRotationY(GameManager.Instance.Player.HeadContainer.localRotation);
			}
		}
	}
}
