using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINotificationBox : UIController
{
	[SerializeField]
	private RectTransform m_Content;

	private List<UIElementNotificationBoxLabel> m_Labels = new List<UIElementNotificationBoxLabel>();

	private int m_UIElementCount;

	public void Show(string label, string icon, int amount, float delay)
	{
		UIElementNotificationBoxLabel uIElementNotificationBoxLabel;
		uIElementNotificationBoxLabel = (UIElementNotificationBoxLabel)UIManager.CreateElement(_data: new UIElementNotificationBoxLabelDataVO(label, icon, amount, delay), parent: this.m_Content, _prefabKey: "UI/Elements/UIElementNotificationBoxLabel");
		if (uIElementNotificationBoxLabel != null)
		{
			uIElementNotificationBoxLabel.OnComplete -= this.HandleElementOnComplete;
			uIElementNotificationBoxLabel.OnComplete += this.HandleElementOnComplete;
			if (this.m_Labels != null)
			{
				this.m_Labels.Add(uIElementNotificationBoxLabel);
			}
		}
	}

	private void Update()
	{
		if (!base.IsDisposed && GameManager.Instance.GameState != GameState.PAUSE && this.m_Labels != null && this.m_Labels.Count > 0)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_Content);
		}
	}

	private void HandleElementOnComplete(object sender, EventArgs e)
	{
		(sender as UIElementNotificationBoxLabel).OnComplete -= this.HandleElementOnComplete;
		unchecked
		{
			this.m_UIElementCount++;
			if (this.m_UIElementCount >= this.m_Labels.Count)
			{
				this.ForceClear();
			}
		}
	}

	public void ForceClear()
	{
		unchecked
		{
			for (int num = this.m_Labels.Count - 1; num > -1; num--)
			{
				UIElementNotificationBoxLabel uIElementNotificationBoxLabel;
				uIElementNotificationBoxLabel = this.m_Labels[num];
				uIElementNotificationBoxLabel.OnComplete -= this.HandleElementOnComplete;
				uIElementNotificationBoxLabel.Dispose();
			}
			this.m_Labels.Clear();
			this.m_UIElementCount = 0;
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_Labels != null)
		{
			for (int i = 0; i < this.m_Labels.Count; i = unchecked(i + 1))
			{
				this.m_Labels[i].OnComplete -= this.HandleElementOnComplete;
			}
			this.m_Labels.Clear();
			this.m_Labels = null;
		}
		base.OnDisposed();
	}
}
