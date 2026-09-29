using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionIllusionController : SectionController
{
	private Illusion[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<Illusion>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			Illusion illusion;
			illusion = this.m_Group[i];
			if (!illusion.InitializeOnAwake)
			{
				illusion.Initialize();
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
