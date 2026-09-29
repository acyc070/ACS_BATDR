using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIArchGateIntro : UIController
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	private Sequence m_Sequence;

	protected override void OnInitialized(object _data)
	{
		this.m_CanvasGroup.alpha = 0f;
	}

	public override void PlayIn()
	{
		this.InitializeSequence();
		this.m_Sequence.Insert(1f, this.m_CanvasGroup.DOFade(1f, 1f).SetEase(Ease.OutQuad));
		this.m_Sequence.Insert(3f, this.m_CanvasGroup.DOFade(0f, 1f).SetEase(Ease.InQuad));
		this.m_Sequence.OnComplete(this.PlayInComplete);
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		this.PlayOut();
	}

	private void InitializeSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		this.m_Sequence?.Kill();
		this.m_Sequence = null;
	}

	protected override void OnDisposed()
	{
		this.KillSequence();
		base.OnDisposed();
	}
}
