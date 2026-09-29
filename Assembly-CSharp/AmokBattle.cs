using System;
using DG.Tweening;
using UnityEngine;

public class AmokBattle : JMonoBehaviour
{
	[Serializable]
	public class AmokEnemyGroup
	{
		public Enemy Enemy;

		public CharacterNode CharacterNode;
	}

	[SerializeField]
	private AmokEnemyGroup[] m_Enemies;

	[SerializeField]
	private Door m_Door;

	public void Begin()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
			GameManager.Instance.Player.OnDeath += this.HandlePlayerOnDeath;
		}
		for (int i = 0; i < this.m_Enemies.Length; i = unchecked(i + 1))
		{
			AmokEnemyGroup amokEnemyGroup;
			amokEnemyGroup = this.m_Enemies[i];
			if (amokEnemyGroup.Enemy != null && amokEnemyGroup.Enemy.gameObject.activeSelf && amokEnemyGroup.Enemy.CurrentState != State.Character.Death)
			{
				amokEnemyGroup.Enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
				amokEnemyGroup.Enemy.OnCharacterDeath += this.HandleEnemyOnCharacterDeath;
			}
		}
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		if (!this.m_Door.Content.IsActivated)
		{
			this.m_Door.Content.ForceActivate();
		}
		for (int i = 0; i < this.m_Enemies.Length; i = unchecked(i + 1))
		{
			AmokEnemyGroup amokEnemyGroup;
			amokEnemyGroup = this.m_Enemies[i];
			if (amokEnemyGroup.Enemy != null && amokEnemyGroup.Enemy.gameObject.activeSelf && amokEnemyGroup.Enemy.CurrentState != State.Character.Death)
			{
				amokEnemyGroup.Enemy.SetNode(amokEnemyGroup.CharacterNode);
				amokEnemyGroup.Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
				amokEnemyGroup.Enemy.OnNodeReached += this.HandleEnemyOnNodeReached;
				amokEnemyGroup.Enemy.SetState(State.Character.Patrol);
			}
		}
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		Enemy obj;
		obj = sender as Enemy;
		obj.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
		obj.OnNodeReached -= this.HandleEnemyOnNodeReached;
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		Enemy enemy;
		enemy = sender as Enemy;
		enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
		enemy.transform.DOMove(enemy.CurrentNode.transform.position, 0.25f);
		enemy.transform.DORotate(enemy.CurrentNode.transform.eulerAngles, 0.25f);
		enemy.Content.Animator.SetMovementState(0f, smooth: false);
		enemy.Content.Animator.SetMovementSpeed(0f, smooth: false);
		enemy.SetState(State.Character.Idle);
	}

	public void End()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		}
		for (int i = 0; i < this.m_Enemies.Length; i = unchecked(i + 1))
		{
			AmokEnemyGroup amokEnemyGroup;
			amokEnemyGroup = this.m_Enemies[i];
			if (amokEnemyGroup.Enemy != null)
			{
				amokEnemyGroup.Enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
				amokEnemyGroup.Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.End();
		base.OnDisposed();
	}
}
