using UnityEngine;

public class MeatlyContent : JMonoBehaviour
{
	[SerializeField]
	private MeatlyDirector m_Director;

	[SerializeField]
	private CutsceneActivator m_Activator;

	public MeatlyDirector Director => this.m_Director;

	public CutsceneActivator Activator => this.m_Activator;
}
