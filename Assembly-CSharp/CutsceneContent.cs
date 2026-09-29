using System;
using UnityEngine;

public class CutsceneContent : JMonoBehaviour
{
	[SerializeField]
	private CutsceneDirector m_Director;

	[SerializeField]
	private CutsceneActivator m_Activator;

	[SerializeField]
	private GameObject m_InitializeContent;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private bool m_CompleteOnPlay;

	public CutsceneDirector Director => this.m_Director;

	public CutsceneActivator Activator => this.m_Activator;

	public GameObject InitializeContent => this.m_InitializeContent;

	public Transform StartLocation => this.m_StartLocation;

	public bool CompleteOnPlay => this.m_CompleteOnPlay;

	public event EventHandler OnForceComplete;

	public void ForceComplete()
	{
		this.OnForceComplete.Send(this);
	}

	public void Initialize()
	{
		if (this.m_InitializeContent != null)
		{
			this.m_InitializeContent.SetActive(value: true);
		}
	}

	protected override void OnDisposed()
	{
		this.OnForceComplete = null;
		base.OnDisposed();
	}
}
