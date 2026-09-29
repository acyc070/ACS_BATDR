using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementResource : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private Image m_Icon;

	private UIElementResourceDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementResourceDataVO)_data;
		this.m_Icon.sprite = this.m_DataVO.Sprite;
		this.m_Icon.SetNativeSize();
		this.m_Label.text = this.m_DataVO.Amount.ToString();
	}
}
