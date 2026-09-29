using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentPartsController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentPartsGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public GentPartsGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentPartsGroup gentPartsGroup;
			gentPartsGroup = this.m_Group[i];
			if (gentPartsGroup != null)
			{
				gentPartsGroup.Controller.OnActivate -= this.HandleControllerOnActivate;
				if ((GentPartsDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentPartsDataObject>(this.m_SectionID, gentPartsGroup.ID) != null)
				{
					gentPartsGroup.IsComplete = true;
					gentPartsGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentPartsGroup.Controller.Content as GentPartsContent).InitializeContent();
					gentPartsGroup.Controller.OnActivate += this.HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentParts gentParts;
		gentParts = sender as GentParts;
		gentParts.OnActivate -= this.HandleControllerOnActivate;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentPartsGroup gentPartsGroup;
			gentPartsGroup = this.m_Group[i];
			if (gentPartsGroup.Controller == gentParts)
			{
				if ((GentPartsDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentPartsDataObject>(this.m_SectionID, gentPartsGroup.ID) == null)
				{
					GentPartsDataObject value;
					value = DataObject<int, GentPartsDataObject>.Create(gentPartsGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				gentPartsGroup.IsComplete = true;
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
