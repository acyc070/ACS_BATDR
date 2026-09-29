using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentBatteryCasingController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentBatteryCasingGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public GentBatteryCasingGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentBatteryCasingGroup gentBatteryCasingGroup;
			gentBatteryCasingGroup = this.m_Group[i];
			if (gentBatteryCasingGroup != null)
			{
				gentBatteryCasingGroup.Controller.OnActivate -= this.HandleControllerOnActivate;
				if ((GentBatteryCasingDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentBatteryCasingDataObject>(this.m_SectionID, gentBatteryCasingGroup.ID) != null)
				{
					gentBatteryCasingGroup.IsComplete = true;
					gentBatteryCasingGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentBatteryCasingGroup.Controller.Content as GentBatteryCasingContent).InitializeContent();
					gentBatteryCasingGroup.Controller.OnActivate += this.HandleControllerOnActivate;
				}
			}
		}
		yield return null;
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentBatteryCasing gentBatteryCasing;
		gentBatteryCasing = sender as GentBatteryCasing;
		gentBatteryCasing.OnActivate -= this.HandleControllerOnActivate;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentBatteryCasingGroup gentBatteryCasingGroup;
			gentBatteryCasingGroup = this.m_Group[i];
			if (gentBatteryCasingGroup.Controller == gentBatteryCasing)
			{
				if ((GentBatteryCasingDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentBatteryCasingDataObject>(this.m_SectionID, gentBatteryCasingGroup.ID) == null)
				{
					GentBatteryCasingDataObject value;
					value = DataObject<int, GentBatteryCasingDataObject>.Create(gentBatteryCasingGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				gentBatteryCasingGroup.IsComplete = true;
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
