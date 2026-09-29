using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio;

public class S13AccessorFromDistanceTrigger : S13ConditionalTrigger
{
	public class AdvancedCollisionInfo : ConditionInfo
	{
		public Collider collider;

		public AdvancedCollisionInfo(string tag, string name, int layer, Collider collider)
			: base(tag, name, layer)
		{
			this.collider = collider;
		}
	}

	[FormerlySerializedAs("parameterControlID")]
	[SerializeField]
	[S13LockableField]
	private string accessorID = "";

	[Tooltip("The non moving point or emitter of the sound")]
	public Transform stationaryPoint;

	[Tooltip("The minimum distance to be considered 100% within the sound")]
	[SerializeField]
	private float closeRange = 0.1f;

	[Tooltip("The maximum distance to be considered within the sound")]
	[SerializeField]
	private float farRange = 10f;

	[SerializeField]
	private bool setMinValueOnExit = true;

	private Dictionary<Transform, Coroutine> trackedRoutines = new Dictionary<Transform, Coroutine>();

	protected override void Start()
	{
		base.Start();
		if (string.IsNullOrEmpty(this.accessorID))
		{
			S13Debug.LogWarning(" accessorID not set on " + this.ToString() + " this will result in further errors");
		}
		if (this.stationaryPoint == null)
		{
			this.stationaryPoint = base.transform;
		}
	}

	public override ConditionInfo GetColliderInfo(Collider collider)
	{
		return new AdvancedCollisionInfo(collider.tag, collider.name, collider.gameObject.layer, collider);
	}

	protected override void OnEnter(ConditionInfo colliderInfo)
	{
		if (colliderInfo is AdvancedCollisionInfo { collider: var collider } advancedCollisionInfo && !(collider?.transform == null))
		{
			Coroutine value;
			value = base.StartCoroutine(this.UpdateDistance(advancedCollisionInfo.collider.transform));
			this.trackedRoutines.Add(advancedCollisionInfo.collider.transform, value);
		}
	}

	protected override void OnExit(ConditionInfo colliderInfo)
	{
		if (colliderInfo is AdvancedCollisionInfo advancedCollisionInfo && !(advancedCollisionInfo.collider == null) && !(advancedCollisionInfo.collider.transform == null) && this.trackedRoutines.ContainsKey(advancedCollisionInfo.collider.transform))
		{
			base.StopCoroutine(this.trackedRoutines[advancedCollisionInfo.collider.transform]);
			this.trackedRoutines.Remove(advancedCollisionInfo.collider.transform);
			if (this.setMinValueOnExit)
			{
				S13Manager.SetAccessor(this.accessorID, 0f);
			}
		}
	}

	private IEnumerator UpdateDistance(Transform movingPoint)
	{
		while (true)
		{
			if (!base.enabled)
			{
				yield return null;
				continue;
			}
			if (movingPoint == null)
			{
				break;
			}
			float num;
			num = Vector3.Distance(this.stationaryPoint.position, movingPoint.position);
			if (num < this.farRange)
			{
				S13Manager.SetAccessor(value: 1f - Mathf.Clamp01((num - this.closeRange) / (this.farRange - this.closeRange)), accessorID: this.accessorID);
			}
			else
			{
				S13Manager.SetAccessor(this.accessorID, 0f);
			}
			yield return null;
		}
		if (this.trackedRoutines.ContainsKey(movingPoint))
		{
			this.trackedRoutines.Remove(movingPoint);
		}
	}
}
