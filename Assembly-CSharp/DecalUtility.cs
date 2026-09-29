using System.Linq;
using UnityEngine;

public static class DecalUtility
{
	public static MeshFilter[] GetAffectedObjects(Decal decal)
	{
		Bounds bounds;
		bounds = DecalUtility.GetBounds(decal.transform);
		return (from obj in decal.transform.root.GetComponentsInChildren<MeshRenderer>()
			where obj.gameObject.isStatic
			where DecalUtility.HasLayer(decal.affectedLayers, obj.gameObject.layer)
			where obj.GetComponent<Decal>() == null
			where obj.transform.parent?.GetComponent<Decal>() == null
			where bounds.Intersects(obj.bounds)
			select obj.GetComponent<MeshFilter>() into obj
			where obj != null && obj.sharedMesh != null
			select obj).ToArray();
	}

	private static bool HasLayer(LayerMask mask, int layer)
	{
		return (mask.value & (1 << layer)) != 0;
	}

	private static Bounds GetBounds(Transform transform)
	{
		Vector3 lossyScale;
		lossyScale = transform.lossyScale;
		Vector3 vector;
		vector = -lossyScale / 2f;
		Vector3 vector2;
		vector2 = lossyScale / 2f;
		Vector3[] source;
		source = new Vector3[8]
		{
			new Vector3(vector.x, vector.y, vector.z),
			new Vector3(vector2.x, vector.y, vector.z),
			new Vector3(vector.x, vector2.y, vector.z),
			new Vector3(vector2.x, vector2.y, vector.z),
			new Vector3(vector.x, vector.y, vector2.z),
			new Vector3(vector2.x, vector.y, vector2.z),
			new Vector3(vector.x, vector2.y, vector2.z),
			new Vector3(vector2.x, vector2.y, vector2.z)
		}.Select(transform.TransformDirection).ToArray();
		vector = source.Aggregate(Vector3.Min);
		return new Bounds(size: source.Aggregate(Vector3.Max) - vector, center: transform.position);
	}
}
