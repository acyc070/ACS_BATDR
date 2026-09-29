using System;
using DG.Tweening;
using UnityEngine;

public class TourBoxContent : ActionEventContent<TourBoxContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public LightBulbController Light;
	}

	private AudioClip m_AudioClip;

	private float m_AudioClipTimer;

	private float m_AudioClipLength;

	public bool IsAudioPlaying { get; private set; }

	public event EventHandler OnActivateContent;

	public event EventHandler OnDeactivateContent;

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.IsAudioPlaying)
		{
			if (this.m_AudioClipTimer > this.m_AudioClipLength + 0.5f)
			{
				this.ResetAudio();
			}
			else
			{
				this.m_AudioClipTimer += Time.deltaTime;
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			base.CombatDisable();
		}
		else
		{
			base.CombatEnable();
		}
	}

	protected override void OnInitialized()
	{
		base.Connectable.OnActivated -= this.HandleConnectableOnActivated;
		base.Connectable.OnActivated += this.HandleConnectableOnActivated;
	}

	protected override void OnActivate()
	{
		this.OnActivateContent.Send(this);
		base.m_Sequencer?.New();
		int animatorHash;
		animatorHash = this.GetHashCode();
		for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
		{
			this.ActivationDOTween(base.m_ActiveProperties[i], ref animatorHash);
		}
	}

	private void ActivationDOTween(Properties properties, ref int animatorHash)
	{
		if (animatorHash != properties.Animator.GetHashCode())
		{
			base.DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, 0.3f, Ease.OutBack);
		}
		animatorHash = properties.Animator.GetHashCode();
	}

	protected override void OnDeactivate()
	{
		this.OnDeactivateContent.Send(this);
		base.m_Sequencer?.New();
		int animatorHash;
		animatorHash = this.GetHashCode();
		for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
		{
			this.DeactivationDOTween(base.m_ActiveProperties[i], ref animatorHash);
		}
	}

	private void DeactivationDOTween(Properties properties, ref int animatorHash)
	{
		if (animatorHash != properties.Animator.GetHashCode())
		{
			base.DoAnimation(properties, properties.m_OriginPosition, properties.m_OriginRotation, 0.3f, Ease.InBack);
		}
		animatorHash = properties.Animator.GetHashCode();
	}

	public void SetAudioClip(AudioClip audioClip)
	{
		this.m_AudioClip = audioClip;
		this.m_AudioClipLength = this.m_AudioClip.length;
	}

	private void PlayAudioClip()
	{
		if (this.m_AudioClip != null)
		{
			for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
			{
				Properties properties;
				properties = base.m_ActiveProperties[i];
				properties.ActionEvent.SetActive(active: false);
				GameManager.Instance.Player.Interaction.ResetInteraction();
				properties.Light.TurnOn();
				this.m_AudioClipLength = this.m_AudioClip.length;
				this.IsAudioPlaying = true;
			}
		}
	}

	private void ResetAudio()
	{
		if (this.m_AudioClip != null)
		{
			for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
			{
				Properties properties;
				properties = base.m_ActiveProperties[i];
				this.IsAudioPlaying = false;
				this.m_AudioClipTimer = 0f;
				this.m_AudioClipLength = 0f;
				properties.Light.TurnOff();
				properties.ActionEvent.ResetAction();
				properties.ActionEvent.SetActive(active: true);
			}
			this.Deactivate();
		}
	}

	private void HandleConnectableOnActivated(object sender, EventArgs e)
	{
		this.PlayAudioClip();
	}

	protected override void OnDisposed()
	{
		this.OnActivateContent = null;
		this.OnDeactivateContent = null;
		base.Connectable.OnActivated -= this.HandleConnectableOnActivated;
		base.OnDisposed();
	}
}
