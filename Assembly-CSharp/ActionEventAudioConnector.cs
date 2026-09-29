using System;
using S13Audio;
using UnityEngine;

public class ActionEventAudioConnector : JMonoBehaviour
{
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private AudioClip[] m_AudioClips;

	[SerializeField]
	private float m_RepeatDelay;

	[SerializeField]
	private bool m_IncludeOnExit;

	[SerializeField]
	private bool m_UseS13;

	[Header("Studio13 Audio")]
	[SerializeField]
	private S13LocalAction onPlayAction;

	private bool m_IsPlayed;

	private float m_RepeatDelayCount;

	private AudioClip m_CurrentAudioClip;

	public event EventHandler OnEnter;

	public override void Start()
	{
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.OnInteract += this.HandleActionEventOnInteract;
			if (this.m_IncludeOnExit)
			{
				this.m_ActionEvent.OnExit += this.HandleActionEventOnInteract;
			}
		}
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		this.PlayS13Audio();
		if (!this.m_IsPlayed)
		{
			this.PlayAudio();
		}
	}

	private void Update()
	{
		if (this.m_IsPlayed && this.m_RepeatDelay != 0f)
		{
			this.m_RepeatDelayCount += Time.deltaTime;
			if (this.m_RepeatDelayCount >= this.m_RepeatDelay)
			{
				this.m_IsPlayed = false;
				this.m_RepeatDelayCount = 0f;
			}
		}
	}

	private void PlayAudio()
	{
		if (!this.m_UseS13)
		{
			AudioClip currentAudioClip;
			currentAudioClip = this.m_CurrentAudioClip;
			if (this.m_AudioClips.Length > 1)
			{
				while (currentAudioClip == this.m_CurrentAudioClip)
				{
					this.m_CurrentAudioClip = this.m_AudioClips[global::UnityEngine.Random.Range(0, this.m_AudioClips.Length)];
				}
			}
			else
			{
				if (this.m_AudioClips.Length != 1)
				{
					return;
				}
				this.m_CurrentAudioClip = this.m_AudioClips[0];
			}
			if (this.m_AudioSource != null)
			{
				this.m_AudioSource.clip = this.m_CurrentAudioClip;
				this.m_AudioSource.Play();
			}
		}
		this.m_IsPlayed = true;
		this.OnEnter.Send(this);
	}

	private void PlayS13Audio()
	{
		if (this.onPlayAction.IsExecutable)
		{
			this.onPlayAction.Execute();
		}
	}

	protected override void OnDisposed()
	{
		this.OnEnter = null;
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
			if (this.m_IncludeOnExit)
			{
				this.m_ActionEvent.OnExit -= this.HandleActionEventOnInteract;
			}
		}
		base.OnDisposed();
	}
}
