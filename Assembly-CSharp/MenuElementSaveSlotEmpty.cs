using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementSaveSlotEmpty : UIElement
{
	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_SaveSlotLabel;

	[SerializeField]
	private TextMeshProUGUI m_NewGameSaveLabel;

	[Header("Images")]
	[SerializeField]
	private Image[] m_Images;

	[Header("Colors")]
	[SerializeField]
	protected Color m_Color;

	[SerializeField]
	protected Color m_ColorReal;

	private UIElementSaveSlotEmptyDataVO m_DataVO;

	protected Color Color
	{
		get
		{
			if (!GameManager.Instance.IsRealWorld)
			{
				return this.m_Color;
			}
			return this.m_ColorReal;
		}
	}

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = _data as UIElementSaveSlotEmptyDataVO;
		Image[] images;
		images = this.m_Images;
		for (int i = 0; i < images.Length; i = unchecked(i + 1))
		{
			images[i].color = this.Color;
		}
		this.m_SaveSlotLabel.text = this.m_DataVO.SaveSlotLabel;
		this.m_SaveSlotLabel.color = this.Color;
		this.m_NewGameSaveLabel.text = this.m_DataVO.NewGameSaveLabel;
		this.m_NewGameSaveLabel.color = this.Color;
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
