using System;
using UnityEngine;

public class ObjectiveShipAhoyDirector : Objective
{
	[SerializeField]
	private ShipAhoyDirector m_Director;

	[SerializeField]
	private Transform m_StartLocation;

	protected override void InternalInitialize()
	{
		this.m_Director.OnComplete -= this.HandleDirectorOnComplete;
		this.m_Director.OnComplete += this.HandleDirectorOnComplete;
		this.m_Director.Initialize(this.m_StartLocation);
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		this.m_Director.OnComplete -= this.HandleDirectorOnComplete;
		base.SendOnComplete();
	}

	protected override void InternalDisable()
	{
		this.m_Director.Deactivate();
	}

	protected override void OnDisposed()
	{
		this.m_Director.OnComplete -= this.HandleDirectorOnComplete;
		base.OnDisposed();
	}
}
