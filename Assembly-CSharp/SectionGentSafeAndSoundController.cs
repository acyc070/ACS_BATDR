using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentSafeAndSoundController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentSafeAndSoundGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public GentSafeAndSoundGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentSafeAndSoundGroup gentSafeAndSoundGroup;
			gentSafeAndSoundGroup = this.m_Group[i];
			if (gentSafeAndSoundGroup != null)
			{
				if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards >= 1)
				{
					gentSafeAndSoundGroup.Controller.Content.Enable();
				}
				else
				{
					gentSafeAndSoundGroup.Controller.Content.Disable();
				}
			}
		}
		yield return null;
	}
}
