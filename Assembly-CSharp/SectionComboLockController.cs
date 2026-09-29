using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionComboLockController : SectionController
{
	private ComboLock[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<ComboLock>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			ComboLock comboLock;
			comboLock = this.m_Group[i];
			ComboLockDataObject comboLockDataObject;
			comboLockDataObject = (ComboLockDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ComboLockDataObject>(base.Section.SectionID, comboLock.ID);
			if (comboLockDataObject != null)
			{
				if (comboLockDataObject.IsComplete == 1)
				{
					comboLock.ForceComplete();
					continue;
				}
				comboLock.InitializeCode(comboLockDataObject.Code);
				comboLock.OnComplete -= this.HandleGroupOnComplete;
				comboLock.OnComplete += this.HandleGroupOnComplete;
			}
			else
			{
				comboLockDataObject = DataObject<int, ComboLockDataObject>.Create(comboLock.ID);
				comboLockDataObject.SetCode(this.SetCode(comboLock));
				GameManager.Instance.GameData.CurrentSave.AddData(base.Section.SectionID, comboLockDataObject);
				comboLock.Initialize();
				comboLock.OnComplete -= this.HandleGroupOnComplete;
				comboLock.OnComplete += this.HandleGroupOnComplete;
			}
		}
		yield return null;
	}

	private void HandleGroupOnComplete(object sender, EventArgs e)
	{
		this.SaveComplete(sender as ComboLock);
	}

	private int SetCode(ComboLock group)
	{
		string text;
		text = "";
		for (int i = 0; i < group.Content.NumberWheels.Count; i = unchecked(i + 1))
		{
			text += group.Content.NumberWheels[i].CurrentIndex;
		}
		return text.ParseInt();
	}

	public void SaveComplete(ComboLock group)
	{
		this.SaveData();
		((ComboLockDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ComboLockDataObject>(base.Section.SectionID, group.ID))?.SetComplete();
	}

	public override void SaveData()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			ComboLock comboLock;
			comboLock = this.m_Group[i];
			ComboLockDataObject comboLockDataObject;
			comboLockDataObject = (ComboLockDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ComboLockDataObject>(base.Section.SectionID, comboLock.ID);
			if (comboLockDataObject != null)
			{
				comboLockDataObject.SetCode(this.SetCode(comboLock));
				continue;
			}
			comboLockDataObject = DataObject<int, ComboLockDataObject>.Create(comboLock.ID);
			comboLockDataObject.SetCode(this.SetCode(comboLock));
			GameManager.Instance.GameData.CurrentSave.AddData(base.Section.SectionID, comboLockDataObject);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				this.m_Group[i].OnComplete -= this.HandleGroupOnComplete;
			}
			this.m_Group = null;
		}
	}
}
