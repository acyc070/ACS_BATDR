using System;
using UnityEngine;

public class InteractableController : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationBase m_Interactable;

	public event EventHandler OnEntered;

	public event EventHandler OnExited;

	public override void Awake()
	{
		this.m_Interactable.OnInteractionComplete += this.HandleInteractableOnEntered;
		this.m_Interactable.OnInteractionExit += this.HandleInteractableOnExited;
	}

	private void HandleInteractableOnEntered(object sender, EventArgs e)
	{
		this.InternalOnEntered();
		this.OnEntered.Send(this);
	}

	protected virtual void InternalOnEntered()
	{
	}

	private void HandleInteractableOnExited(object sender, EventArgs e)
	{
		this.InternalOnExited();
		this.OnExited.Send(this);
	}

	protected virtual void InternalOnExited()
	{
	}

	protected override void OnDisposed()
	{
		this.OnEntered = null;
		this.OnExited = null;
		base.OnDisposed();
	}
}
