using System;
using UnityEngine;

public class MeshOutlineConnector : JComponent
{
	[SerializeField]
	private MeshOutline m_MeshOutline;

	[Header("Interactable Group")]
	[SerializeField]
	private Interactable[] m_Interactables;

	public override void Start()
	{
		if (this.m_MeshOutline != null)
		{
			this.AddListeners();
		}
	}

	private void HandleInteractableOnEnter(object sender, EventArgs e)
	{
		this.m_MeshOutline.Enter();
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.m_MeshOutline.Interact();
	}

	private void HandleInteractableOnExit(object sender, EventArgs e)
	{
		this.m_MeshOutline.Exit();
	}

	private void AddListeners()
	{
		for (int i = 0; i < this.m_Interactables.Length; i = unchecked(i + 1))
		{
			Interactable obj;
			obj = this.m_Interactables[i];
			obj.OnEnter -= this.HandleInteractableOnEnter;
			obj.OnEnter += this.HandleInteractableOnEnter;
			obj.OnInteract -= this.HandleInteractableOnInteract;
			obj.OnInteract += this.HandleInteractableOnInteract;
			obj.OnExit -= this.HandleInteractableOnExit;
			obj.OnExit += this.HandleInteractableOnExit;
		}
	}

	private void RemoveListeners()
	{
		for (int i = 0; i < this.m_Interactables.Length; i = unchecked(i + 1))
		{
			Interactable obj;
			obj = this.m_Interactables[i];
			obj.OnEnter -= this.HandleInteractableOnEnter;
			obj.OnInteract -= this.HandleInteractableOnInteract;
			obj.OnExit -= this.HandleInteractableOnExit;
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
