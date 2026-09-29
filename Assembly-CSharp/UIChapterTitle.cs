using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIChapterTitle : UIController
{
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private TextMeshProUGUI m_ChapterLabel;

	[SerializeField]
	private TextMeshProUGUI m_TitleLabel;

	private string[] m_Labels;

	protected override void OnInitialized(object _data)
	{
		this.m_Labels = (string[])_data;
		this.m_CanvasGroup.alpha = 0f;
		if (this.m_Labels == null)
		{
			JDebug.LogError("UIChapterTitle :: OnInitialized => m_Labels = " + this.m_Labels, this);
		}
		if (this.m_Labels != null)
		{
			this.m_ChapterLabel.text = this.m_Labels[0];
			this.m_TitleLabel.text = this.m_Labels[1];
		}
	}

	public override void PlayIn()
	{
		this.m_CanvasGroup.DOFade(1f, 2f).SetDelay(0.25f).SetEase(Ease.Linear)
			.OnComplete(this.PlayInComplete);
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		this.PlayOut();
	}

	public override void PlayOut()
	{
		this.m_CanvasGroup.DOFade(0f, 3f).SetEase(Ease.Linear).SetDelay(4.5f)
			.OnComplete(this.PlayOutComplete);
	}

	protected override void OnDisposed()
	{
		this.m_CanvasGroup.DOKill();
		base.OnDisposed();
	}
}
