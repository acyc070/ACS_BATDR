using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICutsceneBars : UIController
{
	[Header("Cutscene Bars")]
	[SerializeField]
	private RectTransform m_TopBar;

	[SerializeField]
	private RectTransform m_BottomBar;

	[Header("Skip Content")]
	[SerializeField]
	private RectTransform m_SkipContainer;

	[SerializeField]
	private Image m_SkipFiller;

	[SerializeField]
	private Image m_SkipControllerInputImage;

	[SerializeField]
	private TextMeshProUGUI m_SkipInputLabel;

	[SerializeField]
	private TextMeshProUGUI m_SkipLabel;

	private CutsceneDirector m_CutsceneDirector;

	private Sequence m_Sequence;

	private Vector2 m_TopStartPosition;

	private Vector2 m_TopEndPosition;

	private Vector2 m_BottomStartPosition;

	private Vector2 m_BottomEndPosition;

	private bool m_IsActive;

	private bool m_CanSkip;

	private bool m_IsSkipping;

	private bool m_DisableSkip;

	private float m_InputTimer;

	private float m_InputTimerMax = 1.25f;

	private int m_SkipCount;

	private float m_SkipTimer;

	private float m_SkipTimerMax = 2f;

	private bool m_HasController;

	protected override void OnInitialized(object _data)
	{
		this.m_CutsceneDirector = (CutsceneDirector)_data;
		this.m_TopStartPosition = this.m_TopBar.anchoredPosition;
		this.m_BottomStartPosition = this.m_BottomBar.anchoredPosition;
		this.m_TopEndPosition = this.m_TopBar.anchoredPosition;
		this.m_TopEndPosition.y += this.m_TopBar.sizeDelta.y;
		this.m_TopBar.anchoredPosition = this.m_TopEndPosition;
		this.m_BottomEndPosition = this.m_BottomBar.anchoredPosition;
		this.m_BottomEndPosition.y -= this.m_BottomBar.sizeDelta.y;
		this.m_BottomBar.anchoredPosition = this.m_BottomEndPosition;
		this.m_SkipInputLabel.text = "E";
		this.m_SkipInputLabel.gameObject.SetActive(value: false);
		this.m_SkipControllerInputImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action1);
		this.m_SkipControllerInputImage.gameObject.SetActive(value: false);
		this.m_SkipLabel.text = TextUtility.GetKey("UI_SKIP").ToUpper();
		this.ClearTimer(setActive: false);
	}

	public void SkipPlayIn()
	{
		this.KillSequece();
		this.m_TopBar.anchoredPosition = this.m_TopStartPosition;
		this.m_BottomBar.anchoredPosition = this.m_BottomStartPosition;
		this.PlayInComplete();
	}

	public void DisableSkip()
	{
		this.m_DisableSkip = true;
	}

	public override void PlayIn()
	{
		this.ResetSequence();
		this.m_Sequence.Insert(0f, this.m_TopBar.DOAnchorPos(this.m_TopStartPosition, 1f).SetEase(Ease.OutSine));
		this.m_Sequence.Insert(0f, this.m_BottomBar.DOAnchorPos(this.m_BottomStartPosition, 1f).SetEase(Ease.OutSine));
		this.m_Sequence.InsertCallback(1.5f, this.PlayInComplete);
	}

	public override void PlayInComplete()
	{
		this.m_IsActive = true;
		base.PlayInComplete();
	}

	public override void PlayOut()
	{
		this.m_IsActive = false;
		this.ClearTimer(setActive: false);
		this.ResetSequence();
		this.m_Sequence.Insert(0f, this.m_TopBar.DOAnchorPos(this.m_TopEndPosition, 1f).SetEase(Ease.InSine));
		this.m_Sequence.Insert(0f, this.m_BottomBar.DOAnchorPos(this.m_BottomEndPosition, 1f).SetEase(Ease.InSine));
		this.m_Sequence.OnComplete(this.PlayOutComplete);
	}

	private void Update()
	{
		if (!this.m_IsActive || this.m_CutsceneDirector == null || this.m_IsSkipping || base.IsDisposed || GameManager.Instance.IsPaused || this.m_DisableSkip)
		{
			return;
		}
		unchecked
		{
			if (!this.m_CanSkip)
			{
				if (PlayerInput.Any())
				{
					this.m_SkipCount++;
					if (this.m_SkipCount >= 2)
					{
						this.m_HasController = GameManager.Instance.HasController;
						this.m_CanSkip = true;
						this.ClearTimer(setActive: true);
					}
				}
			}
			else if (this.m_HasController)
			{
				this.m_SkipInputLabel.gameObject.SetActive(value: false);
				this.m_SkipControllerInputImage.gameObject.SetActive(value: true);
				if (PlayerInput.ControllerAction1Up())
				{
					this.ClearTimer(setActive: true);
				}
				else if (PlayerInput.ControllerAction1Hold())
				{
					this.CheckInputTimer();
				}
				else
				{
					this.CheckSkipTimer();
				}
			}
			else
			{
				this.m_SkipControllerInputImage.gameObject.SetActive(value: false);
				this.m_SkipInputLabel.gameObject.SetActive(value: true);
				if (PlayerInput.InteractOnReleased())
				{
					this.ClearTimer(setActive: true);
				}
				else if (PlayerInput.InteractOnHold())
				{
					this.CheckInputTimer();
				}
				else
				{
					this.CheckSkipTimer();
				}
			}
			if (this.m_IsSkipping)
			{
				this.ClearTimer(setActive: false);
				this.m_CutsceneDirector.Skip();
			}
		}
	}

	private void CheckInputTimer()
	{
		this.m_SkipTimer = 0f;
		if (this.m_InputTimer < this.m_InputTimerMax)
		{
			this.m_InputTimer += Time.unscaledDeltaTime;
			float num;
			num = this.m_InputTimer / this.m_InputTimerMax;
			GameManager.Instance.TriggerRumble(0.05f, num);
			if (num >= 1f)
			{
				num = 1f;
				this.m_IsSkipping = true;
			}
			this.m_SkipFiller.fillAmount = num;
		}
	}

	private void CheckSkipTimer()
	{
		if (this.m_SkipTimer >= this.m_SkipTimerMax)
		{
			this.m_SkipCount = 0;
			this.m_CanSkip = false;
			this.ClearTimer(setActive: false);
		}
		else
		{
			this.m_SkipTimer += Time.unscaledDeltaTime;
		}
	}

	private void ClearTimer(bool setActive)
	{
		this.m_InputTimer = 0f;
		this.m_SkipFiller.fillAmount = 0f;
		this.m_SkipContainer.gameObject.SetActive(setActive);
	}

	private void ResetSequence()
	{
		this.KillSequece();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequece()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.KillSequece();
		this.m_CutsceneDirector = null;
		base.OnDisposed();
	}
}
