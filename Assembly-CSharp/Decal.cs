using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class Decal : JMonoBehaviour
{
	[SerializeField]
	protected Vector3[] vertices;

	[SerializeField]
	protected Vector3[] normals;

	[SerializeField]
	protected Vector2[] uv;

	[SerializeField]
	protected Vector2[] uv2;

	[SerializeField]
	protected int[] triangles;

	[SerializeField]
	public DecalData data;

	protected GameObject[] m_AffectedObjects;

	protected MaterialPropertyBlock m_MaterialPropertyBlock;

	public float maxAngle = 90f;

	public float pushDistance = 0.0005f;

	public LayerMask affectedLayers = -1;

	[SerializeField]
	protected MeshFilter m_MeshFilter;

	[SerializeField]
	protected MeshRenderer m_MeshRenderer;

	[SerializeField]
	protected Transform m_MeshTransform;

	public MeshFilter MeshFilter => this.m_MeshFilter ?? (this.m_MeshFilter = this.MeshTransform?.GetComponent<MeshFilter>());

	public MeshRenderer MeshRenderer => this.m_MeshRenderer ?? (this.m_MeshRenderer = this.MeshTransform?.GetComponent<MeshRenderer>());

	public Transform MeshTransform => this.m_MeshTransform ?? (this.m_MeshTransform = ((base.transform.childCount > 0) ? base.transform.GetChild(0) : null));

	public abstract GameObject[] Build();

	protected void DeserializeMeshData()
	{
		this.ResetMeshTransform();
		this.GetRendererMaterial();
		if (!(this.MeshFilter.sharedMesh != null) && this.vertices != null && this.vertices.Length != 0)
		{
			this.MeshFilter.sharedMesh = new Mesh
			{
				vertices = this.vertices,
				normals = this.normals,
				uv = this.uv,
				uv2 = this.uv2,
				triangles = this.triangles
			};
		}
	}

	private void ResetMeshTransform()
	{
		if (!(this.MeshTransform == null))
		{
			if (this.MeshTransform.localPosition != Vector3.zero)
			{
				this.MeshTransform.localPosition = Vector3.zero;
			}
			if (this.MeshTransform.localEulerAngles != Vector3.zero)
			{
				this.MeshTransform.localEulerAngles = Vector3.zero;
			}
			if (this.MeshTransform.localScale != Vector3.one)
			{
				this.MeshTransform.localScale = Vector3.one;
			}
			this.CheckHideFlags();
		}
	}

	private void CheckHideFlags()
	{
	}

	private void GetRendererMaterial()
	{
		if (this.data != null && (this.m_MaterialPropertyBlock == null || !(this.m_MaterialPropertyBlock.GetTexture("_MainTex") == this.data.Texture)))
		{
			this.m_MaterialPropertyBlock = new MaterialPropertyBlock();
			this.MeshRenderer.GetPropertyBlock(this.m_MaterialPropertyBlock);
			this.MeshRenderer.sharedMaterial = this.data.SelectedDecalMaterial;
			if ((bool)this.data.Texture)
			{
				this.m_MaterialPropertyBlock.SetTexture("_MainTex", this.data.Texture);
			}
			if ((bool)this.data.Emissive)
			{
				this.m_MaterialPropertyBlock.SetTexture("_Emissive", this.data.Emissive);
			}
			if ((bool)this.data.Normals)
			{
				this.m_MaterialPropertyBlock.SetTexture("_Normal", this.data.Normals);
			}
			this.m_MaterialPropertyBlock.SetColor("_Color", this.data.Color);
			this.m_MaterialPropertyBlock.SetColor("_EmissiveColor", this.data.EmissiveColor);
			this.m_MaterialPropertyBlock.SetFloat("_Specular", this.data.Specular);
			this.m_MaterialPropertyBlock.SetFloat("_Alpha", this.data.Alpha);
			this.MeshRenderer.SetPropertyBlock(this.m_MaterialPropertyBlock);
		}
	}

	protected void SerializeMeshData()
	{
		Mesh sharedMesh;
		sharedMesh = this.MeshFilter.sharedMesh;
		if (!(sharedMesh == null))
		{
			this.vertices = sharedMesh.vertices;
			this.normals = sharedMesh.normals;
			this.uv = sharedMesh.uv;
			this.uv2 = sharedMesh.uv2;
			this.triangles = sharedMesh.triangles;
		}
	}

	protected override void OnDisposed()
	{
		this.m_AffectedObjects = null;
		this.m_MaterialPropertyBlock = null;
		base.OnDisposed();
	}
}
