public class UIPromptDataVO
{
	public string HeaderLabel;

	public string MessageLabel;

	public UIElementButtonDataVO m_ButtonA;

	public UIElementButtonDataVO m_ButtonB;

	public UIPromptDataVO(string headerLabel, string messageLabel, UIElementButtonDataVO buttonA, UIElementButtonDataVO buttonB = null)
	{
		this.HeaderLabel = headerLabel;
		this.MessageLabel = messageLabel;
		this.m_ButtonA = buttonA;
		this.m_ButtonB = buttonB;
	}
}
