using System;
using UnityEngine;

public class AudioRandomizer : JMonoBehaviour
{
	[Serializable]
	public class AudioGroup
	{
		public AudioClip AudioClip;

		public string Subtitles;
	}

	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private AudioGroup[] m_AudioGroups;

	private AudioGroup m_CurrentAudioGroup;

	private int m_Index;

	public void Play()
	{
		while (this.m_CurrentAudioGroup == null || this.m_CurrentAudioGroup.AudioClip == this.m_AudioSource.clip)
		{
			this.GetAudioClip();
		}
		this.m_AudioSource.clip = this.m_CurrentAudioGroup.AudioClip;
		this.m_AudioSource.Play();
		GameManager.Instance.ShowSubtitles(TextUtility.GetKey(this.m_CurrentAudioGroup.Subtitles), this.m_CurrentAudioGroup.AudioClip.length, isTrimmed: true);
	}

	private void GetAudioClip()
	{
		this.m_Index = global::UnityEngine.Random.Range(0, this.m_AudioGroups.Length);
		this.m_CurrentAudioGroup = this.m_AudioGroups[this.m_Index];
	}

	protected override void OnDisposed()
	{
		this.m_CurrentAudioGroup = null;
		base.OnDisposed();
	}
}
