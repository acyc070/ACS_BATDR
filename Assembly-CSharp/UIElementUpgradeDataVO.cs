using System.Collections.Generic;
using UnityEngine;

public class UIElementUpgradeDataVO
{
	public string Header;

	public string Description;

	public Sprite Icon;

	public List<UIElementUpgradeLevelDataVO> Levels;

	public UpgradeType UpgradeType;

	public UIElementUpgradeDataVO(string header, string description, Sprite icon, List<UIElementUpgradeLevelDataVO> levels, UpgradeType upgradeType)
	{
		this.Header = header;
		this.Description = description;
		this.Icon = icon;
		this.Levels = levels;
		this.UpgradeType = upgradeType;
	}
}
