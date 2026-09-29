using UnityEngine;

namespace S13Audio;

public static class S13AudioUtil
{
	private static readonly float SemitoneRatio;

	public static float SemitoneToPitch(float semitones)
	{
		return Mathf.Pow(S13AudioUtil.SemitoneRatio, semitones);
	}

	public static float PitchToSemitone(float pitch)
	{
		return Mathf.Log(pitch, S13AudioUtil.SemitoneRatio);
	}

	public static float DecibelToLinear(float dBVolume)
	{
		if (dBVolume <= -80f)
		{
			return 0f;
		}
		return Mathf.Clamp01(Mathf.Pow(10f, dBVolume / 20f));
	}

	public static float LinearToDecibel(float linearVolume)
	{
		if (linearVolume <= 0f)
		{
			return -60f;
		}
		return Mathf.Max(20f * Mathf.Log10(linearVolume), -80f);
	}

	static S13AudioUtil()
	{
		S13AudioUtil.SemitoneRatio = Mathf.Pow(2f, 1f / 12f);
	}
}
