using System;
using UnityEngine;

public class ObjectiveHide : Objective
{
	[SerializeField]
	private InteractableHide m_InteractableHide;

	[SerializeField]
	private bool m_ControlLock = true;

	[SerializeField]
	private float m_Delay;

	protected override void InternalInitialize()
	{
		base.Enable();
	}

	protected override void InternalEnable()
	{
		base.Invoke("ActualEnable", this.m_Delay);
	}

	private void ActualEnable()
	{
		if (!(this.m_InteractableHide == null))
		{
			this.m_InteractableHide.OnEnter -= this.HandleInteractableHideOnEnter;
			this.m_InteractableHide.OnEnter += this.HandleInteractableHideOnEnter;
		}
	}

	private void HandleInteractableHideOnEnter(object sender, EventArgs e)
	{
		this.m_InteractableHide.OnEnter -= this.HandleInteractableHideOnEnter;
		if (this.m_ControlLock)
		{
			(this.m_InteractableHide.Interactable as InteractableAnimationEnter).SetLock(isLocked: true);
		}
		base.SendOnComplete();
	}

	protected override void InternalInactive()
	{
		base.Disable();
	}

	protected override void InternalDisable()
	{
		base.Invoke("ActualDisable", this.m_Delay);
	}

	private void ActualDisable()
	{
		if (!(this.m_InteractableHide == null))
		{
			if (this.m_ControlLock)
			{
				(this.m_InteractableHide.Interactable as InteractableAnimationEnter).SetLock(isLocked: false);
			}
			this.m_InteractableHide.OnExit -= this.HandleInteractableHideOnExit;
			this.m_InteractableHide.OnExit += this.HandleInteractableHideOnExit;
		}
	}

	private void HandleInteractableHideOnExit(object sender, EventArgs e)
	{
		this.m_InteractableHide.OnExit -= this.HandleInteractableHideOnExit;
		base.SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (this.m_InteractableHide != null)
		{
			this.m_InteractableHide.OnEnter -= this.HandleInteractableHideOnEnter;
		}
		if (this.m_InteractableHide != null)
		{
			this.m_InteractableHide.OnExit -= this.HandleInteractableHideOnExit;
		}
	}
}
