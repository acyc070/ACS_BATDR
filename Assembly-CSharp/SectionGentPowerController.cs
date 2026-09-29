using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentPowerController : SectionController
{
	private GentPower[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<GentPower>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			this.m_Group[i].Initialize();
			yield return null;
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_Group = null;
	}
}
