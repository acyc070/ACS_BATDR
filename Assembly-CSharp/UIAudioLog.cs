using DG.Tweening;
using InControl;
using S13Audio.BATDR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioLog : UIController
{
	[Header("Content")]
	[SerializeField]
	private Image m_Filler;

	[SerializeField]
	private RectTransform m_Reel_01;

	[SerializeField]
	private RectTransform m_Reel_02;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private TextMeshProUGUI m_ListenLaterLabel;

	[Header("Keyboard")]
	[SerializeField]
	private GameObject m_KeyboardContainer;

	[SerializeField]
	private TextMeshProUGUI m_KeyboardInputLabel;

	[Header("Controller")]
	[SerializeField]
	private GameObject m_ControllerContainer;

	[SerializeField]
	private Image m_ControllerInputImage;

	private Sequence m_Sequence;

	private bool m_CanCancel;

	private bool m_IsPlayingOut;

	protected override void OnInitialized(object _data)
	{
	}

	public void Show(string name, AudioClip audioClip)
	{
		this.CheckInput();
		this.m_ListenLaterLabel.text = TextUtility.GetKey("UI_AUDIO_LOG_LISTEN_LATER");
		this.m_Label.text = name;
		this.m_Filler.fillAmount = 0f;
		this.m_Reel_01.DOKill();
		this.m_Reel_02.DOKill();
		this.m_Reel_01.DOLocalRotate(new Vector3(0f, 0f, -360f), 0.75f, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
		this.m_Reel_02.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.75f, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
		this.ResetSequence();
		this.m_Sequence.Insert(0f, this.m_Filler.DOFillAmount(1f, audioClip.length).SetEase(Ease.Linear));
		this.m_Sequence.InsertCallback(0.5f, () =>
		{
			this.m_CanCancel = true;
		});
		this.m_Sequence.OnComplete(this.PlayOut);
		BATDRAudioLogPlayer.Play(audioClip);
	}

	public override void PlayOut()
	{
		this.m_CanCancel = false;
		this.m_IsPlayingOut = true;
		this.m_Reel_01.DOKill();
		this.m_Reel_02.DOKill();
		this.KillSequence();
		BATDRAudioLogPlayer.Stop(this.InternalPlayOutComplete);
	}

	private void InternalPlayOutComplete()
	{
		if (!base.IsDisposed)
		{
			this.PlayOutComplete();
		}
	}

	private void Update()
	{
		if (!this.m_IsPlayingOut && this.m_CanCancel && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			this.CheckInput();
			if (PlayerInput.Cancel())
			{
				this.PlayOut();
			}
		}
	}

	private void CheckInput()
	{
		if (!GameManager.Instance.HasController)
		{
			if (this.m_ControllerContainer != null)
			{
				this.m_ControllerContainer.SetActive(value: false);
			}
			if (this.m_KeyboardContainer != null)
			{
				this.m_KeyboardContainer.SetActive(value: true);
			}
			this.m_KeyboardInputLabel.text = PlayerInputConstants.CANCEL;
		}
		else
		{
			if (this.m_KeyboardContainer != null)
			{
				this.m_KeyboardContainer.SetActive(value: false);
			}
			if (this.m_ControllerContainer != null)
			{
				this.m_ControllerContainer.SetActive(value: true);
			}
			this.m_ControllerInputImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action2);
		}
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
		this.m_Reel_01.DOKill();
		this.m_Reel_02.DOKill();
		this.KillSequence();
		base.OnDisposed();
	}
}
