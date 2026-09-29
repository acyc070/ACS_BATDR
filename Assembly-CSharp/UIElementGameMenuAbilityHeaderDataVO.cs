public class UIElementGameMenuAbilityHeaderDataVO : UIElementDataVO
{
	public string HeaderLabel;

	public string DescriptionLabel;

	public UIElementGameMenuAbilityHeaderDataVO(string prefabKey, string headerLabel, string descriptionLabel)
		: base(prefabKey)
	{
		this.HeaderLabel = headerLabel;
		this.DescriptionLabel = descriptionLabel;
	}
}
