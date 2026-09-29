using System;
using UnityEngine;

public class ObjectiveLever : Objective
{
	[SerializeField]
	private Lever m_Lever;

	[SerializeField]
	private bool m_ClearNavigation = true;

	[SerializeField]
	private bool m_EnableOnInitialize = true;

	[SerializeField]
	private bool m_DontSendCompleteOnForceComplete;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		this.RemoveListeners();
		this.AddListeners();
		if (this.m_EnableOnInitialize)
		{
			this.m_Lever.Content.Enable();
			this.CheckInteractableInactive(isActive: true);
		}
		else
		{
			this.m_Lever.Content.Disable();
			this.CheckInteractableInactive(isActive: false);
		}
	}

	protected override void InternalDisable()
	{
		base.Inactive();
	}

	protected override void InternalInactive()
	{
		this.RemoveListeners();
		this.m_Lever.Content.Disable();
		this.CheckInteractableInactive(isActive: false);
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
		this.m_Lever.Content.Disable();
		this.CheckInteractableInactive(isActive: false);
	}

	protected override void InternalForceComplete()
	{
		this.m_Lever.Content.ForceActivateComplete();
		this.CheckInteractableInactive(isActive: false);
		if (!this.m_DontSendCompleteOnForceComplete)
		{
			base.SendOnComplete();
		}
	}

	private void CheckInteractableInactive(bool isActive)
	{
		InteractableInactive componentInChildren;
		componentInChildren = this.m_Lever.GetComponentInChildren<InteractableInactive>(includeInactive: true);
		if (componentInChildren != null)
		{
			if (isActive)
			{
				componentInChildren.InitializeInactive();
			}
			else
			{
				componentInChildren.DisableInitialize();
			}
		}
	}

	private void HandleLeverOnActivate(object sender, EventArgs e)
	{
		this.m_Lever.OnActivate -= this.HandleLeverOnActivate;
		GameManager.Instance.ClearNavigation();
	}

	private void HandleActionEventControllerOnActivated(object sender, EventArgs e)
	{
		this.m_Lever.OnActivated -= this.HandleActionEventControllerOnActivated;
		base.SendOnComplete();
	}

	protected void AddListeners()
	{
		if (this.m_ClearNavigation)
		{
			this.m_Lever.OnActivate += this.HandleLeverOnActivate;
		}
		this.m_Lever.OnActivated += this.HandleActionEventControllerOnActivated;
	}

	protected override void RemoveListeners()
	{
		this.m_Lever.OnActivate -= this.HandleLeverOnActivate;
		this.m_Lever.OnActivated -= this.HandleActionEventControllerOnActivated;
	}
}
