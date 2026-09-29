using System;
using UnityEngine;

public class InsaneHitAudio : JMonoBehaviour
{
	public event EventHandler OnHit;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > 6f)
		{
			this.OnHit.Send(this);
		}
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		base.OnDisposed();
	}
}
