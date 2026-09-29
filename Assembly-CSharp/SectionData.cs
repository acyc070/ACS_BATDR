using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game Data/Requirements/New Section")]
public class SectionData : Requirement
{
	protected override bool InternalIsComplete()
	{
		bool result;
		result = true;
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(base.SectionID);
		if (sectionDataObject != null)
		{
			if (!sectionDataObject.IsComplete)
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
