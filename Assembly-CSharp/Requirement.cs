using UnityEngine;

public class Requirement : ScriptableObject
{
	[SerializeField]
	protected SectionID m_SectionID;

	[SerializeField]
	protected int m_ID = -1;

	public SectionID SectionID => this.m_SectionID;

	public int ID => this.m_ID;

	public bool IsComplete()
	{
		return this.InternalIsComplete();
	}

	protected virtual bool InternalIsComplete()
	{
		return true;
	}
}
