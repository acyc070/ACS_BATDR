public class UIElementGameMenuSettingsButtonDataVO : UIElementDataVO
{
	public string Label;

	public GameMenuSettingsType SettingsType;

	public UIElementGameMenuSettingsButtonDataVO(string prefabKey, string label, GameMenuSettingsType settingsType)
		: base(prefabKey)
	{
		this.Label = label;
		this.SettingsType = settingsType;
	}
}
