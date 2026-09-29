using System.Collections.Generic;
using UnityEngine;

public static class UILayouts
{
	public static UIGameMenu GameMenu()
	{
		return GameManager.Instance.UIManager.Show<UIGameMenu>("UI/GameMenu/UIGameMenu", "GAME MENU", UIGameMenuDataVO.Create());
	}

	public static List<UIElementDataVO> GetTitleMenu()
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		if (GameManager.Instance.GameData.SaveFileDirectory.Count > 0)
		{
			list.Add(new UIElementTitleMenuButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_CONTINUE").ToUpper(), TitleMenuType.Continue));
		}
		list.Add(new UIElementTitleMenuButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_NEW_GAME").ToUpper(), TitleMenuType.NewGame));
		if (GameManager.Instance.GameData.SaveFileDirectory.Count > 0)
		{
			list.Add(new UIElementTitleMenuButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_LOAD_GAME").ToUpper(), TitleMenuType.LoadGame));
		}
		list.Add(new UIElementTitleMenuButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_OPTIONS").ToUpper(), TitleMenuType.Options));
		if (GameManager.Instance.GameData.Credits || GameManager.Instance.AchievementManager.GetAchievement(AchievementName.THE_MASTERS_PEN))
		{
			list.Add(new UIElementTitleMenuButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_ARCHIVES").ToUpper(), TitleMenuType.Archives));
			list.Add(new UIElementTitleMenuButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_CREDITS").ToUpper(), TitleMenuType.Credits));
		}
		list.Add(new UIElementTitleMenuButtonDataVO("UI/Elements/UIElementTitleMenuButtonLabel", TextUtility.GetKey("MENU_QUIT").ToUpper(), TitleMenuType.Quit));
		return list;
	}

	public static List<UIElementDataVO> GetStatus(List<UIElementGameMenuObjectiveDataVO> objectives)
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f));
		if (GameManager.Instance.Player.CurrentZone != "")
		{
			UIElementLabelDataVO uIElementLabelDataVO;
			uIElementLabelDataVO = new UIElementLabelDataVO("UI/Elements/UIElementLabel");
			uIElementLabelDataVO.Label = string.Concat(TextUtility.GetKey("MENU_GAME_MENU_LOCATION") + ": ", TextUtility.GetKey(GameManager.Instance.Player.CurrentZone));
			uIElementLabelDataVO.Color = (GameManager.Instance.IsRealWorld ? UIColors.ACTIVE_LABEL_REAL : UIColors.ACTIVE_LABEL);
			uIElementLabelDataVO.FontSize = 50f;
			uIElementLabelDataVO.SizeDelta = new Vector2(1285f, 100f);
			list.Add(uIElementLabelDataVO);
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.TaskDirectory.Count > 0)
		{
			UIElementLabelDataVO uIElementLabelDataVO2;
			uIElementLabelDataVO2 = new UIElementLabelDataVO("UI/Elements/UIElementLabel");
			uIElementLabelDataVO2.Label = TextUtility.GetKey("MENU_GAME_MENU_OBJECTIVES");
			uIElementLabelDataVO2.Color = (GameManager.Instance.IsRealWorld ? UIColors.ACTIVE_LABEL_REAL : UIColors.ACTIVE_LABEL);
			uIElementLabelDataVO2.FontSize = 76f;
			uIElementLabelDataVO2.SizeDelta = new Vector2(1285f, 120f);
			list.Add(uIElementLabelDataVO2);
			foreach (UIElementGameMenuObjectiveDataVO objective in objectives)
			{
				list.Add(objective);
			}
			list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 180f));
		}
		return list;
	}

	public static List<UIElementDataVO> GetWeapons()
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		list.Add(new UIElementGameMenuWeaponHeaderDataVO("UI/Elements/Weapon/UIElementGameMenuWeaponHeader", TextUtility.GetKey("MENU_WEAPON_CONTENT_TITLE").ToUpper(), TextUtility.GetKey("MENU_WEAPON_CONTENT_UNLOCK").ToUpper()));
		UIElementGameMenuWeaponEmptyDataVO item;
		item = new UIElementGameMenuWeaponEmptyDataVO("UI/Elements/Weapon/UIElementGameMenuWeaponEmpty", TextUtility.GetKey("MENU_WEAPON_CONTENT_LOCKED").GetLinebreaks());
		WeaponUpgradeData asset;
		asset = GameManager.Instance.AssetManager.GetAsset<WeaponUpgradeData>("Weapons/WeaponUpgradeData");
		UIElementGameMenuWeaponDataVO item2;
		item2 = new UIElementGameMenuWeaponDataVO("UI/Elements/Weapon/UIElementGameMenuWeapon", TextUtility.GetKey("UI_INFO_POPUP_GENT_LOCK"), TextUtility.GetKey("MENU_WEAPON_GENT_LOCK_DESCRIPTION"), asset.Lv2);
		UIElementGameMenuWeaponDataVO item3;
		item3 = new UIElementGameMenuWeaponDataVO("UI/Elements/Weapon/UIElementGameMenuWeapon", TextUtility.GetKey("UI_INFO_POPUP_SHOCK_PIPE"), TextUtility.GetKey("MENU_WEAPON_SHOCK_PIPE_DESCRIPTION").GetChargeInput(), asset.Lv3);
		UIElementGameMenuWeaponDataVO item4;
		item4 = new UIElementGameMenuWeaponDataVO("UI/Elements/Weapon/UIElementGameMenuWeapon", TextUtility.GetKey("UI_INFO_POPUP_STUN_PIPE"), TextUtility.GetKey("MENU_WEAPON_STUN_PIPE_DESCRIPTION").GetChargeInput(), asset.Lv4);
		switch (GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID)
		{
		case WeaponType.LEVEL_1:
			list.Add(item);
			break;
		case WeaponType.LEVEL_2:
			list.Add(item2);
			list.Add(item);
			break;
		case WeaponType.LEVEL_3:
			list.Add(item2);
			list.Add(item3);
			list.Add(item);
			break;
		case WeaponType.LEVEL_4:
			list.Add(item2);
			list.Add(item3);
			list.Add(item4);
			break;
		}
		return list;
	}

	public static List<UIElementDataVO> GetAbilities()
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		list.Add(new UIElementGameMenuAbilityHeaderDataVO("UI/Elements/Abilities/UIElementGameMenuAbilityHeader", TextUtility.GetKey("MENU_ABILITY_CONTENT_TITLE").ToUpper(), TextUtility.GetKey("MENU_ABILITY_CONTENT_UNLOCK").ToUpper()));
		UIElementGameMenuAbilityEmptyDataVO item;
		item = new UIElementGameMenuAbilityEmptyDataVO("UI/Elements/Abilities/UIElementGameMenuAbilityEmpty", TextUtility.GetKey("MENU_ABILITY_CONTENT_LOCKED").GetLinebreaks());
		AbilityData asset;
		asset = GameManager.Instance.AssetManager.GetAsset<AbilityData>("Abilities/AbilityData");
		UIElementGameMenuAbilityDataVO item2;
		item2 = new UIElementGameMenuAbilityDataVO("UI/Elements/Abilities/UIElementGameMenuAbility", TextUtility.GetKey("UI_INFO_POPUP_BANISH"), TextUtility.GetKey("UI_INFO_POPUP_BANISH_MESSAGE").Replace("{0}", " "), asset.Banish);
		UIElementGameMenuAbilityDataVO item3;
		item3 = new UIElementGameMenuAbilityDataVO("UI/Elements/Abilities/UIElementGameMenuAbility", TextUtility.GetKey("UI_INFO_POPUP_FLOW"), TextUtility.GetKey("UI_INFO_POPUP_FLOW_MESSAGE").Replace("{0}", " "), asset.Flow);
		UIElementGameMenuAbilityDataVO item4;
		item4 = new UIElementGameMenuAbilityDataVO("UI/Elements/Abilities/UIElementGameMenuAbility", TextUtility.GetKey("UI_INFO_POPUP_FAST_TRAVEL"), TextUtility.GetKey("UI_INFO_POPUP_FAST_TRAVEL_MESSAGE").Replace("{0}", " "), asset.FastTravel);
		bool flag;
		flag = true;
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.ContainsKey(State.PlayerAbility.Banish))
		{
			list.Add(item2);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.ContainsKey(State.PlayerAbility.Flow))
		{
			list.Add(item3);
		}
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.ContainsKey(State.PlayerAbility.FastTravel))
		{
			list.Add(item4);
			flag = false;
		}
		if (flag)
		{
			list.Add(item);
		}
		return list;
	}

	public static List<UIElementDataVO> GetResources()
	{
		return new List<UIElementDataVO>
		{
			new UIElementResourceDataVO("UI/Elements/UIElementResource", GameManager.Instance.AssetManager.GetAsset<IconData>("Icon/Collectables/Small/UIIcon_Slug").Icon, GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs),
			new UIElementResourceDataVO("UI/Elements/UIElementResource", GameManager.Instance.AssetManager.GetAsset<IconData>("Icon/Collectables/Small/UIIcon_Card").Icon, GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards),
			new UIElementResourceDataVO("UI/Elements/UIElementResource", GameManager.Instance.AssetManager.GetAsset<IconData>("Icon/Collectables/Small/UIIcon_Battery").Icon, GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Batteries),
			new UIElementResourceDataVO("UI/Elements/UIElementResource", GameManager.Instance.AssetManager.GetAsset<IconData>("Icon/Collectables/Small/UIIcon_Parts").Icon, GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Parts),
			new UIElementResourceDataVO("UI/Elements/UIElementResource", GameManager.Instance.AssetManager.GetAsset<IconData>("Icon/Collectables/Small/UIIcon_Toolkit").Icon, GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Toolkits),
			new UIElementResourceDataVO("UI/Elements/UIElementResource", GameManager.Instance.AssetManager.GetAsset<IconData>("Icon/Collectables/Small/UIIcon_BatteryCasing").Icon, GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.BatteryCasings)
		};
	}

	public static List<UIElementAudioLogDataVO> GetAudioLogs()
	{
		List<UIElementAudioLogDataVO> list;
		list = new List<UIElementAudioLogDataVO>();
		for (int i = 0; i < GameManager.Instance.GameData.CurrentSave.DataDirectories.AudioLogDirectory.Count; i = unchecked(i + 1))
		{
			AudioLogDataObject audioLogDataObject;
			audioLogDataObject = GameManager.Instance.GameData.CurrentSave.DataDirectories.AudioLogDirectory.Values[i];
			list.Add(new UIElementAudioLogDataVO(button: new UIElementButtonDataVO("UI/Elements/UIElementAudioLogButtonLabel", "<size=160%>" + TextUtility.GetKey(audioLogDataObject.ID.ToString().Replace("AudioLog_", "AudioLogTitle_"))), prefabKey: "UI/Elements/UIElementAudioLog", id: audioLogDataObject.ID));
		}
		return list;
	}

	public static List<UIElementMemoDataVO> GetMemos()
	{
		List<UIElementMemoDataVO> list;
		list = new List<UIElementMemoDataVO>();
		for (int i = 0; i < GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.Count; i = unchecked(i + 1))
		{
			MemoDataObject memoDataObject;
			memoDataObject = GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.Values[i];
			list.Add(new UIElementMemoDataVO(button: new UIElementButtonDataVO("UI/Elements/UIElementMemoButtonLabel", "<size=160%>" + TextUtility.GetKey(memoDataObject.ID.ToString().Replace("Memo_", "MemoTitle_"))), prefabKey: "UI/Elements/UIElementMemo", id: memoDataObject.ID));
		}
		return list;
	}

	public static List<UIElementMemoryDataVO> GetMemories()
	{
		List<UIElementMemoryDataVO> list;
		list = new List<UIElementMemoryDataVO>();
		for (int i = 0; i < GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.Count; i = unchecked(i + 1))
		{
			MemoryDataObject memoryDataObject;
			memoryDataObject = GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.Values[i];
			list.Add(new UIElementMemoryDataVO(button: new UIElementButtonDataVO("UI/Elements/UIElementMemoryButtonLabel", "<size=160%>" + TextUtility.GetKey(memoryDataObject.ID.ToString().Replace("Memory_", "MemoryTitle_"))), prefabKey: "UI/Elements/UIElementMemory", id: memoryDataObject.ID, sprite: UILayouts.GetMemoryImage(memoryDataObject.ID)));
		}
		return list;
	}

	private static Sprite GetMemoryImage(MemoryID id)
	{
		Sprite result;
		result = null;
		string text;
		text = "";
		switch (id)
		{
		case MemoryID.Memory_S105_ID101_Baseball:
			text = "Icon/Memories/UIIcon_Memory_Ball";
			break;
		case MemoryID.Memory_S107_ID101_PaperPlane:
			text = "Icon/Memories/UIIcon_Memory_Plane";
			break;
		case MemoryID.Memory_S110_ID101_Duck:
			text = "Icon/Memories/UIIcon_Memory_Duck";
			break;
		case MemoryID.Memory_S113_ID101_OilTin:
			text = "Icon/Memories/UIIcon_Memory_Oil";
			break;
		case MemoryID.Memory_S115_ID101_AlarmClock:
			text = "Icon/Memories/UIIcon_Memory_Clock";
			break;
		case MemoryID.Memory_S118_ID101_Hat:
			text = "Icon/Memories/UIIcon_Memory_Hat";
			break;
		case MemoryID.Memory_S120_ID101_Crayons:
			text = "Icon/Memories/UIIcon_Memory_Crayons";
			break;
		case MemoryID.Memory_S122_ID101_MilkCarton:
			text = "Icon/Memories/UIIcon_Memory_Milk";
			break;
		case MemoryID.Memory_S124_ID101_CrackedMug:
			text = "Icon/Memories/UIIcon_Memory_Cup";
			break;
		case MemoryID.Memory_S128_ID101_HotDog:
			text = "Icon/Memories/UIIcon_Memory_HotDog";
			break;
		}
		if (text != "")
		{
			result = GameManager.Instance.AssetManager.GetAsset<IconData>(text).Icon;
		}
		return result;
	}

	public static List<UIElementDataVO> GetSettingsElements()
	{
		return new List<UIElementDataVO>
		{
			new UIElementGameMenuSettingsButtonDataVO("UI/Elements/UIElementGameMenuSettingsButton", TextUtility.GetKey("MENU_OPTIONS_AUDIO"), GameMenuSettingsType.Audio),
			new UIElementGameMenuSettingsButtonDataVO("UI/Elements/UIElementGameMenuSettingsButton", TextUtility.GetKey("MENU_OPTIONS_GRAPHICS"), GameMenuSettingsType.Graphics),
			new UIElementGameMenuSettingsButtonDataVO("UI/Elements/UIElementGameMenuSettingsButton", TextUtility.GetKey("MENU_OPTIONS_CONTROLS"), GameMenuSettingsType.Controls)
		};
	}

	public static List<UIElementDataVO> GetMenuElements()
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		if (GameManager.Instance.Player != null && GameManager.Instance.Player.CurrentState != State.Player.Cutscene && GameManager.Instance.Player.BattleStatus == BattleStatus.None)
		{
			list.Add(new UIElementGameMenuMenuButtonDataVO("UI/Elements/UIElementGameMenuMenuButton", TextUtility.GetKey("MENU_SETTINGS_SAVE_GAME"), GameMenuMenuType.SaveGame));
		}
		else
		{
			list.Add(new UIElementGameMenuMenuButtonDataVO("UI/Elements/UIElementGameMenuMenuButton", TextUtility.GetKey("MENU_SETTINGS_CANNOT_SAVE"), GameMenuMenuType.CannotSaveGame));
		}
		list.Add(new UIElementGameMenuMenuButtonDataVO("UI/Elements/UIElementGameMenuMenuButton", TextUtility.GetKey("MENU_LOAD_GAME"), GameMenuMenuType.LoadGame));
		list.Add(new UIElementGameMenuMenuButtonDataVO("UI/Elements/UIElementGameMenuMenuButton", TextUtility.GetKey("MENU_SETTINGS_MAIN_MENU"), GameMenuMenuType.MainMenu));
		return list;
	}

	public static List<UIElementDataVO> GetAudioLayout(bool isTitleMenu = true)
	{
		return new List<UIElementDataVO>
		{
			new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f),
			new UIElementSliderDataVO(UILayouts.GetSlider(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_AUDIO_VOLUME_MASTER", isTitleMenu), "VOLUME"),
			new UIElementSliderDataVO(UILayouts.GetSlider(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_AUDIO_VOLUME_EFFECTS", isTitleMenu), "SFX_VOLUME"),
			new UIElementSliderDataVO(UILayouts.GetSlider(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_AUDIO_VOLUME_MUSIC", isTitleMenu), "MUSIC_VOLUME"),
			new UIElementSliderDataVO(UILayouts.GetSlider(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_AUDIO_VOLUME_DIALOGUE", isTitleMenu), "DIALOGUE_VOLUME"),
			new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_AUDIO_SUBTITLES", isTitleMenu), "SUBTITLES"),
			new UIElementSelectorDataVO(UILayouts.GetSelector(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_AUDIO_LANGUAGE", isTitleMenu), "LANGUAGE", GameManager.Instance.PlayerSettings.Languages),
			new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f)
		};
	}

	public static List<UIElementDataVO> GetGraphicsLayout(bool isTitleMenu = true)
	{
		return new List<UIElementDataVO>
		{
			new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f),
			new UIElementSelectorDataVO(UILayouts.GetSelector(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_QUALITY", isTitleMenu), "QUALITY", GameManager.Instance.PlayerSettings.Qualites),
			new UIElementSelectorDataVO(UILayouts.GetSelector(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_RESOLUTION", isTitleMenu), "RESOLUTION_CURRENT", GameManager.Instance.PlayerSettings.ResolutionsList),
			new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_FULLSCREEN", isTitleMenu), "FULLSCREEN"),
			new UIElementSliderDataVO(UILayouts.GetSlider(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_BRIGHTNESS", isTitleMenu), "BRIGHTNESS"),
			new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_DOF", isTitleMenu), "DOF"),
			new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_MOTION_BLUR", isTitleMenu), "MOTION_BLUR"),
			new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_BLOOM", isTitleMenu), "BLOOM"),
			new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_AMBIENT_OCCLUSION", isTitleMenu), "AMBIENT_OCCLUSION"),
			new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_ANTI_ALIASING", isTitleMenu), "ANTI_ALIASING"),
			new UIElementSelectorDataVO(UILayouts.GetSelector(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_GRAPHICS_VSYNC", isTitleMenu), "V_SYNC", GameManager.Instance.PlayerSettings.VSyncs),
			new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f)
		};
	}

	public static List<UIElementDataVO> GetControlsLayout(bool isTitleMenu = true)
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f));
		list.Add(new UIElementSliderDataVO(UILayouts.GetSlider(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_CONTROLS_SENSITIVITY", isTitleMenu), "SENSITIVITY"));
		list.Add(new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_CONTROLS_INVERTED", isTitleMenu), "INVERTED"));
		list.Add(new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_CONTROLS_CROSSHAIR", isTitleMenu), "CROSSHAIR"));
		list.Add(new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_CONTROLS_VIEW_SWAYING", isTitleMenu), "VIEW_SWAYING"));
		list.Add(new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_CONTROLS_SMOOTH_CAMERA", isTitleMenu), "SMOOTH_CAMERA"));
		if (GameManager.Instance.HasController)
		{
			list.Add(new UIElementSliderDataVO(UILayouts.GetSlider(isTitleMenu), UILayouts.GetText("MENU_OPTIONS_CONTROLS_RUMBLE", isTitleMenu), "RUMBLE"));
		}
		if (isTitleMenu && SeasonalCheck.IsSeasonal())
		{
			list.Add(new UIElementToggleDataVO(UILayouts.GetToggle(isTitleMenu), UILayouts.GetText("MENU_SEASONAL", isTitleMenu), "SEASONAL"));
		}
		list.Add(new UIElementSpaceDataVO("UI/Elements/MenuElementSpace", 120f));
		return list;
	}

	private static string GetToggle(bool isTitleMenu)
	{
		if (!isTitleMenu)
		{
			return "UI/GameMenu/Settings/UIElementGameMenuToggle";
		}
		return "UI/Elements/Options/UIElementTitleMenuToggle";
	}

	private static string GetSelector(bool isTitleMenu)
	{
		if (!isTitleMenu)
		{
			return "UI/GameMenu/Settings/UIElementGameMenuSelector";
		}
		return "UI/Elements/Options/UIElementTitleMenuSelector";
	}

	private static string GetSlider(bool isTitleMenu)
	{
		if (!isTitleMenu)
		{
			return "UI/GameMenu/Settings/UIElementGameMenuSlider";
		}
		return "UI/Elements/Options/UIElementTitleMenuSlider";
	}

	private static string GetText(string value, bool isUpper)
	{
		if (!isUpper)
		{
			return TextUtility.GetKey(value);
		}
		return TextUtility.GetKey(value).ToUpper();
	}
}
