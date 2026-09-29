using System;
using UnityEngine;
using UnityEngine.Audio;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Compound/Music Stem Group")]
public class S13ObjectStemGroup : ScriptableObject
{
	[Serializable]
	public struct Stem
	{
		public AudioClip clip;

		public AudioMixerGroup mixer;
	}

	[Header("Clips and Mixers")]
	public Stem[] stems;

	[Header("Stingers")]
	public AudioMixerGroup stingerMixerGroup;

	public AudioClip[] stingers;

	public bool loop;

	[Header("Timing and Time Signature")]
	[SerializeField]
	[Range(30f, 300f)]
	public float tempo = 100f;

	[Space]
	[SerializeField]
	[Range(1f, 32f)]
	public int beatsPerBar = 4;

	[SerializeField]
	[Range(1f, 32f)]
	public int meter = 4;

	public double Duration => (double)this.stems[0].clip.samples / (double)this.stems[0].clip.frequency;
}
