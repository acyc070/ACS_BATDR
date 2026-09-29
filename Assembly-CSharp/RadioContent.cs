using System;
using System.Collections.Generic;
using S13Audio.BATDR;
using UnityEngine;

public class RadioContent : JMonoBehaviour
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private BATDRRadioPlayer m_radioPlayer;

	[SerializeField]
	private AudioClip[] m_AudioClips;

	private List<AudioClip> m_AvailableAudioClips = new List<AudioClip>();

	private AudioClip m_CurrentAudioClip;

	private float m_AudioClipTimer;

	private float m_AudioClipLength;

	private bool m_IsPlaying;

	public event EventHandler OnInteract;

	public event EventHandler OnComplete;

	public override void Start()
	{
		if (this.m_AudioClips.Length != 0)
		{
			this.SetAudioClips();
			this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
			this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_Interactable.SetActive(active: false);
		GameManager.Instance.Player.Interaction.ResetInteraction();
		this.GetNextAudioClip();
		this.m_IsPlaying = true;
		this.OnInteract.Send(this);
	}

	private void Update()
	{
		if (this.m_IsPlaying && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			if (this.m_AudioClipTimer > this.m_AudioClipLength && this.m_radioPlayer.IsPlaying)
			{
				this.ResetRadio();
			}
			else
			{
				this.m_AudioClipTimer += Time.deltaTime;
			}
		}
	}

	private void SetAudioClips()
	{
		this.m_AvailableAudioClips.Clear();
		for (int i = 0; i < this.m_AudioClips.Length; i = unchecked(i + 1))
		{
			AudioClip item;
			item = this.m_AudioClips[i];
			if (!this.m_AvailableAudioClips.Contains(item))
			{
				this.m_AvailableAudioClips.Add(item);
			}
		}
	}

	private void GetNextAudioClip()
	{
		AudioClip audioClip;
		audioClip = this.m_AudioClips[0];
		if (this.m_AudioClips.Length > 1)
		{
			audioClip = this.GetAudioClip();
			if (this.m_AvailableAudioClips.Contains(audioClip))
			{
				this.m_AvailableAudioClips.Remove(audioClip);
			}
		}
		this.PlayAudioClip(audioClip);
	}

	private AudioClip GetAudioClip()
	{
		AudioClip audioClip;
		audioClip = this.m_AvailableAudioClips[global::UnityEngine.Random.Range(0, this.m_AvailableAudioClips.Count)];
		if (audioClip == this.m_CurrentAudioClip)
		{
			audioClip = this.GetAudioClip();
		}
		return audioClip;
	}

	private void PlayAudioClip(AudioClip audioClip)
	{
		this.m_CurrentAudioClip = audioClip;
		this.m_AudioClipLength = this.m_CurrentAudioClip.length - 0.5f;
		this.m_radioPlayer.Play(this.m_CurrentAudioClip);
	}

	private void ResetRadio()
	{
		this.m_radioPlayer.Stop();
		this.m_AudioClipTimer = 0f;
		this.m_AudioClipLength = 0f;
		if (this.m_AvailableAudioClips.Count <= 0)
		{
			this.SetAudioClips();
		}
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
		this.m_Interactable.ResetAction();
		this.m_Interactable.SetActive(active: true);
		this.m_IsPlaying = false;
		this.OnComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnInteract = null;
		this.OnComplete = null;
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_CurrentAudioClip = null;
		base.OnDisposed();
	}
}
