using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementNotificationBoxLabel : UIElement
{
	[SerializeField]
	private RectTransform m_Content;

	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private TextMeshProUGUI m_AmountLabel;

	[SerializeField]
	private Image m_Icon;

	private UIElementNotificationBoxLabelDataVO m_DataVO;

	private Sequence m_Sequence;

	private Vector2 m_OriginAnchoredPosition;

	public event EventHandler OnComplete;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementNotificationBoxLabelDataVO)_data;
		this.m_CanvasGroup.alpha = 0f;
		this.m_OriginAnchoredPosition = this.m_Content.anchoredPosition;
		this.m_Content.anchoredPosition += new Vector2(400f, 0f);
		this.m_Label.text = this.m_DataVO.Label;
		this.m_Label.ForceMeshUpdate();
		if (this.m_DataVO.Icon != null)
		{
			this.m_Icon.sprite = this.m_DataVO.Icon;
			if (this.m_DataVO.Amount > 0)
			{
				this.m_AmountLabel.text = "x" + this.m_DataVO.Amount;
				this.m_AmountLabel.ForceMeshUpdate();
			}
			else
			{
				this.m_AmountLabel.gameObject.SetActive(value: false);
			}
		}
		else
		{
			this.m_AmountLabel.gameObject.SetActive(value: false);
			this.m_Icon.gameObject.SetActive(value: false);
		}
		Vector2 sizeDelta;
		sizeDelta = base.rectTransform.sizeDelta;
		sizeDelta.y = 0f;
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence().SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		this.m_Sequence.Insert(this.m_DataVO.Delay + 0f, this.m_CanvasGroup.DOFade(1f, 0.25f).SetEase(Ease.InSine));
		this.m_Sequence.Insert(this.m_DataVO.Delay + 0f, this.m_Content.DOAnchorPos(this.m_OriginAnchoredPosition, 0.25f).SetEase(Ease.InSine));
		this.m_Sequence.Insert(this.m_DataVO.Delay + 2.5f, this.m_CanvasGroup.DOFade(0f, 0.4f).SetEase(Ease.OutSine));
		this.m_Sequence.Insert(this.m_DataVO.Delay + 2.5f, this.m_Content.DOAnchorPosY(this.m_OriginAnchoredPosition.y + 200f, 0.5f).SetEase(Ease.OutSine));
		this.m_Sequence.Insert(this.m_DataVO.Delay + 3f, base.rectTransform.DOSizeDelta(sizeDelta, 0.25f).SetEase(Ease.OutSine));
		this.m_Sequence.InsertCallback(this.m_DataVO.Delay + 3.25f, this.SendOnComplete);
	}

	private void Update()
	{
		if (!base.IsDisposed && GameManager.Instance.GameState != GameState.PAUSE && this.m_Sequence != null && !this.m_Sequence.IsPlaying())
		{
			this.m_Sequence.Play();
		}
	}

	private void KillSequence()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	private void SendOnComplete()
	{
		this.m_DataVO = null;
		this.KillSequence();
		this.OnComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.KillSequence();
		base.OnDisposed();
	}
}
