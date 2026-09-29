using UnityEngine;

namespace S13Audio;

public class S13LinkAreaSoundByTag : S13LinkAreaSound
{
	[Tooltip("Any object matching this tag is a valid candidate for the area sound.")]
	[SerializeField]
	[TagSelector]
	private string _targetTag;

	private Transform _targetObject;

	public string targetTag
	{
		get
		{
			return this._targetTag;
		}
		set
		{
			this._targetTag = value;
			this._targetObject = null;
		}
	}

	protected override Transform targetObject
	{
		get
		{
			if (!this._targetObject)
			{
				this._targetObject = GameObject.FindWithTag(this.targetTag)?.transform;
			}
			return this._targetObject;
		}
	}
}
