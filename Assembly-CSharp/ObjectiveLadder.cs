using UnityEngine;

public class ObjectiveLadder : Objective
{
	[SerializeField]
	private Ladder m_Ladder;

	protected override void InternalInitialize()
	{
		base.Enable();
	}

	protected override void InternalEnable()
	{
		this.m_Ladder.Content.Enable();
	}

	protected override void InternalInactive()
	{
		base.Disable();
	}

	protected override void InternalDisable()
	{
		this.m_Ladder.Content.Disable();
	}

	protected override void InternalForceComplete()
	{
		this.m_Ladder.Content.ForceActivateComplete();
		base.SendOnComplete();
	}
}
