using System;
using UnityEngine;

public class ObjectiveConcertTicket : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Interactable")]
	[SerializeField]
	private ActionEvent m_ActionEvent;

	protected override void InternalInitialize()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
			this.m_ActionEvent.SetActive(active: false);
		}
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		this.m_ActionEvent.Dispose();
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
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
			this.m_ActionEvent.OnInteract += this.HandleActionEventOnInteract;
			this.m_ActionEvent.SetActive(active: true);
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
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
		}
	}
}
