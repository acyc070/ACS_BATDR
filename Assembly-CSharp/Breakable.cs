using System;
using UnityEngine;

public class Breakable : JMonoBehaviour, IHittable
{
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private bool m_IsCollision;

	[Header("Breakable Base Settings")]
	[SerializeField]
	private bool m_IsPlayerBreakable = true;

	[SerializeField]
	private bool m_DisposeOnHit = true;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	private bool m_IsTriggered;

	public bool IsBroken { get; private set; }

	public bool IsPlayerBreakable => this.m_IsPlayerBreakable;

	public event EventHandler OnHit;

	public event EventHandler OnBroken;

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	public void Hit(RaycastHit hit)
	{
		if (this.m_IsActive && !this.IsBroken)
		{
			this.InternalHit(hit);
		}
	}

	protected virtual void InternalHit(RaycastHit hit)
	{
		this.HitOnComplete();
		if (this.m_ActiveSetter != null)
		{
			this.m_ActiveSetter.SetActive(active: true);
			Collider[] components;
			components = base.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i = unchecked(i + 1))
			{
				components[i].enabled = false;
			}
		}
		if (this.m_IsTriggered)
		{
			Rigidbody component;
			component = base.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.detectCollisions = false;
			}
		}
		this.BrokenOnComplete();
		if (this.m_DisposeOnHit)
		{
			base.Dispose();
		}
	}

	protected void HitOnComplete()
	{
		if (!base.IsDisposed)
		{
			this.SendOnHit();
		}
	}

	protected void BrokenOnComplete()
	{
		if (!base.IsDisposed)
		{
			this.IsBroken = true;
			this.SendOnBroken();
		}
	}

	public void SendOnHit()
	{
		this.OnHit.Send(this);
	}

	public void SendOnBroken()
	{
		this.OnBroken.Send(this);
	}

	public void ForceComplete()
	{
		if (this.m_ActiveSetter != null)
		{
			this.m_ActiveSetter.ForceComplete();
		}
		Collider[] components;
		components = base.GetComponents<Collider>();
		for (int i = 0; i < components.Length; i = unchecked(i + 1))
		{
			components[i].enabled = false;
		}
		this.IsBroken = true;
		this.InternalForceComplete();
		if (this.m_DisposeOnHit)
		{
			base.Dispose();
		}
	}

	protected virtual void InternalForceComplete()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.m_IsCollision && !this.IsBroken && !base.IsDisposed && other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			RaycastHit hit;
			hit = new RaycastHit
			{
				point = base.transform.position
			};
			this.m_IsTriggered = true;
			this.InternalHit(hit);
		}
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		this.OnBroken = null;
		base.OnDisposed();
	}
}
