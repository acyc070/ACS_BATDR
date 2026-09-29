using System;
using UnityEngine;

public class ObjectiveBettyBox : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Interactable")]
	[SerializeField]
	private ActionEventController m_ActionEventController;

	protected override void InternalInitialize()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
			this.m_ActionEventController.Content.Disable();
		}
	}

	private void HandleActionEventOnActivate(object sender, EventArgs e)
	{
		this.m_ActionEventController.OnActivate -= this.HandleActionEventOnActivate;
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		this.m_ActionEventController.Dispose();
		base.SendOnComplete();
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
	}

	private bool CheckStatus()
	{
		bool flag;
		flag = true;
		if (this.m_Requirements != null)
		{
			flag = this.m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			this.m_ActionEventController.OnActivate -= this.HandleActionEventOnActivate;
			this.m_ActionEventController.OnActivate += this.HandleActionEventOnActivate;
			this.m_ActionEventController.Content.Enable();
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		this.CheckStatus();
	}

	protected override void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (this.m_ActionEventController != null)
		{
			this.m_ActionEventController.OnActivate -= this.HandleActionEventOnActivate;
		}
	}
}
