using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

public static class IEnumerableExt
{
	public static T RandomElement<T>(this IEnumerable<T> t)
	{
		return t.ElementAt(global::UnityEngine.Random.Range(0, t.Count()));
	}

	public static T RandomElementOrDefault<T>(this IEnumerable<T> t)
	{
		try
		{
			return t.RandomElement();
		}
		catch (Exception)
		{
			return default;
		}
	}
}
