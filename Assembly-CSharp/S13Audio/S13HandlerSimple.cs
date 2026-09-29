using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerSimple : S13HandlerPrimitiveSingleClip<S13ObjectSimple, S13HandlerSimple>
{
	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		base.VoiceClip(base.targetObject.clip, base.targetObject.loop, parent);
	}
}
