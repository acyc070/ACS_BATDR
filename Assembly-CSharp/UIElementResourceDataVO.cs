using UnityEngine;

public class UIElementResourceDataVO : UIElementDataVO
{
	public Sprite Sprite;

	public int Amount;

	public UIElementResourceDataVO(string prefabKey, Sprite sprite, int amount)
		: base(prefabKey)
	{
		this.Sprite = sprite;
		this.Amount = amount;
	}
}
