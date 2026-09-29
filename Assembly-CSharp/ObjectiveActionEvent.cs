using System;
using UnityEngine;

public class ObjectiveActionEvent : Objective
{
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[SerializeField]
	private bool m_SetActiveOnInitialize = true;

	[SerializeField]
	private bool m_CanInterrupt;

	[SerializeField]
	private bool m_DisposeOnForceComplete;

	protected override void InternalUpdate()
	{
		if (GameManager.Instance.Player == null || this.m_CanInterrupt || base.IsComplete)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat || GameManager.Instance.Player._inkDemonActive)
		{
			if (this.m_ActionEvent.IsActive)
			{
				this.m_ActionEvent.SetActive(active: false);
			}
		}
		else if (!this.m_ActionEvent.IsActive)
		{
			this.m_ActionEvent.SetActive(active: true);
		}
	}

	protected override void InternalInitialize()
	{
		base.Enable();
	}

	protected override void InternalEnable()
	{
		if (!(this.m_ActionEvent == null))
		{
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
			this.m_ActionEvent.OnInteract += this.HandleActionEventOnInteract;
			if (this.m_SetActiveOnInitialize)
			{
				this.m_ActionEvent.SetActive(active: true);
			}
		}
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
		base.SendOnComplete();
	}

	protected override void InternalInactive()
	{
		base.Disable();
	}

	protected override void InternalDisable()
	{
		if (!(this.m_ActionEvent == null))
		{
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
			this.m_ActionEvent.SetActive(active: false);
		}
	}

	protected override void InternalForceComplete()
	{
		if (this.m_DisposeOnForceComplete && this.m_ActionEvent != null)
		{
			this.m_ActionEvent.Dispose();
		}
		base.SendOnComplete();
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
	}

	protected override void RemoveListeners()
	{
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
		}
	}
}
