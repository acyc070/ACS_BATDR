using DG.Tweening;
using UnityEngine;

public class EmissionController : JMonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer m_MaterialEmission;

	[SerializeField]
	private Texture2D m_Sigil;

	[SerializeField]
	private string m_EmissionVariable = "_EmitPower";

	[SerializeField]
	private float m_EndValue;

	[SerializeField]
	private float m_Duration;

	public void SetEmission()
	{
		if (!(this.m_MaterialEmission == null))
		{
			Texture2D texture2D;
			texture2D = this.m_Sigil;
			if (texture2D == null)
			{
				texture2D = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_Banish");
			}
			this.m_MaterialEmission.sharedMaterial.SetTexture("_Emission", texture2D);
			this.m_MaterialEmission.sharedMaterial.SetFloat(this.m_EmissionVariable, this.m_EndValue);
		}
	}

	public void TweenEmission()
	{
		if (!(this.m_MaterialEmission == null))
		{
			Texture2D texture2D;
			texture2D = this.m_Sigil;
			if (texture2D == null)
			{
				texture2D = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_Banish");
			}
			this.m_MaterialEmission.sharedMaterial.SetTexture("_Emission", texture2D);
			this.m_MaterialEmission.sharedMaterial.DOKill();
			this.m_MaterialEmission.sharedMaterial.DOFloat(this.m_EndValue, this.m_EmissionVariable, this.m_Duration).SetEase(Ease.InOutSine);
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_MaterialEmission != null)
		{
			this.m_MaterialEmission.sharedMaterial.DOKill();
		}
		base.OnDisposed();
	}
}
