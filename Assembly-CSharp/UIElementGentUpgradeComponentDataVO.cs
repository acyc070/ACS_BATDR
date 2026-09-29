using UnityEngine;

public class UIElementGentUpgradeComponentDataVO
{
	public Sprite Icon;

	public int Amount;

	public int Max;

	public UIElementGentUpgradeComponentDataVO(string icon, int amount, int max)
	{
		this.Icon = GameManager.Instance.AssetManager.GetAsset<IconData>(icon).Icon;
		this.Amount = amount;
		this.Max = max;
	}
}
