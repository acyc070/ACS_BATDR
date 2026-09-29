using System;
using UnityEngine;

[DefaultExecutionOrder(-55)]
public class ObjectiveCharacterDeath : Objective
{
	[SerializeField]
	private Character m_Character;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		if (this.m_Character != null)
		{
			Enemy enemy;
			enemy = this.m_Character as Enemy;
			if (enemy != null)
			{
				enemy.SetSection(base.gameObject.GetComponentInParent<Section>().SectionID);
			}
			this.m_Character.OnCharacterDeath -= this.HandleOnCharacterDeath;
			this.m_Character.OnCharacterDeath += this.HandleOnCharacterDeath;
			this.m_Character.SetTarget(GameManager.Instance.Player.transform);
			this.m_Character.SetState(State.Character.Follow);
			if (this.m_Character.Agent != null)
			{
				this.m_Character.Agent.ResetAgent();
			}
		}
	}

	protected override void InternalDisable()
	{
		base.Inactive();
	}

	protected override void InternalInactive()
	{
		if (this.m_Character != null)
		{
			this.m_Character.OnCharacterDeath -= this.HandleOnCharacterDeath;
			this.m_Character.OnCharacterDeath += this.HandleOnCharacterDeath;
		}
	}

	private void HandleOnCharacterDeath(object sender, EventArgs e)
	{
		if (this.m_Character != null)
		{
			this.m_Character.OnCharacterDeath -= this.HandleOnCharacterDeath;
		}
		base.SendOnComplete();
	}

	protected override void RemoveListeners()
	{
		if (this.m_Character != null)
		{
			this.m_Character.OnCharacterDeath -= this.HandleOnCharacterDeath;
		}
	}
}
