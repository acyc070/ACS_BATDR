using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionVendingMachineController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private VendingMachineGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public VendingMachineGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			VendingMachineGroup vendingMachineGroup;
			vendingMachineGroup = this.m_Group[i];
			if (vendingMachineGroup != null)
			{
				if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs >= 5)
				{
					vendingMachineGroup.Controller.Content.Enable();
				}
				else
				{
					vendingMachineGroup.Controller.Content.Disable();
				}
			}
		}
		yield return null;
	}
}
