using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerPrimitiveStartLoopStop : S13HandlerPrimitiveMultiClip<S13ObjectPrimitiveStartLoopStop, S13HandlerPrimitiveStartLoopStop>
{
	private Transform targetParent;

	private Vector3 targetOffset;

	private HashSet<AudioSource> startVoices = new HashSet<AudioSource>();

	private HashSet<AudioSource> loopVoices = new HashSet<AudioSource>();

	private HashSet<AudioSource> stopVoices = new HashSet<AudioSource>();

	protected override void PlayHandler(Transform parent)
	{
		this.ClearVoices();
		if (base.targetObject.cutStop)
		{
			foreach (AudioSource stopVoice in this.stopVoices)
			{
				this.StopChildVoice(stopVoice, ignoreFades: false);
			}
		}
		this.targetParent = parent;
		this.targetOffset = base.targetObject.GetPositionOffset();
		Vector3 voicePosition;
		voicePosition = (this.targetParent?.position ?? Vector3.zero) + this.targetOffset;
		AudioSource validVoice;
		validVoice = base.GetValidVoice(voicePosition);
		if ((bool)validVoice)
		{
			this.startVoices.Add(validVoice);
			base.VoiceClip(base.targetObject.startSegment, loop: false, this.targetParent, this.targetOffset, validVoice);
		}
		validVoice = base.GetValidVoice(voicePosition);
		if ((bool)validVoice)
		{
			this.loopVoices.Add(validVoice);
			base.VoiceClip(base.targetObject.loopSegment, loop: true, this.targetParent, this.targetOffset, validVoice);
		}
	}

	protected override void StopHandler(bool ignoreFades)
	{
		this.ClearVoices();
		if (base.targetObject.cutStart)
		{
			foreach (AudioSource startVoice in this.startVoices)
			{
				this.StopChildVoice(startVoice, ignoreFades);
			}
		}
		foreach (AudioSource loopVoice in this.loopVoices)
		{
			this.StopChildVoice(loopVoice, ignoreFades);
		}
		AudioSource validVoice;
		validVoice = base.GetValidVoice(this.targetParent.position + this.targetOffset);
		if ((bool)validVoice)
		{
			this.stopVoices.Add(validVoice);
			base.VoiceClip(base.targetObject.stopSegment, loop: false, this.targetParent, this.targetOffset, validVoice);
		}
	}

	protected override void ShutdownHandler()
	{
		this.ClearVoices();
		foreach (AudioSource startVoice in this.startVoices)
		{
			base.StopVoice(startVoice);
		}
		foreach (AudioSource loopVoice in this.loopVoices)
		{
			base.StopVoice(loopVoice);
		}
	}

	private void StopChildVoice(AudioSource v, bool ignoreFades)
	{
		if (ignoreFades)
		{
			base.StopVoice(v);
			return;
		}
		switch (base.voiceHandlers[v].state)
		{
		case VoiceHandlerData.State.Waiting:
			base.StopVoice(v);
			break;
		case VoiceHandlerData.State.FadingIn:
		case VoiceHandlerData.State.Playing:
			base.voiceHandlers[v].state = VoiceHandlerData.State.FadingOut;
			break;
		case VoiceHandlerData.State.Stopped:
		case VoiceHandlerData.State.FadingOut:
			break;
		}
	}

	private void ClearVoices()
	{
		this.startVoices.RemoveWhere((AudioSource x) => !base.voiceHandlers[x].isPlaying);
		this.loopVoices.RemoveWhere((AudioSource x) => !base.voiceHandlers[x].isPlaying);
		this.stopVoices.RemoveWhere((AudioSource x) => !base.voiceHandlers[x].isPlaying);
	}
}
