using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using InControl;
using S13Audio;
using S13Audio.BATDR;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : JDisposable
{
	public bool isGameLoaded;

	public bool bAsyncFileIOInProgress;

	public JAudioListener AudioListener;

	public GameDataManager GameDataManager;

	public PlayerPreferences PlayerSettings;

	public AssetManager AssetManager;

	public InControlManager InControlManager;

	public S13Manager AudioManager;

	public UIManager UIManager;

	public PoolingManager PoolingManager;

	public SectionManager SectionManager;

	public RespawnManager RespawnManager;

	public AchievementManager AchievementManager;

	public SteamManager SteamManager;

	public DummyManager DummyManager;

	public InkDemonManager InkDemonManager;

	public GameController GameController;

	public GameData GameData;

	public GameCamera GameCamera;

	public Player Player;

	public CharacterControllable BeastBendy;

	public Companion Bendy;

	private AsyncLoaderController m_AsyncLoader;

	private UIBlocker m_UIBlocker;

	private UIChapterTitle m_UIChapterTitle;

	private UIObjective m_UIObjective;

	private UIHUD m_UIHUD;

	private UIHitBorder m_UIHitBorder;

	private UINavigation m_UINavigation;

	private UIAudioLog m_UIAudioLog;

	private UIInspect m_UIInspect;

	private UIInteractionInvalid m_UIInteractionInvalid;

	private UIInteraction m_UIInteraction;

	private UICrosshair m_UICrosshair;

	private UITeleport m_UITeleport;

	private UISubtitles m_UISubtitles;

	private UICutsceneBars m_UICutsceneBars;

	private UIInfoPopup m_UIInfoPopup;

	private UIUpgrades m_UIUpgrades;

	private UIBook m_UIBook;

	private UIMemo m_UIMemo;

	private UINotificationText m_UINotificationText;

	private UINotificationBox m_UINotificationBox;

	private GentCardExchange m_GentExchange;

	private GentUpgrades m_GentUpgrades;

	private UIGameOver m_UIGameOver;

	private JCoroutine m_AudioLogCoroutine;

	private Action m_BlockerOnShow;

	private Action m_BlockerOnHide;

	private bool m_IsGameMenu;

	private static ControllerRumble controllerRumbleManager;

	private ControllerInputMappingGraphics m_ControllerInput;

	private static GameManager m_Instance;

	public AudioLogClipGroup AudioLogClipGroup { get; private set; }

	public GameState GameState { get; private set; }

	public bool IsPauseReady { get; private set; }

	public bool IsPaused { get; private set; }

	public bool HasController => PlayerInput.HasController;

	public bool IsRealWorld
	{
		get
		{
			if (this.GameData == null || this.GameData.CurrentSave == null)
			{
				return true;
			}
			return this.GameData.CurrentSave.CursorState != 414;
		}
	}

	public ControllerInputMappingGraphics ControllerInput
	{
		get
		{
			if (this.m_ControllerInput == null)
			{
				this.m_ControllerInput = this.AssetManager.GetAsset<ControllerInputMappingGraphics>("MappingGraphics/ControllerInputMappingGraphics");
			}
			return this.m_ControllerInput;
		}
	}

	public static GameManager Instance
	{
		get
		{
			if (GameManager.m_Instance == null)
			{
				GameManager.m_Instance = new GameManager();
			}
			return GameManager.m_Instance;
		}
	}

	public event EventHandler OnUnpaused;

	public event EventHandler OnObjectiveComplete;

	protected GameManager()
	{
		this.IsPaused = false;
		GameManager.controllerRumbleManager = new ControllerRumble();
	}

	public void SetGameState(GameState gameState)
	{
		this.GameState = gameState;
	}

	public void CompleteObjective(ObjectiveData objective)
	{
		if (objective != null)
		{
			this.OnObjectiveComplete.Send(this);
		}
	}

	public void CompleteSection()
	{
		this.OnObjectiveComplete.Send(this);
	}

	public void ShowScreenBlocker(float duration = 0.5f, float delay = 0f, Action onComplete = null, string uiLayer = "BLOCKER", bool isIndependent = false, bool isWhite = false)
	{
		this.CheckScreenBlocker(uiLayer, isIndependent);
		this.InternalShowScreenBlocker(duration, delay, isWhite, onComplete);
	}

	private void InternalShowScreenBlocker(float duration, float delay, bool isWhite, Action onComplete)
	{
		this.m_BlockerOnShow = onComplete;
		if (this.m_UIBlocker.Show(duration, delay, isWhite))
		{
			this.m_UIBlocker.OnShow -= this.HandleScreenBlockerOnShown;
			this.m_UIBlocker.OnShow += this.HandleScreenBlockerOnShown;
		}
	}

	public void HideScreenBlocker(float duration = 0.5f, float delay = 0f, Action onComplete = null, string uiLayer = "BLOCKER", bool isIndependent = false, bool isWhite = false)
	{
		this.CheckScreenBlocker(uiLayer, isIndependent);
		this.InternalHideScreenBlocker(duration, delay, isWhite, onComplete);
	}

	private void InternalHideScreenBlocker(float duration, float delay, bool isWhite, Action onComplete)
	{
		this.m_BlockerOnHide = onComplete;
		if (this.m_UIBlocker.Hide(duration, delay, isWhite))
		{
			this.m_UIBlocker.OnHide -= this.HandleScreenBlockerOnHide;
			this.m_UIBlocker.OnHide += this.HandleScreenBlockerOnHide;
		}
	}

	private void HandleScreenBlockerOnShown(object sender, EventArgs e)
	{
		if (this.m_UIBlocker != null)
		{
			this.m_UIBlocker.OnShow -= this.HandleScreenBlockerOnShown;
		}
		this.m_BlockerOnShow?.Invoke();
	}

	private void HandleScreenBlockerOnHide(object sender, EventArgs e)
	{
		if (this.m_UIBlocker != null)
		{
			this.m_UIBlocker.OnHide -= this.HandleScreenBlockerOnHide;
		}
		this.m_BlockerOnHide?.Invoke();
	}

	private void CheckScreenBlocker(string uiLayer, bool isIndependent)
	{
		if (this.m_UIBlocker != null && this.m_UIBlocker.rectTransform.parent.GetComponent<UILayer>().Name != uiLayer)
		{
			this.ClearScreenBlocker();
		}
		if (this.m_UIBlocker == null)
		{
			this.m_UIBlocker = this.UIManager.Show<UIBlocker>("UI/Blocker/UIBlocker", uiLayer);
		}
		this.m_UIBlocker.SetIndependent(isIndependent);
	}

	public void ClearScreenBlocker()
	{
		if (this.m_UIBlocker != null)
		{
			this.m_BlockerOnHide = null;
			this.m_BlockerOnShow = null;
			this.m_UIBlocker.OnShow -= this.HandleScreenBlockerOnShown;
			this.m_UIBlocker.OnHide -= this.HandleScreenBlockerOnHide;
			this.m_UIBlocker.Dispose();
			this.m_UIBlocker = null;
		}
	}

	public void ShowAsyncLoader()
	{
		this.ClearAsyncLoader();
		this.m_AsyncLoader = this.UIManager.Show<AsyncLoaderController>("UI/Loaders/AsyncLoader", "ASYNC LOADER");
	}

	public void CompleteAsyncLoader()
	{
		if (this.m_AsyncLoader != null)
		{
			this.m_AsyncLoader.LoadingComplete();
		}
		this.ClearAsyncLoader();
	}

	public void ClearAsyncLoader()
	{
		if (this.m_AsyncLoader != null)
		{
			this.m_AsyncLoader.Dispose();
			this.m_AsyncLoader = null;
		}
	}

	public void ShowChapterTitle(string chapter, string title)
	{
		this.ClearChapterTitle();
		this.m_UIChapterTitle = this.UIManager.Show<UIChapterTitle>("UI/Modals/UIChapterTitle", "CHAPTER TITLE", new string[2] { chapter, title });
	}

	public void ClearChapterTitle()
	{
		if (this.m_UIChapterTitle != null)
		{
			this.m_UIChapterTitle.Dispose();
			this.m_UIChapterTitle = null;
		}
	}

	public void ShowObjective(string header, string message, bool isReal = false)
	{
		this.m_UIObjective?.Dispose();
		this.m_UIObjective = this.UIManager.Show<UIObjective>(isReal ? "UI/Objectives/UIObjectiveReal" : "UI/Objectives/UIObjective", "OBJECTIVE", new UIObjectiveDataVO(header, message));
	}

	public void ClearObjective()
	{
		if (this.m_UIObjective != null)
		{
			this.m_UIObjective.Dispose();
			this.m_UIObjective = null;
		}
	}

	private void CheckHUD()
	{
		if (this.m_UIHUD == null)
		{
			this.m_UIHUD = this.UIManager.Show<UIHUD>("UI/Views/UIHUD", "HUD");
		}
	}

	public void ShowHealthBar(float value, float max)
	{
		this.CheckHUD();
		this.m_UIHUD.SetHealthBar(value, max);
	}

	public void HideHealthBar()
	{
		this.CheckHUD();
		this.m_UIHUD.HideHealthBar();
	}

	public void ShowSprintBar(float value, float max)
	{
		this.CheckHUD();
		this.m_UIHUD.SetSprintBar(value, max);
	}

	public void RefillSprintBar(bool isFilling)
	{
		if (!(this.m_UIHUD == null))
		{
			this.m_UIHUD.SetRefill(isFilling);
		}
	}

	public void HideSprintBar()
	{
		if (this.m_UIHUD != null)
		{
			this.m_UIHUD.HideSprintBar();
		}
	}

	public void ShowStealth()
	{
		this.CheckHUD();
		this.m_UIHUD.ShowSteath();
	}

	public void HideStealth()
	{
		this.CheckHUD();
		this.m_UIHUD.HideStealth();
	}

	public void BreakStealth()
	{
		GameManager.Instance.Player.SetCanStealth(active: false);
		this.CheckHUD();
		this.m_UIHUD.BreakStealth();
	}

	public void ShowCutsceneBars(CutsceneDirector cutsceneDirector = null, bool skipPlayIn = false, bool disableSkip = false)
	{
		if (this.m_UICutsceneBars != null)
		{
			this.m_UICutsceneBars.Dispose();
		}
		this.m_UICutsceneBars = this.UIManager.Show<UICutsceneBars>("UI/Modals/UICutsceneBars", "CUTSCENE BARS", cutsceneDirector);
		if (skipPlayIn)
		{
			this.m_UICutsceneBars.SkipPlayIn();
		}
		if (disableSkip)
		{
			this.m_UICutsceneBars.DisableSkip();
		}
	}

	public void HideCutsceneBars()
	{
		if (this.m_UICutsceneBars != null)
		{
			this.m_UICutsceneBars.PlayOut();
		}
	}

	public void ClearCutsceneBars()
	{
		if (this.m_UICutsceneBars != null)
		{
			this.m_UICutsceneBars.Dispose();
			this.m_UICutsceneBars = null;
		}
	}

	public void ShowNavigation(Transform target, bool isRealWorld = false)
	{
		if (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty == DifficultyLevel.Easy || GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty == DifficultyLevel.Normal)
		{
			string assetKey;
			assetKey = (isRealWorld ? "UI/Navigation/UINavigationReal" : "UI/Navigation/UINavigation");
			if (this.m_UINavigation == null)
			{
				this.m_UINavigation = this.UIManager.Show<UINavigation>(assetKey, "NAVIGATION");
			}
			this.m_UINavigation.SetTarget(target);
		}
	}

	public void ClearNavigation()
	{
		if (this.m_UINavigation != null)
		{
			this.m_UINavigation.Clear();
			this.m_UINavigation = null;
		}
	}

	public void ShowInspect(string actionLabel, bool isReal = false)
	{
		InspectDataVO data;
		data = new InspectDataVO(actionLabel);
		string assetKey;
		assetKey = (isReal ? "UI/Prompts/UIInspect" : "UI/Prompts/UIInspect");
		if (this.m_UIInspect != null)
		{
			this.m_UIInspect.Show(data);
		}
		else
		{
			this.m_UIInspect = this.UIManager.Show<UIInspect>(assetKey, "INSPECT", data);
		}
	}

	public void ShowInspectArchives(ArchivesInspectID archivesTitleID, ArchivesInspectID archivesDescriptionID)
	{
		InspectDataVO data;
		data = new InspectDataVO(archivesTitleID, archivesDescriptionID);
		if (this.m_UIInspect != null)
		{
			this.m_UIInspect.Show(data);
		}
		else
		{
			this.m_UIInspect = this.UIManager.Show<UIInspect>("UI/Prompts/UIInspect", "INSPECT", data);
		}
	}

	public void HideInspect()
	{
		this.m_UIInspect?.Hide();
	}

	public void ClearInspect()
	{
		if (this.m_UIInspect != null)
		{
			this.m_UIInspect.Dispose();
			this.m_UIInspect = null;
		}
	}

	public void ShowInteractInvalid(string actionLabel, bool isReal = false)
	{
		InspectDataVO data;
		data = new InspectDataVO(actionLabel);
		string assetKey;
		assetKey = (isReal ? "UI/Prompts/UIInteractionInvalid" : "UI/Prompts/UIInteractionInvalid");
		if (this.m_UIInteractionInvalid != null)
		{
			this.m_UIInteractionInvalid.Show(data);
		}
		else
		{
			this.m_UIInteractionInvalid = this.UIManager.Show<UIInteractionInvalid>(assetKey, "INSPECT", data);
		}
	}

	public void HideInteractInvalid()
	{
		this.m_UIInteractionInvalid?.Hide();
	}

	public void ClearInteractInvalid()
	{
		if (this.m_UIInteractionInvalid != null)
		{
			this.m_UIInteractionInvalid.Dispose();
			this.m_UIInteractionInvalid = null;
		}
	}

	public void ShowInteraction(string actionLabel, bool isReal = false)
	{
		InteractableDataVO data;
		data = new InteractableDataVO(actionLabel);
		string assetKey;
		assetKey = (isReal ? "UI/Prompts/UIInteractionReal" : "UI/Prompts/UIInteraction");
		if (this.m_UIInteraction != null)
		{
			this.m_UIInteraction.Show(data);
		}
		else
		{
			this.m_UIInteraction = this.UIManager.Show<UIInteraction>(assetKey, "PROMPT", data);
		}
	}

	public void HideInteraction()
	{
		this.m_UIInteraction?.Hide();
	}

	public void ClearInteraction()
	{
		if (this.m_UIInteraction != null)
		{
			this.m_UIInteraction.Dispose();
			this.m_UIInteraction = null;
		}
	}

	public void SetCrosshair(bool active)
	{
		if (active)
		{
			this.ShowCrosshair(this.IsRealWorld);
		}
		else
		{
			this.ClearCrosshair();
		}
	}

	public void ShowCrosshair(bool isReal = false)
	{
		if (this.CheckCrosshair(isReal ? "UI/Crosshairs/UICrosshairReal" : "UI/Crosshairs/UICrosshair"))
		{
			this.m_UICrosshair.Show();
		}
	}

	public void HideCrosshair()
	{
		if (this.CheckCrosshair(this.IsRealWorld ? "UI/Crosshairs/UICrosshairReal" : "UI/Crosshairs/UICrosshair"))
		{
			this.m_UICrosshair.Hide();
		}
	}

	private bool CheckCrosshair(string prefab)
	{
		if (this.Player == null || !this.PlayerSettings.Crosshair)
		{
			this.ClearCrosshair();
			return false;
		}
		if (this.m_UICrosshair == null)
		{
			this.m_UICrosshair = this.UIManager.Show<UICrosshair>(prefab, "CROSSHAIR");
		}
		return true;
	}

	public void ClearCrosshair()
	{
		if (this.m_UICrosshair != null)
		{
			this.m_UICrosshair.Dispose();
			this.m_UICrosshair = null;
		}
	}

	public void ShowTeleport()
	{
		if (this.CheckTeleport())
		{
			this.m_UITeleport.Show();
		}
	}

	public void HideTeleport()
	{
		if (this.CheckTeleport())
		{
			this.m_UITeleport.Hide();
		}
	}

	public void DisplayTeleport()
	{
		if (this.CheckTeleport())
		{
			this.m_UITeleport.DisplayTeleport();
		}
	}

	public void ShowTeleportCooldown(float cooldown, float cooldownCount = 0f)
	{
		if (this.CheckTeleport())
		{
			this.m_UITeleport.DOCooldown(cooldown, cooldownCount);
		}
	}

	public void ShowTeleportCooldownComplete()
	{
		if (this.CheckTeleport())
		{
			this.m_UITeleport.DOCooldownComplete();
		}
	}

	public void UseTeleportAbility(bool use = false)
	{
		if (use && this.CheckTeleport())
		{
			this.m_UITeleport.Use();
		}
	}

	public void DisableTeleport()
	{
		if (this.CheckTeleport())
		{
			this.m_UITeleport.DisableTeleport();
		}
	}

	public void EnableTeleport()
	{
		if (this.CheckTeleport())
		{
			this.m_UITeleport.EnableTeleport();
		}
	}

	private bool CheckTeleport()
	{
		if (this.Player == null)
		{
			this.ClearTeleport();
			return false;
		}
		if (this.m_UITeleport == null)
		{
			this.m_UITeleport = this.UIManager.Show<UITeleport>("UI/Teleport/UITeleport", "TELEPORT");
		}
		return true;
	}

	public void ClearTeleport()
	{
		if (this.m_UITeleport != null)
		{
			this.m_UITeleport.Dispose();
			this.m_UITeleport = null;
		}
	}

	public void ShowNotificationText(string text, bool isInkDemon = false)
	{
		this.ClearNotificationText();
		this.m_UINotificationText = this.UIManager.Show<UINotificationText>("UI/Modals/UINotificationText", "NOTIFICATION TEXT", new UINotificationDataVO(text, isInkDemon));
	}

	public void ShowNotificationTextInput(string pressLabel, string actionLabel, string inputLabel)
	{
		this.ClearNotificationText();
		this.m_UINotificationText = this.UIManager.Show<UINotificationText>("UI/Modals/UINotificationText", "NOTIFICATION TEXT", new UINotificationDataVO(pressLabel, actionLabel, inputLabel));
	}

	public void ShowNotificationTextInput(string pressLabel, string actionLabel, InputControlType inputControlType)
	{
		this.ClearNotificationText();
		this.m_UINotificationText = this.UIManager.Show<UINotificationText>("UI/Modals/UINotificationText", "NOTIFICATION TEXT", new UINotificationDataVO(pressLabel, actionLabel, inputControlType));
	}

	public void ClearNotificationText()
	{
		if (this.m_UINotificationText != null)
		{
			this.m_UINotificationText.Dispose();
			this.m_UINotificationText = null;
		}
	}

	public void ShowNotificationBox(string label, string icon = "", int amount = 0, float delay = 0f)
	{
		if (this.m_UINotificationBox == null)
		{
			this.m_UINotificationBox = this.UIManager.Show<UINotificationBox>("UI/Modals/UINotificationBox", "NOTIFICATION BOX");
		}
		this.m_UINotificationBox.Show(label, icon, amount, delay);
	}

	public void ClearNotificationBox()
	{
		if (this.m_UINotificationBox != null)
		{
			this.m_UINotificationBox.Dispose();
			this.m_UINotificationBox = null;
		}
	}

	public void ShowSubtitles(string subtitles, float duration, bool isTrimmed = false)
	{
		if (this.PlayerSettings.Subtitles)
		{
			if (this.m_UISubtitles == null)
			{
				this.m_UISubtitles = this.UIManager.Show<UISubtitles>("UI/Subtitles/UISubtitles", "SUBTITLES");
			}
			this.m_UISubtitles.ShowSubtitles(subtitles, duration, isTrimmed);
		}
		else
		{
			this.ClearSubtitles();
		}
	}

	public void ClearSubtitles()
	{
		if (this.m_UISubtitles != null)
		{
			this.m_UISubtitles.Dispose();
			this.m_UISubtitles = null;
		}
	}

	private UIElementUpgradeLevelDataVO GetUpgradeLevelData(bool isActive)
	{
		return new UIElementUpgradeLevelDataVO(isActive);
	}

	private UIElementUpgradeDataVO GetUpgradeData(string header, string description, Sprite icon, List<UIElementUpgradeLevelDataVO> upgradeLevelData, UpgradeType upgradeType)
	{
		return new UIElementUpgradeDataVO(header, description, icon, upgradeLevelData, upgradeType);
	}

	public void ShowUpgrades()
	{
		UpgradeData asset;
		asset = this.AssetManager.GetAsset<UpgradeData>("UpgradesData/UpgradesData");
		List<UIElementUpgradeLevelDataVO> list;
		list = new List<UIElementUpgradeLevelDataVO>();
		int upgradeLevelHealth;
		upgradeLevelHealth = GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelHealth;
		list.Add(this.GetUpgradeLevelData(upgradeLevelHealth > 0));
		list.Add(this.GetUpgradeLevelData(upgradeLevelHealth > 1));
		list.Add(this.GetUpgradeLevelData(upgradeLevelHealth > 2));
		string text;
		text = TextUtility.GetKey("UI_UPGRADES_PERMANENLTY").ToUpper() + "\n";
		List<UIElementUpgradeDataVO> list2;
		list2 = new List<UIElementUpgradeDataVO>();
		list2.Add(this.GetUpgradeData(TextUtility.GetKey("UI_UPGRADES_HEALTH").ToUpper(), text + TextUtility.GetKey("UI_UPGRADES_HEALTH_DESCRIPTION").ToUpper(), asset.Health, list, UpgradeType.Health));
		List<UIElementUpgradeLevelDataVO> list3;
		list3 = new List<UIElementUpgradeLevelDataVO>();
		int upgradeLevelStamina;
		upgradeLevelStamina = GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelStamina;
		list3.Add(this.GetUpgradeLevelData(upgradeLevelStamina > 0));
		list3.Add(this.GetUpgradeLevelData(upgradeLevelStamina > 1));
		list3.Add(this.GetUpgradeLevelData(upgradeLevelStamina > 2));
		list2.Add(this.GetUpgradeData(TextUtility.GetKey("UI_UPGRADES_STAMINA").ToUpper(), text + TextUtility.GetKey("UI_UPGRADES_STAMINA_DESCRIPTION").ToUpper(), asset.Stamina, list3, UpgradeType.Stamina));
		Sprite icon;
		icon = asset.Toner;
		string header;
		header = TextUtility.GetKey("UI_UPGRADES_ABILITY").ToUpper();
		string description;
		description = text + TextUtility.GetKey("UI_UPGRADES_ABILITY_DESCRIPTION").ToUpper();
		List<UIElementUpgradeLevelDataVO> list4;
		list4 = new List<UIElementUpgradeLevelDataVO>();
		if (GameManager.Instance.Player.Abilities == null)
		{
			header = "";
			description = "";
			icon = asset.Blank;
		}
		else
		{
			int upgradeLevelAbility;
			upgradeLevelAbility = GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpgradeLevelAbility;
			list4.Add(this.GetUpgradeLevelData(upgradeLevelAbility > 0));
			list4.Add(this.GetUpgradeLevelData(upgradeLevelAbility > 1));
			list4.Add(this.GetUpgradeLevelData(upgradeLevelAbility > 2));
		}
		list2.Add(this.GetUpgradeData(header, description, icon, list4, UpgradeType.Ability));
		this.ShowUpgrades(new UIUpgradesDataVO(TextUtility.GetKey("UI_UPGRADES_SELECT").ToUpper(), TextUtility.GetKey("UI_UPGRADES_DESCRIPTION").ToUpper(), list2));
	}

	public void ShowUpgrades(UIUpgradesDataVO dataVO)
	{
		this.SoftPause();
		this.ClearUpgrades();
		this.m_UIUpgrades = this.UIManager.Show<UIUpgrades>("UI/Modals/UIUpgrades", "MODAL", dataVO);
	}

	public void HideUpgrades()
	{
		this.ClearUpgrades();
		this.SoftUnpause();
	}

	private void ClearUpgrades()
	{
		if (this.m_UIUpgrades != null)
		{
			this.m_UIUpgrades.Dispose();
			this.m_UIUpgrades = null;
		}
	}

	public void ShowInfoPopup(UIInfoPopupDataVO dataVO)
	{
		this.SoftPause();
		this.ClearInfoPopup();
		this.m_UIInfoPopup = this.UIManager.Show<UIInfoPopup>("UI/Modals/UIInfoPopup", "MODAL", dataVO);
	}

	public void HideInfoPopup()
	{
		this.ClearInfoPopup();
		this.SoftUnpause();
	}

	private void ClearInfoPopup()
	{
		if (this.m_UIInfoPopup != null)
		{
			this.m_UIInfoPopup.Dispose();
			this.m_UIInfoPopup = null;
		}
	}

	public AudioClip GetAudioLogClip(AudioLogID audioLogID)
	{
		AudioClip result;
		result = null;
		if (this.AudioLogClipGroup == null)
		{
			this.AudioLogClipGroup = Resources.Load<AudioLogClipGroup>("AudioLogs/AudioLogClipGroup");
		}
		if (this.AudioLogClipGroup != null)
		{
			for (int i = 0; i < this.AudioLogClipGroup.AudioLogs.Length; i = unchecked(i + 1))
			{
				AudioLogData audioLogData;
				audioLogData = this.AudioLogClipGroup.AudioLogs[i];
				if (audioLogData != null && audioLogData.ID == audioLogID)
				{
					result = audioLogData.AudioClip;
					break;
				}
			}
		}
		return result;
	}

	public void ShowAudioLog(string name, AudioClip audioClip)
	{
		if (this.m_UIAudioLog != null)
		{
			this.CloseAudioLog(name, audioClip);
		}
		else
		{
			this.InternalShowAudioLog(name, audioClip);
		}
	}

	private void CloseAudioLog(string name, AudioClip audioClip)
	{
		if (this.m_AudioLogCoroutine != null)
		{
			this.ClearAudioLogCoroutine();
		}
		this.m_AudioLogCoroutine = new GameObject("JCoroutine").AddComponent<JCoroutine>();
		global::UnityEngine.Object.DontDestroyOnLoad(this.m_AudioLogCoroutine);
		this.m_AudioLogCoroutine.OnComplete -= this.HandleLoadSectionCoroutineOnComplete;
		this.m_AudioLogCoroutine.OnComplete += this.HandleLoadSectionCoroutineOnComplete;
		this.m_AudioLogCoroutine.StartCoroutine(this.CloseAudioLog(), () =>
		{
			this.InternalShowAudioLog(name, audioClip);
		});
	}

	private IEnumerator CloseAudioLog()
	{
		this.KillAudioLog();
		yield return new WaitForSeconds(1f);
		yield return new WaitForEndOfFrame();
	}

	private void HandleAudioLogCoroutineOnComplete(object sender, EventArgs e)
	{
		this.ClearAudioLogCoroutine();
	}

	private void ClearAudioLogCoroutine()
	{
		if (this.m_AudioLogCoroutine != null)
		{
			this.m_AudioLogCoroutine.OnComplete -= this.HandleAudioLogCoroutineOnComplete;
			this.m_AudioLogCoroutine.Dispose();
			this.m_AudioLogCoroutine = null;
		}
	}

	private void InternalShowAudioLog(string name, AudioClip audioClip)
	{
		this.m_UIAudioLog = this.UIManager.Show<UIAudioLog>("UI/Modals/UIAudioLog", "AUDIO LOG");
		this.m_UIAudioLog.Show(name, audioClip);
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_AUDIO_LOG"));
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.AudioLogDirectory.Count <= 0)
		{
			this.ClearNotificationText();
			string text;
			text = " <color=#FEC97D>";
			string text2;
			text2 = "<color=#FFFFFF> ";
			string text3;
			text3 = TextUtility.GetKey("NOTIFICATION_AUDIO_LOG_MENU").ToUpper();
			string text4;
			text4 = TextUtility.GetKey("NOTIFICATION_MENU_01").ToUpper();
			string text5;
			text5 = TextUtility.GetKey("NOTIFICATION_MENU_02").ToUpper();
			string text6;
			text6 = TextUtility.GetKey("NOTIFICATION_MENU_03").ToUpper();
			string text7;
			text7 = TextUtility.GetKey("NOTIFICATION_MENU_04").ToUpper();
			this.ShowNotificationText(text + text3 + text2 + text4 + text + text5 + text2 + text6 + text + text7 + " ");
		}
	}

	public void KillAudioLog()
	{
		if (this.m_UIAudioLog != null)
		{
			this.m_UIAudioLog.PlayOut();
		}
	}

	public void ClearAudioLog()
	{
		if (this.m_UIAudioLog != null)
		{
			this.m_UIAudioLog.Dispose();
			this.m_UIAudioLog = null;
			BATDRAudioLogPlayer.ForceStop();
		}
	}

	public void ShowMemo(UIMemoDataVO memoDataVO)
	{
		this.SoftPause();
		this.ClearMemo();
		this.m_UIMemo = this.UIManager.Show<UIMemo>("UI/Prompts/UIMemo", "MEMO", memoDataVO);
	}

	public void HideMemo()
	{
		this.ClearMemo();
		this.Player.Interaction.ResetInteraction();
		this.ClearInteraction();
		this.SoftUnpause();
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_MEMO"));
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.Count < 2)
		{
			this.ClearNotificationText();
			string text;
			text = " <color=#FEC97D>";
			string text2;
			text2 = "<color=#FFFFFF> ";
			string text3;
			text3 = TextUtility.GetKey("NOTIFICATION_MEMO_MENU").ToUpper();
			string text4;
			text4 = TextUtility.GetKey("NOTIFICATION_MENU_01").ToUpper();
			string text5;
			text5 = TextUtility.GetKey("NOTIFICATION_MENU_02").ToUpper();
			string text6;
			text6 = TextUtility.GetKey("NOTIFICATION_MENU_03").ToUpper();
			string text7;
			text7 = TextUtility.GetKey("NOTIFICATION_MENU_04").ToUpper();
			this.ShowNotificationText(text + text3 + text2 + text4 + text + text5 + text2 + text6 + text + text7 + " ");
		}
	}

	public void ClearMemo()
	{
		if (this.m_UIMemo != null)
		{
			this.m_UIMemo.Dispose();
			this.m_UIMemo = null;
		}
	}

	public void ShowBook(UIBookDataVO dataVO)
	{
		this.SoftPause();
		this.ClearBook();
		this.m_UIBook = this.UIManager.Show<UIBook>("UI/Prompts/UIBook", "MEMO", dataVO);
	}

	public void HideBook()
	{
		this.ClearBook();
		this.Player.Interaction.ResetInteraction();
		this.ClearInteraction();
		this.SoftUnpause();
	}

	private void ClearBook()
	{
		if (this.m_UIBook != null)
		{
			this.m_UIBook.Dispose();
			this.m_UIBook = null;
		}
	}

	public void ShowGentExchange(GentCardExchange gentExchange)
	{
		this.SoftPause();
		this.HideCrosshair();
		this.m_GentExchange = gentExchange;
		this.UIManager.Show<UIGentExchange>("UI/Modals/UIGentExchange", "MODAL", this.m_GentExchange);
		if (!this.HasController)
		{
			UIManager.SetCursor(active: true);
		}
		else
		{
			UIManager.SetCursor(active: false);
		}
	}

	public void ClearGentExchange()
	{
		if (this.m_GentExchange != null)
		{
			this.m_GentExchange.Content.Enable();
		}
		this.m_GentExchange = null;
		UIManager.SetCursor(active: false);
		this.SoftUnpause();
		this.ShowCrosshair();
	}

	public void ExchangeGentExchange()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveBatteryCasing(GentCardExchanteRequirements.GetBatteryCasings());
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveToolkits(GentCardExchanteRequirements.GetToolkits());
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveParts(GentCardExchanteRequirements.GetParts());
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_CARD"), "Icon/Collectables/Small/UIIcon_Card", 1);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddCard();
	}

	public void ShowGentUpgrades(GentUpgrades gentUpgrade)
	{
		this.SoftPause();
		this.HideCrosshair();
		if (this.Player.HasTeleport())
		{
			this.HideTeleport();
		}
		this.m_GentUpgrades = gentUpgrade;
		this.UIManager.Show<UIGentUpgrades>("UI/Modals/UIGentUpgrades", "MODAL", this.m_GentUpgrades);
		if (!this.HasController)
		{
			UIManager.SetCursor(active: true);
		}
		else
		{
			UIManager.SetCursor(active: false);
		}
	}

	public void ClearGentUpgrades()
	{
		if (this.m_GentUpgrades != null)
		{
			this.m_GentUpgrades.Close();
		}
		this.m_GentUpgrades = null;
		UIManager.SetCursor(active: false);
		this.SoftUnpause();
		this.ShowCrosshair();
		if (this.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active && this.Player.HasTeleport())
		{
			GameManager.Instance.DisplayTeleport();
		}
	}

	public void UpgradeGentUpgrade()
	{
		this.SoftUnpause();
		this.m_GentUpgrades.Upgrade();
		UIManager.SetCursor(active: false);
	}

	public void ShowGameOver(GameOverType gameOverType)
	{
		this.UIManager.Clear();
		UIGameOverDataVO data;
		data = new UIGameOverDataVO(gameOverType);
		this.m_UIGameOver = this.UIManager.Show<UIGameOver>("UI/GameOver/UIGameOver", "VIEW", data);
	}

	public void ClearGameOver()
	{
		if (this.m_UIGameOver != null)
		{
			this.m_UIGameOver.Dispose();
			this.m_UIGameOver = null;
		}
	}

	public void SoftPause()
	{
		this.SetGameState(GameState.UI);
		this.LockPause();
		this.SetPause(isPaused: true);
		this.PauseGame();
	}

	public void SoftUnpause()
	{
		this.SetGameState(GameState.PLAY);
		this.SetPause(isPaused: false);
		this.UnpauseGame();
		this.UnlockPause();
	}

	public void LockPause()
	{
		this.IsPauseReady = false;
	}

	public void UnlockPause()
	{
		this.IsPauseReady = true;
	}

	public void PauseGame()
	{
		Time.timeScale = 0f;
		S13Manager.PauseGame();
		List<Tween> list;
		list = DOTween.PlayingTweens();
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i = unchecked(i + 1))
		{
			Tween tween;
			tween = list[i];
			if (tween != null)
			{
				bool flag;
				flag = true;
				if (tween.target is GameObject gameObject)
				{
					flag = gameObject.activeInHierarchy;
				}
				if (flag && tween.playedOnce)
				{
					tween.Pause();
				}
			}
		}
	}

	public void UnpauseGame()
	{
		Time.timeScale = 1f;
		S13Manager.ResumeGame();
		List<Tween> list;
		list = DOTween.PausedTweens();
		if (list != null)
		{
			for (int i = 0; i < list.Count; i = unchecked(i + 1))
			{
				Tween tween;
				tween = list[i];
				if (tween != null)
				{
					bool flag;
					flag = true;
					if (tween.target is GameObject gameObject)
					{
						flag = gameObject.activeInHierarchy;
					}
					if (flag && tween.playedOnce)
					{
						tween.Play();
					}
				}
			}
		}
		this.OnUnpaused.Send(this);
	}

	public void Pause()
	{
		if (!this.IsPaused)
		{
			this.ForcedPause();
		}
		else
		{
			this.ForcedUnpause();
		}
	}

	public void ForcedPause()
	{
		this.PauseGame();
		this.GameData.CurrentSave?.PrepareSave();
		this.SetGameState(GameState.PAUSE);
		this.IsPaused = true;
		InputManager.ActiveDevice.Vibrate(0f);
	}

	public void ForcedUnpause()
	{
		this.UIManager.Clear("VIEW", "GAME MENU PROMPT");
		UIManager.SetCursor(active: false);
		this.SetGameState(GameState.PLAY);
		this.IsPaused = false;
		this.UnpauseGame();
	}

	public void SetPause(bool isPaused)
	{
		this.IsPaused = isPaused;
	}

	private void HandlePauseGameOnPlayOutComplete(object sender, EventArgs e)
	{
		(sender as UIController).OnPlayOutComplete -= this.HandlePauseGameOnPlayOutComplete;
		this.IsPauseReady = false;
		this.UIManager.Clear();
		UIManager.SetCursor(active: false);
		this.IsPauseReady = true;
		this.IsPaused = false;
		this.UnpauseGame();
	}

	public void Unpause()
	{
		if (this.IsPaused)
		{
			this.IsPauseReady = false;
			this.UIManager.Clear();
			UIManager.SetCursor(active: false);
			this.IsPauseReady = true;
			this.IsPaused = false;
			this.UnpauseGame();
		}
	}

	public void GameMenu()
	{
		if (!this.m_IsGameMenu)
		{
			UIManager.SetCursor(active: true);
			this.m_IsGameMenu = true;
			this.IsPauseReady = false;
			this.IsPaused = true;
		}
		else
		{
			this.m_IsGameMenu = false;
			UIManager.SetCursor(active: false);
			this.IsPauseReady = true;
			this.IsPaused = false;
		}
	}

	public void ShowCursor()
	{
		if (this.HasController)
		{
			this.HideCursor();
		}
		else
		{
			this.SetCursor(visible: true);
		}
	}

	public void HideCursor()
	{
		this.SetCursor(visible: false);
	}

	public void SetCursor(bool visible)
	{
		UIManager.SetCursor(visible);
	}

	public void ClearPlayer()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Load);
			GameManager.Instance.Player.Dispose();
			GameManager.Instance.Player = null;
		}
	}

	public void ClearCharacters()
	{
		if (this.BeastBendy != null)
		{
			this.BeastBendy.Dispose();
			this.BeastBendy = null;
		}
		if (this.Bendy != null)
		{
			this.Bendy.Dispose();
			this.Bendy = null;
		}
	}

	public void ClearUI()
	{
		GameManager.Instance.ClearInteraction();
		GameManager.Instance.ClearInteractInvalid();
		GameManager.Instance.ClearNotificationText();
		GameManager.Instance.ClearNotificationBox();
		GameManager.Instance.ClearCrosshair();
		GameManager.Instance.ClearAudioLog();
		GameManager.Instance.ClearCutsceneBars();
		GameManager.Instance.ClearObjective();
		GameManager.Instance.ClearCrosshair();
		GameManager.Instance.ClearSubtitles();
		GameManager.Instance.ClearTeleport();
		GameManager.Instance.ClearChapterTitle();
	}

	public void TestLoadSave(string fileName)
	{
		if (!GameManager.Instance.GameData.GetSaveFile(fileName, out var saveFileData))
		{
			Debug.LogError("Unable to load save file " + fileName + ".");
			return;
		}
		GameManager.Instance.GameData.GetSave(saveFileData.ID);
		this.LoadSaveGameFromFileData(saveFileData);
	}

	public void LoadSaveGameFromFileData(SaveFileData saveFileData)
	{
		if (this.m_AsyncLoader != null)
		{
			this.m_AsyncLoader.Dispose();
			this.m_AsyncLoader = null;
		}
		this.m_AsyncLoader = this.UIManager.Show<AsyncLoaderController>("UI/Loaders/AsyncLoader", "ASYNC LOADER");
		this.ClearUI();
		GameManager.Instance.SectionManager?.Clear();
		GameManager.Instance.RespawnManager?.Clear();
		GameManager.Instance.InkDemonManager?.Clear();
		GameManager.Instance.GameController?.Dispose();
		this.GameController = null;
		GameManager.Instance.ShowScreenBlocker(0.5f, 0.1f, () =>
		{
			if (S13Manager.Instance != null)
			{
				S13Manager.StopAll(ignoreFades: true);
			}
			this.ForcedUnpause();
			this.LockPause();
			this.UIManager.ClearGameMenu();
			this.LoadSection("Game", () =>
			{
				this.LoadSectionData(saveFileData);
			});
		}, "GAME MENU SCREEN BLOCKER", isIndependent: true);
	}

	public void LoadSectionData(SaveFileData saveFileData)
	{
		JCoroutine jCoroutine;
		jCoroutine = new GameObject("JCoroutine - LoadSectionData - (" + saveFileData.Name + ")").AddComponent<JCoroutine>();
		global::UnityEngine.Object.DontDestroyOnLoad(jCoroutine);
		jCoroutine.OnComplete -= this.HandleLoadSectionCoroutineOnComplete;
		jCoroutine.OnComplete += this.HandleLoadSectionCoroutineOnComplete;
		jCoroutine.StartCoroutine(this.InternalLoadSectionData(saveFileData), null);
	}

	private IEnumerator InternalLoadSectionData(SaveFileData saveFileData)
	{
		while (GameManager.Instance.bAsyncFileIOInProgress)
		{
			yield return null;
		}
		this.ClearCharacters();
		this.ClearPlayer();
		string assetKey;
		assetKey = "Characters/Player/Player_Audrey";
		if (GameManager.Instance.IsRealWorld)
		{
			assetKey = "Characters/Player/Player_Audrey_RealWorld";
		}
		GameManager.Instance.Player = GameManager.Instance.AssetManager.CreateAsset<Player>(assetKey);
		GameManager.Instance.Player.transform.position = new Vector3(0f, 2400f, 0f);
		GameManager.Instance.Player.Initialize();
		GameManager.Instance.Player.SetSectionID(GameManager.Instance.GameData.CurrentSaveFile.CurrentSectionID);
		GameManager.Instance.Player.SetZone(GameManager.Instance.GameData.CurrentSaveFile.CurrentZone);
		GameManager.Instance.Player.SetInitialLocation(GameManager.Instance.GameData.CurrentSave.PlayerData.Transform);
		GameManager.Instance.Player.SetCameraPivotLerp(active: false);
		GameManager.Instance.Player.SetInteraction(active: false);
		if (!GameManager.Instance.IsRealWorld)
		{
			GameManager.Instance.Player.SetHealth(GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Health);
			if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.IsCrouched)
			{
				GameManager.Instance.Player.ForceCrouch();
			}
			if (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData != null && GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.WeaponStatus == WeaponStatus.Active && GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID != WeaponType.NONE)
			{
				GameManager.Instance.Player.EquipWeapon(new WeaponData(GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID));
			}
			if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory != null)
			{
				foreach (AbilityDataObject value in GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.Values)
				{
					GameManager.Instance.Player.SetAbility(value.ID);
				}
				if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Inactive)
				{
					GameManager.Instance.Player.LockAbilities();
					if (GameManager.Instance.Player.HasTeleport())
					{
						GameManager.Instance.HideTeleport();
					}
				}
				else if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active)
				{
					GameManager.Instance.Player.UnlockAbilities();
					if (GameManager.Instance.Player.Abilities != null)
					{
						foreach (PlayerAbilityState ability in GameManager.Instance.Player.Abilities)
						{
							ability?.ForceSetCooldown(ability._CooldownCount);
						}
					}
					if (GameManager.Instance.Player.HasTeleport())
					{
						GameManager.Instance.EnableTeleport();
						GameManager.Instance.DisplayTeleport();
					}
				}
			}
		}
		GameManager.Instance.Player.gameObject.SetActive(value: false);
		global::UnityEngine.Object.DontDestroyOnLoad(GameManager.Instance.Player);
		Time.timeScale = 0f;
		this.SectionManager.LoadSectionsAsync(GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.ActiveSections.ToArray(), GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.InitializedSections.ToArray(), () =>
		{
			Time.timeScale = 1f;
			GameManager.Instance.GameController?.Initialize();
			GameManager.Instance.Player.gameObject.SetActive(value: true);
			GameManager.Instance.Player.SetCombatStatus(GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.CombatStatus);
			if (!GameManager.Instance.IsRealWorld)
			{
				if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.IsCrouched)
				{
					GameManager.Instance.Player.ForceCrouch();
				}
				GameManager.Instance.Player.ShowWeapon();
			}
			GameManager.Instance.HideScreenBlocker(0.5f, 0.15f, () =>
			{
				GameManager.Instance.Player.SetCameraPivotLerp(active: true);
				GameManager.Instance.Player.SetInteraction(active: true);
				GameManager.Instance.ShowCrosshair(GameManager.Instance.IsRealWorld);
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_GAME_LOADED"));
				GameManager.Instance.SetGameState(GameState.PLAY);
				GameManager.Instance.UnlockPause();
			}, "GAME MENU SCREEN BLOCKER", isIndependent: true);
			if (this.m_AsyncLoader != null)
			{
				this.m_AsyncLoader.LoadingComplete();
				this.m_AsyncLoader.Dispose();
				this.m_AsyncLoader = null;
			}
		});
	}

	public void LoadSection(string scene, Action callback = null)
	{
		JCoroutine jCoroutine;
		jCoroutine = new GameObject("JCoroutine - LoadSection - (" + scene + ")").AddComponent<JCoroutine>();
		global::UnityEngine.Object.DontDestroyOnLoad(jCoroutine);
		jCoroutine.OnComplete -= this.HandleLoadSectionCoroutineOnComplete;
		jCoroutine.OnComplete += this.HandleLoadSectionCoroutineOnComplete;
		jCoroutine.StartCoroutine(this.LoadSection(scene), callback);
	}

	private void HandleLoadSectionCoroutineOnComplete(object sender, EventArgs e)
	{
		JCoroutine obj;
		obj = sender as JCoroutine;
		obj.OnComplete -= this.HandleLoadSectionCoroutineOnComplete;
		obj.Dispose();
	}

	private IEnumerator LoadSection(string scene)
	{
		if (GameManager.Instance.bAsyncFileIOInProgress)
		{
			Debug.Log("Waiting on async read to complete.");
		}
		while (GameManager.Instance.bAsyncFileIOInProgress)
		{
			yield return null;
		}
		AsyncOperation asyncLoad;
		asyncLoad = SceneManager.LoadSceneAsync(scene);
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}

	public string GetProjectVersion()
	{
		return VersionSettings.Instance.GetVersion();
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void Clear()
	{
		this.OnUnpaused = null;
		this.OnObjectiveComplete = null;
		this.AudioLogClipGroup = null;
		this.AudioListener?.Dispose();
		this.AudioListener = null;
		this.GameDataManager?.Dispose();
		this.GameDataManager = null;
		this.PlayerSettings?.Dispose();
		this.PlayerSettings = null;
		this.AssetManager?.Dispose();
		this.AssetManager = null;
		if (this.InControlManager != null)
		{
			global::UnityEngine.Object.Destroy(this.InControlManager);
		}
		this.InControlManager = null;
		if (this.AudioManager != null)
		{
			global::UnityEngine.Object.Destroy(this.AudioManager);
		}
		this.AudioManager = null;
		this.UIManager?.Dispose();
		this.UIManager = null;
		this.PoolingManager?.Dispose();
		this.PoolingManager = null;
		this.SectionManager?.Dispose();
		this.SectionManager = null;
		this.RespawnManager?.Dispose();
		this.RespawnManager = null;
		this.AchievementManager?.Dispose();
		this.AchievementManager = null;
		this.SteamManager?.Dispose();
		this.SteamManager = null;
		this.DummyManager?.Dispose();
		this.DummyManager = null;
		this.InkDemonManager?.Dispose();
		this.InkDemonManager = null;
		this.GameController?.Dispose();
		this.GameController = null;
		this.GameData = null;
		this.Player = null;
		this.GameCamera = null;
		this.BeastBendy = null;
		this.m_AsyncLoader = null;
		this.m_UIBlocker = null;
		this.m_UIChapterTitle = null;
		this.m_UIObjective = null;
		this.m_UIHUD = null;
		this.m_UIHitBorder = null;
		this.m_UINavigation = null;
		this.m_UIAudioLog = null;
		this.m_UIInspect = null;
		this.m_UIInteractionInvalid = null;
		this.m_UIInteraction = null;
		this.m_UICrosshair = null;
		this.m_UISubtitles = null;
		this.m_UICutsceneBars = null;
		this.m_UIInfoPopup = null;
		this.m_UIUpgrades = null;
		this.m_UIBook = null;
		this.m_UIMemo = null;
		this.m_UINotificationText = null;
		this.m_UINotificationBox = null;
		this.m_GentExchange = null;
		this.m_GentUpgrades = null;
		this.ClearGameOver();
		this.IsPaused = false;
		this.m_BlockerOnHide = null;
		this.m_BlockerOnShow = null;
	}

	protected override void OnDisposed()
	{
		this.Clear();
		base.OnDisposed();
	}

	public void TriggerRumble(float duration, float strength = 10f, bool isIndependentUpdate = false)
	{
		GameManager.controllerRumbleManager.SetVibration(duration, strength, 10, 90f, fadeOut: true, isIndependentUpdate);
	}

	public void TriggerRumble(Vector2 DurationAndStrength, bool isIndependentUpdate = false)
	{
		GameManager.controllerRumbleManager.SetVibration(DurationAndStrength, isIndependentUpdate);
	}
}
