using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElementGameMenuObjective : UIElement
{
	[Header("Containers")]
	[SerializeField]
	private RectTransform m_Space;

	[SerializeField]
	private RectTransform m_TitleContainer;

	[SerializeField]
	private RectTransform m_Decor;

	[Header("Images")]
	[SerializeField]
	private Image m_DecorImage;

	[Header("Colors")]
	[SerializeField]
	private Color m_DecorColor;

	[SerializeField]
	private Color m_DecorColorReal;

	private UIElementGameMenuObjectiveDataVO m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementGameMenuObjectiveDataVO)_data;
		this.m_DecorImage.color = (GameManager.Instance.IsRealWorld ? this.m_DecorColorReal : this.m_DecorColor);
		UIElement uIElement;
		uIElement = UIManager.CreateElement(this.m_TitleContainer, this.m_DataVO.Title.PrefabKey, this.m_DataVO.Title);
		Vector2 sizeDelta;
		sizeDelta = base.rectTransform.sizeDelta;
		sizeDelta.y += this.m_Space.sizeDelta.y;
		sizeDelta.y += uIElement.rectTransform.sizeDelta.y;
		sizeDelta.y += this.m_Decor.sizeDelta.y;
		List<UIElementDataVO>.Enumerator enumerator;
		enumerator = this.m_DataVO.Objectives.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!(enumerator.Current.PrefabKey == string.Empty))
			{
				UIElement uIElement2;
				uIElement2 = UIManager.CreateElement(base.rectTransform, enumerator.Current.PrefabKey, enumerator.Current);
				sizeDelta.y += uIElement2.rectTransform.sizeDelta.y;
			}
		}
		base.rectTransform.sizeDelta = sizeDelta;
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
