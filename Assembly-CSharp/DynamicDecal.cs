using UnityEngine;

[AddComponentMenu("Decals/Dynamic Decal")]
public class DynamicDecal : Decal
{
	public override GameObject[] Build()
	{
		base.m_AffectedObjects = new GameObject[0];
		base.MeshFilter.sharedMesh = null;
		base.m_AffectedObjects = DecalBuilder.BuildDynamic(this);
		base.DeserializeMeshData();
		base.SerializeMeshData();
		return base.m_AffectedObjects;
	}
}
