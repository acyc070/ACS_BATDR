using System;
using UnityEngine;

public class ObjectiveLeverRequirements : Objective
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Lever")]
	[SerializeField]
	private Lever m_Lever;

	[Header("Interact Settings")]
	[SerializeField]
	private bool m_ClearNavigation = true;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		this.RemoveListeners();
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
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
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
		this.m_Lever.Content.Disable();
	}

	protected override void InternalForceComplete()
	{
		this.m_Lever.Content.ForceActivateComplete();
		base.SendOnComplete();
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

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		this.CheckStatus();
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
			this.RemoveListeners();
			this.AddListeners();
			this.m_Lever.Content.Enable();
		}
		return flag;
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
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		this.m_Lever.OnActivate -= this.HandleLeverOnActivate;
		this.m_Lever.OnActivated -= this.HandleActionEventControllerOnActivated;
	}
}
