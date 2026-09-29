using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractionCounter : JMonoBehaviour
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private int m_MaxInteractions;

	[SerializeField]
	private float m_Delay;

	[SerializeField]
	private UnityEvent m_Event;

	private int m_Count;

	public override void Start()
	{
		if (!(this.m_Interactable == null))
		{
			this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		unchecked
		{
			this.m_Count++;
			if (this.m_Count >= this.m_MaxInteractions)
			{
				this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
				base.Invoke("Action", this.m_Delay);
			}
		}
	}

	private void Action()
	{
		this.m_Event?.Invoke();
	}
}
