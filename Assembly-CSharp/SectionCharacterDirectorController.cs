using System.Collections;

public class SectionCharacterDirectorController : SectionController
{
	private CharacterDirector[] m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_Group = base.GetComponentsInChildren<CharacterDirector>(includeInactive: true);
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			CharacterDirector characterDirector;
			characterDirector = this.m_Group[i];
			characterDirector.SetSectionID(base.Section.SectionID);
			if (characterDirector.OnStart)
			{
				characterDirector.Initialize();
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
