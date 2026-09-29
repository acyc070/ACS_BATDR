using UnityEngine;

public class ObjectiveBattleStatus : Objective
{
	[SerializeField]
	private BattleStatus m_BattleStatus;

	protected override void InternalInitialize()
	{
		GameManager.Instance.Player.SetBattleStatus(this.m_BattleStatus);
		base.SendOnComplete();
	}
}
