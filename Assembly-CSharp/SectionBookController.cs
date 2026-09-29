using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionBookController : SectionController
{
	private Book[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<Book>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			Book book;
			book = this.m_Group[i];
			if (!book.InitializeOnAwake)
			{
				book.Initialize();
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
