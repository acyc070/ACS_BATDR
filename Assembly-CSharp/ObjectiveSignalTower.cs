using UnityEngine;

public class ObjectiveSignalTower : Objective
{
	[SerializeField]
	private SignalTower m_SignalTower;

	[SerializeField]
	private bool m_TurnOff = true;

	[SerializeField]
	private bool m_OnForceComplete = true;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		this.m_SignalTower.TurnOn();
		base.SendOnComplete();
	}

	protected override void InternalDisable()
	{
		base.Inactive();
	}

	protected override void InternalInactive()
	{
		this.m_SignalTower.TurnOff();
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		if (this.m_OnForceComplete)
		{
			if (this.m_TurnOff)
			{
				this.m_SignalTower.TurnOff();
			}
			else
			{
				this.m_SignalTower.TurnOn();
			}
		}
		base.SendOnComplete();
	}
}
