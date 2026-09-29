using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementGameMenuWeapon : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLabel;

	[SerializeField]
	private Image m_UpgradeIcon;

	private UIElementGameMenuWeaponDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementGameMenuWeaponDataVO)_data;
		this.m_HeaderLabel.text = this.m_DataVO.HeaderLabel;
		this.m_DescriptionLabel.text = this.m_DataVO.DescriptionLabel;
		this.m_UpgradeIcon.sprite = this.m_DataVO.UpgradeIcon;
	}
}
