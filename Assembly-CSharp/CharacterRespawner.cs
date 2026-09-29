using System;
using UnityEngine;

public class CharacterRespawner : JMonoBehaviour
{
	[SerializeField]
	private Character m_Character;

	[SerializeField]
	private Transform m_RespawnLocation;

	[SerializeField]
	private EventTrigger m_EventTrigger;

	public void Activate()
	{
		GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnRespawn += this.HandlePlayerOnRespawn;
	}

	public void Deactivate()
	{
		GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		if (this.m_EventTrigger != null)
		{
			this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
			this.m_EventTrigger.SetActive(active: false);
		}
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		if (this.m_Character != null && this.m_RespawnLocation != null)
		{
			this.m_Character.SetState(State.Character.Cutscene);
			this.m_Character.SetTarget(null);
			this.m_Character.CancelPath();
			if (this.m_Character.Content != null)
			{
				this.m_Character.Content.Animator.SetMovementState(0f, smooth: false);
				this.m_Character.Content.Animator.SetMovementSpeed(0f, smooth: false);
				this.m_Character.ClearAnimationTriggers();
				this.m_Character.Content.Animator.enabled = false;
				this.m_Character.Content.Animator.enabled = true;
			}
			this.m_Character.transform.position = this.m_RespawnLocation.position;
			this.m_Character.transform.eulerAngles = this.m_RespawnLocation.eulerAngles;
			if (this.m_EventTrigger != null)
			{
				this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
				this.m_EventTrigger.OnEnter += this.HandleEventTriggerOnEnter;
				this.m_EventTrigger.ResetAction();
				this.m_EventTrigger.SetActive(active: true);
			}
			else
			{
				this.m_Character.SetTarget(GameManager.Instance.Player.transform);
				this.m_Character.SetState(State.Character.Follow);
				this.m_Character.Agent.ResetAgent();
			}
		}
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.SetActive(active: false);
		this.m_Character.SetTarget(GameManager.Instance.Player.transform);
		this.m_Character.SetState(State.Character.Follow);
		this.m_Character.Agent.ResetAgent();
	}

	protected override void OnDisposed()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		}
		if (this.m_EventTrigger != null)
		{
			this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
