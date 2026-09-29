using System;
using UnityEngine;

namespace I2.Loc;

[Serializable]
public class EventCallback
{
	public MonoBehaviour Target;

	public string MethodName = string.Empty;

	public void Execute(global::UnityEngine.Object Sender = null)
	{
		if (this.HasCallback() && Application.isPlaying)
		{
			this.Target.gameObject.SendMessage(this.MethodName, Sender, SendMessageOptions.DontRequireReceiver);
		}
	}

	public bool HasCallback()
	{
		if (this.Target != null)
		{
			return !string.IsNullOrEmpty(this.MethodName);
		}
		return false;
	}
}
