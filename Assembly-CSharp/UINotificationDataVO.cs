using InControl;

public class UINotificationDataVO
{
	public string Message;

	public bool IsInkDemon;

	public string PressLabel;

	public string ActionLabel;

	public string InputLabel;

	public InputControlType InputControlType;

	public bool HasInput { get; private set; }

	public UINotificationDataVO(string message, bool isInkDemon = false)
	{
		this.Message = message;
		this.IsInkDemon = isInkDemon;
	}

	public UINotificationDataVO(string pressLabel, string actionLabel, string inputLabel)
	{
		this.PressLabel = pressLabel;
		this.ActionLabel = actionLabel;
		this.InputLabel = inputLabel;
		this.HasInput = true;
	}

	public UINotificationDataVO(string pressLabel, string actionLabel, InputControlType inputControlType)
	{
		this.PressLabel = pressLabel;
		this.ActionLabel = actionLabel;
		this.InputControlType = inputControlType;
		this.HasInput = true;
	}
}
