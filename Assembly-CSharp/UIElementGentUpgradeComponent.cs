using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementGentUpgradeComponent : UIElement
{
	[SerializeField]
	private TextMeshProUGUI m_AmountLabel;

	[SerializeField]
	private Image m_Icon;

	[SerializeField]
	private GameObject m_Off;

	[SerializeField]
	private GameObject m_On;

	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	private UIElementGentUpgradeComponentDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementGentUpgradeComponentDataVO)_data;
		this.m_AmountLabel.text = this.m_DataVO.Amount + " / " + this.m_DataVO.Max;
		bool flag;
		flag = this.m_DataVO.Amount >= this.m_DataVO.Max;
		this.m_Icon.sprite = this.m_DataVO.Icon;
		if ((bool)this.m_On)
		{
			this.m_On.SetActive(flag);
		}
		if ((bool)this.m_Off)
		{
			this.m_Off.SetActive(!flag);
		}
		if ((bool)this.m_CanvasGroup)
		{
			this.m_CanvasGroup.alpha = (flag ? 1f : 0.2f);
		}
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
