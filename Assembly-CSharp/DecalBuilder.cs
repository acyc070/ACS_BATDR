using System.Linq;
using UnityEngine;

public static class DecalBuilder
{
	private static readonly DecalMeshBuilder builder;

	public static GameObject[] BuildDynamic(Decal decal)
	{
		return DecalBuilder.Build(decal, isStatic: false);
	}

	public static GameObject[] BuildStatic(Decal decal)
	{
		return DecalBuilder.Build(decal, isStatic: true);
	}

	private static GameObject[] Build(Decal decal, bool isStatic)
	{
		MeshFilter meshFilter;
		meshFilter = decal.MeshFilter;
		MeshRenderer meshRenderer;
		meshRenderer = decal.MeshRenderer;
		if (meshFilter.sharedMesh != null && !meshFilter.sharedMesh.isReadable)
		{
			return null;
		}
		if (decal.data.SelectedDecalMaterial == null || decal.data.Texture == null)
		{
			Object.DestroyImmediate(meshFilter.sharedMesh);
			meshFilter.sharedMesh = null;
			meshRenderer.sharedMaterial = null;
			return null;
		}
		MeshFilter[] affectedObjects;
		affectedObjects = DecalUtility.GetAffectedObjects(decal);
		MeshFilter[] array;
		array = affectedObjects;
		foreach (MeshFilter meshFilter2 in array)
		{
			if (isStatic)
			{
				DecalBuilder.BuildStatic(decal, meshFilter2);
			}
			else
			{
				DecalBuilder.BuildDynamic(decal, meshFilter2);
			}
		}
		DecalBuilder.builder.Push(decal.pushDistance);
		if (meshFilter.sharedMesh == null)
		{
			meshFilter.sharedMesh = new Mesh();
			meshFilter.sharedMesh.name = "StaticDecal";
		}
		DecalBuilder.builder.ToMesh(meshFilter.sharedMesh);
		return affectedObjects.Select((MeshFilter meshFilter3) => meshFilter3.gameObject).ToArray();
	}

	private static void BuildStatic(Decal decal, MeshFilter @object)
	{
		Mesh sharedMesh;
		sharedMesh = @object.sharedMesh;
		if (sharedMesh.isReadable)
		{
			DecalBuilder.BuildMesh(decal, sharedMesh, @object.transform);
		}
	}

	private static void BuildDynamic(Decal decal, MeshFilter @object)
	{
		Mesh sharedMesh;
		sharedMesh = @object.sharedMesh;
		if (!sharedMesh.isReadable)
		{
			MeshCollider component;
			component = @object.GetComponent<MeshCollider>();
			if (component == null)
			{
				return;
			}
			sharedMesh = component.sharedMesh;
		}
		DecalBuilder.BuildMesh(decal, sharedMesh, @object.transform);
	}

	private static void BuildMesh(Decal decal, Mesh mesh, Transform _transform)
	{
		Matrix4x4 matrix4x;
		matrix4x = decal.transform.worldToLocalMatrix * _transform.localToWorldMatrix;
		Vector3[] vertices;
		vertices = mesh.vertices;
		int[] triangles;
		triangles = mesh.triangles;
		unchecked
		{
			for (int i = 0; i < triangles.Length; i += 3)
			{
				int num;
				num = triangles[i];
				int num2;
				num2 = triangles[i + 1];
				DecalBuilder.AddTriangle(v3: matrix4x.MultiplyPoint(vertices[triangles[i + 2]]), decal: decal, v1: matrix4x.MultiplyPoint(vertices[num]), v2: matrix4x.MultiplyPoint(vertices[num2]));
			}
		}
	}

	private static void AddTriangle(Decal decal, Vector3 v1, Vector3 v2, Vector3 v3)
	{
		Rect uvRect;
		uvRect = DecalBuilder.To01(new Rect(0f, 0f, decal.data.Texture.width, decal.data.Texture.height), decal.data.Texture);
		Vector3 normalized;
		normalized = Vector3.Cross(v2 - v1, v3 - v1).normalized;
		if (Vector3.Angle(Vector3.forward, -normalized) <= decal.maxAngle)
		{
			Vector3[] array;
			array = PolygonClippingUtility.Clip(v1, v2, v3);
			if (array.Length != 0)
			{
				DecalBuilder.builder.AddPolygon(array, normalized, uvRect);
			}
		}
	}

	private static Rect To01(Rect rect, Texture2D texture)
	{
		rect.x /= texture.width;
		rect.y /= texture.height;
		rect.width /= texture.width;
		rect.height /= texture.height;
		return rect;
	}

	static DecalBuilder()
	{
		DecalBuilder.builder = new DecalMeshBuilder();
	}
}
