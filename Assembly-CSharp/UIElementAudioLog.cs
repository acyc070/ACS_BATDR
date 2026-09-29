using TMPro;
using UnityEngine;

public class UIElementAudioLog : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_Label;

	private UIElementAudioLogDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementAudioLogDataVO)_data;
		this.m_Label.text = TextUtility.GetKey(this.m_DataVO.ID.ToString());
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
