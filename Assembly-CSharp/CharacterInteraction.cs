using System;
using UnityEngine;

[Serializable]
public class CharacterInteraction
{
	public Animator InteractableProp;

	public string AnimationTrigger;

	public ActionEventController ActionEvent;

	public TimelineUnityEvent TimelineUnityEvent;

	public bool IsTriggered { get; private set; }

	public void Trigger()
	{
		if (this.InteractableProp != null)
		{
			this.InteractableProp.SetTrigger(this.AnimationTrigger);
		}
		else if (this.ActionEvent != null)
		{
			this.ActionEvent.Content.Action();
		}
		if (this.TimelineUnityEvent != null)
		{
			this.TimelineUnityEvent.Action();
		}
		this.IsTriggered = true;
	}

	public void ResetTrigger()
	{
		this.IsTriggered = false;
	}
}
