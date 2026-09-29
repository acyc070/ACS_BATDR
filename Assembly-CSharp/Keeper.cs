using UnityEngine;

public class Keeper : JMonoBehaviour
{
	[SerializeField]
	private Enemy m_Enemy;

	[SerializeField]
	private SkinnedMeshRenderer m_MeshRenderer;

	[SerializeField]
	private Material m_DefaultMaterial;

	[SerializeField]
	private Material m_EliteMaterial;

	private bool m_InCombat;

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.m_Enemy.InCombat)
		{
			if (!this.m_InCombat)
			{
				this.m_InCombat = true;
				this.m_MeshRenderer.material = this.m_EliteMaterial;
			}
		}
		else if (this.m_InCombat)
		{
			this.m_InCombat = false;
			this.m_MeshRenderer.material = this.m_DefaultMaterial;
		}
	}
}
