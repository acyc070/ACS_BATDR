using UnityEngine;
using UnityEngine.UI;

public class UIElementMemoryImage : UIElement
{
	[Header("Image")]
	[SerializeField]
	protected Image m_Image;

	private UIElementMemoryDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementMemoryDataVO)_data;
		if (this.m_DataVO.Sprite != null)
		{
			this.m_Image.sprite = this.m_DataVO.Sprite;
			this.m_Image.SetNativeSize();
			Vector2 sizeDelta;
			sizeDelta = base.rectTransform.sizeDelta;
			sizeDelta.y = this.m_Image.rectTransform.sizeDelta.y * 2f;
			base.rectTransform.sizeDelta = sizeDelta;
		}
		else
		{
			this.m_Image.enabled = false;
		}
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
