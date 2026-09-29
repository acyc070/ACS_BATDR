using UnityEngine;

public static class HandlesUtility
{
	private static bool IsEditor => Application.isEditor;

	public static void DrawArrow(Transform transform, float size = 1f, Color? color = null)
	{
		_ = HandlesUtility.IsEditor;
	}
}
