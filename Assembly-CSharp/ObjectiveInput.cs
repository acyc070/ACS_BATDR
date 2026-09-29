using System;
using UnityEngine;

public class ObjectiveInput : Objective
{
	[Header("Action Event")]
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[Header("Input Options")]
	[SerializeField]
	private InteractionType m_InteractionType;

	[Header("Real World Check")]
	[SerializeField]
	private bool m_IsReal;

	private bool m_IsInitialized;

	protected override void InternalInitialize()
	{
		if (this.m_ActionEvent != null)
		{
			this.AddListeners();
		}
		else
		{
			this.DisplayInput();
		}
	}

	protected override void InternalInactive()
	{
		this.RemoveListeners();
		this.ClearInteraction();
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
		this.ClearInteraction();
	}

	private void ClearInteraction()
	{
		GameManager.Instance.Player.Interaction.ResetInteraction();
		GameManager.Instance.ClearInteraction();
	}

	protected override void InternalUpdate()
	{
		if (this.m_IsInitialized && PlayerInput.InteractOnReleased())
		{
			this.m_IsInitialized = false;
			base.SendOnComplete();
		}
	}

	private void DisplayInput()
	{
		string text;
		text = TextUtility.GetKey(this.m_InteractionType.ToString());
		if (this.m_IsReal)
		{
			text = text.ToUpper();
		}
		GameManager.Instance.ShowInteraction(text, this.m_IsReal);
		this.m_IsInitialized = true;
	}

	private void HandleActionEventOnEnter(object sender, EventArgs e)
	{
		this.DisplayInput();
	}

	private void HandleActionEventOnExit(object sender, EventArgs e)
	{
		this.m_ActionEvent.ResetAction();
		GameManager.Instance.HideInteraction();
	}

	protected void AddListeners()
	{
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.OnEnter += this.HandleActionEventOnEnter;
			this.m_ActionEvent.OnExit += this.HandleActionEventOnExit;
			this.m_ActionEvent.SetActive(active: true);
		}
	}

	protected override void RemoveListeners()
	{
		if (this.m_ActionEvent != null)
		{
			this.m_ActionEvent.OnEnter -= this.HandleActionEventOnEnter;
			this.m_ActionEvent.OnExit -= this.HandleActionEventOnExit;
			this.m_ActionEvent.SetActive(active: false);
		}
	}
}
