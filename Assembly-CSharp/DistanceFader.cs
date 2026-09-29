using UnityEngine;

public class DistanceFader : JComponent
{
	[Header("Renderer")]
	[SerializeField]
	private MeshRenderer m_MeshRenderer;

	[Header("Distance Check")]
	[SerializeField]
	private float m_DistanceMin = 25f;

	[SerializeField]
	private float m_DistanceMax = 30f;

	[Header("Fade Settings")]
	[SerializeField]
	private float m_FadeDuration = 8f;

	[SerializeField]
	private float m_FadeOut;

	[SerializeField]
	private float m_FadeIn = 1f;

	[Header("Other Settings")]
	[SerializeField]
	private bool m_IsSingleFade = true;

	private float m_ColorLerpTime;

	private bool m_IsFadingIn;

	private bool m_IsFadingOut;

	private bool m_IsFading;

	private bool m_IsFaded;

	private void Update()
	{
		if (this.m_MeshRenderer == null || base.IsDisposed || GameManager.Instance.IsPaused || GameManager.Instance.GameCamera == null || (this.m_IsSingleFade && this.m_IsFaded))
		{
			return;
		}
		Vector3 position;
		position = GameManager.Instance.GameCamera.transform.position;
		Color color;
		color = this.m_MeshRenderer.material.GetColor("_Color");
		Color color2;
		color2 = color;
		Vector3 position2;
		position2 = base.transform.position;
		position2.y = position.y;
		float num;
		num = Vector3.Distance(position, position2);
		if (!this.m_IsSingleFade)
		{
			if (num > this.m_DistanceMax)
			{
				if (this.m_IsFadingOut)
				{
					this.m_IsFadingOut = false;
					this.m_ColorLerpTime = 0f;
				}
				this.m_IsFadingIn = true;
				color2.a = this.m_FadeIn;
			}
			else if (num < this.m_DistanceMin)
			{
				if (this.m_IsFadingIn)
				{
					this.m_IsFadingIn = false;
					this.m_ColorLerpTime = 0f;
				}
				this.m_IsFadingOut = true;
				color2.a = this.m_FadeOut;
			}
		}
		else
		{
			if (num < this.m_DistanceMin)
			{
				this.m_IsFading = true;
			}
			if (this.m_IsFading)
			{
				this.m_IsFadingOut = true;
				color2.a = this.m_FadeOut;
			}
		}
		if (color != color2)
		{
			color2 = Color.Lerp(color, color2, this.m_ColorLerpTime);
			this.m_MeshRenderer.material.SetColor("_Color", color2);
			if (this.m_ColorLerpTime < 1f)
			{
				this.m_ColorLerpTime += Time.deltaTime / this.m_FadeDuration;
			}
		}
		else if (this.m_IsFading)
		{
			this.m_IsFaded = true;
		}
	}
}
