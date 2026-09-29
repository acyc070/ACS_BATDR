using System;
using UnityEngine;

public class ObjectiveLootable : Objective
{
	[SerializeField]
	private Loot m_Loot;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		this.RemoveListeners();
		this.m_Loot.OnActivated += this.HandleLootOnActivated;
	}

	private void HandleLootOnActivated(object sender, EventArgs e)
	{
		this.m_Loot.OnActivated -= this.HandleLootOnActivated;
		base.SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		this.m_Loot.OnActivated -= this.HandleLootOnActivated;
	}
}
