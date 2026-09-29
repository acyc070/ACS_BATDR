using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Primitive/Container", order = 1)]
public class S13ObjectPrimitiveContainer : S13ObjectPrimitiveMultiClip<S13ObjectPrimitiveContainer, S13HandlerPrimitiveContainer>, IEnumerable<AudioClip>, IEnumerable
{
	[Space]
	public AudioClip[] containedClips;

	public IEnumerator<AudioClip> GetEnumerator()
	{
		foreach (AudioClip item in this.containedClips.Where((AudioClip x) => x))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (AudioClip item in this.containedClips.Where((AudioClip x) => x))
		{
			yield return item;
		}
	}
}
