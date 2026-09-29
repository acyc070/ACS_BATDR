using System;
using UnityEngine;

public class AudioCueController : JMonoBehaviour
{
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[SerializeField]
	private bool m_ResetOnEnd;

	[SerializeField]
	private AudioSource[] m_AudioSources;

	private AudioSource m_CurrentAudioSource;

	private float m_CurrentTime;

	private int m_AudioIndex;

	public bool IsTriggered { get; private set; }

	public event EventHandler OnPlayStart;

	public event EventHandler OnPlayEnd;

	public override void Start()
	{
		this.m_AudioIndex = 0;
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
			this.m_ActionEvent.OnInteract += this.HandleActionEventOnInteract;
		}
		for (int i = 0; i < this.m_AudioSources.Length; i = unchecked(i + 1))
		{
			this.m_AudioSources[i].gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (!this.IsTriggered || this.m_CurrentAudioSource == null || this.m_CurrentAudioSource.clip == null)
		{
			return;
		}
		this.m_CurrentTime += Time.deltaTime;
		if (this.m_CurrentTime >= this.m_CurrentAudioSource.clip.length)
		{
			this.m_CurrentAudioSource.gameObject.SetActive(value: false);
			this.m_CurrentAudioSource = null;
			this.m_CurrentTime = 0f;
			this.IsTriggered = false;
			if (this.m_ResetOnEnd)
			{
				this.ResetEvent();
			}
			this.OnPlayEnd.Send(this);
		}
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
		unchecked
		{
			if (!this.IsTriggered && this.m_AudioSources.Length != 0)
			{
				this.IsTriggered = true;
				if (this.m_ResetOnEnd)
				{
					GameManager.Instance.Player.Interaction.ResetInteraction();
				}
				this.OnPlayStart.Send(this);
				this.m_CurrentAudioSource = this.m_AudioSources[this.m_AudioIndex];
				this.m_CurrentAudioSource.gameObject.SetActive(value: true);
				this.m_CurrentAudioSource.Play();
				this.m_AudioIndex++;
				if (this.m_AudioIndex >= this.m_AudioSources.Length)
				{
					this.m_AudioIndex = 0;
				}
			}
		}
	}

	public void DisableEvent()
	{
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.SetActive(active: false);
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
		}
	}

	public void ResetEvent()
	{
		this.IsTriggered = false;
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
			this.m_ActionEvent.OnInteract += this.HandleActionEventOnInteract;
			this.m_ActionEvent.ResetAction();
			this.m_ActionEvent.SetActive(active: true);
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.OnInteract -= this.HandleActionEventOnInteract;
		}
		this.m_CurrentAudioSource = null;
		base.OnDisposed();
	}
}
