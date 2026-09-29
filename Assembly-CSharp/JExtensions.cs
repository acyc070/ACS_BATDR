using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class JExtensions
{
	public static Transform FindChildName(this Transform t, string name)
	{
		Transform result;
		result = null;
		t.Children();
		foreach (Transform item in t.Children())
		{
			if (item.name.Contains(name))
			{
				result = item;
				break;
			}
		}
		return result;
	}

	public static IEnumerable<Transform> Children(this Transform t)
	{
		foreach (Transform item in t)
		{
			yield return item;
		}
	}

	public static int ParseInt(this string value)
	{
		int num;
		num = 0;
		unchecked
		{
			for (int i = 0; i < value.Length; i++)
			{
				num = 10 * num + (int)char.GetNumericValue(value[i]);
			}
			return num;
		}
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		global::System.Random random;
		random = new global::System.Random();
		int num;
		num = list.Count;
		unchecked
		{
			while (num > 1)
			{
				num--;
				int index;
				index = random.Next(num + 1);
				T value;
				value = list[index];
				list[index] = list[num];
				list[num] = value;
			}
		}
	}

	public static T RandomItem<T>(this IList<T> list)
	{
		if (list.Count == 0)
		{
			throw new IndexOutOfRangeException("Cannot select a random item from an empty list");
		}
		return list[global::UnityEngine.Random.Range(0, list.Count)];
	}

	public static T RemoveRandom<T>(this IList<T> list)
	{
		if (list.Count == 0)
		{
			throw new IndexOutOfRangeException("Cannot remove a random item from an empty list");
		}
		int index;
		index = global::UnityEngine.Random.Range(0, list.Count);
		T result;
		result = list[index];
		list.RemoveAt(index);
		return result;
	}

	public static void Send(this EventHandler handler, object sender)
	{
		handler?.Invoke(sender, EventArgs.Empty);
	}

	public static Transform FindDeepChild(this Transform aParent, string aName)
	{
		Queue<Transform> queue;
		queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0)
		{
			Transform transform;
			transform = queue.Dequeue();
			if (transform.name == aName)
			{
				return transform;
			}
			foreach (Transform item in transform)
			{
				queue.Enqueue(item);
			}
		}
		return null;
	}

	public static Transform FindDeepChildContaining(this Transform aParent, string aName)
	{
		Queue<Transform> queue;
		queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0)
		{
			Transform transform;
			transform = queue.Dequeue();
			if (transform.name.ToLower().Contains(aName.ToLower()))
			{
				return transform;
			}
			foreach (Transform item in transform)
			{
				queue.Enqueue(item);
			}
		}
		return null;
	}

	public static void Dispose<T>(this List<T> source)
	{
		source?.Clear();
		source = null;
	}

	public static void Dispose<K, V>(this Dictionary<K, V> source)
	{
		source?.Clear();
		source = null;
	}

	public static List<List<T>> Split<T>(this List<T> source, int size)
	{
		return (from x in source.Select((T x, int i) => new
			{
				Index = i,
				Value = x
			})
			group x by unchecked(x.Index / size) into x
			select x.Select(v => v.Value).ToList()).ToList();
	}

	public static void RemoveRange<T>(this List<T> list, IEnumerable<T> collection)
	{
		foreach (T item in collection)
		{
			list.RemoveAll((T x) => x.Equals(item));
		}
	}

	public static void RemoveRange<T>(this HashSet<T> hash, IEnumerable<T> collection)
	{
		foreach (T item in collection)
		{
			hash.Remove(item);
		}
	}

	public static void AddRange<T>(this HashSet<T> hash, IEnumerable<T> collection)
	{
		foreach (T item in collection)
		{
			hash.Add(item);
		}
	}

	public static void ForEach<T>(this T[] array, Action<T> action)
	{
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			action(array[i]);
		}
	}

	public static void ForEach<T>(this HashSet<T> array, Action<T> action)
	{
		foreach (T item in array)
		{
			action(item);
		}
	}

	public static string RemoveNumbers(this string _string)
	{
		return _string = Regex.Replace(_string, "[\\d-]", string.Empty);
	}

	public static string RemoveSymbols(this string _string)
	{
		return _string.Replace("'", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty)
			.Replace("(", string.Empty)
			.Replace(")", string.Empty)
			.Replace(" ", string.Empty)
			.Replace(".", string.Empty)
			.Replace("-", string.Empty)
			.Replace("+", string.Empty)
			.Replace("[", string.Empty)
			.Replace("]", string.Empty)
			.Replace("?", string.Empty);
	}

	public static string RemoveUnderscores(this string _string)
	{
		return _string.Replace("_", string.Empty);
	}

	public static string ReplaceBackslash(this string _string)
	{
		return _string.Replace("\\", "/");
	}

	public static string ReplaceForwardSlash(this string _string)
	{
		return _string.Replace("/", "\\");
	}

	public static string ReplaceDataPathToAssets(this string _string)
	{
		return _string.Replace(Application.dataPath + "/", "Assets/");
	}

	public static string GetWhite(this string value)
	{
		return value.Replace("{1}", "<color=#FFFFFF>");
	}

	public static string GetColor(this string value)
	{
		return value.Replace("{2}", "<color=#FFC343>");
	}

	public static string GetColorNotification(this string value)
	{
		return value.Replace("{3}", "<color=#FEC97D>");
	}

	public static string GetLinebreaks(this string value)
	{
		return value.Replace("{0}", "\n");
	}

	public static string GetInput(this string value, string input)
	{
		return value.Replace("{9}", "<color=#FFC343>" + input + "<color=#FFFFFF>");
	}

	public static string ConvertAll(this string value, string input = "")
	{
		return value.GetWhite().GetColor().GetInput(input)
			.GetLinebreaks();
	}

	public static string GetChargeInput(this string value)
	{
		string newValue;
		newValue = "TAB";
		if (GameManager.Instance.HasController)
		{
			newValue = "<size=200%><sprite index=0></size>";
		}
		return value.Replace("{414}", newValue);
	}
}
