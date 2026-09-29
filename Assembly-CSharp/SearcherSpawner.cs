using System;
using UnityEngine;
using UnityEngine.AI;

public class SearcherSpawner : Spawner
{
	[SerializeField]
	private bool m_OnStart;

	[Header("Settings")]
	[SerializeField]
	private LayerMask m_LayerMask;

	[SerializeField]
	private Transform m_SpawnLocation;

	[SerializeField]
	private bool m_LookAtPlayer;

	[Header("Spawn Options")]
	[SerializeField]
	private int m_SpawnLimit = 1;

	[SerializeField]
	private float m_SpawnTimerLimit = 1.5f;

	[SerializeField]
	private float m_SpawnDistance = 8f;

	[SerializeField]
	private bool m_IsSingleSpawn;

	[Header("Enemy Animation Clips")]
	[SerializeField]
	private Animator m_PropAnimator;

	[SerializeField]
	private AnimationClipGroup[] m_EnemySpawnClips;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy[] m_EnemyPrefabs;

	private Enemy m_Enemy;

	private bool m_IsActive;

	private bool m_IsSpawning;

	private float m_SpawnTimer;

	private int m_SpawnCount;

	private float m_Distance => Vector3.Distance(this.m_SpawnLocation.position, GameManager.Instance.Player.transform.position);

	public override void Start()
	{
		if (this.m_OnStart)
		{
			this.Activate();
		}
	}

	public override void Activate()
	{
		GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnRespawn += this.HandlePlayerOnRespawn;
		this.m_SpawnTimer = this.m_SpawnTimerLimit;
		this.m_IsActive = true;
		if (this.m_SpawnLocation == null)
		{
			this.m_SpawnLocation = base.transform;
		}
	}

	public override void Deactivate()
	{
		GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		this.ResetEnemy();
		this.m_IsActive = false;
		this.m_IsSpawning = false;
		this.m_SpawnTimer = this.m_SpawnTimerLimit;
		this.m_SpawnCount = 0;
	}

	private void KillEnemy()
	{
		if (this.m_Enemy != null)
		{
			GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink").transform.position = this.m_Enemy.transform.position + Vector3.up * 3f;
			this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
			this.m_Enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
			this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyDespawnOnAnimationComplete;
			this.m_Enemy.Dispose();
			this.m_Enemy = null;
		}
	}

	private void ResetEnemy()
	{
		if (this.m_Enemy != null)
		{
			this.m_Enemy.SetState(State.Character.Cutscene);
			this.m_Enemy.DespawnAnimationClip.name = "Interact";
			this.m_Enemy.Content.UpdateClipOverride(this.m_Enemy.DespawnAnimationClip);
			this.m_Enemy.Content.SetAnimationTrigger("Interact");
			this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
			this.m_Enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
			this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyDespawnOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete += this.HandleEnemyDespawnOnAnimationComplete;
		}
	}

	private void HandleEnemyDespawnOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyDespawnOnAnimationComplete;
		this.m_Enemy.Dispose();
		this.m_Enemy = null;
		this.m_IsSpawning = false;
		this.m_SpawnTimer = this.m_SpawnTimerLimit;
		this.m_SpawnCount = 0;
	}

	private void Update()
	{
		if (!this.m_IsActive || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (!this.m_IsSpawning)
		{
			if (this.m_SpawnCount < this.m_SpawnLimit)
			{
				this.m_SpawnTimer += Time.deltaTime;
			}
			if (this.IsSpawnDistance())
			{
				this.Spawn();
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
			this.m_Enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
			this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyDespawnOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete += this.HandleEnemyDespawnOnAnimationComplete;
			this.m_IsSpawning = false;
		}
	}

	public override void Spawn()
	{
		unchecked
		{
			if (!this.m_IsSpawning && this.m_SpawnCount < this.m_SpawnLimit && !(this.m_SpawnTimer < this.m_SpawnTimerLimit))
			{
				this.m_IsSpawning = true;
				this.m_SpawnCount++;
				this.m_SpawnTimer = 0f;
				this.m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(this.m_EnemyPrefabs[global::UnityEngine.Random.Range(0, this.m_EnemyPrefabs.Length)]);
				this.m_Enemy.SetSection(base.gameObject.GetComponentInParent<Section>().SectionID);
				this.m_Enemy.transform.position = this.m_SpawnLocation.position;
				this.m_Enemy.transform.rotation = this.m_SpawnLocation.rotation;
				this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
				this.m_Enemy.OnInitializeOnComplete += this.HandleEnemyOnInitializeOnComplete;
			}
		}
	}

	public void Disable()
	{
		if (this.m_Enemy != null)
		{
			this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
			this.m_Enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
			this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyDespawnOnAnimationComplete;
			this.m_Enemy.Dispose();
			this.m_Enemy = null;
		}
		this.Deactivate();
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		this.m_Enemy.SetState(State.Character.Cutscene);
		this.m_Enemy.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		this.m_Enemy.Controller.enabled = false;
		this.m_Enemy.transform.position = this.m_SpawnLocation.position;
		if (this.m_LookAtPlayer)
		{
			Vector3 position;
			position = GameManager.Instance.Player.transform.position;
			position.y = this.m_SpawnLocation.position.y;
			Vector3 normalized;
			normalized = (position - this.m_SpawnLocation.position).normalized;
			this.m_Enemy.transform.rotation = Quaternion.LookRotation(normalized);
		}
		else
		{
			this.m_Enemy.transform.rotation = this.m_SpawnLocation.rotation;
		}
		int num;
		num = global::UnityEngine.Random.Range(0, this.m_EnemySpawnClips.Length);
		AnimationClipGroup animationClipGroup;
		animationClipGroup = this.m_EnemySpawnClips[num];
		animationClipGroup.Initialize();
		this.m_Enemy.Content.UpdateClipOverrides(animationClipGroup.AnimationClip);
		this.m_Enemy.Content.SetAnimationTrigger("InteractInstant");
		if (this.m_PropAnimator != null)
		{
			this.m_PropAnimator.SetTrigger("InteractInstant");
		}
		this.m_Enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
		this.m_Enemy.OnCharacterDeath += this.HandleEnemyOnCharacterDeath;
		this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleSpawnedEnemyOnAnimationComplete;
		base.SendOnSpawn();
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		this.m_Enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
		this.SingleSpawn();
		this.m_Enemy = null;
		unchecked
		{
			if (this.m_SpawnCount > 0)
			{
				this.m_SpawnCount--;
			}
			if (this.m_SpawnCount < 0)
			{
				this.m_SpawnCount = 0;
			}
			this.m_SpawnTimer = 0f;
			this.m_IsSpawning = false;
		}
	}

	private void SingleSpawn()
	{
		if (this.m_IsSingleSpawn)
		{
			GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
			this.m_IsActive = false;
			this.m_IsSpawning = false;
			this.m_SpawnTimer = this.m_SpawnTimerLimit;
			this.m_SpawnCount = 0;
			if (this.m_Enemy != null)
			{
				this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
				this.m_Enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
				this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
				this.m_Enemy.OnAnimationComplete -= this.HandleEnemyDespawnOnAnimationComplete;
			}
		}
	}

	private void HandleSpawnedEnemyOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
		this.m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		this.m_Enemy.SetState(State.Character.Follow);
		this.m_Enemy.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		this.m_Enemy.Agent.ResetAgent();
		this.m_Enemy.Controller.enabled = true;
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		if (this.m_IsActive)
		{
			this.m_SpawnTimer = this.m_SpawnTimerLimit;
			this.m_SpawnCount = 0;
			this.m_Enemy = null;
			this.m_IsSpawning = false;
		}
	}

	private bool IsSpawnDistance()
	{
		bool flag;
		flag = this.m_Distance < this.m_SpawnDistance;
		if (flag)
		{
			Vector3 vector;
			vector = this.m_SpawnLocation.position + Vector3.up;
			if (Physics.SphereCast(direction: (GameManager.Instance.GameCamera.transform.position - vector).normalized, origin: this.m_SpawnLocation.position + Vector3.up, radius: 0.25f, hitInfo: out var hitInfo, maxDistance: float.PositiveInfinity, layerMask: ~(int)this.m_LayerMask, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
		}
		return flag;
	}
}
