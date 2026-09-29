using S13Audio;
using UnityEngine;

public class ObjectiveS13Event : Objective
{
	[SerializeField]
	private S13ScriptableEvent m_ScriptableEvent;

	protected override void InternalInitialize()
	{
		this.m_ScriptableEvent?.Raise();
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		this.m_ScriptableEvent?.Raise();
	}
}
