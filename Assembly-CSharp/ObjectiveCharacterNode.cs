using UnityEngine;

public class ObjectiveCharacterNode : Objective
{
	[SerializeField]
	private Transform m_CharacterNodeParent;

	[SerializeField]
	private bool m_Enable = true;

	private CharacterNode[] m_CharacterNodes;

	protected override void InternalInitialize()
	{
		this.SetNodes();
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		this.SetNodes();
	}

	private void SetNodes()
	{
		this.m_CharacterNodes = this.m_CharacterNodeParent.GetComponentsInChildren<CharacterNode>(includeInactive: true);
		CharacterNode[] characterNodes;
		characterNodes = this.m_CharacterNodes;
		for (int i = 0; i < characterNodes.Length; i = unchecked(i + 1))
		{
			characterNodes[i].SetActive(this.m_Enable);
		}
	}
}
