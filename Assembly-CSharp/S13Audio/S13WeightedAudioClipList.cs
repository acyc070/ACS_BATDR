using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13WeightedAudioClipList : IEnumerable<AudioClip>, IEnumerable
{
	[Serializable]
	private class WeightedClipList : ReorderableArray<S13WeightedClipEntry>
	{
	}

	[Serializable]
	[S13HorizontalPropertyHolder(forceExpand = true, relativeFieldSizes = new int[] { 4, 1, 1 })]
	public class S13WeightedClipEntry : IS13HorizontalPropertyHolder, ISerializationCallbackReceiver
	{
		public AudioClip clip;

		public float weight = 1f;

		[S13ReadOnlyField]
		public string __weightDisplay = "XXX%";

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.weight == 0f)
			{
				this.weight = 1f;
			}
		}
	}

	[SerializeField]
	[Reorderable(surrogateType = new Type[] { typeof(AudioClip) }, surrogateProperty = new string[] { "clip" })]
	private WeightedClipList clipList = new WeightedClipList();

	public void CalculateWeights()
	{
		if (this.clipList.Count == 0)
		{
			return;
		}
		float num;
		num = Mathf.Max(this.clipList.Sum((S13WeightedClipEntry x) => x.weight), float.Epsilon);
		foreach (S13WeightedClipEntry clip in this.clipList)
		{
			clip.__weightDisplay = (clip.weight / num).ToString("P1");
		}
	}

	public AudioClip GetRandom()
	{
		float num;
		num = global::UnityEngine.Random.Range(0f, Mathf.Max(this.clipList.Sum((S13WeightedClipEntry x) => x.weight), float.Epsilon));
		for (int num2 = 0; num2 < this.clipList.Count; num2 = unchecked(num2 + 1))
		{
			if ((num -= this.clipList[num2].weight) < 0f)
			{
				return this.clipList[num2].clip;
			}
		}
		return null;
	}

	public S13WeightedClipEntry[] GetWeightedClips()
	{
		return this.clipList.ToArray();
	}

	public AudioClip[] GetClips()
	{
		return this.clipList?.Select((S13WeightedClipEntry x) => x.clip).ToArray() ?? new AudioClip[0];
	}

	public void SetClips(AudioClip[] clips)
	{
		if (this.clipList == null)
		{
			this.clipList = new WeightedClipList();
		}
		this.clipList.Clear();
		for (int i = 0; i < clips.Length; i = unchecked(i + 1))
		{
			this.clipList.Add(new S13WeightedClipEntry
			{
				clip = clips[i]
			});
		}
		this.CalculateWeights();
	}

	public IEnumerator<AudioClip> GetEnumerator()
	{
		foreach (S13WeightedClipEntry item in this.clipList.Where((S13WeightedClipEntry x) => x != null))
		{
			yield return item.clip;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (S13WeightedClipEntry item in this.clipList.Where((S13WeightedClipEntry x) => x != null))
		{
			yield return item.clip;
		}
	}
}
