using System;
using UnityEngine;

[Serializable]
public class AudioLogDataObject : DataObject<AudioLogID, AudioLogDataObject>, IDataObject<AudioLogID>, IDataObject
{
	[SerializeField]
	private AudioLogID m_AudioLogID;

	public override AudioLogID ID => this.m_AudioLogID;

	protected override void Deserialize()
	{
		this.m_AudioLogID = base.m_ID;
	}
}
