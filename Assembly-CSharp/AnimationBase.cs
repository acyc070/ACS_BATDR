using System;
using DG.Tweening;
using UnityEngine;

public class AnimationBase : JMonoBehaviour
{
	[Header("Locations")]
	[SerializeField]
	protected Transform m_StartLocation;

	[SerializeField]
	protected Transform m_EndLocation;

	[Header("Options")]
	[SerializeField]
	protected bool m_HideCrosshair = true;

	[SerializeField]
	protected bool m_IsReal;

	protected Sequence m_Sequence;

	public bool HasStartLocation => this.m_StartLocation != null;

	public bool HasEndLocation => this.m_EndLocation != null;

	public event EventHandler OnComplete;

	public event EventHandler OnExit;

	public virtual void Enter()
	{
	}

	public virtual void Exit()
	{
	}

	protected void SendOnComplete()
	{
		this.OnComplete.Send(this);
	}

	protected void SendOnExit()
	{
		this.OnExit.Send(this);
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
		this.OnComplete = null;
		this.OnExit = null;
		this.KillSequence();
		base.OnDisposed();
	}
}
