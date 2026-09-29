using UnityEngine;

public class UIElementNotificationBoxLabelDataVO
{
	public string Label = "";

	public Sprite Icon;

	public int Amount;

	public float Delay;

	public UIElementNotificationBoxLabelDataVO(string label, string icon, int amount, float delay = 0f)
	{
		this.Label = label;
		if (icon != "")
		{
			this.Icon = GameManager.Instance.AssetManager.GetAsset<IconData>(icon).Icon;
			this.Amount = amount;
		}
		this.Delay = delay;
	}
}
