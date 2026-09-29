using System;
using UnityEngine;

[Serializable]
public class AbilityDataDirectory : DataDirectory<State.PlayerAbility, AbilityDataObject>
{
	[SerializeField]
	private AbilityStatus m_AbilityStatus;

	public AbilityStatus AbilityStatus => this.m_AbilityStatus;

	public void SetStatus(AbilityStatus status)
	{
		this.m_AbilityStatus = status;
	}
}
