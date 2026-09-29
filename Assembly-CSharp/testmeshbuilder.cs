using UnityEngine;

public class testmeshbuilder : JMonoBehaviour
{
	public SkinnedMeshRenderer skinnedmeshrenderer;

	public MeshFilter meshfilter;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			Mesh mesh;
			mesh = new Mesh();
			this.skinnedmeshrenderer.BakeMesh(mesh);
			this.meshfilter.mesh = mesh;
		}
	}
}
