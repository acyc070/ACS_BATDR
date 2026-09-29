using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace S13Audio;

public abstract class S13ObjectPrimitive<TObject, THandler> : S13Object<TObject, THandler>, IS13ObjectPrimitive, IS13Controllable where TObject : S13ObjectPrimitive<TObject, THandler> where THandler : S13HandlerPrimitive<TObject, THandler>
{
	[Space]
	[FormerlySerializedAs("mixerGroup")]
	public AudioMixerGroup outputOverride;

	[S13Foldout("Advanced Settings", "S13ObjectPrimitive")]
	public bool spatialize;

	[S13Foldout("Advanced Settings", "S13ObjectPrimitive")]
	public bool bypassReverbZones;

	[S13Slider("Priority", 0f, 256f)]
	[S13Foldout("Advanced Settings", "S13ObjectPrimitive")]
	public int priority = 128;

	public S13Voice voiceTemplate;

	public bool loop;

	public S13MinMaxFloat volumeRange = new S13MinMaxFloat("Volume Range (dB)", -80f, 0f, 0f);

	[S13Slider("Fade In Time (s)", 0f, 60f)]
	public float fadeInTime;

	[S13Slider("Fade Out Time (s)", 0f, 60f)]
	public float fadeOutTime;

	[SerializeField]
	private S13CurveReference _fadeCurve;

	public S13MinMaxFloat pitchRange = new S13MinMaxFloat("Pitch Range (semitones)", -12f, 12f);

	[S13Foldout("Advanced Settings", "S13ObjectPrimitive")]
	public S13MinMaxFloat positionOffsetRange = new S13MinMaxFloat("Position Offset Range", 0f, 50f);

	[S13Foldout("Advanced Settings", "S13ObjectPrimitive")]
	public bool positionOffsetLockY;

	public int voiceCount = 1;

	public VoiceMode voiceMode;

	public FollowMode followMode;

	public AnimationCurve fadeCurve => this._fadeCurve.Curve;

	public abstract float startTime { get; }

	public Vector3 GetPositionOffset()
	{
		if (this.positionOffsetLockY)
		{
			Vector3 insideUnitSphere;
			insideUnitSphere = Random.insideUnitSphere;
			insideUnitSphere.y = 0f;
			return insideUnitSphere.normalized * this.positionOffsetRange.value;
		}
		return Random.onUnitSphere * this.positionOffsetRange.value;
	}

	public sealed override IEnumerable<S13Object> GetDependencies()
	{
		yield break;
	}

	protected virtual void OnValidate()
	{
		this.voiceCount = Mathf.Max(this.voiceCount, 1);
	}
}
