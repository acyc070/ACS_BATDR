using System;
using UnityEngine;

public class PictureFrame : JMonoBehaviour
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		this.m_Interactable.OnInteract -= this.HandleOnInteract;
		this.m_Interactable.OnInteract += this.HandleOnInteract;
		this.m_Interactable.SetActive(active: true);
	}

	private void HandleOnInteract(object sender, EventArgs e)
	{
		this.m_Interactable.OnInteract -= this.HandleOnInteract;
		this.m_ActiveSetter.SetActiveTrue();
		this.OnComplete.Send(this);
	}

	public void ForceComplete()
	{
		this.m_Interactable.OnInteract -= this.HandleOnInteract;
		this.m_Interactable.Dispose();
		this.m_ActiveSetter.SetActiveTrue();
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		if (this.m_Interactable != null)
		{
			this.m_Interactable.OnInteract -= this.HandleOnInteract;
		}
		base.OnDisposed();
	}
}
