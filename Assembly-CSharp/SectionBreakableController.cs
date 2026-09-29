using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionBreakableController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private BreakableGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public BreakableGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			BreakableGroup breakableGroup;
			breakableGroup = this.m_Group[i];
			if (breakableGroup != null)
			{
				breakableGroup.Controller.OnBroken -= this.HandleBreakableOnBroken;
				if ((BreakableDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, BreakableDataObject>(this.m_SectionID, breakableGroup.ID) != null)
				{
					breakableGroup.IsComplete = true;
					breakableGroup.Controller.ForceComplete();
				}
				else
				{
					breakableGroup.Controller.OnBroken += this.HandleBreakableOnBroken;
				}
			}
			yield return null;
		}
	}

	private void HandleBreakableOnBroken(object sender, EventArgs e)
	{
		Breakable breakable;
		breakable = sender as Breakable;
		breakable.OnBroken -= this.HandleBreakableOnBroken;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			BreakableGroup breakableGroup;
			breakableGroup = this.m_Group[i];
			if (breakableGroup.Controller == breakable)
			{
				if ((BreakableDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, BreakableDataObject>(this.m_SectionID, breakableGroup.ID) == null)
				{
					BreakableDataObject value;
					value = DataObject<int, BreakableDataObject>.Create(breakableGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				breakableGroup.IsComplete = true;
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			this.m_Group[i].Controller.OnBroken -= this.HandleBreakableOnBroken;
		}
		base.OnDisposed();
	}
}
