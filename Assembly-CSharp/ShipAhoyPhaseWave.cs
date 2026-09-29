using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipAhoyPhaseWave : BossPhase
{
	[Header("Boss")]
	[SerializeField]
	private GameObject m_Boss;

	[SerializeField]
	private CharacterContent m_CharacterContent;

	[SerializeField]
	private AnimationClip m_ShockAnimationClip;

	[Header("Spawn Locations")]
	[SerializeField]
	private Transform[] m_SpawnLocations;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy[] m_EnemyPrefabs;

	private List<Enemy> m_ActivePrefabs;

	private Transform m_CurrentSpawnLocation;

	private float m_Timer;

	private float m_TimerLimit = 1f;

	private int m_MaxWave = 5;

	private int m_MaxWaveCount;

	private int m_Kills;

	private int m_SpawnCount;

	public bool IsActive { get; private set; }

	protected override void InternalInitialize()
	{
		this.m_SpawnCount = global::UnityEngine.Random.Range(0, this.m_SpawnLocations.Length);
		this.m_ShockAnimationClip.name = "Idle";
		this.m_CharacterContent.UpdateClipOverride(this.m_ShockAnimationClip);
		this.m_CharacterContent.SetAnimationTrigger("Exit");
		this.m_CharacterContent.Animator.enabled = false;
		this.m_CharacterContent.Animator.enabled = true;
		this.SetActive(active: true);
	}

	public void SetActive(bool active)
	{
		this.IsActive = active;
	}

	private void Update()
	{
		if (!this.IsActive)
		{
			return;
		}
		unchecked
		{
			if (this.m_Timer >= this.m_TimerLimit && this.m_MaxWaveCount < this.m_MaxWave)
			{
				if (this.m_SpawnCount >= this.m_SpawnLocations.Length)
				{
					this.m_SpawnCount = 0;
				}
				this.m_CurrentSpawnLocation = this.m_SpawnLocations[this.m_SpawnCount];
				Enemy assetPrefab;
				assetPrefab = this.m_EnemyPrefabs[global::UnityEngine.Random.Range(0, this.m_EnemyPrefabs.Length)];
				Enemy enemy;
				enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(assetPrefab);
				enemy.SetSection(base.gameObject.GetComponentInParent<Section>().SectionID);
				enemy.AddData();
				enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
				enemy.OnCharacterDeath += this.HandleEnemyOnCharacterDeath;
				enemy.transform.position = this.m_CurrentSpawnLocation.position;
				enemy.transform.eulerAngles = this.m_CurrentSpawnLocation.eulerAngles;
				enemy.SetTarget(GameManager.Instance.Player.transform);
				this.AddEnemy(enemy);
				this.m_Timer = 0f;
				this.m_MaxWaveCount++;
				this.m_SpawnCount++;
			}
			else
			{
				this.m_Timer += Time.deltaTime;
			}
		}
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		Enemy enemy;
		enemy = sender as Enemy;
		enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
		this.RemoveEnemy(enemy);
		float health;
		health = UpgradeCheck.GetHealth();
		if (GameManager.Instance.Player.Health < health)
		{
			CameraEffects.Damage(0f);
		}
		unchecked
		{
			GameManager.Instance.Player.Heal((int)health);
			this.m_Kills++;
			if (this.m_Kills >= this.m_MaxWave)
			{
				base.Complete();
			}
		}
	}

	protected override void InternalComplete()
	{
		this.m_Boss.SetActive(value: true);
		this.m_MaxWaveCount = 0;
		this.m_Timer = 0f;
		this.m_Kills = 0;
		this.ClearEnemies();
		this.SetActive(active: false);
	}

	private void AddEnemy(Enemy enemy)
	{
		if (this.m_ActivePrefabs == null)
		{
			this.m_ActivePrefabs = new List<Enemy>();
		}
		this.m_ActivePrefabs.Add(enemy);
	}

	private void RemoveEnemy(Enemy enemy)
	{
		if (this.m_ActivePrefabs != null)
		{
			this.m_ActivePrefabs.Remove(enemy);
		}
	}

	private void ClearEnemies()
	{
		if (this.m_ActivePrefabs == null)
		{
			return;
		}
		for (int i = 0; i < this.m_ActivePrefabs.Count; i = unchecked(i + 1))
		{
			Enemy enemy;
			enemy = this.m_ActivePrefabs[i];
			if (enemy != null)
			{
				enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
			}
		}
		this.m_ActivePrefabs.Clear();
		this.m_ActivePrefabs = null;
	}

	protected override void OnDisposed()
	{
		this.ClearEnemies();
		this.m_CurrentSpawnLocation = null;
		base.OnDisposed();
	}
}
