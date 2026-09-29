using System;
using UnityEngine;

public class ObjectiveSectionLoad : Objective
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private bool m_Unload;

	[SerializeField]
	private bool m_ShowLoader;

	[SerializeField]
	private bool m_UnloadAllExcept;

	protected override void InternalInitialize()
	{
		unchecked
		{
			if (this.m_SectionID != SectionID.NONE)
			{
				if (this.m_UnloadAllExcept)
				{
					Section[] allSections;
					allSections = GameManager.Instance.SectionManager.GetAllSections();
					if (allSections.Length != 0)
					{
						for (int num = allSections.Length - 1; num >= 0; num--)
						{
							Section section;
							section = allSections[num];
							if (section.SectionID != this.m_SectionID)
							{
								GameManager.Instance.SectionManager.Remove(section.SectionID);
							}
						}
					}
					base.SendOnComplete();
				}
				else if (!GameManager.Instance.SectionManager.Contains(this.m_SectionID))
				{
					if (!this.m_Unload)
					{
						GameManager.Instance.SectionManager.OnLoaded += this.HandleSectionOnLoaded;
						GameManager.Instance.SectionManager.LoadSectionAsync(this.m_SectionID);
						if (this.m_ShowLoader)
						{
							GameManager.Instance.ShowAsyncLoader();
						}
					}
					else
					{
						base.SendOnComplete();
					}
				}
				else
				{
					GameManager.Instance.SectionManager.Remove(this.m_SectionID);
					base.SendOnComplete();
				}
			}
			else
			{
				base.SendOnComplete();
			}
		}
	}

	private void HandleSectionOnLoaded(object sender, EventArgs e)
	{
		if ((sender as Section).SectionID == this.m_SectionID)
		{
			GameManager.Instance.SectionManager.OnLoaded -= this.HandleSectionOnLoaded;
			if (this.m_ShowLoader)
			{
				GameManager.Instance.CompleteAsyncLoader();
			}
			base.SendOnComplete();
		}
	}
}
