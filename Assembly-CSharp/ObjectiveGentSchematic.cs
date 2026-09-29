using System;
using UnityEngine;

public class ObjectiveGentSchematic : Objective
{
	[SerializeField]
	private GentSchematic m_GentSchematic;

	protected override void InternalInitialize()
	{
		base.Enable();
	}

	protected override void InternalEnable()
	{
		this.AddListeners();
	}

	private void HandleClimbOnActivate(object sender, EventArgs e)
	{
		this.m_GentSchematic.OnActivate -= this.HandleClimbOnActivate;
		base.SendOnComplete();
	}

	protected override void InternalComplete()
	{
		this.RemoveListeners();
	}

	protected void AddListeners()
	{
		this.m_GentSchematic.OnActivate += this.HandleClimbOnActivate;
	}

	protected override void RemoveListeners()
	{
		this.m_GentSchematic.OnActivate -= this.HandleClimbOnActivate;
	}
}
