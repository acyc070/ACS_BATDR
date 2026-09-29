using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentLockController : SectionController
{
	private GentLock[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<GentLock>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			this.m_Group[i].Initialize();
			yield return null;
		}
	}

	public override void SaveData()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentLock gentLock;
			gentLock = this.m_Group[i];
			if (!(gentLock == null) && !(gentLock.Content == null))
			{
				GentLockDataObject gentLockDataObject;
				gentLockDataObject = (GentLockDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.GentLockDirectory.GetValue(gentLock.GentLockID);
				if (gentLockDataObject != null && gentLockDataObject.Status == CutsceneStatus.Active)
				{
					gentLockDataObject.SetTimeline(gentLock.Content.Director.PlayableDirector.time);
				}
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_Group = null;
	}
}
