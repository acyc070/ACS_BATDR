public class UIElementTitleDataVO : UIElementDataVO
{
	public string Label;

	public UIElementTitleDataVO(string prefabKey, string label)
		: base(prefabKey)
	{
		this.Label = label;
	}
}
