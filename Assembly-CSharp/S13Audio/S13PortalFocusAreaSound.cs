using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

public class S13PortalFocusAreaSound : MonoBehaviour, IS13AreaSound
{
	[Tooltip("The transform that should be moved toward the target.")]
	public Transform follower;

	[Tooltip("Transform whose hierarchy contains the set of portal triggers. Optional, and used only when pressing the 'Get Portal Triggers' button.")]
	[SerializeField]
	private Transform portalContainer;

	[Tooltip("Transform that represents where the follower should go if there are no open portals. Leaving null will default to the center of the first collider.")]
	[SerializeField]
	private Transform portalFallbackPosition;

	[Tooltip("Defines the set of colliders wherein the follower may venture when the target position is outside the area.")]
	public List<Collider> portalTriggers = new List<Collider>();

	[Tooltip("Defines the set of colliders wherein the follower may venture.")]
	public List<Collider> triggers = new List<Collider>();

	private Vector3 _targetPosition = Vector3.zero;

	private bool _targetWithinTrigger;

	private bool _targetWithinPortal;

	public Vector3 targetPosition
	{
		get
		{
			return this._targetPosition;
		}
		set
		{
			this._targetPosition = value;
			this.UpdateBoundedPosition();
		}
	}

	public bool targetWithinTrigger
	{
		get
		{
			return this._targetWithinTrigger;
		}
		set
		{
			this._targetWithinTrigger = value;
		}
	}

	public event Action onAreaEnter;

	public event Action onAreaExit;

	public void GetContainedTriggers()
	{
		this.triggers = (from x in base.GetComponentsInChildren<Collider>()
			where x.isTrigger
			select x).ToList();
	}

	public void GetPortalTriggers()
	{
		this.portalTriggers = (from x in this.portalContainer.GetComponentsInChildren<Collider>()
			where x.isTrigger
			select x).ToList();
	}

	private void UpdateBoundedPosition()
	{
		if (!this.follower || this.triggers == null || this.triggers.Count <= 0)
		{
			return;
		}
		float num;
		num = float.PositiveInfinity;
		Vector3 position;
		position = Vector3.positiveInfinity;
		foreach (Collider trigger in this.triggers)
		{
			Vector3 vector;
			vector = trigger.ClosestPoint(this.targetPosition);
			float num2;
			num2 = Vector3.Distance(this.targetPosition, vector);
			bool flag;
			flag = this.portalTriggers.Contains(trigger);
			if (flag && trigger.TryGetComponent<S13ClosablePortal>(out var component) && !component.IsOpen())
			{
				continue;
			}
			if (num2 <= float.Epsilon)
			{
				if (!this.targetWithinTrigger)
				{
					S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " " + (flag ? "Portal " : " ") + " Entered", base.gameObject, S13Debug.LogType.AreaSound);
					if (S13Settings.Debug_AreaSoundUI)
					{
						S13Debug.DisplayCanvasText("Area_" + base.transform.parent.name, "");
					}
					this.onAreaEnter?.Invoke();
				}
				this.targetWithinTrigger = true;
				this._targetWithinPortal = flag;
				this.follower.position = this.targetPosition;
				return;
			}
			if (flag && num2 < num)
			{
				num = num2;
				position = vector;
			}
		}
		if (float.IsPositiveInfinity(position.x))
		{
			position = (this.portalFallbackPosition ? this.portalFallbackPosition.position : this.triggers.First().bounds.center);
		}
		this.follower.position = position;
		if (this.targetWithinTrigger)
		{
			S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " " + (this._targetWithinPortal ? "Portal " : " ") + " Exited", base.gameObject, S13Debug.LogType.AreaSound);
			if (S13Settings.Debug_AreaSoundUI)
			{
				S13Debug.HideCanvasText("Area_" + base.transform.parent.name);
			}
			this.onAreaExit?.Invoke();
			this._targetWithinPortal = false;
			this.targetWithinTrigger = false;
		}
	}
}
