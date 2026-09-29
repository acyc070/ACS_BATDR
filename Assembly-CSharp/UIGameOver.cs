using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOver : UIController
{
	public enum GameOverState
	{
		None = 0,
		Main = 1,
		Load = 2
	}

	[Header("Blocker")]
	[SerializeField]
	private GameObject m_RaycastBlocker;

	[Header("Images")]
	[SerializeField]
	private Image m_BlackOverlay;

	[SerializeField]
	private Image m_Background;

	[Header("Main Content")]
	[SerializeField]
	private CanvasGroup m_MainContent;

	[SerializeField]
	private CanvasGroup m_MainContentButtons;

	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_SubHeaderLabel;

	[Header("Main Content Button Container")]
	[SerializeField]
	private RectTransform m_ButtonContainer;

	[Space]
	[Header("Load Content")]
	[SerializeField]
	private CanvasGroup m_LoadContent;

	[SerializeField]
	private RectTransform m_LoadContentElementContainer;

	[SerializeField]
	private UIScrollRect m_LoadContentScrollView;

	[Header("Load Content Button Container")]
	[SerializeField]
	private RectTransform m_LoadContentButtonContainer;

	[SerializeField]
	private RectTransform m_LoadContentButtonBackController;

	[SerializeField]
	private Image m_LoadContentButtonBackControllerInput;

	[Space]
	[Header("Game Over Data")]
	[SerializeField]
	private GameOverData m_GameOverData;

	private UIGameOverDataVO m_DataVO;

	private Sequencer m_Sequencer;

	private List<UIElementButtonLabel> m_Buttons = new List<UIElementButtonLabel>();

	private UIElementButtonLabel m_ActiveButton;

	private float m_InputDelay = 0.25f;

	private float m_InputTimer;

	private int m_SelectionIndex;

	private bool m_HasLoaded;

	private bool m_IsTransitioning;

	private List<UIElementButtonLabel> m_LoadButtons = new List<UIElementButtonLabel>();

	private UIElementButtonLabel m_ActiveLoadButton;

	private int m_LoadSelectionIndex;

	private GameOverState m_GameOverState;

	public event EventHandler OnExit;

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UIGameOverDataVO)_data;
		this.LockInput();
		this.m_HeaderLabel.text = TextUtility.GetKey("MENU_GAME_OVER_HEADER").ToLower();
		this.m_SubHeaderLabel.text = TextUtility.GetKey("MENU_GAME_OVER_SUB_HEADER");
		this.m_LoadContentButtonBackControllerInput.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action2);
		this.SetGameManager();
		this.PlayDeathAudio();
		if (GameManager.Instance.GameData.SaveFileDirectory.Count > 0)
		{
			this.GetButton(TextUtility.GetKey("MENU_LOAD_AUTOSAVE").ToUpper(), this.LoadAutosaveOnClick);
		}
		if (GameManager.Instance.GameData.SaveFileDirectory.Count > 1)
		{
			this.GetLoadGameData();
			this.GetButton(TextUtility.GetKey("MENU_LOAD_GAME").ToUpper(), this.LoadGameOnClick);
			UIElementLabelDataVO uIElementLabelDataVO;
			uIElementLabelDataVO = new UIElementLabelDataVO("");
			uIElementLabelDataVO.Label = TextUtility.GetKey("MENU_BACK").ToUpper();
			uIElementLabelDataVO.FontSize = 45f;
			uIElementLabelDataVO.Color = UIColors.ACTIVE_LABEL_REAL;
			UIElementButtonDataVO uIElementButtonDataVO;
			uIElementButtonDataVO = new UIElementButtonDataVO("UI/GameOver/UIElementGameOverButton", uIElementLabelDataVO, this.LoadGameBackOnClick);
			UIElementButtonLabel uIElementButtonLabel;
			uIElementButtonLabel = (UIElementButtonLabel)UIManager.CreateElement(this.m_LoadContentButtonContainer, uIElementButtonDataVO.PrefabKey, uIElementButtonDataVO);
			Vector2 sizeDelta;
			sizeDelta = uIElementButtonLabel.LabelUI.rectTransform.sizeDelta;
			sizeDelta.y = 100f;
			uIElementButtonLabel.rectTransform.sizeDelta = sizeDelta;
		}
		this.GetButton(TextUtility.GetKey("MENU_SETTINGS_MAIN_MENU").ToUpper(), this.MainMenuOnClick);
		this.m_LoadContent.alpha = 0f;
		this.m_BlackOverlay.color = Color.black;
		this.m_MainContent.alpha = 1f;
		this.m_MainContentButtons.alpha = 0f;
		this.m_LoadContentScrollView.verticalScrollbar.value = 1f;
		this.m_LoadContentScrollView.verticalScrollbar.size = 0f;
		this.m_LoadContent.gameObject.SetActive(value: false);
		GameManager.Instance.SectionManager.Clear();
	}

	public override void PlayIn()
	{
		this.m_Sequencer = new Sequencer(isIndependent: true);
		this.m_Sequencer.RestartSequence();
		this.m_Sequencer.Insert(6f, this.m_BlackOverlay.DOFade(0f, 2f).SetEase(Ease.Linear));
		this.m_Sequencer.Insert(9f, this.m_MainContentButtons.DOFade(1f, 1f).SetEase(Ease.Linear));
		this.m_Sequencer.Insert(9f, () =>
		{
			this.SetCursor(active: true);
		});
		this.m_Sequencer.OnComplete(this.PlayInComplete);
	}

	public override void PlayInComplete()
	{
		if (GameManager.Instance.HasController)
		{
			this.ShowSelection(this.m_Buttons[this.m_SelectionIndex]);
		}
		this.m_BlackOverlay.enabled = false;
		this.m_HasLoaded = true;
		this.UnlockInput();
		this.m_GameOverState = GameOverState.Main;
		base.PlayInComplete();
	}

	private void Update()
	{
		if (this.m_IsTransitioning || !this.m_HasLoaded || base.IsDisposed)
		{
			return;
		}
		if (PlayerInput.CheckController())
		{
			this.m_LoadContentButtonBackController.gameObject.SetActive(value: true);
			GameManager.Instance.HideCursor();
		}
		else
		{
			this.m_LoadContentButtonBackController.gameObject.SetActive(value: false);
			GameManager.Instance.ShowCursor();
		}
		if (this.m_GameOverState == GameOverState.None)
		{
			return;
		}
		float num;
		num = PlayerInput.ControllerY();
		bool flag;
		flag = PlayerInput.ControllerAction1();
		bool cancel;
		cancel = PlayerInput.ControllerAction2();
		if (this.m_InputTimer >= this.m_InputDelay)
		{
			this.GetMenuInput(num, flag, cancel);
			if (num != 0f || PlayerInput.MoveXRaw() != 0f)
			{
				this.m_InputTimer = 0f;
			}
		}
		else
		{
			this.m_InputTimer += Time.unscaledDeltaTime;
		}
	}

	private void GetMenuInput(float input, bool select, bool cancel)
	{
		unchecked
		{
			if (cancel)
			{
				this.GetCancel();
			}
			else if (select)
			{
				this.GetSelection();
			}
			else if (input > 0.1f)
			{
				if (this.m_GameOverState == GameOverState.Main)
				{
					if (this.m_SelectionIndex > 0)
					{
						this.m_SelectionIndex--;
						this.ShowSelection(this.m_Buttons[this.m_SelectionIndex]);
					}
				}
				else if (this.m_GameOverState == GameOverState.Load && this.m_LoadSelectionIndex > 0)
				{
					this.m_LoadSelectionIndex--;
					this.ShowLoadSelection(this.m_LoadButtons[this.m_LoadSelectionIndex]);
				}
			}
			else
			{
				if (!(input < -0.1f))
				{
					return;
				}
				if (this.m_GameOverState == GameOverState.Main)
				{
					if (this.m_SelectionIndex < this.m_Buttons.Count - 1)
					{
						this.m_SelectionIndex++;
						this.ShowSelection(this.m_Buttons[this.m_SelectionIndex]);
					}
				}
				else if (this.m_GameOverState == GameOverState.Load && this.m_LoadSelectionIndex < this.m_LoadButtons.Count - 1)
				{
					this.m_LoadSelectionIndex++;
					this.ShowLoadSelection(this.m_LoadButtons[this.m_LoadSelectionIndex]);
				}
			}
		}
	}

	private void GetCancel()
	{
		if (this.m_GameOverState == GameOverState.Load)
		{
			this.LoadGameBackOnClick();
		}
	}

	private void GetSelection()
	{
		if (this.m_GameOverState == GameOverState.Main)
		{
			if (this.m_ActiveButton != null)
			{
				this.m_ActiveButton.Callback?.Invoke();
			}
		}
		else if (this.m_GameOverState == GameOverState.Load && this.m_ActiveLoadButton != null)
		{
			this.m_ActiveLoadButton.Callback?.Invoke();
		}
	}

	private void ShowSelection(UIElementButtonLabel button)
	{
		if (this.m_ActiveButton != null)
		{
			this.m_ActiveButton.ForceOnExit();
			this.m_ActiveButton.Button.selected = false;
			this.m_ActiveButton.Button.interactable = true;
		}
		if (button != null)
		{
			if (this.m_Buttons.Contains(button))
			{
				this.m_SelectionIndex = this.m_Buttons.IndexOf(button);
			}
			this.m_ActiveButton = button;
			this.m_ActiveButton.Button.selected = true;
			this.m_ActiveButton.ForceOnEnter();
		}
	}

	private void ShowLoadSelection(UIElementButtonLabel button)
	{
		if (this.m_ActiveLoadButton != null)
		{
			this.m_ActiveLoadButton.ForceOnExit();
			this.m_ActiveLoadButton.Button.selected = false;
			this.m_ActiveLoadButton.Button.interactable = true;
		}
		if (!(button != null))
		{
			return;
		}
		if (this.m_LoadButtons.Contains(button))
		{
			this.m_SelectionIndex = this.m_LoadButtons.IndexOf(button);
		}
		this.m_ActiveLoadButton = button;
		this.m_ActiveLoadButton.Button.selected = true;
		this.m_ActiveLoadButton.ForceOnEnter();
		if (this.m_LoadContentScrollView.verticalScrollbar.gameObject.activeInHierarchy)
		{
			float endValue;
			endValue = 1f - (float)this.m_SelectionIndex / (float)this.m_LoadButtons.Count;
			if (this.m_SelectionIndex == 0)
			{
				endValue = 1f;
			}
			else if (this.m_SelectionIndex == unchecked(this.m_LoadButtons.Count - 1))
			{
				endValue = 0f;
			}
			this.m_LoadContentScrollView.DOKill();
			this.m_LoadContentScrollView.DOVerticalNormalizedPos(endValue, 0.15f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		}
	}

	public void LoadAutosaveOnClick()
	{
		this.ExitGameOver(this.LoadAutosaveOnClickComplete);
	}

	private void LoadAutosaveOnClickComplete()
	{
		GameManager.Instance.SetGameState(GameState.PLAY);
		SaveFileData saveFileData;
		saveFileData = (SaveFileData)GameManager.Instance.GameData.SaveFileDirectory.GetValueOfIndex(0);
		if (saveFileData != null)
		{
			GameManager.Instance.TestLoadSave(saveFileData.Name);
		}
	}

	public void LoadGameOnClick()
	{
		this.LockInput();
		this.m_Sequencer.RestartSequence();
		this.m_Sequencer.Insert(0f, this.m_MainContent.DOFade(0f, 0.5f).OnComplete(this.ShowLoadGame));
		this.m_Sequencer.Insert(0.5f, this.m_LoadContent.DOFade(1f, 0.5f));
		this.m_Sequencer.OnComplete(this.LoadGameOnClickComplete);
		this.m_LoadContentScrollView.verticalScrollbar.value = 1f;
		this.m_LoadSelectionIndex = 0;
		if (GameManager.Instance.HasController)
		{
			this.ShowLoadSelection(this.m_LoadButtons[this.m_LoadSelectionIndex]);
		}
	}

	private void ShowLoadGame()
	{
		this.m_MainContent.gameObject.SetActive(value: false);
		this.m_LoadContent.gameObject.SetActive(value: true);
	}

	private void LoadGameOnClickComplete()
	{
		this.m_GameOverState = GameOverState.Load;
		this.UnlockInput();
	}

	private void LoadGameBackOnClick()
	{
		this.LockInput();
		this.m_Sequencer.RestartSequence();
		this.m_Sequencer.Insert(0f, this.m_LoadContent.DOFade(0f, 0.5f).OnComplete(this.ShowMain));
		this.m_Sequencer.Insert(0.5f, this.m_MainContent.DOFade(1f, 0.5f));
		this.m_Sequencer.OnComplete(this.ShowMainOnComplete);
	}

	private void ShowMain()
	{
		this.m_LoadContent.gameObject.SetActive(value: false);
		this.m_MainContent.gameObject.SetActive(value: true);
	}

	private void ShowMainOnComplete()
	{
		this.m_GameOverState = GameOverState.Main;
		this.UnlockInput();
	}

	private void GetLoadGameData()
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		List<UIElementSaveDataVO> list2;
		list2 = new List<UIElementSaveDataVO>();
		unchecked
		{
			for (int i = 0; i < GameManager.Instance.GameData.SaveFileDirectory.Count; i++)
			{
				if (i == 0)
				{
					continue;
				}
				SaveFileData saveFileData;
				saveFileData = (SaveFileData)GameManager.Instance.GameData.SaveFileDirectory.GetValueOfIndex(i);
				if (saveFileData == null)
				{
					continue;
				}
				string path;
				path = Path.Combine(path2: string.Concat("newgame_" + saveFileData.ID, ".png"), path1: Path.Combine(Application.persistentDataPath, "images"));
				if (File.Exists(path))
				{
					byte[] data;
					data = File.ReadAllBytes(path);
					Texture2D texture2D;
					texture2D = new Texture2D(2, 2);
					texture2D.LoadImage(data);
					UIElementSaveDataVO uIElementSaveDataVO;
					uIElementSaveDataVO = new UIElementSaveDataVO("UI/Elements/UIElementTitleMenuSave", TextUtility.GetKey(saveFileData.CurrentZone), saveFileData.TimeStamp, TextUtility.GetKey("MENU_SETTINGS_LOAD_GAME_DIFFICULTY") + ": " + UIGameOver.GetDifficulity(saveFileData.Difficulty), texture2D);
					uIElementSaveDataVO.Callback = () =>
					{
						this.LoadSave(saveFileData.Name);
					};
					list2.Add(uIElementSaveDataVO);
				}
			}
			list2.Sort((UIElementSaveDataVO d1, UIElementSaveDataVO d2) => DateTime.Parse(d2.DateTimeLabel).CompareTo(DateTime.Parse(d1.DateTimeLabel)));
			for (int num = 0; num < list2.Count; num++)
			{
				list.Add(list2[num]);
			}
			list.Insert(0, new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 50f));
			list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f));
			this.m_LoadButtons.Clear();
			this.m_LoadSelectionIndex = 0;
			List<UIElementDataVO>.Enumerator enumerator;
			enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current.PrefabKey == string.Empty) && UIManager.CreateElement(this.m_LoadContentElementContainer, enumerator.Current.PrefabKey, enumerator.Current) is UIElementButtonLabel item)
				{
					this.m_LoadButtons.Add(item);
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

	private void LoadSave(string fileName)
	{
		this.m_HasLoaded = false;
		UIManager.SetCursor(active: false);
		this.m_BlackOverlay.gameObject.SetActive(value: true);
		this.m_Sequencer.RestartSequence();
		this.m_Sequencer.Insert(0f, this.m_BlackOverlay.DOFade(1f, 1f).SetEase(Ease.Linear));
		this.m_Sequencer.OnComplete(() =>
		{
			GameManager.Instance.SetPause(isPaused: true);
			GameManager.Instance.UnlockPause();
			GameManager.Instance.TestLoadSave(fileName);
		});
	}

	public void MainMenuOnClick()
	{
		this.ExitGameOver(this.MainMenuOnClickComplete);
	}

	private void MainMenuOnClickComplete()
	{
		GameManager.Instance.SetGameState(GameState.MENU);
		GameManager.Instance.SetPause(isPaused: false);
		GameManager.Instance.UnpauseGame();
		SceneManager.LoadScene("Reset");
	}

	private void ExitGameOver(Action onComplete)
	{
		this.LockInput();
		this.SetCursor(active: false);
		this.OnExit.Send(this);
		Color black;
		black = Color.black;
		black.a = 0f;
		this.m_BlackOverlay.color = black;
		this.m_BlackOverlay.enabled = true;
		this.m_Sequencer.RestartSequence();
		this.m_Sequencer.Insert(0f, this.m_MainContent.DOFade(0f, 1f).SetEase(Ease.Linear));
		this.m_Sequencer.Insert(0.5f, this.m_BlackOverlay.DOFade(1f, 2f).SetEase(Ease.Linear));
		this.m_Sequencer.Insert(3f, () =>
		{
			GameManager.Instance.ShowScreenBlocker(0f);
		});
		this.m_Sequencer.OnComplete(() =>
		{
			onComplete?.Invoke();
			this.PlayOut();
		});
	}

	private void SetGameManager()
	{
		GameManager.Instance.SetGameState(GameState.UI);
		GameManager.Instance.LockPause();
		GameManager.Instance.SetPause(isPaused: true);
		GameManager.Instance.PauseGame();
	}

	private void PlayDeathAudio()
	{
		for (int i = 0; i < this.m_GameOverData.Group.Length; i = unchecked(i + 1))
		{
			GameOverData.GameOverGroup gameOverGroup;
			gameOverGroup = this.m_GameOverData.Group[i];
			if (this.m_DataVO.GameOverType == gameOverGroup.GameOverType)
			{
				this.m_Background.sprite = gameOverGroup.Background;
				gameOverGroup.Audio.Play();
				break;
			}
		}
	}

	private void GetButton(string label, Action callback)
	{
		UIElementButtonDataVO uIElementButtonDataVO;
		uIElementButtonDataVO = new UIElementButtonDataVO("UI/GameOver/UIElementGameOverButton", label, callback, "", TextAlignmentOptions.Midline);
		UIElementButtonLabel item;
		item = (UIElementButtonLabel)UIManager.CreateElement(this.m_ButtonContainer, uIElementButtonDataVO.PrefabKey, uIElementButtonDataVO);
		this.m_Buttons.Add(item);
	}

	private void SetCursor(bool active)
	{
		if (!GameManager.Instance.HasController)
		{
			UIManager.SetCursor(active);
		}
		else
		{
			UIManager.SetCursor(active: false);
		}
	}

	private void LockInput()
	{
		this.m_IsTransitioning = true;
		this.m_RaycastBlocker.SetActive(value: true);
	}

	private void UnlockInput()
	{
		this.m_IsTransitioning = false;
		this.m_RaycastBlocker.SetActive(value: false);
	}

	protected override void OnDisposed()
	{
		this.OnExit = null;
		this.m_DataVO = null;
		this.m_ActiveButton = null;
		if (this.m_Buttons != null)
		{
			this.m_Buttons.Clear();
			this.m_Buttons = null;
		}
		if (this.m_Sequencer != null)
		{
			this.m_Sequencer.Kill();
			this.m_Sequencer = null;
		}
		base.OnDisposed();
	}
}
