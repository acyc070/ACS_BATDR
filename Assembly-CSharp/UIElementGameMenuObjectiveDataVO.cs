using System.Collections.Generic;
using UnityEngine;

public class UIElementGameMenuObjectiveDataVO : UIElementDataVO
{
	public UIElementDataVO Title;

	public List<UIElementDataVO> Objectives;

	public UIElementGameMenuObjectiveDataVO(string prefabKey)
		: base(prefabKey)
	{
	}

	public static UIElementGameMenuObjectiveDataVO Create(ObjectiveDataVO objective)
	{
		UIElementLabelDataVO uIElementLabelDataVO;
		uIElementLabelDataVO = new UIElementLabelDataVO("UI/Elements/UIElementLabel");
		uIElementLabelDataVO.Label = TextUtility.GetKey(objective.Title.ToString());
		uIElementLabelDataVO.Color = (GameManager.Instance.IsRealWorld ? UIColors.ACTIVE_LABEL_REAL : UIColors.ACTIVE_LABEL);
		uIElementLabelDataVO.FontSize = 50f;
		uIElementLabelDataVO.SizeDelta = new Vector2(1285f, 80f);
		List<TaskEntryDataVO> list;
		list = new List<TaskEntryDataVO>();
		unchecked
		{
			for (int i = 0; i < objective.Tasks.Length; i++)
			{
				list.Add(objective.Tasks[i]);
			}
			list.Sort((TaskEntryDataVO d1, TaskEntryDataVO d2) => d1.IsComplete.CompareTo(d2.IsComplete));
			List<UIElementDataVO> list2;
			list2 = new List<UIElementDataVO>();
			for (int num = 0; num < list.Count; num++)
			{
				list2.Add(UIElementGameMenuObjectiveDataVO.GetTaskEntry(list[num]));
			}
			return new UIElementGameMenuObjectiveDataVO("UI/GameMenu/UIElementGameMenuObjective")
			{
				Title = uIElementLabelDataVO,
				Objectives = list2
			};
		}
	}

	private static UIElementLabelDataVO GetTaskEntry(TaskEntryDataVO taskEntryID)
	{
		UIElementLabelDataVO uIElementLabelDataVO;
		uIElementLabelDataVO = new UIElementLabelDataVO("UI/Elements/UIElementLabel");
		string text;
		text = TextUtility.GetKey(taskEntryID.ID.ToString());
		if (taskEntryID.ID.ToString().Contains("E99"))
		{
			uIElementLabelDataVO.Label = text;
			uIElementLabelDataVO.Color = (GameManager.Instance.IsRealWorld ? UIColors.ACTIVE_LABEL_REAL : UIColors.ACTIVE_LABEL);
			uIElementLabelDataVO.FontSize = 45f;
			uIElementLabelDataVO.SizeDelta = new Vector2(1285f, 80f);
		}
		else
		{
			if (taskEntryID.IsComplete)
			{
				text = "<s>" + text + "</s>";
			}
			uIElementLabelDataVO.Label = "<indent=20> •  " + text + "</indent>";
			Color color;
			color = (GameManager.Instance.IsRealWorld ? UIColors.ACTIVE_LABEL_REAL : UIColors.ACTIVE_LABEL);
			Color color2;
			color2 = (GameManager.Instance.IsRealWorld ? UIColors.INACTIVE_LABEL_REAL : UIColors.INACTIVE_LABEL);
			uIElementLabelDataVO.Color = (taskEntryID.IsComplete ? color2 : color);
			uIElementLabelDataVO.FontSize = 40f;
			uIElementLabelDataVO.SizeDelta = new Vector2(1285f, 60f);
		}
		return uIElementLabelDataVO;
	}
}
