using System;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Scriptable/Control Group", order = 10)]
public class S13ScriptableControlGroup : ScriptableObject
{
	public enum S13ConditionalForceMode
	{
		IgnoreNew = 0,
		KillRandom = 1,
		KillOld = 2
	}

	[Serializable]
	public class ConditionalList : ReorderableArray<ControlGroupConditional>
	{
	}

	[Reorderable(elementNameProperty = "controlGroupCondition")]
	public ConditionalList conditionals;

	private S13ControlGroupHandler GetControlGroupData()
	{
		return S13Manager.GetControlGroupHandler(this);
	}

	public bool RequestPlay(S13AudioHandler audioHandler, bool shouldRegister)
	{
		S13ControlGroupHandler controlGroupData;
		controlGroupData = this.GetControlGroupData();
		S13Object obj;
		obj = audioHandler.GetObject();
		S13Handler handler;
		handler = audioHandler.GetHandler();
		string resultMessage;
		resultMessage = "";
		foreach (ControlGroupConditional conditional in this.conditionals)
		{
			if (!conditional.CheckCondition(audioHandler, controlGroupData, out resultMessage))
			{
				S13Debug.Log("Failed " + resultMessage, handler, S13Debug.LogType.ControlGroup);
				if (!conditional.ForcePlay(audioHandler, controlGroupData, out resultMessage))
				{
					return false;
				}
			}
		}
		if (shouldRegister)
		{
			controlGroupData.RegisterObject(obj, handler);
			controlGroupData.MarkApprovedPlay(obj, handler);
		}
		S13Debug.Log("Approved " + resultMessage, handler, S13Debug.LogType.ControlGroup);
		return true;
	}
}
