using System;
using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGentExchange : UIController
{
	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_ExchangeTitleLabel;

	[SerializeField]
	private TextMeshProUGUI m_ExchangeDescriptionLabel;

	[Header("Cards")]
	[SerializeField]
	private RectTransform m_CardIconContaner;

	[SerializeField]
	private TextMeshProUGUI m_CardAmountLabel;

	[Header("Requirements")]
	[SerializeField]
	private TextMeshProUGUI m_RequirementsTitleLabel;

	[SerializeField]
	private RectTransform m_ComponentsContainer;

	[Header("Exchange Buttons")]
	[SerializeField]
	private CanvasGroup m_ExchangeCanvasGroup;

	[SerializeField]
	private UIButton m_ExchangeButton;

	[SerializeField]
	private GameObject m_ExchangeButtonIcon;

	[SerializeField]
	private TextMeshProUGUI m_ExchangeButtonLabel;

	[SerializeField]
	private Image m_ExchangeButtonIconImage;

	[Header("Back Buttons")]
	[SerializeField]
	private UIButton m_BackButton;

	[SerializeField]
	private GameObject m_BackButtonIcon;

	[SerializeField]
	private TextMeshProUGUI m_BackButtonLabel;

	[SerializeField]
	private Image m_BackButtonIconImage;

	[Header("Button Colors")]
	[SerializeField]
	private Color m_SelectColor;

	[SerializeField]
	private Color m_DeselectColor;

	private float m_InputDelay = 0.25f;

	private float m_InputTimer;

	private bool m_IsActive;

	private bool m_CanExchange;

	public event EventHandler OnExchanged;

	protected override void OnInitialized(object _data)
	{
		this.RemoveListeners();
		this.m_ExchangeTitleLabel.text = TextUtility.GetKey("UI_GENT_EXCHANGE_HEADER").ToUpper();
		this.m_ExchangeDescriptionLabel.text = TextUtility.GetKey("UI_GENT_EXCHANGE_MESSAGE").ToUpper();
		this.m_RequirementsTitleLabel.text = TextUtility.GetKey("UI_GENT_EXCHANGE_REQUIRED").ToUpper();
		this.m_CardAmountLabel.text = "x" + GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards;
		this.SetRequirements();
		this.SetButtons(GameManager.Instance.HasController);
	}

	private void SetRequirements()
	{
		UIElementGentUpgradeComponent[] componentsInChildren;
		componentsInChildren = this.m_ComponentsContainer.GetComponentsInChildren<UIElementGentUpgradeComponent>(includeInactive: true);
		if (componentsInChildren != null)
		{
			UIElementGentUpgradeComponent[] array;
			array = componentsInChildren;
			for (int i = 0; i < array.Length; i = unchecked(i + 1))
			{
				array[i].Dispose();
			}
		}
		this.m_CanExchange = true;
		this.SetComponent("Icon/Collectables/Large/UIIcon_BatteryCasing_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.BatteryCasings, GentCardExchanteRequirements.GetBatteryCasings());
		this.SetComponent("Icon/Collectables/Large/UIIcon_Toolkit_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Toolkits, GentCardExchanteRequirements.GetToolkits());
		this.SetComponent("Icon/Collectables/Large/UIIcon_Parts_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Parts, GentCardExchanteRequirements.GetParts());
	}

	private void SetComponent(string icon, int amount, int max)
	{
		if (amount >= max)
		{
			amount = max;
		}
		else
		{
			this.m_CanExchange = false;
		}
		_ = (UIElementGentUpgradeComponent)UIManager.CreateElement(_data: new UIElementGentUpgradeComponentDataVO(icon, amount, max), parent: this.m_ComponentsContainer, _prefabKey: "UI/Elements/UIElementGentExchangeComponent");
	}

	private void SetButtons(bool hasController)
	{
		if (!hasController)
		{
			this.m_ExchangeButtonLabel.alignment = TextAlignmentOptions.Midline;
			this.m_BackButtonLabel.alignment = TextAlignmentOptions.Midline;
		}
		this.m_ExchangeButtonIcon.SetActive(hasController);
		this.m_ExchangeButtonIconImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action3);
		if (this.m_CanExchange)
		{
			this.m_ExchangeCanvasGroup.alpha = 1f;
			if (!hasController)
			{
				this.m_ExchangeButton.OnEnter += this.HandleExchangeButtonOnEnter;
				this.m_ExchangeButton.OnExit += this.HandleExchangeButtonOnExit;
				this.m_ExchangeButton.OnClick += this.HandleExchangeButtonOnClick;
			}
		}
		else
		{
			this.m_ExchangeCanvasGroup.alpha = 0.2f;
		}
		this.m_ExchangeButtonLabel.text = TextUtility.GetKey("UI_GENT_EXCHANGE_EXCHANGE").ToUpper();
		this.m_ExchangeButtonLabel.ForceMeshUpdate();
		Vector2 sizeDelta;
		sizeDelta = this.m_ExchangeButtonLabel.rectTransform.sizeDelta;
		sizeDelta.x = this.m_ExchangeButtonLabel.GetRenderedValues().x;
		this.m_ExchangeButtonLabel.rectTransform.sizeDelta = sizeDelta;
		this.m_BackButtonIcon.SetActive(hasController);
		this.m_BackButtonIconImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action2);
		if (!hasController)
		{
			this.m_BackButton.OnEnter += this.HandleBackButtonOnEnter;
			this.m_BackButton.OnExit += this.HandleBackButtonOnExit;
			this.m_BackButton.OnClick += this.HandleBackButtonOnClick;
		}
		this.m_BackButtonLabel.text = TextUtility.GetKey("MENU_BACK").ToUpper();
		this.m_BackButtonLabel.ForceMeshUpdate();
		Vector2 sizeDelta2;
		sizeDelta2 = this.m_BackButtonLabel.rectTransform.sizeDelta;
		sizeDelta2.x = this.m_BackButtonLabel.GetRenderedValues().x;
		this.m_BackButtonLabel.rectTransform.sizeDelta = sizeDelta2;
	}

	private void HandleExchangeButtonOnEnter(object sender, EventArgs e)
	{
		if (this.m_CanExchange)
		{
			this.m_ExchangeButtonLabel.color = this.m_SelectColor;
		}
	}

	private void HandleExchangeButtonOnExit(object sender, EventArgs e)
	{
		if (this.m_CanExchange)
		{
			this.m_ExchangeButtonLabel.color = this.m_DeselectColor;
		}
	}

	private void HandleExchangeButtonOnClick(object sender, EventArgs e)
	{
		if (this.m_CanExchange)
		{
			this.Exchange();
		}
	}

	private void HandleBackButtonOnEnter(object sender, EventArgs e)
	{
		this.m_BackButtonLabel.color = this.m_SelectColor;
	}

	private void HandleBackButtonOnExit(object sender, EventArgs e)
	{
		this.m_BackButtonLabel.color = this.m_DeselectColor;
	}

	private void HandleBackButtonOnClick(object sender, EventArgs e)
	{
		this.RemoveListeners();
		GameManager.Instance.ClearGentExchange();
		this.PlayOut();
	}

	public override void PlayIn()
	{
		this.m_IsActive = true;
		base.PlayIn();
	}

	public override void PlayOut()
	{
		this.m_IsActive = false;
		base.PlayOut();
	}

	private void Update()
	{
		if (!this.m_IsActive || !GameManager.Instance.HasController || base.IsDisposed)
		{
			return;
		}
		if (this.m_InputTimer >= this.m_InputDelay)
		{
			if (PlayerInput.Cancel())
			{
				this.Clear();
			}
			else if (this.CheckExchange())
			{
				this.Exchange();
			}
		}
		else
		{
			this.m_InputTimer += Time.unscaledDeltaTime;
		}
	}

	private bool CheckExchange()
	{
		if (this.m_CanExchange)
		{
			return PlayerInput.InteractOnReleased();
		}
		return false;
	}

	private void Clear()
	{
		GameManager.Instance.ClearGentExchange();
		this.PlayOut();
	}

	private void Exchange()
	{
		GameManager.Instance.ExchangeGentExchange();
		this.m_CardIconContaner.localScale = Vector3.one;
		this.m_CardIconContaner.DOKill();
		this.m_CardIconContaner.DOPunchScale(Vector3.one * 0.1f, 0.25f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		this.m_CardAmountLabel.text = "x" + GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards;
		this.SetRequirements();
		this.m_InputTimer = 0f;
		if (!this.m_CanExchange)
		{
			this.m_ExchangeButton.OnEnter -= this.HandleExchangeButtonOnEnter;
			this.m_ExchangeButton.OnExit -= this.HandleExchangeButtonOnExit;
			this.m_ExchangeButton.OnClick -= this.HandleExchangeButtonOnClick;
			this.m_ExchangeButton.interactable = false;
			this.m_ExchangeCanvasGroup.alpha = 0.2f;
		}
		this.OnExchanged.Send(this);
	}

	private void RemoveListeners()
	{
		this.m_ExchangeButton.OnEnter -= this.HandleExchangeButtonOnEnter;
		this.m_ExchangeButton.OnExit -= this.HandleExchangeButtonOnExit;
		this.m_ExchangeButton.OnClick -= this.HandleExchangeButtonOnClick;
		this.m_BackButton.OnEnter -= this.HandleBackButtonOnEnter;
		this.m_BackButton.OnExit -= this.HandleBackButtonOnExit;
		this.m_BackButton.OnClick -= this.HandleBackButtonOnClick;
	}

	protected override void OnDisposed()
	{
		this.OnExchanged = null;
		this.m_CardIconContaner.DOKill();
		this.RemoveListeners();
		base.OnDisposed();
	}
}
