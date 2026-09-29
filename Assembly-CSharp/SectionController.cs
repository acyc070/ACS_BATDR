using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionController : Controller
{
	private Section m_Section;

	public Section Section => this.m_Section;

	public void SetSection(Section section)
	{
		this.m_Section = section;
	}

	public virtual void SaveData()
	{
	}
}
