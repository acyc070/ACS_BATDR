using System;
using UnityEngine;

public class ObjectiveSwitch : Objective
{
	[SerializeField]
	private Switch m_Switch;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		this.RemoveListeners();
		this.AddListeners();
		this.m_Switch.Content.Enable();
	}

	protected override void InternalDisable()
	{
		base.Inactive();
	}

	protected override void InternalInactive()
	{
		this.RemoveListeners();
		this.m_Switch.Content.Disable();
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
		this.m_Switch.Content.Disable();
	}

	protected override void InternalForceComplete()
	{
		this.m_Switch.Content.ForceActivateComplete();
		base.SendOnComplete();
	}

	private void HandleActionEventControllerOnActivated(object sender, EventArgs e)
	{
		this.m_Switch.OnActivated -= this.HandleActionEventControllerOnActivated;
		base.SendOnComplete();
	}

	protected void AddListeners()
	{
		this.m_Switch.OnActivated += this.HandleActionEventControllerOnActivated;
	}

	protected override void RemoveListeners()
	{
		this.m_Switch.OnActivated -= this.HandleActionEventControllerOnActivated;
	}
}
