using System;
using UnityEngine;

[Serializable]
public class CompanionInteraction
{
	public Animator InteractableProp;

	public string AnimationTrigger;

	public ActionEventController ActionEvent;

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
	}
}
