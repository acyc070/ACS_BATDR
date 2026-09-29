using UnityEngine;

namespace S13Audio;

public abstract class S13ObjectPrimitiveSingleClip<TObject, THandler> : S13ObjectPrimitive<TObject, THandler>, IS13ObjectPrimitiveSingleClip, IS13ObjectPrimitive, IS13Controllable where TObject : S13ObjectPrimitiveSingleClip<TObject, THandler> where THandler : S13HandlerPrimitiveSingleClip<TObject, THandler>
{
	[Space]
	public AudioClip clip;

	public S13MinMaxFloat startTimeRange = new S13MinMaxFloat("Start Time Range (s)");

	public override float startTime => this.startTimeRange.value;

	public AudioClip GetClip()
	{
		return this.clip;
	}

	public void SetClip(AudioClip clip)
	{
		this.clip = clip;
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		this.startTimeRange.maxLimit = (this.clip ? this.clip.length : 0f);
		this.startTimeRange.ClampValues();
	}
}
