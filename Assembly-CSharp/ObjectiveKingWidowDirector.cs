using System;
using UnityEngine;

public class ObjectiveKingWidowDirector : Objective
{
	[SerializeField]
	private KingWidowDirector m_Director;

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

	protected override void OnDisposed()
	{
		this.m_Director.OnComplete -= this.HandleDirectorOnComplete;
		base.OnDisposed();
	}
}
