using UnityEngine;
using UnityEngine.Events;

public class ObjectiveUnityEvent : Objective
{
	[Header("Force Complete")]
	[SerializeField]
	private bool m_ForceComplete = true;

	[Header("Unity Events")]
	[SerializeField]
	private UnityEvent m_UnityEvent;

	protected override void InternalInitialize()
	{
		this.m_UnityEvent?.Invoke();
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		if (this.m_ForceComplete)
		{
			base.Initialize();
		}
		else
		{
			base.SendOnComplete();
		}
	}
}
