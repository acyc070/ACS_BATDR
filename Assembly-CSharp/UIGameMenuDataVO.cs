using System.Collections.Generic;

public class UIGameMenuDataVO
{
	public List<UIElementDataVO> NavigationUIElements = new List<UIElementDataVO>();

	public List<UIElementGameMenuObjectiveDataVO> objectivelist = new List<UIElementGameMenuObjectiveDataVO>();

	private static bool m_HasWeapon
	{
		get
		{
			if (GameManager.Instance.BeastBendy == null && GameManager.Instance.Player != null)
			{
				return GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.ID != WeaponType.NONE;
			}
			return false;
		}
	}

	private static bool m_HasAbility
	{
		get
		{
			if (GameManager.Instance.BeastBendy == null && GameManager.Instance.Player != null)
			{
				return GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.Count > 0;
			}
			return false;
		}
	}

	private static bool m_HasMemo => GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.Count > 0;

	private static bool m_HasAudioLogs => GameManager.Instance.GameData.CurrentSave.DataDirectories.AudioLogDirectory.Count > 0;

	private static bool m_HasMemories => GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.Count > 0;

	public UIGameMenuDataVO(List<UIElementDataVO> uiElements)
	{
		this.NavigationUIElements = uiElements;
	}

	public static UIGameMenuDataVO Create()
	{
		List<UIElementDataVO> list;
		list = new List<UIElementDataVO>();
		if (GameManager.Instance.BeastBendy == null)
		{
			if (GameManager.Instance.IsRealWorld)
			{
				list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconStatusReal"));
			}
			else
			{
				list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconStatusInk"));
			}
		}
		else
		{
			list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconStatusBeast"));
		}
		if (UIGameMenuDataVO.m_HasWeapon)
		{
			list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconWeapon"));
		}
		if (UIGameMenuDataVO.m_HasAbility)
		{
			list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconAbilities"));
		}
		if (UIGameMenuDataVO.m_HasMemo)
		{
			list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconMemos"));
		}
		if (UIGameMenuDataVO.m_HasAudioLogs)
		{
			list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconAudioLogs"));
		}
		if (UIGameMenuDataVO.m_HasMemories)
		{
			list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconMemories"));
		}
		list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconSettings"));
		list.Add(UIGameMenuDataVO.GetMenuButton("UI/GameMenu/Icons/UIElementGameMenuIconMenu"));
		List<UIElementGameMenuObjectiveDataVO> list2;
		list2 = new List<UIElementGameMenuObjectiveDataVO>();
		unchecked
		{
			if (GameManager.Instance.GameData.CurrentSave.DataDirectories.TaskDirectory.Count > 0)
			{
				for (int num = GameManager.Instance.GameData.CurrentSave.DataDirectories.TaskDirectory.Count - 1; num >= 0; num--)
				{
					TaskDataObject taskDataObject;
					taskDataObject = GameManager.Instance.GameData.CurrentSave.DataDirectories.TaskDirectory.Values[num];
					if (taskDataObject != null && !taskDataObject.IsComplete)
					{
						List<TaskEntryDataVO> list3;
						list3 = new List<TaskEntryDataVO>();
						foreach (TaskEntryDataObject task in taskDataObject.Tasks)
						{
							list3.Add(TaskEntryDataVO.Create(task.ID, task.IsComplete));
						}
						list2.Add(UIElementGameMenuObjectiveDataVO.Create(ObjectiveDataVO.Create(taskDataObject.ID, list3.ToArray())));
					}
				}
			}
			return new UIGameMenuDataVO(list)
			{
				objectivelist = list2
			};
		}
	}

	private static UIElementDataVO GetMenuButton(string prefabKey)
	{
		return new UIElementDataVO(prefabKey);
	}
}
