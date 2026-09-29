using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementUpgradeStatus : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private Image[] m_Levels;

	public UIElementUpgradeStatusDataVO Data { get; private set; }

	public override void Initialize(object _data)
	{
		this.Data = (UIElementUpgradeStatusDataVO)_data;
		this.m_Label.text = this.Data.Label;
		unchecked
		{
			for (int i = 0; i < this.m_Levels.Length; i++)
			{
				this.m_Levels[i].enabled = i + 1 <= this.Data.Level;
			}
		}
	}
}
