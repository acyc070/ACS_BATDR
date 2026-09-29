namespace S13Audio;

public interface IS13ControlGroupConditional
{
	bool CheckCondition(S13AudioHandler audioHandler, S13ControlGroupHandler controlGroupHandler, out string resultMessage);

	bool ForcePlay(S13AudioHandler audioHandler, S13ControlGroupHandler controlGroupHandler, out string resultMessage);
}
