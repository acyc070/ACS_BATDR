using System;
using UnityEngine;

[Serializable]
public class AbilityDataObject : DataObject<State.PlayerAbility, AbilityDataObject>, IDataObject<State.PlayerAbility>, IDataObject
{
	[SerializeField]
	private State.PlayerAbility m_AbilityID;

	public override State.PlayerAbility ID => this.m_AbilityID;

	protected override void Deserialize()
	{
		this.m_AbilityID = base.m_ID;
	}
}
