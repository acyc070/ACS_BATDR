using System;
using UnityEngine;

public class ObjectiveBreakables : Objective
{
	[Header("Breakables")]
	[SerializeField]
	private Breakable[] m_Breakables;

	protected override void InternalInitialize()
	{
		if (this.m_Breakables != null && this.m_Breakables.Length != 0)
		{
			for (int i = 0; i < this.m_Breakables.Length; i = unchecked(i + 1))
			{
				Breakable obj;
				obj = this.m_Breakables[i];
				obj.OnHit -= this.HandleBreakableOnHit;
				obj.OnHit += this.HandleBreakableOnHit;
				obj.SetActive(active: true);
			}
		}
	}

	private void HandleBreakableOnHit(object sender, EventArgs e)
	{
		this.RemoveListeners();
		base.SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (this.m_Breakables != null)
		{
			for (int i = 0; i < this.m_Breakables.Length; i = unchecked(i + 1))
			{
				this.m_Breakables[i].OnHit -= this.HandleBreakableOnHit;
			}
		}
	}
}
