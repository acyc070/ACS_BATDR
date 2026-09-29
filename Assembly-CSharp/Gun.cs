using System;
using System.Collections;
using UnityEngine;

public class Gun : JMonoBehaviour
{
	[Header("Weapon")]
	[SerializeField]
	private Transform m_ProjectileLocation;

	[SerializeField]
	private GameObject m_Muzzle;

	[SerializeField]
	private float m_FireRate = 0.125f;

	[SerializeField]
	private float m_FireSpread = 15f;

	[SerializeField]
	private int m_AmmoCount = -1;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_HitPercentage = 0.2f;

	[SerializeField]
	private LayerMask m_LayerMask;

	private float m_FireTime;

	private int m_Ammo;

	public bool IsActive { get; private set; }

	public bool IsReloading { get; private set; }

	public event EventHandler OnFire;

	public event EventHandler OnReload;

	private void FixedUpdate()
	{
		if (!this.IsReloading && this.IsActive && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			this.Fire();
		}
	}

	private void Fire()
	{
		float fireRate;
		fireRate = this.m_FireRate;
		this.m_FireTime += Time.deltaTime;
		unchecked
		{
			if (this.m_FireTime > fireRate)
			{
				this.m_FireTime = 0f;
				this.m_Ammo++;
				if (this.m_AmmoCount > -1 && this.m_Ammo > this.m_AmmoCount)
				{
					this.ForceReload();
				}
				else
				{
					this.InternalFire();
				}
			}
		}
	}

	public void ForceFire(bool hasSound)
	{
		this.InternalFire(hasSound);
	}

	private void InternalFire(bool hasSound = true)
	{
		base.StopCoroutine(this.Muzzle());
		base.StartCoroutine(this.Muzzle());
		float fireSpread;
		fireSpread = this.m_FireSpread;
		float x;
		x = global::UnityEngine.Random.Range(0f - fireSpread, fireSpread);
		float y;
		y = global::UnityEngine.Random.Range(0f - fireSpread, fireSpread);
		this.m_ProjectileLocation.localEulerAngles = new Vector3(x, y, 0f);
		if (Physics.Raycast(this.m_ProjectileLocation.position, this.m_ProjectileLocation.forward, out var hitInfo, float.PositiveInfinity, this.m_LayerMask, QueryTriggerInteraction.Ignore))
		{
			Vector3 vector;
			vector = Vector3.Lerp(this.m_ProjectileLocation.position, hitInfo.point, 0.8f);
			LineRenderer component;
			component = GameManager.Instance.PoolingManager.GetFromPool("Projectiles/Projectile_Bullet_Line", 0.02f).transform.GetComponent<LineRenderer>();
			component.transform.position = this.m_ProjectileLocation.position;
			component.SetPositions(new Vector3[2]
			{
				this.m_ProjectileLocation.position,
				vector
			});
			IHittable componentInParent;
			componentInParent = hitInfo.transform.GetComponentInParent<IHittable>();
			if (GameManager.Instance.Player != null && hitInfo.transform == GameManager.Instance.Player.transform)
			{
				if (global::UnityEngine.Random.value < this.m_HitPercentage)
				{
					int damage;
					damage = ((global::UnityEngine.Random.value < this.m_HitPercentage) ? 1 : 0);
					GameManager.Instance.Player.Hit(hitInfo, null, damage);
				}
			}
			else if (componentInParent != null && !componentInParent.IsPlayerBreakable)
			{
				componentInParent.Hit(hitInfo);
			}
			GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Bullet", 5f).GetComponent<Impact>().Initialize(hitInfo, isPooled: true);
		}
		if (hasSound)
		{
			this.OnFire.Send(this);
		}
	}

	private IEnumerator Muzzle()
	{
		this.m_Muzzle.SetActive(value: true);
		this.m_Muzzle.transform.localEulerAngles = new Vector3(0f, 0f, global::UnityEngine.Random.Range(0f, 360f));
		yield return new WaitForSeconds(0.015f);
		if (base.IsDisposed)
		{
			yield return null;
		}
		this.m_Muzzle.SetActive(value: false);
	}

	public void ForceReload()
	{
		if (!this.IsReloading)
		{
			base.StartCoroutine(this.Reload());
		}
	}

	private IEnumerator Reload()
	{
		this.IsReloading = true;
		this.IsActive = false;
		this.m_Ammo = 0;
		this.OnReload.Send(this);
		yield return new WaitForSeconds(3f);
		if (base.IsDisposed)
		{
			yield return null;
		}
		this.IsActive = true;
		this.IsReloading = false;
	}

	public bool IsInView(Vector3 from, Vector3 to, float angle)
	{
		return Vector3.Angle(from, to) < angle;
	}

	public void SetActive(bool active)
	{
		this.IsActive = active;
	}

	protected override void OnDisposed()
	{
		this.OnFire = null;
		this.OnReload = null;
		base.OnDisposed();
	}
}
