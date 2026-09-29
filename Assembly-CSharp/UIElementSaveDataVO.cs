using System;
using UnityEngine;

public class UIElementSaveDataVO : UIElementButtonDataVO
{
	public string DateTimeLabel;

	public string MessageLabel;

	public UIPrompt UIPrompt;

	public Texture2D Image;

	public event EventHandler OnPlayOut;

	public UIElementSaveDataVO(string prefabKey, string label, string dateTimeLabel, string messageLabel, Texture2D image, UIPrompt uiPrompt = null)
		: base(prefabKey, label)
	{
		this.DateTimeLabel = dateTimeLabel;
		this.MessageLabel = messageLabel;
		this.Image = image;
		this.UIPrompt = uiPrompt;
	}

	public void PlayOut()
	{
		if (this.UIPrompt != null)
		{
			this.UIPrompt.PlayOut();
			this.OnPlayOut.Send(this);
		}
	}
}
