using System;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRPlayerHealth : MonoBehaviour
{
	private const string healthAccessorID = "Player_Health";

	[SerializeField]
	[Tooltip("Percentage at which health events occur")]
	[Range(0f, 100f)]
	private int threshold = 50;

	[SerializeField]
	private float maxTimerSpeed = 4f;

	[SerializeField]
	private float minTimerSpeed = 1f;

	[SerializeField]
	private float recoverySpeed = 3.5f;

	[Header("Actions")]
	[SerializeField]
	private S13LocalAction enterHurtState;

	[SerializeField]
	private S13LocalAction exitHurtState;

	[SerializeField]
	private S13LocalAction timerTick;

	[Header("Debug")]
	[SerializeField]
	[S13ReadOnlyField]
	private float timerSpeed;

	[SerializeField]
	[S13ReadOnlyField]
	private float heartbeatTimer;

	[SerializeField]
	[S13ReadOnlyField]
	private bool isPlayerHurt;

	private float lastValue;

	private float healthThreshold => (float)this.threshold / 100f;

	private void OnEnable()
	{
		BATDRPlayerHealthAudioController.OnHealthChanged = (Action<float>)Delegate.Combine(BATDRPlayerHealthAudioController.OnHealthChanged, new Action<float>(this.OnHealthChanged));
	}

	private void Update()
	{
		if (this.isPlayerHurt)
		{
			this.heartbeatTimer -= Time.deltaTime;
			if (this.heartbeatTimer <= 0f)
			{
				this.timerTick.Execute();
				this.heartbeatTimer = this.timerSpeed;
			}
			this.timerSpeed += this.recoverySpeed * Time.deltaTime;
			if (this.timerSpeed > this.maxTimerSpeed)
			{
				this.ExitHurtState();
			}
		}
	}

	private void OnDisable()
	{
		BATDRPlayerHealthAudioController.OnHealthChanged = (Action<float>)Delegate.Remove(BATDRPlayerHealthAudioController.OnHealthChanged, new Action<float>(this.OnHealthChanged));
	}

	private void SetTimer(float newHealthValue)
	{
		this.timerSpeed = (this.heartbeatTimer = this.ScaleHealthValue(newHealthValue));
	}

	private void OnHealthChanged(float newHealthValue)
	{
		if (newHealthValue <= this.healthThreshold && newHealthValue < this.lastValue)
		{
			this.EnterHurtState();
		}
		else if (this.isPlayerHurt && newHealthValue > this.healthThreshold)
		{
			this.ExitHurtState();
		}
		this.lastValue = newHealthValue;
		this.SetTimer(newHealthValue);
		S13Manager.SetAccessor("Player_Health", newHealthValue);
	}

	private void EnterHurtState()
	{
		if (!this.isPlayerHurt)
		{
			this.isPlayerHurt = true;
			this.enterHurtState.Execute();
		}
	}

	private void ExitHurtState()
	{
		if (this.isPlayerHurt)
		{
			this.isPlayerHurt = false;
			this.exitHurtState.Execute();
			S13Manager.SetAccessor("Player_Health", 1f);
		}
	}

	private float ScaleHealthValue(float newHealthValue)
	{
		float num;
		num = this.maxTimerSpeed - this.minTimerSpeed + this.minTimerSpeed;
		return newHealthValue / 10f * num;
	}
}
