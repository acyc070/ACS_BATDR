using TMPro;
using UnityEngine;

public class UIElementGameMenuWeaponEmpty : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	private UIElementGameMenuWeaponEmptyDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementGameMenuWeaponEmptyDataVO)_data;
		this.m_Label.text = this.m_DataVO.Label;
	}
}
