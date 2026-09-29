using UnityEngine;

public class UIElement : JMonoBehaviour, IUIElement
{
	private RectTransform m_RectTransform;

	public RectTransform rectTransform => this.m_RectTransform ?? (this.m_RectTransform = base.GetComponent<RectTransform>());

	public string PrefabKey { get; private set; }

	public virtual void Initialize(object _data)
	{
		if (_data != null)
		{
			this.PrefabKey = (_data as UIElementDataVO)?.PrefabKey;
		}
	}
}
