using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace S13Audio;

[Serializable]
public class S13Action_SetAccessor : IS13Executable
{
	[Serializable]
	private class S13MixerParameter : IS13HorizontalPropertyHolder
	{
		[SerializeField]
		public AudioMixer mixer;

		[SerializeField]
		[S13LockableField]
		public string exposedParameter;
	}

	public enum UpdateMode
	{
		Instant = 0,
		LerpFromConstant = 1,
		LerpFromMixerParameter = 2
	}

	private const float PARAM_ROUND_VALUE = 0.01f;

	[Tooltip("The accessorID to set")]
	[SerializeField]
	[S13LockableField]
	[FormerlySerializedAs("parameterID")]
	private string accessorID;

	[Tooltip("The value to send through")]
	[SerializeField]
	[FormerlySerializedAs("paramValue")]
	private float targetValue;

	[Tooltip("Which UpdateMode should be used?")]
	[SerializeField]
	[FormerlySerializedAs("parameterUpdateMethod")]
	private UpdateMode updateMode;

	[Tooltip("What value should the lerp start at?")]
	[SerializeField]
	private float lerpStartValue;

	[Tooltip("What Mixer and ExposedParameter to target")]
	[SerializeField]
	private S13MixerParameter mixerParameter;

	[Tooltip("How long should it take to update to paramValue?")]
	[SerializeField]
	private float lerpDuration = 1f;

	[SerializeField]
	[S13CurveRange(0f, 0f, 1f, 1f)]
	private AnimationCurve lerpCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	private Coroutine runningRoutine;

	public bool IsExecutable => !string.IsNullOrEmpty(this.accessorID);

	public void Execute()
	{
		switch (this.updateMode)
		{
		case UpdateMode.Instant:
			S13Manager.SetAccessor(this.accessorID, this.targetValue);
			break;
		case UpdateMode.LerpFromConstant:
			this.BeginLerp(this.lerpStartValue, this.targetValue);
			break;
		case UpdateMode.LerpFromMixerParameter:
		{
			if (!this.mixerParameter.mixer.GetFloat(this.mixerParameter.exposedParameter, out var value))
			{
				S13Debug.LogWarning("Mixer " + this.mixerParameter.mixer.name + " does not have exposed parameter " + this.mixerParameter.exposedParameter);
				break;
			}
			this.lerpStartValue = S13AudioUtil.DecibelToLinear(value);
			this.BeginLerp(this.lerpStartValue, this.targetValue);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void BeginLerp(float startValue, float endValue)
	{
		if (!(Math.Abs(startValue - endValue) < 0.01f))
		{
			if (this.runningRoutine != null)
			{
				S13Manager.Instance.StopCoroutine(this.runningRoutine);
			}
			this.runningRoutine = S13Manager.Instance.StartCoroutine(this.LerpRoutine(this.accessorID, startValue, endValue, this.lerpDuration, this.lerpCurve));
		}
	}

	private IEnumerator LerpRoutine(string accID, float startVal, float endVal, float durration, AnimationCurve curve)
	{
		float elapsed;
		elapsed = 0f;
		while (elapsed < this.lerpDuration)
		{
			S13Manager.SetAccessor(accID, Mathf.Lerp(startVal, endVal, curve.Evaluate(elapsed / durration)));
			elapsed += Time.deltaTime;
			yield return null;
		}
		S13Manager.SetAccessor(accID, endVal);
	}
}
