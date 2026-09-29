using UnityEngine;

public class MemoryContent : JMonoBehaviour
{
	[SerializeField]
	private MemoryDirector m_Director;

	[SerializeField]
	private CutsceneActivator m_Activator;

	[SerializeField]
	private GameObject m_InitializeContent;

	[SerializeField]
	private Transform m_StartLocation;

	public MemoryDirector Director => this.m_Director;

	public CutsceneActivator Activator => this.m_Activator;

	public GameObject InitializeContent => this.m_InitializeContent;

	public Transform StartLocation => this.m_StartLocation;

	public void Initialize()
	{
		if (this.m_InitializeContent != null)
		{
			this.m_InitializeContent.SetActive(value: true);
		}
	}
}
