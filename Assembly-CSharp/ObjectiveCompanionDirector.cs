using UnityEngine;

public class ObjectiveCompanionDirector : Objective
{
	[SerializeField]
	private CompanionDirector m_CompanionDirector;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private bool m_DisposeCompanionOnUnload;

	protected override void InternalInitialize()
	{
		this.m_CompanionDirector.Initialize(this.m_StartLocation);
	}

	protected override void InternalDisable()
	{
		this.m_CompanionDirector.UnloadData(this.m_DisposeCompanionOnUnload);
	}
}
