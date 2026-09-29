using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShipAhoyDirector : JMonoBehaviour
{
	[Header("Locations")]
	[SerializeField]
	private Transform m_InitialLocation;

	[SerializeField]
	private CharacterNode m_AnimationLocation;

	[SerializeField]
	private CharacterNode m_PunchLocation;

	[Header("Bounds")]
	[SerializeField]
	private BoxCollider[] m_Bounds;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClip m_PlayerDeathAnimationClip;

	[SerializeField]
	private AnimationClip m_EnemyPlayerDeathAnimationClip;

	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy m_EnemyPrefab;

	private Enemy m_Enemy;

	private HittableWilson m_Wilson;

	private EventTriggerDamage m_AnchorSwingDamage;

	private bool m_IsActive;

	private bool m_IsAttacking;

	private float m_Timer;

	private float m_TimeLimit = 4f;

	private float m_AnchorTimer;

	private float m_AnchorTimeLimit = 9f;

	private int m_WilsonHitPoints;

	private int m_PhaseHits;

	private bool m_IsStuck;

	private float m_StuckTimer;

	private float m_StuckTimerLimit = 5f;

	public event EventHandler OnComplete;

	public void Initialize(Transform startLocation)
	{
		this.m_Timer = this.m_TimeLimit;
		this.m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(this.m_EnemyPrefab);
		this.m_Enemy.transform.position = startLocation.position;
		this.m_Enemy.transform.eulerAngles = startLocation.eulerAngles;
		this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		this.m_Enemy.OnInitializeOnComplete += this.HandleEnemyOnInitializeOnComplete;
		this.m_Wilson = this.m_Enemy.GetComponentInChildren<HittableWilson>();
		this.m_AnchorSwingDamage = this.m_Enemy.GetComponentInChildren<EventTriggerDamage>();
		for (int i = 0; i < this.m_AnimationClipOverrideGroup.Length; i = unchecked(i + 1))
		{
			this.m_AnimationClipOverrideGroup[i].Initialize();
		}
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		GameManager.Instance.Player.OnDeath += this.HandlePlayerOnDeath;
		GameManager.Instance.Player.SetDeathSequence(active: true);
	}

	private void Update()
	{
		if (!this.m_IsActive || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.m_IsStuck)
		{
			if (this.m_StuckTimer >= this.m_StuckTimerLimit)
			{
				this.NextPhase();
			}
			else
			{
				this.m_StuckTimer += Time.deltaTime;
			}
		}
		else
		{
			if (!this.m_IsAttacking || this.m_Enemy.CurrentState != State.Character.Follow)
			{
				return;
			}
			if (this.m_AnchorTimer >= this.m_AnchorTimeLimit)
			{
				this.m_IsAttacking = false;
				this.m_Timer = 0f;
				this.m_AnchorTimer = 0f;
				this.UpdateAnimationClips("Flee");
				this.m_Enemy.SetCanAttack(active: false);
				this.m_Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
				this.m_Enemy.OnNodeReached += this.HandleEnemyOnNodeReached;
				this.m_Enemy.SetNode(this.m_AnimationLocation);
				this.m_Enemy.Movement.SetRunSpeed(8f);
			}
			else if (this.m_Timer >= this.m_TimeLimit)
			{
				this.m_IsAttacking = false;
				this.m_Timer = 0f;
				this.UpdateAnimationClips("Flee");
				this.m_Enemy.SetCanAttack(active: false);
				Vector3 position;
				position = this.m_Enemy.transform.position;
				Vector3 vector;
				vector = GameManager.Instance.Player.transform.position;
				vector.y = position.y;
				Vector3 normalized;
				normalized = (vector - position).normalized;
				float num;
				num = Vector3.Distance(position, vector);
				if (Physics.Raycast(position + Vector3.up * 1f, normalized, out var hitInfo, float.PositiveInfinity, ~(LayerMaskUtility.Player() | LayerMaskUtility.Enemy()), QueryTriggerInteraction.Ignore))
				{
					vector = hitInfo.point + -normalized * 10f;
				}
				bool flag;
				flag = false;
				for (int i = 0; i < this.m_Bounds.Length; i = unchecked(i + 1))
				{
					if (this.m_Bounds[i].bounds.Contains(vector))
					{
						flag = true;
					}
				}
				if (num < 12f || !flag)
				{
					this.GroundSlam();
					this.m_Enemy.transform.DOKill();
					this.m_Enemy.transform.DORotateQuaternion(Quaternion.LookRotation(normalized), 0.9f).SetEase(Ease.OutSine);
					return;
				}
				vector.y = this.m_Enemy.transform.position.y;
				this.m_PunchLocation.transform.position = vector;
				this.m_PunchLocation.transform.rotation = Quaternion.LookRotation((vector - this.m_Enemy.transform.position).normalized);
				this.m_Enemy.OnNodeReached -= this.HandleEnemyPunchOnNodeReached;
				this.m_Enemy.OnNodeReached += this.HandleEnemyPunchOnNodeReached;
				this.m_Enemy.SetNode(this.m_PunchLocation);
				this.m_Enemy.Movement.SetRunSpeed(12f);
				this.m_Enemy.SetDisableVision(active: true);
				this.m_Enemy.SetState(State.Character.Flee);
			}
			else
			{
				this.m_Timer += Time.deltaTime;
				this.m_AnchorTimer += Time.deltaTime;
			}
		}
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		this.m_Enemy.Content.GenericAnimationEvents.SetReciever(this);
		this.m_EnemyPlayerDeathAnimationClip.name = "Death";
		this.m_Enemy.Content.UpdateClipOverride(this.m_EnemyPlayerDeathAnimationClip);
		this.m_Enemy.Content.SetAnimationTrigger("Death");
		this.m_PlayerDeathAnimationClip.name = "Cutscene";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_PlayerDeathAnimationClip);
		GameManager.Instance.Player.Death();
		CameraEffects.Damage();
		GameManager.Instance.Player.transform.DORotate(this.m_Enemy.transform.eulerAngles + new Vector3(0f, 180f, 0f), 0.3f).SetEase(Ease.InOutSine);
		GameManager.Instance.Player.transform.DOMove(this.m_Enemy.transform.position + this.m_Enemy.transform.forward * 8.5f, 0.3f).SetEase(Ease.InOutSine);
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		this.m_Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
		this.m_Enemy.SetState(State.Character.Cutscene);
		this.m_Enemy.ForceStop(smooth: false);
		this.m_Enemy.Agent.Agent.enabled = false;
		this.UpdateAnimationClips("AnchorAttack");
		this.m_Enemy.OnAnimationEnter -= this.HandleEnemyAnchorOnAnimationEnter;
		this.m_Enemy.OnAnimationEnter += this.HandleEnemyAnchorOnAnimationEnter;
		this.m_Enemy.OnAnimationEvent -= this.HandleEnemyAnchorOnAnimationEvent;
		this.m_Enemy.OnAnimationEvent += this.HandleEnemyAnchorOnAnimationEvent;
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyAnchorAttackOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleEnemyAnchorAttackOnAnimationComplete;
		this.m_Enemy.Content.SetAnimationTrigger("Interact");
	}

	private void HandleEnemyAnchorOnAnimationEnter(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationEnter -= this.HandleEnemyAnchorOnAnimationEnter;
		this.m_AnchorSwingDamage.SetActive(active: true);
	}

	private void HandleEnemyAnchorOnAnimationEvent(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationEvent -= this.HandleEnemyAnchorOnAnimationEvent;
		this.m_AnchorSwingDamage.SetActive(active: false);
	}

	private void HandleEnemyAnchorAttackOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyAnchorAttackOnAnimationComplete;
		this.UpdateAnimationClips("AnchorStuck");
		this.m_Wilson.SetActive(active: true);
		this.m_IsStuck = true;
	}

	private void HandleEnemyPunchOnNodeReached(object sender, EventArgs e)
	{
		this.m_Enemy.OnNodeReached -= this.HandleEnemyPunchOnNodeReached;
		this.GroundSlam();
	}

	private void GroundSlam()
	{
		this.m_Enemy.SetState(State.Character.Cutscene);
		this.m_Enemy.ForceStop(smooth: false);
		this.m_Enemy.Agent.Agent.enabled = false;
		this.UpdateAnimationClips("PunchDown");
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyPunchOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleEnemyPunchOnAnimationComplete;
		this.m_Enemy.Content.SetAnimationTrigger("Interact");
		GroundSlam groundSlam;
		groundSlam = this.m_Enemy.GetComponentInChildren<GroundSlam>(includeInactive: true);
		if (!(groundSlam != null))
		{
			return;
		}
		Sequence s;
		s = DOTween.Sequence();
		s.InsertCallback(0.95f, () =>
		{
			groundSlam.Enable();
			CharacterAction characterAction;
			characterAction = this.m_Enemy.ActionGroupAttack.Actions[0];
			if (characterAction != null)
			{
				characterAction.Action(this.m_Enemy);
			}
		});
		s.InsertCallback(4.9f, groundSlam.Disable);
	}

	private void HandleEnemyPunchOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyPunchOnAnimationComplete;
		this.UpdateAnimationClips("Default");
		this.m_Enemy.SetNode(null);
		this.m_Enemy.Agent.Agent.enabled = true;
		this.m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		this.m_Enemy.SetState(State.Character.Follow);
		this.m_Enemy.SetDisableVision(active: false);
		this.m_Enemy.Movement.SetRunSpeed(4f);
		this.m_Enemy.SetCanAttack(active: true);
		this.m_IsAttacking = true;
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		this.m_Enemy.Agent.Agent.enabled = true;
		this.m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		this.m_Enemy.SetState(State.Character.Follow);
		this.m_Enemy.SetDisableVision(active: false);
		this.UpdateAnimationClips("Default");
		this.m_Enemy.SetCanAttack(active: true);
		this.m_Wilson.OnHit -= this.HandleWilsonOnHit;
		this.m_Wilson.OnHit += this.HandleWilsonOnHit;
		this.m_IsActive = true;
		this.m_IsAttacking = true;
	}

	private void HandleWilsonOnHit(object sender, EventArgs e)
	{
		unchecked
		{
			this.m_WilsonHitPoints++;
			this.m_PhaseHits++;
			if (this.m_WilsonHitPoints >= 9)
			{
				this.KillShipAhoy();
			}
			else if (this.m_PhaseHits >= 3)
			{
				this.NextPhase();
			}
			else
			{
				this.AnchorStuckHit();
			}
		}
	}

	private void NextPhase()
	{
		if (this.m_IsStuck)
		{
			this.m_IsStuck = false;
			this.m_StuckTimer = 0f;
			this.m_PhaseHits = 0;
			this.m_Wilson.SetActive(active: false);
			this.UpdateAnimationClips("AnchorPickUp");
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyAnchorAttackPickUpOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete += this.HandleEnemyAnchorAttackPickUpOnAnimationComplete;
			this.m_Enemy.Content.SetAnimationTrigger("Interact");
		}
	}

	private void AnchorStuckHit()
	{
		if (this.m_IsStuck)
		{
			this.UpdateAnimationClips("AnchorStuckHit");
			this.m_Enemy.Content.SetAnimationTrigger("Interact");
		}
	}

	private void HandleEnemyAnchorAttackPickUpOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyAnchorAttackPickUpOnAnimationComplete;
		this.UpdateAnimationClips("Default");
		this.m_Enemy.SetNode(null);
		this.m_Enemy.Agent.Agent.enabled = true;
		this.m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		this.m_Enemy.SetState(State.Character.Follow);
		this.m_Enemy.Movement.SetRunSpeed(4f);
		this.m_Enemy.SetCanAttack(active: true);
		this.m_IsAttacking = true;
	}

	private void KillShipAhoy()
	{
		this.m_Wilson.OnHit -= this.HandleWilsonOnHit;
		this.m_IsActive = false;
		this.m_IsStuck = false;
		this.m_Timer = 0f;
		this.m_AnchorTimer = 0f;
		this.m_StuckTimer = 0f;
		this.m_PhaseHits = 0;
		this.m_Wilson.SetActive(active: false);
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		GameManager.Instance.Player.SetDeathSequence(active: false);
		GameManager.Instance.Player.Heal(unchecked((int)UpgradeCheck.GetHealth()));
		GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.SetStatus(AbilityStatus.Inactive);
		GameManager.Instance.Player.LockAbilities();
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.HideTeleport();
		}
		this.OnComplete.Send(this);
	}

	public void Deactivate()
	{
		if (this.m_Enemy != null)
		{
			this.m_Wilson.OnHit -= this.HandleWilsonOnHit;
			this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
			this.m_Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
			this.m_Enemy.Dispose();
			this.m_Enemy = null;
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		}
	}

	public void UpdateAnimationClips(string name)
	{
		List<AnimationClip> list;
		list = new List<AnimationClip>();
		unchecked
		{
			for (int i = 0; i < this.m_AnimationClipOverrideGroup.Length; i++)
			{
				AnimationClipOverrideGroup animationClipOverrideGroup;
				animationClipOverrideGroup = this.m_AnimationClipOverrideGroup[i];
				if (animationClipOverrideGroup.Name == name)
				{
					for (int j = 0; j < animationClipOverrideGroup.AnimationClipGroups.Length; j++)
					{
						list.Add(animationClipOverrideGroup.AnimationClipGroups[j].AnimationClip);
					}
					break;
				}
			}
			this.m_Enemy.Content.UpdateClipOverrides(list.ToArray());
		}
	}

	public void GameOver()
	{
		GameManager.Instance.ShowGameOver(GameOverType.ShipAhoy);
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		if (this.m_Enemy != null)
		{
			this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
			this.m_Wilson.OnHit -= this.HandleWilsonOnHit;
			this.m_Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
			this.m_Enemy.OnAnimationEnter -= this.HandleEnemyAnchorOnAnimationEnter;
			this.m_Enemy.OnAnimationEvent -= this.HandleEnemyAnchorOnAnimationEvent;
			this.m_Enemy = null;
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		}
		base.OnDisposed();
	}
}
