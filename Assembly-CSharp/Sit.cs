using System;
using UnityEngine;

public class Sit : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationEnter m_Interaction;

	private bool m_IsInactive;

	public override void Awake()
	{
		if (this.m_Interaction != null)
		{
			this.m_Interaction.OnInteract -= this.HandleInteractionOnInteract;
			this.m_Interaction.OnInteract += this.HandleInteractionOnInteract;
			this.m_Interaction.OnInteractionExit -= this.HandleInteractionActivatedOnInteractionExit;
			this.m_Interaction.OnInteractionExit += this.HandleInteractionActivatedOnInteractionExit;
		}
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		bool flag;
		flag = GameManager.Instance.Player.CombatStatus == CombatStatus.Combat;
		if (flag)
		{
			if (!this.m_IsInactive)
			{
				if (this.m_Interaction.IsInteracted)
				{
					this.m_Interaction.OnInteractionExit -= this.HandleInteractionOnInteractionExit;
					this.m_Interaction.OnInteractionExit += this.HandleInteractionOnInteractionExit;
					this.m_Interaction.Exit();
				}
				if (this.m_Interaction.IsActive)
				{
					this.m_Interaction.SetActive(active: false);
				}
			}
		}
		else if (this.m_IsInactive && !this.m_Interaction.IsActive)
		{
			this.m_Interaction.SetActive(active: true);
		}
		this.m_IsInactive = flag;
	}

	private void HandleInteractionOnInteractionExit(object sender, EventArgs e)
	{
		InteractableAnimationEnter obj;
		obj = sender as InteractableAnimationEnter;
		obj.OnInteractionExit -= this.HandleInteractionOnInteractionExit;
		obj.SetActive(active: false);
	}

	private void HandleInteractionOnInteract(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.None)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.Sit);
		}
	}

	private void HandleInteractionActivatedOnInteractionExit(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.BattleStatus == BattleStatus.Sit)
		{
			GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
		}
	}

	private void RemoveListeners()
	{
		this.m_Interaction.OnInteract -= this.HandleInteractionOnInteract;
		this.m_Interaction.OnInteractionExit -= this.HandleInteractionActivatedOnInteractionExit;
		this.m_Interaction.OnInteractionExit -= this.HandleInteractionOnInteractionExit;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
