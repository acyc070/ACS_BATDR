using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SeasonalEnemy : Character
{
	private int m_EnemyID;

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

	[Header("Initialize Options")]
	[SerializeField]
	private bool m_UseAwake = true;

	[Header("Alert Mesh")]
	[SerializeField]
	private MeshRenderer m_AlertMesh;

	[Header("Animations")]
	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	private SectionID m_SectionID;

	protected int m_HitPoints;

	private bool m_IsCharacterInitialized;

	private bool m_IsSeen;

	private bool m_IsLeaving;

	private CharacterInteractionNode[] m_Nodes;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => base.m_RunInput;

	public override float MoveXInput => base.m_MoveXSpeed;

	public override float MoveYInput => base.m_MoveSpeed;

	public override float RotateXInput => 1f;

	public override bool AttackInput => false;

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

	protected override bool UseAwake => this.m_UseAwake;

	public SectionID SectionID => this.m_SectionID;

	public int HitPoints => this.m_HitPoints;

	public void SetSection(SectionID sectionID)
	{
		this.m_SectionID = sectionID;
	}

	protected override void CharacterInitialized()
	{
		for (int i = 0; i < this.m_AnimationClipOverrideGroup.Length; i = unchecked(i + 1))
		{
			this.m_AnimationClipOverrideGroup[i].Initialize();
		}
		this.m_IsCharacterInitialized = true;
	}

	protected override void InternalUpdate()
	{
		if (this.m_IsCharacterInitialized && !(base.Target == null))
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

	protected override void InternalLateUpdate()
	{
		if (this.m_IsCharacterInitialized && !this.m_IsSeen && this.m_AlertMesh != null && this.m_AlertMesh.isVisible)
		{
			this.m_IsSeen = true;
			this.UpdateAnimationClips("Jump");
			base.Content.SetAnimationTrigger("Interact");
		}
	}

	public override void OnHit(RaycastHit hit)
	{
		if (base.CurrentState == State.Character.Death)
		{
			return;
		}
		base.CancelSlide();
		if (this.m_IsLeaving)
		{
			this.m_IsLeaving = false;
			base.Content.transform.DOKill();
		}
		GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink", 6f).transform.position = hit.point;
		bool flag;
		flag = false;
		bool flag2;
		flag2 = false;
		if (GameManager.Instance.Player != null && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.IsCharged)
		{
			GentPipeChargeEffects componentInChildren;
			componentInChildren = GameManager.Instance.Player.CurrentWeapon.Weapon.GetComponentInChildren<GentPipeChargeEffects>();
			if (componentInChildren != null)
			{
				flag = componentInChildren.IsCharged;
				flag2 = componentInChildren.IsOvercharged;
				componentInChildren.Hit();
				GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Electric", 1f).GetComponent<Impact>().Initialize(hit, isPooled: true);
			}
		}
		unchecked
		{
			this.m_HitPoints += ((!flag) ? 1 : (flag2 ? 3 : 2));
			base.SendOnDamageTaken();
			if (this.m_HitPoints >= this.MaxHitPoints)
			{
				base.SetState(State.Character.Death);
				base.ForceStop(smooth: false);
				base.Agent.Agent.enabled = false;
				base.Controller.enabled = false;
				base.Content.UpdateMaterials();
				base.ClearAnimationTriggers();
				base.Content.Animator.SetTrigger("Death");
				LootableEnemy lootableEnemy;
				lootableEnemy = new GameObject("LootableEnemy").AddComponent<LootableEnemy>();
				lootableEnemy.Initialize(base.Content.gameObject, this.m_SectionID, this.m_EnemyType, LostOneType.NONE, isColor: false, base.transform.parent);
				lootableEnemy.AddData();
				base.CharacterDeath();
			}
			else
			{
				this.HitAnimation();
				this.HitGlowFader();
			}
		}
	}

	private void HitAnimation()
	{
		if (base.m_State.ID != State.Character.SeasonalHit)
		{
			base.SetState(State.Character.SeasonalHit);
		}
	}

	private void HitGlowFader()
	{
		ShaderEffects.Fade("_PostHitGlow", 2f, 0.3f, 0.75f, base.Content.ModelRenderers);
	}

	public void Leave()
	{
		this.m_IsLeaving = true;
		this.UpdateAnimationClips("Jump");
		base.Content.SetAnimationTrigger("Interact");
	}

	public void EnterLeave()
	{
		if (this.m_IsLeaving)
		{
			base.Controller.enabled = false;
			base.Content.transform.DOKill();
			base.Content.transform.DOScale(0f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
			{
				GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Looted_Enemy", 10f).GetComponent<LootedEnemy>().Initialize(base.transform.position);
				GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Hit_Ink", 6f).transform.position = base.transform.position;
				base.Dispose();
			});
		}
	}

	public void JumpComplete()
	{
		if (!this.m_IsLeaving)
		{
			this.GetRandomNode();
		}
	}

	public void GetRandomNode()
	{
		base.SetNode(this.m_Nodes[Random.Range(0, this.m_Nodes.Length)]);
	}

	public void SetNodes(CharacterInteractionNode[] nodes)
	{
		this.m_Nodes = nodes;
	}

	public override void OnDeath()
	{
		base.Dispose();
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
			base.Content.UpdateClipOverrides(list.ToArray());
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
