using DG.Tweening;
using S13Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITeleport : UIController
{
	[Header("Teleport")]
	[SerializeField]
	private CanvasGroup m_Canvas;

	[SerializeField]
	private Image m_Teleport;

	[Header("Effects")]
	[SerializeField]
	private Image m_Shimmer;

	[SerializeField]
	private RectTransform m_Unavailable;

	[Header("Button")]
	[SerializeField]
	private RectTransform m_Button;

	[SerializeField]
	private CanvasGroup m_ButtonKeyboard;

	[SerializeField]
	private CanvasGroup m_ButtonController;

	[SerializeField]
	private TextMeshProUGUI m_ButtonControllerLabel;

	[Space]
	[Header("S13 - Audio")]
	[SerializeField]
	private S13AudioHandler m_AudioOnReady;

	[SerializeField]
	private S13AudioHandler m_AudioOnUse;

	protected override void OnInitialized(object _data)
	{
		this.m_Teleport.fillAmount = 0f;
		this.m_Teleport.transform.parent.gameObject.SetActive(value: false);
		this.m_Shimmer.gameObject.SetActive(value: false);
		this.m_Unavailable.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused || this.m_ButtonKeyboard == null || this.m_ButtonController == null)
		{
			return;
		}
		if (GameManager.Instance.HasController)
		{
			if (this.m_ButtonKeyboard.gameObject.activeInHierarchy && !this.m_ButtonController.gameObject.activeInHierarchy)
			{
				this.EnableControllerButton();
			}
		}
		else if (this.m_ButtonController.gameObject.activeInHierarchy && !this.m_ButtonKeyboard.gameObject.activeInHierarchy)
		{
			this.EnableKeyboardButton();
		}
	}

	private void EnableControllerButton()
	{
		this.m_ButtonController.gameObject.SetActive(value: true);
		this.m_ButtonController.alpha = 1f;
		this.m_ButtonKeyboard.gameObject.SetActive(value: false);
		this.m_ButtonKeyboard.alpha = 0f;
		this.m_ButtonControllerLabel.text = "LT";
	}

	private void EnableKeyboardButton()
	{
		this.m_ButtonKeyboard.gameObject.SetActive(value: true);
		this.m_ButtonKeyboard.alpha = 1f;
		this.m_ButtonController.gameObject.SetActive(value: false);
		this.m_ButtonController.alpha = 0f;
	}

	public void Show()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus != AbilityStatus.Active)
		{
			return;
		}
		if (GameManager.Instance.HasController)
		{
			if (this.m_ButtonController != null)
			{
				this.m_ButtonController.gameObject.SetActive(value: true);
			}
			if (this.m_ButtonKeyboard != null)
			{
				this.m_ButtonKeyboard.gameObject.SetActive(value: false);
			}
		}
		else
		{
			if (this.m_ButtonKeyboard != null)
			{
				this.m_ButtonKeyboard.gameObject.SetActive(value: true);
			}
			if (this.m_ButtonController != null)
			{
				this.m_ButtonController.gameObject.SetActive(value: false);
			}
		}
		this.m_Teleport.transform.parent.gameObject.SetActive(value: true);
		this.m_Teleport.fillAmount = 1f;
	}

	public void Hide()
	{
		this.m_Canvas.DOKill();
		this.m_Canvas.DOFade(0f, 0.5f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true).OnComplete(this.ForceHide);
	}

	public void ForceHide()
	{
		this.m_Canvas.gameObject.SetActive(value: false);
	}

	public void Use()
	{
		this.DOFillAmount(0f);
		if (GameManager.Instance.HasController)
		{
			this.DOButton(ref this.m_ButtonController);
		}
		else
		{
			this.DOButton(ref this.m_ButtonKeyboard);
		}
		this.m_AudioOnUse.Play();
	}

	private void DOFillAmount(float endValue, float duration = 0.25f)
	{
		this.m_Teleport.DOKill();
		this.m_Teleport.DOFillAmount(endValue, duration).SetEase(Ease.Linear);
	}

	private void DOButton(ref CanvasGroup _button)
	{
		CanvasGroup button;
		button = _button;
		if (button != null)
		{
			button.transform.DOKill();
			button.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
			{
				button.transform.localScale = Vector3.one;
			});
			button.DOKill();
			button.DOFade(0f, 0.3f).OnComplete(() =>
			{
				button.gameObject.SetActive(value: false);
			});
		}
	}

	public void DOCooldown(float value, float max)
	{
		Color color;
		color = this.m_Teleport.color;
		color.a = 0.5f;
		this.m_Teleport.color = color;
		float fillAmount;
		fillAmount = value / max;
		if (value >= max)
		{
			fillAmount = 1f;
		}
		this.m_Teleport.fillAmount = fillAmount;
	}

	public void DOCooldownComplete()
	{
		Color color;
		color = this.m_Teleport.color;
		color.a = 1f;
		this.m_Teleport.color = color;
		this.m_Teleport.transform.localScale = Vector3.one;
		this.m_Teleport.transform.DOKill();
		this.m_Teleport.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f).OnComplete(() =>
		{
			if (GameManager.Instance.HasController)
			{
				this.DOTeleportCooldown(ref this.m_ButtonController);
			}
			else
			{
				this.DOTeleportCooldown(ref this.m_ButtonKeyboard);
			}
			Vector2 shimmerAnchor;
			shimmerAnchor = this.m_Shimmer.rectTransform.anchoredPosition;
			shimmerAnchor.y = -150f;
			this.m_Shimmer.rectTransform.anchoredPosition = shimmerAnchor;
			this.m_Shimmer.gameObject.SetActive(value: true);
			this.m_Shimmer.DOKill();
			this.m_Shimmer.rectTransform.DOAnchorPosY(150f, 0.5f).OnComplete(() =>
			{
				this.m_Shimmer.gameObject.SetActive(value: false);
				this.m_Shimmer.rectTransform.anchoredPosition = shimmerAnchor;
			});
		});
		this.m_AudioOnReady.Play();
	}

	private void DOTeleportCooldown(ref CanvasGroup _button)
	{
		CanvasGroup obj;
		obj = _button;
		obj.transform.localScale = Vector3.one;
		obj.gameObject.SetActive(value: true);
		obj.alpha = 0f;
		obj.DOKill();
		obj.DOFade(1f, 0.25f);
	}

	public void DisplayTeleport()
	{
		this.m_Canvas.gameObject.SetActive(value: true);
		this.m_Teleport.transform.parent.gameObject.SetActive(value: true);
		this.m_Canvas.DOKill();
		this.m_Canvas.DOFade(1f, 0.5f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
	}

	public void DisableTeleport()
	{
		this.m_Unavailable.gameObject.SetActive(value: true);
		this.m_Button.gameObject.SetActive(value: false);
		this.m_ButtonKeyboard.gameObject.SetActive(value: false);
		this.m_ButtonController.gameObject.SetActive(value: false);
		this.m_Teleport.gameObject.SetActive(value: false);
	}

	public void EnableTeleport()
	{
		this.m_Unavailable.gameObject.SetActive(value: false);
		this.m_Button.gameObject.SetActive(value: true);
		if (GameManager.Instance.HasController)
		{
			this.m_ButtonController.gameObject.SetActive(value: true);
			this.m_ButtonKeyboard.gameObject.SetActive(value: false);
		}
		else
		{
			this.m_ButtonKeyboard.gameObject.SetActive(value: true);
			this.m_ButtonController.gameObject.SetActive(value: false);
		}
		this.m_Teleport.gameObject.SetActive(value: true);
	}

	protected override void OnDisposed()
	{
		if ((bool)this.m_ButtonKeyboard)
		{
			this.m_ButtonKeyboard.DOKill();
		}
		if ((bool)this.m_ButtonController)
		{
			this.m_ButtonController.DOKill();
		}
		if ((bool)this.m_Canvas)
		{
			this.m_Canvas.DOKill();
		}
		if ((bool)this.m_Teleport)
		{
			this.m_Teleport.DOKill();
		}
		base.OnDisposed();
	}
}
