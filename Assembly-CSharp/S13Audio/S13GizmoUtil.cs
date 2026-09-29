using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public static class S13GizmoUtil
{
	public static void DrawTriggerGizmos(List<Collider> triggers, Color selectedColor, bool forceSelected = false)
	{
	}

	public static void DrawWireSphere(Vector3 point, float radius, Color color)
	{
		Color color2;
		color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawWireSphere(point, radius);
		Gizmos.color = color2;
	}

	private static void DrawColliders(List<Collider> colliders, Color color)
	{
	}
}
