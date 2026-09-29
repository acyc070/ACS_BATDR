using System;
using UnityEngine;

public class ObjectiveRadio : Objective
{
	[SerializeField]
	private Radio m_Radio;

	[SerializeField]
	private Door m_Door;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		this.InitializeObjective();
	}

	protected override void InternalForceComplete()
	{
		this.InitializeObjective();
	}

	private void HandleRadioOnInteract(object sender, EventArgs e)
	{
		this.m_Door.Content.ForceActivate();
	}

	private void HandleRadioOnComplete(object sender, EventArgs e)
	{
		this.m_Door.Content.ForceDeactivate();
	}

	private void InitializeObjective()
	{
		this.m_Door.Content.ForceDeactivateComplete();
		this.RemoveListeners();
		this.m_Radio.Content.OnInteract += this.HandleRadioOnInteract;
		this.m_Radio.Content.OnComplete += this.HandleRadioOnComplete;
	}

	protected override void RemoveListeners()
	{
		this.m_Radio.Content.OnInteract -= this.HandleRadioOnInteract;
		this.m_Radio.Content.OnComplete -= this.HandleRadioOnComplete;
	}
}
