using System;
using DG.Tweening;
using UnityEngine;

public class DoorContent : ActionEventContent<DoorContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public DoorType DoorType;

		public float Speed = 1f;

		public Ease Ease = Ease.InOutSine;

		public bool SyncSpeed;

		public float Delay;
	}

	private Door m_Door;

	private OcclusionPortal m_Portal;

	[SerializeField]
	protected DoorFunctionType m_DoorFunctionType;

	[SerializeField]
	protected EventTrigger m_ProximityEventTrigger;

	public event EventHandler OnForceActivateCompleted;

	public event EventHandler OnForceDeactivateCompleted;

	protected override void OnInitialize()
	{
		this.m_Door = (Door)base.Connectable;
		this.m_Portal = base.GetComponent<OcclusionPortal>();
		if (this.m_Portal == null)
		{
			this.m_Portal = base.GetComponentInChildren<OcclusionPortal>(includeInactive: true);
		}
		if (this.m_Portal != null)
		{
			this.m_Portal.open = false;
		}
		if (this.m_DoorFunctionType == DoorFunctionType.Proximity && this.m_ProximityEventTrigger != null)
		{
			this.RemoveDoorListeners();
			this.m_ProximityEventTrigger.OnEnter += this.HandleProximityEventTriggerOnEnter;
			this.m_ProximityEventTrigger.OnExit += this.HandleProximityEventTriggerOnExit;
		}
	}

	protected override void OnActivate()
	{
		this.InternalActivation();
		GameManager.Instance.Player.Interaction.ResetInteraction();
	}

	private void InternalActivation(bool isAnimation = true)
	{
		if ((bool)this.m_Portal)
		{
			this.m_Portal.open = true;
		}
		if (isAnimation)
		{
			base.m_Sequencer.New();
		}
		for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_ActiveProperties[i];
			if (isAnimation)
			{
				this.CheckAnimation(properties);
			}
			else
			{
				this.CheckAnimationComplete(properties);
			}
		}
	}

	private void CheckAnimation(Properties properties)
	{
		if (properties.DoorType == DoorType.Single)
		{
			this.m_Door.Data.SetType(DoorType.Single);
		}
		else if (properties.DoorType == DoorType.Front)
		{
			this.m_Door.Data.SetType(DoorType.Front);
		}
		else if (properties.DoorType == DoorType.Back)
		{
			this.m_Door.Data.SetType(DoorType.Back);
		}
		else if (properties.DoorType == DoorType.Left)
		{
			this.m_Door.Data.SetType(DoorType.Left);
		}
		else if (properties.DoorType == DoorType.Right)
		{
			this.m_Door.Data.SetType(DoorType.Right);
		}
		properties.Container.SetParent(properties.Animator);
		base.DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, properties.Speed, properties.Ease, properties.Delay);
	}

	private void CheckAnimationComplete(Properties properties)
	{
		DoorType type;
		type = this.m_Door.Data.Type;
		if (type == DoorType.Single || this.m_Door.Content.IsConnected)
		{
			this.InternalActivationForceComplete(properties);
		}
		else if (type == DoorType.Front && properties.DoorType == DoorType.Front)
		{
			this.InternalActivationForceComplete(properties);
		}
		else if (type == DoorType.Back && properties.DoorType == DoorType.Back)
		{
			this.InternalActivationForceComplete(properties);
		}
		else if (type == DoorType.Left && properties.DoorType == DoorType.Left)
		{
			this.InternalActivationForceComplete(properties);
		}
		else if (type == DoorType.Right && properties.DoorType == DoorType.Right)
		{
			this.InternalActivationForceComplete(properties);
		}
	}

	private void InternalActivationForceComplete(Properties properties)
	{
		properties.Container.SetParent(properties.Animator);
		base.ForceAnimationComplete(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles);
	}

	protected override void OnForceActivateComplete()
	{
		if (base.IsActivated)
		{
			this.OnForceActivateCompleted.Send(this);
		}
		base.m_Sequencer?.Kill();
		base.RemoveListeners();
		base.GetActiveProperties();
		this.InternalActivation(isAnimation: false);
		this.ResetActionEvents();
		base.AddListeners();
		base.IsActivated = true;
	}

	public override void OnIsInactive()
	{
		this.ResetActionEvents();
	}

	protected override void OnDeactivate()
	{
		this.InternalDeactivation();
		GameManager.Instance.Player.Interaction.ResetInteraction();
	}

	protected override void OnForceDeactivateComplete()
	{
		if (!base.IsActivated)
		{
			this.OnForceDeactivateCompleted.Send(this);
		}
		base.m_Sequencer?.Kill();
		this.InternalDeactivation(isAnimation: false);
		this.ResetPivots();
		base.IsActivated = false;
	}

	private void InternalDeactivation(bool isAnimation = true)
	{
		if (isAnimation)
		{
			base.m_Sequencer.New();
		}
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (isAnimation)
			{
				base.DoAnimation(duration: properties.SyncSpeed ? properties.Speed : 0.4f, properties: properties, position: Vector3.zero, rotation: Vector3.zero, ease: Ease.InOutSine);
				continue;
			}
			base.ForceAnimationComplete(properties, Vector3.zero, Vector3.zero);
		}
	}

	protected override void OnDeactivateComplete()
	{
		if ((bool)this.m_Portal)
		{
			this.m_Portal.open = false;
		}
		this.ResetPivots();
	}

	private void ResetPivots()
	{
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			base.m_Properties[i].Animator.localEulerAngles = Vector3.zero;
		}
	}

	private void HandleProximityEventTriggerOnEnter(object sender, EventArgs e)
	{
		if (!base.IsDisabled && (!base.IsActivated || base.m_Sequencer == null || base.m_Sequencer.IsPlaying) && (this.m_Door.Data.Status == DoorStatus.Open || this.m_Door.Data.Status == DoorStatus.Closed))
		{
			this.ForceActivate();
		}
	}

	private void HandleProximityEventTriggerOnExit(object sender, EventArgs e)
	{
		if (!base.IsDisabled && (this.m_Door.Data.Status == DoorStatus.Open || this.m_Door.Data.Status == DoorStatus.Closed))
		{
			this.ForceDeactivate();
			this.Enable();
		}
	}

	private void RemoveDoorListeners()
	{
		if (this.m_DoorFunctionType == DoorFunctionType.Proximity && this.m_ProximityEventTrigger != null)
		{
			this.m_ProximityEventTrigger.OnEnter -= this.HandleProximityEventTriggerOnEnter;
			this.m_ProximityEventTrigger.OnExit -= this.HandleProximityEventTriggerOnExit;
		}
	}

	protected override void OnDisposed()
	{
		this.OnForceActivateCompleted = null;
		this.OnForceDeactivateCompleted = null;
		this.RemoveDoorListeners();
		this.m_Door = null;
		this.m_Portal = null;
		base.OnDisposed();
	}
}
