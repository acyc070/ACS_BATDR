using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionMemoController : SectionController
{
	private Memo[] m_Memos;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Memos = base.transform.GetComponentsInChildren<Memo>(includeInactive: true);
		for (int i = 0; i < this.m_Memos.Length; i = unchecked(i + 1))
		{
			Memo memo;
			memo = this.m_Memos[i];
			if (!memo.InitializeOnAwake)
			{
				memo.Initialize();
			}
			yield return null;
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_Memos = null;
	}
}
