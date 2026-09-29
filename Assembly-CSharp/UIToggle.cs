using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIToggle : Toggle
{
	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnClick;

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.OnEnter.Send(this);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.OnExit.Send(this);
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		this.OnClick.Send(this);
	}
}
