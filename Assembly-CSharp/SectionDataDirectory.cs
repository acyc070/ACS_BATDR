using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SectionDataDirectory : DataDirectory<SectionID, SectionDataObject>
{
	[SerializeField]
	private List<SectionID> m_ActiveSections = new List<SectionID>();

	[SerializeField]
	private List<SectionID> m_InitializedSections = new List<SectionID>();

	public List<SectionID> ActiveSections => this.m_ActiveSections;

	public List<SectionID> InitializedSections => this.m_InitializedSections;

	public void Update()
	{
		Section[] allSections;
		allSections = GameManager.Instance.SectionManager.GetAllSections();
		List<SectionID> list;
		list = new List<SectionID>();
		List<SectionID> list2;
		list2 = new List<SectionID>();
		foreach (Section section in allSections)
		{
			if (!(section != null))
			{
				continue;
			}
			SectionID item;
			item = (SectionID)section.ID;
			if (!list.Contains(item))
			{
				if (section.IsInitialized)
				{
					list2.Add(item);
				}
				list.Add(item);
			}
		}
		this.m_ActiveSections = list;
		this.m_InitializedSections = list2;
	}
}
