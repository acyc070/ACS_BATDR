using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementGameMenuAbility : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLabel;

	[SerializeField]
	private Image m_AbilityIcon;

	private UIElementGameMenuAbilityDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementGameMenuAbilityDataVO)_data;
		this.m_HeaderLabel.text = this.m_DataVO.HeaderLabel;
		this.m_DescriptionLabel.text = this.m_DataVO.DescriptionLabel;
		this.m_AbilityIcon.sprite = this.m_DataVO.AbilityIcon;
	}
}
