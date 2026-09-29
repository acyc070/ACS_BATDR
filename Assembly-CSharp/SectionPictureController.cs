using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionPictureController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private PictureGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public PictureGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			PictureGroup pictureGroup;
			pictureGroup = this.m_Group[i];
			if (pictureGroup != null)
			{
				pictureGroup.Controller.OnActivate -= this.HandleControllerOnActivate;
				if ((PictureDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PictureDataObject>(this.m_SectionID, pictureGroup.ID) != null)
				{
					pictureGroup.IsComplete = true;
					pictureGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(pictureGroup.Controller.Content as PictureContent).InitializeContent();
					pictureGroup.Controller.OnActivate += this.HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		Picture picture;
		picture = sender as Picture;
		picture.OnActivate -= this.HandleControllerOnActivate;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			PictureGroup pictureGroup;
			pictureGroup = this.m_Group[i];
			if (pictureGroup.Controller == picture)
			{
				if ((PictureDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, PictureDataObject>(this.m_SectionID, pictureGroup.ID) == null)
				{
					PictureDataObject value;
					value = DataObject<int, PictureDataObject>.Create(pictureGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				pictureGroup.IsComplete = true;
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
