using UnityEngine;

public class Interactable : ActionEvent
{
	[SerializeField]
	protected PlayerInputType m_PlayerInputType = PlayerInputType.InteractOnReleased;

	[SerializeField]
	private bool m_ResetOnInteract;

	[SerializeField]
	protected ControllerRumble.RUMBLE_PRESETS m_presetRumble = ControllerRumble.RUMBLE_PRESETS.NONE;

	[SerializeField]
	protected Vector2 m_CustomRumble;

	public PlayerInputType PlayerInputType => this.m_PlayerInputType;

	public ControllerRumble.RUMBLE_PRESETS PresetRumble => this.m_presetRumble;

	public Vector2 CustomRumble => this.m_CustomRumble;

	public bool Enter(Vector3 origin, RaycastHit hit, object sender = null)
	{
		if (!base.m_IsActive || base.IsActioned || !this.InternalEnterCheck(origin, hit, sender))
		{
			return false;
		}
		base.ActivateAction(base.IsActioned, () =>
		{
			this.OnInternalEnter(origin, hit, sender);
		}, () =>
		{
			base.SendOnEnter(this);
		});
		return true;
	}

	public bool EnterCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return this.InternalEnterCheck(origin, hit, sender);
	}

	protected virtual bool InternalEnterCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return true;
	}

	protected virtual void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null)
	{
	}

	public bool Exit(Vector3 origin, RaycastHit hit, object sender = null)
	{
		if (!base.m_IsActive || base.IsActioned || !this.InternalExitCheck(origin, hit, sender))
		{
			return false;
		}
		base.ActivateAction(base.IsActioned, () =>
		{
			this.OnInternalExit(origin, hit, sender);
		}, () =>
		{
			base.SendOnExit(this);
		});
		return true;
	}

	protected virtual bool InternalExitCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return true;
	}

	protected virtual void OnInternalExit(Vector3 origin, RaycastHit hit, object sender = null)
	{
	}

	public bool Interact(Vector3 origin, RaycastHit hit, object sender = null)
	{
		if (!base.m_IsActive || base.IsActioned || !this.InternalInteractCheck(origin, hit, sender))
		{
			return false;
		}
		base.ActivateAction(true, () =>
		{
			this.OnInternalInteract(origin, hit, sender);
		}, () =>
		{
			base.SendOnInteract(this);
		});
		if (base.m_IsSingleAction)
		{
			base.Clear();
		}
		else if (this.m_ResetOnInteract)
		{
			base.ResetAction();
		}
		if (this.m_presetRumble != ControllerRumble.RUMBLE_PRESETS.NONE)
		{
			GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[unchecked((int)this.m_presetRumble)]);
		}
		else
		{
			GameManager.Instance.TriggerRumble(this.m_CustomRumble);
		}
		return true;
	}

	protected virtual bool InternalInteractCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return true;
	}

	protected virtual void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
	}
}
