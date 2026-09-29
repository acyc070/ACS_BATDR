using System;
using DG.Tweening;
using UnityEngine;

public class EventTriggerAudioFader : JMonoBehaviour
{
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private CutsceneDirector m_CutsceneDirector;

	public void Activate()
	{
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.OnEnter += this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.SetActive(active: true);
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_AudioSource.DOKill();
		this.m_AudioSource.DOFade(0f, 2f).SetEase(Ease.Linear).OnComplete(() =>
		{
			this.m_AudioSource.Stop();
			if (this.m_CutsceneDirector != null)
			{
				this.m_CutsceneDirector.Stop();
			}
		});
	}

	protected override void OnDisposed()
	{
		if (this.m_EventTrigger != null)
		{
			this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		}
		this.m_AudioSource.DOKill();
		base.OnDisposed();
	}
}
