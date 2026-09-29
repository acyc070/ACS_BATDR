using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionSlowZoneController : SectionController
{
	private EventTrigger[] m_EventTriggers;

	protected override IEnumerator InternalInitialize()
	{
		this.m_EventTriggers = base.GetComponentsInChildren<EventTrigger>(includeInactive: true);
		this.RemoveListeners();
		this.AddListeners();
		yield return null;
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		if (!(GameManager.Instance.Player == null))
		{
			GameManager.Instance.Player.PlayerMovement.SetMoveSpeed(0.5f, _isSlowed: true);
		}
	}

	private void HandleEventTriggerOnExit(object sender, EventArgs e)
	{
		if (!(GameManager.Instance.Player == null))
		{
			GameManager.Instance.Player.PlayerMovement.ResetMoveSpeed();
		}
	}

	private void AddListeners()
	{
		if (this.m_EventTriggers != null)
		{
			for (int i = 0; i < this.m_EventTriggers.Length; i = unchecked(i + 1))
			{
				EventTrigger obj;
				obj = this.m_EventTriggers[i];
				obj.OnEnter += this.HandleEventTriggerOnEnter;
				obj.OnExit += this.HandleEventTriggerOnExit;
			}
		}
	}

	protected override void RemoveListeners()
	{
		if (this.m_EventTriggers != null)
		{
			for (int i = 0; i < this.m_EventTriggers.Length; i = unchecked(i + 1))
			{
				EventTrigger obj;
				obj = this.m_EventTriggers[i];
				obj.OnEnter -= this.HandleEventTriggerOnEnter;
				obj.OnExit -= this.HandleEventTriggerOnExit;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_EventTriggers = null;
	}
}
