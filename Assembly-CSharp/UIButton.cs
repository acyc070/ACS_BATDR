using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIButton : JMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField]
	public bool interactable = true;

	public bool selected { get; set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnClick;

	public event EventHandler OnMouseDragBegin;

	public event EventHandler OnMouseDrag;

	public event EventHandler OnMouseDragEnd;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.interactable && !this.selected)
		{
			this.OnEnter.Send(this);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.interactable && !this.selected)
		{
			this.OnExit.Send(this);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.interactable && !this.selected)
		{
			this.OnClick.Send(this);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (this.interactable && !this.selected)
		{
			this.OnMouseDragBegin.Send(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (this.interactable && !this.selected)
		{
			this.OnMouseDrag.Send(eventData);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (this.interactable && !this.selected)
		{
			this.OnMouseDragEnd.Send(eventData);
		}
	}

	protected override void OnDisposed()
	{
		this.OnClick = null;
		this.OnEnter = null;
		this.OnExit = null;
		this.OnMouseDragBegin = null;
		this.OnMouseDrag = null;
		this.OnMouseDragEnd = null;
		base.OnDisposed();
	}
}
