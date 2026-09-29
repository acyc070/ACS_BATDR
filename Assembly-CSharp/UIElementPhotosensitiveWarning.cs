using TMPro;
using UnityEngine;

public class UIElementPhotosensitiveWarning : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLabel;

	private UIElementPhotosensitiveWarningDataVO m_DataVO;

	public TextMeshProUGUI HeaderLabel => this.m_HeaderLabel;

	public TextMeshProUGUI DescriptionLabel => this.m_DescriptionLabel;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementPhotosensitiveWarningDataVO)_data;
		this.m_HeaderLabel.text = this.m_DataVO.Header;
		this.m_DescriptionLabel.text = this.m_DataVO.Description;
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
