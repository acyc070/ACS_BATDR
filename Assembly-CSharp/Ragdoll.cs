using System;
using UnityEngine;

public class Ragdoll : JMonoBehaviour, IHittable
{
	private Rigidbody m_Rigidbody;

	private Collider m_Collider;

	private bool m_IsActive;

	private bool m_IsColor;

	public Rigidbody Rigidbody => this.m_Rigidbody;

	public Collider Collider => this.m_Collider;

	private bool m_HasRigidbody => this.m_Rigidbody != null;

	private bool m_HasCollider => this.m_Collider != null;

	public bool IsBroken { get; private set; }

	public bool IsPlayerBreakable => true;

	public bool IsActive => this.m_IsActive;

	public event EventHandler OnHit;

	public override void Awake()
	{
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
		this.m_Collider = base.GetComponent<Collider>();
		this.DisableRigidobdy();
		this.DisableCollider();
	}

	public void Enable()
	{
		this.EnableRigidbody();
		this.EnableCollider();
	}

	public void Disable()
	{
		this.DisableRigidobdy();
	}

	public Ragdoll Activate(bool isColor = false)
	{
		this.m_IsColor = isColor;
		this.EnableCollider();
		this.EnableRigidbody();
		return this;
	}

	public void AddForce(float force, Vector3 position, float radius, float upwardsModifier)
	{
		this.m_Rigidbody.AddExplosionForce(force * 30f, position, radius, upwardsModifier, ForceMode.Force);
	}

	public void EnableCollider()
	{
		if (this.m_HasCollider)
		{
			this.m_Collider.enabled = true;
		}
	}

	public void DisableCollider()
	{
		if (this.m_HasCollider)
		{
			this.m_Collider.enabled = false;
		}
	}

	public void EnableRigidbody()
	{
		if (this.m_HasRigidbody)
		{
			this.m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			this.m_Rigidbody.isKinematic = false;
			this.m_IsActive = true;
		}
	}

	public void DisableRigidobdy()
	{
		if (this.m_HasRigidbody)
		{
			this.m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			this.m_Rigidbody.isKinematic = true;
			this.m_Rigidbody.Sleep();
			this.m_IsActive = false;
		}
	}

	public void Hit(RaycastHit hit)
	{
		string prefab;
		prefab = "Impacts/Impact_Ink";
		if (this.m_IsColor)
		{
			prefab = "Impacts/Impact_Ink_Color";
		}
		GameManager.Instance.PoolingManager.GetFromPool(prefab, 6f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		this.OnHit.Send(this);
		this.AddForce(50f, hit.point, 20f, 2f);
	}

	protected override void OnDisposed()
	{
		this.m_Rigidbody = null;
		this.m_Collider = null;
		base.OnDisposed();
	}
}
