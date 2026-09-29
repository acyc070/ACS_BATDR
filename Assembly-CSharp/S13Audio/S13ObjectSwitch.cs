using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Compound/Switch", order = 3)]
public class S13ObjectSwitch : S13ObjectCompound<S13ObjectSwitch, S13HandlerSwitch>, IEnumerable<S13Object>, IEnumerable
{
	[Serializable]
	private class SwitchStateList : ReorderableArray<S13ObjectSwitchState>
	{
		public SwitchStateList(int length)
			: base(length)
		{
		}

		public SwitchStateList()
		{
		}
	}

	[Serializable]
	[S13HorizontalPropertyHolder(forceExpand = true, label = null)]
	private class S13ObjectSwitchState : IS13HorizontalPropertyHolder
	{
		public S13ScriptableSwitch s13ScriptableSwitch;

		public S13Object s13Object;

		public S13CompoundInterruptMode interuptMode = S13CompoundInterruptMode.Stop;
	}

	[Tooltip("List of S13ScriptableSwitch and the desired S13Object")]
	[SerializeField]
	[Reorderable(surrogateType = new Type[]
	{
		typeof(S13Object),
		typeof(S13ScriptableSwitch)
	}, surrogateProperty = new string[] { "s13Object", "s13ScriptableSwitch" })]
	private SwitchStateList switchStates;

	[Tooltip("Should the S13HandlerSwitch call Play on the new state after receiving a SetSwitch call. Intended use is for Global switches, where it is hard to get a specific play call")]
	[SerializeField]
	private bool playOnSwitch;

	public bool PlayOnSwitch => this.playOnSwitch;

	public S13ScriptableSwitch GetDefaultState()
	{
		if (this.switchStates.Count == 0)
		{
			S13Debug.LogWarning("S13ObjectSwitch " + this?.ToString() + " has no states", this);
		}
		return this.switchStates.First()?.s13ScriptableSwitch;
	}

	public S13Object FindS13Object(S13ScriptableSwitch toFind)
	{
		return this.switchStates.FirstOrDefault((S13ObjectSwitchState x) => x.s13ScriptableSwitch == toFind)?.s13Object;
	}

	public bool ShouldInterupt(S13ScriptableSwitch toFind)
	{
		S13ObjectSwitchState s13ObjectSwitchState;
		s13ObjectSwitchState = this.switchStates.FirstOrDefault((S13ObjectSwitchState x) => x.s13ScriptableSwitch == toFind);
		if (s13ObjectSwitchState == null)
		{
			return false;
		}
		return s13ObjectSwitchState.interuptMode switch
		{
			S13CompoundInterruptMode.Continue => false, 
			S13CompoundInterruptMode.Stop => true, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public bool UsesScriptableSwitch(S13ScriptableSwitch toFind)
	{
		return this.switchStates.Select((S13ObjectSwitchState x) => x.s13ScriptableSwitch).Contains(toFind);
	}

	public S13ScriptableSwitch[] GetSwitches()
	{
		return this.switchStates.Select((S13ObjectSwitchState x) => x.s13ScriptableSwitch).ToArray();
	}

	public Dictionary<S13ScriptableSwitch, S13Object> GetEntries()
	{
		return this.switchStates.ToDictionary((S13ObjectSwitchState x) => x.s13ScriptableSwitch, (S13ObjectSwitchState y) => y.s13Object);
	}

	public override IEnumerable<S13Object> GetDependencies()
	{
		return this.switchStates.Select((S13ObjectSwitchState x) => x.s13Object).ToArray();
	}

	public IEnumerator<S13Object> GetEnumerator()
	{
		foreach (S13Object item in this.switchStates.Select((S13ObjectSwitchState x) => x.s13Object))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (S13Object item in this.switchStates.Select((S13ObjectSwitchState x) => x.s13Object))
		{
			yield return item;
		}
	}
}
