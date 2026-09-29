using TMPro;
using UnityEngine;

public class UIElementGameMenuAbilityHeader : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLabel;

	private UIElementGameMenuAbilityHeaderDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementGameMenuAbilityHeaderDataVO)_data;
		this.m_HeaderLabel.text = this.m_DataVO.HeaderLabel;
		this.m_DescriptionLabel.text = this.m_DataVO.DescriptionLabel;
	}
}
