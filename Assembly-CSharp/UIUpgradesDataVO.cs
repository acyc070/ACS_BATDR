using System.Collections.Generic;

public class UIUpgradesDataVO
{
	public string TitleLabel;

	public string DescriptionLabel;

	public List<UIElementUpgradeDataVO> Upgrades;

	public UIUpgradesDataVO(string titleLabel, string descriptionLabel, List<UIElementUpgradeDataVO> upgrades)
	{
		this.TitleLabel = titleLabel;
		this.DescriptionLabel = descriptionLabel;
		this.Upgrades = upgrades;
	}
}
