using UnityEngine;

namespace S13Audio;

public static class GameObjectExt
{
	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		T val;
		val = gameObject.GetComponent<T>();
		if (!val)
		{
			val = gameObject.AddComponent<T>();
		}
		return val;
	}
}
