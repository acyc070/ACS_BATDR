using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionGentToolkitController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private GentToolkitGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public GentToolkitGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentToolkitGroup gentToolkitGroup;
			gentToolkitGroup = this.m_Group[i];
			if (gentToolkitGroup != null)
			{
				gentToolkitGroup.Controller.OnActivate -= this.HandleControllerOnActivate;
				if ((GentToolkitDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentToolkitDataObject>(this.m_SectionID, gentToolkitGroup.ID) != null)
				{
					gentToolkitGroup.IsComplete = true;
					gentToolkitGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(gentToolkitGroup.Controller.Content as GentToolkitContent).InitializeContent();
					gentToolkitGroup.Controller.OnActivate += this.HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		GentToolkit gentToolkit;
		gentToolkit = sender as GentToolkit;
		gentToolkit.OnActivate -= this.HandleControllerOnActivate;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			GentToolkitGroup gentToolkitGroup;
			gentToolkitGroup = this.m_Group[i];
			if (gentToolkitGroup.Controller == gentToolkit)
			{
				if ((GentToolkitDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, GentToolkitDataObject>(this.m_SectionID, gentToolkitGroup.ID) == null)
				{
					GentToolkitDataObject value;
					value = DataObject<int, GentToolkitDataObject>.Create(gentToolkitGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				gentToolkitGroup.IsComplete = true;
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
