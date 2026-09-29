using TMPro;
using UnityEngine;

public class UIElementMemory : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_MemoLabel;

	private UIElementMemoryDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementMemoryDataVO)_data;
		this.m_MemoLabel.text = TextUtility.GetKey(this.m_DataVO.ID.ToString());
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
