public class UIElementTitlePauseDataVO : UIElementDataVO
{
	public string HeaderLabel;

	public string BodyLabel;

	public string FooterLabel;

	public UIElementTitlePauseDataVO(string prefabKey, string headerLabel = "", string bodyLabel = "", string footerLabel = "")
		: base(prefabKey)
	{
		this.HeaderLabel = headerLabel;
		this.BodyLabel = bodyLabel;
		this.FooterLabel = footerLabel;
	}
}
