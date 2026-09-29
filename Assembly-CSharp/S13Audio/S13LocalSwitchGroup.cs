using System;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

public class S13LocalSwitchGroup : MonoBehaviour, IS13ActionGroup
{
	[Serializable]
	internal class S13SwitchJunction
	{
		[SerializeField]
		private S13ScriptableSwitch scriptableSwitch;

		[SerializeField]
		public S13LocalAction _action = new S13LocalAction();

		public S13ScriptableSwitch ScriptableSwitch => this.scriptableSwitch;

		public void Execute()
		{
			this._action.Execute();
		}
	}

	[Serializable]
	internal class S13SwitchJunctionList : ReorderableArray<S13SwitchJunction>
	{
	}

	[Tooltip("Setup a switch default, or it will use the first switch in the list.")]
	[SerializeField]
	private S13ScriptableSwitch selectedScriptableSwitch;

	[SerializeField]
	[Reorderable]
	private S13SwitchJunctionList switchJunctionList = new S13SwitchJunctionList();

	private void Awake()
	{
		if (this.selectedScriptableSwitch == null)
		{
			this.selectedScriptableSwitch = this.switchJunctionList.First().ScriptableSwitch;
		}
	}

	public void Execute()
	{
		foreach (S13SwitchJunction switchJunction in this.switchJunctionList)
		{
			if (!(this.selectedScriptableSwitch != switchJunction.ScriptableSwitch))
			{
				switchJunction.Execute();
				return;
			}
		}
		S13Debug.LogError("S13SwitchGroup::Execute() selected switch does not match possibilities");
	}

	public void SetSwitch(S13ScriptableSwitch targetScriptableSwitch)
	{
		foreach (S13SwitchJunction switchJunction in this.switchJunctionList)
		{
			if (!(targetScriptableSwitch != switchJunction.ScriptableSwitch))
			{
				this.selectedScriptableSwitch = targetScriptableSwitch;
				return;
			}
		}
		S13Debug.LogWarning("S13SwitchGroup::SetSwitch() target switch does not match possibilities");
	}
}
