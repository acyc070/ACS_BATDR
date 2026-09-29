using System;
using S13Audio.BATDR;
using UnityEngine;

public class UnderWaterController : JMonoBehaviour
{
	[SerializeField]
	private EventTrigger m_EventTrigger;

	private bool m_IsActive;

	private bool m_IsUnderWater;

	private float m_UnderWaterTimer;

	private float m_UnderWaterRate = 1.5f;

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnDrowning;

	public event EventHandler OnDeath;

	public override void Start()
	{
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.OnEnter += this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.OnExit -= this.HandleEventTriggerOnExit;
		this.m_EventTrigger.OnExit += this.HandleEventTriggerOnExit;
	}

	private void HandleEventTriggerOnExit(object sender, EventArgs e)
	{
		this.m_IsActive = false;
		GameManager.Instance.GameCamera.SetUnderWater(active: false);
		if (this.m_IsUnderWater)
		{
			GameManager.Instance.Player.ShowWeapon();
			GameManager.Instance.Player.PlayerMovement.ResetMoveSpeed();
			GameManager.Instance.Player.PlayerMovement.UnlockJump();
			BATDRPlayerHealthAudioController.SetPlayerHealth(GameManager.Instance.Player.Health);
			this.OnExit.Send(this);
		}
		this.m_IsUnderWater = false;
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		this.m_IsActive = true;
	}

	private void Update()
	{
		if (!this.m_IsActive)
		{
			return;
		}
		if (GameManager.Instance.GameCamera.transform.position.y <= base.transform.position.y + 0.01f)
		{
			if (!this.m_IsUnderWater)
			{
				this.m_IsUnderWater = true;
				GameManager.Instance.GameCamera.SetUnderWater(active: true);
				GameManager.Instance.Player.HideWeapon();
				GameManager.Instance.Player.PlayerMovement.SetMoveSpeed(0.5f, _isSlowed: true);
				GameManager.Instance.Player.PlayerMovement.LockJump();
				GameManager.Instance.Player.PlayerMovement.ForceLockRun();
				GameManager.Instance.HideSprintBar();
				this.OnEnter.Send(this);
			}
		}
		else if (this.m_IsUnderWater)
		{
			this.m_IsUnderWater = false;
			GameManager.Instance.GameCamera.SetUnderWater(active: false);
			BATDRPlayerHealthAudioController.SetPlayerHealth(GameManager.Instance.Player.Health);
			GameManager.Instance.Player.ShowWeapon();
			GameManager.Instance.Player.PlayerMovement.ResetMoveSpeed();
			GameManager.Instance.Player.PlayerMovement.UnlockJump();
			GameManager.Instance.Player.PlayerMovement.ForceUnlockRun();
			this.OnExit.Send(this);
		}
		if (!this.m_IsUnderWater)
		{
			return;
		}
		this.m_UnderWaterTimer += Time.deltaTime;
		if (this.m_UnderWaterTimer >= this.m_UnderWaterRate)
		{
			this.m_UnderWaterTimer = 0f;
			float num;
			num = GameManager.Instance.Player.Health - 1f;
			if (num <= 0f)
			{
				GameManager.Instance.Player.ForceRespawn();
				GameManager.Instance.Player.ShowWeapon();
				GameManager.Instance.GameCamera.SetUnderWater(active: false);
				this.m_IsActive = false;
				this.m_IsUnderWater = false;
				this.m_EventTrigger.ResetAction();
				this.OnDeath.Send(this);
			}
			else
			{
				CameraEffects.ShakeRotation(1f, 0.5f);
				CameraEffects.Damage((UpgradeCheck.GetHealth() - num) / 4f);
				GameManager.Instance.Player.SetHealth(unchecked((int)num), isSilent: true);
				this.OnDrowning.Send(this);
			}
		}
	}

	protected override void OnDisposed()
	{
		this.OnEnter = null;
		this.OnExit = null;
		this.OnDrowning = null;
		this.OnDeath = null;
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.OnExit -= this.HandleEventTriggerOnExit;
		base.OnDisposed();
	}
}
