using System;
using TMPro;

public class UIElementSliderDataVO : UIElementDataVO
{
	public string Label;

	public string PlayerPrefs;

	public Action Callback;

	public string InvokeCallback;

	public TextAlignmentOptions Alignment = TextAlignmentOptions.MidlineLeft;

	public UIElementSliderDataVO(string prefabKey, string label, string playerPrefs = "", Action callback = null, string invokeCallback = "", TextAlignmentOptions alignment = TextAlignmentOptions.MidlineLeft)
		: base(prefabKey)
	{
		this.Label = label;
		this.PlayerPrefs = playerPrefs;
		this.Callback = callback;
		this.InvokeCallback = invokeCallback;
		this.Alignment = alignment;
	}
}
