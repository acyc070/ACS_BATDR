using UnityEngine;

public static class VisionUtility
{
	public static Transform GetTarget(Transform from, float radius, int layerMask)
	{
		if (from == null)
		{
			return null;
		}
		Collider[] array;
		array = new Collider[1];
		if (Physics.OverlapSphereNonAlloc(from.position, radius, array, layerMask, QueryTriggerInteraction.Ignore) <= 0)
		{
			return null;
		}
		return array[0].transform;
	}

	public static bool CheckFOV(Transform from, Transform to, float angle, float radius, int layerMask)
	{
		if (to == null || from == null)
		{
			return false;
		}
		Vector3 normalized;
		normalized = (to.position - from.position).normalized;
		normalized.y *= 0f;
		float num;
		num = Vector3.Angle(from.forward, normalized);
		Vector3 position;
		position = to.position;
		position.y = to.position.y + from.position.y;
		if (num < angle && Physics.SphereCast(from.position, 0.2f, position - from.position, out var hitInfo, radius, layerMask, QueryTriggerInteraction.Ignore) && hitInfo.transform == to)
		{
			return true;
		}
		return false;
	}

	public static bool CheckFOV(Vector3 forward, Vector3 from, Transform to, float angle, float radius, int layerMask)
	{
		if (to == null)
		{
			return false;
		}
		Vector3 normalized;
		normalized = (to.position - from).normalized;
		normalized.y *= 0f;
		if (Vector3.Angle(forward, normalized) < angle && Physics.SphereCast(from, 0.2f, to.position + Vector3.up * 3.25f - from, out var hitInfo, radius, layerMask, QueryTriggerInteraction.Ignore) && hitInfo.transform == to)
		{
			return true;
		}
		return false;
	}
}
