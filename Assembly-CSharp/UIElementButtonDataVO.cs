using System;
using InControl;
using TMPro;

public class UIElementButtonDataVO : UIElementDataVO
{
	public UIElementLabelDataVO LabelDataVO;

	public string Label;

	public Action Callback;

	public string InvokeCallback;

	public TextAlignmentOptions Alignment = TextAlignmentOptions.MidlineLeft;

	public InputControlType InputControlType;

	public UIElementButtonDataVO(string prefabKey, UIElementLabelDataVO label, Action callback = null, string invokeCallback = "")
		: base(prefabKey)
	{
		this.LabelDataVO = label;
		this.Callback = callback;
		this.InvokeCallback = invokeCallback;
	}

	public UIElementButtonDataVO(string prefabKey, string label, Action callback = null, string invokeCallback = "", TextAlignmentOptions alignment = TextAlignmentOptions.MidlineLeft, InputControlType inputControlType = InputControlType.None)
		: base(prefabKey)
	{
		this.Label = label;
		this.Callback = callback;
		this.InvokeCallback = invokeCallback;
		this.Alignment = alignment;
		this.InputControlType = inputControlType;
	}
}
