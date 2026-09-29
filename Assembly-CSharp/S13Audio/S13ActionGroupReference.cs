using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13ActionGroupReference : IS13ActionGroup
{
	[SerializeField]
	private S13GlobalActionGroup global;

	[SerializeField]
	private S13LocalActionGroup local;

	public void Execute()
	{
		if ((bool)this.global)
		{
			this.global.Execute();
		}
		else if ((bool)this.local)
		{
			this.local.Execute();
		}
	}

	public override string ToString()
	{
		if ((bool)this.global)
		{
			return this.global.ToString();
		}
		if ((bool)this.local)
		{
			return this.local.ToString();
		}
		return "Empty Reference";
	}
}
