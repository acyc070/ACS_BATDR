using System;
using UnityEngine;

public class GameController : JMonoBehaviour
{
	[SerializeField]
	private SectionSoftLoaderGroup[] m_Group;

	private Bounds m_Bounds;

	public override void Start()
	{
		GameManager.Instance.GameController = this;
		this.m_Bounds = new Bounds(Vector3.zero, Vector3.one * 5000f);
	}

	public void Initialize()
	{
		this.AddListeners();
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && GameManager.Instance.Player != null)
		{
			_ = this.m_Bounds;
			if (!this.m_Bounds.Contains(GameManager.Instance.Player.transform.position))
			{
				GameManager.Instance.Player.ForceDeath();
			}
		}
	}

	private void HandleGroupOnLoad(object sender, EventArgs e)
	{
		GameManager.Instance.SectionManager.LoadSectionAsync((sender as SectionSoftLoaderGroup).ID, initialize: false);
	}

	private void HandleGroupOnUnload(object sender, EventArgs e)
	{
		Section section;
		section = GameManager.Instance.SectionManager.GetSection((sender as SectionSoftLoaderGroup).ID);
		if (section != null)
		{
			this.UnloadSection(section);
		}
	}

	private void UnloadSection(Section section)
	{
		GameManager.Instance.SectionManager.Remove(section.SectionID);
		if (GameManager.Instance.InkDemonManager != null)
		{
			GameManager.Instance.InkDemonManager.CheckAvailability();
		}
	}

	private void AddListeners()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			SectionSoftLoaderGroup obj;
			obj = this.m_Group[i];
			obj.OnLoad -= this.HandleGroupOnLoad;
			obj.OnLoad += this.HandleGroupOnLoad;
			obj.OnUnload -= this.HandleGroupOnUnload;
			obj.OnUnload += this.HandleGroupOnUnload;
			obj.Initialize();
		}
	}

	protected void RemoveListeners()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			SectionSoftLoaderGroup obj;
			obj = this.m_Group[i];
			obj.OnLoad -= this.HandleGroupOnLoad;
			obj.OnUnload -= this.HandleGroupOnUnload;
			obj.RemoveListeners();
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
