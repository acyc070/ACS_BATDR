using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionProjectorController : SectionController
{
	private ProjectorSlides[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.transform.GetComponentsInChildren<ProjectorSlides>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			ProjectorSlides projectorSlides;
			projectorSlides = this.m_Group[i];
			ProjectorDataObject projectorDataObject;
			projectorDataObject = (ProjectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ProjectorDataObject>(base.Section.SectionID, projectorSlides.ID);
			if (projectorDataObject != null)
			{
				projectorSlides.SetIndex(projectorDataObject.Index);
			}
			else
			{
				projectorDataObject = DataObject<int, ProjectorDataObject>.Create(projectorSlides.ID);
				projectorDataObject.SetIndex(projectorSlides.Index);
				GameManager.Instance.GameData.CurrentSave.AddData(base.Section.SectionID, projectorDataObject);
			}
			projectorSlides.Initialize();
		}
		yield return null;
	}

	public override void SaveData()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			ProjectorSlides projectorSlides;
			projectorSlides = this.m_Group[i];
			ProjectorDataObject projectorDataObject;
			projectorDataObject = (ProjectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ProjectorDataObject>(base.Section.SectionID, projectorSlides.ID);
			if (projectorDataObject != null)
			{
				projectorDataObject.SetIndex(projectorSlides.Index);
				continue;
			}
			projectorDataObject = DataObject<int, ProjectorDataObject>.Create(projectorSlides.ID);
			projectorDataObject.SetIndex(projectorSlides.Index);
			GameManager.Instance.GameData.CurrentSave.AddData(base.Section.SectionID, projectorDataObject);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_Group = null;
	}
}
