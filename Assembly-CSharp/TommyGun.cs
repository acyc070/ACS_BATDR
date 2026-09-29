using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TommyGun : JMonoBehaviour
{
	[Header("Character")]
	[SerializeField]
	private CharacterContent m_CharacterContent;

	[SerializeField]
	private Transform m_CharacterForward;

	[SerializeField]
	private AnimationClip m_ReloadAnimationClip;

	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	[Header("Weapon")]
	[SerializeField]
	private Transform m_ProjectileLocation;

	[SerializeField]
	private GameObject m_Muzzle;

	[SerializeField]
	private float m_FireRate = 0.075f;

	[SerializeField]
	private float m_FireRateTarget = 0.1f;

	[SerializeField]
	private float m_FireSpread = 15f;

	[SerializeField]
	private float m_FireSpreadTarget = 1f;

	[SerializeField]
	private int m_AmmoCount = 250;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_HitPercentage;

	[SerializeField]
	private LayerMask m_LayerMask;

	[Header("Tracking")]
	[SerializeField]
	private float m_VisionRate = 0.5f;

	[SerializeField]
	private float m_VisionLostRate = 2f;

	[SerializeField]
	private float m_TrackingSpeed = 15f;

	[SerializeField]
	private Transform m_PivotBone;

	[SerializeField]
	private Vector3 m_ForwardOffset = Vector3.zero;

	[SerializeField]
	private Transform m_ArmLBone;

	[SerializeField]
	private Vector3 m_ArmLOffset = Vector3.zero;

	[SerializeField]
	private Transform m_ArmRBone;

	[SerializeField]
	private Vector3 m_ArmROffset = Vector3.zero;

	private Quaternion m_LastLookRotation;

	private Quaternion m_LastTargetRotation;

	private Quaternion m_LastArmLRotation;

	private Quaternion m_LastArmRRotation;

	private Vector3 m_LastKnownLocation;

	private float m_LostTime;

	private float m_LostTimeRate = 10f;

	private float m_FireTime;

	private float m_VisionTime;

	private float m_VisionLostTime;

	private int m_Ammo;

	private bool m_InRange;

	private bool m_HasTarget;

	private bool m_IsInVision;

	private bool m_IsVisible;

	public bool IsActive { get; private set; }

	public bool IsReloading { get; private set; }

	public event EventHandler OnFire;

	public event EventHandler OnReload;

	public event EventHandler OnLost;

	public event EventHandler OnSpotted;

	public override void OnEnable()
	{
		this.m_CharacterContent.GenericAnimationEvents.SetReciever(this);
		for (int i = 0; i < this.m_AnimationClipOverrideGroup.Length; i = unchecked(i + 1))
		{
			this.m_AnimationClipOverrideGroup[i].Initialize();
		}
		this.m_LastLookRotation = this.m_CharacterContent.transform.rotation;
		this.m_LastTargetRotation = this.m_PivotBone.rotation;
		this.m_LastArmLRotation = this.m_ArmLBone.rotation;
		this.m_LastArmRRotation = this.m_ArmRBone.rotation;
	}

	private void FixedUpdate()
	{
		if (this.IsReloading || this.m_CharacterContent == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		Vector3 vector;
		vector = this.m_CharacterContent.transform.position + Vector3.up * 6f;
		Vector3 direction;
		direction = GameManager.Instance.GameCamera.transform.position - vector;
		Vector3 position;
		position = GameManager.Instance.GameCamera.transform.position;
		position.y = this.m_CharacterContent.transform.position.y;
		Vector3 to;
		to = position - this.m_CharacterContent.transform.position;
		this.m_InRange = false;
		if (Physics.SphereCast(vector, 0.25f, direction, out var hitInfo, float.PositiveInfinity, this.m_LayerMask, QueryTriggerInteraction.Ignore))
		{
			if (GameManager.Instance.Player != null && hitInfo.transform == GameManager.Instance.Player.transform)
			{
				if (GameManager.Instance.Player.CombatStatus != CombatStatus.Stealth && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
				{
					this.SetTarget(active: true);
				}
				else if (!this.m_HasTarget)
				{
					if (this.IsInView(this.m_CharacterContent.transform.forward, to, 65f))
					{
						this.SetTarget(active: true);
					}
				}
				else
				{
					this.m_InRange = true;
				}
			}
			else
			{
				this.SetTarget(active: false);
			}
		}
		if (!this.IsActive)
		{
			return;
		}
		this.CheckVision();
		if (this.m_IsVisible)
		{
			this.Fire();
			return;
		}
		this.m_LostTime += Time.deltaTime;
		if (this.m_LostTime > this.m_LostTimeRate)
		{
			this.m_LostTime = 0f;
			this.OnLost.Send(this);
		}
	}

	private void LateUpdate()
	{
		if (!(GameManager.Instance.Player == null) && !(this.m_CharacterContent == null) && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			float num;
			num = Vector3.Distance(this.m_CharacterContent.transform.position, GameManager.Instance.Player.transform.position);
			Vector3 vector;
			vector = GameManager.Instance.GameCamera.transform.position;
			if (num < 15f)
			{
				vector = GameManager.Instance.Player.transform.position + Vector3.up * 2.5f;
			}
			Vector3 vector2;
			vector2 = ((!this.m_HasTarget) ? this.m_LastKnownLocation : vector);
			vector2.y = this.m_CharacterContent.transform.position.y;
			Vector3 vector3;
			vector3 = vector2 - this.m_CharacterContent.transform.position;
			Quaternion b;
			b = (this.m_IsVisible ? Quaternion.LookRotation(vector3) : this.m_CharacterForward.rotation);
			Quaternion b2;
			b2 = this.m_PivotBone.rotation;
			Quaternion b3;
			b3 = this.m_ArmLBone.rotation;
			Quaternion b4;
			b4 = this.m_ArmRBone.rotation;
			if (this.m_InRange && this.m_IsInVision && this.IsInView(this.m_CharacterContent.transform.forward, vector3, 45f))
			{
				Vector3 vector4;
				vector4 = ((!this.m_HasTarget || this.IsReloading) ? this.m_LastKnownLocation : GameManager.Instance.GameCamera.transform.position);
				b2 = Quaternion.LookRotation(vector4 - this.m_PivotBone.position) * Quaternion.Euler(this.m_ForwardOffset);
				b3 = Quaternion.LookRotation(vector4 - this.m_ArmLBone.position) * Quaternion.Euler(this.m_ArmLOffset);
				b4 = Quaternion.LookRotation(vector4 - this.m_ArmRBone.position) * Quaternion.Euler(this.m_ArmROffset);
			}
			this.m_CharacterContent.transform.rotation = Quaternion.Slerp(this.m_LastLookRotation, b, this.m_TrackingSpeed * Time.deltaTime);
			this.m_LastLookRotation = this.m_CharacterContent.transform.rotation;
			this.m_PivotBone.rotation = Quaternion.Slerp(this.m_LastTargetRotation, b2, this.m_TrackingSpeed * Time.deltaTime);
			this.m_LastTargetRotation = this.m_PivotBone.rotation;
			this.m_ArmLBone.rotation = Quaternion.Slerp(this.m_LastArmLRotation, b3, this.m_TrackingSpeed * Time.deltaTime);
			this.m_LastArmLRotation = this.m_ArmLBone.rotation;
			this.m_ArmRBone.rotation = Quaternion.Slerp(this.m_LastArmRRotation, b4, this.m_TrackingSpeed * Time.deltaTime);
			this.m_LastArmRRotation = this.m_ArmRBone.rotation;
		}
	}

	private void CheckVision()
	{
		if (!this.m_IsInVision)
		{
			if (this.m_InRange && this.m_HasTarget)
			{
				this.m_VisionTime += Time.deltaTime;
				if (this.m_VisionTime > this.m_VisionRate)
				{
					if (GameManager.Instance.Player.CombatStatus != CombatStatus.Stealth && GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
					{
						this.m_IsInVision = true;
					}
					else
					{
						this.SetTarget(active: false);
						this.m_IsInVision = false;
						this.m_VisionTime = 0f;
					}
				}
			}
			else
			{
				this.m_VisionTime = 0f;
			}
		}
		else
		{
			this.m_LastKnownLocation = GameManager.Instance.GameCamera.transform.position;
			if (!this.m_InRange || !this.m_HasTarget)
			{
				this.SetTarget(active: false);
				this.m_IsInVision = false;
				this.m_VisionTime = 0f;
			}
		}
		if (this.m_IsInVision && !this.m_IsVisible)
		{
			this.m_IsVisible = true;
			this.m_VisionLostTime = this.m_VisionLostRate;
			this.UpdateAnimationClips("Fire");
			this.OnSpotted.Send(this);
		}
		else if (!this.m_IsInVision && this.m_IsVisible)
		{
			this.m_VisionLostTime -= Time.deltaTime;
			if (this.m_VisionLostTime <= 0f)
			{
				this.m_VisionLostTime = this.m_VisionLostRate;
				this.m_IsVisible = false;
				this.m_LostTime = 0f;
				this.UpdateAnimationClips("Idle");
				this.OnLost.Send(this);
			}
		}
	}

	private void Fire()
	{
		float num;
		num = (this.m_InRange ? this.m_FireRateTarget : this.m_FireRate);
		this.m_FireTime += Time.deltaTime;
		if (!(this.m_FireTime > num))
		{
			return;
		}
		this.m_FireTime = 0f;
		unchecked
		{
			this.m_Ammo++;
			if (this.m_Ammo > this.m_AmmoCount)
			{
				this.ForceReload();
				return;
			}
			base.StopCoroutine(this.Muzzle());
			base.StartCoroutine(this.Muzzle());
			float num2;
			num2 = (this.m_InRange ? this.m_FireSpreadTarget : this.m_FireSpread);
			float x;
			x = global::UnityEngine.Random.Range(0f - num2, num2);
			float y;
			y = global::UnityEngine.Random.Range(0f - num2, num2);
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
						GameManager.Instance.Player.Hit(hitInfo, null, this.GetDamage());
					}
				}
				else if (componentInParent != null && !componentInParent.IsPlayerBreakable)
				{
					componentInParent.Hit(hitInfo);
				}
				GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Bullet", 5f).GetComponent<Impact>().Initialize(hitInfo, isPooled: true);
			}
			this.OnFire.Send(this);
		}
	}

	private int GetDamage()
	{
		int result;
		result = 1;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Easy:
			result = 1;
			break;
		case DifficultyLevel.Normal:
			result = 1;
			break;
		case DifficultyLevel.Hard:
			result = 2;
			break;
		case DifficultyLevel.Impossible:
			result = 3;
			break;
		}
		if (global::UnityEngine.Random.value < this.m_HitPercentage / 2f)
		{
			result = 0;
		}
		return result;
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

	private void Reload()
	{
		if (!this.IsReloading)
		{
			this.IsActive = false;
			this.IsReloading = true;
			this.m_Ammo = 0;
			this.UpdateAnimationClips("Reload");
			this.m_CharacterContent.Animator.SetTrigger("Interact");
			this.OnReload.Send(this);
		}
	}

	public void ReloadComplete()
	{
		this.IsActive = true;
		this.IsReloading = false;
	}

	public void ForceReload()
	{
		this.Reload();
	}

	private void SetTarget(bool active)
	{
		this.m_InRange = active;
		this.m_HasTarget = active;
	}

	public bool IsInView(Vector3 from, Vector3 to, float angle)
	{
		return Vector3.Angle(from, to) < angle;
	}

	public void SetActive(bool active)
	{
		this.IsActive = active;
	}

	public void UpdateAnimationClips(string name)
	{
		List<AnimationClip> list;
		list = new List<AnimationClip>();
		unchecked
		{
			for (int i = 0; i < this.m_AnimationClipOverrideGroup.Length; i++)
			{
				AnimationClipOverrideGroup animationClipOverrideGroup;
				animationClipOverrideGroup = this.m_AnimationClipOverrideGroup[i];
				if (animationClipOverrideGroup.Name == name)
				{
					for (int j = 0; j < animationClipOverrideGroup.AnimationClipGroups.Length; j++)
					{
						list.Add(animationClipOverrideGroup.AnimationClipGroups[j].AnimationClip);
					}
					break;
				}
			}
			this.m_CharacterContent.UpdateClipOverrides(list.ToArray());
		}
	}

	protected override void OnDisposed()
	{
		this.OnFire = null;
		this.OnReload = null;
		this.OnLost = null;
		this.OnSpotted = null;
		base.OnDisposed();
	}
}
