using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Primitive/Ambient", order = 0)]
public class S13ObjectAmbient : S13ObjectPrimitiveSingleClip<S13ObjectAmbient, S13HandlerAmbient>
{
	protected override void OnValidate()
	{
		base.OnValidate();
		base.loop = true;
		base.pitchRange.minValue = 0f;
		base.pitchRange.maxValue = 0f;
		base.voiceMode = VoiceMode.IgnoreNew;
		base.voiceCount = 1;
	}
}
