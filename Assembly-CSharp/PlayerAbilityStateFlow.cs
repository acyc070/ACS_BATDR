using System;
using DG.Tweening;
using JDS.PostProcessing;
using UnityEngine;

public class PlayerAbilityStateFlow : PlayerAbilityState
{
	private LayerMask m_TeleportLayers;

	private RaycastHit m_TargetHit;

	private RaycastHit m_TargetColliderToHit;

	private Vector3 m_TargetPosition;

	private Vector3 m_TargetPositionOffset;

	private Vector3 m_ToTargetPosition;

	private Vector3 m_ToTargetColliderPosition;

	private Sequence m_Sequence;

	private FlowMantle m_FlowMantle;

	private bool m_CanFlow;

	protected override float m_CooldownCount => UpgradeCheck.GetAbility();

	public PlayerAbilityStateFlow(Player player, State.PlayerAbility id)
		: base(player, id)
	{
	}

	protected override void InternalStateEnterOnComplete()
	{
		GameManager.Instance.ShowTeleport();
	}

	public override void InternalAbilityInitializer()
	{
		this.m_TeleportLayers = ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("AI")));
		base.m_AbilityPostProcessEffect = GameManager.Instance.GameCamera.BasePostProcess.profile.GetSetting<Teleport>();
		base.m_Sigil = GameManager.Instance.AssetManager.GetAsset<Texture2D>("Audrey_Sigil_Flow");
		base.m_AbilityUseDelay = 0.1f;
		base.m_AbilityDistanceMin = 3f;
		base.m_AbilityDistanceMax = 25f;
		base.m_Cooldown = this.m_CooldownCount;
	}

	protected override void CheckCooldown()
	{
		GameManager.Instance.ShowTeleportCooldown(base.m_Cooldown, this.m_CooldownCount);
	}

	protected override void CooldownComplete()
	{
		GameManager.Instance.ShowTeleportCooldownComplete();
	}

	protected override bool UseInput()
	{
		if (base.Actor.IsCrouchSequenceActive)
		{
			return false;
		}
		return PlayerInput.AttackSecondary();
	}

	protected override bool InternalCheckUse()
	{
		bool canUse;
		canUse = base.CanUse;
		if (canUse)
		{
			base.m_HandMaterial.SetTexture("_Emission", base.m_Sigil);
		}
		return canUse;
	}

	protected override bool InternalUse()
	{
		base.Actor.ShowAbilityInstant();
		base.Actor.Ability();
		if (this.m_CanFlow)
		{
			base.Actor.SetHeadContainerSlerp(active: false);
			base.Actor.SetState(State.Player.Ability);
		}
		else
		{
			base.Actor.CancelMovement();
			base.Actor.PlayerMovement.StopRun();
		}
		GameManager.Instance.GameCamera.TeleportParticles.Play();
		GameManager.Instance.UseTeleportAbility(use: true);
		this.ResetSequence();
		float num;
		num = 0f;
		float num2;
		num2 = 0.25f;
		if (this.m_CanFlow)
		{
			this.m_Sequence.Insert(num, base.Actor.transform.DOLocalMove(this.m_ToTargetPosition, num2 * 2f).SetEase(Ease.InSine));
		}
		else
		{
			base.Actor.AddForce(base.Actor.GameCamera.transform.forward * 50f);
		}
		this.m_Sequence.Insert(num, base.DOPostProcess(1f, num2, Ease.Linear));
		this.m_Sequence.Insert(num + num2, base.DOPostProcess(0f, num2, Ease.Linear));
		this.m_Sequence.OnComplete(base.UseOnComplete);
		return true;
	}

	protected override void InternalUseOnComplete()
	{
		GameManager.Instance.GameCamera.TeleportParticles.Stop();
		if (this.m_FlowMantle != null)
		{
			base.Actor.SetHeadContainerSlerp(active: true);
			this.m_FlowMantle.InteractableAnimation.OnInteractionComplete -= this.HandleFlowMantleOnInteractionComplete;
			this.m_FlowMantle.InteractableAnimation.OnInteractionComplete += this.HandleFlowMantleOnInteractionComplete;
			this.m_FlowMantle.Enter();
		}
		else if (this.m_CanFlow)
		{
			base.Actor.SetHeadContainerSlerp(active: true);
			base.Actor.ResetRotation();
			base.Actor.SetState(State.Player.Default);
		}
		else
		{
			base.Actor.CancelMovement();
		}
	}

	private void HandleFlowMantleOnInteractionComplete(object sender, EventArgs e)
	{
		this.m_FlowMantle.InteractableAnimation.OnInteractionComplete -= this.HandleFlowMantleOnInteractionComplete;
		this.m_FlowMantle = null;
		base.Actor.ResetRotation();
		base.Actor.SetState(State.Player.Default);
	}

	protected override void InternalUpdate()
	{
	}

	protected override void InternalFixedUpdate()
	{
		Vector3 vector;
		vector = base.Actor.HeadContainer.position - base.Actor.HeadContainer.forward;
		Vector3 forward;
		forward = base.Actor.HeadContainer.forward;
		Vector3 hitPoint;
		hitPoint = vector + forward * base.m_AbilityDistanceMax;
		if (Physics.SphereCast(vector, 0.2f, forward, out this.m_TargetHit, base.m_AbilityDistanceMax, this.m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			this.m_FlowMantle = this.m_TargetHit.transform.GetComponent<FlowMantle>();
			hitPoint = this.m_TargetHit.point;
		}
		if (this.m_FlowMantle != null)
		{
			this.m_ToTargetPosition = this.m_FlowMantle.StartPosition;
			this.m_CanFlow = true;
			base.BoostAbilityEmission();
		}
		else if (this.CheckTarget(vector, hitPoint))
		{
			this.m_CanFlow = true;
			base.BoostAbilityEmission();
		}
		else
		{
			this.m_CanFlow = false;
			base.BoostAbilityEmission();
		}
	}

	protected override void ClearCheck()
	{
		if (base.CanUse)
		{
			base.CanUse = false;
		}
	}

	private bool CheckTarget(Vector3 origin, Vector3 hitPoint)
	{
		float height;
		height = base.Actor.CharacterController.height;
		float radius;
		radius = base.Actor.CharacterController.radius;
		this.m_TargetPosition = hitPoint;
		Vector3 vector;
		vector = ((this.m_TargetPosition.y > origin.y + radius) ? Vector3.down : Vector3.up);
		Vector3 vector2;
		vector2 = origin - this.m_TargetPosition;
		this.m_TargetPositionOffset = this.m_TargetPosition + (vector2 / vector2.magnitude).normalized * (radius * 2f);
		this.m_TargetPositionOffset.y = this.m_TargetPosition.y;
		this.m_TargetPositionOffset += vector * radius * 1.1f;
		float maxDistance;
		maxDistance = height;
		if (vector.y < 0f)
		{
			this.m_TargetPositionOffset += vector * radius;
			maxDistance = radius * 1.5f;
		}
		if (Physics.SphereCast(this.m_TargetPositionOffset, radius, -vector, out this.m_TargetColliderToHit, maxDistance, this.m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			this.m_ToTargetColliderPosition = this.m_TargetColliderToHit.point;
			if (vector.y < 0f)
			{
				this.m_ToTargetColliderPosition.y -= height + radius / 2f;
			}
			this.m_ToTargetPosition = this.m_ToTargetColliderPosition;
		}
		else
		{
			this.m_ToTargetPosition = (this.m_ToTargetColliderPosition = ((vector.y < 0f) ? (this.m_TargetPositionOffset + vector * (height - radius * 2f)) : (this.m_TargetPositionOffset - vector * (height + radius))));
		}
		if (Physics.SphereCast(this.m_ToTargetPosition + Vector3.up, radius, Vector3.up * height, out var _, height - 1.1f - radius, this.m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			this.m_ToTargetPosition = base.Actor.transform.position;
		}
		if (this.m_ToTargetPosition == Vector3.zero || this.m_ToTargetPosition == base.Actor.transform.position)
		{
			return false;
		}
		Vector3 toTargetPosition;
		toTargetPosition = this.m_ToTargetPosition;
		bool flag;
		flag = Vector3.Distance(base.Actor.transform.position, toTargetPosition) > base.m_AbilityDistanceMin;
		if (!base.CanUse)
		{
			base.CanUse = true;
		}
		if (flag)
		{
			Vector3 vector3;
			vector3 = toTargetPosition + Vector3.up * height;
			if (Physics.Linecast(toTargetPosition, vector3, this.m_TeleportLayers, QueryTriggerInteraction.Ignore))
			{
				flag = false;
			}
			if (Physics.Linecast(vector3, toTargetPosition + Vector3.up * 0.01f, this.m_TeleportLayers, QueryTriggerInteraction.Ignore))
			{
				flag = false;
			}
		}
		return flag;
	}

	protected override void InternalLateUpdate()
	{
	}

	protected override void InternalResetOnComplete()
	{
		this.m_TargetHit = default;
		this.m_TargetPosition = Vector3.zero;
		this.m_TargetPositionOffset = Vector3.zero;
		this.m_ToTargetPosition = base.Actor.transform.position;
	}

	protected override void InternalOnStateExit()
	{
	}

	protected override void InternalForceCancel()
	{
		this.KillSequence();
		GameManager.Instance.GameCamera.TeleportParticles.Stop();
		base.Actor.SetHeadContainerSlerp(active: true);
		base.Actor.ResetRotation();
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
		this.m_Sequence.SetUpdate(UpdateType.Fixed);
	}

	private void KillSequence()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.KillSequence();
		base.OnDisposed();
	}
}
