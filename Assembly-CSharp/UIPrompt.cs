using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrompt : UIController
{
	[Header("Containers")]
	[SerializeField]
	private RectTransform m_ButtonContainer;

	[Header("Background")]
	[SerializeField]
	private Image m_BackgroundImage;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_MessageLabel;

	[Header("Title Check")]
	[SerializeField]
	private bool m_IsTitle;

	private UIPromptDataVO m_DataVO;

	private List<UIElement> m_Buttons = new List<UIElement>();

	private int m_ButtonIndex;

	private bool m_IsSelected;

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UIPromptDataVO)_data;
		if (this.m_BackgroundImage != null)
		{
			this.m_BackgroundImage.color = (GameManager.Instance.IsRealWorld ? UIColors.INACTIVE_LABEL_REAL : UIColors.INACTIVE_LABEL);
		}
		this.m_HeaderLabel.text = this.m_DataVO.HeaderLabel;
		this.m_HeaderLabel.color = ((this.m_IsTitle || GameManager.Instance.IsRealWorld) ? UIColors.ACTIVE_LABEL_REAL : UIColors.ACTIVE_LABEL);
		this.m_MessageLabel.text = this.m_DataVO.MessageLabel;
		this.m_MessageLabel.color = ((this.m_IsTitle || GameManager.Instance.IsRealWorld) ? UIColors.ACTIVE_LABEL_REAL : UIColors.ACTIVE_LABEL);
		this.m_Buttons.Add(UIManager.CreateElement(this.m_ButtonContainer, this.m_DataVO.m_ButtonA.PrefabKey, this.m_DataVO.m_ButtonA));
		if (this.m_DataVO.m_ButtonB != null)
		{
			this.m_Buttons.Add(UIManager.CreateElement(this.m_ButtonContainer, this.m_DataVO.m_ButtonB.PrefabKey, this.m_DataVO.m_ButtonB));
		}
	}

	private void Update()
	{
		if (this.m_IsSelected || base.IsDisposed || !PlayerInput.CheckController())
		{
			return;
		}
		if (PlayerInput.ControllerAction1())
		{
			UIElement uIElement;
			uIElement = this.m_Buttons[0];
			if (uIElement != null)
			{
				if (uIElement is UIElementButton { Callback: var callback })
				{
					callback?.Invoke();
				}
				else if (uIElement is MenuElementButton { Callback: { } callback2 })
				{
					callback2();
				}
			}
			this.m_IsSelected = true;
		}
		else
		{
			if (!PlayerInput.ControllerAction2())
			{
				return;
			}
			UIElement uIElement2;
			uIElement2 = this.m_Buttons[1];
			if (uIElement2 != null)
			{
				if (uIElement2 is UIElementButton { Callback: var callback3 })
				{
					callback3?.Invoke();
				}
				else if (uIElement2 is MenuElementButton { Callback: { } callback4 })
				{
					callback4();
				}
			}
			this.m_IsSelected = true;
		}
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		if (this.m_Buttons != null)
		{
			this.m_Buttons.Clear();
			this.m_Buttons = null;
		}
		base.OnDisposed();
	}
}
