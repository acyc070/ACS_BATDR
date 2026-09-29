using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

public abstract class S13CompoundTrigger<TInfo> : MonoBehaviour, IS13CompoundTrigger where TInfo : IEquatable<TInfo>
{
	private List<TInfo> newColliders = new List<TInfo>(8);

	private List<TInfo> existingColliders = new List<TInfo>(8);

	protected virtual void Start()
	{
		foreach (Collider item in from x in base.GetComponentsInChildren<Collider>(includeInactive: true)
			where x.isTrigger
			select x)
		{
			item.GetOrAddComponent<S13CompoundTriggerHelper>().AddOwner(this);
		}
	}

	protected virtual void FixedUpdate()
	{
		unchecked
		{
			for (int i = 0; i < this.newColliders.Count; i++)
			{
				TInfo val;
				val = this.newColliders[i];
				if (!this.existingColliders.Contains(val))
				{
					this.existingColliders.Add(val);
					this.OnCompoundTriggerEnter(val);
				}
			}
			for (int num = this.existingColliders.Count - 1; num >= 0; num--)
			{
				TInfo val;
				val = this.existingColliders[num];
				if (!this.newColliders.Contains(val))
				{
					this.OnCompoundTriggerExit(val);
					this.existingColliders.RemoveAt(num);
				}
			}
			this.newColliders.Clear();
		}
	}

	public abstract TInfo GetColliderInfo(Collider collider);

	protected virtual void OnCompoundTriggerEnter(TInfo colliderInfo)
	{
	}

	protected virtual void OnCompoundTriggerExit(TInfo colliderInfo)
	{
	}

	public void RecordTriggerStay(Collider collider)
	{
		TInfo colliderInfo;
		colliderInfo = this.GetColliderInfo(collider);
		if (!this.newColliders.Contains(colliderInfo))
		{
			this.newColliders.Add(colliderInfo);
		}
	}
}
