using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Primitive/Selector", order = 2)]
public class S13ObjectPrimitiveSelector : S13ObjectPrimitiveMultiClip<S13ObjectPrimitiveSelector, S13HandlerPrimitiveSelector>, IEnumerable<AudioClip>, IEnumerable
{
	[Space]
	[Tooltip("The clips that can be randomly selected. The second column represents its chance to be chosen, as shown in the readonly field.")]
	public S13WeightedAudioClipList selectableWeightedClips = new S13WeightedAudioClipList();

	[Tooltip("The number of previous selections that should be avoided when randomizing the next choice. Setting to 0 will allow full randomness.")]
	public int avoidRepeatHistory;

	protected override void OnValidate()
	{
		base.OnValidate();
		int a;
		a = this.avoidRepeatHistory;
		S13WeightedAudioClipList s13WeightedAudioClipList;
		s13WeightedAudioClipList = this.selectableWeightedClips;
		this.avoidRepeatHistory = Mathf.Max(0, Mathf.Min(a, ((s13WeightedAudioClipList == null) ? ((int?)null) : unchecked(s13WeightedAudioClipList.Where((AudioClip x) => x)?.Distinct().Count() - 1)).GetValueOrDefault()));
		this.selectableWeightedClips.CalculateWeights();
	}

	public IEnumerator<AudioClip> GetEnumerator()
	{
		foreach (AudioClip item in this.selectableWeightedClips.Where((AudioClip x) => x))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (AudioClip item in this.selectableWeightedClips.Where((AudioClip x) => x))
		{
			yield return item;
		}
	}
}
