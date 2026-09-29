using System;
using UnityEngine;

[Serializable]
public class PlayerInteraction : JDisposable
{
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private LayerMask m_IgnoreLayers;

	[SerializeField]
	private float m_LookDistance = 7f;

	[SerializeField]
	private float m_SphereCastThickness = 0.25f;

	[SerializeField]
	private bool DebugLines = true;

	public bool IsActive => this.m_IsActive;

	public RaycastHit Hit { get; private set; }

	public Interactable Interactable { get; private set; }

	public void ResetInteraction()
	{
		if (this.Interactable != null)
		{
			this.ExitInteraction(this.Interactable.transform.position);
		}
		this.Interactable = null;
	}

	public void Update(Transform sender)
	{
		if (this.m_IsActive && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			this.InternalUpdate(sender.position, sender.forward, this.m_LookDistance);
		}
	}

	private void InternalUpdate(Vector3 origin, Vector3 direction, float distance)
	{
		if (Physics.SphereCast(origin, this.m_SphereCastThickness, direction, out var hitInfo, distance, ~(int)this.m_IgnoreLayers))
		{
			this.Hit = hitInfo;
			Interactable component;
			component = this.Hit.transform.root.GetComponent<Interactable>();
			Interactable interactable;
			interactable = this.Hit.transform.GetComponent<Interactable>();
			if (interactable == null)
			{
				interactable = ((!(component == null)) ? component : this.Hit.transform.GetComponentInParent<Interactable>());
			}
			if (interactable != null)
			{
				this.DrawDebugLine(origin, this.Hit.point, Color.yellow);
				if (this.Interactable != interactable)
				{
					this.ExitInteraction(origin);
					this.Interactable = interactable;
					this.EnterInteraction(origin);
				}
				else if ((this.Interactable.PlayerInputType == PlayerInputType.InteractOnPressed || this.Interactable.PlayerInputType == PlayerInputType.InteractOnReleased) && PlayerInput.InteractOnReleased())
				{
					this.Interact(origin);
				}
				else if (this.Interactable.PlayerInputType == PlayerInputType.SpecialAction && PlayerInput.SpecialAction())
				{
					this.Interact(origin);
				}
				else if (this.Interactable.PlayerInputType == PlayerInputType.SpecialActionReleased && PlayerInput.SpecialActionReleased())
				{
					this.Interact(origin);
				}
				else if (!interactable.EnterCheck(origin, this.Hit))
				{
					this.ExitInteraction(origin);
				}
				return;
			}
		}
		this.ExitInteraction(origin);
	}

	private void Interact(Vector3 origin)
	{
		this.Interactable?.Interact(origin, this.Hit, this);
	}

	private void EnterInteraction(Vector3 origin)
	{
		if (!(this.Interactable == null) && !this.Interactable.Enter(origin, this.Hit, this))
		{
			this.ExitInteraction(origin);
		}
	}

	private void ExitInteraction(Vector3 origin)
	{
		this.Interactable?.Exit(origin, this.Hit, this);
		this.Interactable = null;
	}

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	protected override void OnDisposed()
	{
		this.Interactable = null;
		base.OnDisposed();
	}

	private void DrawDebugLine(Vector3 start, Vector3 end, Color color)
	{
	}
}
