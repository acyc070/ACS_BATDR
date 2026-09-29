using System;
using DG.Tweening;
using S13Audio.BATDR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementAudioLogButtonLabel : UIElementButton
{
	[Header("Button Label")]
	[SerializeField]
	protected UIElementLabel m_Label;

	[Header("Button")]
	[SerializeField]
	protected UIButton m_PlayButton;

	[SerializeField]
	private RectTransform m_PlayContainer;

	[SerializeField]
	private RectTransform m_StopContainer;

	[SerializeField]
	private Image m_PlayIcon;

	[SerializeField]
	private Image m_PlayIconController;

	[SerializeField]
	private TextMeshProUGUI m_PlayInputLabel;

	[SerializeField]
	private Image m_StopIcon;

	[SerializeField]
	private Image m_StopIconController;

	[SerializeField]
	private TextMeshProUGUI m_StopInputLabel;

	[Header("Filler")]
	[SerializeField]
	protected Image m_SelectImage;

	[Header("Label Settings")]
	[SerializeField]
	protected float m_EnterSize = 100f;

	[SerializeField]
	protected float m_ExitSize = 76f;

	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	private Sequencer m_Sequencer;

	private UIElementAudioLogDataVO m_DataVO;

	public string Label => this.m_Label.Label.text;

	public UIElementAudioLogDataVO DataVO => this.m_DataVO;

	public bool IsPlaying { get; private set; }

	public event EventHandler OnPlay;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementAudioLogDataVO)_data;
		this.m_Sequencer = new Sequencer(isIndependent: true);
		if (this.m_DataVO.Button.LabelDataVO != null)
		{
			this.m_Label.Label.text = this.m_DataVO.Button.LabelDataVO.Label;
			this.m_Label.Label.color = this.m_DataVO.Button.LabelDataVO.Color;
			this.m_Label.Label.fontSizeMax = this.m_DataVO.Button.LabelDataVO.FontSize;
		}
		else
		{
			this.m_Label.Label.text = this.m_DataVO.Button.Label;
			this.m_Label.Label.color = this.m_InactiveColor;
			this.m_Label.Label.fontSizeMax = this.m_ExitSize;
		}
		this.SelectImageOff();
		this.m_PlayButton.gameObject.SetActive(value: false);
	}

	public void ShowPlayButton()
	{
		this.m_PlayButton.gameObject.SetActive(value: true);
		this.ShowPlayIcon();
		this.m_PlayButton.OnClick -= this.HandlePlayButtonOnClick;
		this.m_PlayButton.OnClick += this.HandlePlayButtonOnClick;
	}

	private void HandlePlayButtonOnClick(object sender, EventArgs e)
	{
		this.OnPlay.Send(this);
	}

	public void PlayAudioClip(BATDRAudioLogPlayer bATDRAudioLogPlayer)
	{
		GameManager.Instance.ClearAudioLog();
		BATDRAudioLogPlayer.Instance = bATDRAudioLogPlayer;
		AudioClip audioLogClip;
		audioLogClip = GameManager.Instance.GetAudioLogClip(this.m_DataVO.ID);
		if (audioLogClip != null)
		{
			this.IsPlaying = true;
			this.ShowStopIcon();
			BATDRAudioLogPlayer.Play(audioLogClip);
			this.m_SelectImage.DOFillAmount(0f, audioLogClip.length).SetEase(Ease.Linear).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
				.OnComplete(this.StopAudioClip);
		}
	}

	public void StopAudioClip()
	{
		this.IsPlaying = false;
		this.ShowPlayIcon();
		BATDRAudioLogPlayer.ForceStop();
		this.m_SelectImage.DOKill();
		this.m_SelectImage.fillAmount = 1f;
	}

	private void ShowPlayIcon()
	{
		this.m_PlayContainer.gameObject.SetActive(value: true);
		this.m_PlayIconController.enabled = GameManager.Instance.HasController;
		this.m_PlayIcon.enabled = !GameManager.Instance.HasController;
		this.m_StopContainer.gameObject.SetActive(value: false);
		this.m_StopInputLabel.text = string.Empty;
		if (GameManager.Instance.HasController)
		{
			this.m_PlayInputLabel.text = "A";
		}
		else
		{
			this.m_PlayInputLabel.text = string.Empty;
		}
	}

	private void ShowStopIcon()
	{
		this.m_StopContainer.gameObject.SetActive(value: true);
		this.m_StopIconController.enabled = GameManager.Instance.HasController;
		this.m_StopIcon.enabled = !GameManager.Instance.HasController;
		this.m_PlayContainer.gameObject.SetActive(value: false);
		this.m_PlayInputLabel.text = string.Empty;
		if (GameManager.Instance.HasController)
		{
			this.m_StopInputLabel.text = "A";
		}
		else
		{
			this.m_StopInputLabel.text = string.Empty;
		}
	}

	private void CancelAudioClip()
	{
		this.m_PlayButton.OnClick -= this.HandlePlayButtonOnClick;
		this.m_PlayButton.gameObject.SetActive(value: false);
		this.StopAudioClip();
	}

	public override void ForceOnEnter()
	{
		this.m_Label.Label.color = this.m_ActiveColor;
		this.m_Label.Label.fontSize = this.m_EnterSize;
		if ((bool)this.m_SelectImage)
		{
			this.m_SelectImage.enabled = true;
			this.m_SelectImage.color = new Color(this.m_SelectImage.color.r, this.m_SelectImage.color.g, this.m_SelectImage.color.b, 1f);
		}
	}

	public override void ForceOnExit()
	{
		this.m_PlayButton.OnClick -= this.HandlePlayButtonOnClick;
		this.m_PlayButton.gameObject.SetActive(value: false);
		this.StopAudioClip();
		this.m_Label.Label.color = this.m_InactiveColor;
		this.m_Label.Label.fontSize = this.m_ExitSize;
		this.SelectImageOff();
	}

	private void SelectImageOff()
	{
		if ((bool)this.m_SelectImage)
		{
			this.m_SelectImage.enabled = false;
			this.m_SelectImage.color = new Color(this.m_SelectImage.color.r, this.m_SelectImage.color.g, this.m_SelectImage.color.b, 0f);
		}
	}

	protected override void InternalHandleButtonOnExit(object sender, EventArgs e)
	{
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOFontSize(this.m_ExitSize, 0.1f), this.m_Label.Label.DOColor(this.m_InactiveColor, 0.1f));
	}

	protected override void InternalHandleButtonOnEnter(object sender, EventArgs e)
	{
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOFontSize(this.m_EnterSize, 0.1f), this.m_Label.Label.DOColor(this.m_ActiveColor, 0.1f));
	}

	protected override void InteralResetButton()
	{
		this.ForceOnExit();
		this.ResetListeners();
	}

	public void ResetListeners()
	{
		this.RemoveListeners();
		this.AddListeners();
	}

	protected override void OnDisposed()
	{
		this.CancelAudioClip();
		this.m_Sequencer?.Dispose();
		this.m_Sequencer = null;
		this.m_SelectImage.DOKill();
		base.OnDisposed();
	}
}
