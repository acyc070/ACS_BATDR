using System;
using System.Collections.Generic;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgrades : UIController
{
	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_TitleLabel;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLabel;

	[SerializeField]
	private RectTransform m_UpgradeElementContainer;

	[Header("Upgrade Buttons")]
	[SerializeField]
	private CanvasGroup m_UpgradeCanvasGroup;

	[SerializeField]
	private UIButton m_UpgradeButton;

	[SerializeField]
	private GameObject m_UpgradeButtonIcon;

	[SerializeField]
	private TextMeshProUGUI m_UpgradeButtonLabel;

	[SerializeField]
	private Image m_UpgradeButtonIconImage;

	[Header("Button Colors")]
	[SerializeField]
	private Color m_SelectColor;

	[SerializeField]
	private Color m_DeselectColor;

	private UIUpgradesDataVO m_DataVO;

	private List<UIElementUpgrade> m_Upgrades = new List<UIElementUpgrade>();

	private UpgradeType m_SelectedUpgradeType;

	private int m_InputIndex;

	private float m_InputDelay = 0.3f;

	private float m_InputTimer;

	private bool m_HasController;

	public event EventHandler OnUpgrade;

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UIUpgradesDataVO)_data;
		this.m_HasController = GameManager.Instance.HasController;
		this.m_TitleLabel.text = this.m_DataVO.TitleLabel;
		this.m_DescriptionLabel.text = this.m_DataVO.DescriptionLabel;
		this.m_UpgradeButtonLabel.text = TextUtility.GetKey("UI_UPGRADES_UPGRADE").ToUpper();
		unchecked
		{
			for (int i = 0; i < this.m_DataVO.Upgrades.Count; i++)
			{
				UIElementUpgradeDataVO data;
				data = this.m_DataVO.Upgrades[i];
				UIElementUpgrade uIElementUpgrade;
				uIElementUpgrade = GameManager.Instance.AssetManager.CreateAsset<UIElementUpgrade>("UI/Elements/UIElementUpgrade", data);
				uIElementUpgrade.transform.SetParent(this.m_UpgradeElementContainer);
				uIElementUpgrade.transform.localScale = Vector3.one;
				if (!this.m_Upgrades.Contains(uIElementUpgrade) && !uIElementUpgrade.IsFullydUpgraded)
				{
					uIElementUpgrade.OnSelected -= this.HandleUpgradeOnSelected;
					uIElementUpgrade.OnSelected += this.HandleUpgradeOnSelected;
					this.m_Upgrades.Add(uIElementUpgrade);
				}
			}
			UIManager.SetCursor(!this.m_HasController);
			this.m_UpgradeButtonIcon.SetActive(this.m_HasController);
			if (this.m_HasController)
			{
				this.m_UpgradeButtonIconImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action1);
			}
			this.RemoveListeners();
			if (!this.m_HasController)
			{
				this.m_UpgradeButton.OnEnter += this.HandleUpgradeButtonOnEnter;
				this.m_UpgradeButton.OnExit += this.HandleUpgradeButtonOnExit;
				this.m_UpgradeButton.OnClick += this.HandleUpgradeButtonOnClick;
				return;
			}
			UIElementUpgrade uIElementUpgrade2;
			uIElementUpgrade2 = this.m_Upgrades[this.m_InputIndex];
			for (int j = 0; j < this.m_Upgrades.Count; j++)
			{
				UIElementUpgrade uIElementUpgrade3;
				uIElementUpgrade3 = this.m_Upgrades[j];
				if (uIElementUpgrade3 != uIElementUpgrade2)
				{
					uIElementUpgrade3.Deselect();
				}
				else
				{
					uIElementUpgrade3.Select();
				}
			}
		}
	}

	private void HandleUpgradeOnSelected(object sender, EventArgs e)
	{
		UIElementUpgrade uIElementUpgrade;
		uIElementUpgrade = (UIElementUpgrade)sender;
		if (uIElementUpgrade.IsSelected)
		{
			return;
		}
		this.m_SelectedUpgradeType = uIElementUpgrade.UpgradeType;
		for (int i = 0; i < this.m_Upgrades.Count; i = unchecked(i + 1))
		{
			UIElementUpgrade uIElementUpgrade2;
			uIElementUpgrade2 = this.m_Upgrades[i];
			if (uIElementUpgrade2 != uIElementUpgrade)
			{
				uIElementUpgrade2.Deselect();
			}
		}
	}

	private void HandleUpgradeButtonOnEnter(object sender, EventArgs e)
	{
		this.m_UpgradeButtonLabel.color = this.m_SelectColor;
	}

	private void HandleUpgradeButtonOnExit(object sender, EventArgs e)
	{
		this.m_UpgradeButtonLabel.color = this.m_DeselectColor;
	}

	private void HandleUpgradeButtonOnClick(object sender, EventArgs e)
	{
		this.Upgrade();
	}

	private void Update()
	{
		if (!this.m_HasController || base.IsDisposed)
		{
			return;
		}
		if (PlayerInput.ControllerX() == 0f)
		{
			this.m_InputTimer = this.m_InputDelay;
		}
		unchecked
		{
			if (this.m_InputTimer >= this.m_InputDelay)
			{
				if (this.m_Upgrades.Count > 1)
				{
					bool flag;
					flag = false;
					if (PlayerInput.ControllerX() > 0.15f)
					{
						if (this.m_InputIndex >= this.m_Upgrades.Count - 1)
						{
							return;
						}
						this.m_InputTimer = 0f;
						flag = true;
						this.m_InputIndex++;
					}
					else if (PlayerInput.ControllerX() < -0.15f)
					{
						if (this.m_InputIndex <= 0)
						{
							return;
						}
						this.m_InputTimer = 0f;
						flag = true;
						this.m_InputIndex--;
					}
					if (flag)
					{
						UIElementUpgrade uIElementUpgrade;
						uIElementUpgrade = this.m_Upgrades[this.m_InputIndex];
						for (int i = 0; i < this.m_Upgrades.Count; i++)
						{
							UIElementUpgrade uIElementUpgrade2;
							uIElementUpgrade2 = this.m_Upgrades[i];
							if (uIElementUpgrade2 != uIElementUpgrade)
							{
								uIElementUpgrade2.Deselect();
								continue;
							}
							uIElementUpgrade2.Select();
							this.m_SelectedUpgradeType = uIElementUpgrade2.UpgradeType;
						}
						return;
					}
				}
				if (PlayerInput.ControllerAction1())
				{
					this.Upgrade();
				}
			}
			else
			{
				this.m_InputTimer += Time.unscaledDeltaTime;
			}
		}
	}

	private void Upgrade()
	{
		this.OnUpgrade.Send(this);
		this.RemoveListeners();
		UIManager.SetCursor(active: false);
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_UPGRADE"));
		if (this.m_SelectedUpgradeType == UpgradeType.Health)
		{
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeHealth();
			GameManager.Instance.Player.SetHealth(unchecked((int)UpgradeCheck.GetHealth()));
		}
		else if (this.m_SelectedUpgradeType == UpgradeType.Stamina)
		{
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeStamina();
			GameManager.Instance.Player.ResetRunTimer();
		}
		else if (this.m_SelectedUpgradeType == UpgradeType.Ability)
		{
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeAbility();
			if (GameManager.Instance.Player.Abilities != null && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active)
			{
				foreach (PlayerAbilityState ability in GameManager.Instance.Player.Abilities)
				{
					ability?.ForceSetCooldown(ability._CooldownCount);
				}
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelHealth >= 3 && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelStamina >= 3 && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelAbility >= 3)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_INK_PROVIDES);
		}
		GameManager.Instance.HideUpgrades();
	}

	private void RemoveListeners()
	{
		this.m_UpgradeButton.OnEnter -= this.HandleUpgradeButtonOnEnter;
		this.m_UpgradeButton.OnExit -= this.HandleUpgradeButtonOnExit;
		this.m_UpgradeButton.OnClick -= this.HandleUpgradeButtonOnClick;
	}

	protected override void OnDisposed()
	{
		this.OnUpgrade = null;
		this.RemoveListeners();
		this.m_DataVO = null;
		if (this.m_Upgrades != null)
		{
			this.m_Upgrades.Clear();
			this.m_Upgrades = null;
		}
		base.OnDisposed();
	}
}
