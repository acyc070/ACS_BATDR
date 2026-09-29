using System;
using UnityEngine;

public class GentLockContent : JMonoBehaviour
{
	[SerializeField]
	private GentLockDirector m_Director;

	[SerializeField]
	private CutsceneActivator m_Activator;

	[SerializeField]
	private Transform m_StartLocation;

	public GentLockDirector Director => this.m_Director;

	public CutsceneActivator Activator => this.m_Activator;

	public Transform StartLocation => this.m_StartLocation;

	public event EventHandler OnForceComplete;

	public void ForceComplete()
	{
		this.OnForceComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnForceComplete = null;
		base.OnDisposed();
	}
}
