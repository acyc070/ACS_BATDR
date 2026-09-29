public class UIElementTitleMenuButtonDataVO : UIElementDataVO
{
	public string Label;

	public TitleMenuType TitleMenuType;

	public UIElementTitleMenuButtonDataVO(string prefabKey, string label, TitleMenuType titleMenuType)
		: base(prefabKey)
	{
		this.Label = label;
		this.TitleMenuType = titleMenuType;
	}
}
