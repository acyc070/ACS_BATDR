using UnityEngine;
using UnityEngine.Rendering;

public class MeshRendererActions : JMonoBehaviour
{
	[SerializeField]
	private Renderer[] m_Renderers;

	public void SetShadowsOnly()
	{
		for (int i = 0; i < this.m_Renderers.Length; i = unchecked(i + 1))
		{
			this.m_Renderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
		}
	}
}
