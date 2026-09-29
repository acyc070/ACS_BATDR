using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionAudioLogController : SectionController
{
	private AudioLog[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<AudioLog>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			AudioLog audioLog;
			audioLog = this.m_Group[i];
			if (!audioLog.InitializeOnAwake)
			{
				audioLog.Initialize();
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
