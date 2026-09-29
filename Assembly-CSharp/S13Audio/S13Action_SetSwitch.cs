using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio;

[Serializable]
public class S13Action_SetSwitch : IS13Executable
{
	public enum SwitchType
	{
		Global = 0,
		Local = 1
	}

	public enum PlayOnSwitch
	{
		DontPlay = 0,
		Play = 1,
		PlayIfChanged = 2
	}

	[Tooltip("Are we targeting all valid switches (Global) or a specific S13AudioReference (local)")]
	public SwitchType switchType = SwitchType.Local;

	[Tooltip("What S13ScriptableSwitch should the Switch be set to?")]
	public S13ScriptableSwitch scriptableSwitch;

	[Tooltip("Should we play the target S13AudioReference after switching?")]
	public PlayOnSwitch playOnSwitch = PlayOnSwitch.Play;

	[Tooltip("What target should be switched? target must be a S13ObjectSwitch or S13AudioHandler containing a S13ObjectSwitch")]
	[FormerlySerializedAs("parent")]
	public S13AudioReference target;

	public bool IsExecutable
	{
		get
		{
			if (this.scriptableSwitch == null)
			{
				return false;
			}
			switch (this.switchType)
			{
			case SwitchType.Global:
				return this.scriptableSwitch != null;
			case SwitchType.Local:
			{
				if (this.target == null)
				{
					return false;
				}
				IS13Controllable reference;
				reference = this.target.Reference;
				if (!(reference is S13ObjectSwitch obj))
				{
					if (reference is S13AudioHandler s13AudioHandler)
					{
						return s13AudioHandler.GetHandler() is S13HandlerSwitch;
					}
					return false;
				}
				return S13Manager.GetGlobalHandler(obj) is S13HandlerSwitch;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	public void Execute()
	{
		switch (this.switchType)
		{
		case SwitchType.Global:
			S13Manager.SetSwitch(this.scriptableSwitch);
			break;
		case SwitchType.Local:
		{
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
				S13ScriptableSwitch currentScriptableSwitch;
				currentScriptableSwitch = iS13Switchable.CurrentScriptableSwitch;
				S13Manager.SetSwitch(iS13Switchable, this.scriptableSwitch);
				switch (this.playOnSwitch)
				{
				case PlayOnSwitch.Play:
					this.target.Play();
					break;
				case PlayOnSwitch.PlayIfChanged:
					if (currentScriptableSwitch != this.scriptableSwitch)
					{
						this.target.Play();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				case PlayOnSwitch.DontPlay:
					break;
				}
			}
			else
			{
				global::UnityEngine.Object obj2;
				obj2 = this.target.Reference as global::UnityEngine.Object;
				S13Debug.LogWarning("Could not set switch " + obj2?.name + ", target is not switchable", obj2);
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public override string ToString()
	{
		string text;
		text = "SetSwitch ";
		switch (this.switchType)
		{
		case SwitchType.Global:
			if (this.scriptableSwitch != null)
			{
				text = text + " Set Global " + this.scriptableSwitch.name;
			}
			break;
		case SwitchType.Local:
			if (this.target.Reference != null && this.scriptableSwitch != null)
			{
				text = text + " on " + this.target.name + " to " + this.scriptableSwitch.name;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return text;
	}
}
