using System;
using System.Collections.Generic;
using System.IO;
using InControl;
using TMPro;
using UnityEngine;

public class UIMenuDataVO
{
	public const float BODY_HEIGHT = 1080f;

	public UIElementDataVO TitleElement;

	public List<UIElementDataVO> UIElements = new List<UIElementDataVO>();

	public UIElementDataVO FooterElement;

	public float MenuHeight;

	public UIMenuDataVO(List<UIElementDataVO> uiElements)
	{
		this.UIElements = uiElements;
	}

	public static UIMenuDataVO Create(MenuType menuType)
	{
		return UIMenuDataVO.GetMenu(menuType);
	}

	private static UIMenuDataVO GetMenu(MenuType menuType)
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		UIElementDataVO titleElement;
		titleElement = null;
		string prefabKey;
		prefabKey = "UI/Elements/UIElementMenuFooter";
		float menuHeight;
		menuHeight = 1080f;
		unchecked
		{
			switch (menuType)
			{
			case MenuType.LoadGame:
			{
				List<UIElementSaveDataVO> list3;
				list3 = new List<UIElementSaveDataVO>();
				for (int num4 = 0; num4 < 11; num4++)
				{
					SaveFileData saveFileData2;
					saveFileData2 = (SaveFileData)GameManager.Instance.GameData.SaveFileDirectory.GetValueOfIndex(num4);
					if (saveFileData2 == null)
					{
						continue;
					}
					string path2;
					path2 = Path.Combine(path2: ((num4 == 0) ? "autosave" : ("newgame_" + saveFileData2.ID)) + ".png", path1: Path.Combine(Application.persistentDataPath, "images"));
					if (File.Exists(path2))
					{
						byte[] data2;
						data2 = File.ReadAllBytes(path2);
						Texture2D texture2D2;
						texture2D2 = new Texture2D(2, 2);
						texture2D2.LoadImage(data2);
						string key;
						key = TextUtility.GetKey(saveFileData2.CurrentZone);
						if (saveFileData2.ID == 0)
						{
							key = TextUtility.GetKey("MENU_SETTINGS_LOAD_GAME_AUTOSAVE");
						}
						UIElementSaveDataVO uIElementSaveDataVO;
						uIElementSaveDataVO = new UIElementSaveDataVO("UI/GameMenu/Saves/MenuElementSave", key, saveFileData2.TimeStamp, TextUtility.GetKey("MENU_SETTINGS_LOAD_GAME_DIFFICULTY") + ": " + UIMenuDataVO.GetDifficulity(saveFileData2.Difficulty), texture2D2);
						uIElementSaveDataVO.Callback = () =>
						{
							UIMenuDataVO.LoadSave(saveFileData2.Name);
						};
						if (num4 == 0)
						{
							list.Insert(0, uIElementSaveDataVO);
							list.Insert(1, new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 40f));
						}
						else
						{
							list3.Add(uIElementSaveDataVO);
						}
					}
				}
				list3.Sort((UIElementSaveDataVO d1, UIElementSaveDataVO d2) => DateTime.Parse(d2.DateTimeLabel).CompareTo(DateTime.Parse(d1.DateTimeLabel)));
				for (int num5 = 0; num5 < list3.Count; num5++)
				{
					list.Add(list3[num5]);
				}
				list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f));
				prefabKey = "UI/Elements/UIElementMenuFooterButton";
				break;
			}
			case MenuType.SaveGame:
			{
				List<UIElementSaveDataVO> list2;
				list2 = new List<UIElementSaveDataVO>();
				for (int i = 1; i < 11; i++)
				{
					SaveFileData saveFileData;
					saveFileData = (SaveFileData)GameManager.Instance.GameData.SaveFileDirectory.GetValueOfIndex(i);
					if (saveFileData == null || saveFileData.ID == 0)
					{
						continue;
					}
					string path;
					path = Path.Combine(Path.Combine(Application.persistentDataPath, "images"), "newgame_" + saveFileData.ID + ".png");
					if (File.Exists(path))
					{
						byte[] data;
						data = File.ReadAllBytes(path);
						Texture2D texture2D;
						texture2D = new Texture2D(2, 2);
						texture2D.LoadImage(data);
						UIElementSaveDataVO elementDataVO;
						elementDataVO = new UIElementSaveDataVO("UI/GameMenu/Saves/MenuElementSave", TextUtility.GetKey(saveFileData.CurrentZone), saveFileData.TimeStamp, TextUtility.GetKey("MENU_SETTINGS_LOAD_GAME_DIFFICULTY") + ": " + UIMenuDataVO.GetDifficulity(saveFileData.Difficulty), texture2D);
						elementDataVO.Callback = () =>
						{
							UIMenuDataVO.OverwriteSave(saveFileData.Name, elementDataVO);
						};
						list2.Add(elementDataVO);
					}
				}
				list2.Sort((UIElementSaveDataVO d1, UIElementSaveDataVO d2) => DateTime.Parse(d2.DateTimeLabel).CompareTo(DateTime.Parse(d1.DateTimeLabel)));
				for (int num = 0; num < list2.Count; num++)
				{
					list.Add(list2[num]);
				}
				if (GameManager.Instance.GameData.SaveFileDirectory.Count <= 10)
				{
					list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 40f));
					list.Add(new UIElementButtonDataVO("UI/GameMenu/Saves/MenuElementSaveSlot", TextUtility.GetKey("MENU_SETTINGS_SAVE_GAME_SAVE_SLOT") + " " + GameManager.Instance.GameData.SaveFileDirectory.Count.ToString("00"), UIMenuDataVO.Save));
				}
				int num2;
				num2 = 10 - GameManager.Instance.GameData.SaveFileDirectory.Count;
				for (int num3 = 0; num3 < num2; num3++)
				{
					list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 15f));
					list.Add(new UIElementSaveSlotEmptyDataVO("UI/GameMenu/Saves/MenuElementSaveSlotEmpty", TextUtility.GetKey("MENU_SETTINGS_SAVE_GAME_SAVE_SLOT") + " " + (GameManager.Instance.GameData.SaveFileDirectory.Count + 1 + num3).ToString("00"), TextUtility.GetKey("MENU_SETTINGS_SAVE_GAME_NEW")));
				}
				list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f));
				prefabKey = "UI/Elements/UIElementMenuFooterButton";
				break;
			}
			}
			return new UIMenuDataVO(list)
			{
				TitleElement = titleElement,
				MenuHeight = menuHeight,
				FooterElement = new UIElementDataVO(prefabKey)
			};
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

	private static void LoadSave(string fileName)
	{
		GameManager.Instance.TestLoadSave(fileName);
	}

	private static void OverwriteSave(string fileName, UIElementSaveDataVO uIElementSaveDataVO)
	{
		if (uIElementSaveDataVO != null)
		{
			uIElementSaveDataVO.UIPrompt = GameManager.Instance.UIManager.Show<UIPrompt>("UI/GameMenuPrompt/UIPrompt", "GAME MENU PROMPT", new UIPromptDataVO(TextUtility.GetKey("MENU_SETTINGS_SAVE_GAME_HEADER"), TextUtility.GetKey("MENU_SETTINGS_SAVE_GAME_SUB_HEADER"), new UIElementButtonDataVO("UI/GameMenu/MenuElementButtonControllerLabel", TextUtility.GetKey("MENU_YES"), () =>
			{
				UIMenuDataVO.OverwriteSaveInternal(fileName);
			}, "", TextAlignmentOptions.Midline, InputControlType.Action1), new UIElementButtonDataVO("UI/GameMenu/MenuElementButtonControllerLabel", TextUtility.GetKey("MENU_NO"), uIElementSaveDataVO.PlayOut, "", TextAlignmentOptions.Midline, InputControlType.Action2)));
		}
	}

	private static void OverwriteSaveInternal(string fileName)
	{
		JDebug.Log("[SAVING] - Overwriting " + fileName, null, JDebug.JDebugType.SaveData);
		GameManager.Instance.GameData.GetSaveFile(fileName, out var _);
		UIMenuDataVO.SaveInternal();
	}

	private static void Save()
	{
		JDebug.Log("[SAVING] - New Save", null, JDebug.JDebugType.SaveData);
		SaveFileData saveFileData;
		saveFileData = GameManager.Instance.GameData.NewSaveFile("newgame", "Saved Data");
		GameManager.Instance.GameData.NewSave(saveFileData.ID, GameManager.Instance.GameData.CurrentSave);
		UIMenuDataVO.SaveInternal();
	}

	private static void SaveInternal()
	{
		GameManager.Instance.GameData.Update();
		GameManager.Instance.GameDataManager.Save(UIMenuDataVO.OnSaveComplete);
	}

	private static void OnSaveComplete(bool success)
	{
		GameManager.Instance.GameData.CurrentSave.PostSave();
		GameManager.Instance.UIManager.PlayOutGameMenu();
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_GAME_SAVED"));
	}
}
