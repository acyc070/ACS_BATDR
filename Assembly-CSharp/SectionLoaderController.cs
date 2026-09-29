using System;
using System.Collections;
using UnityEngine;

public class SectionLoaderController : SectionController
{
	[Header("Section")]
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private SectionLoaderGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	protected override IEnumerator InternalInitialize()
	{
		this.AddListeners();
		yield return null;
	}

	private void HandleGroupOnLoad(object sender, EventArgs e)
	{
		GameManager.Instance.SectionManager.InitializeSection((sender as SectionLoaderGroup).ID);
	}

	private void HandleGroupOnUnload(object sender, EventArgs e)
	{
		Section section;
		section = GameManager.Instance.SectionManager.GetSection((sender as SectionLoaderGroup).ID);
		if (section != null)
		{
			section.SetActive(active: false);
			if (GameManager.Instance.InkDemonManager != null)
			{
				GameManager.Instance.InkDemonManager.CheckAvailability();
			}
		}
	}

	private void AddListeners()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			SectionLoaderGroup obj;
			obj = this.m_Group[i];
			obj.OnLoad -= this.HandleGroupOnLoad;
			obj.OnLoad += this.HandleGroupOnLoad;
			obj.OnUnload -= this.HandleGroupOnUnload;
			obj.OnUnload += this.HandleGroupOnUnload;
			obj.Initialize();
		}
	}

	protected override void RemoveListeners()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			SectionLoaderGroup obj;
			obj = this.m_Group[i];
			obj.OnLoad -= this.HandleGroupOnLoad;
			obj.OnUnload -= this.HandleGroupOnUnload;
			obj.RemoveListeners();
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				this.m_Group[i].Clear();
			}
		}
	}
}
