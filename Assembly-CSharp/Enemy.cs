using DG.Tweening;
using UnityEngine;

public class Enemy : Character
{
	private int m_EnemyID;

	[SerializeField]
	private float m_PatrolSpeed = 2f;

	[Header("Enemy Type")]
	[SerializeField]
	private EnemyType m_EnemyType;

	[Header("Health Settings")]
	[Header("Easy")]
	[SerializeField]
	private int m_MaxHitPointsEasy = 9;

	[Header("Normal")]
	[SerializeField]
	private int m_MaxHitPoints = 9;

	[Header("Hard")]
	[SerializeField]
	private int m_MaxHitPointsHard = 9;

	[Header("Hit Settings")]
	[SerializeField]
	private bool m_HasHitAnimation = true;

	[Header("Stun Settings")]
	[SerializeField]
	private AnimationClip m_StunAnimationClip;

	[Header("Loot")]
	[SerializeField]
	private bool m_IsLootable = true;

	[Header("Despawn")]
	[SerializeField]
	private AnimationClip m_DespawnAnimationClip;

	[Header("Banish")]
	[SerializeField]
	private bool m_CanBanish;

	[SerializeField]
	private AnimationClip m_AudreyBanishClip;

	[Header("Initialize Options")]
	[SerializeField]
	private bool m_UseAwake = true;

	private SectionID m_SectionID;

	private Shock m_Shock;

	protected int m_HitPoints;

	private float m_DeathCheck;

	private bool m_IsDeathChecked;

	private bool m_IsImmune;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => base.m_RunInput;

	public override float MoveXInput => base.m_MoveXSpeed;

	public override float MoveYInput => base.m_MoveSpeed;

	public override float RotateXInput => 1f;

	public override bool AttackInput => false;

	public float PatrolSpeed => this.m_PatrolSpeed;

	public EnemyType EnemyType => this.m_EnemyType;

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

	public bool HasHitAnimation => this.m_HasHitAnimation;

	public AnimationClip StunAnimationClip => this.m_StunAnimationClip;

	public bool IsLootable => this.m_IsLootable;

	public AnimationClip DespawnAnimationClip => this.m_DespawnAnimationClip;

	public bool CanBanish => this.m_CanBanish;

	public AnimationClip AudreyBanishClip => this.m_AudreyBanishClip;

	protected override bool UseAwake => this.m_UseAwake;

	public SectionID SectionID => this.m_SectionID;

	public int HitPoints => this.m_HitPoints;

	public bool IsImmune => this.m_IsImmune;

	public void SetSection(SectionID sectionID)
	{
		this.m_SectionID = sectionID;
	}

	protected override void CharacterInitialized()
	{
		this.m_Shock = base.GetComponentInChildren<Shock>();
	}

	protected override void InternalUpdate()
	{
		if (base.m_State.ID == State.Character.Death && base.Content.RagdollController == null)
		{
			if (this.m_DeathCheck >= 5f && !this.m_IsDeathChecked)
			{
				this.m_IsDeathChecked = true;
				if (base.Content.Animator != null)
				{
					base.Content.Animator.enabled = true;
					base.Content.Animator.SetTrigger("Death");
				}
			}
			else
			{
				this.m_DeathCheck += Time.deltaTime;
			}
		}
		else if (!(base.Target == null))
		{
			CharacterNode component;
			component = base.Target.GetComponent<CharacterNode>();
			if (component != null && !(component.GetComponent<CharacterInteractionNode>() != null) && Vector3.Distance(base.Target.position, base.transform.position) <= base.Agent.Agent.stoppingDistance)
			{
				base.ForceNodeOnReached(0.75f);
				base.SetTarget(null);
			}
		}
	}

	protected override void InternalFixedUpdate()
	{
		if (base.Rotation != null)
		{
			base.Rotation.SetSensitivity(3f);
		}
	}

	public override void OnAttack()
	{
		if (base.m_ActiveAction != null)
		{
			base.m_ActiveAction.Action(this);
			base.m_ActiveAction = null;
		}
	}

	public override void OnAttackRanged()
	{
		if (!(GameManager.Instance.Player == null) && !(GameManager.Instance.GameCamera == null))
		{
			Projectile component;
			component = GameManager.Instance.PoolingManager.GetFromPool("Projectiles/Projectile_Ink").GetComponent<Projectile>();
			if (base.Content.EyeSight != null)
			{
				component.transform.position = base.Content.EyeSight.position + base.transform.forward * 2f;
			}
			else
			{
				component.transform.position = base.transform.position + Vector3.up * (base.Agent.Agent.height / 2f) + base.transform.forward * 2f;
			}
			component.transform.LookAt(GameManager.Instance.GameCamera.transform.position);
			component.gameObject.SetActive(value: true);
			component.Initialize();
		}
	}

	public void SetHitPoints(int hitPoints)
	{
		this.m_HitPoints = hitPoints;
	}

	public void SetImmune(bool active)
	{
		this.m_IsImmune = active;
	}

	public override void OnHit(RaycastHit hit)
	{
		if (base.CurrentState == State.Character.Death)
		{
			return;
		}
		bool flag;
		flag = this.m_EnemyType == EnemyType.LostOneColor;
		string prefab;
		prefab = "Effects/Effects_Hit_Ink";
		if (flag)
		{
			prefab = "Effects/Effects_Hit_Ink_Color";
		}
		if (this.m_EnemyType == EnemyType.ShipAhoy)
		{
			prefab = "Effects/Effects_Hit_Smoke";
		}
		else if (this.EnemyType == EnemyType.Keeper)
		{
			prefab = "Impacts/Impact_Sparks";
		}
		GameManager.Instance.PoolingManager.GetFromPool(prefab, 6f).transform.position = hit.point;
		bool flag2;
		flag2 = false;
		bool flag3;
		flag3 = false;
		if (GameManager.Instance.Player != null && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.IsCharged)
		{
			GentPipeChargeEffects componentInChildren;
			componentInChildren = GameManager.Instance.Player.CurrentWeapon.Weapon.GetComponentInChildren<GentPipeChargeEffects>();
			if (componentInChildren != null)
			{
				flag2 = componentInChildren.IsCharged;
				flag3 = componentInChildren.IsOvercharged;
				componentInChildren.Hit();
				GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Electric", 1f).GetComponent<Impact>().Initialize(hit, isPooled: true);
			}
		}
		if (!this.m_IsImmune && (this.MaxHitPoints >= 99 || this.MaxHitPoints == -1))
		{
			this.SetImmune(active: true);
		}
		unchecked
		{
			if (!this.m_IsImmune)
			{
				this.m_HitPoints += ((!flag2) ? 1 : (flag3 ? 3 : 2));
			}
			base.SendOnDamageTaken();
			if (!this.m_IsImmune)
			{
				if (this.m_HitPoints >= this.MaxHitPoints)
				{
					base.SetState(State.Character.Death);
					base.ForceStop(smooth: false);
					base.Agent.Agent.enabled = false;
					base.Controller.enabled = false;
					base.Content.UpdateMaterials();
					if (base.Content.RagdollController != null)
					{
						base.Content.Animator.enabled = false;
						base.Content.RagdollController.Activate(hit, flag);
					}
					else
					{
						if (this.m_EnemyType == EnemyType.InkWidow)
						{
							base.Content.Animator.speed = 1f * Random.Range(1f, 1.5f);
						}
						else if (this.m_EnemyType == EnemyType.Searcher)
						{
							base.Content.Animator.speed = 1f * Random.Range(1f, 1.25f);
						}
						base.ClearAnimationTriggers();
						base.Content.Animator.SetTrigger("Death");
						Collider[] componentsInChildren;
						componentsInChildren = base.Content.GetComponentsInChildren<Collider>();
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].enabled = true;
						}
					}
					if (this.m_IsLootable)
					{
						LostOneType lostOneType;
						lostOneType = LostOneType.NONE;
						if (this.m_EnemyType == EnemyType.LostOne || this.m_EnemyType == EnemyType.LostOneColor)
						{
							LostOneCharacterContent lostOneCharacterContent;
							lostOneCharacterContent = base.Content as LostOneCharacterContent;
							if (lostOneCharacterContent != null)
							{
								lostOneType = lostOneCharacterContent.LostOneType;
							}
						}
						LootableEnemy lootableEnemy;
						lootableEnemy = new GameObject("LootableEnemy").AddComponent<LootableEnemy>();
						lootableEnemy.Initialize(base.Content.gameObject, this.m_SectionID, this.m_EnemyType, lostOneType, flag);
						lootableEnemy.AddData();
					}
					base.CharacterDeath();
					if (flag2)
					{
						GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddShockKill();
					}
					else
					{
						GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddKill();
					}
					if (this.m_EnemyType == EnemyType.Searcher || this.m_EnemyType == EnemyType.LostOne || this.m_EnemyType == EnemyType.LostOneColor)
					{
						SectionDataObject sectionDataObject;
						sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID);
						if (sectionDataObject != null && sectionDataObject.EnemyData != null)
						{
							EnemyDataObject enemyDataObject;
							enemyDataObject = (EnemyDataObject)sectionDataObject.EnemyData.GetValue(this.m_EnemyID);
							if (enemyDataObject != null)
							{
								sectionDataObject.EnemyData.Remove(enemyDataObject);
							}
						}
					}
				}
				else
				{
					this.HitAnimation();
					this.HitGlowFader();
				}
			}
			else
			{
				if (this.m_EnemyType == EnemyType.Keeper)
				{
					if (flag3 && this.m_StunAnimationClip != null)
					{
						this.OnShock();
						base.SetState(State.Character.Stun);
					}
				}
				else if (this.m_EnemyType == EnemyType.KingWidow)
				{
					this.HitAnimation();
				}
				this.HitGlowFader();
			}
			if (flag2)
			{
				this.OnShock();
			}
			if (base.Content.RagdollController != null && base.CurrentState == State.Character.Death)
			{
				base.Dispose();
			}
		}
	}

	private void HitAnimation()
	{
		if (base.m_State.ID != State.Character.Hit && base.m_State.ID != State.Character.Attack && base.m_State.ID != State.Character.Flee && base.m_State.ID != State.Character.Evade && this.m_HasHitAnimation)
		{
			base.SetState(State.Character.Hit);
		}
	}

	public void SetHitAnimation(bool active)
	{
		this.m_HasHitAnimation = active;
	}

	private void HitGlowFader()
	{
		ShaderEffects.Fade("_PostHitGlow", 2f, 0.3f, 0.75f, base.Content.ModelRenderers);
	}

	public void OnShock()
	{
		this.m_Shock?.Play();
	}

	public void AutoAttack()
	{
		if (base.CurrentState != State.Character.Flee && base.m_PreviousState != State.Character.Flee)
		{
			if (base.Target != GameManager.Instance.Player.transform)
			{
				base.SetTarget(GameManager.Instance.Player.transform);
			}
			base.SetState(State.Character.Follow);
		}
	}

	public void Takedown()
	{
		if (base.Content.Animator != null)
		{
			base.Content.Animator.SetTrigger("Banish");
		}
		base.SetState(State.Character.Death);
		base.ForceStop(smooth: false);
		base.Agent.Agent.enabled = false;
		base.Controller.enabled = false;
		Renderer[] componentsInChildren;
		componentsInChildren = base.GetComponentsInChildren<Renderer>();
		Sequence sequence;
		sequence = DOTween.Sequence();
		float duration;
		duration = 5.25f;
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			Material material;
			material = componentsInChildren[i].material;
			material.SetFloat("_DeathPower", 0.25f);
			sequence.Insert(0f, material.DOFloat(1f, "_DeathPower", duration).SetEase(Ease.InSine));
		}
		sequence.OnComplete(this.TakedownOnComplete);
	}

	private void TakedownOnComplete()
	{
		base.CharacterDeath();
		base.Dispose();
	}

	public override void OnDeath()
	{
		base.Dispose();
	}

	public void AddData()
	{
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID);
		this.m_EnemyID = base.gameObject.GetInstanceID();
		unchecked
		{
			while (sectionDataObject.EnemyData.ContainsKey(this.m_EnemyID))
			{
				this.m_EnemyID += Random.Range(-100, 100);
			}
			EnemyDataObject enemyDataObject;
			enemyDataObject = DataObject<int, EnemyDataObject>.Create(this.m_EnemyID);
			enemyDataObject.SetEnemyType(this.m_EnemyType);
			if (base.Content is LostOneCharacterContent lostOneCharacterContent)
			{
				enemyDataObject.SetLostOneType(lostOneCharacterContent.LostOneType);
			}
			if (base.CurrentNode != null)
			{
				enemyDataObject.SetCurrentNode(base.CurrentNode.ID);
			}
			enemyDataObject.SetGameObjectData(base.gameObject);
			sectionDataObject.EnemyData.Add(this.m_EnemyID, enemyDataObject);
		}
	}

	public void UpdateData()
	{
		if (!base.IsDisposed)
		{
			EnemyDataObject enemyDataObject;
			enemyDataObject = (EnemyDataObject)((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID)).EnemyData.GetValue(this.m_EnemyID);
			enemyDataObject.SetEnemyType(this.m_EnemyType);
			if (base.Content is LostOneCharacterContent lostOneCharacterContent)
			{
				enemyDataObject.SetLostOneType(lostOneCharacterContent.LostOneType);
			}
			if (base.CurrentNode != null)
			{
				enemyDataObject.SetCurrentNode(base.CurrentNode.ID);
			}
			if (GameManager.Instance.Player != null)
			{
				enemyDataObject.SetCombatState(base.Target == GameManager.Instance.Player.transform);
			}
			else
			{
				enemyDataObject.SetCombatState(inCombat: false);
			}
			enemyDataObject.SetGameObjectData(base.gameObject);
		}
	}

	public void SetData(int id)
	{
		this.m_EnemyID = id;
		EnemyDataObject enemyDataObject;
		enemyDataObject = (EnemyDataObject)((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID)).EnemyData.GetValue(this.m_EnemyID);
		if (enemyDataObject != null)
		{
			base.gameObject.SetData(enemyDataObject.GameObjectDatas);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
