using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionJackatoyController : SectionController
{
	private Jackatoy[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<Jackatoy>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			Jackatoy jackatoy;
			jackatoy = this.m_Group[i];
			if (!jackatoy.InitializeOnAwake)
			{
				jackatoy.Initialize();
			}
			yield return null;
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_Group = null;
	}
}
