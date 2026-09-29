using UnityEngine;

public class ObjectiveActiveSetter : Objective
{
	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	[SerializeField]
	private bool m_ForceComplete = true;

	[SerializeField]
	private bool m_ActiveOnComplete;

	protected override void InternalInitialize()
	{
		this.SetActive();
	}

	protected override void InternalEnable()
	{
		this.m_ActiveSetter?.SetActive(active: true);
	}

	protected override void InternalDisable()
	{
		this.m_ActiveSetter?.SetActive(active: false);
	}

	private void SetActive()
	{
		this.m_ActiveSetter?.SetActive(!this.m_ActiveSetter.IsActive);
	}

	protected override void InternalForceComplete()
	{
		if (this.m_ForceComplete)
		{
			this.m_ActiveSetter.ForceComplete();
		}
		else if (this.m_ActiveOnComplete)
		{
			this.m_ActiveSetter.SetActive(active: true);
		}
		base.SendOnComplete();
	}
}
