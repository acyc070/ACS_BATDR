using System;
using UnityEngine;

public class Projectile : JMonoBehaviour
{
	private Rigidbody m_Rigidbody;

	public event EventHandler OnHit;

	public void Initialize()
	{
		if (this.m_Rigidbody == null)
		{
			this.m_Rigidbody = base.GetComponent<Rigidbody>();
		}
		if (!(this.m_Rigidbody == null))
		{
			this.m_Rigidbody.AddForce(base.transform.forward * 100f + Vector3.up * 5f, ForceMode.Impulse);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
		{
			RaycastHit hit;
			hit = new RaycastHit
			{
				point = base.transform.position - base.transform.forward * 4f
			};
			GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink", 8f).transform.transform.position = hit.point;
			if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				GameManager.Instance.Player.Hit(hit, null, DamageCheck.ProjectileInk());
			}
			this.OnHit.Send(this);
			if (this.m_Rigidbody != null)
			{
				this.m_Rigidbody.velocity = Vector3.zero;
				this.m_Rigidbody.angularVelocity = Vector3.zero;
			}
			base.gameObject.SetActive(value: false);
		}
	}

	protected override void OnDisposed()
	{
		this.OnHit = null;
		this.m_Rigidbody = null;
		base.OnDisposed();
	}
}
