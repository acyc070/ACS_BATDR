using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionCharacterNodeController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private CharacterNodeGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public CharacterNodeGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		yield return null;
	}
}
