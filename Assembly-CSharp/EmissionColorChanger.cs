using DG.Tweening;
using UnityEngine;

public class EmissionColorChanger : JMonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer m_MeshRenderer;

	[SerializeField]
	private float m_HueMin;

	[SerializeField]
	private float m_HueMax;

	[SerializeField]
	private float m_SaturationMin;

	[SerializeField]
	private float m_SaturationMax;

	[SerializeField]
	private float m_ValueMin;

	[SerializeField]
	private float m_ValueMax;

	[SerializeField]
	private float m_AlphaMin;

	[SerializeField]
	private float m_AlphaMax;

	public override void Start()
	{
		this.DOColor();
	}

	private void DOColor()
	{
		if (this.m_MeshRenderer != null && this.m_MeshRenderer.gameObject != null)
		{
			Color32 color;
			color = Random.ColorHSV(this.m_HueMin, this.m_HueMax, this.m_SaturationMin, this.m_SaturationMax, this.m_ValueMin, this.m_ValueMax, this.m_AlphaMin, this.m_AlphaMax);
			Sequence sequence;
			sequence = DOTween.Sequence();
			for (int i = 0; i < this.m_MeshRenderer.materials.Length; i = unchecked(i + 1))
			{
				sequence.Insert(0f, this.m_MeshRenderer.materials[i].DOColor(color, "_EmissionColor", 0.1f).SetEase(Ease.Linear));
			}
			sequence.OnComplete(this.DOColor);
		}
	}
}
