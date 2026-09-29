using UnityEngine;

public class ObjectiveAbility : Objective
{
	[SerializeField]
	private State.PlayerAbility m_AbilityType;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		GameManager.Instance.Player.SetAbility(this.m_AbilityType);
		base.SendOnComplete();
	}
}
