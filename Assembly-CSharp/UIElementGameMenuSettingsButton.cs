public class UIElementGameMenuSettingsButton : UIElementButtonLabel
{
	private GameMenuSettingsType m_SettingsType;

	public GameMenuSettingsType SettingsType => this.m_SettingsType;

	protected override bool m_ForceTextSize => false;

	public void SetType(GameMenuSettingsType settingsType)
	{
		this.m_SettingsType = settingsType;
	}
}
