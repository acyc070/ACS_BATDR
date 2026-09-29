using System;
using System.Collections.Generic;
using DG.Tweening;
using InControl;
using S13Audio.BATDR;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameMenu : UIController
{
	[Serializable]
	public class UIModelGroup
	{
		public string Name;

		public GameObject Container;

		public Animator Animator;

		public GameObject UIModelImage;

		public void SetActive(bool active)
		{
			this.Container.SetActive(active);
			this.UIModelImage.SetActive(active);
		}
	}

	[Header("Disolves")]
	[SerializeField]
	private Image[] m_Disolves;

	[Header("Background")]
	[SerializeField]
	private Image m_BackgroundImage;

	[SerializeField]
	private Sprite m_BackgroundInk;

	[SerializeField]
	private Sprite m_BackgroundReal;

	[Header("UI Elements")]
	[SerializeField]
	private Color m_ColorDefaultReal;

	[SerializeField]
	private Color m_ColorDefaultSelectReal;

	[SerializeField]
	private Color m_ColorDefaultInk;

	[SerializeField]
	private Color m_ColorDefaultSelectInk;

	[Space]
	[SerializeField]
	private Image[] m_UIDecor;

	[Space]
	[Header("Status Glow")]
	[SerializeField]
	private Image m_StatusGlow;

	[SerializeField]
	private Color m_StatusGlowColorReal;

	[SerializeField]
	private Color m_StatusGlowColorInk;

	[Header("Navigation Content")]
	[SerializeField]
	private RectTransform m_NavigationElementContainer;

	[SerializeField]
	private RectTransform m_NavLeft;

	[SerializeField]
	private RectTransform m_NavRight;

	[SerializeField]
	private Image m_NavLeftIcon;

	[SerializeField]
	private Image m_NavLeftInput;

	[SerializeField]
	private Image m_NavRightIcon;

	[SerializeField]
	private Image m_NavRightInput;

	[Header("Container Content")]
	[SerializeField]
	private RectTransform m_InfoScrollableContent;

	[SerializeField]
	private RectTransform m_ContentScrollableContent;

	[Header("Status")]
	[SerializeField]
	private RectTransform m_StatusContainer;

	[SerializeField]
	private RectTransform m_UpgradeContainer;

	[SerializeField]
	private RectTransform m_WeaponContainer;

	[SerializeField]
	private RectTransform m_WeaponContentContainer;

	[SerializeField]
	private RectTransform m_AbilityContainer;

	[SerializeField]
	private RectTransform m_AbilityContentContainer;

	[SerializeField]
	private RectTransform m_HealthContainer;

	[SerializeField]
	private RectTransform m_ResourceContainer;

	[Header("Scroll Views")]
	[SerializeField]
	private UIScrollRect m_InfoScrollView;

	[SerializeField]
	private UIScrollRect m_ContentScrollView;

	[Header("Weapon Content")]
	[SerializeField]
	private Transform m_HeldWeaponParent;

	[SerializeField]
	private Transform m_WeaponParent;

	[Header("Model Rendering")]
	[SerializeField]
	private GameObject m_Camera;

	[SerializeField]
	private List<UIModelGroup> m_UIModelGroups = new List<UIModelGroup>();

	[Space]
	[Header("S13 - Audio Content")]
	[SerializeField]
	private Transform m_AudioParent;

	[SerializeField]
	private GameObject m_AudioControllerReal;

	[SerializeField]
	private GameObject m_AudioControllerInk;

	[SerializeField]
	private GameObject m_AudioControllerBeast;

	[Space]
	[SerializeField]
	private BATDRAudioLogPlayer m_AudioLogPlayer;

	private float m_InputTimer;

	private float m_InputTimerLimit = 0.25f;

	private int m_NavIndex;

	private List<UIElementGameMenuButton> m_NavButtons = new List<UIElementGameMenuButton>();

	private UIElementGameMenuButton m_ActiveNav;

	private int m_SelectionIndex;

	private int m_SubSelectionIndex;

	private List<UIElementMemoButtonLabel> m_Memos = new List<UIElementMemoButtonLabel>();

	private List<UIElementAudioLogButtonLabel> m_AudioLogs = new List<UIElementAudioLogButtonLabel>();

	private List<UIElementMemoryButtonLabel> m_Memories = new List<UIElementMemoryButtonLabel>();

	private List<UIElementGameMenuSettingsButton> m_Settings = new List<UIElementGameMenuSettingsButton>();

	private UIElementGameMenuSettingsButton m_ActiveSettingsButton;

	private List<UIElement> m_SubSettings = new List<UIElement>();

	private UIElement m_ActiveSubSettings;

	private List<UIElementGameMenuMenuButton> m_Menus = new List<UIElementGameMenuMenuButton>();

	private UIElementGameMenuMenuButton m_ActiveMenuButton;

	private List<UIElement> m_SubMenus = new List<UIElement>();

	private UIElement m_ActiveSubMenu;

	private UIGameMenuDataVO m_DataVO;

	private Sequence m_DisolveSequence;

	private UIPrompt m_MenuExitPrompt;

	private bool m_IsPromptActive;

	public bool IsActive { get; private set; }

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UIGameMenuDataVO)_data;
		this.IsActive = false;
		Image[] disolves;
		disolves = this.m_Disolves;
		unchecked
		{
			for (int i = 0; i < disolves.Length; i++)
			{
				disolves[i].color = new Color(1f, 1f, 1f, 0f);
			}
			this.m_BackgroundImage.sprite = (GameManager.Instance.IsRealWorld ? this.m_BackgroundReal : this.m_BackgroundInk);
			this.m_BackgroundImage.color = new Color(1f, 1f, 1f, 1f);
			if (GameManager.Instance.BeastBendy == null)
			{
				if (GameManager.Instance.IsRealWorld)
				{
					global::UnityEngine.Object.Instantiate(this.m_AudioControllerReal, this.m_AudioParent, worldPositionStays: false);
				}
				else
				{
					global::UnityEngine.Object.Instantiate(this.m_AudioControllerInk, this.m_AudioParent, worldPositionStays: false);
				}
			}
			else
			{
				global::UnityEngine.Object.Instantiate(this.m_AudioControllerBeast, this.m_AudioParent, worldPositionStays: false);
			}
			disolves = this.m_UIDecor;
			for (int i = 0; i < disolves.Length; i++)
			{
				disolves[i].color = (GameManager.Instance.IsRealWorld ? this.m_ColorDefaultReal : this.m_ColorDefaultInk);
			}
			this.m_StatusGlow.color = (GameManager.Instance.IsRealWorld ? this.m_StatusGlowColorReal : this.m_StatusGlowColorInk);
			bool flag;
			flag = true;
			List<UIElementDataVO>.Enumerator enumerator;
			enumerator = this.m_DataVO.NavigationUIElements.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current.PrefabKey == string.Empty) && UIManager.CreateElement(this.m_NavigationElementContainer, enumerator.Current.PrefabKey, enumerator.Current) is UIElementGameMenuButton uIElementGameMenuButton)
				{
					uIElementGameMenuButton.OnClick -= this.HandleButtonOnClick;
					uIElementGameMenuButton.OnClick += this.HandleButtonOnClick;
					uIElementGameMenuButton.Button.selected = flag;
					if (flag)
					{
						uIElementGameMenuButton.ForceOnEnter();
						this.m_ActiveNav = uIElementGameMenuButton;
						flag = false;
					}
					if (!this.m_NavButtons.Contains(uIElementGameMenuButton))
					{
						this.m_NavButtons.Add(uIElementGameMenuButton);
					}
				}
			}
			if (GameManager.Instance.HasController)
			{
				this.m_NavLeft.gameObject.SetActive(value: true);
				this.m_NavLeftIcon.color = (GameManager.Instance.IsRealWorld ? UIColors.INACTIVE_LABEL_REAL : UIColors.INACTIVE_LABEL);
				this.m_NavLeftInput.color = (GameManager.Instance.IsRealWorld ? UIColors.ACTIVE_LABEL_REAL : UIColors.ACTIVE_LABEL);
				this.m_NavLeftInput.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.LeftBumper);
				this.m_NavLeft.SetAsFirstSibling();
				this.m_NavRight.gameObject.SetActive(value: true);
				this.m_NavRightIcon.color = (GameManager.Instance.IsRealWorld ? UIColors.INACTIVE_LABEL_REAL : UIColors.INACTIVE_LABEL);
				this.m_NavRightInput.color = (GameManager.Instance.IsRealWorld ? UIColors.ACTIVE_LABEL_REAL : UIColors.ACTIVE_LABEL);
				this.m_NavRightInput.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.RightBumper);
				this.m_NavRight.SetAsLastSibling();
			}
			else
			{
				this.m_NavLeft.gameObject.SetActive(value: false);
				this.m_NavRight.gameObject.SetActive(value: false);
			}
			this.m_Camera.SetActive(value: false);
			foreach (UIModelGroup uIModelGroup in this.m_UIModelGroups)
			{
				uIModelGroup.SetActive(active: false);
			}
			this.m_WeaponContainer.gameObject.SetActive(value: false);
			this.m_AbilityContainer.gameObject.SetActive(value: false);
			UIElement[] componentsInChildren;
			componentsInChildren = this.m_InfoScrollableContent.GetComponentsInChildren<UIElement>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Dispose();
			}
			if (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID != WeaponType.NONE)
			{
				WeaponType iD;
				iD = GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID;
				string assetKey;
				assetKey = "";
				switch (iD)
				{
				case WeaponType.LEVEL_1:
					assetKey = "Weapons/Weapon_GentPipe_01";
					break;
				case WeaponType.LEVEL_2:
					assetKey = "Weapons/Weapon_GentPipe_02";
					break;
				case WeaponType.LEVEL_3:
					assetKey = "Weapons/Weapon_GentPipe_03";
					break;
				case WeaponType.LEVEL_4:
					assetKey = "Weapons/Weapon_GentPipe_04";
					break;
				}
				Transform transform;
				transform = GameManager.Instance.AssetManager.CreateAsset<Transform>(assetKey);
				MeshRenderer[] componentsInChildren2;
				componentsInChildren2 = transform.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				foreach (MeshRenderer obj in componentsInChildren2)
				{
					obj.gameObject.layer = LayerMask.NameToLayer("UIModel");
					obj.shadowCastingMode = ShadowCastingMode.On;
				}
				this.InitWeaponParent(weapon: global::UnityEngine.Object.Instantiate(transform).gameObject, weaponParent: this.m_WeaponParent);
				if (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.WeaponStatus == WeaponStatus.Active)
				{
					this.InitWeaponParent(this.m_HeldWeaponParent, transform.gameObject);
				}
				else
				{
					global::UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			this.ShowStatus();
			this.m_InfoScrollView.verticalScrollbar.value = 1f;
			this.m_InfoScrollView.verticalScrollbar.size = 0f;
		}
	}

	public override void PlayIn()
	{
		this.m_DisolveSequence = DOTween.Sequence().SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		Image[] disolves;
		disolves = this.m_Disolves;
		for (int i = 0; i < disolves.Length; i = unchecked(i + 1))
		{
			TweenSettingsExtensions.Insert(t: disolves[i].DOFade(1f, 0.4f).SetEase(Ease.InOutSine), s: this.m_DisolveSequence, atPosition: 0f);
		}
		this.m_DisolveSequence.OnComplete(this.PlayInComplete);
	}

	public override void PlayInComplete()
	{
		this.IsActive = true;
		UIManager.SetCursor(!GameManager.Instance.HasController);
		base.PlayInComplete();
	}

	public override void PlayOut()
	{
		this.IsActive = false;
		UIManager.SetCursor(active: false);
		this.m_AudioParent.gameObject.SetActive(value: false);
		this.m_DisolveSequence?.Kill();
		this.m_DisolveSequence = DOTween.Sequence().SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		Image[] disolves;
		disolves = this.m_Disolves;
		for (int i = 0; i < disolves.Length; i = unchecked(i + 1))
		{
			TweenSettingsExtensions.Insert(t: disolves[i].DOFade(0f, 0.4f).SetEase(Ease.OutSine), s: this.m_DisolveSequence, atPosition: 0f);
		}
		this.m_DisolveSequence.OnComplete(this.PlayOutComplete);
	}

	private void InitWeaponParent(Transform weaponParent, GameObject weapon)
	{
		weapon.transform.SetParent(weaponParent);
		weapon.transform.localPosition = Vector3.zero;
		weapon.transform.localEulerAngles = Vector3.zero;
		weapon.transform.localScale = Vector3.one;
		GentPipePowerIndicator componentInChildren;
		componentInChildren = weapon.GetComponentInChildren<GentPipePowerIndicator>(includeInactive: true);
		if (componentInChildren != null)
		{
			componentInChildren.SetPowerLevel(GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power);
		}
		if (GameManager.Instance.Player.CurrentWeapon == null)
		{
			return;
		}
		GentPipeChargeEffects componentInChildren2;
		componentInChildren2 = weapon.GetComponentInChildren<GentPipeChargeEffects>(includeInactive: true);
		if (componentInChildren2 != null && GameManager.Instance.Player.CurrentWeapon.IsCharged)
		{
			componentInChildren2.ChargeLevel.gameObject.SetActive(value: true);
			GentPipeChargeEffects componentInChildren3;
			componentInChildren3 = GameManager.Instance.Player.CurrentWeapon.Weapon.GetComponentInChildren<GentPipeChargeEffects>(includeInactive: true);
			if (componentInChildren3 != null)
			{
				componentInChildren2.ChargeLevel.localScale = componentInChildren3.ChargeLevel.localScale;
			}
		}
	}

	private void Update()
	{
		if (!this.IsActive || this.m_IsPromptActive || !GameManager.Instance.HasController || base.IsDisposed)
		{
			return;
		}
		unchecked
		{
			if (PlayerInput.ControllerLeftBumper())
			{
				if (this.m_NavIndex > 0)
				{
					this.m_NavIndex--;
					this.ShowSelection(this.m_NavButtons[this.m_NavIndex], isController: true);
				}
			}
			else if (PlayerInput.ControllerRightBumper() && this.m_NavIndex < this.m_NavButtons.Count - 1)
			{
				this.m_NavIndex++;
				this.ShowSelection(this.m_NavButtons[this.m_NavIndex], isController: true);
			}
			float num;
			num = PlayerInput.ControllerX();
			float num2;
			num2 = PlayerInput.ControllerY();
			bool flag;
			flag = PlayerInput.ControllerAction1();
			bool cancel;
			cancel = PlayerInput.ControllerAction2();
			if (num2 == 0f && num == 0f)
			{
				this.m_InputTimer = this.m_InputTimerLimit;
			}
			if (this.m_InputTimer >= this.m_InputTimerLimit)
			{
				if (this.m_ActiveNav.GameMenuType == GameMenuType.Status)
				{
					if (!this.m_InfoScrollView.verticalScrollbar.gameObject.activeInHierarchy)
					{
						return;
					}
					float num3;
					num3 = 0f;
					if (num2 > 0f)
					{
						if (this.m_InfoScrollView.verticalScrollbar.value < 1f)
						{
							num3 += 0.25f;
						}
					}
					else if (num2 < 0f && this.m_InfoScrollView.verticalScrollbar.value > 0f)
					{
						num3 -= 0.25f;
					}
					num3 += this.m_InfoScrollView.verticalScrollbar.value;
					if (num3 != this.m_InfoScrollView.verticalScrollbar.value)
					{
						if (num3 < 0f)
						{
							num3 = 0f;
						}
						else if (num3 > 1f)
						{
							num3 = 1f;
						}
						this.m_InfoScrollView.DOKill();
						this.m_InfoScrollView.DOVerticalNormalizedPos(num3, 0.1f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
						this.m_InputTimer = 1.5f;
					}
				}
				else
				{
					this.GetSelection(num2, num, flag, cancel);
					if (num2 != 0f || num != 0f)
					{
						this.m_InputTimer = 0f;
					}
				}
			}
			else
			{
				this.m_InputTimer += Time.unscaledDeltaTime;
			}
		}
	}

	private void GetSelection(float inputY, float inputX, bool select, bool cancel)
	{
		if (this.m_ActiveNav.GameMenuType == GameMenuType.Memos)
		{
			if (this.GetSelection(inputY, inputX, select, cancel, this.m_Memos.Count))
			{
				this.ShowMemoSelection(this.m_Memos[this.m_SelectionIndex]);
			}
		}
		else if (this.m_ActiveNav.GameMenuType == GameMenuType.AudioLogs)
		{
			if (this.GetSelection(inputY, inputX, select, cancel, this.m_AudioLogs.Count))
			{
				this.ShowAudioLogSelection(this.m_AudioLogs[this.m_SelectionIndex]);
			}
		}
		else if (this.m_ActiveNav.GameMenuType == GameMenuType.Memories)
		{
			if (this.GetSelection(inputY, inputX, select, cancel, this.m_Memories.Count))
			{
				this.ShowMemorySelection(this.m_Memories[this.m_SelectionIndex]);
			}
		}
		else if (this.m_ActiveNav.GameMenuType == GameMenuType.Settings)
		{
			if (this.m_ActiveSubSettings != null)
			{
				this.GetSettingsSubMenu(inputY, inputX, select, cancel);
			}
			else if (this.GetSelection(inputY, inputX, select, cancel, this.m_Settings.Count))
			{
				this.GetSettingsSelection(this.m_Settings[this.m_SelectionIndex]);
			}
		}
		else if (this.m_ActiveNav.GameMenuType == GameMenuType.Menu)
		{
			if (this.m_ActiveSubMenu != null)
			{
				this.GetMenuSubMenu(inputY, inputX, select, cancel);
			}
			else if (this.GetSelection(inputY, inputX, select, cancel, this.m_Menus.Count))
			{
				this.GetMenuSelection(this.m_Menus[this.m_SelectionIndex]);
			}
		}
	}

	private bool GetSelection(float inputY, float inputX, bool select, bool cancel, int count)
	{
		bool result;
		result = false;
		unchecked
		{
			if (!cancel)
			{
				if (select)
				{
					if (this.m_ActiveNav.GameMenuType != GameMenuType.Memos)
					{
						if (this.m_ActiveNav.GameMenuType == GameMenuType.AudioLogs)
						{
							this.PlayAudioLog(this.m_AudioLogs[this.m_SelectionIndex]);
						}
						else if (this.m_ActiveNav.GameMenuType != GameMenuType.Memories)
						{
							if (this.m_ActiveNav.GameMenuType == GameMenuType.Settings)
							{
								this.ShowSettingsSelection(this.m_Settings[this.m_SelectionIndex]);
								this.m_SubSelectionIndex = 0;
								this.ShowSettingsSubSelection(this.m_SubSettings[this.m_SubSelectionIndex]);
							}
							else if (this.m_ActiveNav.GameMenuType == GameMenuType.Menu)
							{
								if (this.m_ActiveMenuButton.MenuType == GameMenuMenuType.MainMenu)
								{
									this.ShowMenuExit();
								}
								else if (this.m_ActiveMenuButton.MenuType == GameMenuMenuType.CannotSaveGame || this.m_ActiveMenuButton.MenuType == GameMenuMenuType.CannotLoadGame)
								{
									this.m_SubSelectionIndex = 0;
								}
								else
								{
									this.ShowMenuSelection(this.m_Menus[this.m_SelectionIndex]);
									this.m_SubSelectionIndex = 0;
									this.ShowMenuSubSelection(this.m_SubMenus[this.m_SubSelectionIndex]);
								}
							}
						}
					}
				}
				else if (inputY > 0f)
				{
					if (this.m_SelectionIndex > 0)
					{
						this.m_SelectionIndex--;
						if (this.m_SelectionIndex <= 0)
						{
							this.m_SelectionIndex = 0;
						}
						result = true;
					}
				}
				else if (inputY < 0f && this.m_SelectionIndex < count - 1)
				{
					this.m_SelectionIndex++;
					if (this.m_SelectionIndex >= count)
					{
						this.m_SelectionIndex = count - 1;
					}
					result = true;
				}
			}
			return result;
		}
	}

	private void ShowStatus()
	{
		JDebug.Log("GAME MENU: Status", this, JDebug.JDebugType.UI);
		if (GameManager.Instance.HasController)
		{
			this.m_NavLeft.gameObject.SetActive(value: false);
		}
		if (GameManager.Instance.BeastBendy != null)
		{
			UIModelGroup uIModelGroup;
			uIModelGroup = this.m_UIModelGroups.Find((UIModelGroup x) => x.Name.ToLower() == "status_beast");
			if (uIModelGroup != null)
			{
				this.m_Camera.SetActive(value: true);
				uIModelGroup.SetActive(active: true);
			}
		}
		else if (GameManager.Instance.IsRealWorld)
		{
			UIModelGroup uIModelGroup2;
			uIModelGroup2 = this.m_UIModelGroups.Find((UIModelGroup x) => x.Name.ToLower() == "status_real");
			if (uIModelGroup2 != null)
			{
				this.m_Camera.SetActive(value: true);
				uIModelGroup2.SetActive(active: true);
			}
		}
		else
		{
			UIModelGroup uIModelGroup3;
			uIModelGroup3 = this.m_UIModelGroups.Find((UIModelGroup x) => x.Name.ToLower() == "status_ink");
			if (uIModelGroup3 != null)
			{
				this.m_Camera.SetActive(value: true);
				uIModelGroup3.SetActive(active: true);
				if (uIModelGroup3.Animator != null)
				{
					uIModelGroup3.Animator.SetBool("HasWeapon", GameManager.Instance.Player.CurrentWeapon != null);
					uIModelGroup3.Animator.SetBool("HasAbility", GameManager.Instance.Player.Abilities != null && GameManager.Instance.Player.Abilities.Count > 0);
					if (SeasonalCheck.IsSeasonal(out var seasonalType) && SeasonalCheck.HasHat)
					{
						switch (seasonalType)
						{
						case SeasonalType.WinterAbyss:
						{
							GameObject gameObject2;
							gameObject2 = (GameObject)global::UnityEngine.Object.Instantiate(Resources.Load("Hats/Prefab_Seasonal_Winter_Hat_01"));
							foreach (Transform item in gameObject2.transform)
							{
								item.gameObject.layer = LayerMask.NameToLayer("UIModel");
							}
							Transform transform2;
							transform2 = uIModelGroup3.Animator.transform.FindDeepChild("WinterHat");
							if (transform2 != null)
							{
								gameObject2.transform.SetParent(transform2);
								gameObject2.transform.localScale = Vector3.one;
								gameObject2.transform.localPosition = Vector3.zero;
								gameObject2.transform.localEulerAngles = Vector3.zero;
							}
							break;
						}
						case SeasonalType.InkDemonsEve:
						{
							GameObject gameObject;
							gameObject = (GameObject)global::UnityEngine.Object.Instantiate(Resources.Load("Hats/Prefab_Seasonal_Halloween_Hat_01_UIModel"));
							foreach (Transform item2 in gameObject.transform)
							{
								item2.gameObject.layer = LayerMask.NameToLayer("UIModel");
							}
							Transform transform;
							transform = uIModelGroup3.Animator.transform.FindDeepChild("HalloweenHat");
							if (transform != null)
							{
								gameObject.transform.SetParent(transform);
								gameObject.transform.localScale = Vector3.one;
								gameObject.transform.localPosition = Vector3.zero;
								gameObject.transform.localEulerAngles = Vector3.zero;
							}
							break;
						}
						}
					}
				}
			}
		}
		this.m_StatusContainer.gameObject.SetActive(value: true);
		List<UIElementDataVO>.Enumerator enumerator2;
		enumerator2 = UILayouts.GetStatus(this.m_DataVO.objectivelist).GetEnumerator();
		while (enumerator2.MoveNext())
		{
			UIElementDataVO current;
			current = enumerator2.Current;
			UIManager.CreateElement(this.m_InfoScrollableContent, current.PrefabKey, current);
		}
		if (GameManager.Instance.BeastBendy == null && !GameManager.Instance.IsRealWorld)
		{
			this.ShowResources();
			this.ShowUpgrades();
			this.ShowHealth();
		}
	}

	private void ShowResources()
	{
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = UILayouts.GetResources().GetEnumerator();
		while (enumerator.MoveNext())
		{
			UIElementDataVO current;
			current = enumerator.Current;
			UIManager.CreateElement(this.m_ResourceContainer, current.PrefabKey, current);
		}
	}

	private void ShowUpgrades()
	{
		this.m_UpgradeContainer.gameObject.SetActive(value: true);
		int num;
		num = 0;
		switch (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID)
		{
		case WeaponType.LEVEL_2:
			num = 1;
			break;
		case WeaponType.LEVEL_3:
			num = 2;
			break;
		case WeaponType.LEVEL_4:
			num = 3;
			break;
		}
		if (num > 0)
		{
			UIManager.CreateElement(this.m_UpgradeContainer, "UI/Elements/UIElementUpgradesStatus", new UIElementUpgradeStatusDataVO(TextUtility.GetKey("UI_UPGRADES_GENT_PIPE"), num));
		}
		int upgradeLevelHealth;
		upgradeLevelHealth = GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelHealth;
		if (upgradeLevelHealth > 0)
		{
			UIManager.CreateElement(this.m_UpgradeContainer, "UI/Elements/UIElementUpgradesStatus", new UIElementUpgradeStatusDataVO(TextUtility.GetKey("UI_UPGRADES_HEALTH"), upgradeLevelHealth));
		}
		int upgradeLevelStamina;
		upgradeLevelStamina = GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelStamina;
		if (upgradeLevelStamina > 0)
		{
			UIManager.CreateElement(this.m_UpgradeContainer, "UI/Elements/UIElementUpgradesStatus", new UIElementUpgradeStatusDataVO(TextUtility.GetKey("UI_UPGRADES_STAMINA"), upgradeLevelStamina));
		}
		int upgradeLevelAbility;
		upgradeLevelAbility = GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelAbility;
		if (upgradeLevelAbility > 0)
		{
			UIManager.CreateElement(this.m_UpgradeContainer, "UI/Elements/UIElementUpgradesStatus", new UIElementUpgradeStatusDataVO(TextUtility.GetKey("UI_UPGRADES_ABILITY"), upgradeLevelAbility));
		}
	}

	private void ShowWeaponUpgrade()
	{
		this.ClearUpgradeContent();
		int num;
		num = 0;
		switch (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID)
		{
		case WeaponType.LEVEL_2:
			num = 1;
			break;
		case WeaponType.LEVEL_3:
			num = 2;
			break;
		case WeaponType.LEVEL_4:
			num = 3;
			break;
		}
		if (num > 0)
		{
			this.m_StatusContainer.gameObject.SetActive(value: true);
			this.m_UpgradeContainer.gameObject.SetActive(value: true);
			UIManager.CreateElement(this.m_UpgradeContainer, "UI/Elements/UIElementUpgradesStatus", new UIElementUpgradeStatusDataVO(TextUtility.GetKey("UI_UPGRADES_GENT_PIPE"), num));
		}
	}

	private void ShowAbilityUpgrade()
	{
		this.ClearUpgradeContent();
		int upgradeLevelAbility;
		upgradeLevelAbility = GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelAbility;
		if (upgradeLevelAbility > 0)
		{
			this.m_StatusContainer.gameObject.SetActive(value: true);
			this.m_UpgradeContainer.gameObject.SetActive(value: true);
			UIManager.CreateElement(this.m_UpgradeContainer, "UI/Elements/UIElementUpgradesStatus", new UIElementUpgradeStatusDataVO(TextUtility.GetKey("UI_UPGRADES_ABILITY"), upgradeLevelAbility));
		}
	}

	private void ShowHealth()
	{
		this.m_HealthContainer.gameObject.SetActive(value: true);
		UIManager.CreateElement(this.m_HealthContainer, "UI/Elements/UIElementHealthStatus");
	}

	private void ShowWeapons()
	{
		JDebug.Log("GAME MENU: Weapon", this, JDebug.JDebugType.UI);
		UIModelGroup uIModelGroup;
		uIModelGroup = this.m_UIModelGroups.Find((UIModelGroup x) => x.Name.ToLower() == "weapon");
		if (uIModelGroup != null)
		{
			this.m_Camera.SetActive(value: true);
			uIModelGroup.SetActive(active: true);
		}
		this.ClearWeaponContaner();
		this.m_WeaponContainer.gameObject.SetActive(value: true);
		this.ShowWeaponUpgrade();
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = UILayouts.GetWeapons().GetEnumerator();
		while (enumerator.MoveNext())
		{
			UIElementDataVO current;
			current = enumerator.Current;
			UIManager.CreateElement(this.m_WeaponContentContainer, current.PrefabKey, current);
		}
	}

	private void ShowAbilities()
	{
		JDebug.Log("GAME MENU: Abilities", this, JDebug.JDebugType.UI);
		UIModelGroup uIModelGroup;
		uIModelGroup = this.m_UIModelGroups.Find((UIModelGroup x) => x.Name.ToLower() == "ability");
		if (uIModelGroup != null)
		{
			this.m_Camera.SetActive(value: true);
			uIModelGroup.SetActive(active: true);
		}
		this.ClearAbilityContainer();
		this.m_AbilityContainer.gameObject.SetActive(value: true);
		this.ShowAbilityUpgrade();
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = UILayouts.GetAbilities().GetEnumerator();
		while (enumerator.MoveNext())
		{
			UIElementDataVO current;
			current = enumerator.Current;
			UIManager.CreateElement(this.m_AbilityContentContainer, current.PrefabKey, current);
		}
	}

	private void ShowMemos()
	{
		JDebug.Log("GAME MENU: Memos", this, JDebug.JDebugType.UI);
		this.ClearMemos();
		UIElementSpaceDataVO uIElementSpaceDataVO;
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		List<UIElementMemoDataVO>.Enumerator enumerator;
		enumerator = UILayouts.GetMemos().GetEnumerator();
		while (enumerator.MoveNext())
		{
			UIElementMemoButtonLabel uIElementMemoButtonLabel;
			uIElementMemoButtonLabel = (UIElementMemoButtonLabel)UIManager.CreateElement(this.m_InfoScrollableContent, enumerator.Current.Button.PrefabKey, enumerator.Current);
			this.m_Memos.Add(uIElementMemoButtonLabel);
			uIElementMemoButtonLabel.OnClick -= this.HandleMemoOnClick;
			uIElementMemoButtonLabel.OnClick += this.HandleMemoOnClick;
		}
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 180f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		this.m_SelectionIndex = 0;
		this.ShowMemoSelection(this.m_Memos[this.m_SelectionIndex]);
	}

	private void HandleMemoOnClick(object sender, EventArgs e)
	{
		this.ShowMemoSelection(sender as UIElementMemoButtonLabel);
	}

	private void ShowMemoSelection(UIElementMemoButtonLabel element)
	{
		unchecked
		{
			for (int i = 0; i < this.m_Memos.Count; i++)
			{
				this.SetListButton(this.m_Memos[i], element);
			}
			this.ClearContentScrollableContent();
			UIElementSpaceDataVO uIElementSpaceDataVO;
			uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f);
			UIManager.CreateElement(this.m_ContentScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
			UIManager.CreateElement(this.m_ContentScrollableContent, "UI/Elements/UIElementMemo", element.DataVO);
			if (this.m_InfoScrollView.verticalScrollbar.gameObject.activeInHierarchy)
			{
				int count;
				count = this.m_Memos.Count;
				float endValue;
				endValue = 1f - (float)this.m_SelectionIndex / (float)count;
				if (this.m_SelectionIndex == 0)
				{
					endValue = 1f;
				}
				else if (this.m_SelectionIndex == count - 1)
				{
					endValue = 0f;
				}
				this.m_InfoScrollView.DOKill();
				this.m_InfoScrollView.DOVerticalNormalizedPos(endValue, 0.15f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
			}
		}
	}

	private void ShowAudioLogs()
	{
		JDebug.Log("GAME MENU: Audio Logs", this, JDebug.JDebugType.UI);
		this.ClearAudioLogs();
		UIElementSpaceDataVO uIElementSpaceDataVO;
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		List<UIElementAudioLogDataVO>.Enumerator enumerator;
		enumerator = UILayouts.GetAudioLogs().GetEnumerator();
		while (enumerator.MoveNext())
		{
			UIElementAudioLogButtonLabel uIElementAudioLogButtonLabel;
			uIElementAudioLogButtonLabel = (UIElementAudioLogButtonLabel)UIManager.CreateElement(this.m_InfoScrollableContent, enumerator.Current.Button.PrefabKey, enumerator.Current);
			this.m_AudioLogs.Add(uIElementAudioLogButtonLabel);
			uIElementAudioLogButtonLabel.OnClick -= this.HandleAudioLogOnClick;
			uIElementAudioLogButtonLabel.OnClick += this.HandleAudioLogOnClick;
		}
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 180f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		this.m_SelectionIndex = 0;
		this.ShowAudioLogSelection(this.m_AudioLogs[this.m_SelectionIndex]);
	}

	private void HandleAudioLogOnClick(object sender, EventArgs e)
	{
		this.ShowAudioLogSelection(sender as UIElementAudioLogButtonLabel);
	}

	private void ShowAudioLogSelection(UIElementAudioLogButtonLabel element)
	{
		unchecked
		{
			for (int i = 0; i < this.m_AudioLogs.Count; i++)
			{
				UIElementAudioLogButtonLabel uIElementAudioLogButtonLabel;
				uIElementAudioLogButtonLabel = this.m_AudioLogs[i];
				this.SetListButton(uIElementAudioLogButtonLabel, element);
				uIElementAudioLogButtonLabel.OnPlay -= this.HandleAudioLogOnPlay;
				uIElementAudioLogButtonLabel.OnPlay += this.HandleAudioLogOnPlay;
			}
			this.ClearContentScrollableContent();
			UIElementSpaceDataVO uIElementSpaceDataVO;
			uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f);
			UIManager.CreateElement(this.m_ContentScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
			UIManager.CreateElement(this.m_ContentScrollableContent, "UI/Elements/UIElementAudioLog", element.DataVO);
			element.ShowPlayButton();
			if (this.m_InfoScrollView.verticalScrollbar.gameObject.activeInHierarchy)
			{
				int count;
				count = this.m_AudioLogs.Count;
				float endValue;
				endValue = 1f - (float)this.m_SelectionIndex / (float)count;
				if (this.m_SelectionIndex == 0)
				{
					endValue = 1f;
				}
				else if (this.m_SelectionIndex == count - 1)
				{
					endValue = 0f;
				}
				this.m_InfoScrollView.DOKill();
				this.m_InfoScrollView.DOVerticalNormalizedPos(endValue, 0.15f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
			}
		}
	}

	private void HandleAudioLogOnPlay(object sender, EventArgs e)
	{
		this.PlayAudioLog(sender as UIElementAudioLogButtonLabel);
	}

	private void PlayAudioLog(UIElementAudioLogButtonLabel uIElementAudioLogButtonLabel)
	{
		if (uIElementAudioLogButtonLabel.IsPlaying)
		{
			uIElementAudioLogButtonLabel.StopAudioClip();
		}
		else
		{
			uIElementAudioLogButtonLabel.PlayAudioClip(this.m_AudioLogPlayer);
		}
	}

	private void ShowMemories()
	{
		JDebug.Log("GAME MENU: Memories", this, JDebug.JDebugType.UI);
		this.ClearMemories();
		UIElementSpaceDataVO uIElementSpaceDataVO;
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		List<UIElementMemoryDataVO>.Enumerator enumerator;
		enumerator = UILayouts.GetMemories().GetEnumerator();
		while (enumerator.MoveNext())
		{
			UIElementMemoryButtonLabel uIElementMemoryButtonLabel;
			uIElementMemoryButtonLabel = (UIElementMemoryButtonLabel)UIManager.CreateElement(this.m_InfoScrollableContent, enumerator.Current.Button.PrefabKey, enumerator.Current);
			this.m_Memories.Add(uIElementMemoryButtonLabel);
			uIElementMemoryButtonLabel.OnClick -= this.HandleMemoriesOnClick;
			uIElementMemoryButtonLabel.OnClick += this.HandleMemoriesOnClick;
		}
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 180f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		this.m_SelectionIndex = 0;
		this.ShowMemorySelection(this.m_Memories[this.m_SelectionIndex]);
	}

	private void HandleMemoriesOnClick(object sender, EventArgs e)
	{
		this.ShowMemorySelection(sender as UIElementMemoryButtonLabel);
	}

	private void ShowMemorySelection(UIElementMemoryButtonLabel element)
	{
		unchecked
		{
			for (int i = 0; i < this.m_Memories.Count; i++)
			{
				this.SetListButton(this.m_Memories[i], element);
			}
			this.ClearContentScrollableContent();
			UIElementSpaceDataVO uIElementSpaceDataVO;
			uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f);
			UIManager.CreateElement(this.m_ContentScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
			UIManager.CreateElement(this.m_ContentScrollableContent, "UI/Elements/UIElementMemoryImage", element.DataVO);
			UIManager.CreateElement(this.m_ContentScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
			UIManager.CreateElement(this.m_ContentScrollableContent, "UI/Elements/UIElementMemory", element.DataVO);
			if (this.m_InfoScrollView.verticalScrollbar.gameObject.activeInHierarchy)
			{
				int count;
				count = this.m_Memories.Count;
				float endValue;
				endValue = 1f - (float)this.m_SelectionIndex / (float)count;
				if (this.m_SelectionIndex == 0)
				{
					endValue = 1f;
				}
				else if (this.m_SelectionIndex == count - 1)
				{
					endValue = 0f;
				}
				this.m_InfoScrollView.DOKill();
				this.m_InfoScrollView.DOVerticalNormalizedPos(endValue, 0.15f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
			}
		}
	}

	private void ShowSettings()
	{
		JDebug.Log("GAME MENU: Settings", this, JDebug.JDebugType.UI);
		this.ClearSettings();
		UIElementSpaceDataVO uIElementSpaceDataVO;
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = UILayouts.GetSettingsElements().GetEnumerator();
		while (enumerator.MoveNext())
		{
			UIElementGameMenuSettingsButtonDataVO uIElementGameMenuSettingsButtonDataVO;
			uIElementGameMenuSettingsButtonDataVO = enumerator.Current as UIElementGameMenuSettingsButtonDataVO;
			UIElementGameMenuSettingsButton uIElementGameMenuSettingsButton;
			uIElementGameMenuSettingsButton = UIManager.CreateElement(this.m_InfoScrollableContent, uIElementGameMenuSettingsButtonDataVO.PrefabKey, uIElementGameMenuSettingsButtonDataVO) as UIElementGameMenuSettingsButton;
			if ((bool)uIElementGameMenuSettingsButton)
			{
				this.m_Settings.Add(uIElementGameMenuSettingsButton);
				uIElementGameMenuSettingsButton.SetType(uIElementGameMenuSettingsButtonDataVO.SettingsType);
				uIElementGameMenuSettingsButton.OnClick -= this.HandleSettingsButtonOnClick;
				uIElementGameMenuSettingsButton.OnClick += this.HandleSettingsButtonOnClick;
			}
		}
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 180f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		this.m_SelectionIndex = 0;
	}

	private void HandleSettingsButtonOnClick(object sender, EventArgs e)
	{
		UIElementGameMenuSettingsButton element;
		element = sender as UIElementGameMenuSettingsButton;
		this.GetSettingsSelection(element);
		this.ShowSettingsSelection(element);
	}

	private void GetSettingsSelection(UIElementGameMenuSettingsButton element)
	{
		this.m_SelectionIndex = this.m_Settings.IndexOf(element);
		this.ClearContentScrollableContent();
		if (this.m_ActiveSettingsButton != null)
		{
			this.m_ActiveSettingsButton.Button.selected = false;
			this.m_ActiveSettingsButton.ForceOnExit();
			this.m_ActiveSettingsButton.ResetListeners();
			this.m_ActiveSettingsButton.Button.interactable = true;
			if (this.m_ActiveSettingsButton.LabelUI.rectTransform.anchoredPosition != Vector2.zero)
			{
				this.m_ActiveSettingsButton.LabelUI.DOKill();
				this.m_ActiveSettingsButton.LabelUI.rectTransform.DOAnchorPos(Vector2.zero, 0.15f).SetEase(Ease.InQuad).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
			}
		}
		this.m_ActiveSettingsButton = element;
		this.m_ActiveSettingsButton.Button.selected = true;
		this.m_ActiveSettingsButton.ForceOnEnter();
		this.m_ActiveSettingsButton.Button.interactable = false;
	}

	private void ShowSettingsSelection(UIElementGameMenuSettingsButton element)
	{
		element.LabelUI.rectTransform.DOKill();
		element.LabelUI.rectTransform.DOAnchorPosX(1120f - element.LabelUI.renderedWidth, 0.15f).SetEase(Ease.InQuad).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		if (this.m_SelectionIndex == 0)
		{
			list = UILayouts.GetAudioLayout(isTitleMenu: false);
		}
		else if (this.m_SelectionIndex == 1)
		{
			list = UILayouts.GetGraphicsLayout(isTitleMenu: false);
		}
		else if (this.m_SelectionIndex == 2)
		{
			list = UILayouts.GetControlsLayout(isTitleMenu: false);
		}
		this.m_ActiveSubSettings = element;
		this.m_SubSettings.Clear();
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!(enumerator.Current.PrefabKey == string.Empty))
			{
				UIElement uIElement;
				uIElement = UIManager.CreateElement(this.m_ContentScrollableContent, enumerator.Current.PrefabKey, enumerator.Current);
				if (uIElement is UIElementSlider item)
				{
					this.m_SubSettings.Add(item);
				}
				else if (uIElement is UIElementToggle item2)
				{
					this.m_SubSettings.Add(item2);
				}
				else if (uIElement is UIElementSelector item3)
				{
					this.m_SubSettings.Add(item3);
				}
			}
		}
	}

	private void GetSettingsSubMenu(float inputY, float inputX, bool select, bool cancel)
	{
		unchecked
		{
			if (cancel)
			{
				UIElementGameMenuSettingsButton uIElementGameMenuSettingsButton;
				uIElementGameMenuSettingsButton = this.m_Settings[this.m_SelectionIndex];
				uIElementGameMenuSettingsButton.LabelUI.DOKill();
				uIElementGameMenuSettingsButton.LabelUI.rectTransform.DOAnchorPos(Vector2.zero, 0.15f).SetEase(Ease.InQuad).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
				this.ClearContentScrollableContent();
				this.m_ActiveSubSettings = null;
			}
			else if (select)
			{
				if (this.m_ActiveSubSettings != null && this.m_ActiveSubSettings is UIElementToggle uIElementToggle)
				{
					uIElementToggle.ChangeValue();
				}
			}
			else if (inputX > 0.2f)
			{
				if (this.m_ActiveSubSettings != null)
				{
					if (this.m_ActiveSubSettings is UIElementSlider uIElementSlider)
					{
						uIElementSlider.Increase();
					}
					else if (this.m_ActiveSubSettings is UIElementSelector uIElementSelector)
					{
						uIElementSelector.Increase(isController: true);
					}
				}
			}
			else if (inputX < -0.2f)
			{
				if (this.m_ActiveSubSettings != null)
				{
					if (this.m_ActiveSubSettings is UIElementSlider uIElementSlider2)
					{
						uIElementSlider2.Reduce();
					}
					else if (this.m_ActiveSubSettings is UIElementSelector uIElementSelector2)
					{
						uIElementSelector2.Reduce(isController: true);
					}
				}
			}
			else if (inputY > 0.1f)
			{
				if (this.m_SubSelectionIndex > 0)
				{
					this.m_SubSelectionIndex--;
					this.ShowSettingsSubSelection(this.m_SubSettings[this.m_SubSelectionIndex]);
				}
			}
			else if (inputY < -0.1f && this.m_SubSelectionIndex < this.m_SubSettings.Count - 1)
			{
				this.m_SubSelectionIndex++;
				this.ShowSettingsSubSelection(this.m_SubSettings[this.m_SubSelectionIndex]);
			}
		}
	}

	private void ShowSettingsSubSelection(UIElement element)
	{
		if (this.m_ActiveSubSettings != null)
		{
			if (this.m_ActiveSubSettings is UIElementSlider uIElementSlider)
			{
				uIElementSlider.ForceOnExit();
			}
			else if (this.m_ActiveSubSettings is UIElementToggle uIElementToggle)
			{
				uIElementToggle.ForceOnExit();
			}
			else if (this.m_ActiveSubSettings is UIElementSelector uIElementSelector)
			{
				uIElementSelector.ForceOnExit();
			}
		}
		if (element != null)
		{
			this.m_ActiveSubSettings = element;
			if (this.m_ActiveSubSettings is UIElementSlider uIElementSlider2)
			{
				uIElementSlider2.ForceOnEnter();
			}
			else if (this.m_ActiveSubSettings is UIElementToggle uIElementToggle2)
			{
				uIElementToggle2.ForceOnEnter();
			}
			else if (this.m_ActiveSubSettings is UIElementSelector uIElementSelector2)
			{
				uIElementSelector2.ForceOnEnter();
			}
		}
	}

	private void ShowMenu()
	{
		JDebug.Log("GAME MENU: Menu", this, JDebug.JDebugType.UI);
		if (GameManager.Instance.HasController)
		{
			this.m_NavRight.gameObject.SetActive(value: false);
		}
		this.ClearMenus();
		UIElementSpaceDataVO uIElementSpaceDataVO;
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = UILayouts.GetMenuElements().GetEnumerator();
		while (enumerator.MoveNext())
		{
			UIElementGameMenuMenuButtonDataVO uIElementGameMenuMenuButtonDataVO;
			uIElementGameMenuMenuButtonDataVO = enumerator.Current as UIElementGameMenuMenuButtonDataVO;
			UIElementGameMenuMenuButton uIElementGameMenuMenuButton;
			uIElementGameMenuMenuButton = UIManager.CreateElement(this.m_InfoScrollableContent, uIElementGameMenuMenuButtonDataVO.PrefabKey, uIElementGameMenuMenuButtonDataVO) as UIElementGameMenuMenuButton;
			if ((bool)uIElementGameMenuMenuButton)
			{
				this.m_Menus.Add(uIElementGameMenuMenuButton);
				uIElementGameMenuMenuButton.SetType(uIElementGameMenuMenuButtonDataVO.MenuType);
				if (uIElementGameMenuMenuButton.MenuType == GameMenuMenuType.CannotSaveGame || uIElementGameMenuMenuButton.MenuType == GameMenuMenuType.CannotLoadGame)
				{
					uIElementGameMenuMenuButton.SetDisabled(disabled: true);
					continue;
				}
				uIElementGameMenuMenuButton.OnClick -= this.HandleMenuButtonOnClick;
				uIElementGameMenuMenuButton.OnClick += this.HandleMenuButtonOnClick;
			}
		}
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 180f);
		UIManager.CreateElement(this.m_InfoScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		this.m_SelectionIndex = 0;
	}

	private void HandleMenuButtonOnClick(object sender, EventArgs e)
	{
		UIElementGameMenuMenuButton element;
		element = sender as UIElementGameMenuMenuButton;
		this.GetMenuSelection(element);
		if (this.m_ActiveMenuButton.MenuType == GameMenuMenuType.MainMenu)
		{
			this.ShowMenuExit();
		}
		else if (this.m_ActiveMenuButton.MenuType != GameMenuMenuType.CannotSaveGame && this.m_ActiveMenuButton.MenuType != GameMenuMenuType.CannotLoadGame)
		{
			this.ShowMenuSelection(element);
		}
	}

	private void GetMenuSelection(UIElementGameMenuMenuButton element)
	{
		this.m_SelectionIndex = this.m_Menus.IndexOf(element);
		this.ClearContentScrollableContent();
		if (this.m_ActiveMenuButton != null)
		{
			this.m_ActiveMenuButton.Button.selected = false;
			this.m_ActiveMenuButton.ForceOnExit();
			this.m_ActiveMenuButton.ResetListeners();
			this.m_ActiveMenuButton.Button.interactable = true;
			if (this.m_ActiveMenuButton.LabelUI.rectTransform.anchoredPosition != Vector2.zero)
			{
				this.m_ActiveMenuButton.LabelUI.DOKill();
				this.m_ActiveMenuButton.LabelUI.rectTransform.DOAnchorPos(Vector2.zero, 0.15f).SetEase(Ease.InQuad).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
			}
		}
		this.m_ActiveMenuButton = element;
		this.m_ActiveMenuButton.Button.selected = true;
		this.m_ActiveMenuButton.ForceOnEnter();
		this.m_ActiveMenuButton.Button.interactable = false;
	}

	private void ShowMenuExit()
	{
		this.m_IsPromptActive = true;
		this.m_MenuExitPrompt = GameManager.Instance.UIManager.Show<UIPrompt>("UI/GameMenuPrompt/UIPrompt", "GAME MENU PROMPT", new UIPromptDataVO(TextUtility.GetKey("MENU_SETTINGS_MAIN_MENU"), TextUtility.GetKey("MENU_SETTINGS_MAIN_MENU_HEADDER"), new UIElementButtonDataVO("UI/GameMenu/MenuElementButtonControllerLabel", TextUtility.GetKey("MENU_YES"), () =>
		{
			SceneManager.LoadScene("Reset");
		}, "", TextAlignmentOptions.Midline, InputControlType.Action1), new UIElementButtonDataVO("UI/GameMenu/MenuElementButtonControllerLabel", TextUtility.GetKey("MENU_NO"), this.PlayOutMenuExit, "", TextAlignmentOptions.Midline, InputControlType.Action2)));
	}

	private void PlayOutMenuExit()
	{
		if (!GameManager.Instance.HasController)
		{
			foreach (UIElementGameMenuMenuButton menu in this.m_Menus)
			{
				if (menu is UIElementGameMenuMenuButton uIElementGameMenuMenuButton)
				{
					uIElementGameMenuMenuButton.Button.selected = false;
					uIElementGameMenuMenuButton.ForceOnExit();
					uIElementGameMenuMenuButton.ResetListeners();
					uIElementGameMenuMenuButton.Button.interactable = true;
				}
			}
		}
		if (this.m_MenuExitPrompt != null)
		{
			this.m_MenuExitPrompt.PlayOut();
		}
		this.m_IsPromptActive = false;
	}

	private void ShowMenuSelection(UIElementGameMenuMenuButton element)
	{
		element.LabelUI.rectTransform.DOKill();
		element.LabelUI.rectTransform.DOAnchorPosX(1120f - element.LabelUI.renderedWidth, 0.15f).SetEase(Ease.InQuad).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		UIElementSpaceDataVO uIElementSpaceDataVO;
		uIElementSpaceDataVO = new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f);
		UIManager.CreateElement(this.m_ContentScrollableContent, uIElementSpaceDataVO.PrefabKey, uIElementSpaceDataVO);
		UIMenuDataVO uIMenuDataVO;
		uIMenuDataVO = null;
		if (this.m_SelectionIndex == 0)
		{
			uIMenuDataVO = UIMenuDataVO.Create(MenuType.SaveGame);
		}
		else if (this.m_SelectionIndex == 1)
		{
			uIMenuDataVO = UIMenuDataVO.Create(MenuType.LoadGame);
		}
		if (uIMenuDataVO != null)
		{
			this.m_ActiveSubMenu = element;
			this.m_SubMenus.Clear();
			List<UIElementDataVO>.Enumerator enumerator;
			enumerator = uIMenuDataVO.UIElements.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current.PrefabKey == string.Empty))
				{
					UIElement uIElement;
					uIElement = UIManager.CreateElement(this.m_ContentScrollableContent, enumerator.Current.PrefabKey, enumerator.Current);
					if (uIElement is MenuElementSave menuElementSave)
					{
						this.m_SubMenus.Add(menuElementSave);
						menuElementSave.Data.OnPlayOut -= this.HandleSavePromptOnPlayOutComplete;
						menuElementSave.Data.OnPlayOut += this.HandleSavePromptOnPlayOutComplete;
					}
					if (uIElement is MenuElementSaveSlot item)
					{
						this.m_SubMenus.Add(item);
					}
				}
			}
		}
		this.m_InfoScrollView.verticalScrollbar.value = 1f;
	}

	private void GetMenuSubMenu(float inputY, float inputX, bool select, bool cancel)
	{
		unchecked
		{
			if (cancel)
			{
				this.m_Menus[this.m_SelectionIndex].LabelUI.rectTransform.DOAnchorPos(Vector2.zero, 0.15f).SetEase(Ease.InQuad).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
				this.ClearContentScrollableContent();
				this.m_ActiveSubMenu = null;
			}
			else if (select)
			{
				if (!(this.m_ActiveSubMenu != null))
				{
					return;
				}
				if (this.m_ActiveMenuButton.MenuType == GameMenuMenuType.SaveGame)
				{
					if (this.m_ActiveSubMenu is MenuElementSave menuElementSave)
					{
						this.m_IsPromptActive = true;
						menuElementSave.Callback?.Invoke();
					}
					else if (this.m_ActiveSubMenu is MenuElementSaveSlot { Callback: { } callback })
					{
						callback();
					}
				}
				else if (this.m_ActiveMenuButton.MenuType == GameMenuMenuType.LoadGame && this.m_ActiveSubMenu is MenuElementSave { Callback: { } callback2 })
				{
					callback2();
				}
			}
			else if (inputY > 0.1f)
			{
				if (this.m_SubSelectionIndex > 0)
				{
					this.m_SubSelectionIndex--;
					this.ShowMenuSubSelection(this.m_SubMenus[this.m_SubSelectionIndex]);
				}
			}
			else if (inputY < -0.1f && this.m_SubSelectionIndex < this.m_SubMenus.Count - 1)
			{
				this.m_SubSelectionIndex++;
				this.ShowMenuSubSelection(this.m_SubMenus[this.m_SubSelectionIndex]);
			}
		}
	}

	private void HandleSavePromptOnPlayOutComplete(object sender, EventArgs e)
	{
		if (!GameManager.Instance.HasController)
		{
			foreach (UIElement subMenu in this.m_SubMenus)
			{
				if (subMenu is MenuElementSave menuElementSave)
				{
					menuElementSave.Button.selected = false;
					menuElementSave.ForceOnExit();
					menuElementSave.Button.interactable = true;
					menuElementSave.RemoveListeners();
					menuElementSave.AddListeners();
				}
			}
		}
		this.m_IsPromptActive = false;
	}

	private void ShowMenuSubSelection(UIElement element)
	{
		if (this.m_ActiveSubMenu != null)
		{
			if (this.m_ActiveSubMenu is MenuElementSave menuElementSave)
			{
				menuElementSave.ForceOnExit();
			}
			else if (this.m_ActiveSubMenu is MenuElementSaveSlot menuElementSaveSlot)
			{
				menuElementSaveSlot.ForceOnExit();
			}
		}
		if (!(element != null))
		{
			return;
		}
		this.m_ActiveSubMenu = element;
		if (this.m_ActiveSubMenu is MenuElementSave menuElementSave2)
		{
			menuElementSave2.ForceOnEnter();
		}
		else if (this.m_ActiveSubMenu is MenuElementSaveSlot menuElementSaveSlot2)
		{
			menuElementSaveSlot2.ForceOnEnter();
		}
		if (this.m_ContentScrollView.verticalScrollbar.gameObject.activeInHierarchy)
		{
			int num;
			num = this.m_SubMenus.Count;
			if (this.m_ActiveMenuButton.MenuType == GameMenuMenuType.SaveGame)
			{
				num = 10;
			}
			float endValue;
			endValue = 1f - (float)this.m_SubSelectionIndex / (float)num;
			if (this.m_SubSelectionIndex == 0)
			{
				endValue = 1f;
			}
			else if (this.m_SubSelectionIndex == unchecked(num - 1))
			{
				endValue = 0f;
			}
			this.m_ContentScrollView.DOKill();
			this.m_ContentScrollView.DOVerticalNormalizedPos(endValue, 0.15f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		}
	}

	private void HandleButtonOnClick(object sender, EventArgs e)
	{
		this.ShowSelection((UIElementGameMenuButton)sender);
	}

	private void ShowSelection(UIElementGameMenuButton button, bool isController = false)
	{
		if (this.m_NavButtons.Contains(button))
		{
			this.m_NavIndex = this.m_NavButtons.IndexOf(button);
		}
		if ((bool)this.m_ActiveNav)
		{
			this.m_ActiveNav.Button.selected = false;
			this.m_ActiveNav.ForceOnExit();
		}
		this.m_ActiveNav = button;
		this.m_ActiveNav.Button.selected = true;
		this.m_ActiveNav.ForceOnEnter();
		this.m_Camera.SetActive(value: false);
		foreach (UIModelGroup uIModelGroup in this.m_UIModelGroups)
		{
			uIModelGroup.SetActive(active: false);
		}
		this.m_StatusContainer.gameObject.SetActive(value: false);
		this.m_WeaponContainer.gameObject.SetActive(value: false);
		this.m_AbilityContainer.gameObject.SetActive(value: false);
		this.m_UpgradeContainer.gameObject.SetActive(value: false);
		this.m_HealthContainer.gameObject.SetActive(value: false);
		this.ClearInfoScrollableContent();
		this.ClearContentScrollableContent();
		this.ClearUpgradeContent();
		this.ClearHealthContent();
		this.ClearResourceContainer();
		if (GameManager.Instance.HasController && (button.GameMenuType != GameMenuType.Status || button.GameMenuType != GameMenuType.Menu))
		{
			this.m_NavLeft.gameObject.SetActive(value: true);
			this.m_NavRight.gameObject.SetActive(value: true);
		}
		if (button.GameMenuType == GameMenuType.Status)
		{
			this.ShowStatus();
		}
		else if (button.GameMenuType == GameMenuType.Weapon)
		{
			this.ShowWeapons();
		}
		else if (button.GameMenuType == GameMenuType.Abilities)
		{
			this.ShowAbilities();
		}
		else if (button.GameMenuType == GameMenuType.Memos)
		{
			this.ShowMemos();
		}
		else if (button.GameMenuType == GameMenuType.AudioLogs)
		{
			this.ShowAudioLogs();
		}
		else if (button.GameMenuType == GameMenuType.Memories)
		{
			this.ShowMemories();
		}
		else if (button.GameMenuType == GameMenuType.Settings)
		{
			this.ShowSettings();
			if (isController)
			{
				this.GetSettingsSelection(this.m_Settings[this.m_SelectionIndex]);
			}
		}
		else if (button.GameMenuType == GameMenuType.Menu)
		{
			this.ShowMenu();
			if (isController)
			{
				this.GetMenuSelection(this.m_Menus[this.m_SelectionIndex]);
			}
		}
		this.m_InfoScrollView.verticalScrollbar.value = 1f;
	}

	private void SetListButton(UIElementButton button, UIElementButton selected)
	{
		if (button != selected)
		{
			button.Button.selected = false;
			button.ResetButton();
		}
		else
		{
			button.Button.selected = true;
			button.ForceOnEnter();
		}
	}

	private void ClearNavButtons(bool dispose = false)
	{
		if (this.m_NavButtons != null)
		{
			for (int i = 0; i < this.m_NavButtons.Count; i = unchecked(i + 1))
			{
				this.m_NavButtons[i].OnClick -= this.HandleButtonOnClick;
			}
			this.m_NavButtons.Clear();
			if (dispose)
			{
				this.m_NavButtons = null;
			}
		}
	}

	private void ClearMemos(bool dispose = false)
	{
		if (this.m_Memos != null)
		{
			for (int i = 0; i < this.m_Memos.Count; i = unchecked(i + 1))
			{
				this.m_Memos[i].OnClick -= this.HandleMemoOnClick;
			}
			this.m_Memos.Clear();
			if (dispose)
			{
				this.m_Memos = null;
			}
		}
	}

	private void ClearAudioLogs(bool dispose = false)
	{
		if (this.m_AudioLogs != null)
		{
			for (int i = 0; i < this.m_AudioLogs.Count; i = unchecked(i + 1))
			{
				UIElementAudioLogButtonLabel uIElementAudioLogButtonLabel;
				uIElementAudioLogButtonLabel = this.m_AudioLogs[i];
				uIElementAudioLogButtonLabel.OnPlay -= this.HandleAudioLogOnPlay;
				uIElementAudioLogButtonLabel.OnClick -= this.HandleAudioLogOnClick;
			}
			this.m_AudioLogs.Clear();
			if (dispose)
			{
				this.m_AudioLogs = null;
			}
		}
	}

	private void ClearMemories(bool dispose = false)
	{
		if (this.m_Memories != null)
		{
			for (int i = 0; i < this.m_Memories.Count; i = unchecked(i + 1))
			{
				this.m_Memories[i].OnClick -= this.HandleMemoriesOnClick;
			}
			this.m_Memories.Clear();
			if (dispose)
			{
				this.m_Memories = null;
			}
		}
	}

	private void ClearSettings(bool dispose = false)
	{
		if (this.m_Settings != null)
		{
			for (int i = 0; i < this.m_Settings.Count; i = unchecked(i + 1))
			{
				this.m_Settings[i].OnClick -= this.HandleSettingsButtonOnClick;
			}
			this.m_Settings.Clear();
			if (dispose)
			{
				this.m_Settings = null;
			}
		}
	}

	private void ClearMenus(bool dispose = false)
	{
		if (this.m_Menus != null)
		{
			for (int i = 0; i < this.m_Menus.Count; i = unchecked(i + 1))
			{
				this.m_Menus[i].OnClick -= this.HandleMenuButtonOnClick;
			}
			this.m_Menus.Clear();
			if (dispose)
			{
				this.m_Menus = null;
			}
		}
	}

	private void ClearInfoScrollableContent()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_InfoScrollableContent.GetComponentsInChildren<UIElement>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].Dispose();
		}
	}

	private void ClearContentScrollableContent()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_ContentScrollableContent.GetComponentsInChildren<UIElement>(includeInactive: true);
		foreach (UIElement obj in componentsInChildren)
		{
			if (obj is MenuElementSave menuElementSave && menuElementSave != null && menuElementSave.Data != null)
			{
				menuElementSave.Data.OnPlayOut -= this.HandleSavePromptOnPlayOutComplete;
			}
			obj.Dispose();
		}
	}

	private void ClearResourceContainer()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_ResourceContainer.GetComponentsInChildren<UIElement>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].Dispose();
		}
	}

	private void ClearUpgradeContent()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_UpgradeContainer.GetComponentsInChildren<UIElement>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].Dispose();
		}
	}

	private void ClearHealthContent()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_HealthContainer.GetComponentsInChildren<UIElement>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].Dispose();
		}
	}

	private void ClearWeaponContaner()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_WeaponContainer.GetComponentsInChildren<UIElement>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].Dispose();
		}
	}

	private void ClearAbilityContainer()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_AbilityContainer.GetComponentsInChildren<UIElement>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].Dispose();
		}
	}

	private void ClearAll()
	{
		this.ClearNavButtons(dispose: true);
		this.ClearMemos(dispose: true);
		this.ClearAudioLogs(dispose: true);
		this.ClearMemories(dispose: true);
		this.ClearSettings(dispose: true);
		this.ClearMenus(dispose: true);
	}

	protected override void OnDisposed()
	{
		this.ClearAll();
		if (this.m_NavButtons != null)
		{
			this.m_NavButtons.Clear();
			this.m_NavButtons = null;
		}
		this.m_ActiveNav = null;
		if (this.m_Memos != null)
		{
			this.m_Memos.Clear();
			this.m_Memos = null;
		}
		if (this.m_AudioLogs != null)
		{
			this.m_AudioLogs.Clear();
			this.m_AudioLogs = null;
		}
		if (this.m_Memories != null)
		{
			this.m_Memories.Clear();
			this.m_Memories = null;
		}
		if (this.m_Settings != null)
		{
			foreach (UIElementGameMenuSettingsButton setting in this.m_Settings)
			{
				if (setting != null)
				{
					setting.LabelUI.rectTransform?.DOKill();
				}
			}
			this.m_Settings.Clear();
			this.m_Settings = null;
		}
		if (this.m_SubSettings != null)
		{
			this.m_SubSettings.Clear();
			this.m_SubSettings = null;
		}
		this.m_ActiveSubSettings = null;
		if (this.m_Menus != null)
		{
			foreach (UIElementGameMenuMenuButton menu in this.m_Menus)
			{
				if (menu != null)
				{
					menu.LabelUI.rectTransform?.DOKill();
				}
			}
			this.m_Menus.Clear();
			this.m_Menus = null;
		}
		if (this.m_SubMenus != null)
		{
			this.m_SubMenus.Clear();
			this.m_SubMenus = null;
		}
		this.m_ActiveSubMenu = null;
		if (this.m_ActiveSettingsButton != null)
		{
			this.m_ActiveSettingsButton.LabelUI?.DOKill();
			this.m_ActiveSettingsButton = null;
		}
		if (this.m_ActiveMenuButton != null)
		{
			this.m_ActiveMenuButton.LabelUI?.DOKill();
			this.m_ActiveMenuButton = null;
		}
		this.m_InfoScrollView?.DOKill();
		this.m_DisolveSequence?.Kill();
		this.m_DisolveSequence = null;
		this.m_DataVO = null;
		this.m_MenuExitPrompt = null;
		base.OnDisposed();
	}
}
