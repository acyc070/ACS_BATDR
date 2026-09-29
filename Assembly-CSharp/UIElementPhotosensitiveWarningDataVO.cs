public class UIElementPhotosensitiveWarningDataVO : UIElementDataVO
{
	public string Header;

	public string Description;

	public UIElementPhotosensitiveWarningDataVO(string header, string description, string prefabKey)
		: base(prefabKey)
	{
		this.Header = header;
		this.Description = description;
	}
}
