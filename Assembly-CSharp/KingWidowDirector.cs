using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class KingWidowDirector : JMonoBehaviour
{
	[Header("Locations")]
	[SerializeField]
	private Transform m_InitialLocation;

	[SerializeField]
	private CharacterNode m_AnimationLocation;

	[SerializeField]
	private Transform m_ShootLocation;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClip m_PlayerDeathAnimationClip;

	[SerializeField]
	private AnimationClip m_KingWidowPlayerDeathAnimationClip;

	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy m_EnemyPrefab;

	[SerializeField]
	private InkWidowEgg m_ShotPrefab;

	private Enemy m_Enemy;

	private Transform[] m_ShootLocations;

	private bool m_IsActive;

	private bool m_IsAttacking;

	private int m_ShotIndex = 1;

	private int m_EnemyIndex;

	private int m_HitPoints;

	private int m_PhaseHits;

	public event EventHandler OnJumpUp;

	public event EventHandler OnJumpDown;

	public event EventHandler OnSpit;

	public event EventHandler OnPlayerDeath;

	public event EventHandler OnComplete;

	public void Initialize(Transform startLocation)
	{
		this.m_ShootLocations = this.m_AnimationLocation.GetComponentsInChildren<Transform>();
		for (int i = 0; i < this.m_AnimationClipOverrideGroup.Length; i = unchecked(i + 1))
		{
			this.m_AnimationClipOverrideGroup[i].Initialize();
		}
		this.m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(this.m_EnemyPrefab);
		this.m_Enemy.transform.position = startLocation.position;
		this.m_Enemy.transform.eulerAngles = startLocation.eulerAngles;
		this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		this.m_Enemy.OnInitializeOnComplete += this.HandleEnemyOnInitializeOnComplete;
		this.m_Enemy.Initialize();
		this.m_Enemy.SetSection(base.GetComponentInParent<Section>().SectionID);
	}

	private void Update()
	{
		if (this.m_IsAttacking && this.m_IsActive && !base.IsDisposed && !GameManager.Instance.IsPaused && GameManager.Instance.Player.CombatStatus == CombatStatus.None)
		{
			this.m_IsAttacking = true;
			this.m_Enemy.SetHitAnimation(active: true);
			this.m_Enemy.SetTarget(GameManager.Instance.Player.transform);
			this.m_Enemy.SetState(State.Character.Follow);
		}
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		this.SetDeathSequence(active: true);
		this.m_IsActive = true;
		this.m_IsAttacking = true;
		this.m_Enemy.SetHitAnimation(active: true);
		this.m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		this.m_Enemy.SetState(State.Character.Follow);
		this.UpdateAnimationClips("Ground");
		this.m_Enemy.OnDamageTaken -= this.HandleEnemyOnDamageTaken;
		this.m_Enemy.OnDamageTaken += this.HandleEnemyOnDamageTaken;
	}

	private void HandleEnemyOnDamageTaken(object sender, EventArgs e)
	{
		unchecked
		{
			if (this.m_IsAttacking)
			{
				this.m_HitPoints++;
				this.m_PhaseHits++;
				if (this.m_HitPoints >= this.GetHitPointMax())
				{
					this.KillKingWidow();
				}
				else if (this.m_PhaseHits >= this.GetPhaseHitMax())
				{
					this.NextPhase();
				}
			}
		}
	}

	private int GetHitPointMax()
	{
		int result;
		result = 9;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Normal:
			result = 12;
			break;
		case DifficultyLevel.Hard:
			result = 15;
			break;
		case DifficultyLevel.Impossible:
			result = 30;
			break;
		}
		return result;
	}

	private int GetPhaseHitMax()
	{
		int result;
		result = 3;
		switch (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty)
		{
		case DifficultyLevel.Normal:
			result = 4;
			break;
		case DifficultyLevel.Hard:
			result = 5;
			break;
		case DifficultyLevel.Impossible:
			result = 10;
			break;
		}
		return result;
	}

	private void NextPhase()
	{
		this.m_IsAttacking = false;
		this.m_Enemy.SetHitAnimation(active: false);
		this.m_Enemy.OnDamageTaken -= this.HandleEnemyOnDamageTaken;
		this.m_Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
		this.SetDeathSequence(active: false);
		this.m_Enemy.SetTarget(null);
		this.m_Enemy.SetNode(null);
		this.m_Enemy.SetDisableVision(active: true);
		this.m_Enemy.Movement.SetMoveSpeed(9f);
		this.m_Enemy.Movement.SetRunSpeed(9f);
		this.UpdateAnimationClips("Flee");
		this.m_ShotIndex = 1;
		this.m_PhaseHits = 0;
		Vector3 eulerAngles;
		eulerAngles = this.m_InitialLocation.eulerAngles;
		eulerAngles.y += unchecked(45 * global::UnityEngine.Random.Range(1, 9));
		this.m_AnimationLocation.transform.eulerAngles = eulerAngles;
		this.m_Enemy.OnNodeReached += this.HandleEnemyOnNodeReached;
		this.m_Enemy.SetNode(this.m_AnimationLocation);
		this.m_Enemy.SetState(State.Character.Flee);
	}

	private void KillKingWidow()
	{
		this.m_IsActive = false;
		this.m_IsAttacking = false;
		this.m_Enemy.SetHitAnimation(active: false);
		this.m_PhaseHits = 0;
		this.m_Enemy.SetState(State.Character.Death);
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		GameManager.Instance.Player.SetDeathSequence(active: false);
		this.m_Enemy.OnDamageTaken -= this.HandleEnemyOnDamageTaken;
		this.m_Enemy.SetTarget(null);
		this.m_Enemy.ForceStop(smooth: false);
		this.m_Enemy.Agent.Agent.enabled = false;
		this.m_Enemy.Controller.enabled = false;
		Collider[] componentsInChildren;
		componentsInChildren = this.m_Enemy.Content.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].enabled = true;
		}
		EventTriggerDamage componentInChildren;
		componentInChildren = this.m_Enemy.GetComponentInChildren<EventTriggerDamage>();
		if (componentInChildren != null)
		{
			componentInChildren.Dispose();
		}
		this.m_Enemy.Content.Animator.SetTrigger("Death");
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyOnDeathComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleEnemyOnDeathComplete;
		this.OnComplete.Send(this);
	}

	private void HandleEnemyOnDeathComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyOnDeathComplete;
		LootableEnemy lootableEnemy;
		lootableEnemy = new GameObject("LootableEnemy").AddComponent<LootableEnemy>();
		lootableEnemy.Initialize(this.m_Enemy.Content.gameObject, this.m_Enemy.SectionID, this.m_Enemy.EnemyType);
		lootableEnemy.AddData();
		this.m_Enemy.Dispose();
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		this.m_Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
		this.m_Enemy.SetState(State.Character.Cutscene);
		this.m_Enemy.ForceStop(smooth: false);
		this.UpdateAnimationClips("JumpUp");
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyJumpUpOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleEnemyJumpUpOnAnimationComplete;
		this.m_Enemy.Content.SetAnimationTrigger("Interact");
		this.m_Enemy.Controller.enabled = false;
		Collider[] componentsInChildren;
		componentsInChildren = this.m_Enemy.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].enabled = false;
		}
		this.m_Enemy.Agent.Agent.enabled = false;
		this.OnJumpUp.Send(this);
	}

	private void HandleEnemyJumpUpOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyJumpUpOnAnimationComplete;
		this.m_EnemyIndex = 8;
		this.UpdateAnimationClips("SpitEggs");
		this.m_Enemy.OnAnimationEvent -= this.HandleEnemyOnAnimationEvent;
		this.m_Enemy.OnAnimationEvent += this.HandleEnemyOnAnimationEvent;
		this.m_Enemy.Content.SetAnimationTrigger("Interact");
	}

	private void HandleEnemyOnAnimationEvent(object sender, EventArgs e)
	{
		this.OnSpit.Send(this);
		Vector3 position;
		position = this.m_ShootLocations[this.m_ShotIndex].position;
		InkWidowEgg inkWidowEgg;
		inkWidowEgg = GameManager.Instance.AssetManager.CreateAsset<InkWidowEgg>(this.m_ShotPrefab);
		inkWidowEgg.transform.position = this.m_ShootLocation.position;
		inkWidowEgg.transform.localScale = Vector3.zero;
		inkWidowEgg.transform.DOScale(Vector3.one, 0.1f);
		inkWidowEgg.transform.DOMove(position, 0.35f).SetEase(Ease.Linear);
		ShortcutExtensions.DORotate(endValue: new Vector3(720f, 720f, 720f), target: inkWidowEgg.transform, duration: 0.35f, mode: RotateMode.LocalAxisAdd).SetEase(Ease.Linear).OnComplete(() =>
		{
			inkWidowEgg.OnDeath -= this.HandleInkWidowOnDeath;
			inkWidowEgg.OnDeath += this.HandleInkWidowOnDeath;
			inkWidowEgg.Initialize();
		});
		unchecked
		{
			this.m_ShotIndex++;
		}
	}

	private void HandleInkWidowOnDeath(object sender, EventArgs e)
	{
		((InkWidowEgg)sender).OnDeath -= this.HandleInkWidowOnDeath;
		unchecked
		{
			this.m_EnemyIndex--;
			if (this.m_ShotIndex >= this.m_ShootLocations.Length && this.m_EnemyIndex <= 0)
			{
				this.UpdateAnimationClips("JumpDown");
				this.m_Enemy.OnAnimationComplete -= this.HandleEnemyJumpDownOnAnimationComplete;
				this.m_Enemy.OnAnimationComplete += this.HandleEnemyJumpDownOnAnimationComplete;
				this.m_Enemy.Content.SetAnimationTrigger("Interact");
				this.m_Enemy.Controller.enabled = true;
				Collider[] componentsInChildren;
				componentsInChildren = this.m_Enemy.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = true;
				}
				this.m_Enemy.Agent.Agent.enabled = true;
				Vector3 position;
				position = GameManager.Instance.Player.transform.position;
				position.y = this.m_Enemy.transform.position.y;
				ShortcutExtensions.DORotateQuaternion(endValue: Quaternion.LookRotation((position - this.m_Enemy.transform.position).normalized), target: this.m_Enemy.transform, duration: 0.75f).SetDelay(0.25f).SetEase(Ease.Linear);
				this.OnJumpDown.Send(this);
			}
		}
	}

	private void HandleEnemyJumpDownOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyJumpDownOnAnimationComplete;
		this.UpdateAnimationClips("Ground");
		this.m_Enemy.SetNode(null);
		this.m_Enemy.SetDisableVision(active: false);
		this.m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		this.m_Enemy.SetState(State.Character.Follow);
		this.SetDeathSequence(active: true);
		this.m_Enemy.Movement.SetMoveSpeed(3f);
		this.m_Enemy.Movement.SetRunSpeed(3f);
		this.m_Enemy.OnDamageTaken -= this.HandleEnemyOnDamageTaken;
		this.m_Enemy.OnDamageTaken += this.HandleEnemyOnDamageTaken;
		this.m_IsAttacking = true;
		this.m_Enemy.SetHitAnimation(active: true);
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		GameManager.Instance.Player.ForceAbilitiesCancel();
		this.m_Enemy.SetDisableVision(active: true);
		this.m_Enemy.SetState(State.Character.Cutscene);
		this.m_Enemy.ForceStop();
		this.m_Enemy.Controller.enabled = false;
		Collider[] componentsInChildren;
		componentsInChildren = this.m_Enemy.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].enabled = false;
		}
		this.m_Enemy.Agent.Agent.enabled = false;
		this.m_KingWidowPlayerDeathAnimationClip.name = "Death";
		this.m_Enemy.Content.UpdateClipOverride(this.m_KingWidowPlayerDeathAnimationClip);
		this.m_Enemy.Content.SetAnimationTrigger("Death");
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyJumpUpOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyJumpDownOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyPlayerDeathOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleEnemyPlayerDeathOnAnimationComplete;
		this.m_PlayerDeathAnimationClip.name = "Cutscene";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_PlayerDeathAnimationClip);
		GameManager.Instance.Player.Death();
		CameraEffects.Damage();
		GameManager.Instance.Player.transform.DORotate(this.m_Enemy.transform.eulerAngles + new Vector3(0f, 180f, 0f), 0.3f).SetEase(Ease.InOutSine);
		GameManager.Instance.Player.transform.DOMove(this.m_Enemy.transform.position + this.m_Enemy.transform.forward * 5f, 0.3f).SetEase(Ease.InOutSine);
		this.OnPlayerDeath.Send(this);
	}

	private void HandleEnemyPlayerDeathOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyPlayerDeathOnAnimationComplete;
		GameManager.Instance.ShowGameOver(GameOverType.KingWidow);
	}

	private void SetDeathSequence(bool active)
	{
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		if (active)
		{
			GameManager.Instance.Player.OnDeath += this.HandlePlayerOnDeath;
		}
		GameManager.Instance.Player.SetDeathSequence(active);
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

	protected override void OnDisposed()
	{
		this.OnJumpUp = null;
		this.OnJumpDown = null;
		this.OnSpit = null;
		this.OnComplete = null;
		this.OnPlayerDeath = null;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		}
		if (this.m_Enemy != null)
		{
			this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
			this.m_Enemy.OnDamageTaken -= this.HandleEnemyOnDamageTaken;
			this.m_Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyJumpUpOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyJumpDownOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyPlayerDeathOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyOnDeathComplete;
			this.m_Enemy = null;
		}
		this.m_ShootLocations = null;
		base.OnDisposed();
	}
}
