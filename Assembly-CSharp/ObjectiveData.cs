using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Requirements/New Objective")]
public class ObjectiveData : Requirement
{
	protected override bool InternalIsComplete()
	{
		bool result;
		result = true;
		ObjectiveDataObject objectiveDataObject;
		objectiveDataObject = (ObjectiveDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, ObjectiveDataObject>(base.SectionID, base.ID);
		if (objectiveDataObject != null)
		{
			if (objectiveDataObject.Status != ObjectiveStatus.Complete)
			{
				result = false;
			}
		}
		else
		{
			result = false;
		}
		return result;
	}
}
