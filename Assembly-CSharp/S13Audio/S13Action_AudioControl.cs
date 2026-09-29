using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13Action_AudioControl<TAudio> : S13Action_AudioControl where TAudio : IS13Controllable
{
	[Tooltip("The audio to control.")]
	[SerializeField]
	public TAudio audio;

	public override bool IsExecutable
	{
		get
		{
			if (base.IsExecutable)
			{
				return this.audio != null;
			}
			return false;
		}
	}

	public override void Execute()
	{
		switch (base.audioControl)
		{
		case AudioControl.Play:
			if (base.audioDelay > 0f)
			{
				if (base.audioDuration > 0f)
				{
					ref TAudio reference3 = ref this.audio;
					float delayTime2;
					delayTime2 = base.audioDelay;
					float duration;
					duration = base.audioDuration;
					reference3.PlayDelayed(delayTime2, duration);
				}
				else
				{
					ref TAudio reference4 = ref this.audio;
					float delayTime3;
					delayTime3 = base.audioDelay;
					reference4.PlayDelayed(delayTime3);
				}
			}
			else if (base.audioDuration > 0f)
			{
				ref TAudio reference5 = ref this.audio;
				float duration2;
				duration2 = base.audioDuration;
				reference5.Play(duration2);
			}
			else
			{
				this.audio.Play();
			}
			break;
		case AudioControl.Stop:
			if (base.audioDelay > 0f)
			{
				ref TAudio reference = ref this.audio;
				float delayTime;
				delayTime = base.audioDelay;
				bool ignoreFades;
				ignoreFades = base.audioIgnoreFades;
				reference.StopDelayed(delayTime, ignoreFades);
			}
			else
			{
				ref TAudio reference2 = ref this.audio;
				bool ignoreFades2;
				ignoreFades2 = base.audioIgnoreFades;
				reference2.Stop(ignoreFades2);
			}
			break;
		case AudioControl.Pause:
			this.audio.Pause();
			break;
		case AudioControl.Resume:
			this.audio.Resume();
			break;
		case AudioControl.Shutdown:
			this.audio.Shutdown();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case AudioControl.None:
			break;
		}
	}

	public override string ToString()
	{
		string text;
		text = $" {base.audioControl} '{this.audio}'";
		switch (base.audioControl)
		{
		case AudioControl.Play:
			if (base.audioDelay > 0f)
			{
				text += $" Delay: {base.audioDelay} ";
			}
			if (base.audioDuration > 0f)
			{
				text += $" Duration: {base.audioDuration}";
			}
			break;
		case AudioControl.Stop:
			if (base.audioDelay > 0f)
			{
				text += $" Delay: {base.audioDelay}";
			}
			if (base.audioIgnoreFades)
			{
				text += $" IgnoreFades: {base.audioIgnoreFades}";
			}
			break;
		case AudioControl.Pause:
			text += " Pause";
			break;
		case AudioControl.Resume:
			text += " Resume";
			break;
		case AudioControl.Shutdown:
			text += " Shutdown";
			break;
		}
		return text;
	}
}
[Serializable]
public class S13Action_AudioControl : IS13Executable
{
	[Serializable]
	public enum AudioControl
	{
		None = 0,
		Play = 1,
		Stop = 2,
		Pause = 3,
		Resume = 4,
		Shutdown = 5
	}

	[Tooltip("The action to take on the audio.")]
	[SerializeField]
	public AudioControl audioControl;

	[Tooltip("The delay before performing the audio action.")]
	[SerializeField]
	public float audioDelay;

	[Tooltip("The duration to play the audio.")]
	[SerializeField]
	public float audioDuration;

	[Tooltip("Should the audio cut off?")]
	[SerializeField]
	public bool audioIgnoreFades;

	public virtual bool IsExecutable => this.audioControl != AudioControl.None;

	public virtual void Execute()
	{
	}
}
