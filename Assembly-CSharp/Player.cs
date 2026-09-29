using System;
using System.Collections.Generic;
using DG.Tweening;
using InControl;
using S13Audio.BATDR;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-90)]
public class Player : PlayerStateMachine
{
	[Header("Camera Transforms")]
	[SerializeField]
	private Transform m_CameraPivot;

	[SerializeField]
	private Transform m_HeadContainer;

	[SerializeField]
	private Transform m_CameraContainer;

	[SerializeField]
	private Transform m_AnimationContainer;

	[Header("Head Tracking")]
	[SerializeField]
	private Transform m_HeadBobTracker;

	[SerializeField]
	private TransformTracker m_HeadTracker;

	[Header("Arm Tracking")]
	[SerializeField]
	private Transform m_ArmPivot;

	[SerializeField]
	private TransformTracker m_WeaponTracker;

	[SerializeField]
	private TransformTracker m_AbilityTracker;

	[Header("Parent Transforms")]
	[SerializeField]
	private Transform m_WeaponParent;

	[SerializeField]
	private Transform m_BodyWeaponParent;

	[Header("Controllers")]
	[SerializeField]
	private PlayerMovement m_PlayerMovement;

	[SerializeField]
	private PlayerLook m_PlayerLook;

	[SerializeField]
	private PlayerLook m_PlayerArmLook;

	[SerializeField]
	private PlayerInteraction m_PlayerInteraction;

	[SerializeField]
	private CameraFOV m_CameraFOV;

	private GameObject m_InternalTracker;

	private float m_RunTimer;

	private float m_RunCooldown = 5f;

	private float m_ShimmyX;

	private float m_Health;

	private bool m_CanStealth = true;

	private bool m_IsDead;

	private bool m_HasDeathSequence;

	private bool m_CanHeadContainerSlerp = true;

	private bool m_CanCameraPivotLerp = true;

	private bool m_IsRunCooldown;

	private bool m_InternalJump;

	private Sequence m_CrouchSequence;

	private bool m_InternalCrouch;

	private List<Character> m_Enemies = new List<Character>();

	private int m_HandWeaponAttackType;

	private Sequence m_WeaponSequence;

	private Sequence m_AbilitySequence;

	private PlayerWeaponGentPipe m_WeaponState;

	private Tweener m_DeathVibration;

	[Header("S13 Section")]
	[SerializeField]
	private PlayerEvents m_PlayerEvents;

	[SerializeField]
	private LayerMask m_FloorMaterialIgnoreLayers;

	public InkDemon test_inkdemon;

	private bool __InkDemonActive;

	public Transform CameraPivot => this.m_CameraPivot;

	public Transform HeadContainer => this.m_HeadContainer;

	public Transform CameraParent => this.m_CameraContainer;

	public Transform AnimationContainer => this.m_AnimationContainer;

	public Transform WeaponParent => this.m_WeaponParent;

	public Transform BodyWeaponParent => this.m_BodyWeaponParent;

	public PlayerMovement PlayerMovement => this.m_PlayerMovement;

	public PlayerInteraction Interaction => this.m_PlayerInteraction;

	public CameraFOV CameraFOV => this.m_CameraFOV;

	protected override bool UseAwake => false;

	public float Health => this.m_Health;

	public CameraMovements CameraMovement { get; private set; }

	public GameCamera GameCamera { get; private set; }

	public PlayerContent PlayerContent { get; private set; }

	public PlayerModelLayers ModelLayers { get; private set; }

	public Material HandMaterial { get; private set; }

	public CharacterController CharacterController { get; private set; }

	public CapsuleCollider Collider { get; private set; }

	public PlayerMusicController PlayerMusicController { get; private set; }

	public Animator AnimatorBody => this.PlayerContent.Animator;

	public Animator AnimatorArms => this.GameCamera.FirstPersonArmsAnimator;

	public bool IsGrounded => this.CharacterController.isGrounded;

	public bool IsCrouchSequenceActive
	{
		get
		{
			if (this.m_CrouchSequence != null)
			{
				return this.m_CrouchSequence.IsPlaying();
			}
			return false;
		}
	}

	public bool IsCrouched => this.m_InternalCrouch;

	public bool IsWeaponLocked { get; private set; }

	public bool IsAbilityLocked { get; private set; }

	public CombatStatus CombatStatus { get; private set; }

	public CombatStatus PreviousCombatStatus { get; private set; }

	public BattleStatus BattleStatus { get; private set; }

	public SectionID CurrentSectionID { get; private set; }

	public string CurrentZone { get; private set; }

	public GameObject ActiveWeaponDuplicate { get; private set; }

	public GameObject ActiveWeaponMirrorDuplicate { get; private set; }

	public bool IsAttacking { get; private set; }

	public bool IsUnderWanter => GameManager.Instance.GameCamera.IsUnderWater;

	public BATDRPlayerAudioController.FloorMaterials CurrentFloorType { get; private set; }

	public BATDRPlayerAudioController PlayerAudioController { get; private set; }

	public bool _inkDemonActive => this.__InkDemonActive;

	public event EventHandler OnAbilityUse;

	public event EventHandler OnAnimationComplete;

	public event EventHandler OnAnimationInteract;

	public event EventHandler OnDeath;

	public event EventHandler OnRespawn;

	public event EventHandler OnAttackStart;

	public event EventHandler OnForceDeath;

	public void SetSectionID(SectionID sectionID)
	{
		this.CurrentSectionID = sectionID;
	}

	public string SetZone(string zone)
	{
		return this.CurrentZone = zone;
	}

	protected override void InternalInitializeOnComplete()
	{
		UIManager.SetCursor(active: false);
		this.m_Health = UpgradeCheck.GetHealth();
		this.CharacterController = base.GetComponent<CharacterController>();
		this.Collider = base.gameObject.AddComponent<CapsuleCollider>();
		this.UpdateCollider();
		this.CameraMovement = base.GetComponentInChildren<CameraMovements>(includeInactive: true);
		this.GameCamera = base.GetComponentInChildren<GameCamera>(includeInactive: true);
		if (this.GameCamera != null)
		{
			this.GameCamera.Initialize(this.m_HeadContainer, this.m_CameraContainer);
			this.m_CameraFOV.Init(this.GameCamera.Camera, this.GameCamera.FirstPersonCamera);
		}
		this.m_PlayerMovement.Initialize(this.CharacterController);
		this.m_PlayerLook.Initialize(base.transform, this.m_HeadContainer);
		this.m_PlayerArmLook.Initialize(base.transform, this.m_ArmPivot);
		this.m_InternalTracker = new GameObject("PlayerInternalTracker");
		this.m_InternalTracker.transform.SetParent(base.transform);
		this.m_InternalTracker.transform.position = this.m_HeadBobTracker.position;
		this.HandMaterial = GameManager.Instance.AssetManager.GetAsset<Material>("AudreyInkHand");
		if (this.HandMaterial != null)
		{
			this.HandMaterial.SetFloat("_EmitPower", 0f);
		}
		this.PlayerContent = base.GetComponentInChildren<PlayerContent>();
		this.ModelLayers = this.PlayerContent.GetComponentInChildren<PlayerModelLayers>();
		this.PlayerAudioController = this.PlayerContent.GetComponentInChildren<BATDRPlayerAudioController>();
		this.SetCombatStatus(CombatStatus.None);
		base.SetState(State.Player.Default);
		this.DisableAllTrackers();
		this.ModelLayers.EnableFirstPerson();
		this.PlayerMusicController = base.GetComponentInChildren<PlayerMusicController>();
		if (this.PlayerMusicController != null)
		{
			this.PlayerMusicController.Initialize();
		}
		this.SetHeadTracker(active: true);
		this.ShowFirstPersonArms();
	}

	public void UpdatePlayerContentClipOverrides(params AnimationClip[] clips)
	{
		this.PlayerContent.UpdateClipOverrides(clips);
	}

	private void UpdateCollider(float height)
	{
		this.CharacterController.height = height;
		this.CharacterController.center = new Vector3(0f, height / 2f, 0f);
		this.UpdateCollider();
	}

	private void UpdateCollider()
	{
		this.Collider.center = this.CharacterController.center;
		this.Collider.radius = this.CharacterController.radius;
		this.Collider.height = this.CharacterController.height;
	}

	public void ResetRunTimer()
	{
		this.m_RunTimer = 0f;
		this.m_PlayerMovement.UnlockRun();
		this.m_IsRunCooldown = false;
		GameManager.Instance.RefillSprintBar(isFilling: false);
	}

	protected override void InternalUpdate()
	{
		this.CheckEnemies();
		if (base.Disabled)
		{
			this.m_CameraFOV.UpdateVOD(this.m_PlayerMovement.IsRunning);
			return;
		}
		float stamina;
		stamina = UpgradeCheck.GetStamina();
		if (this.m_PlayerMovement.CanRun)
		{
			if (!this.m_PlayerMovement.IsRunLocked && this.m_PlayerMovement.CanJump && this.CharacterController.isGrounded && PlayerInput.Jump() && base.CurrentState == State.Player.Default)
			{
				this.m_RunTimer += stamina * 0.15f;
			}
			if (this.m_PlayerMovement.IsRunning && !this.m_PlayerMovement.IsRunLocked)
			{
				this.m_RunTimer += Time.deltaTime;
			}
			else if (!this.m_PlayerMovement.IsRunning && this.m_PlayerMovement.IsRunLocked)
			{
				this.m_RunTimer -= Time.deltaTime;
				if (this.m_RunTimer <= 0f)
				{
					this.ResetRunTimer();
				}
			}
			else if (!this.m_PlayerMovement.IsRunning && !this.m_PlayerMovement.IsRunLocked && this.m_RunTimer > 0f)
			{
				this.m_RunTimer -= Time.deltaTime * 2f;
				if (this.m_RunTimer < 0f)
				{
					this.m_RunTimer = 0f;
				}
			}
			if (this.m_RunTimer > stamina)
			{
				this.m_PlayerMovement.StopRun();
				this.m_PlayerMovement.LockRun();
				this.m_RunTimer = this.m_RunCooldown;
				this.m_IsRunCooldown = true;
				GameManager.Instance.RefillSprintBar(isFilling: true);
			}
			GameManager.Instance.ShowSprintBar(this.m_RunTimer, this.m_IsRunCooldown ? this.m_RunCooldown : stamina);
		}
		this.m_CameraFOV.UpdateVOD(this.m_PlayerMovement.IsRunning);
		this.CheckFloorMaterial();
	}

	protected override void InternalFixedUpdate()
	{
		this.m_PlayerLook.UpdateCursorLock();
		this.GetHeadTracker();
	}

	protected override void InternalLateUpdate()
	{
	}

	public void UpdateLookInput()
	{
		this.m_PlayerLook.GetInput();
		this.m_PlayerArmLook.GetInput();
	}

	public void UpdateMovementInput()
	{
		this.m_PlayerMovement.UpdateMovementInput();
		this.m_ShimmyX = PlayerInput.LookX();
		float num;
		num = (this.m_PlayerMovement.IsCrouched ? 1f : 2f);
		this.m_ShimmyX = Mathf.Clamp(this.m_ShimmyX, -1f, 1f) * num;
	}

	public void UpdateInteractionInput()
	{
		this.m_PlayerInteraction.Update(this.m_CameraContainer);
	}

	public void UpdateAbilitiesInput()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i = unchecked(i + 1))
			{
				base.Abilities[i]?.Update();
			}
		}
	}

	public void UpdateWeaponInput()
	{
		base.CurrentWeapon?.Update();
	}

	public void UpdateMovement()
	{
		this.m_PlayerMovement.UpdateMovement(base.transform);
	}

	public void UpdateRotations()
	{
		this.m_PlayerLook.Rotation(base.transform, this.m_HeadContainer);
		this.m_PlayerArmLook.Rotation(base.transform, this.m_ArmPivot);
	}

	public void UpdateAbilities()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i = unchecked(i + 1))
			{
				base.Abilities[i]?.FixedUpdate();
			}
		}
	}

	public void UpdateWeapon()
	{
		base.CurrentWeapon?.FixedUpdate();
	}

	public void UpdateAnimationRotations()
	{
		this.m_PlayerLook.Rotation(this.m_AnimationContainer, this.m_HeadContainer);
	}

	public void UpdateAnimations()
	{
		this.InternalUpdateAnimations();
	}

	public void UpdateAbilitiesLateUpdate()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i = unchecked(i + 1))
			{
				base.Abilities[i]?.LateUpdate();
			}
		}
	}

	public void UpdateWeaponLateUpdate()
	{
		base.CurrentWeapon?.LateUpdate();
	}

	private void InternalUpdateAnimations()
	{
		float num;
		num = (this.m_PlayerMovement.IsRunning ? (this.m_PlayerMovement.MovementInput.x * 2f) : this.m_PlayerMovement.MovementInput.x);
		float num2;
		num2 = (this.m_PlayerMovement.IsRunning ? (this.m_PlayerMovement.MovementInput.y * 2f) : this.m_PlayerMovement.MovementInput.y);
		if ((double)Vector3.Distance(this.m_PlayerMovement.CurrentPosition, this.m_PlayerMovement.PreviousPosition) <= 0.01)
		{
			if (num2 != 0f)
			{
				num2 = 0f;
			}
			if (num != 0f)
			{
				num = 0f;
			}
		}
		if (this.m_PlayerMovement.IsSlowed)
		{
			if (num != 0f)
			{
				num = this.m_PlayerMovement.MovementInput.x;
			}
			if (num2 != 0f)
			{
				num2 = this.m_PlayerMovement.MovementInput.y;
			}
			float num3;
			num3 = 0.25f;
			if (num > 0f)
			{
				num -= num3;
			}
			else if (num < 0f)
			{
				num += num3;
			}
			if (num2 > 0f)
			{
				num2 -= num3;
			}
			else if (num2 < 0f)
			{
				num2 += num3;
			}
		}
		if (!this.IsGrounded && !this.m_PlayerMovement.PreviouslyGrounded && !this.m_InternalJump)
		{
			if (!Physics.Linecast(end: base.transform.position + Vector3.down * 0.3f, start: base.transform.position, hitInfo: out var _, layerMask: ~(int)this.m_FloorMaterialIgnoreLayers, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				this.m_InternalJump = true;
			}
		}
		else if (this.IsGrounded && this.m_PlayerMovement.PreviouslyGrounded && this.m_InternalJump)
		{
			this.m_InternalJump = false;
			bool flag;
			flag = false;
			if (this.CharacterController.transform.position.y < this.m_PlayerMovement.InitialAirPosition.y)
			{
				Vector3 initialAirPosition;
				initialAirPosition = this.m_PlayerMovement.InitialAirPosition;
				initialAirPosition.x = this.CharacterController.transform.position.x;
				initialAirPosition.z = this.CharacterController.transform.position.z;
				this.m_PlayerMovement.InitialAirPosition = initialAirPosition;
				if (Vector3.Distance(this.m_PlayerMovement.InitialAirPosition, this.CharacterController.transform.position) > 15f)
				{
					int damage;
					damage = 2;
					if (GameManager.Instance.Player.Health <= 2f)
					{
						damage = 0;
					}
					this.Damage(damage);
					this.ShowHealthBar();
					CameraEffects.Damage();
					CameraEffects.ShakeRotation(0.5f, 2f, 10, 90f, fadeOut: false);
					this.S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Hit);
					flag = true;
				}
			}
			if (!flag)
			{
				CameraEffects.ShakeRotation(0.2f, 0.8f, 2, 90f, fadeOut: true, vibrate: false);
				this.S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Land);
			}
		}
		if (this.IsGrounded && this.m_PlayerMovement.IsCrouched && !this.m_InternalCrouch)
		{
			this.m_InternalCrouch = true;
			this.SetCanStealth(active: true);
			if (this.m_CanStealth && this.CombatStatus != CombatStatus.Combat && this.CombatStatus != CombatStatus.Hide)
			{
				GameManager.Instance.ShowStealth();
				this.SetCombatStatus(CombatStatus.Stealth);
			}
			float characterControllerHeight;
			characterControllerHeight = this.CharacterController.height;
			this.m_CrouchSequence?.Kill();
			this.m_CrouchSequence = DOTween.Sequence();
			this.m_CrouchSequence.Insert(0f, DOTween.To(() => characterControllerHeight, (float value) =>
			{
				characterControllerHeight = value;
			}, 4.5f, 0.5f).SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine)
				.OnUpdate(() =>
				{
					this.UpdateCollider(characterControllerHeight);
				}));
			this.m_CrouchSequence.OnComplete(() =>
			{
				this.UpdateCollider();
				this.m_CrouchSequence = null;
			});
			this.S13SetCrouching(this.m_InternalCrouch);
		}
		else if (this.IsGrounded && !this.m_PlayerMovement.IsCrouched && this.m_InternalCrouch)
		{
			if (!Physics.SphereCast(base.transform.position, this.CharacterController.radius, Vector3.up, out var _, 6.5f - this.CharacterController.radius, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
			{
				this.m_InternalCrouch = false;
				if (this.m_CanStealth && this.CombatStatus != CombatStatus.Combat && this.CombatStatus != CombatStatus.Hide)
				{
					GameManager.Instance.HideStealth();
					this.SetCombatStatus(CombatStatus.None);
				}
				float characterControllerHeight2;
				characterControllerHeight2 = this.CharacterController.height;
				this.m_CrouchSequence?.Kill();
				this.m_CrouchSequence = DOTween.Sequence();
				this.m_CrouchSequence.Insert(0f, DOTween.To(() => characterControllerHeight2, (float value) =>
				{
					characterControllerHeight2 = value;
				}, 6.5f, 0.25f).SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine)
					.OnUpdate(() =>
					{
						this.UpdateCollider(characterControllerHeight2);
					}));
				this.m_CrouchSequence.OnComplete(() =>
				{
					this.UpdateCollider();
					this.m_CrouchSequence = null;
				});
				this.S13SetCrouching(this.m_InternalCrouch);
			}
			else
			{
				this.m_PlayerMovement.SetCrouchInput(isCrouched: true);
			}
		}
		if (num2 > 1f)
		{
			this.SetAnimationMovementState(2f);
		}
		else if (num2 > 0f)
		{
			this.SetAnimationMovementState(1f);
		}
		else if (num2 < 0f)
		{
			this.SetAnimationMovementState(-1f);
		}
		else
		{
			this.SetAnimationMovementState(0f);
		}
		if (this.IsGrounded)
		{
			if (!this.m_PlayerMovement.IsCrouched)
			{
				this.SetAnimationCrouchState(0f);
			}
			else
			{
				this.SetAnimationCrouchState(1f);
			}
		}
		else
		{
			this.SetAnimationCrouchState(-1f);
		}
		if (num == 0f && num2 == 0f)
		{
			this.SetAnimationShimmySpeed(this.m_ShimmyX);
		}
		else
		{
			this.SetAnimationShimmySpeed(0f);
		}
		this.SetAnimationStrafeSpeed(num);
		this.SetAnimationSpeed(num2);
		this.SetAnimationCrouchSpeed(num2);
	}

	public void CheckEnemies()
	{
		if (this.CombatStatus != CombatStatus.Combat)
		{
			return;
		}
		unchecked
		{
			if (this.m_Enemies.Count > 0)
			{
				for (int num = this.m_Enemies.Count - 1; num >= 0; num--)
				{
					Character character;
					character = this.m_Enemies[num];
					if (character != null)
					{
						if (character.Target != base.transform || character.CurrentState == State.Character.Patrol)
						{
							this.RemoveEnemy(character);
						}
					}
					else
					{
						this.m_Enemies.RemoveAt(num);
					}
				}
			}
			else if (this.BattleStatus == BattleStatus.None)
			{
				if (this.IsCrouched)
				{
					this.SetCombatStatus(CombatStatus.Stealth);
				}
				else
				{
					this.SetCombatStatus(CombatStatus.None);
				}
			}
		}
	}

	public void AddEnemy(Character character)
	{
		if (!(character == null) && !this.m_Enemies.Contains(character))
		{
			this.m_Enemies.Add(character);
			this.SetCombatStatus(CombatStatus.Combat);
		}
	}

	public void RemoveEnemy(Character character)
	{
		unchecked
		{
			for (int num = this.m_Enemies.Count - 1; num >= 0; num--)
			{
				if (this.m_Enemies[num] == null)
				{
					this.m_Enemies.RemoveAt(num);
				}
			}
			if (this.m_Enemies.Contains(character))
			{
				this.m_Enemies.Remove(character);
			}
			if (this.m_Enemies.Count <= 0)
			{
				this.ClearEnemies();
			}
		}
	}

	public void ClearEnemies()
	{
		this.m_Enemies.Clear();
		if (this.CombatStatus != CombatStatus.Hide && this.CombatStatus != CombatStatus.Stealth)
		{
			this.SetCombatStatus(CombatStatus.None);
		}
	}

	public void ForceEnemiesFlee()
	{
		if (this.m_Enemies.Count <= 0)
		{
			return;
		}
		unchecked
		{
			for (int num = this.m_Enemies.Count - 1; num >= 0; num--)
			{
				Character character;
				character = this.m_Enemies[num];
				if (character != null)
				{
					if (character.CurrentNode != null)
					{
						character.SetNode(character.CurrentNode);
					}
					else if (character.PreviousNode != null)
					{
						character.SetNode(character.PreviousNode);
					}
					character.SetState(State.Character.Flee);
				}
				else
				{
					this.m_Enemies.RemoveAt(num);
				}
			}
		}
	}

	public void Hit(RaycastHit hit, Character character = null, int damage = 1)
	{
		this.Damage(damage);
		if (this.m_Health <= 0f)
		{
			if (this.m_HasDeathSequence)
			{
				if (!this.m_IsDead)
				{
					this.m_IsDead = true;
				}
				this.OnDeath.Send(this);
			}
			else
			{
				this.ForceRespawn();
			}
			return;
		}
		this.ShowHealthBar();
		CameraEffects.Damage();
		CameraEffects.ShakeRotation(0.35f, 2f, 10, 90f, fadeOut: false);
		if (damage > 0)
		{
			Vector3 point;
			point = hit.point;
			point.y = base.transform.position.y;
			this.AddForce((base.transform.position - point).normalized * (2f * (float)damage));
		}
		this.S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Hit);
	}

	public void SetHealth(int health, bool isSilent = false)
	{
		this.m_Health = health;
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetHealth(unchecked((int)this.m_Health));
		GameManager.Instance.ShowHealthBar(this.m_Health, UpgradeCheck.GetHealth());
		if (!isSilent)
		{
			BATDRPlayerHealthAudioController.SetPlayerHealth(this.m_Health);
		}
	}

	public void Damage(int damage)
	{
		if (damage > 0)
		{
			this.m_Health -= damage;
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetHealth(unchecked((int)this.m_Health));
			BATDRPlayerHealthAudioController.SetPlayerHealth(this.m_Health);
		}
	}

	public void Heal(int amount)
	{
		float health;
		health = UpgradeCheck.GetHealth();
		if (!(this.m_Health >= health))
		{
			float num;
			num = this.m_Health + (float)amount;
			if (num > health)
			{
				num = health;
			}
			this.m_Health = num;
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetHealth(unchecked((int)this.m_Health));
			GameManager.Instance.ShowHealthBar(this.m_Health, health);
			BATDRPlayerHealthAudioController.SetPlayerHealth(this.m_Health);
		}
	}

	public void ShowHealthBar()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetHealth(unchecked((int)this.m_Health));
		GameManager.Instance.ShowHealthBar(this.m_Health, UpgradeCheck.GetHealth());
	}

	public void Death()
	{
		this.GameCamera.SetFirstPersonArmsActive(active: false);
		this.ModelLayers.EnableCutscene();
		this.EnterInternalAnimation();
		this.DisableAllTrackers();
		base.SetState(State.Player.Cutscene);
		this.SetAnimationTrigger("Cutscene");
		GameManager.Instance.HideCrosshair();
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active && GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.HideTeleport();
		}
		GameManager.Instance.HideHealthBar();
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpdateLife();
	}

	public void ForceRespawn(float blockerDuration = 0f)
	{
		this.m_DeathVibration.Kill();
		if (GameManager.Instance.HasController)
		{
			float vibration;
			vibration = 2.5f;
			this.m_DeathVibration = DOTween.To(() => vibration, (float x) =>
			{
				vibration = x;
			}, 0f, 1.5f).OnUpdate(() =>
			{
				if (GameManager.Instance.HasController)
				{
					InputManager.ActiveDevice.Vibrate(vibration * 0.5f);
				}
			}).OnComplete(() =>
			{
				InputManager.ActiveDevice.Vibrate(0f);
			});
		}
		GameManager.Instance.ShowScreenBlocker(blockerDuration);
		this.ForceDeath();
	}

	public void ForceDeath()
	{
		if (this.BattleStatus == BattleStatus.None)
		{
			this.SetBattleStatus(BattleStatus.Dead);
		}
		this.S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Dead);
		this.OnForceDeath.Send(this);
		this.AnimatorBody.SetTrigger("Cancel");
		this.AnimatorArms.SetTrigger("Cancel");
		GameManager.Instance.GameCamera.SetUnderWater(active: false);
		this.ForceAbilitiesCancel();
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpdateLife();
		this.OnDeath.Send(this);
		this.SetRespawnState(GameManager.Instance.RespawnManager.GetClosest(base.transform.position));
	}

	private void SetRespawnState(PlayerRespawn playerRespawn)
	{
		if (!(playerRespawn == null))
		{
			playerRespawn.Respawn();
			this.m_Health = UpgradeCheck.GetHealth() / 2f;
			BATDRPlayerHealthAudioController.SetPlayerHealth(this.m_Health);
			this.m_IsDead = false;
		}
	}

	public void Respawn()
	{
		GameManager.Instance.ShowHealthBar(this.m_Health, UpgradeCheck.GetHealth());
		if (base.Abilities != null)
		{
			foreach (PlayerAbilityState ability in base.Abilities)
			{
				if (ability != null && ability is PlayerAbilityStateFlow)
				{
					ability.ForceCooldown();
					break;
				}
			}
		}
		this.OnRespawn.Send(this);
		if (this.BattleStatus == BattleStatus.Dead)
		{
			this.SetBattleStatus(BattleStatus.None);
		}
		this.S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.Respawn);
	}

	public void OnAttack()
	{
		this.IsAttacking = false;
		base.CurrentWeapon?.OnAttack();
	}

	public void Attack()
	{
		this.IsAttacking = true;
		int num;
		num = global::UnityEngine.Random.Range(1, 4);
		while (this.m_HandWeaponAttackType == num)
		{
			num = global::UnityEngine.Random.Range(1, 4);
		}
		this.m_HandWeaponAttackType = num;
		this.AnimatorBody.SetInteger("HandWeaponAttackType", this.m_HandWeaponAttackType);
		if (this.AnimatorArms != null)
		{
			this.AnimatorArms.SetInteger("HandWeaponAttackType", this.m_HandWeaponAttackType);
		}
		this.AnimatorBody.SetTrigger("HandWeaponAttack");
		if (this.AnimatorArms != null)
		{
			this.AnimatorArms.SetTrigger("HandWeaponAttack");
		}
		this.OnAttackStart.Send(this);
	}

	public void OnAttackComplete()
	{
		this.IsAttacking = false;
		base.CurrentWeapon?.OnAttackComplete();
	}

	public void AttackComplete()
	{
		this.IsAttacking = false;
		this.AnimatorBody.SetInteger("HandWeaponAttackType", 0);
		if (this.AnimatorArms != null)
		{
			this.AnimatorArms.SetInteger("HandWeaponAttackType", 0);
		}
		this.AnimatorBody.ResetTrigger("HandWeaponAttack");
		if (this.AnimatorArms != null)
		{
			this.AnimatorArms.ResetTrigger("HandWeaponAttack");
		}
	}

	public void Ability()
	{
		this.AnimatorBody.SetTrigger("AbilityUse");
		if (this.AnimatorArms != null)
		{
			this.AnimatorArms.SetTrigger("AbilityUse");
		}
		this.OnAbilityUse.Send(this);
	}

	public void ForceAbilitiesCancel()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i = unchecked(i + 1))
			{
				base.Abilities[i].ForceCancel();
			}
		}
	}

	public void ForceAbilitiesCooldown()
	{
		if (base.Abilities != null)
		{
			for (int i = 0; i < base.Abilities.Count; i = unchecked(i + 1))
			{
				base.Abilities[i].ForceCooldown();
			}
		}
	}

	public void LockWeapon()
	{
		this.IsWeaponLocked = true;
	}

	public void UnlockWeapon()
	{
		this.IsWeaponLocked = false;
	}

	public void LockAbilities(bool playAudio = false)
	{
		if (playAudio && this.m_PlayerEvents != null)
		{
			this.m_PlayerEvents.AbilitiesLose();
		}
		this.IsAbilityLocked = true;
	}

	public void UnlockAbilities(bool playAudio = false)
	{
		if (playAudio && this.m_PlayerEvents != null)
		{
			this.m_PlayerEvents.AbilitiesReturn();
		}
		this.IsAbilityLocked = false;
	}

	public void SendOnUseAbility()
	{
		this.OnAbilityUse.Send(this);
	}

	public void SetInitialLocation(Transform _transform)
	{
		base.transform.position = _transform.position;
		base.transform.eulerAngles = _transform.eulerAngles;
		this.ForcePlayerRotation(_transform.rotation);
		this.ResetRotation();
	}

	public void SetInitialLocation(PlayerTransform playerTransform)
	{
		base.transform.position = playerTransform.Position;
		base.transform.eulerAngles = playerTransform.Rotation;
		this.ResetRotation();
		this.ForceRotation(Quaternion.Euler(playerTransform.Rotation), Quaternion.Euler(playerTransform.HeadRotation));
	}

	public void ResetRotation()
	{
		this.m_PlayerLook.Initialize(base.transform, this.m_HeadContainer);
		this.m_PlayerArmLook.Initialize(base.transform, this.m_ArmPivot);
	}

	public void ForceRotation(Quaternion playerRotation, Quaternion cameraRotation)
	{
		this.ForcePlayerRotation(playerRotation);
		this.ForceCameraRotation(cameraRotation);
	}

	public void ForcePlayerRotation(Quaternion playerRotation)
	{
		this.m_PlayerLook.ForceRotation(playerRotation);
	}

	public void ForceCameraRotation(Quaternion cameraRotation)
	{
		this.m_PlayerLook.ForceCameraRotation(cameraRotation);
	}

	public void LockRotation(float x, float y)
	{
		this.m_PlayerLook.HorizontalClampSetActive(active: true);
		this.m_PlayerLook.SetHorizontalClamp(x);
		this.m_PlayerLook.SetVerticalClamp(y);
	}

	public void UnlockRotation()
	{
		this.m_PlayerLook.HorizontalClampSetActive(active: false);
		this.m_PlayerLook.ResetVerticalClamp();
	}

	public void AddForce(Vector3 force)
	{
		this.m_PlayerMovement.AddForce(force);
	}

	public void CancelMovement()
	{
		this.m_PlayerMovement.CancelMovement();
	}

	public void SetCanStealth(bool active)
	{
		this.m_CanStealth = active;
	}

	public void SetInteraction(bool active)
	{
		this.m_PlayerInteraction.SetActive(active);
	}

	public void SetCollision(bool active)
	{
		this.CharacterController.enabled = active;
		this.Collider.enabled = active;
	}

	public void ResetAnimation()
	{
		this.SetAnimationMovementState(0f);
		this.SetAnimationSpeed(0f);
		this.SetAnimationStrafeSpeed(0f);
		this.SetAnimationCrouchSpeed(0f);
		this.m_ShimmyX = 0f;
	}

	public void ForceResetAnimation()
	{
		this.AnimatorBody.SetMovementState(0f, smooth: false);
		this.AnimatorBody.SetMovementSpeed(0f, smooth: false);
		this.AnimatorBody.SetStrafeSpeed(0f, smooth: false);
		this.AnimatorBody.SetCrouchSpeed(0f, smooth: false);
		this.m_ShimmyX = 0f;
	}

	public void SetAnimationInt(string trigger, int value)
	{
		if (!(trigger == ""))
		{
			this.AnimatorBody.SetInteger(trigger, value);
		}
	}

	public void SetAnimationTrigger(string trigger)
	{
		this.SetBodyAnimationTrigger(trigger);
		this.SetArmAnimationTrigger(trigger);
	}

	public void ResetAnimationTrigger(string trigger)
	{
		if (!(trigger == ""))
		{
			this.AnimatorBody.ResetTrigger(trigger);
			if (this.AnimatorArms != null)
			{
				this.AnimatorArms.ResetTrigger(trigger);
			}
		}
	}

	public void SetBodyAnimationTrigger(string trigger)
	{
		if (!(trigger == ""))
		{
			this.AnimatorBody.SetTrigger(trigger);
		}
	}

	public void SetArmAnimationTrigger(string trigger)
	{
		if (!(trigger == "") && this.AnimatorArms != null)
		{
			this.AnimatorArms.SetTrigger(trigger);
		}
	}

	public void SetAnimationType(string name, int value)
	{
		this.AnimatorBody.SetInteger(name, value);
	}

	private void SetAnimationMovementState(float state)
	{
		this.AnimatorBody.SetMovementState(state);
	}

	private void SetAnimationCrouchState(float state)
	{
		this.AnimatorBody.SetCrouchState(state, smooth: true, 0.3f);
	}

	private void SetAnimationShimmySpeed(float speed, bool isSmooth = true)
	{
		this.AnimatorBody.SetShimmySpeed(speed, isSmooth);
	}

	private void SetAnimationSpeed(float speed)
	{
		this.AnimatorBody.SetMovementSpeed(speed, speed == 0f);
	}

	private void SetAnimationStrafeSpeed(float speed)
	{
		this.AnimatorBody.SetStrafeSpeed(speed, speed == 0f);
	}

	private void SetAnimationCrouchSpeed(float speed)
	{
		this.AnimatorBody.SetCrouchSpeed(speed, speed == 0f);
	}

	private void SetAnimationLayerWeight(int layer, float weight)
	{
		if (this.AnimatorBody != null)
		{
			this.AnimatorBody.SetLayerWeight(layer, weight);
		}
		if (this.AnimatorArms != null)
		{
			this.AnimatorArms.SetLayerWeight(layer, weight);
		}
	}

	private void UpdateClipOverrides(AnimationClip clip = null)
	{
		if (clip != null)
		{
			clip.name = "Cutscene";
			this.PlayerContent.UpdateClipOverrides(clip);
		}
	}

	public void SetArmTrackersActive(bool active)
	{
		this.m_AbilityTracker.SetActive(active: false);
		if (base.CurrentWeapon == null)
		{
			this.m_WeaponTracker.SetActive(active: false);
		}
		else
		{
			this.m_WeaponTracker.SetActive(active);
		}
	}

	public void ForceStand(bool isOutOfCombat = true)
	{
		this.m_CrouchSequence.Kill();
		this.AnimatorBody.SetCrouchState(0f, smooth: false);
		this.SetAnimationShimmySpeed(0f, isSmooth: false);
		this.m_InternalCrouch = false;
		this.m_PlayerMovement.ForceStand();
		this.UpdateCollider(6.5f);
		if (isOutOfCombat)
		{
			this.SetCombatStatus(CombatStatus.None);
		}
		this.S13SetCrouching(this.m_InternalCrouch);
	}

	public void ForceCrouch(bool isInternalCrouch = true)
	{
		this.m_CrouchSequence.Kill();
		this.AnimatorBody.SetCrouchState(1f, smooth: false);
		this.SetAnimationShimmySpeed(0f, isSmooth: false);
		this.m_InternalCrouch = isInternalCrouch;
		if (isInternalCrouch)
		{
			this.SetCanStealth(active: true);
			this.m_PlayerMovement.ForceCrouch();
			this.UpdateCollider(4.5f);
			this.SetCombatStatus(CombatStatus.Stealth);
		}
		this.S13SetCrouching(this.m_InternalCrouch);
	}

	public void EnableAnimationRotation(float x = 15f, float y = 20f, bool isCutscene = false)
	{
		base.SetState(isCutscene ? State.Player.CutscenePeek : State.Player.Peek);
		this.m_PlayerLook.Initialize(this.m_AnimationContainer, this.m_HeadContainer);
		this.LockRotation(x, y);
	}

	public void EnterInteraction(string trigger)
	{
		JDebug.Log("Player :: EnterInteraction :: (trigger = " + trigger + ")", this, JDebug.JDebugType.Player);
		base.SetState(State.Player.Cutscene);
		this.CancelMovement();
		this.ResetAnimation();
		this.ModelLayers.EnableCutscene();
		this.SetHeadTracker(active: false);
		this.HideFirstPersonArms();
		this.SetBodyAnimationTrigger(trigger);
		DOTween.Sequence().InsertCallback(0.05f, this.EnterInternalAnimation);
	}

	private void EnterInternalAnimation()
	{
		this.m_HeadContainer.SetParent(this.m_AnimationContainer);
	}

	public void EnterInteractionInstant(string trigger)
	{
		JDebug.Log("Player :: EnterInteractionInstant :: (trigger = " + trigger + ")", this, JDebug.JDebugType.Player);
		base.SetState(State.Player.Cutscene);
		this.CancelMovement();
		this.ForceResetAnimation();
		this.ResetAnimation();
		this.ModelLayers.EnableCutscene();
		this.SetHeadTracker(active: false);
		if (trigger.ToLower().Contains("instant"))
		{
			this.SetAnimationLayerWeight(1, 0f);
			this.SetAnimationLayerWeight(2, 0f);
		}
		this.HideFirstPersonArms();
		this.SetBodyAnimationTrigger(trigger);
		this.m_HeadContainer.SetParent(this.m_AnimationContainer);
		this.m_HeadContainer.localPosition = Vector3.zero;
		this.m_HeadContainer.localEulerAngles = Vector3.zero;
	}

	public void ExitInteraction()
	{
		JDebug.Log("Player :: ExitInteraction", this, JDebug.JDebugType.Player);
		base.SetState(State.Player.Cutscene);
		this.SetAnimationTrigger("ExitInteraction");
		if (base.gameObject.scene.buildIndex != -1 && base.gameObject.transform.parent == null)
		{
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		this.UnlockRotation();
	}

	public void ExitAnimation()
	{
		this.AnimationComplete();
		this.ResetRotation();
		DOTween.Sequence().InsertCallback(0.05f, this.ExitInternalAnimation);
	}

	private void ExitInternalAnimation()
	{
		this.EnableAllTrackers();
		this.ShowFirstPersonArms();
		this.ExitAnimationCamera();
		this.ResetRotation();
		base.SetState(State.Player.Default);
	}

	public void ExitAnimationCamera()
	{
		this.m_HeadContainer.SetParent(this.m_CameraPivot);
		this.m_AnimationContainer.localPosition = Vector3.zero;
		this.m_AnimationContainer.localEulerAngles = Vector3.zero;
	}

	public void EnterCutscene(AnimationClip animationClip)
	{
		animationClip.name = "Cutscene";
		this.UpdateClipOverrides(animationClip);
		this.EnterInteraction("Cutscene");
	}

	public void AnimationInteract()
	{
		this.OnAnimationInteract.Send(this);
	}

	public void AnimationComplete()
	{
		this.OnAnimationComplete.Send(this);
	}

	public void AnimationClear()
	{
		this.m_AnimationContainer.DOLocalMove(this.m_HeadBobTracker.localPosition, 0.25f).SetEase(Ease.InOutSine);
	}

	public void SlideToLocation(Transform location, float duration = 0.25f, Ease ease = Ease.InOutSine)
	{
		this.SlideToLocation(location.position, location.eulerAngles, duration, ease);
	}

	public void SlideToLocation(Vector3 position, Vector3 rotation, float duration = 0.25f, Ease ease = Ease.InOutSine)
	{
		base.transform.DOMove(position, duration).SetEase(ease);
		base.transform.DORotate(rotation, duration).SetEase(ease).OnComplete(() =>
		{
			this.GameCamera.SetFirstPersonArmsActive(active: false);
		});
	}

	private void EnableAllTrackers()
	{
		this.SetAllTrackers(active: true);
	}

	private void DisableAllTrackers()
	{
		this.SetAllTrackers(active: false);
	}

	public void SetHeadTracker(bool active)
	{
		this.m_HeadTracker.SetActive(active);
	}

	public void SetAllTrackers(bool active)
	{
		this.m_HeadTracker.SetActive(active);
		this.SetArmTrackersActive(active);
		this.SetAnimationLayerWeight(1, 0f);
		if (this.m_WeaponState != null && GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.WeaponStatus == WeaponStatus.Active)
		{
			this.SetAnimationLayerWeight(2, 1f);
		}
		else
		{
			this.SetAnimationLayerWeight(2, 0f);
		}
	}

	public void SetCombatStatus(CombatStatus combatStatus)
	{
		if (this.CombatStatus != combatStatus)
		{
			this.PreviousCombatStatus = this.CombatStatus;
			this.CombatStatus = combatStatus;
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.SetCombatStatus(this.CombatStatus);
			JDebug.Log("[Player] - SetCombatStatus ::  [CombatStatus: " + this.CombatStatus.ToString() + "] | [PreviousCombatStatus: " + this.PreviousCombatStatus.ToString() + "]", this, JDebug.JDebugType.Player);
		}
	}

	public void SetBattleStatus(BattleStatus battleStatus)
	{
		this.BattleStatus = battleStatus;
		JDebug.Log("[Player] - SetBattleStatus ::  [BattleStatus: " + this.BattleStatus.ToString() + "]", this, JDebug.JDebugType.Player);
	}

	public void SetDeathSequence(bool active)
	{
		this.m_HasDeathSequence = active;
	}

	public void SetCameraPivotLerp(bool active)
	{
		this.m_CanCameraPivotLerp = active;
	}

	public void SetHeadContainerSlerp(bool active)
	{
		this.m_CanHeadContainerSlerp = active;
	}

	private void GetHeadTracker()
	{
		this.m_InternalTracker.transform.position = this.m_HeadBobTracker.position;
		if (this.m_CanCameraPivotLerp)
		{
			if (GameManager.Instance.PlayerSettings.SmoothCamera)
			{
				Vector3 localPosition;
				localPosition = this.m_InternalTracker.transform.localPosition;
				Vector3 localPosition2;
				localPosition2 = this.m_CameraPivot.localPosition;
				localPosition2.x = Mathf.Lerp(localPosition2.x, localPosition.x, 5f * Time.deltaTime);
				if (this.m_PlayerMovement.IsCrouched)
				{
					localPosition2.y = Mathf.Lerp(localPosition2.y, localPosition.y, 5f * Time.deltaTime);
				}
				else
				{
					localPosition2.y = Mathf.Lerp(localPosition2.y, localPosition.y, 5f * Time.deltaTime);
				}
				localPosition2.z = Mathf.Lerp(localPosition2.z, localPosition.z, 5f * Time.deltaTime);
				this.m_CameraPivot.localPosition = localPosition2;
				if (GameManager.Instance.GameCamera.ArmsContainer != null)
				{
					Vector3 localPosition3;
					localPosition3 = GameManager.Instance.GameCamera.ArmsContainer.localPosition;
					localPosition3.x = 0f - Mathf.Lerp(this.m_CameraPivot.localPosition.x * 1.5f, localPosition.x * 1.5f, 1.5f * Time.deltaTime);
					GameManager.Instance.GameCamera.ArmsContainer.localPosition = localPosition3;
				}
			}
			else
			{
				this.m_CameraPivot.localPosition = this.m_InternalTracker.transform.localPosition;
			}
		}
		else
		{
			this.m_CameraPivot.localPosition = this.m_InternalTracker.transform.localPosition;
		}
		if (this.m_CanHeadContainerSlerp)
		{
			if (this.m_HeadContainer.localPosition != Vector3.zero)
			{
				this.m_HeadContainer.localPosition = Vector3.Slerp(this.m_HeadContainer.localPosition, Vector3.zero, 5f * Time.deltaTime);
			}
			if (this.m_HeadContainer.localRotation != Quaternion.identity)
			{
				this.m_HeadContainer.localRotation = Quaternion.Slerp(this.m_HeadContainer.localRotation, Quaternion.identity, 5f * Time.deltaTime);
			}
		}
	}

	private void CheckFloorMaterial()
	{
		Vector3 start;
		start = base.transform.position + Vector3.up * this.CharacterController.height;
		Vector3 vector;
		vector = base.transform.position + Vector3.down;
		if (Physics.Linecast(start, vector, out var hitInfo, ~(int)this.m_FloorMaterialIgnoreLayers, QueryTriggerInteraction.Collide))
		{
			Debug.DrawLine(start, hitInfo.point, Color.green);
			FloorMaterial component;
			component = hitInfo.transform.GetComponent<FloorMaterial>();
			if (this.S13CheckIfSecondCastRequired(component))
			{
				this.S13StepInInkPuddle();
				if (Physics.Linecast(end: vector + Vector3.down * 0.1f, start: hitInfo.point, hitInfo: out var hitInfo2, layerMask: ~(int)this.m_FloorMaterialIgnoreLayers, queryTriggerInteraction: QueryTriggerInteraction.Collide))
				{
					component = hitInfo2.transform.GetComponent<FloorMaterial>();
				}
			}
			this.S13SetFloorMaterial(component);
		}
		else
		{
			Debug.DrawLine(start, vector, Color.red);
		}
	}

	public void UnlockPhotoMode()
	{
	}

	public void ShowFirstPersonArms()
	{
		if (base.Abilities == null)
		{
			this.ModelLayers.DisableAbility();
		}
		if (this.m_WeaponState == null)
		{
			this.ModelLayers.DisableWeapon();
		}
		else
		{
			this.ShowWeapon();
		}
	}

	public void HideFirstPersonArms()
	{
		this.HideAbility();
		this.HideWeapon();
	}

	public void ShowAbility(Action onComplete = null)
	{
		if (base.Abilities == null)
		{
			return;
		}
		this.GameCamera.SetFirstPersonArmsActive(active: true);
		this.m_AbilityTracker.SetActive(active: true);
		if (this.m_AbilitySequence != null)
		{
			this.m_AbilitySequence.Kill();
			this.m_AbilitySequence = null;
		}
		this.m_AbilitySequence = DOTween.Sequence();
		float animatorLayer;
		animatorLayer = this.AnimatorBody.GetLayerWeight(1);
		this.m_AbilitySequence.Insert(0.01f, DOTween.To(() => animatorLayer, (float x) =>
		{
			this.AnimatorBody.SetLayerWeight(1, x);
		}, 1f, 0.5f).SetEase(Ease.Linear));
		if (this.AnimatorArms != null)
		{
			float armAnimatorLayer;
			armAnimatorLayer = this.AnimatorArms.GetLayerWeight(1);
			this.m_AbilitySequence.Insert(0.01f, DOTween.To(() => armAnimatorLayer, (float x) =>
			{
				this.AnimatorArms.SetLayerWeight(1, x);
			}, 1f, 0.5f).SetEase(Ease.Linear));
		}
		this.m_AbilitySequence.OnComplete(() =>
		{
			onComplete?.Invoke();
		});
		this.ModelLayers.EnableAbility();
	}

	public void ShowAbilityInstant(Action onComplete = null)
	{
		if (base.Abilities != null)
		{
			this.GameCamera.SetFirstPersonArmsActive(active: true);
			this.AnimatorBody.SetLayerWeight(1, 1f);
			if (this.AnimatorArms != null)
			{
				this.AnimatorArms.SetLayerWeight(1, 1f);
			}
			onComplete?.Invoke();
			this.ModelLayers.EnableAbility();
		}
	}

	public void HideAbility(Action onComplete = null)
	{
		if (this.m_AbilitySequence != null)
		{
			this.m_AbilitySequence.Kill();
			this.m_AbilitySequence = null;
		}
		this.m_AbilitySequence = DOTween.Sequence();
		if (this.AnimatorBody != null)
		{
			float animatorLayer;
			animatorLayer = this.AnimatorBody.GetLayerWeight(1);
			this.m_AbilitySequence.Insert(0f, DOTween.To(() => animatorLayer, (float x) =>
			{
				this.AnimatorBody.SetLayerWeight(1, x);
			}, 0f, 0.5f).SetEase(Ease.Linear));
		}
		if (this.AnimatorArms != null)
		{
			float armAnimatorLayer;
			armAnimatorLayer = this.AnimatorArms.GetLayerWeight(1);
			this.m_AbilitySequence.Insert(0f, DOTween.To(() => armAnimatorLayer, (float x) =>
			{
				this.AnimatorArms.SetLayerWeight(1, x);
			}, 0f, 0.5f).SetEase(Ease.Linear));
		}
		this.m_AbilitySequence.OnComplete(() =>
		{
			this.ModelLayers.DisableAbility();
			onComplete?.Invoke();
		});
		this.m_AbilityTracker.SetActive(active: false);
	}

	public void SetNewWeapon(WeaponData weaponData)
	{
		this.DestroyWeapon();
		base.SetWeapon(new PlayerWeaponGentPipe(this, weaponData));
		this.m_WeaponState = (PlayerWeaponGentPipe)base.CurrentWeapon;
		GentPipePowerIndicator[] componentsInChildren;
		componentsInChildren = base.gameObject.GetComponentsInChildren<GentPipePowerIndicator>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].SetPowerLevel(GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.Power);
		}
		MeshRenderer[] componentsInChildren2;
		componentsInChildren2 = this.m_WeaponState.Weapon.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		this.ActiveWeaponMirrorDuplicate = this.DuplicateWeapon(componentsInChildren2, "Mirror", shadowsOnly: false);
		this.ActiveWeaponDuplicate = this.DuplicateWeapon(componentsInChildren2, "Player", shadowsOnly: true);
		this.m_WeaponState.SwapShaderToFirstPerson();
		GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetWeapon(this.m_WeaponState.Weapon.Data.WeaponType);
	}

	public void EquipWeapon(WeaponData weaponData)
	{
		if (weaponData.WeaponType != WeaponType.NONE)
		{
			this.SetNewWeapon(weaponData);
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetWeapon(weaponData.WeaponType);
			GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.SetStatus(WeaponStatus.Active);
			this.ShowWeapon();
		}
	}

	public void ShowWeapon()
	{
		if (this.m_WeaponState == null || GameManager.Instance.GameData.CurrentSave.PlayerData.WeaponData.WeaponStatus != WeaponStatus.Active)
		{
			return;
		}
		this.m_WeaponState.SetFirstPersonFlag(active: true);
		this.GameCamera.SetFirstPersonArmsActive(active: true);
		this.m_BodyWeaponParent.gameObject.SetActive(value: true);
		this.m_WeaponTracker.SetActive(active: true);
		this.AnimatorBody.SetInteger("HandWeaponStatus", 1);
		if (this.AnimatorArms != null)
		{
			this.AnimatorArms.SetInteger("HandWeaponStatus", 1);
		}
		this.ModelLayers.EnableWeapon();
		if (this.m_WeaponSequence != null)
		{
			this.m_WeaponSequence.Kill();
			this.m_WeaponSequence = null;
		}
		this.m_WeaponSequence = DOTween.Sequence();
		float animatorLayer;
		animatorLayer = this.AnimatorBody.GetLayerWeight(2);
		this.m_WeaponSequence.Insert(0.01f, DOTween.To(() => animatorLayer, (float x) =>
		{
			this.AnimatorBody.SetLayerWeight(2, x);
		}, 1f, 0.5f).SetEase(Ease.Linear));
		if (this.AnimatorArms != null)
		{
			float armAnimatorLayer;
			armAnimatorLayer = this.AnimatorArms.GetLayerWeight(2);
			this.m_WeaponSequence.Insert(0.01f, DOTween.To(() => armAnimatorLayer, (float x) =>
			{
				this.AnimatorArms.SetLayerWeight(2, x);
			}, 1f, 0.5f).SetEase(Ease.Linear));
		}
		this.m_WeaponSequence.OnComplete(this.m_WeaponState.Enable);
	}

	private void DestroyWeapon()
	{
		if (this.m_WeaponState != null)
		{
			this.m_WeaponState.Dispose();
			this.m_WeaponState = null;
		}
		base.ClearWeapon();
		global::UnityEngine.Object.Destroy(this.ActiveWeaponDuplicate);
		global::UnityEngine.Object.Destroy(this.ActiveWeaponMirrorDuplicate);
	}

	public void RemoveWeapon()
	{
		this.HideWeapon();
		if (this.m_WeaponState != null)
		{
			this.m_WeaponState.Dispose();
			this.m_WeaponState = null;
		}
		base.ClearWeapon();
		global::UnityEngine.Object.Destroy(this.ActiveWeaponDuplicate, 0.5f);
		global::UnityEngine.Object.Destroy(this.ActiveWeaponMirrorDuplicate, 0.5f);
	}

	public void HideWeapon()
	{
		if (this.m_WeaponState != null)
		{
			this.m_WeaponState.SetFirstPersonFlag(active: false);
		}
		if (this.m_WeaponSequence != null)
		{
			this.m_WeaponSequence.Kill();
			this.m_WeaponSequence = null;
		}
		this.m_WeaponSequence = DOTween.Sequence();
		if (this.AnimatorBody != null)
		{
			float animatorLayer;
			animatorLayer = this.AnimatorBody.GetLayerWeight(2);
			this.m_WeaponSequence.Insert(0f, DOTween.To(() => animatorLayer, (float x) =>
			{
				this.AnimatorBody.SetLayerWeight(2, x);
			}, 0f, 0.3f).SetEase(Ease.Linear));
		}
		if (this.AnimatorArms != null)
		{
			float armAnimatorLayer;
			armAnimatorLayer = this.AnimatorArms.GetLayerWeight(2);
			this.m_WeaponSequence.Insert(0f, DOTween.To(() => armAnimatorLayer, (float x) =>
			{
				this.AnimatorArms.SetLayerWeight(2, x);
			}, 0f, 0.3f).SetEase(Ease.Linear));
		}
		this.m_WeaponSequence.OnComplete(() =>
		{
			this.ModelLayers.DisableWeapon();
			this.GameCamera.SetFirstPersonArmsActive(active: false);
			if (this.m_BodyWeaponParent != null)
			{
				this.m_BodyWeaponParent.gameObject.SetActive(value: false);
			}
		});
		if (this.m_WeaponTracker != null)
		{
			this.m_WeaponTracker.SetActive(active: false);
		}
		this.AnimatorBody.SetInteger("HandWeaponStatus", 0);
		if (this.AnimatorArms != null)
		{
			this.AnimatorArms.SetInteger("HandWeaponStatus", 0);
		}
	}

	private GameObject DuplicateWeapon(MeshRenderer[] weaponRenderers, string layer, bool shadowsOnly)
	{
		GameObject gameObject;
		gameObject = new GameObject("DuplicateWeapon");
		gameObject.transform.SetParent(this.m_WeaponParent);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		GentPipePowerIndicator gentPipePowerIndicator;
		gentPipePowerIndicator = new GameObject("Weapon_GentPipe_PowerIndicator").AddComponent<GentPipePowerIndicator>();
		gentPipePowerIndicator.transform.SetParent(gameObject.transform);
		gentPipePowerIndicator.transform.localPosition = Vector3.zero;
		List<GameObject> list;
		list = new List<GameObject>();
		GentPipeChargeEffects gentPipeChargeEffects;
		gentPipeChargeEffects = new GameObject("ChargeEffects").AddComponent<GentPipeChargeEffects>();
		gentPipeChargeEffects.transform.SetParent(gameObject.transform);
		gentPipeChargeEffects.transform.localPosition = Vector3.zero;
		gentPipeChargeEffects.transform.localEulerAngles = Vector3.zero;
		gentPipeChargeEffects.transform.localScale = Vector3.one;
		GameObject gameObject2;
		gameObject2 = new GameObject("ChargeLevel");
		gameObject2.transform.SetParent(gentPipeChargeEffects.transform);
		gameObject2.transform.localPosition = new Vector3(-0.409f, 0.6395195f, -0.1221f);
		gameObject2.transform.localEulerAngles = Vector3.zero;
		gameObject2.transform.localScale = Vector3.one;
		foreach (MeshRenderer meshRenderer in weaponRenderers)
		{
			GameObject gameObject3;
			gameObject3 = global::UnityEngine.Object.Instantiate(meshRenderer.gameObject);
			foreach (Transform item in gameObject3.transform)
			{
				global::UnityEngine.Object.Destroy(item.gameObject);
			}
			gameObject3.name = meshRenderer.gameObject.name;
			gameObject3.transform.SetParent(gameObject.transform);
			gameObject3.transform.position = meshRenderer.transform.position;
			gameObject3.transform.eulerAngles = meshRenderer.transform.eulerAngles;
			gameObject3.layer = LayerMask.NameToLayer(layer);
			if (shadowsOnly)
			{
				gameObject3.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			}
			if (meshRenderer.gameObject.name.ToLower().Contains("powerlvl"))
			{
				list.Add(gameObject3);
			}
			else if (meshRenderer.gameObject.name.ToLower().Contains("powerindicator"))
			{
				gameObject3.transform.SetParent(gentPipePowerIndicator.transform);
			}
			else if (meshRenderer.gameObject.name.ToLower().Contains("filler"))
			{
				gameObject3.transform.SetParent(gameObject2.transform);
			}
		}
		gentPipePowerIndicator.Initialize(list.ToArray());
		for (int j = 0; j < list.Count; j = unchecked(j + 1))
		{
			list[j].transform.SetParent(gentPipePowerIndicator.transform);
		}
		gentPipeChargeEffects.SetChargeLevel(gameObject2.transform);
		gameObject.transform.SetParent(this.m_BodyWeaponParent);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = Vector3.zero;
		return gameObject;
	}

	private void S13SetFloorMaterial(FloorMaterial floorMaterial)
	{
		if (floorMaterial != null)
		{
			this.CurrentFloorType = floorMaterial.FloorType;
			if (this.PlayerAudioController != null)
			{
				this.PlayerAudioController.SetFloorMaterial(this.CurrentFloorType);
			}
		}
	}

	private bool S13CheckIfSecondCastRequired(FloorMaterial floorMaterial)
	{
		if (floorMaterial != null)
		{
			return floorMaterial.FloorType == BATDRPlayerAudioController.FloorMaterials.InkPuddle;
		}
		return false;
	}

	private void S13StepInInkPuddle()
	{
		if (this.PlayerAudioController != null)
		{
			this.PlayerAudioController.StepInInkPuddle();
		}
	}

	public void S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction reaction)
	{
		if (this.PlayerAudioController != null)
		{
			this.PlayerAudioController.React(reaction);
		}
	}

	public void S13SetCartMove(bool isMoving)
	{
		if (this.PlayerAudioController != null)
		{
			this.PlayerAudioController.SetCartMove(isMoving);
		}
	}

	public void S13SetClimbing(bool isClimbing)
	{
		if (this.PlayerAudioController != null)
		{
			this.PlayerAudioController.SetClimbing(isClimbing);
		}
	}

	public void S13SetHiding(bool isHiding)
	{
		if (this.PlayerAudioController != null)
		{
			this.PlayerAudioController.SetHiding(isHiding);
		}
	}

	public void S13SetCrouching(bool isCrouching)
	{
		if (this.PlayerAudioController != null)
		{
			this.PlayerAudioController.SetCrouching(isCrouching);
		}
	}

	protected override void OnDisposed()
	{
		this.OnAbilityUse = null;
		this.OnAnimationComplete = null;
		this.OnAnimationInteract = null;
		this.OnDeath = null;
		this.OnRespawn = null;
		this.OnAttackStart = null;
		this.OnForceDeath = null;
		this.m_CrouchSequence?.Kill();
		this.m_CrouchSequence = null;
		if (this.HandMaterial != null)
		{
			this.HandMaterial.SetTexture("_Emission", null);
			this.HandMaterial.SetFloat("_EmitPower", 0f);
		}
		if (this.test_inkdemon != null)
		{
			this.test_inkdemon.Dispose();
		}
		GameManager.Instance.Player = null;
		this.CharacterController = null;
		base.OnDisposed();
	}

	public void InkDemonTest()
	{
		if (this.__InkDemonActive)
		{
			return;
		}
		if (this.test_inkdemon != null)
		{
			this.test_inkdemon.Enable();
		}
		this.__InkDemonActive = true;
		GameManager.Instance.ShowNotificationText(TextUtility.GetKey("NOTIFICATION_INK_DEMON"), isInkDemon: true);
		this.TestShakeLoop();
		CameraEffects.InkDemon(10f);
		DOTween.Sequence().InsertCallback(10f, () =>
		{
			if ((bool)this.test_inkdemon)
			{
				this.test_inkdemon.SetActive(active: true);
			}
			this.__InkDemonActive = false;
		});
	}

	private void TestShakeLoop()
	{
		if (this.__InkDemonActive)
		{
			CameraEffects.ShakeRotation(1.5f, 0.5f, 10, 90f, fadeOut: false);
			DOTween.Sequence().InsertCallback(1.5f, this.TestShakeLoop);
		}
	}
}
