using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Primitive/Ambient MultiOutput", order = 1)]
public class S13ObjectAmbientMultiOutput : S13ObjectPrimitiveMultiClip<S13ObjectAmbientMultiOutput, S13HandlerAmbientMultiOutput>, IEnumerable<AudioClip>, IEnumerable
{
	[Serializable]
	public class ClipOutputPair : IS13HorizontalPropertyHolder
	{
		public AudioClip clip;

		public AudioMixerGroup mixerOutput;
	}

	public ClipOutputPair[] containedClipOutputs;

	protected override void OnValidate()
	{
		base.OnValidate();
		base.voiceCount = this.containedClipOutputs.Length;
		base.outputOverride = null;
	}

	public IEnumerator<AudioClip> GetEnumerator()
	{
		foreach (ClipOutputPair item in this.containedClipOutputs.Where((ClipOutputPair x) => x?.clip != null))
		{
			yield return item.clip;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (ClipOutputPair item in this.containedClipOutputs.Where((ClipOutputPair x) => x?.clip != null))
		{
			yield return item.clip;
		}
	}
}
