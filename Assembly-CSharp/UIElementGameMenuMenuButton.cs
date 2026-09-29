public class UIElementGameMenuMenuButton : UIElementButtonLabel
{
	private GameMenuMenuType m_MenuType;

	public GameMenuMenuType MenuType => this.m_MenuType;

	protected override bool m_ForceTextSize => false;

	public void SetType(GameMenuMenuType menuType)
	{
		this.m_MenuType = menuType;
	}
}
