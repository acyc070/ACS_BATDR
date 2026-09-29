using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentSchematicController : SectionController
{
	private GentSchematic[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<GentSchematic>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentSchematic gentSchematic;
			gentSchematic = this.m_Group[i];
			if (!gentSchematic.InitializeOnAwake)
			{
				gentSchematic.Initialize();
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
