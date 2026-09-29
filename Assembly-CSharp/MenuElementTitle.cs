using TMPro;
using UnityEngine;

public class MenuElementTitle : UIElement
{
	[Header("Menu Title Label")]
	[SerializeField]
	private TextMeshProUGUI m_Label;

	public string Label => this.m_Label.text;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		if ((bool)this.m_Label)
		{
			this.m_Label.text = (_data as UIElementTitleDataVO).Label;
		}
	}
}
