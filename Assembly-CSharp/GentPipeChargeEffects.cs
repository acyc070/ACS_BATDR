using System;
using UnityEngine;

public class GentPipeChargeEffects : JMonoBehaviour
{
	[Header("Charges")]
	[SerializeField]
	private Transform m_ChargeLevel;

	[Header("Particles")]
	[SerializeField]
	private ParticleSystem m_ChargeParticles;

	[SerializeField]
	private ParticleSystem m_OverchargeParticles;

	private float m_ChargeLength = 6f;

	private float m_ChargeTimer;

	private bool m_IsCharged;

	private bool m_IsOvercharged;

	public Transform ChargeLevel => this.m_ChargeLevel;

	public bool IsCharged => this.m_IsCharged;

	public bool IsOvercharged => this.m_IsOvercharged;

	public event EventHandler OnCharge;

	public event EventHandler OnEnd;

	public event EventHandler OnOvercharge;

	public override void Awake()
	{
		this.Disable();
	}

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused || this.m_ChargeLevel == null)
		{
			return;
		}
		if (this.m_IsCharged)
		{
			Vector3 localScale;
			localScale = this.m_ChargeLevel.localScale;
			localScale.y = this.m_ChargeTimer / this.m_ChargeLength;
			this.m_ChargeLevel.localScale = localScale;
			if (this.m_ChargeTimer <= 0f)
			{
				this.m_IsCharged = false;
				this.m_IsOvercharged = false;
				this.m_ChargeTimer = 0f;
				this.Disable();
				this.OnEnd.Send(this);
				return;
			}
		}
		this.m_ChargeTimer -= Time.deltaTime;
	}

	public void SetChargeLevel(Transform chargeLevel)
	{
		this.m_ChargeLevel = chargeLevel;
		this.Disable();
	}

	public void Enable()
	{
		if (this.m_ChargeLevel != null)
		{
			if (this.m_IsCharged)
			{
				this.m_IsOvercharged = true;
			}
			else
			{
				this.m_IsOvercharged = false;
			}
			this.m_IsCharged = true;
			this.m_ChargeTimer = this.m_ChargeLength;
			this.m_ChargeLevel.localScale = Vector3.one;
			this.m_ChargeLevel.gameObject.SetActive(value: true);
		}
		if (this.m_IsOvercharged)
		{
			if (this.m_OverchargeParticles != null)
			{
				this.m_OverchargeParticles.Play();
			}
			this.OnOvercharge.Send(this);
		}
		else if (this.m_IsCharged)
		{
			if (this.m_ChargeParticles != null)
			{
				this.m_ChargeParticles.Play();
			}
			this.OnCharge.Send(this);
		}
	}

	public void Disable()
	{
		if (this.m_ChargeParticles != null)
		{
			this.m_ChargeParticles.Stop();
		}
		if (this.m_OverchargeParticles != null)
		{
			this.m_OverchargeParticles.Stop();
		}
		if (this.m_ChargeLevel != null)
		{
			this.m_ChargeLevel.gameObject.SetActive(value: false);
		}
	}

	public void Hit()
	{
		this.m_ChargeTimer -= 0.5f;
		if (this.m_ChargeTimer <= 0f)
		{
			this.m_ChargeTimer = 0.0001f;
		}
	}

	protected override void OnDisposed()
	{
		this.OnCharge = null;
		this.OnEnd = null;
		this.OnOvercharge = null;
		base.OnDisposed();
	}
}
