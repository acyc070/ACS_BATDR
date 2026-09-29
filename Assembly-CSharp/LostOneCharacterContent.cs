using UnityEngine;

public class LostOneCharacterContent : CharacterContent
{
	[Header("Lost One Type")]
	[SerializeField]
	private LostOneType m_LostOneType;

	[Header("Dead Material")]
	[SerializeField]
	private Material m_DeadMaterial;

	public LostOneType LostOneType => this.m_LostOneType;

	public override void UpdateMaterials()
	{
		if (this.m_DeadMaterial != null)
		{
			for (int i = 0; i < base.ModelRenderers.Length; i = unchecked(i + 1))
			{
				if (base.ModelRenderers[i] is SkinnedMeshRenderer skinnedMeshRenderer)
				{
					skinnedMeshRenderer.material = this.m_DeadMaterial;
				}
			}
		}
		this.SetLidUp(base.transform.FindDeepChildContaining("L_M_EyeLid_Up"));
		this.SetLidUp(base.transform.FindDeepChildContaining("R_M_EyeLid_Up"));
		this.SetLidUp(base.transform.FindDeepChildContaining("L_F_EyeLid_Up"));
		this.SetLidUp(base.transform.FindDeepChildContaining("R_F_EyeLid_Up"));
		this.SetLidDown(base.transform.FindDeepChildContaining("L_M_EyeLid_DN"));
		this.SetLidDown(base.transform.FindDeepChildContaining("R_M_EyeLid_DN"));
		this.SetLidDown(base.transform.FindDeepChildContaining("L_F_EyeLid_DN"));
		this.SetLidDown(base.transform.FindDeepChildContaining("R_F_EyeLid_DN"));
	}

	private void SetLidUp(Transform child)
	{
		if (child != null)
		{
			child.localPosition += new Vector3(0.03f, 0f, 0f);
		}
	}

	private void SetLidDown(Transform child)
	{
		if (child != null)
		{
			child.localPosition -= new Vector3(0.03f, 0f, 0f);
		}
	}
}
