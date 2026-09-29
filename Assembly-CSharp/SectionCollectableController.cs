using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionCollectableController : SectionController
{
	private Collectable[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<Collectable>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			Collectable collectable;
			collectable = this.m_Group[i];
			if (!collectable.InitializeOnAwake)
			{
				collectable.Initialize();
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
