using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerPrimitiveContainer : S13HandlerPrimitiveMultiClip<S13ObjectPrimitiveContainer, S13HandlerPrimitiveContainer>
{
	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		Vector3 positionOffset;
		positionOffset = base.targetObject.GetPositionOffset();
		foreach (AudioClip item in base.targetObject)
		{
			base.VoiceClip(item, base.targetObject.loop, parent, positionOffset);
		}
	}
}
