using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionRespawnController : SectionController
{
	private PlayerRespawn[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<PlayerRespawn>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			this.m_Group[i].Initialize();
		}
		yield return null;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_Group = null;
	}
}
