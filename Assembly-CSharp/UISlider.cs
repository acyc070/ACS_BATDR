using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlider : Slider
{
	public event EventHandler OnEnter;

	public event EventHandler OnExit;

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
}
