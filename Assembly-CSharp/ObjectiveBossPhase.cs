using System;
using UnityEngine;

public class ObjectiveBossPhase : Objective
{
	[SerializeField]
	private BossPhaseController m_BossPhaseController;

	protected override void InternalInitialize()
	{
		this.m_BossPhaseController.OnComplete -= this.HandleBossPhaseControllerOnComplete;
		this.m_BossPhaseController.OnComplete += this.HandleBossPhaseControllerOnComplete;
	}

	private void HandleBossPhaseControllerOnComplete(object sender, EventArgs e)
	{
		this.m_BossPhaseController.OnComplete -= this.HandleBossPhaseControllerOnComplete;
		base.SendOnComplete();
	}
}
