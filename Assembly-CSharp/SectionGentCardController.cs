using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentCardController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentCardGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public GentCardGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentCardGroup gentCardGroup;
			gentCardGroup = this.m_Group[i];
			if (gentCardGroup != null)
			{
				gentCardGroup.Controller.OnActivate -= this.HandleControllerOnActivate;
				if ((GentCardDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentCardDataObject>(this.m_SectionID, gentCardGroup.ID) != null)
				{
					gentCardGroup.IsComplete = true;
					gentCardGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentCardGroup.Controller.Content as GentCardContent).InitializeContent();
					gentCardGroup.Controller.OnActivate += this.HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentCard gentCard;
		gentCard = sender as GentCard;
		gentCard.OnActivate -= this.HandleControllerOnActivate;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentCardGroup gentCardGroup;
			gentCardGroup = this.m_Group[i];
			if (gentCardGroup.Controller == gentCard)
			{
				if ((GentCardDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentCardDataObject>(this.m_SectionID, gentCardGroup.ID) == null)
				{
					GentCardDataObject value;
					value = DataObject<int, GentCardDataObject>.Create(gentCardGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				gentCardGroup.IsComplete = true;
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				this.m_Group[i].Controller.OnActivate -= this.HandleControllerOnActivate;
			}
		}
	}
}
