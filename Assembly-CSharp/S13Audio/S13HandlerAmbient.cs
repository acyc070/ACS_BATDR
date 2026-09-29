using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerAmbient : S13HandlerPrimitiveSingleClip<S13ObjectAmbient, S13HandlerAmbient>
{
	protected AudioSource voice => base.voices[0];

	protected VoiceHandlerData voiceHandler => base.voiceHandlers[this.voice];

	public override void Initialize(S13ObjectAmbient targetObject)
	{
		base.Initialize(targetObject);
		this.voice.clip = targetObject.clip;
	}

	protected override void PlayHandler(Transform parent)
	{
		base.SetVoiceParent(this.voice, parent, base.targetObject.GetPositionOffset());
		switch (this.voiceHandler.state)
		{
		case VoiceHandlerData.State.Stopped:
			base.StartVoice(this.voice);
			break;
		case VoiceHandlerData.State.FadingOut:
			this.voiceHandler.state = VoiceHandlerData.State.FadingIn;
			break;
		case VoiceHandlerData.State.Waiting:
		case VoiceHandlerData.State.FadingIn:
		case VoiceHandlerData.State.Playing:
			break;
		}
	}

	protected override void StopHandler(bool ignoreFades)
	{
		switch (this.voiceHandler.state)
		{
		case VoiceHandlerData.State.Waiting:
			base.StopVoice(this.voice);
			break;
		case VoiceHandlerData.State.FadingIn:
		case VoiceHandlerData.State.Playing:
		case VoiceHandlerData.State.FadingOut:
			if (ignoreFades)
			{
				base.StopVoice(this.voice);
			}
			else
			{
				this.voiceHandler.state = VoiceHandlerData.State.FadingOut;
			}
			break;
		case VoiceHandlerData.State.Stopped:
			break;
		}
	}
}
