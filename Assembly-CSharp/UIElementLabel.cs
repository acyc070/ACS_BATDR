using TMPro;
using UnityEngine;

public class UIElementLabel : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	private UIElementLabelDataVO m_DataVO;

	public TextMeshProUGUI Label => this.m_Label;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementLabelDataVO)_data;
		this.m_Label.text = this.m_DataVO.Label;
		this.m_Label.color = this.m_DataVO.Color;
		this.m_Label.fontSizeMax = this.m_DataVO.FontSize;
		base.rectTransform.sizeDelta = this.m_DataVO.SizeDelta;
	}
}
