using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LightProbeGroup))]
public class LightProbePlacer : MonoBehaviour
{
	public Vector3 Volume = Vector3.one;

	private const float Spacing = 3f;

	private const float WallCheckDistance = 2f;

	private static LayerMask ObjectLayers;

	private const float padding = 5f;

	public static void GenerateLightProbes()
	{
		LightProbePlacer probeGroup;
		probeGroup = LightProbePlacer.GetProbeGroup();
		List<Vector3> list;
		list = new List<Vector3>();
		LightProbePlacer.ObjectLayers = LayerMask.GetMask("Default", "NavMesh");
		int num;
		num = 0;
		LightProbePlacer.GetSceneBounds(probeGroup);
		for (float num2 = 0f; num2 < probeGroup.Volume.x; num2 += 3f)
		{
			for (float num3 = 0f; num3 < probeGroup.Volume.z; num3 += 3f)
			{
				for (float num4 = 0f; num4 < probeGroup.Volume.y; num4 += 3f)
				{
					Vector3 vector;
					vector = new Vector3(num2, num4, num3);
					if (LightProbePlacer.checkDistances(probeGroup.transform.position + vector))
					{
						list.Add(vector);
						num = unchecked(num + 1);
						if (num > 200000)
						{
							Debug.LogError("Too many probes! Reduce volume!");
							return;
						}
					}
				}
			}
		}
		Debug.LogWarning("Probe Count:" + num);
	}

	public static bool checkDistances(Vector3 Position)
	{
		if (Physics.CheckSphere(Position, 2f, LightProbePlacer.ObjectLayers, QueryTriggerInteraction.Ignore) && !Physics.CheckSphere(Position, 0.25f, LightProbePlacer.ObjectLayers, QueryTriggerInteraction.Ignore))
		{
			return true;
		}
		return false;
	}

	public static LightProbePlacer GetProbeGroup()
	{
		LightProbePlacer lightProbePlacer;
		lightProbePlacer = Object.FindObjectOfType<LightProbePlacer>();
		if (lightProbePlacer == null)
		{
			GameObject gameObject;
			gameObject = new GameObject();
			lightProbePlacer = gameObject.AddComponent<LightProbePlacer>();
			gameObject.name = "LIGHT_PROBE_GROUP";
		}
		return lightProbePlacer;
	}

	public static void GetSceneBounds(LightProbePlacer ProbeGroup)
	{
		Bounds bounds;
		bounds = new Bounds(Vector3.zero, Vector3.zero);
		Object[] array;
		array = Object.FindObjectsOfType(typeof(Renderer));
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			bounds.Encapsulate(((Renderer)array[i]).bounds);
		}
		ProbeGroup.Volume = bounds.size + Vector3.one * 5f * 2f;
		ProbeGroup.transform.position = bounds.center - bounds.extents - Vector3.one * 5f;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(base.transform.position + this.Volume / 2f, this.Volume);
	}
}
