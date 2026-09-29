using TMPro;
using UnityEngine;

public class UIElementLabelDataVO(string prefabKey) : UIElementDataVO(prefabKey)
{
	public string Label;

	public Color Color;

	public float FontSize;

	public Vector2 SizeDelta;

	public TextAlignmentOptions Alignment = TextAlignmentOptions.MidlineLeft;
}
