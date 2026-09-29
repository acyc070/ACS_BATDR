using System;
using UnityEngine;

public class BreakableConnector : Breakable
{
	[Header("Connected Breakables")]
	[SerializeField]
	private Breakable[] m_Breakables;

	public Breakable[] Breakables => this.m_Breakables;

	public override void Start()
	{
		for (int i = 0; i < this.m_Breakables.Length; i = unchecked(i + 1))
		{
			Breakable obj;
			obj = this.m_Breakables[i];
			obj.OnHit += this.HandleBreakableOnHit;
			obj.OnBroken += this.HandleBreakableOnBroken;
		}
	}

	protected override void InternalForceComplete()
	{
		for (int i = 0; i < this.m_Breakables.Length; i = unchecked(i + 1))
		{
			this.m_Breakables[i].ForceComplete();
		}
	}

	private void HandleBreakableOnHit(object sender, EventArgs e)
	{
		for (int i = 0; i < this.m_Breakables.Length; i = unchecked(i + 1))
		{
			this.m_Breakables[i].OnHit -= this.HandleBreakableOnHit;
		}
		base.SendOnHit();
	}

	private void HandleBreakableOnBroken(object sender, EventArgs e)
	{
		for (int i = 0; i < this.m_Breakables.Length; i = unchecked(i + 1))
		{
			this.m_Breakables[i].OnBroken -= this.HandleBreakableOnBroken;
		}
		base.SendOnBroken();
	}

	protected override void OnDisposed()
	{
		if (this.m_Breakables != null)
		{
			for (int i = 0; i < this.m_Breakables.Length; i = unchecked(i + 1))
			{
				Breakable obj;
				obj = this.m_Breakables[i];
				obj.OnHit -= this.HandleBreakableOnHit;
				obj.OnBroken -= this.HandleBreakableOnBroken;
			}
		}
		base.OnDisposed();
	}
}
