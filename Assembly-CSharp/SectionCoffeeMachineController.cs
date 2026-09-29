using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionCoffeeMachineController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private CoffeeMachineGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public CoffeeMachineGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			CoffeeMachineGroup coffeeMachineGroup;
			coffeeMachineGroup = this.m_Group[i];
			if (coffeeMachineGroup != null)
			{
				if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs >= 15)
				{
					coffeeMachineGroup.Controller.Content.Enable();
				}
				else
				{
					coffeeMachineGroup.Controller.Content.Disable();
				}
			}
		}
		yield return null;
	}
}
