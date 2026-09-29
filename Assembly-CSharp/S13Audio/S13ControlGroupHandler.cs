using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13ControlGroupHandler : MonoBehaviour, IEnumerable<S13Handler>, IEnumerable
{
	[Serializable]
	private class S13ControlGroupEntry : IS13HorizontalPropertyHolder, IEquatable<S13ControlGroupEntry>
	{
		public S13Object obj;

		public S13Handler handler;

		private float _lastApprovedTime;

		public float LastApprovedTime
		{
			get
			{
				return this._lastApprovedTime;
			}
			set
			{
				this._lastApprovedTime = value;
			}
		}

		public bool Equals(S13ControlGroupEntry other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (object.Equals(this.obj, other.obj))
			{
				return object.Equals(this.handler, other.handler);
			}
			return false;
		}
	}

	public S13ScriptableControlGroup reference;

	[SerializeField]
	private List<S13ControlGroupEntry> bindings = new List<S13ControlGroupEntry>();

	public float lastPlayApprovedTime = float.NegativeInfinity;

	public void RegisterObject(S13Object obj, S13Handler handler)
	{
		S13ControlGroupEntry item;
		item = new S13ControlGroupEntry
		{
			obj = obj,
			handler = handler
		};
		if (!this.bindings.Contains(item))
		{
			this.bindings.Add(item);
		}
	}

	public void MarkApprovedPlay(S13Object obj, S13Handler handler)
	{
		S13ControlGroupEntry s13ControlGroupEntry;
		s13ControlGroupEntry = this.bindings.FirstOrDefault((S13ControlGroupEntry x) => x.handler == handler && x.obj == obj);
		if (s13ControlGroupEntry == null)
		{
			S13Debug.LogWarning($"Received MarkApprovedPlay call for {handler}, but there is no matching binding", obj, S13Debug.LogType.ControlGroup);
			return;
		}
		s13ControlGroupEntry.LastApprovedTime = Time.time;
		this.lastPlayApprovedTime = s13ControlGroupEntry.LastApprovedTime;
	}

	public S13Handler GetOldestHandler()
	{
		if (this.bindings.Count == 1)
		{
			return this.bindings[0].handler;
		}
		return this.bindings.Where((S13ControlGroupEntry x) => x.handler.isPlaying).Aggregate((S13ControlGroupEntry x, S13ControlGroupEntry y) => (!(x.LastApprovedTime < y.LastApprovedTime)) ? y : x).handler;
	}

	public bool IsObjectPlaying(S13Object obj)
	{
		return this.bindings.Any((S13ControlGroupEntry x) => x.obj == obj && x.handler.isPlaying);
	}

	public IEnumerator<S13Handler> GetEnumerator()
	{
		return (from obj in this.bindings
			select obj.handler into x
			where x != null
			select x).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}
