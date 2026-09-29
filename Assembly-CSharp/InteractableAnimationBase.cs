using System;
using DG.Tweening;
using UnityEngine;

public class InteractableAnimationBase : InteractableInputDisplay
{
	[Header("Locations")]
	[SerializeField]
	protected Transform m_StartLocation;

	[SerializeField]
	protected Transform m_EndLocation;

	[Header("Options")]
	[SerializeField]
	protected bool m_IsSingleInteraction;

	[SerializeField]
	protected bool m_HideCrosshair = true;

	protected Sequence m_Sequence;

	public Transform StartLocation => this.m_StartLocation;

	public Transform EndLocation => this.m_EndLocation;

	public bool HasStartLocation => this.m_StartLocation != null;

	public bool HasEndLocation => this.m_EndLocation != null;

	public event EventHandler OnInteractionComplete;

	public event EventHandler OnInteractionExit;

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		base.OnInternalInteract(origin, hit, sender);
		GameManager.Instance.Player.Interaction.ResetInteraction();
		this.Enter();
	}

	public virtual void Enter()
	{
	}

	public virtual void ForceEnter(Quaternion rotationX, Quaternion rotationY)
	{
	}

	public virtual void Exit()
	{
	}

	protected void SendToEndLocation()
	{
		if (this.HasEndLocation)
		{
			Vector3 vector;
			vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
			GameManager.Instance.Player.transform.position = this.m_EndLocation.position + vector;
			GameManager.Instance.Player.transform.eulerAngles = this.m_EndLocation.eulerAngles;
		}
	}

	protected void CheckCrosshair(bool show)
	{
		if (this.m_HideCrosshair)
		{
			if (show)
			{
				GameManager.Instance.ShowCrosshair(base.m_IsReal);
			}
			else
			{
				GameManager.Instance.HideCrosshair();
			}
		}
	}

	protected void SendOnInteractionComplete()
	{
		this.OnInteractionComplete.Send(this);
	}

	protected void SendOnInteractionExit()
	{
		this.OnInteractionExit.Send(this);
	}

	protected void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		this.m_Sequence?.Kill();
		this.m_Sequence = null;
	}

	protected override void OnDisposed()
	{
		this.OnInteractionComplete = null;
		this.OnInteractionExit = null;
		this.KillSequence();
		base.OnDisposed();
	}
}
