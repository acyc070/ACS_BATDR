using System;
using UnityEngine;

public class ObjectiveHambush : Objective
{
	[SerializeField]
	private HambushController m_HambushController;

	protected override void InternalInitialize()
	{
		this.m_HambushController.OnComplete -= this.HandleHambushControllerOnComplete;
		this.m_HambushController.OnComplete += this.HandleHambushControllerOnComplete;
		this.m_HambushController.Initialize();
	}

	private void HandleHambushControllerOnComplete(object sender, EventArgs e)
	{
		this.m_HambushController.OnComplete -= this.HandleHambushControllerOnComplete;
		base.SendOnComplete();
	}

	protected override void OnDisposed()
	{
		this.m_HambushController.OnComplete -= this.HandleHambushControllerOnComplete;
		base.OnDisposed();
	}
}
