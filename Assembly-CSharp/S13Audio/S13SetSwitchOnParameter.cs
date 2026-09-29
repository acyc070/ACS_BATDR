using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

public class S13SetSwitchOnParameter : MonoBehaviour
{
	[Serializable]
	private class FloatSwitchPair : IS13HorizontalPropertyHolder
	{
		public float minVal;

		public S13ScriptableSwitch ScriptableSwitch;
	}

	private const string defaultParamName = "";

	[SerializeField]
	[S13LockableField]
	private string paramName = "";

	[SerializeField]
	private S13AudioReference target;

	private Transform targetHandlerParent;

	[Tooltip("Enter the MIN value that relates to given entry, must be ordered least to most")]
	[SerializeField]
	private List<FloatSwitchPair> ranges;

	private float savedValue;

	private FloatSwitchPair currentPair;

	private void OnEnable()
	{
		if (!string.Equals(this.paramName, ""))
		{
			S13Manager.RegisterAccessorListener(this.paramName, this.SetValue);
		}
	}

	private void OnDisable()
	{
		if (!string.Equals(this.paramName, ""))
		{
			S13Manager.DeRegisterAccessorListener(this.paramName, this.SetValue);
		}
	}

	private void OnValidate()
	{
		if (!Enumerable.SequenceEqual(second: this.ranges.OrderBy((FloatSwitchPair x) => x.minVal).ToList(), first: this.ranges))
		{
			Debug.LogError("Ranges on SetSwitchOnParameter are out of order!", this);
		}
	}

	public void SetValue(float value)
	{
		if ((double)Math.Abs(this.savedValue - value) < 0.001)
		{
			return;
		}
		this.savedValue = value;
		FloatSwitchPair floatSwitchPair;
		floatSwitchPair = this.ranges.FindLast((FloatSwitchPair x) => this.savedValue > x.minVal);
		if (floatSwitchPair == null || floatSwitchPair == this.currentPair)
		{
			return;
		}
		S13ScriptableSwitch s13ScriptableSwitch;
		s13ScriptableSwitch = floatSwitchPair?.ScriptableSwitch;
		if (s13ScriptableSwitch == null)
		{
			return;
		}
		this.currentPair = floatSwitchPair;
		IS13Switchable iS13Switchable;
		iS13Switchable = null;
		IS13Controllable reference;
		reference = this.target.Reference;
		if (!(reference is S13ObjectSwitch obj))
		{
			if (reference is S13AudioHandler s13AudioHandler)
			{
				iS13Switchable = s13AudioHandler.GetHandler() as S13HandlerSwitch;
			}
		}
		else
		{
			iS13Switchable = S13Manager.GetGlobalHandler(obj) as S13HandlerSwitch;
		}
		if (iS13Switchable != null)
		{
			S13Manager.SetSwitch(iS13Switchable, s13ScriptableSwitch);
			return;
		}
		S13Debug.LogWarning("Could not set switch " + s13ScriptableSwitch.name + ", handler was not found. Disabling to prevent future errors");
		base.enabled = false;
	}
}
