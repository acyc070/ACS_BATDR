using System;
using UnityEngine;

public class BossPhaseController : JMonoBehaviour
{
	[SerializeField]
	private BossPhase[] m_BossPhases;

	private int m_PhaseCount;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		this.InitializePhase();
	}

	private void InitializePhase()
	{
		BossPhase obj;
		obj = this.m_BossPhases[this.m_PhaseCount];
		obj.OnEnd -= this.HandleBossPhaseOnEnd;
		obj.OnEnd += this.HandleBossPhaseOnEnd;
		obj.Initialize();
	}

	private void HandleBossPhaseOnEnd(object sender, EventArgs e)
	{
		(sender as BossPhase).OnEnd -= this.HandleBossPhaseOnEnd;
		unchecked
		{
			this.m_PhaseCount++;
			if (this.m_PhaseCount >= this.m_BossPhases.Length)
			{
				this.OnComplete.Send(this);
			}
			else
			{
				this.InitializePhase();
			}
		}
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		if (this.m_BossPhases != null)
		{
			for (int i = 0; i < this.m_BossPhases.Length; i = unchecked(i + 1))
			{
				this.m_BossPhases[i].OnEnd -= this.HandleBossPhaseOnEnd;
			}
		}
		base.OnDisposed();
	}
}
