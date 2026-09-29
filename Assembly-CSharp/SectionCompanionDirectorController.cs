using System.Collections;

public class SectionCompanionDirectorController : SectionController
{
	private CompanionDirector[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.GetComponentsInChildren<CompanionDirector>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			CompanionDirector companionDirector;
			companionDirector = this.m_Group[i];
			if ((bool)companionDirector)
			{
				companionDirector.SetSectionID(base.Section.SectionID);
				companionDirector.Initialize();
			}
		}
		yield return null;
	}

	public override void SaveData()
	{
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				this.m_Group[i].UpdateData();
			}
		}
	}

	protected override void OnDisposed()
	{
		this.SaveData();
		this.m_Group = null;
		base.OnDisposed();
	}
}
