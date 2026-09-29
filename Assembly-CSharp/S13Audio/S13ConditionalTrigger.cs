using System;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public abstract class S13ConditionalTrigger : S13CompoundTrigger<S13ConditionalTrigger.ConditionInfo>, IS13CompoundTriggerCache
{
	[Serializable]
	public class Condition
	{
		public enum Type
		{
			None = 0,
			Name = 1,
			Tag = 2,
			Layer = 3,
			All = 4
		}

		public Type type;

		public string value;

		public bool Check(ConditionInfo info)
		{
			return this.type switch
			{
				Type.None => false, 
				Type.Name => info.name == this.value, 
				Type.Tag => info.tag == this.value, 
				Type.Layer => info.layer == LayerMask.NameToLayer(this.value), 
				Type.All => true, 
				_ => false, 
			};
		}
	}

	public class ConditionInfo : IEquatable<ConditionInfo>
	{
		public string name;

		public string tag;

		public int layer;

		public ConditionInfo(string tag, string name, int layer)
		{
			this.tag = tag;
			this.name = name;
			this.layer = layer;
		}

		public bool Equals(ConditionInfo other)
		{
			if (this.tag == other.tag && this.name == other.name)
			{
				return this.layer == other.layer;
			}
			return false;
		}
	}

	[SerializeField]
	private Condition condition = new Condition();

	private static Dictionary<Collider, ConditionInfo> __cache;

	public void ClearCache()
	{
		S13ConditionalTrigger.__cache.Clear();
	}

	public override ConditionInfo GetColliderInfo(Collider collider)
	{
		if (S13ConditionalTrigger.__cache.ContainsKey(collider))
		{
			return S13ConditionalTrigger.__cache[collider];
		}
		return S13ConditionalTrigger.__cache[collider] = new ConditionInfo(collider.tag, collider.name, collider.gameObject.layer);
	}

	protected override void OnCompoundTriggerEnter(ConditionInfo colliderInfo)
	{
		base.OnCompoundTriggerEnter(colliderInfo);
		if (this.condition.Check(colliderInfo))
		{
			this.OnEnter(colliderInfo);
		}
	}

	protected override void OnCompoundTriggerExit(ConditionInfo colliderInfo)
	{
		base.OnCompoundTriggerExit(colliderInfo);
		if (this.condition.Check(colliderInfo))
		{
			this.OnExit(colliderInfo);
		}
	}

	protected abstract void OnEnter(ConditionInfo colliderInfo);

	protected abstract void OnExit(ConditionInfo colliderInfo);

	static S13ConditionalTrigger()
	{
		S13ConditionalTrigger.__cache = new Dictionary<Collider, ConditionInfo>();
	}
}
