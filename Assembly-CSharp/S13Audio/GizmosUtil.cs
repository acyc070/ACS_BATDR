using System;
using UnityEngine;

namespace S13Audio;

public static class GizmosUtil
{
	private const int CircleSegments = 32;

	private const float Tau = MathF.PI * 2f;

	public static void DrawCircle(Vector3 center, float radius, Vector3 normal)
	{
		Vector3 vector;
		vector = Vector3.Cross(normal, Vector3.up);
		if (vector.sqrMagnitude <= float.Epsilon)
		{
			vector = Vector3.Cross(normal, Vector3.forward);
		}
		vector.Normalize();
		Vector3 normalized;
		normalized = Vector3.Cross(normal, vector).normalized;
		unchecked
		{
			for (int i = 0; i < 32; i++)
			{
				Gizmos.DrawLine(center + radius * (vector * Mathf.Cos(MathF.PI * 2f * (float)i / 32f) + normalized * Mathf.Sin(MathF.PI * 2f * (float)i / 32f)), center + radius * (vector * Mathf.Cos(MathF.PI * 2f * (float)(i + 1) / 32f) + normalized * Mathf.Sin(MathF.PI * 2f * (float)(i + 1) / 32f)));
			}
		}
	}

	public static void DrawCircularArc(Vector3 center, float radius, Vector3 normal, Vector3 start, float degrees)
	{
		Vector3 normalized;
		normalized = Vector3.ProjectOnPlane(start, normal).normalized;
		Vector3 normalized2;
		normalized2 = Vector3.Cross(normal, normalized).normalized;
		int num;
		num = Mathf.CeilToInt(32f * degrees / 360f);
		unchecked
		{
			for (int i = 0; i < num; i++)
			{
				Gizmos.DrawLine(center + radius * (normalized * Mathf.Cos(MathF.PI * 2f * (degrees / 360f) * (float)i / (float)num) + normalized2 * Mathf.Sin(MathF.PI * 2f * (degrees / 360f) * (float)i / (float)num)), center + radius * (normalized * Mathf.Cos(MathF.PI * 2f * (degrees / 360f) * (float)(i + 1) / (float)num) + normalized2 * Mathf.Sin(MathF.PI * 2f * (degrees / 360f) * (float)(i + 1) / (float)num)));
			}
		}
	}

	public static void DrawCollider(Collider collider)
	{
		Gizmos.matrix = collider.transform.localToWorldMatrix;
	}

	public static void DrawChildColliders(Transform transform)
	{
		Collider[] componentsInChildren;
		componentsInChildren = transform.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			GizmosUtil.DrawCollider(componentsInChildren[i]);
		}
	}
}
