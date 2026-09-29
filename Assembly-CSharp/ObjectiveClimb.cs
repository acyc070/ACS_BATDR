using System;
using UnityEngine;

public class ObjectiveClimb : Objective
{
	[SerializeField]
	private Climb m_Climb;

	[SerializeField]
	private bool m_ClearNavigation = true;

	[SerializeField]
	private bool m_ForceComplete;

	protected override void InternalInitialize()
	{
		base.Enable();
	}

	protected override void InternalEnable()
	{
		this.AddListeners();
		this.m_Climb.Enable();
	}

	protected override void InternalInactive()
	{
		this.m_Climb.Inactive();
		base.SendOnComplete();
	}

	protected override void InternalDisable()
	{
		this.m_Climb.Disable();
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
	}

	protected override void InternalForceComplete()
	{
		if (this.m_ForceComplete)
		{
			this.m_Climb.ForceActivateComplete();
			base.SendOnComplete();
		}
		else
		{
			base.Inactive();
		}
	}

	private void HandleClimbOnInteract(object sender, EventArgs e)
	{
		this.m_Climb.OnInteract -= this.HandleClimbOnInteract;
		GameManager.Instance.ClearNavigation();
	}

	private void HandleClimbOnComplete(object sender, EventArgs e)
	{
		this.RemoveListeners();
		base.SendOnComplete();
	}

	protected void AddListeners()
	{
		if (this.m_ClearNavigation)
		{
			this.m_Climb.OnInteract += this.HandleClimbOnInteract;
		}
		this.m_Climb.OnComplete += this.HandleClimbOnComplete;
	}

	protected override void RemoveListeners()
	{
		if (this.m_ClearNavigation)
		{
			this.m_Climb.OnInteract -= this.HandleClimbOnInteract;
		}
		this.m_Climb.OnComplete -= this.HandleClimbOnComplete;
	}
}
