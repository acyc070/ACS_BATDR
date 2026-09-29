public class UIElementGameMenuWeaponEmptyDataVO : UIElementDataVO
{
	public string Label;

	public UIElementGameMenuWeaponEmptyDataVO(string prefabKey, string label)
		: base(prefabKey)
	{
		this.Label = label;
	}
}
