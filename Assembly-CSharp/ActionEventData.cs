using System;
using UnityEngine;

[Serializable]
public class ActionEventData : DataObject<int, ActionEventData>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_ActionEventID;

	public override int ID => this.m_ActionEventID;

	protected override void Deserialize()
	{
		this.m_ActionEventID = base.m_ID;
	}
}
