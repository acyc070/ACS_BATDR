using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UICredits : UIController
{
	[SerializeField]
	private RectTransform m_Container;

	[SerializeField]
	private Image m_BlackImage;

	[SerializeField]
	private Image[] m_Logos;

	[SerializeField]
	private AudioSource m_AudioSource;

	[Header("Skip")]
	[SerializeField]
	private CanvasGroup m_Canvas;

	[SerializeField]
	private Image m_SkipIcon;

	[SerializeField]
	private TextMeshProUGUI m_InputLabel;

	[SerializeField]
	private Image m_InputImage;

	[SerializeField]
	private TextMeshProUGUI m_ExitLabel;

	private TextMeshProUGUI[] m_AllText;

	private bool m_Data;

	private bool m_CanExit;

	private float m_InputTimer;

	private float m_InputTimerMax = 1.25f;

	private bool m_HasController;

	public bool IsMainMenu { get; private set; }

	public void SetMainMenu(bool isMainMenu)
	{
		this.IsMainMenu = isMainMenu;
	}

	protected override void OnInitialized(object _data)
	{
		if (_data != null)
		{
			this.m_Data = (bool)_data;
		}
		this.m_ExitLabel.text = TextUtility.GetKey("UI_EXIT").ToUpper();
		this.m_InputLabel.text = "E";
		this.m_InputImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action1);
		if (this.m_Data)
		{
			this.m_BlackImage.gameObject.SetActive(value: false);
			this.m_Canvas.alpha = 0f;
			this.m_SkipIcon.fillAmount = 0f;
		}
		else
		{
			this.m_Canvas.gameObject.SetActive(value: false);
			this.m_AllText = this.m_Container.GetComponentsInChildren<TextMeshProUGUI>();
		}
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		unchecked
		{
			if (!this.m_Data)
			{
				GameManager.Instance.HideScreenBlocker(0f);
				this.m_AudioSource.Play();
				Color endValue;
				endValue = new Color(0.725f, 0.533f, 0.13f, 1f);
				float num;
				num = this.m_AudioSource.clip.length / 2f;
				float delay;
				delay = num / 2f;
				for (int i = 0; i < this.m_AllText.Length; i++)
				{
					this.m_AllText[i].DOColor(endValue, num).SetDelay(delay).SetEase(Ease.Linear);
				}
				if (this.m_Logos != null)
				{
					for (int j = 0; j < this.m_Logos.Length; j++)
					{
						this.m_Logos[j].DOColor(endValue, num).SetDelay(delay).SetEase(Ease.Linear);
					}
				}
			}
			this.m_Container.DOAnchorPosY(this.m_Container.sizeDelta.y, this.m_AudioSource.clip.length).SetEase(Ease.Linear).OnComplete(this.CreditsOnComplete);
		}
	}

	private void CreditsOnComplete()
	{
		if (this.m_Data)
		{
			this.m_HasController = GameManager.Instance.HasController;
			if (this.m_HasController)
			{
				this.m_InputLabel.gameObject.SetActive(value: false);
				this.m_InputImage.gameObject.SetActive(value: true);
			}
			else
			{
				this.m_InputImage.gameObject.SetActive(value: false);
				this.m_InputLabel.gameObject.SetActive(value: true);
			}
			this.m_Container.gameObject.SetActive(value: false);
			this.m_Canvas.DOFade(1f, 2f).OnComplete(() =>
			{
				this.m_CanExit = true;
			});
		}
		else
		{
			this.PlayOut();
		}
	}

	private void Update()
	{
		if (base.IsDisposed)
		{
			return;
		}
		if (this.IsMainMenu && PlayerInput.Pause())
		{
			this.PlayOut();
		}
		if (!this.m_CanExit)
		{
			return;
		}
		if (this.m_HasController)
		{
			if (PlayerInput.ControllerAction1Up())
			{
				this.ClearTimer(setActive: true);
			}
			else if (PlayerInput.ControllerAction1Hold())
			{
				this.CheckInputTimer();
			}
		}
		else if (PlayerInput.InteractOnReleased())
		{
			this.ClearTimer(setActive: true);
		}
		else if (PlayerInput.InteractOnHold())
		{
			this.CheckInputTimer();
		}
	}

	private void CheckInputTimer()
	{
		if (this.m_InputTimer < this.m_InputTimerMax)
		{
			this.m_InputTimer += Time.unscaledDeltaTime;
			float num;
			num = this.m_InputTimer / this.m_InputTimerMax;
			if (num >= 1f)
			{
				num = 1f;
				this.PlayOut();
			}
			this.m_SkipIcon.fillAmount = num;
		}
	}

	private void ClearTimer(bool setActive)
	{
		this.m_InputTimer = 0f;
		this.m_SkipIcon.fillAmount = 0f;
		this.m_Canvas.gameObject.SetActive(setActive);
	}

	protected override void OnDisposed()
	{
		this.m_Container.DOKill();
		unchecked
		{
			if (!this.m_Data)
			{
				for (int i = 0; i < this.m_AllText.Length; i++)
				{
					TextMeshProUGUI textMeshProUGUI;
					textMeshProUGUI = this.m_AllText[i];
					if (textMeshProUGUI != null)
					{
						textMeshProUGUI.DOKill();
					}
				}
				if (this.m_Logos != null)
				{
					for (int j = 0; j < this.m_Logos.Length; j++)
					{
						Image image;
						image = this.m_Logos[j];
						if (image != null)
						{
							image.DOKill();
						}
					}
				}
			}
			this.m_AllText = null;
			this.m_Logos = null;
			SceneManager.LoadScene("Reset");
			base.OnDisposed();
		}
	}
}
