using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentCardExchangeController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentCardExchangeGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public GentCardExchangeGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			this.m_Group[i]?.Controller.Content.Enable();
		}
		yield return null;
	}
}
