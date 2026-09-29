using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

namespace S13Audio;

public static class AudioMixerExt
{
	public static AudioMixerSnapshot[] GetSnapshots(this AudioMixer mixer)
	{
		return mixer.GetType().GetProperty("snapshots").GetValue(mixer, null) as AudioMixerSnapshot[];
	}

	public static string[] GetExposedParameters(this AudioMixer mixer)
	{
		List<string> list;
		list = new List<string>();
		Array objArray;
		objArray = (Array)(mixer.GetType().GetProperty("exposedParameters")?.GetValue(mixer, null));
		if (objArray != null)
		{
			list.AddRange(from param in objArray.Cast<object>().Select((object t, int i) => objArray.GetValue(i))
				select (string)param.GetType().GetField("name").GetValue(param));
		}
		return list.ToArray();
	}
}
