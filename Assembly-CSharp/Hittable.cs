using System;
using UnityEngine;

public class Hittable : JMonoBehaviour, IHittable
{
	public bool IsBroken { get; private set; }

	public bool IsPlayerBreakable => true;

	public event EventHandler OnHit;

	public void Hit(RaycastHit hit)
	{
		if (this.InternalHit(hit))
		{
			this.OnHit.Send(this);
		}
	}

	protected virtual bool InternalHit(RaycastHit hit)
	{
		return true;
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		base.OnDisposed();
	}
}
