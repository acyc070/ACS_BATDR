using UnityEngine;

public class SectionZone : EventTrigger
{
	[Header("SectionZone Options")]
	[SerializeField]
	private string m_ZoneName;

	public string Name => this.m_ZoneName;

	protected override void InternalInitialize()
	{
		this.SetActive(active: true);
	}

	public Transform GetClosestTarget(CompanionNode[] targets, Transform primaryTarget, Transform previousTarget = null)
	{
		Transform result;
		result = null;
		float num;
		num = float.PositiveInfinity;
		Vector3 position;
		position = primaryTarget.position;
		foreach (CompanionNode companionNode in targets)
		{
			if (!(previousTarget != null) || !(companionNode.transform == previousTarget))
			{
				float num2;
				num2 = Vector3.Distance(companionNode.transform.position, position);
				if (num2 < num)
				{
					result = companionNode.transform;
					num = num2;
				}
			}
		}
		return result;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
