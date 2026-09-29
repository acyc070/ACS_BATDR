using System;
using UnityEngine;

public class ObjectiveBreakable : Objective
{
	[SerializeField]
	private Breakable m_Breakable;

	protected override void InternalInitialize()
	{
		if (!(this.m_Breakable == null))
		{
			this.m_Breakable.OnHit -= this.HandleBreakableOnHit;
			this.m_Breakable.OnHit += this.HandleBreakableOnHit;
			this.m_Breakable.SetActive(active: true);
		}
	}

	private void HandleBreakableOnHit(object sender, EventArgs e)
	{
		this.m_Breakable.OnHit -= this.HandleBreakableOnHit;
		base.SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (this.m_Breakable != null)
		{
			this.m_Breakable.OnHit -= this.HandleBreakableOnHit;
		}
	}
}
