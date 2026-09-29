using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UITitleView : UIController
{
	[SerializeField]
	private VideoPlayer m_VideoPlayer;

	[SerializeField]
	private Image m_BlackOverlay;

	[Header("Logo")]
	[SerializeField]
	private RectTransform m_Logo;

	[SerializeField]
	private RectTransform m_LogoBG;

	[SerializeField]
	private RectTransform m_LogoEndLocation;

	[Header("Start")]
	[SerializeField]
	private RectTransform m_StartGameContainer;

	[SerializeField]
	private CanvasGroup m_StartGameCanvasGroup;

	[Header("Menu")]
	[SerializeField]
	private CanvasGroup m_MenuCanvasGroup;

	[SerializeField]
	private RectTransform m_MenuContainer;

	[SerializeField]
	private GameObject m_MenuContentContainer;

	[SerializeField]
	private RectTransform m_LoadContentElementContainer;

	[SerializeField]
	private ScrollRect m_LoadScrollRect;

	[Header("Headers")]
	[SerializeField]
	private RectTransform m_MenuHeaderContainer;

	[Header("Difficulty")]
	[SerializeField]
	private RectTransform m_DifficultyContainer;

	[SerializeField]
	private RectTransform m_DifficultyElementContainer;

	[SerializeField]
	private RectTransform m_DifficultyDescriptionContainer;

	[Header("Controller Navigation")]
	[SerializeField]
	private RectTransform m_NavigationContainer;

	[SerializeField]
	private RectTransform m_NavBackContainer;

	[SerializeField]
	private Image m_NavBackButtonIcon;

	[SerializeField]
	private Image m_NavBackButtonInput;

	[SerializeField]
	private TextMeshProUGUI m_NavBackButtonLabel;

	[SerializeField]
	private RectTransform m_NavSelectContainer;

	[SerializeField]
	private Image m_NavSelectButtonIcon;

	[SerializeField]
	private Image m_NavSelectButtonInput;

	[SerializeField]
	private TextMeshProUGUI m_NavSelectButtonLabel;

	[SerializeField]
	private RectTransform m_NavigationNavContainer;

	[SerializeField]
	private Image m_NavNavLButtonIcon;

	[SerializeField]
	private Image m_NavNavLButtonInput;

	[SerializeField]
	private Image m_NavNavRButtonIcon;

	[SerializeField]
	private Image m_NavNavRButtonInput;

	[SerializeField]
	private TextMeshProUGUI m_NavNavButtonLabel;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_CopyrightLabel;

	[SerializeField]
	private TextMeshProUGUI m_VersionLabel;

	private Vector2 m_LogoStartPosition;

	private UIElementButtonLabel m_StartGameButton;

	private float m_InputTimer;

	private float m_InputTimerLimit = 0.25f;

	private int m_SelectionIndex;

	private int m_SubSelectionIndex;

	private bool m_IsInTitleMenu;

	private List<UIElementTitleMenuButton> m_TitleMenu = new List<UIElementTitleMenuButton>();

	private UIElementTitleMenuButton m_ActiveTitleMenu;

	private List<UIElementButtonLabel> m_SubMenu = new List<UIElementButtonLabel>();

	private UIElementButtonLabel m_ActiveSubMenu;

	private UIElementLabel m_DifficultyDescriptionLabel;

	private List<UIElementButtonLabel> m_OptionsMenu = new List<UIElementButtonLabel>();

	private List<UIElement> m_OptionsSubMenu = new List<UIElement>();

	private UIElement m_ActiveSubOption;

	private int m_OptionsIndex;

	private bool m_HasLoaded;

	private bool m_HasStarted;

	private Sequence m_Sequence;

	protected override void OnInitialized(object _data)
	{
		GameManager.Instance.ClearScreenBlocker();
		this.m_VersionLabel.text = GameManager.Instance.GetProjectVersion();
		this.m_NavBackButtonIcon.color = UIColors.INACTIVE_LABEL_REAL;
		this.m_NavBackButtonInput.color = UIColors.ACTIVE_LABEL_REAL;
		this.m_NavBackButtonInput.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action2);
		this.m_NavBackButtonLabel.color = UIColors.INACTIVE_LABEL_REAL;
		this.m_NavBackButtonLabel.text = TextUtility.GetKey("MENU_BACK");
		this.m_NavSelectButtonIcon.color = UIColors.INACTIVE_LABEL_REAL;
		this.m_NavSelectButtonInput.color = UIColors.ACTIVE_LABEL_REAL;
		this.m_NavSelectButtonInput.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action1);
		this.m_NavSelectButtonLabel.color = UIColors.INACTIVE_LABEL_REAL;
		this.m_NavSelectButtonLabel.text = TextUtility.GetKey("MENU_SELECT");
		this.m_NavNavLButtonIcon.color = UIColors.INACTIVE_LABEL_REAL;
		this.m_NavNavLButtonInput.color = UIColors.ACTIVE_LABEL_REAL;
		this.m_NavNavLButtonInput.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.LeftBumper);
		this.m_NavNavRButtonIcon.color = UIColors.INACTIVE_LABEL_REAL;
		this.m_NavNavRButtonInput.color = UIColors.ACTIVE_LABEL_REAL;
		this.m_NavNavRButtonInput.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.RightBumper);
		this.m_NavNavButtonLabel.color = UIColors.INACTIVE_LABEL_REAL;
		this.m_NavNavButtonLabel.text = TextUtility.GetKey("MENU_NAVIGATION");
		this.m_NavigationNavContainer.gameObject.SetActive(value: false);
		this.m_NavBackContainer.gameObject.SetActive(value: false);
		this.m_NavigationContainer.gameObject.SetActive(value: false);
		this.m_VersionLabel.gameObject.SetActive(value: false);
		this.m_LogoStartPosition = this.m_Logo.anchoredPosition;
		this.m_DifficultyContainer.gameObject.SetActive(value: false);
		this.m_MenuContentContainer.SetActive(value: false);
		this.m_BlackOverlay.gameObject.SetActive(value: true);
		this.m_MenuCanvasGroup.gameObject.SetActive(value: false);
		this.m_MenuCanvasGroup.alpha = 0f;
		this.m_StartGameCanvasGroup.alpha = 0f;
		this.m_TitleMenu.Clear();
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = UILayouts.GetTitleMenu().GetEnumerator();
		while (enumerator.MoveNext())
		{
			UIElementTitleMenuButtonDataVO uIElementTitleMenuButtonDataVO;
			uIElementTitleMenuButtonDataVO = enumerator.Current as UIElementTitleMenuButtonDataVO;
			UIElementTitleMenuButton uIElementTitleMenuButton;
			uIElementTitleMenuButton = (UIElementTitleMenuButton)UIManager.CreateElement(this.m_MenuContainer, uIElementTitleMenuButtonDataVO.PrefabKey, uIElementTitleMenuButtonDataVO);
			uIElementTitleMenuButton.SetType(uIElementTitleMenuButtonDataVO.TitleMenuType);
			this.m_TitleMenu.Add(uIElementTitleMenuButton);
			uIElementTitleMenuButton.OnClick -= this.HandleTitleMenuButtonOnClick;
			uIElementTitleMenuButton.OnClick += this.HandleTitleMenuButtonOnClick;
		}
		UIElementDataVO uIElementDataVO;
		uIElementDataVO = new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_START_GAME").ToUpper());
		this.m_StartGameButton = (UIElementButtonLabel)UIManager.CreateElement(this.m_StartGameContainer, uIElementDataVO.PrefabKey, uIElementDataVO);
	}

	public override void PlayIn()
	{
		this.m_Sequence = DOTween.Sequence();
		this.m_Sequence.InsertCallback(2f, () =>
		{
			this.m_VideoPlayer.Play();
		});
		this.m_Sequence.Insert(2f, this.m_BlackOverlay.DOFade(0f, 1f).SetEase(Ease.Linear));
		this.m_Sequence.Insert(2.5f, this.m_StartGameCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.Linear));
		this.m_Sequence.OnComplete(this.PlayInComplete);
	}

	public override void PlayInComplete()
	{
		this.m_BlackOverlay.gameObject.SetActive(value: false);
		this.m_StartGameButton.OnClick -= this.HandleStartGameOnClick;
		this.m_StartGameButton.OnClick += this.HandleStartGameOnClick;
		GameManager.Instance.ShowCursor();
		this.m_NavigationContainer.gameObject.SetActive(GameManager.Instance.HasController);
		this.m_HasLoaded = true;
	}

	private void Update()
	{
		if (!this.m_HasLoaded || base.IsDisposed)
		{
			return;
		}
		bool flag;
		flag = PlayerInput.CheckController();
		if (this.m_NavigationContainer != null)
		{
			if (flag)
			{
				GameManager.Instance.HideCursor();
				this.m_NavigationContainer.gameObject.SetActive(value: true);
			}
			else
			{
				GameManager.Instance.ShowCursor();
				this.m_NavigationContainer.gameObject.SetActive(value: false);
			}
		}
		if (!flag)
		{
			if (this.m_IsInTitleMenu && this.m_ActiveTitleMenu != null)
			{
				this.ShowSelection(null);
				this.m_ActiveTitleMenu = null;
			}
			return;
		}
		float num;
		num = PlayerInput.ControllerX();
		float num2;
		num2 = PlayerInput.ControllerY();
		bool flag2;
		flag2 = PlayerInput.ControllerAction1();
		bool cancel;
		cancel = PlayerInput.ControllerAction2();
		if (num2 == 0f && num == 0f)
		{
			this.m_InputTimer = this.m_InputTimerLimit;
		}
		if (this.m_InputTimer >= this.m_InputTimerLimit)
		{
			if (!this.m_HasStarted)
			{
				if (PlayerInput.ControllerStart() || PlayerInput.ControllerAction1())
				{
					this.ShowTitleMenu();
				}
				return;
			}
			if (this.m_IsInTitleMenu)
			{
				this.GetTitleMenuInput(num2, flag2, cancel);
			}
			else if (this.m_ActiveTitleMenu != null && this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.Options)
			{
				this.GetOptionsSubMenu(num2, num, flag2, cancel);
			}
			else
			{
				this.GetSubMenuInput(num2, flag2, cancel);
			}
			if (num2 != 0f || num != 0f)
			{
				this.m_InputTimer = 0f;
			}
		}
		else
		{
			this.m_InputTimer += Time.deltaTime;
		}
	}

	private void GetTitleMenuInput(float input, bool select, bool cancel)
	{
		unchecked
		{
			if (cancel)
			{
				this.HideTitleMenu();
			}
			else if (select)
			{
				this.GetSelection();
			}
			else if (input > 0f)
			{
				if (this.m_SelectionIndex > 0)
				{
					this.m_SelectionIndex--;
					this.ShowSelection(this.m_TitleMenu[this.m_SelectionIndex]);
				}
			}
			else if (input < 0f && this.m_SelectionIndex < this.m_TitleMenu.Count - 1)
			{
				this.m_SelectionIndex++;
				this.ShowSelection(this.m_TitleMenu[this.m_SelectionIndex]);
			}
		}
	}

	private void GetSubMenuInput(float input, bool select, bool cancel)
	{
		unchecked
		{
			if (cancel)
			{
				this.m_IsInTitleMenu = true;
				this.ClearEverything_();
				this.m_MenuContentContainer.SetActive(value: false);
				this.ShowSelection(this.m_ActiveTitleMenu);
			}
			else if (select)
			{
				if (this.m_ActiveSubMenu != null)
				{
					this.m_ActiveSubMenu.Callback?.Invoke();
				}
			}
			else if (input > 0f)
			{
				if (this.m_SubSelectionIndex > 0)
				{
					this.m_SubSelectionIndex--;
					this.ShowSubSection(this.m_SubMenu[this.m_SubSelectionIndex]);
				}
			}
			else if (input < 0f && this.m_SubSelectionIndex < this.m_SubMenu.Count - 1)
			{
				this.m_SubSelectionIndex++;
				this.ShowSubSection(this.m_SubMenu[this.m_SubSelectionIndex]);
			}
		}
	}

	private void GetOptionsSubMenu(float input, float inputX, bool select, bool cancel)
	{
		unchecked
		{
			if (cancel)
			{
				this.m_NavigationNavContainer.gameObject.SetActive(value: false);
				this.m_IsInTitleMenu = true;
				this.ClearEverything_();
				this.m_MenuContentContainer.SetActive(value: false);
				this.ShowSelection(this.m_ActiveTitleMenu);
			}
			else if (select)
			{
				if (this.m_ActiveSubOption != null && this.m_ActiveSubOption is UIElementToggle uIElementToggle)
				{
					uIElementToggle.ChangeValue();
				}
			}
			else if (PlayerInput.ControllerLeftBumper())
			{
				if (this.m_OptionsIndex > 0)
				{
					this.m_OptionsIndex--;
					this.ShowOptionsTab(this.m_OptionsMenu[this.m_OptionsIndex]);
					this.m_SubSelectionIndex = 0;
					this.GetSubSelection();
				}
			}
			else if (PlayerInput.ControllerRightBumper())
			{
				if (this.m_OptionsIndex < this.m_OptionsMenu.Count - 1)
				{
					this.m_OptionsIndex++;
					this.ShowOptionsTab(this.m_OptionsMenu[this.m_OptionsIndex]);
					this.m_SubSelectionIndex = 0;
					this.GetSubSelection();
				}
			}
			else if (inputX > 0.2f)
			{
				if (this.m_ActiveSubOption != null)
				{
					if (this.m_ActiveSubOption is UIElementSlider uIElementSlider)
					{
						uIElementSlider.Increase();
					}
					else if (this.m_ActiveSubOption is UIElementSelector uIElementSelector)
					{
						uIElementSelector.Increase(isController: true);
					}
				}
			}
			else if (inputX < -0.2f)
			{
				if (this.m_ActiveSubOption != null)
				{
					if (this.m_ActiveSubOption is UIElementSlider uIElementSlider2)
					{
						uIElementSlider2.Reduce();
					}
					else if (this.m_ActiveSubOption is UIElementSelector uIElementSelector2)
					{
						uIElementSelector2.Reduce(isController: true);
					}
				}
			}
			else if (input > 0.1f)
			{
				if (this.m_SubSelectionIndex > 0)
				{
					this.m_SubSelectionIndex--;
					this.GetSubSelection();
				}
			}
			else if (input < -0.1f && this.m_SubSelectionIndex < this.m_OptionsSubMenu.Count - 1)
			{
				this.m_SubSelectionIndex++;
				this.GetSubSelection();
			}
		}
	}

	private void ShowSelection(UIElementTitleMenuButton button)
	{
		if (this.m_ActiveTitleMenu != null)
		{
			this.m_ActiveTitleMenu.ForceOnExit();
			this.m_ActiveTitleMenu.Button.selected = false;
			this.m_ActiveTitleMenu.Button.interactable = true;
		}
		if (button != null)
		{
			if (this.m_TitleMenu.Contains(button))
			{
				this.m_SelectionIndex = this.m_TitleMenu.IndexOf(button);
			}
			this.m_ActiveTitleMenu = button;
			this.m_ActiveTitleMenu.Button.selected = true;
			this.m_ActiveTitleMenu.ForceOnEnter();
		}
	}

	private void ShowSubSection(UIElementButtonLabel button)
	{
		if (this.m_ActiveSubMenu != null)
		{
			this.m_ActiveSubMenu.ForceOnExit();
			this.m_ActiveSubMenu.Button.selected = false;
			this.m_ActiveSubMenu.Button.interactable = true;
		}
		if (!(button != null))
		{
			return;
		}
		if (this.m_SubMenu.Contains(button))
		{
			this.m_SelectionIndex = this.m_SubMenu.IndexOf(button);
		}
		this.m_ActiveSubMenu = button;
		this.m_ActiveSubMenu.Button.selected = true;
		this.m_ActiveSubMenu.ForceOnEnter();
		if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.NewGame)
		{
			this.ShowDifficultyDescription(button);
		}
		else if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.LoadGame && this.m_LoadScrollRect.verticalScrollbar.gameObject.activeInHierarchy)
		{
			float endValue;
			endValue = 1f - (float)this.m_SelectionIndex / (float)this.m_SubMenu.Count;
			if (this.m_SelectionIndex == 0)
			{
				endValue = 1f;
			}
			else if (this.m_SelectionIndex == unchecked(this.m_SubMenu.Count - 1))
			{
				endValue = 0f;
			}
			this.m_LoadScrollRect.DOKill();
			this.m_LoadScrollRect.DOVerticalNormalizedPos(endValue, 0.15f).SetEase(Ease.InOutSine);
		}
	}

	private void ShowOptionsSubSelection(UIElement element)
	{
		if (this.m_ActiveSubOption != null)
		{
			if (this.m_ActiveSubOption is UIElementSlider uIElementSlider)
			{
				uIElementSlider.ForceOnExit();
			}
			else if (this.m_ActiveSubOption is UIElementToggle uIElementToggle)
			{
				uIElementToggle.ForceOnExit();
			}
			else if (this.m_ActiveSubOption is UIElementSelector uIElementSelector)
			{
				uIElementSelector.ForceOnExit();
			}
		}
		if (element != null)
		{
			this.m_ActiveSubOption = element;
			if (this.m_ActiveSubOption is UIElementSlider uIElementSlider2)
			{
				uIElementSlider2.ForceOnEnter();
			}
			else if (this.m_ActiveSubOption is UIElementToggle uIElementToggle2)
			{
				uIElementToggle2.ForceOnEnter();
			}
			else if (this.m_ActiveSubOption is UIElementSelector uIElementSelector2)
			{
				uIElementSelector2.ForceOnEnter();
			}
		}
	}

	private void GetSelection()
	{
		if (this.m_ActiveTitleMenu != null)
		{
			UIElementTitleMenuButton element;
			element = this.m_TitleMenu[this.m_SelectionIndex];
			this.m_ActiveTitleMenu.ForceOnClik();
			if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.Continue)
			{
				SaveFileData saveFileData;
				saveFileData = (SaveFileData)GameManager.Instance.GameData.SaveFileDirectory.GetValueOfIndex(0);
				if (saveFileData != null)
				{
					this.LoadSave(saveFileData.Name);
				}
				return;
			}
			if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.NewGame)
			{
				this.ShowNewGame(element);
			}
			else if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.LoadGame)
			{
				this.ShowLoadGame(element);
			}
			else if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.Options)
			{
				this.ShowOptions(element);
			}
			else if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.Archives)
			{
				this.ShowArchives(element);
			}
			else if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.Credits)
			{
				this.ShowCredits(element);
			}
			else if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.Quit)
			{
				this.ShowQuit(element);
			}
		}
		this.m_SubSelectionIndex = 0;
		if (this.m_ActiveTitleMenu != null && this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.NewGame)
		{
			this.m_SubSelectionIndex = 1;
		}
		this.GetSubSelection();
	}

	private void GetSubSelection()
	{
		if (this.m_ActiveTitleMenu != null)
		{
			if (this.m_ActiveTitleMenu.TitleMenuType == TitleMenuType.Options)
			{
				this.ShowOptionsSubSelection(this.m_OptionsSubMenu[this.m_SubSelectionIndex]);
			}
			else if (this.m_ActiveTitleMenu.TitleMenuType != TitleMenuType.Quit && this.m_SubSelectionIndex >= 0 && this.m_SubSelectionIndex < this.m_SubMenu.Count)
			{
				this.ShowSubSection(this.m_SubMenu[this.m_SubSelectionIndex]);
			}
		}
	}

	private void HandleStartGameOnClick(object sender, EventArgs e)
	{
		this.ShowTitleMenu();
	}

	private void ShowTitleMenu()
	{
		this.m_HasStarted = true;
		this.m_IsInTitleMenu = true;
		this.m_SelectionIndex = 0;
		this.m_NavBackContainer.gameObject.SetActive(value: true);
		if (PlayerInput.CheckController())
		{
			this.ShowSelection(this.m_TitleMenu[this.m_SelectionIndex]);
		}
		this.m_LogoBG.DOAnchorPos(this.m_LogoEndLocation.anchoredPosition, 1f).SetEase(Ease.OutSine);
		this.m_Logo.DOAnchorPos(this.m_LogoEndLocation.anchoredPosition, 1f).SetEase(Ease.OutSine);
		this.m_StartGameCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
		{
			this.m_StartGameCanvasGroup.gameObject.SetActive(value: false);
			this.m_MenuCanvasGroup.gameObject.SetActive(value: true);
			this.m_MenuCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.Linear);
		});
	}

	private void HideTitleMenu()
	{
		this.m_IsInTitleMenu = false;
		this.m_StartGameButton.ForceOnExit();
		this.m_StartGameButton.Button.selected = false;
		this.m_StartGameButton.Button.interactable = true;
		this.m_StartGameButton.ResetListeners();
		this.m_DifficultyContainer.gameObject.SetActive(value: false);
		this.m_MenuContentContainer.SetActive(value: false);
		this.m_NavBackContainer.gameObject.SetActive(value: false);
		this.m_NavigationNavContainer.gameObject.SetActive(value: false);
		this.m_StartGameCanvasGroup.gameObject.SetActive(value: true);
		this.m_MenuCanvasGroup.gameObject.SetActive(value: false);
		this.m_LogoBG.DOAnchorPos(this.m_LogoStartPosition, 1f).SetEase(Ease.OutSine);
		this.m_Logo.DOAnchorPos(this.m_LogoStartPosition, 1f).SetEase(Ease.OutSine);
		this.m_StartGameCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
		{
			this.m_HasStarted = false;
			this.m_IsInTitleMenu = false;
		});
	}

	private void ClearEverything_(UIElementTitleMenuButton element = null)
	{
		this.ClearLoadContentElementContainer();
		this.ClearDifficultyElementContainer();
		this.ClearMenuHeaderContainer();
		for (int i = 0; i < this.m_TitleMenu.Count; i = unchecked(i + 1))
		{
			UIElementButtonLabel uIElementButtonLabel;
			uIElementButtonLabel = this.m_TitleMenu[i];
			if (uIElementButtonLabel != element || element.TitleMenuType == TitleMenuType.Continue)
			{
				uIElementButtonLabel.ForceOnExit();
				uIElementButtonLabel.ResetListeners();
				if (uIElementButtonLabel.LabelUI.rectTransform.anchoredPosition != Vector2.zero)
				{
					uIElementButtonLabel.LabelUI.rectTransform.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.InQuad);
					uIElementButtonLabel.Button.interactable = true;
				}
			}
		}
	}

	private void HandleTitleMenuButtonOnClick(object sender, EventArgs e)
	{
		UIElementTitleMenuButton uIElementTitleMenuButton;
		uIElementTitleMenuButton = sender as UIElementTitleMenuButton;
		this.ShowSelection(uIElementTitleMenuButton);
		if (uIElementTitleMenuButton.TitleMenuType == TitleMenuType.Continue)
		{
			this.m_MenuContentContainer.SetActive(value: false);
			SaveFileData saveFileData;
			saveFileData = (SaveFileData)GameManager.Instance.GameData.SaveFileDirectory.GetValueOfIndex(0);
			if (saveFileData != null)
			{
				this.LoadSave(saveFileData.Name);
			}
		}
		else if (uIElementTitleMenuButton.TitleMenuType == TitleMenuType.NewGame)
		{
			this.ShowNewGame(uIElementTitleMenuButton);
		}
		else if (uIElementTitleMenuButton.TitleMenuType == TitleMenuType.LoadGame)
		{
			this.ShowLoadGame(uIElementTitleMenuButton);
		}
		else if (uIElementTitleMenuButton.TitleMenuType == TitleMenuType.Options)
		{
			this.ShowOptions(uIElementTitleMenuButton);
		}
		else if (uIElementTitleMenuButton.TitleMenuType == TitleMenuType.Archives)
		{
			this.ShowArchives(uIElementTitleMenuButton);
		}
		else if (uIElementTitleMenuButton.TitleMenuType == TitleMenuType.Credits)
		{
			this.ShowCredits(uIElementTitleMenuButton);
		}
		else if (uIElementTitleMenuButton.TitleMenuType == TitleMenuType.Quit)
		{
			this.ShowQuit(uIElementTitleMenuButton);
		}
		else
		{
			this.m_MenuContentContainer.SetActive(value: false);
		}
		this.m_LoadScrollRect.verticalScrollbar.value = 1f;
	}

	private void ShowNewGame(UIElementTitleMenuButton element)
	{
		this.ClearEverything_(element);
		this.m_ActiveTitleMenu = element;
		this.m_SelectionIndex = this.m_TitleMenu.IndexOf(this.m_ActiveTitleMenu);
		this.m_IsInTitleMenu = false;
		element.LabelUI.rectTransform.DOAnchorPosX(800f - element.LabelUI.renderedWidth / 2f, 0.25f).SetEase(Ease.InQuad);
		this.m_MenuContentContainer.SetActive(value: true);
		this.m_DifficultyContainer.gameObject.SetActive(value: true);
		this.m_VersionLabel.gameObject.SetActive(value: false);
		UIElementButtonDataVO uIElementButtonDataVO;
		uIElementButtonDataVO = new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuTab", TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_HEADER").ToUpper());
		UIElementButtonLabel uIElementButtonLabel;
		uIElementButtonLabel = (UIElementButtonLabel)UIManager.CreateElement(this.m_MenuHeaderContainer, uIElementButtonDataVO.PrefabKey, uIElementButtonDataVO);
		uIElementButtonLabel.ForceOnEnter();
		uIElementButtonLabel.Button.interactable = false;
		this.GetDifficultyLayout();
	}

	private void ShowLoadGame(UIElementTitleMenuButton element)
	{
		this.ClearEverything_(element);
		this.m_ActiveTitleMenu = element;
		this.m_SelectionIndex = this.m_TitleMenu.IndexOf(this.m_ActiveTitleMenu);
		this.m_IsInTitleMenu = false;
		element.LabelUI.rectTransform.DOAnchorPosX(800f - element.LabelUI.renderedWidth / 2f, 0.25f).SetEase(Ease.InQuad);
		this.m_MenuContentContainer.SetActive(value: true);
		this.m_VersionLabel.gameObject.SetActive(value: false);
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		List<UIElementSaveDataVO> list2;
		list2 = new List<UIElementSaveDataVO>();
		unchecked
		{
			for (int i = 0; i < GameManager.Instance.GameData.SaveFileDirectory.Count; i++)
			{
				SaveFileData saveFileData;
				saveFileData = (SaveFileData)GameManager.Instance.GameData.SaveFileDirectory.GetValueOfIndex(i);
				if (saveFileData == null)
				{
					continue;
				}
				string path;
				path = Path.Combine(path2: ((i == 0) ? "autosave" : ("newgame_" + saveFileData.ID)) + ".png", path1: Path.Combine(Application.persistentDataPath, "images"));
				if (File.Exists(path))
				{
					byte[] data;
					data = File.ReadAllBytes(path);
					Texture2D texture2D;
					texture2D = new Texture2D(2, 2);
					texture2D.LoadImage(data);
					string key;
					key = TextUtility.GetKey(saveFileData.CurrentZone);
					if (saveFileData.ID == 0)
					{
						key = TextUtility.GetKey("MENU_SETTINGS_LOAD_GAME_AUTOSAVE");
					}
					UIElementSaveDataVO uIElementSaveDataVO;
					uIElementSaveDataVO = new UIElementSaveDataVO("UI/Elements/UIElementTitleMenuSave", key, saveFileData.TimeStamp, TextUtility.GetKey("MENU_SETTINGS_LOAD_GAME_DIFFICULTY") + ": " + UITitleView.GetDifficulity(saveFileData.Difficulty), texture2D);
					uIElementSaveDataVO.Callback = () =>
					{
						this.LoadSave(saveFileData.Name);
					};
					if (i == 0)
					{
						list.Insert(0, uIElementSaveDataVO);
						list.Insert(1, new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 40f));
					}
					else
					{
						list2.Add(uIElementSaveDataVO);
					}
				}
			}
			list2.Sort((UIElementSaveDataVO d1, UIElementSaveDataVO d2) => DateTime.Parse(d2.DateTimeLabel).CompareTo(DateTime.Parse(d1.DateTimeLabel)));
			for (int num = 0; num < list2.Count; num++)
			{
				list.Add(list2[num]);
			}
			list.Insert(0, new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 50f));
			list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f));
			this.m_SubMenu.Clear();
			this.m_SubSelectionIndex = 0;
			List<UIElementDataVO>.Enumerator enumerator;
			enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current.PrefabKey == string.Empty) && UIManager.CreateElement(this.m_LoadContentElementContainer, enumerator.Current.PrefabKey, enumerator.Current) is UIElementButtonLabel item)
				{
					this.m_SubMenu.Add(item);
				}
			}
		}
	}

	private static string GetDifficulity(DifficultyLevel difficultyLevel)
	{
		return difficultyLevel switch
		{
			DifficultyLevel.Normal => TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_NORMAL"), 
			DifficultyLevel.Hard => TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_HARD"), 
			_ => TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_EASY"), 
		};
	}

	private void ShowOptions(UIElementTitleMenuButton element)
	{
		this.ClearEverything_(element);
		this.m_ActiveTitleMenu = element;
		this.m_SelectionIndex = this.m_TitleMenu.IndexOf(this.m_ActiveTitleMenu);
		this.m_IsInTitleMenu = false;
		this.m_VersionLabel.gameObject.SetActive(value: true);
		this.m_NavigationNavContainer.gameObject.SetActive(value: true);
		element.LabelUI.rectTransform.DOAnchorPosX(800f - element.LabelUI.renderedWidth / 2f, 0.25f).SetEase(Ease.InQuad);
		this.m_MenuContentContainer.SetActive(value: true);
		if (this.m_OptionsMenu.Count <= 0)
		{
			UIElementButtonDataVO uIElementButtonDataVO;
			uIElementButtonDataVO = new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuTab", TextUtility.GetKey("MENU_OPTIONS_AUDIO").ToUpper());
			UIElementButtonLabel uIElementButtonLabel;
			uIElementButtonLabel = (UIElementButtonLabel)UIManager.CreateElement(this.m_MenuHeaderContainer, uIElementButtonDataVO.PrefabKey, uIElementButtonDataVO);
			uIElementButtonLabel.OnClick -= this.HandleMenuHeaderOnClick;
			uIElementButtonLabel.OnClick += this.HandleMenuHeaderOnClick;
			this.m_OptionsMenu.Add(uIElementButtonLabel);
			uIElementButtonDataVO = new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuTab", TextUtility.GetKey("MENU_OPTIONS_GRAPHICS").ToUpper());
			uIElementButtonLabel = (UIElementButtonLabel)UIManager.CreateElement(this.m_MenuHeaderContainer, uIElementButtonDataVO.PrefabKey, uIElementButtonDataVO);
			uIElementButtonLabel.OnClick -= this.HandleMenuHeaderOnClick;
			uIElementButtonLabel.OnClick += this.HandleMenuHeaderOnClick;
			this.m_OptionsMenu.Add(uIElementButtonLabel);
			uIElementButtonDataVO = new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuTab", TextUtility.GetKey("MENU_OPTIONS_CONTROLS").ToUpper());
			uIElementButtonLabel = (UIElementButtonLabel)UIManager.CreateElement(this.m_MenuHeaderContainer, uIElementButtonDataVO.PrefabKey, uIElementButtonDataVO);
			uIElementButtonLabel.OnClick -= this.HandleMenuHeaderOnClick;
			uIElementButtonLabel.OnClick += this.HandleMenuHeaderOnClick;
			this.m_OptionsMenu.Add(uIElementButtonLabel);
		}
		this.GetOptionsLayout();
	}

	private void HandleMenuHeaderOnClick(object sender, EventArgs e)
	{
		this.ShowOptionsTab(sender as UIElementButtonLabel);
	}

	private void ShowOptionsTab(UIElementButtonLabel element)
	{
		this.m_OptionsIndex = this.m_OptionsMenu.IndexOf(element);
		this.ClearLoadContentElementContainer();
		this.GetOptionsLayout();
		this.m_LoadScrollRect.verticalScrollbar.value = 1f;
	}

	private void GetOptionsLayout()
	{
		for (int i = 0; i < this.m_OptionsMenu.Count; i = unchecked(i + 1))
		{
			UIElementButtonLabel uIElementButtonLabel;
			uIElementButtonLabel = this.m_OptionsMenu[i];
			if (i == this.m_OptionsIndex)
			{
				uIElementButtonLabel.ForceOnEnter();
				continue;
			}
			uIElementButtonLabel.ForceOnExit();
			uIElementButtonLabel.ResetListeners();
		}
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		if (this.m_OptionsIndex == 0)
		{
			list = UILayouts.GetAudioLayout();
		}
		else if (this.m_OptionsIndex == 1)
		{
			list = UILayouts.GetGraphicsLayout();
		}
		else if (this.m_OptionsIndex == 2)
		{
			list = UILayouts.GetControlsLayout();
		}
		this.m_ActiveSubOption = null;
		this.m_OptionsSubMenu.Clear();
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!(enumerator.Current.PrefabKey == string.Empty))
			{
				UIElement uIElement;
				uIElement = UIManager.CreateElement(this.m_LoadContentElementContainer, enumerator.Current.PrefabKey, enumerator.Current);
				if (uIElement is UIElementSlider item)
				{
					this.m_OptionsSubMenu.Add(item);
				}
				else if (uIElement is UIElementToggle item2)
				{
					this.m_OptionsSubMenu.Add(item2);
				}
				else if (uIElement is UIElementSelector item3)
				{
					this.m_OptionsSubMenu.Add(item3);
				}
			}
		}
	}

	private void GetDifficultyLayout()
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>
		{
			new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_EASY").ToUpper(), () =>
			{
				this.NewGamePrompt(DifficultyLevel.Easy);
			}),
			new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_NORMAL").ToUpper(), () =>
			{
				this.NewGamePrompt(DifficultyLevel.Normal);
			}),
			new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_HARD").ToUpper(), () =>
			{
				this.NewGamePrompt(DifficultyLevel.Hard);
			})
		};
		this.m_SubMenu.Clear();
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!(enumerator.Current.PrefabKey == string.Empty) && UIManager.CreateElement(this.m_DifficultyElementContainer, enumerator.Current.PrefabKey, enumerator.Current) is UIElementButtonLabel uIElementButtonLabel)
			{
				this.m_SubMenu.Add(uIElementButtonLabel);
				uIElementButtonLabel.OnEnter -= this.HandleDifficultySelectionOnEnter;
				uIElementButtonLabel.OnEnter += this.HandleDifficultySelectionOnEnter;
				uIElementButtonLabel.OnExit -= this.HandleDifficultySelectionOnExit;
				uIElementButtonLabel.OnExit += this.HandleDifficultySelectionOnExit;
			}
		}
		UIElementLabelDataVO uIElementLabelDataVO;
		uIElementLabelDataVO = new UIElementLabelDataVO("UI/Elements/UIElementTitleMenuLabel");
		this.m_DifficultyDescriptionLabel = UIManager.CreateElement(this.m_DifficultyDescriptionContainer, uIElementLabelDataVO.PrefabKey, uIElementLabelDataVO) as UIElementLabel;
		Vector2 sizeDelta;
		sizeDelta = this.m_DifficultyDescriptionLabel.rectTransform.sizeDelta;
		sizeDelta.x = this.m_DifficultyDescriptionLabel.Label.GetPreferredValues().x;
		sizeDelta.y = 400f;
		this.m_DifficultyDescriptionLabel.rectTransform.sizeDelta = sizeDelta;
		this.m_DifficultyDescriptionLabel.Label.color = Color.white;
		this.m_DifficultyDescriptionLabel.Label.fontSizeMax = 50f;
	}

	private void HandleDifficultySelectionOnEnter(object sender, EventArgs e)
	{
		this.ShowDifficultyDescription(sender as UIElementButtonLabel);
	}

	private void ShowDifficultyDescription(UIElementButtonLabel button)
	{
		if (button.Label == TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_EASY").ToUpper())
		{
			this.m_DifficultyDescriptionLabel.Label.text = TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_EASY_DESCRIPTION").ToUpper();
		}
		else if (button.Label == TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_NORMAL").ToUpper())
		{
			this.m_DifficultyDescriptionLabel.Label.text = TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_NORMAL_DESCRIPTION").ToUpper();
		}
		else if (button.Label == TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_HARD").ToUpper())
		{
			this.m_DifficultyDescriptionLabel.Label.text = TextUtility.GetKey("MENU_NEW_GAME_DIFFICULTY_HARD_DESCRIPTION").ToUpper();
		}
	}

	private void HandleDifficultySelectionOnExit(object sender, EventArgs e)
	{
		this.m_DifficultyDescriptionLabel.Label.text = "";
	}

	private void NewGamePrompt(DifficultyLevel difficulty)
	{
		this.m_HasLoaded = false;
		if (GameManager.Instance.GameData.SaveFileDirectory.Count > 0)
		{
			GameManager.Instance.UIManager.Show<UIPrompt>("UI/GameMenuPrompt/UITitlePrompt", "PROMPT", new UIPromptDataVO(TextUtility.GetKey("MENU_NEW_GAME_START_HEADER").ToUpper(), TextUtility.GetKey("MENU_NEW_GAME_START_SUB_HEADER").ToUpper(), new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuButtonControllerLabel", TextUtility.GetKey("MENU_YES").ToUpper(), () =>
			{
				this.NewGame(difficulty);
			}, "", TextAlignmentOptions.MidlineLeft, InputControlType.Action1), new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuButtonControllerLabel", TextUtility.GetKey("MENU_NO").ToUpper(), this.NewGameCancel, "", TextAlignmentOptions.MidlineLeft, InputControlType.Action2)));
		}
		else
		{
			this.NewGame(difficulty);
		}
	}

	private void NewGame(DifficultyLevel difficulty)
	{
		GameManager.Instance.UIManager.Clear("PROMPT");
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		this.m_BlackOverlay.gameObject.SetActive(value: true);
		this.m_BlackOverlay.DOKill();
		this.m_BlackOverlay.DOFade(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
		{
			GameManager.Instance.LoadSection("Game", () =>
			{
				GameManager.Instance.GameData.NewSave(0, difficulty);
				GameManager.Instance.SectionManager.OnLoaded -= this.HandleSectionOnLoaded;
				GameManager.Instance.SectionManager.OnLoaded += this.HandleSectionOnLoaded;
				GameManager.Instance.SectionManager.LoadSectionAsync(SectionID.Section_S101_JoeyDrewApartment);
			});
		});
	}

	private void HandleSectionOnLoaded(object sender, EventArgs e)
	{
		GameManager.Instance.SectionManager.OnLoaded -= this.HandleSectionOnLoaded;
		GameManager.Instance.GameController?.Initialize();
		base.Dispose();
	}

	private void NewGameCancel()
	{
		this.m_HasLoaded = true;
		GameManager.Instance.UIManager.Clear("PROMPT");
		if (!GameManager.Instance.HasController)
		{
			for (int i = 0; i < this.m_SubMenu.Count; i = unchecked(i + 1))
			{
				UIElementButtonLabel uIElementButtonLabel;
				uIElementButtonLabel = this.m_SubMenu[i];
				uIElementButtonLabel.ForceOnExit();
				uIElementButtonLabel.ResetListeners();
			}
		}
	}

	private void ShowArchives(UIElementTitleMenuButton element)
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		GameManager.Instance.ShowAsyncLoader();
		this.m_BlackOverlay.gameObject.SetActive(value: true);
		this.m_BlackOverlay.DOKill();
		this.m_BlackOverlay.DOFade(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
		{
			GameManager.Instance.LoadSection("Game", () =>
			{
				GameManager.Instance.GameData.SetCurrentSave(new SaveData(-1, DifficultyLevel.Easy));
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				GameManager.Instance.SectionManager.OnLoaded -= this.HandleArchivesOnLoaded;
				GameManager.Instance.SectionManager.OnLoaded += this.HandleArchivesOnLoaded;
				GameManager.Instance.SectionManager.LoadSectionAsync(SectionID.Section_S133_Archives);
			});
		});
	}

	private void HandleArchivesOnLoaded(object sender, EventArgs e)
	{
		GameManager.Instance.SectionManager.OnLoaded -= this.HandleArchivesOnLoaded;
		GameManager.Instance.CompleteAsyncLoader();
		base.Dispose();
	}

	private void ShowCredits(UIElementTitleMenuButton element)
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		this.m_BlackOverlay.gameObject.SetActive(value: true);
		this.m_BlackOverlay.DOKill();
		this.m_BlackOverlay.DOFade(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
		{
			GameManager.Instance.LoadSection("Empty", () =>
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				GameManager.Instance.UIManager.Show<UICredits>("UI/Views/UICredits", "VIEW").SetMainMenu(isMainMenu: true);
			});
		});
	}

	private void ShowQuit(UIElementTitleMenuButton element)
	{
		this.ClearEverything_();
		this.m_ActiveTitleMenu = element;
		this.m_SelectionIndex = this.m_TitleMenu.IndexOf(this.m_ActiveTitleMenu);
		this.m_IsInTitleMenu = false;
		this.m_HasLoaded = false;
		this.m_MenuContentContainer.SetActive(value: false);
		GameManager.Instance.UIManager.Show<UIPrompt>("UI/GameMenuPrompt/UITitlePrompt", "PROMPT", new UIPromptDataVO(TextUtility.GetKey("MENU_QUIT_HEADER").ToUpper(), TextUtility.GetKey("MENU_QUIT_SUB_HEADER").ToUpper(), new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuButtonControllerLabel", TextUtility.GetKey("MENU_YES").ToUpper(), GameManager.Instance.Quit, "", TextAlignmentOptions.MidlineLeft, InputControlType.Action1), new UIElementButtonDataVO("UI/Elements/UIElementTitleMenuButtonControllerLabel", TextUtility.GetKey("MENU_NO").ToUpper(), this.QuitCancel, "", TextAlignmentOptions.MidlineLeft, InputControlType.Action2)));
	}

	private void QuitCancel()
	{
		GameManager.Instance.UIManager.Clear("PROMPT");
		this.m_HasLoaded = true;
		this.m_IsInTitleMenu = true;
		if (!PlayerInput.CheckController())
		{
			this.m_ActiveTitleMenu.ForceOnExit();
			this.m_ActiveTitleMenu.Button.selected = false;
			this.m_ActiveTitleMenu.Button.interactable = true;
		}
		else
		{
			this.ShowSelection(this.m_ActiveTitleMenu);
		}
	}

	private void ClearLoadContentElementContainer()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_LoadContentElementContainer.GetComponentsInChildren<UIElement>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].Dispose();
		}
	}

	private void ClearDifficultyElementContainer()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_DifficultyContainer.GetComponentsInChildren<UIElement>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].Dispose();
		}
		this.m_DifficultyContainer.gameObject.SetActive(value: false);
	}

	private void ClearMenuHeaderContainer()
	{
		UIElement[] componentsInChildren;
		componentsInChildren = this.m_MenuHeaderContainer.GetComponentsInChildren<UIElement>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].Dispose();
		}
		this.m_OptionsMenu.Clear();
		this.m_OptionsIndex = 0;
	}

	private void LoadSave(string fileName)
	{
		this.m_HasLoaded = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		this.m_BlackOverlay.gameObject.SetActive(value: true);
		this.m_BlackOverlay.DOKill();
		this.m_BlackOverlay.DOFade(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
		{
			GameManager.Instance.SetPause(isPaused: true);
			GameManager.Instance.UnlockPause();
			GameManager.Instance.TestLoadSave(fileName);
		});
	}

	protected override void OnDisposed()
	{
		this.m_Sequence?.Kill();
		this.m_Sequence = null;
		this.m_LoadScrollRect?.DOKill();
		this.m_TitleMenu?.Clear();
		this.m_TitleMenu = null;
		this.m_SubMenu?.Clear();
		this.m_SubMenu = null;
		this.m_OptionsMenu?.Clear();
		this.m_OptionsMenu = null;
		this.m_OptionsSubMenu?.Clear();
		this.m_OptionsSubMenu = null;
		this.m_StartGameButton = null;
		this.m_ActiveTitleMenu = null;
		this.m_ActiveSubMenu = null;
		this.m_DifficultyDescriptionLabel = null;
		this.m_ActiveSubOption = null;
		this.m_StartGameButton = null;
		base.OnDisposed();
	}
}
