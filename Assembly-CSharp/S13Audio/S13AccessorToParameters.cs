using System;
using UnityEngine;
using UnityEngine.Audio;

namespace S13Audio;

public class S13AccessorToParameters : MonoBehaviour
{
	private enum NegativeValueHandling
	{
		Clamp = 0,
		AbsoluteClamp = 1,
		Ignore = 2
	}

	private enum InitialValueHandling
	{
		DoNothing = 0,
		SendMinInput = 1,
		SendMaxInput = 2
	}

	[Serializable]
	private class S13Parameter
	{
		private const string DEFAULT_PARAM_NAME = "MyExposedParam";

		[S13MixerParameter("mixer")]
		public string name = "MyExposedParam";

		[Header("Mapping")]
		public float minOutput;

		public float maxOutput = 1f;

		[S13CurveRange(0f, 0f, 1f, 1f)]
		public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		[Header("Dampening")]
		[Range(0.001f, 3f)]
		[Tooltip("Time to reach the input values in seconds. Lower values are more responsive.")]
		[SerializeField]
		public float damp = 0.001f;

		[SerializeField]
		[S13CurveRange(0f, 0f, 1f, 1f)]
		public AnimationCurve dampCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		[Header("Current Value")]
		[Tooltip("Current value will move to current mixer value on start")]
		public float currentValue;

		private float _startValue;

		private float _targetValue;

		private float _timeElapsed;

		public bool Init(AudioMixer mixer)
		{
			if (!mixer.GetFloat(this.name, out this.currentValue))
			{
				S13Debug.LogWarning(this.name + " was not found on " + mixer.name);
				return false;
			}
			this._targetValue = (this._startValue = this.currentValue);
			return true;
		}

		public void DestinationReached()
		{
			this.currentValue = this._targetValue;
		}

		public void Update(float deltaTime)
		{
			this._timeElapsed = Mathf.Min(this._timeElapsed + deltaTime, this.damp);
			if (this.name.Length != 0)
			{
				if (this._timeElapsed >= this.damp)
				{
					this.DestinationReached();
				}
				else if (this.dampCurve.length > 0)
				{
					this.Tick(this.dampCurve.Evaluate(this._timeElapsed / this.damp));
				}
				else
				{
					this.Tick(this._timeElapsed / this.damp);
				}
			}
		}

		public void Tick(float value)
		{
			this.currentValue = Mathf.Lerp(this._startValue, this._targetValue, value);
		}

		public void SetTarget(float value)
		{
			this._targetValue = this.Evaluate(value);
			this._startValue = this.currentValue;
			this._timeElapsed = 0f;
		}

		private float Evaluate(float value)
		{
			if (value < 0f || 1f < value)
			{
				S13Debug.LogWarning($"Received non normalized value {value}");
				value = Mathf.Clamp01(S13AccessorToParameters.Round(value));
			}
			float num;
			num = Mathf.Clamp01(S13AccessorToParameters.Round(this.curve.Evaluate(value)));
			return (this.maxOutput - this.minOutput) * num + this.minOutput;
		}
	}

	private const string DEFAULT_ACCESSOR_ID = "";

	private const int PLACES_TO_ROUND = 4;

	[Tooltip("Unique Identifier to access this ParameterController from S13Manager")]
	[S13LockableField]
	[SerializeField]
	private string accessorID = "";

	[Tooltip("The mixer with exposed parameters to control")]
	[SerializeField]
	private AudioMixer mixer;

	[Tooltip("Minimum value expected from input")]
	[SerializeField]
	private float minInput;

	[Tooltip("Maximum value expected from input")]
	[SerializeField]
	private float maxInput = 1f;

	[SerializeField]
	private NegativeValueHandling negativeValueHandling;

	[Tooltip("What should the S13AccessorToParameters do on startup?")]
	[SerializeField]
	private InitialValueHandling initialValueHandling;

	[SerializeField]
	private bool ignoreTimeScale = true;

	[Tooltip("Parameters to control. Must be exposed in the AudioMixer, and should be renamed")]
	[SerializeField]
	private S13Parameter[] parameters = new S13Parameter[1];

	public float MinInput => this.minInput;

	public float MaxInput => this.maxInput;

	private float deltaTime
	{
		get
		{
			if (!this.ignoreTimeScale)
			{
				return Time.deltaTime;
			}
			return Time.unscaledDeltaTime;
		}
	}

	private void Awake()
	{
		if (S13Manager.IsManagerLoaded)
		{
			this.Init();
		}
		else
		{
			S13Manager.OnS13ManagerLoad += this.Init;
		}
	}

	private void OnDestroy()
	{
		if (!this.accessorID.Equals(""))
		{
			S13Manager.DeRegisterAccessorListener(this.accessorID, this.UpdateValue);
		}
	}

	private void Update()
	{
		S13Parameter[] array;
		array = this.parameters;
		foreach (S13Parameter s13Parameter in array)
		{
			s13Parameter.Update(this.deltaTime);
			this.mixer.SetFloat(s13Parameter.name, S13AccessorToParameters.Round(s13Parameter.currentValue));
		}
	}

	private void OnValidate()
	{
		if (this.minInput > this.maxInput)
		{
			this.minInput = this.maxInput;
		}
	}

	public void UpdateValue(float nextValue)
	{
		if (nextValue < 0f)
		{
			switch (this.negativeValueHandling)
			{
			case NegativeValueHandling.AbsoluteClamp:
				nextValue = Mathf.Abs(nextValue);
				break;
			case NegativeValueHandling.Ignore:
				return;
			default:
				throw new ArgumentOutOfRangeException();
			case NegativeValueHandling.Clamp:
				break;
			}
		}
		float target;
		target = S13AccessorToParameters.Round(this.Normalize01(Mathf.Clamp(nextValue, this.minInput, this.maxInput)));
		S13Parameter[] array;
		array = this.parameters;
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			array[i].SetTarget(target);
		}
	}

	private void Init()
	{
		S13Manager.OnS13ManagerLoad -= this.Init;
		if (this.accessorID.Equals(""))
		{
			S13Debug.LogWarning(base.name + " does not have unique Accessor_ID", this);
			base.enabled = false;
			return;
		}
		if (this.mixer == null || this.parameters == null || this.parameters.Length == 0)
		{
			S13Debug.LogWarning(base.name + " has null mixer or parameters", this);
			base.enabled = false;
			return;
		}
		S13Parameter[] array;
		array = this.parameters;
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			if (!array[i].Init(this.mixer))
			{
				S13Debug.LogWarning("Parameter is not valid on " + this.mixer.name + ": " + this.parameters[0].name, this);
				base.enabled = false;
				return;
			}
		}
		S13Manager.RegisterAccessorListener(this.accessorID, this.UpdateValue);
		switch (this.initialValueHandling)
		{
		case InitialValueHandling.SendMinInput:
			this.UpdateValue(this.minInput);
			break;
		case InitialValueHandling.SendMaxInput:
			this.UpdateValue(this.maxInput);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case InitialValueHandling.DoNothing:
			break;
		}
	}

	private float Normalize01(float value)
	{
		return Mathf.Abs(value - this.minInput) / Mathf.Abs(this.maxInput - this.minInput);
	}

	private static float Round(float value)
	{
		float num;
		num = Mathf.Pow(10f, 4f);
		return Mathf.Round(value * num) / num;
	}
}
