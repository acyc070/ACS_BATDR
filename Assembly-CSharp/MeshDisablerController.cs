using UnityEngine;

public class MeshDisablerController : JMonoBehaviour
{
	private Renderer m_renderer;

	public void SetMeshEnabled(bool isEnabled)
	{
		if (this.m_renderer == null)
		{
			this.m_renderer = base.GetComponent<MeshRenderer>();
			if (this.m_renderer == null)
			{
				this.m_renderer = base.GetComponentInChildren<MeshRenderer>();
			}
			if (this.m_renderer != null)
			{
				this.m_renderer.enabled = isEnabled;
			}
		}
		else
		{
			this.m_renderer.enabled = isEnabled;
		}
	}

	public void SetSkinnedMeshEnabled(bool isEnabled)
	{
		if (this.m_renderer == null)
		{
			this.m_renderer = base.GetComponent<SkinnedMeshRenderer>();
			if (this.m_renderer == null)
			{
				this.m_renderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
			}
			if (this.m_renderer != null)
			{
				this.m_renderer.enabled = isEnabled;
			}
		}
		else
		{
			this.m_renderer.enabled = isEnabled;
		}
	}
}
