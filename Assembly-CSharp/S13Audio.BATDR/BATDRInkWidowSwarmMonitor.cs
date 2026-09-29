using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio.BATDR;

public class BATDRInkWidowSwarmMonitor : MonoBehaviour
{
	private enum SwarmSize
	{
		None = 0,
		Small = 1,
		Large = 2
	}

	public static HashSet<BATRDRInkWidowSwarmEntry> swarmList;

	[Header("Swarm Position Tracking")]
	[SerializeField]
	private GameObject swarmPositionFollower;

	[SerializeField]
	[Tooltip("Max movement the position follower can move in an update call, used to prevent snapping around")]
	private float swarmPositionMovementDelta = 1f;

	[SerializeField]
	[Tooltip("Max distance an ink widow can be from swarm monitor to be included in the average position calculation")]
	private float maxWidowDistance = 30f;

	[Header("Emitter Follower Settings")]
	[SerializeField]
	private GameObject emitterFollower;

	[SerializeField]
	private float emitterDistance = 15f;

	[Header("Parameter Settings")]
	[SerializeField]
	[S13LockableField]
	[FormerlySerializedAs("paramName")]
	private string accessorID = string.Empty;

	[SerializeField]
	private int maxSwarmSize = 15;

	[Header("Small Swarm")]
	[SerializeField]
	private int minSmallSwarmSize = 5;

	[SerializeField]
	private S13LocalAction onSmallSwarmAction;

	[SerializeField]
	private S13LocalAction onDropBelowSmallAction;

	[Header("Large Swarm")]
	[SerializeField]
	private int minLargeSwarmSize = 10;

	[SerializeField]
	private S13LocalAction onLargeSwarmAction;

	[SerializeField]
	private S13LocalAction onDropBelowLargeAction;

	[Header("SwarmSize")]
	[SerializeField]
	[ReadOnly]
	private int swarmCount;

	[SerializeField]
	[ReadOnly]
	private SwarmSize currentSwarmSize;

	private Transform _listenerTransform;

	private Transform ListenerTransform
	{
		get
		{
			if (!this._listenerTransform)
			{
				this._listenerTransform = global::UnityEngine.Object.FindObjectOfType<AudioListener>().transform;
			}
			return this._listenerTransform;
		}
	}

	private void Start()
	{
		if (this.swarmPositionFollower == null || this.emitterFollower == null)
		{
			S13Debug.LogError("BATDRInkWidowSwarmMonitor does not have its followers assigned, disabling");
			base.enabled = false;
		}
	}

	private void Update()
	{
		this.swarmCount = BATDRInkWidowSwarmMonitor.swarmList.Count;
		if (this.swarmCount > 0)
		{
			this.PositionSwarmFollower();
		}
		this.PositionEmitter();
		this.CheckForSwarmResize();
		if (!string.IsNullOrEmpty(this.accessorID))
		{
			S13Manager.SetAccessor(this.accessorID, (float)this.swarmCount / (float)this.maxSwarmSize);
		}
	}

	private void PositionSwarmFollower()
	{
		Vector3 zero;
		zero = Vector3.zero;
		int num;
		num = 0;
		foreach (BATRDRInkWidowSwarmEntry swarm in BATDRInkWidowSwarmMonitor.swarmList)
		{
			if (!(Vector3.Distance(swarm.transform.position, this.swarmPositionFollower.transform.position) > this.maxWidowDistance))
			{
				zero += swarm.transform.position;
				num = unchecked(num + 1);
			}
		}
		if (num != 0 && (bool)this.swarmPositionFollower)
		{
			Vector3 target;
			target = zero / num;
			this.swarmPositionFollower.transform.position = Vector3.MoveTowards(this.swarmPositionFollower.transform.position, target, this.swarmPositionMovementDelta);
		}
	}

	private void PositionEmitter()
	{
		if ((bool)this.emitterFollower)
		{
			Vector3 vector;
			vector = this.swarmPositionFollower.transform.position - this.ListenerTransform.position;
			vector.y = 0f;
			if (vector.magnitude < this.emitterDistance)
			{
				this.emitterFollower.transform.position = this.ListenerTransform.position + vector;
				return;
			}
			vector.Normalize();
			this.emitterFollower.transform.position = this.ListenerTransform.position + vector * this.emitterDistance;
		}
	}

	private void CheckForSwarmResize()
	{
		switch (this.currentSwarmSize)
		{
		case SwarmSize.None:
			if (this.swarmCount >= this.minSmallSwarmSize)
			{
				this.onSmallSwarmAction.Execute();
				this.currentSwarmSize = SwarmSize.Small;
			}
			break;
		case SwarmSize.Small:
			if (this.swarmCount < this.minSmallSwarmSize)
			{
				this.onDropBelowSmallAction.Execute();
				this.currentSwarmSize = SwarmSize.None;
			}
			else if (this.swarmCount >= this.minLargeSwarmSize)
			{
				this.onLargeSwarmAction.Execute();
				this.currentSwarmSize = SwarmSize.Large;
			}
			break;
		case SwarmSize.Large:
			if (this.swarmCount < this.minLargeSwarmSize)
			{
				this.onDropBelowLargeAction.Execute();
				this.currentSwarmSize = SwarmSize.Small;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public static void RegisterInkWidow(BATRDRInkWidowSwarmEntry entry)
	{
		BATDRInkWidowSwarmMonitor.swarmList.Add(entry);
	}

	public static void DeRegisterInkWidow(BATRDRInkWidowSwarmEntry entry)
	{
		BATDRInkWidowSwarmMonitor.swarmList.Remove(entry);
	}

	static BATDRInkWidowSwarmMonitor()
	{
		BATDRInkWidowSwarmMonitor.swarmList = new HashSet<BATRDRInkWidowSwarmEntry>();
	}
}
