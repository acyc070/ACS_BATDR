using System;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGentUpgrades : UIController
{
	[Header("Gent Upgrade")]
	[SerializeField]
	private TextMeshProUGUI m_UpgradeTitleLabel;

	[SerializeField]
	private TextMeshProUGUI m_UpgradeDescriptionLabel;

	[Header("Schematic")]
	[SerializeField]
	private GameObject m_SchematicReady;

	[SerializeField]
	private Image m_Icon;

	[SerializeField]
	private TextMeshProUGUI m_SchematicTitleLabel;

	[SerializeField]
	private TextMeshProUGUI m_SchematicDescriptionLabel;

	[SerializeField]
	private Sprite m_Icon_LV2;

	[SerializeField]
	private Sprite m_Icon_LV3;

	[SerializeField]
	private Sprite m_Icon_LV4;

	[Header("Requirements")]
	[SerializeField]
	private TextMeshProUGUI m_RequirementsTitleLabel;

	[SerializeField]
	private RectTransform m_ComponentsContainer;

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

	private bool m_CanUpgrade;

	protected override void OnInitialized(object _data)
	{
		this.RemoveListeners();
		this.SetSchematics();
		this.SetRequirements();
		this.m_UpgradeTitleLabel.text = TextUtility.GetKey("UI_GENT_UPGRADES_TITLE").ToUpper();
		this.m_UpgradeDescriptionLabel.text = TextUtility.GetKey("UI_GENT_UPGRADES_DESCRIPTION").GetLinebreaks().ToUpper();
		this.m_RequirementsTitleLabel.text = TextUtility.GetKey("UI_GENT_UPGRADES_REQUIRED").ToUpper();
		this.m_SchematicReady.SetActive(this.m_CanUpgrade);
		this.SetButtons(GameManager.Instance.HasController);
	}

	private void SetSchematics()
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID == WeaponType.LEVEL_1)
		{
			this.SetSchematics("UI_GENT_UPGRADES_GENT_DOOR", "UI_GENT_UPGRADES_GENT_DOOR_MESSAGE", this.m_Icon_LV2);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID == WeaponType.LEVEL_2)
		{
			this.SetSchematics("UI_GENT_UPGRADES_GENT_IMPACT", "UI_GENT_UPGRADES_GENT_IMPACT_MESSAGE", this.m_Icon_LV3);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID == WeaponType.LEVEL_3)
		{
			this.SetSchematics("UI_GENT_UPGRADES_GENT_STUN", "UI_GENT_UPGRADES_GENT_STUN_MESSAGE", this.m_Icon_LV4);
		}
	}

	private void SetSchematics(string title, string description, Sprite sprite)
	{
		this.m_Icon.sprite = sprite;
		this.m_SchematicTitleLabel.text = TextUtility.GetKey(title).ConvertAll().ToUpper();
		this.m_SchematicDescriptionLabel.text = TextUtility.GetKey(description).ConvertAll().ToUpper();
	}

	private void SetRequirements()
	{
		this.m_CanUpgrade = true;
		WeaponType iD;
		iD = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
		switch (iD)
		{
		case WeaponType.LEVEL_1:
			this.SetComponent("Icon/Collectables/Large/UIIcon_BatteryCasing_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.BatteryCasings, GentUpgradesRequirements.GetBatteryCasings(iD));
			this.SetComponent("Icon/Collectables/Large/UIIcon_Toolkit_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Toolkits, GentUpgradesRequirements.GetToolkits(iD));
			this.SetComponent("Icon/Collectables/Large/UIIcon_Parts_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Parts, GentUpgradesRequirements.GetParts(iD));
			break;
		case WeaponType.LEVEL_2:
			this.SetComponent("Icon/Collectables/Large/UIIcon_Battery_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries, GentUpgradesRequirements.GetBatteries(iD));
			this.SetComponent("Icon/Collectables/Large/UIIcon_Toolkit_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Toolkits, GentUpgradesRequirements.GetToolkits(iD));
			this.SetComponent("Icon/Collectables/Large/UIIcon_Parts_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Parts, GentUpgradesRequirements.GetParts(iD));
			break;
		case WeaponType.LEVEL_3:
			this.SetComponent("Icon/Collectables/Large/UIIcon_BatteryCasing_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.BatteryCasings, GentUpgradesRequirements.GetBatteryCasings(iD));
			this.SetComponent("Icon/Collectables/Large/UIIcon_Battery_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries, GentUpgradesRequirements.GetBatteries(iD));
			this.SetComponent("Icon/Collectables/Large/UIIcon_Toolkit_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Toolkits, GentUpgradesRequirements.GetToolkits(iD));
			this.SetComponent("Icon/Collectables/Large/UIIcon_Parts_Large", GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Parts, GentUpgradesRequirements.GetParts(iD));
			break;
		}
	}

	private void SetComponent(string icon, int amount, int max)
	{
		if (amount >= max)
		{
			amount = max;
		}
		else
		{
			this.m_CanUpgrade = false;
		}
		_ = (UIElementGentUpgradeComponent)UIManager.CreateElement(_data: new UIElementGentUpgradeComponentDataVO(icon, amount, max), parent: this.m_ComponentsContainer, _prefabKey: "UI/Elements/UIElementGentUpgradeComponent");
	}

	private void SetButtons(bool hasController)
	{
		if (!hasController)
		{
			this.m_UpgradeButtonLabel.alignment = TextAlignmentOptions.Midline;
			this.m_BackButtonLabel.alignment = TextAlignmentOptions.Midline;
		}
		this.m_UpgradeButtonIcon.SetActive(hasController);
		this.m_UpgradeButtonIconImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action3);
		if (this.m_CanUpgrade)
		{
			this.m_UpgradeCanvasGroup.alpha = 1f;
			if (!hasController)
			{
				this.m_UpgradeButton.OnEnter += this.HandleUpgradeButtonOnEnter;
				this.m_UpgradeButton.OnExit += this.HandleUpgradeButtonOnExit;
				this.m_UpgradeButton.OnClick += this.HandleUpgradeButtonOnClick;
			}
		}
		else
		{
			this.m_UpgradeCanvasGroup.alpha = 0.2f;
		}
		this.m_UpgradeButtonLabel.text = TextUtility.GetKey("UI_UPGRADES_UPGRADE").ToUpper();
		this.m_UpgradeButtonLabel.ForceMeshUpdate();
		Vector2 sizeDelta;
		sizeDelta = this.m_UpgradeButtonLabel.rectTransform.sizeDelta;
		sizeDelta.x = this.m_UpgradeButtonLabel.GetRenderedValues().x;
		this.m_UpgradeButtonLabel.rectTransform.sizeDelta = sizeDelta;
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
		this.Clear();
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
			else if (this.CheckUpgrade())
			{
				this.Upgrade();
			}
		}
		else
		{
			this.m_InputTimer += Time.unscaledDeltaTime;
		}
	}

	private bool CheckUpgrade()
	{
		if (this.m_CanUpgrade)
		{
			return PlayerInput.InteractOnReleased();
		}
		return false;
	}

	private void Clear()
	{
		this.RemoveListeners();
		GameManager.Instance.ClearGentUpgrades();
		this.PlayOut();
	}

	private void Upgrade()
	{
		this.RemoveListeners();
		GameManager.Instance.UpgradeGentUpgrade();
		this.PlayOut();
	}

	private void RemoveListeners()
	{
		this.m_UpgradeButton.OnEnter -= this.HandleUpgradeButtonOnEnter;
		this.m_UpgradeButton.OnExit -= this.HandleUpgradeButtonOnExit;
		this.m_UpgradeButton.OnClick -= this.HandleUpgradeButtonOnClick;
		this.m_BackButton.OnEnter -= this.HandleBackButtonOnEnter;
		this.m_BackButton.OnExit -= this.HandleBackButtonOnExit;
		this.m_BackButton.OnClick -= this.HandleBackButtonOnClick;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
