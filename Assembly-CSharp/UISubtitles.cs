using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UISubtitles : UIController
{
	[SerializeField]
	private TextMeshProUGUI m_SubtitleText;

	private List<UISubtitleDataVO> m_SubtitleQueue = new List<UISubtitleDataVO>();

	private Sequence m_SubtitleSequence;

	private float m_SubtitleTimer;

	private float m_SubtitleDuration;

	public override void Awake()
	{
		this.m_SubtitleText.text = "";
		this.m_SubtitleText.alpha = 0f;
	}

	protected override void OnInitialized(object _data)
	{
		this.m_SubtitleText.text = "";
		this.m_SubtitleText.alpha = 0f;
	}

	public void Update()
	{
		if (this.m_SubtitleQueue.Count > 0)
		{
			this.m_SubtitleTimer += Time.deltaTime;
			if (this.m_SubtitleTimer >= this.m_SubtitleDuration)
			{
				this.m_SubtitleTimer = 0f;
				this.m_SubtitleDuration = 0f;
				UISubtitleDataVO uISubtitleDataVO;
				uISubtitleDataVO = this.m_SubtitleQueue[0];
				this.m_SubtitleDuration = uISubtitleDataVO.Duration;
				this.m_SubtitleQueue.RemoveAt(0);
				this.m_SubtitleText.text = uISubtitleDataVO.Subtitles;
				this.m_SubtitleText.alpha = 0f;
				float num;
				num = 0.5f;
				float atPosition;
				atPosition = uISubtitleDataVO.Duration - (uISubtitleDataVO.IsTrimmed ? num : 0f);
				this.KillSubtitleSequence();
				this.m_SubtitleSequence = DOTween.Sequence();
				this.m_SubtitleSequence.Insert(0f, this.m_SubtitleText.DOFade(1f, num));
				this.m_SubtitleSequence.Insert(atPosition, this.m_SubtitleText.DOFade(0f, num));
				this.m_SubtitleSequence.OnComplete(this.HandleSubtitleOnComplete);
			}
		}
	}

	private void HandleSubtitleOnComplete()
	{
		this.KillSubtitleSequence();
		this.m_SubtitleText.text = "";
		this.m_SubtitleDuration = 0f;
		if (this.m_SubtitleQueue.Count <= 0)
		{
			this.PlayOut();
		}
	}

	public void ShowSubtitles(string subtitles, float duration, bool isTrimmed = false)
	{
		this.m_SubtitleQueue.Add(UISubtitleDataVO.Create(subtitles, duration, isTrimmed));
	}

	private void KillSubtitleSequence()
	{
		this.m_SubtitleSequence?.Kill();
		this.m_SubtitleSequence = null;
	}

	protected override void OnDisposed()
	{
		this.KillSubtitleSequence();
		unchecked
		{
			if (this.m_SubtitleQueue != null)
			{
				for (int num = this.m_SubtitleQueue.Count - 1; num >= 0; num--)
				{
					this.m_SubtitleQueue[num].Dispose();
				}
				this.m_SubtitleQueue.Clear();
				this.m_SubtitleQueue = null;
			}
			base.OnDisposed();
		}
	}
}
