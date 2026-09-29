using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PolygonClippingUtility
{
	private static readonly Plane right;

	private static readonly Plane left;

	private static readonly Plane top;

	private static readonly Plane bottom;

	private static readonly Plane front;

	private static readonly Plane back;

	public static Vector3[] Clip(params Vector3[] poly)
	{
		poly = PolygonClippingUtility.Clip(poly, PolygonClippingUtility.right).ToArray();
		poly = PolygonClippingUtility.Clip(poly, PolygonClippingUtility.left).ToArray();
		poly = PolygonClippingUtility.Clip(poly, PolygonClippingUtility.top).ToArray();
		poly = PolygonClippingUtility.Clip(poly, PolygonClippingUtility.bottom).ToArray();
		poly = PolygonClippingUtility.Clip(poly, PolygonClippingUtility.front).ToArray();
		poly = PolygonClippingUtility.Clip(poly, PolygonClippingUtility.back).ToArray();
		return poly;
	}

	private static IEnumerable<Vector3> Clip(Vector3[] poly, Plane plane)
	{
		unchecked
		{
			for (int i = 0; i < poly.Length; i++)
			{
				int num;
				num = (i + 1) % poly.Length;
				Vector3 v1;
				v1 = poly[i];
				Vector3 v2;
				v2 = poly[num];
				if (plane.GetSide(v1))
				{
					yield return v1;
				}
				if (plane.GetSide(v1) != plane.GetSide(v2))
				{
					yield return PolygonClippingUtility.PlaneLineCast(plane, v1, v2);
				}
			}
		}
	}

	private static Vector3 PlaneLineCast(Plane plane, Vector3 a, Vector3 b)
	{
		Ray ray;
		ray = new Ray(a, b - a);
		plane.Raycast(ray, out var enter);
		return ray.GetPoint(enter);
	}

	static PolygonClippingUtility()
	{
		PolygonClippingUtility.right = new Plane(Vector3.right, 0.5f);
		PolygonClippingUtility.left = new Plane(Vector3.left, 0.5f);
		PolygonClippingUtility.top = new Plane(Vector3.up, 0.5f);
		PolygonClippingUtility.bottom = new Plane(Vector3.down, 0.5f);
		PolygonClippingUtility.front = new Plane(Vector3.forward, 0.5f);
		PolygonClippingUtility.back = new Plane(Vector3.back, 0.5f);
	}
}
