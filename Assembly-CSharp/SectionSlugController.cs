using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionSlugController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private SlugGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public SlugGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			SlugGroup slugGroup;
			slugGroup = this.m_Group[i];
			if (slugGroup != null)
			{
				slugGroup.Controller.OnActivate -= this.HandleControllerOnActivate;
				if ((SlugDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, SlugDataObject>(this.m_SectionID, slugGroup.ID) != null)
				{
					slugGroup.IsComplete = true;
					slugGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(slugGroup.Controller.Content as SlugContent).InitializeContent();
					slugGroup.Controller.OnActivate += this.HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		Slug slug;
		slug = sender as Slug;
		slug.OnActivate -= this.HandleControllerOnActivate;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			SlugGroup slugGroup;
			slugGroup = this.m_Group[i];
			if (slugGroup.Controller == slug)
			{
				if ((SlugDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, SlugDataObject>(this.m_SectionID, slugGroup.ID) == null)
				{
					SlugDataObject value;
					value = DataObject<int, SlugDataObject>.Create(slugGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				slugGroup.IsComplete = true;
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				this.m_Group[i].Controller.OnActivate -= this.HandleControllerOnActivate;
			}
		}
	}
}
