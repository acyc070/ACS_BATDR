using UnityEngine;

public class ObjectiveButcherGangStatus : Objective
{
	[SerializeField]
	private ButcherGangStatus m_ButcherGangStatus;

	protected override void InternalInitialize()
	{
		GameManager.Instance.GameData.CurrentSave.Difficulty.SetButcherGangStatus(this.m_ButcherGangStatus);
	}
}
