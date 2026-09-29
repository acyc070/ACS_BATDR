using UnityEngine;

namespace S13Audio;

public static class ComponentExt
{
	public static T GetOrAddComponent<T>(this Component component) where T : Component
	{
		return component.gameObject.GetOrAddComponent<T>();
	}
}
