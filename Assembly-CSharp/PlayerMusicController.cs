using System;
using System.Collections.Generic;
using S13Audio.BATDR;
using UnityEngine;

public class PlayerMusicController : JMonoBehaviour
{
	[SerializeField]
	private BATDRCombatMusic combatMusic;

	private List<Enemy> m_ActiveCharacters = new List<Enemy>();

	private Enemy m_ClosestCharacter;

	private float m_Distance;

	private bool m_IsAlerted;

	public void Initialize()
	{
		GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnRespawn += this.HandlePlayerOnRespawn;
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		this.combatMusic.StopCombat();
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			this.CheckCombat();
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.None)
		{
			if (GameManager.Instance.Player.PreviousCombatStatus == CombatStatus.Combat)
			{
				if (this.m_IsAlerted)
				{
					this.m_IsAlerted = false;
					this.combatMusic.OnDefeatedEnemies();
				}
			}
			else if (this.m_IsAlerted)
			{
				this.m_IsAlerted = false;
				this.combatMusic.StopCombat();
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			this.CheckHide();
		}
		this.CheckEnemyDistance();
	}

	private void CheckCombat()
	{
		if (!this.m_IsAlerted && this.m_ClosestCharacter != null && Physics.Linecast(end: (this.m_ClosestCharacter.Content.EyeSight == null) ? (this.m_ClosestCharacter.transform.position + Vector3.up * 3f) : this.m_ClosestCharacter.Content.EyeSight.position, start: GameManager.Instance.GameCamera.transform.position, hitInfo: out var hitInfo, layerMask: ~((1 << LayerMask.NameToLayer("InvisibleCollider")) | (1 << LayerMask.NameToLayer("IgnoreInvisibleCollider"))), queryTriggerInteraction: QueryTriggerInteraction.Ignore) && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("AI") && Vector3.Distance(GameManager.Instance.Player.transform.position, this.m_ClosestCharacter.transform.position) < 20f)
		{
			this.m_IsAlerted = true;
			this.combatMusic.OnAlert(this.m_ClosestCharacter.EnemyType);
		}
	}

	private void CheckHide()
	{
		if (GameManager.Instance.Player.PreviousCombatStatus == CombatStatus.Combat && this.m_IsAlerted)
		{
			this.m_IsAlerted = false;
			this.combatMusic.OnHiddenFromEnemies();
		}
	}

	private void CheckEnemyDistance()
	{
		Collider[] array;
		array = Physics.OverlapSphere(GameManager.Instance.Player.transform.position, 30f, 1 << LayerMask.NameToLayer("AI"));
		this.m_ActiveCharacters.Clear();
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			Enemy component;
			component = array[i].GetComponent<Enemy>();
			if (component != null)
			{
				this.m_ActiveCharacters.Add(component);
			}
		}
		this.m_ClosestCharacter = this.GetClosest(this.m_ActiveCharacters.ToArray(), out var minDist);
		if (this.m_ClosestCharacter != null)
		{
			if (this.m_ClosestCharacter.EnemyType != EnemyType.LostOne)
			{
				return;
			}
			if (this.m_ClosestCharacter.Content != null && this.m_ClosestCharacter.transform != null && Physics.Linecast(end: (this.m_ClosestCharacter.Content.EyeSight == null) ? (this.m_ClosestCharacter.transform.position + Vector3.up * 3f) : this.m_ClosestCharacter.Content.EyeSight.position, start: GameManager.Instance.GameCamera.transform.position, hitInfo: out var hitInfo, layerMask: ~((1 << LayerMask.NameToLayer("InvisibleCollider")) | (1 << LayerMask.NameToLayer("IgnoreInvisibleCollider"))), queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				float num;
				num = minDist;
				if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("AI"))
				{
					num += 15f;
				}
				minDist = Mathf.Lerp(this.m_Distance, num, 0.75f * Time.deltaTime);
			}
			this.m_Distance = minDist;
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Combat)
			{
				if (GameManager.Instance.Player.CombatStatus == CombatStatus.None)
				{
					this.combatMusic.StopDanger();
				}
				else
				{
					this.combatMusic.UpdateEnemyDistance(this.m_Distance);
				}
			}
		}
		else
		{
			this.combatMusic.StopDanger();
		}
	}

	private Enemy GetClosest(Enemy[] enemies, out float minDist)
	{
		Enemy result;
		result = null;
		minDist = 50f;
		Vector3 position;
		position = GameManager.Instance.Player.transform.position;
		foreach (Enemy enemy in enemies)
		{
			float num;
			num = Vector3.Distance(enemy.transform.position, position);
			if (num < minDist)
			{
				result = enemy;
				minDist = num;
			}
		}
		return result;
	}

	protected override void OnDisposed()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		}
		base.OnDisposed();
	}
}
