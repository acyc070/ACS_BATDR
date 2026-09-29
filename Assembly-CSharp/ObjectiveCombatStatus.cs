using UnityEngine;

public class ObjectiveCombatStatus : Objective
{
	[SerializeField]
	private CombatStatus m_CombatStatus;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		GameManager.Instance.Player.SetCombatStatus(this.m_CombatStatus);
	}

	protected override void InternalForceComplete()
	{
		base.Enable();
		base.SendOnComplete();
	}
}
