using System;
using UnityEngine;

public class AudioCueConnector : JMonoBehaviour
{
	[SerializeField]
	private AudioCueController[] m_AudioCues;

	public override void Start()
	{
		for (int i = 0; i < this.m_AudioCues.Length; i = unchecked(i + 1))
		{
			AudioCueController obj;
			obj = this.m_AudioCues[i];
			obj.OnPlayStart += this.HandleAudioCueOnPlayStart;
			obj.OnPlayEnd += this.HandleAudioCueOnPlayEnd;
		}
	}

	private void HandleAudioCueOnPlayStart(object sender, EventArgs e)
	{
		for (int i = 0; i < this.m_AudioCues.Length; i = unchecked(i + 1))
		{
			this.m_AudioCues[i].DisableEvent();
		}
	}

	private void HandleAudioCueOnPlayEnd(object sender, EventArgs e)
	{
		AudioCueController audioCueController;
		audioCueController = (AudioCueController)sender;
		for (int i = 0; i < this.m_AudioCues.Length; i = unchecked(i + 1))
		{
			AudioCueController audioCueController2;
			audioCueController2 = this.m_AudioCues[i];
			if (audioCueController2 != audioCueController)
			{
				audioCueController2.ResetEvent();
			}
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < this.m_AudioCues.Length; i = unchecked(i + 1))
		{
			AudioCueController obj;
			obj = this.m_AudioCues[i];
			obj.OnPlayStart -= this.HandleAudioCueOnPlayStart;
			obj.OnPlayEnd -= this.HandleAudioCueOnPlayEnd;
		}
		base.OnDisposed();
	}
}
