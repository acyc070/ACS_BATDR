using TMPro;
using UnityEngine;

public class UIElementMenuTitlePause : UIElement
{
	[Header("Menu Pause Labels")]
	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_BodyLabel;

	[SerializeField]
	private TextMeshProUGUI m_FooterLabel;

	public string HeaderLabel => this.m_HeaderLabel.text;

	public string BodyLabel => this.m_BodyLabel.text;

	public string FooterLabel => this.m_FooterLabel.text;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_HeaderLabel.text = (_data as UIElementTitlePauseDataVO).HeaderLabel;
		this.m_BodyLabel.text = (_data as UIElementTitlePauseDataVO).BodyLabel;
		this.m_FooterLabel.text = (_data as UIElementTitlePauseDataVO).FooterLabel;
	}
}
