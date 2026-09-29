using UnityEngine;

[AddComponentMenu("Decals/Static Decal")]
public class StaticDecal : Decal
{
	public override void OnEnable()
	{
		if (Application.isPlaying)
		{
			base.enabled = false;
			base.DeserializeMeshData();
		}
	}

	public override GameObject[] Build()
	{
		base.m_AffectedObjects = new GameObject[0];
		base.MeshFilter.sharedMesh = null;
		base.m_AffectedObjects = DecalBuilder.BuildStatic(this);
		base.DeserializeMeshData();
		base.SerializeMeshData();
		return base.m_AffectedObjects;
	}
}
