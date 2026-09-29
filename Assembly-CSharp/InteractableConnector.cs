using System;
using UnityEngine;

public class InteractableConnector : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsActive;

	[SerializeField]
	private Interactable[] m_Interactables;

	public event EventHandler OnInteracted;

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	public override void Start()
	{
		if (this.m_Interactables != null)
		{
			for (int i = 0; i < this.m_Interactables.Length; i = unchecked(i + 1))
			{
				Interactable obj;
				obj = this.m_Interactables[i];
				obj.OnInteract -= this.HandleInteractableOnInteract;
				obj.OnInteract += this.HandleInteractableOnInteract;
			}
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.OnInteracted.Send(this);
	}

	private void RemoveListeners()
	{
		if (this.m_Interactables != null)
		{
			for (int i = 0; i < this.m_Interactables.Length; i = unchecked(i + 1))
			{
				this.m_Interactables[i].OnInteract -= this.HandleInteractableOnInteract;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.OnInteracted = null;
		base.OnDisposed();
	}
}
