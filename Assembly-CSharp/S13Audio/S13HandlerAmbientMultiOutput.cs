using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerAmbientMultiOutput : S13HandlerPrimitiveMultiClip<S13ObjectAmbientMultiOutput, S13HandlerAmbientMultiOutput>
{
	public override void Initialize(S13ObjectAmbientMultiOutput targetObject)
	{
		base.Initialize(targetObject);
		if (targetObject.containedClipOutputs.Length != base.voices.Length)
		{
			S13Debug.LogError("S13ObjectAmbientMultiOutput " + base.name + " does not have a voice for each of its contained clips");
			return;
		}
		for (int i = 0; i < targetObject.containedClipOutputs.Length; i = unchecked(i + 1))
		{
			base.voices[i].clip = targetObject.containedClipOutputs[i].clip;
			base.voices[i].outputAudioMixerGroup = targetObject.containedClipOutputs[i].mixerOutput;
		}
	}

	protected override void PlayHandler(Transform parent)
	{
		for (int i = 0; i < base.voices.Length; i = unchecked(i + 1))
		{
			AudioSource audioSource;
			audioSource = base.voices[i];
			VoiceHandlerData voiceHandlerData;
			voiceHandlerData = base.voiceHandlers[audioSource];
			base.SetVoiceParent(audioSource, parent, base.targetObject.GetPositionOffset());
			switch (voiceHandlerData.state)
			{
			case VoiceHandlerData.State.Stopped:
				base.StartVoice(audioSource);
				break;
			case VoiceHandlerData.State.FadingOut:
				voiceHandlerData.state = VoiceHandlerData.State.FadingIn;
				break;
			}
		}
	}

	protected override void StopHandler(bool ignoreFades)
	{
		for (int i = 0; i < base.voices.Length; i = unchecked(i + 1))
		{
			AudioSource audioSource;
			audioSource = base.voices[i];
			VoiceHandlerData voiceHandlerData;
			voiceHandlerData = base.voiceHandlers[audioSource];
			switch (voiceHandlerData.state)
			{
			case VoiceHandlerData.State.Waiting:
				base.StopVoice(audioSource);
				break;
			case VoiceHandlerData.State.FadingIn:
			case VoiceHandlerData.State.Playing:
			case VoiceHandlerData.State.FadingOut:
				if (ignoreFades)
				{
					base.StopVoice(audioSource);
				}
				else
				{
					voiceHandlerData.state = VoiceHandlerData.State.FadingOut;
				}
				break;
			}
		}
	}
}
