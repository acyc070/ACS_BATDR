using UnityEngine;

public class ObjectiveZone : Objective
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private string m_Zone;

	protected override void InternalInitialize()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.SetZone(this.m_Zone);
			GameManager.Instance.Player.SetSectionID(this.m_SectionID);
		}
		base.SendOnComplete();
	}
}
