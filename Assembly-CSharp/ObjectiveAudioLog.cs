using System;
using UnityEngine;

public class ObjectiveAudioLog : Objective
{
	[SerializeField]
	private ActionEventController m_ActionEventController;

	[SerializeField]
	private bool m_OnComplete;

	protected override void InternalInitialize()
	{
		if (!(this.m_ActionEventController == null))
		{
			if (this.m_OnComplete)
			{
				this.m_ActionEventController.OnDeactivate -= this.HandleActionEventControllerOnEvent;
				this.m_ActionEventController.OnDeactivate += this.HandleActionEventControllerOnEvent;
			}
			else
			{
				this.m_ActionEventController.OnActivate -= this.HandleActionEventControllerOnEvent;
				this.m_ActionEventController.OnActivate += this.HandleActionEventControllerOnEvent;
			}
		}
	}

	private void HandleActionEventControllerOnEvent(object sender, EventArgs e)
	{
		this.m_ActionEventController.OnActivate -= this.HandleActionEventControllerOnEvent;
		this.m_ActionEventController.OnDeactivate -= this.HandleActionEventControllerOnEvent;
		base.SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (this.m_ActionEventController != null)
		{
			this.m_ActionEventController.OnActivate -= this.HandleActionEventControllerOnEvent;
			this.m_ActionEventController.OnDeactivate -= this.HandleActionEventControllerOnEvent;
		}
		base.OnDisposed();
	}
}
