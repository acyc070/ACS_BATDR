using UnityEngine;

public class UIElementGameMenuAbilityDataVO : UIElementDataVO
{
	public string HeaderLabel;

	public string DescriptionLabel;

	public Sprite AbilityIcon;

	public UIElementGameMenuAbilityDataVO(string prefabKey, string headerLabel, string descriptionLabel, Sprite abilityIcon)
		: base(prefabKey)
	{
		this.HeaderLabel = headerLabel;
		this.DescriptionLabel = descriptionLabel;
		this.AbilityIcon = abilityIcon;
	}
}
