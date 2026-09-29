namespace S13Audio;

public interface IS13Switchable
{
	S13ScriptableSwitch CurrentScriptableSwitch { get; }

	bool UsesScriptableSwitch(S13ScriptableSwitch scriptableSwitch);
}
