using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class InkBulb : JMonoBehaviour, IHittable, ISpawner
{
	[SerializeField]
	private LayerMask m_LayerMask;

	[Header("Health Settings")]
	[Header("Easy")]
	[SerializeField]
	private int m_MaxHitPointsEasy = 3;

	[Header("Normal")]
	[SerializeField]
	private int m_MaxHitPoints = 5;

	[Header("Hard")]
	[SerializeField]
	private int m_MaxHitPointsHard = 7;

	[Header("Spawn Options")]
	[SerializeField]
	private bool m_CanSpawn = true;

	[SerializeField]
	private bool m_AutoSpawn;

	[SerializeField]
	private int m_SpawnLimit = 5;

	[SerializeField]
	private float m_SpawnTimerLimit = 1.5f;

	[SerializeField]
	private float m_SpawnDistance = 8f;

	[SerializeField]
	private float m_ActiveDistance = 16f;

	[SerializeField]
	private float m_InactiveDistance = 18f;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClipGroup m_ShakeClip;

	[SerializeField]
	private AnimationClipGroup m_SpawnClip;

	[SerializeField]
	private AnimationClipGroup m_InkWidowSpawnClip;

	[Header("Prefabs")]
	[SerializeField]
	private Character[] m_InkWidowPrefabs;

	private CharacterContent m_CharacterContent;

	private List<Character> m_InkWidows = new List<Character>();

	private bool m_IsActive;

	private bool m_IsSpawning;

	private bool m_IsDead;

	private bool m_PauseAutoSpawn;

	private float m_HitType;

	private float m_LastHitType;

	private float[] m_HitTypes = new float[2] { 0f, 1f };

	private float m_SpawnTimer;

	private int m_Health;

	private int m_SpawnCount;

	public int MaxHitPoints
	{
		get
		{
			int result;
			result = this.m_MaxHitPoints;
			switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
			{
			case DifficultyLevel.Easy:
				result = this.m_MaxHitPointsEasy;
				break;
			case DifficultyLevel.Hard:
				result = this.m_MaxHitPointsHard;
				break;
			}
			return result;
		}
	}

	public List<Character> InkWidows => this.m_InkWidows;

	private float m_Distance => Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position);

	public bool IsBroken { get; private set; }

	public bool IsPlayerBreakable => true;

	public event EventHandler OnFlee;

	public event EventHandler OnHit;

	public override void Start()
	{
		this.m_CharacterContent = base.GetComponentInChildren<CharacterContent>();
		this.m_CharacterContent.GenericAnimationEvents.SetReciever(this);
		this.m_SpawnTimer = this.m_SpawnTimerLimit;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
			GameManager.Instance.Player.OnDeath += this.HandlePlayerOnDeath;
			GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
			GameManager.Instance.Player.OnRespawn += this.HandlePlayerOnRespawn;
		}
	}

	private void Update()
	{
		if (this.m_PauseAutoSpawn || this.m_IsDead || this.m_IsSpawning || base.IsDisposed || GameManager.Instance.IsPaused || this.m_Health >= this.MaxHitPoints)
		{
			return;
		}
		this.m_SpawnTimer += Time.deltaTime;
		if (this.m_IsActive)
		{
			if (this.m_AutoSpawn)
			{
				this.Spawn();
			}
			else if (this.IsSpawnDistance())
			{
				this.Spawn();
			}
			else if (this.IsInactiveDistance())
			{
				this.m_IsActive = false;
			}
		}
		else if (!this.m_AutoSpawn && this.IsActiveDistance())
		{
			this.Shake();
		}
	}

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
		this.m_CanSpawn = active;
	}

	public void Shake()
	{
		if (!this.m_IsDead && !this.m_IsSpawning)
		{
			this.m_IsActive = true;
			this.m_ShakeClip.Initialize();
			this.m_CharacterContent.UpdateClipOverrides(this.m_ShakeClip.AnimationClip);
			this.m_CharacterContent.SetAnimationTrigger("Interact");
		}
	}

	public void Spawn()
	{
		unchecked
		{
			if (this.m_CanSpawn && !this.m_IsDead && !this.m_IsSpawning && this.m_SpawnCount < this.m_SpawnLimit && !(this.m_SpawnTimer < this.m_SpawnTimerLimit))
			{
				this.m_IsSpawning = true;
				this.m_SpawnCount++;
				this.m_SpawnTimer = 0f;
				Vector3 vector;
				vector = new Vector3(this.GetRandomCircle(1.5f, 2f), 0f, this.GetRandomCircle(1.5f, 2f));
				vector += base.transform.position;
				Vector3 euler;
				euler = new Vector3(0f, global::UnityEngine.Random.Range(-45f, 45f), 0f);
				euler += base.transform.eulerAngles;
				if (NavMesh.SamplePosition(vector, out var hit, 15f, -1))
				{
					vector = hit.position;
				}
				Character character;
				character = GameManager.Instance.AssetManager.CreateAsset<Character>(this.m_InkWidowPrefabs[global::UnityEngine.Random.Range(0, this.m_InkWidowPrefabs.Length)]);
				character.transform.position = vector;
				character.transform.rotation = Quaternion.Euler(euler);
				this.m_InkWidows.Add(character);
				character.OnInitializeOnComplete -= this.HandleInkWidowOnInitializeOnComplete;
				character.OnInitializeOnComplete += this.HandleInkWidowOnInitializeOnComplete;
			}
		}
	}

	private void HandleInkWidowOnInitializeOnComplete(object sender, EventArgs e)
	{
		Character character;
		character = sender as Character;
		character.OnInitializeOnComplete -= this.HandleInkWidowOnInitializeOnComplete;
		if (this.m_CanSpawn)
		{
			character.SetState(State.Character.Cutscene);
			character.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
			character.Content.Model.transform.position = base.transform.position;
			character.Content.Model.transform.eulerAngles = base.transform.eulerAngles;
			this.m_SpawnClip.Initialize();
			this.m_CharacterContent.UpdateClipOverrides(this.m_SpawnClip.AnimationClip);
			this.m_CharacterContent.SetAnimationTrigger("Interact");
			this.m_InkWidowSpawnClip.Initialize();
			character.Content.UpdateClipOverrides(this.m_InkWidowSpawnClip.AnimationClip);
			character.Content.SetAnimationTrigger("InteractInstant");
			character.OnCharacterDeath -= this.HandleInkWidowOnCharacterDeath;
			character.OnCharacterDeath += this.HandleInkWidowOnCharacterDeath;
			character.OnAnimationEnter -= this.HandleSpawnedInkWidowOnAnimationEnter;
			character.OnAnimationEnter += this.HandleSpawnedInkWidowOnAnimationEnter;
			character.OnAnimationComplete -= this.HandleSpawnedInkWidowOnAnimationComplete;
			character.OnAnimationComplete += this.HandleSpawnedInkWidowOnAnimationComplete;
			character.OnTargetLost -= this.HandleSpawnedInkWidowOnLostTarget;
			character.OnTargetLost += this.HandleSpawnedInkWidowOnLostTarget;
		}
		else
		{
			this.KillInkWidow(character);
		}
	}

	private void HandleInkWidowOnCharacterDeath(object sender, EventArgs e)
	{
		Character character;
		character = (Character)sender;
		character.OnCharacterDeath -= this.HandleInkWidowOnCharacterDeath;
		this.RemoveInkWidow(character);
	}

	private void HandleSpawnedInkWidowOnAnimationEnter(object sender, EventArgs e)
	{
		Character character;
		character = (Character)sender;
		character.OnAnimationEnter -= this.HandleSpawnedInkWidowOnAnimationEnter;
		character.Content.Model.transform.DOKill();
		if (this.m_CanSpawn)
		{
			character.Content.Model.transform.DOLocalMove(new Vector3(0f, 0f, -2.15f), 0.25f).SetEase(Ease.Linear);
			character.Content.Model.transform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.Linear);
		}
		else
		{
			this.KillInkWidow(character);
		}
	}

	private void HandleSpawnedInkWidowOnAnimationComplete(object sender, EventArgs e)
	{
		Character character;
		character = (Character)sender;
		character.OnAnimationComplete -= this.HandleSpawnedInkWidowOnAnimationComplete;
		if (this.m_CanSpawn)
		{
			character.Content.Model.transform.localPosition = Vector3.zero;
			character.Content.Model.transform.localEulerAngles = Vector3.zero;
			character.SetTarget(GameManager.Instance.Player.transform);
			character.SetState(State.Character.Follow);
			character.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
			character.Agent.ResetAgent();
		}
		else
		{
			this.KillInkWidow(character);
		}
	}

	private void HandleSpawnedInkWidowOnLostTarget(object sender, EventArgs e)
	{
		((Character)sender).OnTargetLost -= this.HandleSpawnedInkWidowOnLostTarget;
		this.OnFlee.Send(this);
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		if (this.m_AutoSpawn)
		{
			this.m_PauseAutoSpawn = true;
		}
		this.ClearInkWidows();
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		if (this.m_AutoSpawn)
		{
			this.m_PauseAutoSpawn = false;
		}
		this.m_SpawnTimer = this.m_SpawnTimerLimit;
		this.m_SpawnCount = 0;
		if (this.m_InkWidows != null)
		{
			for (int i = 0; i < this.m_InkWidows.Count; i = unchecked(i + 1))
			{
				Character character;
				character = this.m_InkWidows[i];
				character.SetTarget(GameManager.Instance.Player.transform);
				character.SetState(State.Character.Follow);
			}
		}
	}

	public void AnimationComplete()
	{
		this.m_IsSpawning = false;
	}

	public void Hit(RaycastHit hit)
	{
		GameManager.Instance.GameCamera.ShakeCamera(0.2f, 1.75f, 5, 90f, fadeOut: false);
		GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink").transform.position = hit.point;
		if (this.m_IsDead)
		{
			return;
		}
		ShaderEffects.Fade("_PostHitGlow", 2f, 0.3f, 0.75f, this.m_CharacterContent.ModelRenderers);
		this.OnHit.Send(this);
		if (this.m_IsSpawning)
		{
			return;
		}
		unchecked
		{
			this.m_Health++;
			if (this.m_Health >= this.MaxHitPoints)
			{
				this.m_IsDead = true;
				this.m_CharacterContent.SetAnimationTrigger("Death");
				Collider[] componentsInChildren;
				componentsInChildren = base.transform.GetComponentsInChildren<Collider>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
			}
			else if (!this.m_IsSpawning)
			{
				this.CheckHitType();
				this.m_CharacterContent.Animator.Hit(this.m_HitType);
			}
		}
	}

	public void Activate()
	{
		this.m_CanSpawn = true;
		this.m_SpawnTimer = 0f;
		this.m_SpawnDistance = 8f;
		this.m_ActiveDistance = 18f;
		this.m_InactiveDistance = 20f;
	}

	public void Deactivate()
	{
		this.m_CanSpawn = false;
		this.m_IsSpawning = false;
		this.m_SpawnTimer = 0f;
		this.m_SpawnCount = 0;
		this.m_SpawnDistance = 0f;
		this.m_ActiveDistance = 9f;
		this.m_InactiveDistance = 10f;
	}

	public void Death()
	{
		this.m_Health = this.MaxHitPoints;
		this.m_IsDead = true;
		this.m_CanSpawn = false;
		this.m_IsSpawning = false;
		this.m_CharacterContent.SetAnimationTrigger("Death");
	}

	private void ClearInkWidows()
	{
		if (this.m_InkWidows == null)
		{
			return;
		}
		for (int i = 0; i < this.m_InkWidows.Count; i = unchecked(i + 1))
		{
			Character character;
			character = this.m_InkWidows[i];
			if (character != null)
			{
				character.OnInitializeOnComplete -= this.HandleInkWidowOnInitializeOnComplete;
				character.OnCharacterDeath -= this.HandleInkWidowOnCharacterDeath;
				character.OnAnimationEnter -= this.HandleSpawnedInkWidowOnAnimationEnter;
				character.OnAnimationComplete -= this.HandleSpawnedInkWidowOnAnimationComplete;
				character.OnTargetLost -= this.HandleSpawnedInkWidowOnLostTarget;
				character.Dispose();
			}
		}
		this.m_InkWidows.Clear();
	}

	public void RemoveInkWidow(Character inkWidow)
	{
		if (inkWidow == null)
		{
			return;
		}
		if (this.m_InkWidows.Contains(inkWidow))
		{
			this.m_InkWidows.Remove(inkWidow);
		}
		unchecked
		{
			if (this.m_CanSpawn)
			{
				if (this.m_SpawnCount > 0)
				{
					this.m_SpawnCount--;
				}
				if (this.m_SpawnCount < 0)
				{
					this.m_SpawnCount = 0;
				}
			}
		}
	}

	public void KillInkWidow(Character inkWidow)
	{
		if (!(inkWidow == null))
		{
			this.RemoveInkWidow(inkWidow);
			inkWidow.Dispose();
		}
	}

	private bool IsSpawnDistance()
	{
		bool flag;
		flag = this.m_Distance < this.m_SpawnDistance;
		if (flag)
		{
			Vector3 vector;
			vector = base.transform.position + Vector3.up;
			if (Physics.SphereCast(direction: (GameManager.Instance.GameCamera.transform.position - vector).normalized, origin: base.transform.position + Vector3.up, radius: 0.25f, hitInfo: out var hitInfo, maxDistance: float.PositiveInfinity, layerMask: ~(int)this.m_LayerMask, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
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

	private bool IsInactiveDistance()
	{
		return this.m_Distance > this.m_InactiveDistance;
	}

	private bool IsActiveDistance()
	{
		return this.m_Distance < this.m_ActiveDistance;
	}

	private float GetRandomCircle(float min, float max)
	{
		if (global::UnityEngine.Random.value < 0.5f)
		{
			return global::UnityEngine.Random.Range(min, max);
		}
		return global::UnityEngine.Random.Range(0f - min, 0f - max);
	}

	private void CheckHitType()
	{
		while (this.m_HitType == this.m_LastHitType)
		{
			this.m_HitType = this.m_HitTypes[global::UnityEngine.Random.Range(0, this.m_HitTypes.Length)];
		}
		this.m_LastHitType = this.m_HitType;
	}

	protected override void OnDisposed()
	{
		this.OnFlee = null;
		this.OnHit = null;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
			GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		}
		this.ClearInkWidows();
		this.m_InkWidows = null;
		this.m_CharacterContent = null;
		base.OnDisposed();
	}
}
