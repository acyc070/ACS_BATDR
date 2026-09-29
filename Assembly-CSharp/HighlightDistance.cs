using DG.Tweening;
using UnityEngine;

public class HighlightDistance : JComponent
{
	private const string COLOR_VARIABLE = "_Color";

	private const float COLOR_LERP_TIME = 10f;

	[SerializeField]
	private float m_DistanceMin = 12f;

	[SerializeField]
	private float m_DistanceMax = 14f;

	private MeshRenderer m_MeshRenderer;

	private Sequence m_Sequence;

	private Color m_DefaultColor;

	private bool m_IsActive = true;

	private bool m_IsEntered;

	public override void Start()
	{
		if (this.m_MeshRenderer == null)
		{
			this.m_MeshRenderer = base.GetComponent<MeshRenderer>();
			this.m_DefaultColor = this.m_MeshRenderer.material.GetColor("_Color");
			Color black;
			black = Color.black;
			black.a = 0f;
			this.m_MeshRenderer.material.SetColor("_Color", black);
			this.m_IsActive = true;
		}
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && this.m_IsActive && !(GameManager.Instance.Player == null) && !this.m_IsEntered)
		{
			Vector3 position;
			position = GameManager.Instance.Player.transform.position;
			Color color;
			color = this.m_MeshRenderer.material.GetColor("_Color");
			Color color2;
			color2 = color;
			Vector3 position2;
			position2 = base.transform.position;
			position2.y = position.y;
			float num;
			num = Vector3.Distance(position, position2);
			if (num > this.m_DistanceMax)
			{
				color2 = Color.black;
			}
			else if (num < this.m_DistanceMin)
			{
				color2 = this.m_DefaultColor;
			}
			color2.a = 0f;
			if (color != color2)
			{
				color2 = Color.Lerp(color, color2, Time.deltaTime * 10f);
				this.m_MeshRenderer.material.SetColor("_Color", color2);
			}
		}
	}

	public void Deactivate()
	{
		if (this.m_MeshRenderer == null)
		{
			this.m_MeshRenderer = base.GetComponent<MeshRenderer>();
			this.m_DefaultColor = this.m_MeshRenderer.material.GetColor("_Color");
		}
		this.m_IsActive = false;
		this.m_MeshRenderer.material.SetColor("_Color", this.m_DefaultColor);
		Color black;
		black = Color.black;
		black.a = 0f;
		this.ResetSequence();
		this.m_Sequence.Insert(0f, this.m_MeshRenderer.material.DOColor(black, 1f).SetEase(Ease.OutSine));
	}

	public void Enter()
	{
		this.m_IsEntered = true;
	}

	public void Exit()
	{
		this.m_IsEntered = false;
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.KillSequence();
		this.m_MeshRenderer = null;
		base.OnDisposed();
	}
}
