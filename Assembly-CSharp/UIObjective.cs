using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIObjective : UIController
{
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_MessageLabel;

	private UIObjectiveDataVO m_DataVO;

	private Sequence m_Sequence;

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UIObjectiveDataVO)_data;
		if (this.m_DataVO == null)
		{
			Debug.LogError("UIObjective :: OnInitialized => m_DataVO = " + this.m_DataVO, this);
		}
		this.m_CanvasGroup.transform.localScale = Vector3.one;
		this.m_HeaderLabel.text = this.m_DataVO.HeaderLabel;
		this.m_MessageLabel.text = this.m_DataVO.MessageLabel;
	}

	public override void PlayIn()
	{
		this.m_Sequence?.Kill();
		this.m_Sequence = DOTween.Sequence();
		this.m_Sequence.Insert(0f, this.m_CanvasGroup.transform.DOPunchScale(Vector3.one * 0.125f, 0.1f));
		this.m_Sequence.Insert(3.5f, this.m_CanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.Linear));
		this.m_Sequence.OnComplete(this.PlayOut);
	}

	protected override void OnDisposed()
	{
		this.m_Sequence?.Kill();
		this.m_Sequence = null;
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
