using System;
using UnityEngine;

public class Piano : JMonoBehaviour
{
	[SerializeField]
	private InteractableConnector m_InteractableConnector;

	[SerializeField]
	private MovableContent m_MovableContent;

	[SerializeField]
	private Transform m_SuccessLocation;

	private bool m_IsSuccess;

	private bool m_IsComplete;

	public event EventHandler OnFail;

	public event EventHandler OnSuccess;

	public void Initialize()
	{
		this.RemoveListeners();
		this.m_MovableContent.OnExit += this.HandleMovableContentOnExit;
		this.m_InteractableConnector.OnInteracted += this.HandleInteractableConnectorOnInteracted;
		this.CheckSuccess();
	}

	private void HandleInteractableConnectorOnInteracted(object sender, EventArgs e)
	{
		if (!this.m_IsComplete)
		{
			if (this.m_IsSuccess)
			{
				this.RemoveListeners();
				this.m_IsComplete = true;
				this.OnSuccess.Send(this);
			}
			else
			{
				this.OnFail.Send(this);
			}
		}
	}

	private void HandleMovableContentOnExit(object sender, EventArgs e)
	{
		this.CheckSuccess();
	}

	private void CheckSuccess()
	{
		if (this.m_MovableContent.transform.position == this.m_SuccessLocation.position)
		{
			this.m_IsSuccess = true;
		}
		else
		{
			this.m_IsSuccess = false;
		}
	}

	private void RemoveListeners()
	{
		this.m_MovableContent.OnExit -= this.HandleMovableContentOnExit;
		this.m_InteractableConnector.OnInteracted -= this.HandleInteractableConnectorOnInteracted;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.OnFail = null;
		this.OnSuccess = null;
		base.OnDisposed();
	}
}
