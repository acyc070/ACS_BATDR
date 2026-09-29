using TMPro;
using UnityEngine;

public class UIElementGameMenuWeaponHeader : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLabel;

	private UIElementGameMenuWeaponHeaderDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementGameMenuWeaponHeaderDataVO)_data;
		this.m_HeaderLabel.text = this.m_DataVO.HeaderLabel;
		this.m_DescriptionLabel.text = this.m_DataVO.DescriptionLabel;
	}
}
