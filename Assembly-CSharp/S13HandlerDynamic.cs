using S13Audio;
using UnityEngine;

public class S13HandlerDynamic : S13HandlerPrimitive<S13ObjectDynamic, S13HandlerDynamic>
{
	private AudioClip nextClip;

	public void SetNextClip(AudioClip clip)
	{
		this.nextClip = clip;
	}

	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		base.VoiceClip(this.nextClip, base.targetObject.loop, parent);
	}

	protected override void StopHandler(bool ignoreFades)
	{
		base.StopHandler(ignoreFades);
	}
}
