using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentRechargerController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentRechargerGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public GentRechargerGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentRechargerGroup gentRechargerGroup;
			gentRechargerGroup = this.m_Group[i];
			if (gentRechargerGroup != null)
			{
				GentRechargerDataObject gentRechargerDataObject;
				gentRechargerDataObject = (GentRechargerDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentRechargerDataObject>(this.m_SectionID, gentRechargerGroup.ID);
				if (gentRechargerDataObject != null)
				{
					gentRechargerGroup.Controller.Initialize(gentRechargerDataObject.Batteries);
				}
				else
				{
					gentRechargerGroup.Controller.Initialize(0);
				}
				gentRechargerGroup.Controller.OnDeposit -= this.HandleControllerOnChanged;
				gentRechargerGroup.Controller.OnCharged -= this.HandleControllerOnChanged;
				gentRechargerGroup.Controller.OnDeposit += this.HandleControllerOnChanged;
				gentRechargerGroup.Controller.OnCharged += this.HandleControllerOnChanged;
			}
		}
		yield return null;
	}

	private void HandleControllerOnChanged(object sender, EventArgs e)
	{
		GentRecharger gentRecharger;
		gentRecharger = sender as GentRecharger;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentRechargerGroup gentRechargerGroup;
			gentRechargerGroup = this.m_Group[i];
			if (gentRechargerGroup.Controller == gentRecharger)
			{
				GentRechargerDataObject gentRechargerDataObject;
				gentRechargerDataObject = (GentRechargerDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentRechargerDataObject>(this.m_SectionID, gentRechargerGroup.ID);
				if (gentRechargerDataObject != null)
				{
					gentRechargerDataObject.SetBatteries(gentRecharger.BatteryCount);
					break;
				}
				gentRechargerDataObject = DataObject<int, GentRechargerDataObject>.Create(gentRechargerGroup.ID);
				gentRechargerDataObject.SetBatteries(gentRecharger.BatteryCount);
				GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, gentRechargerDataObject);
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
				GentRechargerGroup obj;
				obj = this.m_Group[i];
				obj.Controller.OnDeposit -= this.HandleControllerOnChanged;
				obj.Controller.OnCharged -= this.HandleControllerOnChanged;
			}
		}
	}
}
