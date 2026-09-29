using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRMultiPortalFollower : MonoBehaviour, IS13AreaSound
{
	private class PortalDataPoint
	{
		public Collider col;

		public Vector3 pos;

		public float dist;

		public bool isPortal;

		public S13ClosablePortal closePortal;
	}

	[Tooltip("The transform that should be moved toward the target.")]
	public Transform[] followers;

	[Tooltip("Transform whose hierarchy contains the set of portal triggers. Optional, and used only when pressing the 'Get Portal Triggers' button.")]
	[SerializeField]
	private Transform portalContainer;

	[Tooltip("Defines the set of colliders wherein the follower may venture when the target position is outside the area.")]
	public List<Collider> portalTriggers = new List<Collider>();

	[Tooltip("Defines the set of colliders wherein the follower may venture.")]
	public List<Collider> triggers = new List<Collider>();

	private Vector3 _targetPosition = Vector3.zero;

	private bool _targetWithinTrigger;

	private bool _targetWithinPortal;

	private List<PortalDataPoint> _triggerData = new List<PortalDataPoint>();

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

	private void Start()
	{
		this._triggerData = new List<PortalDataPoint>(this.triggers.Count);
		for (int i = 0; i < this.triggers.Count; i = unchecked(i + 1))
		{
			this._triggerData.Add(new PortalDataPoint
			{
				col = this.triggers[i],
				pos = Vector3.positiveInfinity,
				dist = float.MaxValue,
				isPortal = this.portalTriggers.Contains(this.triggers[i]),
				closePortal = this.triggers[i].GetComponent<S13ClosablePortal>()
			});
		}
	}

	private void HandleInTrigger(bool isPortal)
	{
		if (!this.targetWithinTrigger)
		{
			S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " " + (isPortal ? "Portal " : " ") + " Entered", base.gameObject, S13Debug.LogType.TriggerMessage);
			this.onAreaEnter?.Invoke();
		}
		this.targetWithinTrigger = true;
		this._targetWithinPortal = isPortal;
	}

	private void HandleOutTrigger()
	{
		if (this.targetWithinTrigger)
		{
			S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " " + (this._targetWithinPortal ? "Portal " : " ") + " Exited", base.gameObject, S13Debug.LogType.TriggerMessage);
			this.onAreaExit?.Invoke();
			this._targetWithinPortal = false;
			this.targetWithinTrigger = false;
		}
	}

	private void UpdateBoundedPosition()
	{
		if (this.followers.Length == 0 || this.triggers == null || this.triggers.Count <= 0 || this._triggerData == null || this._triggerData.Count <= 0)
		{
			return;
		}
		unchecked
		{
			for (int i = 0; i < this._triggerData.Count; i++)
			{
				if (this._triggerData[i].isPortal && (bool)this._triggerData[i].closePortal && !this._triggerData[i].closePortal.IsOpen())
				{
					this._triggerData[i].dist = float.MaxValue;
					continue;
				}
				this._triggerData[i].pos = this._triggerData[i].col.ClosestPoint(this.targetPosition);
				this._triggerData[i].dist = Vector3.Distance(this.targetPosition, this._triggerData[i].pos);
				if (this._triggerData[i].dist < float.Epsilon)
				{
					this.HandleInTrigger(this._triggerData[i].isPortal);
				}
			}
			this._triggerData.Sort((PortalDataPoint x, PortalDataPoint y) => x.dist.CompareTo(y.dist));
			if (float.IsPositiveInfinity(this._triggerData[0].pos.x))
			{
				_ = this.triggers.First().bounds.center;
			}
			this.HandleOutTrigger();
			for (int num = 0; num < this.followers.Length; num++)
			{
				this.followers[num].position = this._triggerData[num].pos;
			}
		}
	}
}
