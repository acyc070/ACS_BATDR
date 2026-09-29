using UnityEngine;

[ExecuteInEditMode]
public class EditorDrawMesh : JMonoBehaviour
{
	[SerializeField]
	private Mesh m_Mesh;

	[SerializeField]
	private Material m_Material;

	public override void Start()
	{
		if (Application.isPlaying)
		{
			base.Dispose();
		}
	}
}
