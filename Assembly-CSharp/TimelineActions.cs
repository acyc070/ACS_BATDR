using System;
using UnityEngine;

[Serializable]
public class TimelineActions
{
	public TimelineActionType ActionType;

	public TimelineActionCameraShake CameraShake;

	public TimelineActionDOTween DOTween;

	public void Action()
	{
		if (this.ActionType == TimelineActionType.CameraShake)
		{
			this.CameraShake.Action();
		}
		else if (this.ActionType == TimelineActionType.DOTween)
		{
			this.DOTween.Action();
		}
		else
		{
			Debug.Log("TimelineActions :: No Action Selected - ActionType = " + this.ActionType);
		}
	}
}
