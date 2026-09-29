using TMPro;
using UnityEngine;

public class UIElementMemo : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_MemoLabel;

	private UIElementMemoDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementMemoDataVO)_data;
		this.m_MemoLabel.text = TextUtility.GetKey(this.m_DataVO.ID.ToString());
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
