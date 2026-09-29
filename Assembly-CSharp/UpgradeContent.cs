using UnityEngine;

public class UpgradeContent : JMonoBehaviour
{
	[SerializeField]
	private UpgradeDirector m_Director;

	[SerializeField]
	private CutsceneActivator m_Activator;

	[SerializeField]
	private Transform m_StartLocation;

	public UpgradeDirector Director => this.m_Director;

	public CutsceneActivator Activator => this.m_Activator;

	public Transform StartLocation => this.m_StartLocation;
}
