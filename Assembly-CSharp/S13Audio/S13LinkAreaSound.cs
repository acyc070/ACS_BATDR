using UnityEngine;

namespace S13Audio;

public abstract class S13LinkAreaSound : MonoBehaviour
{
	private IS13AreaSound _areaSound;

	protected abstract Transform targetObject { get; }

	protected IS13AreaSound areaSound
	{
		get
		{
			if (this._areaSound == null)
			{
				this._areaSound = base.GetComponent<IS13AreaSound>();
			}
			return this._areaSound;
		}
	}

	private void Update()
	{
		if ((bool)this.targetObject)
		{
			this.areaSound.targetPosition = this.targetObject.position;
		}
	}
}
