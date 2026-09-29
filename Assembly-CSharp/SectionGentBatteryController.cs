using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentBatteryController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentBatteryGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public GentBatteryGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentBatteryGroup gentBatteryGroup;
			gentBatteryGroup = this.m_Group[i];
			if (gentBatteryGroup != null)
			{
				gentBatteryGroup.Controller.OnActivate -= this.HandleControllerOnActivate;
				if ((GentBatteryDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentBatteryDataObject>(this.m_SectionID, gentBatteryGroup.ID) != null)
				{
					gentBatteryGroup.IsComplete = true;
					gentBatteryGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentBatteryGroup.Controller.Content as GentBatteryContent).InitializeContent();
					gentBatteryGroup.Controller.OnActivate += this.HandleControllerOnActivate;
				}
			}
		}
		yield return null;
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentBattery gentBattery;
		gentBattery = sender as GentBattery;
		gentBattery.OnActivate -= this.HandleControllerOnActivate;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentBatteryGroup gentBatteryGroup;
			gentBatteryGroup = this.m_Group[i];
			if (gentBatteryGroup.Controller == gentBattery)
			{
				if ((GentBatteryDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentBatteryDataObject>(this.m_SectionID, gentBatteryGroup.ID) == null)
				{
					GentBatteryDataObject value;
					value = DataObject<int, GentBatteryDataObject>.Create(gentBatteryGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				gentBatteryGroup.IsComplete = true;
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
