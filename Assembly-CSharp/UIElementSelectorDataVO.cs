using System;

public class UIElementSelectorDataVO : UIElementDataVO
{
	public string Label;

	public string PlayerPrefs;

	public string[] Selection;

	public Action Callback;

	public string InvokeCallback;

	public UIElementSelectorDataVO(string prefabKey, string label, string playerPrefs = "", string[] selection = null, Action callback = null, string invokeCallback = "")
		: base(prefabKey)
	{
		this.Label = label;
		this.PlayerPrefs = playerPrefs;
		this.Selection = selection;
		this.Callback = callback;
		this.InvokeCallback = invokeCallback;
	}
}
