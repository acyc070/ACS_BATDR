using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionCutsceneController : SectionController
{
	private Cutscene[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<Cutscene>(includeInactive: true);
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
			Cutscene cutscene;
			cutscene = this.m_Group[i];
			if (!(cutscene == null) && !(cutscene.Content == null))
			{
				CutsceneDataObject cutsceneDataObject;
				cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(cutscene.SectionID, cutscene.CutsceneID);
				if (cutsceneDataObject != null && cutsceneDataObject.Status == CutsceneStatus.Active)
				{
					cutsceneDataObject.SetTimeline(cutscene.Content.Director.PlayableDirector.time);
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
