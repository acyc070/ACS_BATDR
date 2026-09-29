using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIArchGateJDSPresents : UIController
{
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private TextMeshProUGUI m_Label;

	private UILabelDataVO m_DataVO;

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UILabelDataVO)_data;
		if (this.m_DataVO != null)
		{
			this.m_Label.text = this.m_DataVO.Message;
		}
		this.m_CanvasGroup.alpha = 0f;
	}

	public override void PlayIn()
	{
		this.m_CanvasGroup.DOFade(1f, 3f).SetEase(Ease.Linear).OnComplete(this.PlayInComplete);
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		this.PlayOut();
	}

	public override void PlayOut()
	{
		this.m_CanvasGroup.DOFade(0f, 2f).SetEase(Ease.Linear).SetDelay(1f)
			.OnComplete(this.PlayOutComplete);
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
