using InControl;
using UnityEngine;

public class ObjectiveNotification : Objective
{
	[Header("Primary Label")]
	[SerializeField]
	private string m_Label = "";

	[SerializeField]
	private bool m_IsKey;

	[SerializeField]
	private bool m_ToUpper;

	[Header("Input Options")]
	[SerializeField]
	private bool m_UseInput;

	[SerializeField]
	private string m_PressKey;

	[SerializeField]
	private string m_ActionKey;

	[SerializeField]
	private string m_PCKeyboardButton = "";

	[SerializeField]
	private InputControlType m_InputControlType;

	protected override void InternalInitialize()
	{
		if (this.m_UseInput)
		{
			if (GameManager.Instance.HasController)
			{
				GameManager.Instance.ShowNotificationTextInput(TextUtility.GetKey(this.m_PressKey).ToUpper(), TextUtility.GetKey(this.m_ActionKey).ToUpper(), this.m_InputControlType);
			}
			else
			{
				GameManager.Instance.ShowNotificationTextInput(TextUtility.GetKey(this.m_PressKey).ToUpper(), TextUtility.GetKey(this.m_ActionKey).ToUpper(), this.m_PCKeyboardButton);
			}
		}
		else if (this.m_IsKey)
		{
			if (this.m_ToUpper)
			{
				GameManager.Instance.ShowNotificationText(TextUtility.GetKey(this.m_Label).GetColorNotification().GetWhite()
					.ToUpper());
			}
			else
			{
				GameManager.Instance.ShowNotificationText(TextUtility.GetKey(this.m_Label).GetColorNotification().GetWhite());
			}
		}
		else if (this.m_ToUpper)
		{
			GameManager.Instance.ShowNotificationText(this.m_Label.ToUpper());
		}
		else
		{
			GameManager.Instance.ShowNotificationText(this.m_Label);
		}
		base.SendOnComplete();
	}
}
