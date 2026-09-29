using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionZoneController : SectionController
{
	private SectionZone[] m_SectionZones;

	private SectionZone m_CurrentZone;

	private SectionZone m_EnterZone;

	private SectionZone m_ExitZone;

	protected override IEnumerator InternalInitialize()
	{
		this.m_SectionZones = base.GetComponentsInChildren<SectionZone>(includeInactive: true);
		this.RemoveListeners();
		this.AddListeners();
		yield return null;
	}

	private void HandleSectionZoneOnEnter(object sender, EventArgs e)
	{
		SectionZone sectionZone;
		sectionZone = (SectionZone)sender;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.SetSectionID(base.Section.SectionID);
		}
		if (this.m_CurrentZone == sectionZone)
		{
			return;
		}
		this.m_EnterZone = sectionZone;
		if (this.m_EnterZone != this.m_ExitZone)
		{
			this.m_CurrentZone = this.m_EnterZone;
			if (GameManager.Instance.Player != null)
			{
				GameManager.Instance.Player.SetZone(this.m_CurrentZone.Name);
			}
		}
	}

	private void HandleSectionZoneOnExit(object sender, EventArgs e)
	{
		this.m_ExitZone = (SectionZone)sender;
		if (this.m_ExitZone != this.m_EnterZone)
		{
			this.m_CurrentZone = this.m_EnterZone;
			if (GameManager.Instance.Player != null)
			{
				GameManager.Instance.Player.SetZone(this.m_CurrentZone.Name);
			}
		}
	}

	private void AddListeners()
	{
		if (this.m_SectionZones != null)
		{
			for (int i = 0; i < this.m_SectionZones.Length; i = unchecked(i + 1))
			{
				SectionZone obj;
				obj = this.m_SectionZones[i];
				obj.OnEnter += this.HandleSectionZoneOnEnter;
				obj.OnExit += this.HandleSectionZoneOnExit;
				obj.Initialize();
			}
		}
	}

	protected override void RemoveListeners()
	{
		if (this.m_SectionZones != null)
		{
			for (int i = 0; i < this.m_SectionZones.Length; i = unchecked(i + 1))
			{
				SectionZone obj;
				obj = this.m_SectionZones[i];
				obj.OnEnter -= this.HandleSectionZoneOnEnter;
				obj.OnExit -= this.HandleSectionZoneOnExit;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		this.m_SectionZones = null;
		this.m_CurrentZone = null;
		this.m_EnterZone = null;
		this.m_ExitZone = null;
	}
}
