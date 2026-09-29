public class UIElementGameMenuMenuButtonDataVO : UIElementDataVO
{
	public string Label;

	public GameMenuMenuType MenuType;

	public UIElementGameMenuMenuButtonDataVO(string prefabKey, string label, GameMenuMenuType menuType)
		: base(prefabKey)
	{
		this.Label = label;
		this.MenuType = menuType;
	}
}
