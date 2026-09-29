using UnityEngine;

namespace S13Audio;

public class S13LinkAreaSoundByReference : S13LinkAreaSound
{
	public Transform _targetObject;

	protected override Transform targetObject => this._targetObject;
}
