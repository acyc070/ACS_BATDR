using System;
using System.Collections;
using UnityEngine;

namespace S13Audio;

public static class S13CoroutineUtil
{
	public static IEnumerator WaitForDuration(float duration, bool ignoreTimeScale, Action action)
	{
		if (action != null)
		{
			if (ignoreTimeScale)
			{
				yield return new WaitForSecondsRealtime(duration);
			}
			else
			{
				yield return new WaitForSeconds(duration);
			}
			action();
		}
	}
}
