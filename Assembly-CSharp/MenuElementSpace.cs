using UnityEngine;

public class MenuElementSpace : UIElement
{
	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		base.rectTransform.sizeDelta = new Vector2(base.rectTransform.sizeDelta.x, (_data as UIElementSpaceDataVO).Space);
	}
}
