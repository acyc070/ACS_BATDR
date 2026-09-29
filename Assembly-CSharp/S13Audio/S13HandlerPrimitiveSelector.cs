using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerPrimitiveSelector : S13HandlerPrimitiveMultiClip<S13ObjectPrimitiveSelector, S13HandlerPrimitiveSelector>
{
	private Queue<(int index, AudioClip clip)> history = new Queue<(int, AudioClip)>();

	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		S13WeightedAudioClipList.S13WeightedClipEntry[] weightedClips;
		weightedClips = base.targetObject.selectableWeightedClips.GetWeightedClips();
		unchecked
		{
			List<int> list;
			list = new List<int>(weightedClips.Length - this.history.Count);
			int num;
			num = -1;
			float num2;
			num2 = 0f;
			int i;
			for (i = 0; i < weightedClips.Length; i++)
			{
				if (this.history.All(((int index, AudioClip clip) x) => x.index != i))
				{
					num2 += Mathf.Max(weightedClips[i].weight, float.Epsilon);
					list.Add(i);
				}
			}
			float num3;
			num3 = Random.Range(0f, num2);
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				if ((num3 -= weightedClips[list[num4]].weight) < 0f)
				{
					num = list[num4];
					break;
				}
			}
			if (num >= 0 && num < weightedClips.Length)
			{
				AudioClip clip;
				clip = weightedClips[num].clip;
				base.VoiceClip(clip, base.targetObject.loop, parent);
				this.history.Enqueue((num, clip));
				while (this.history.Count > 0 && this.history.Count > base.targetObject.avoidRepeatHistory)
				{
					this.history.Dequeue();
				}
			}
		}
	}
}
