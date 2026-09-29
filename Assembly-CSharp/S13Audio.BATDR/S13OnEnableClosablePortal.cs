using UnityEngine;

namespace S13Audio.BATDR;

public class S13OnEnableClosablePortal : S13ClosablePortal
{
	[SerializeField]
	private bool onEnableValue = true;

	private bool _value = true;

	public override bool IsOpen()
	{
		return this._value;
	}

	private void OnEnable()
	{
		this._value = this.onEnableValue;
	}

	private void OnDisable()
	{
		this._value = !this.onEnableValue;
	}
}
