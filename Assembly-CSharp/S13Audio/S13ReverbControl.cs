using System;
using UnityEngine;

namespace S13Audio;

public class S13ReverbControl : MonoBehaviour
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

	private enum ReverbZoneSetting
	{
		NONE = 0,
		Room = 1,
		RoomHF = 2,
		RoomLF = 3,
		DecayTime = 4,
		DecayHFRatio = 5,
		Reflections = 6,
		ReflectionsDecay = 7,
		Reverb = 8,
		ReverbDelay = 9,
		HFReference = 10,
		LFReference = 11,
		Diffusion = 12,
		Density = 13
	}

	[Serializable]
	private class ZoneParameter
	{
		[HideInInspector]
		public string name = "None";

		public ReverbZoneSetting setting;

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
		[S13ReadOnlyField]
		private float currentValue;

		private AudioReverbZone zone;

		private float _startValue;

		private float _targetValue;

		private float _timeElapsed;

		public float CurrentValue => this.currentValue;

		public void Init(AudioReverbZone zone)
		{
			this.zone = zone;
			this.currentValue = this.GetCurrentSettingValue(this.setting);
			this._targetValue = (this._startValue = this.currentValue);
		}

		public void OnValidate()
		{
			this.name = this.setting.ToString();
			Vector2 zoneSettingBounds;
			zoneSettingBounds = this.GetZoneSettingBounds(this.setting);
			if (this.minOutput < zoneSettingBounds.x)
			{
				this.minOutput = zoneSettingBounds.x;
			}
			else if (this.minOutput > zoneSettingBounds.y)
			{
				this.minOutput = zoneSettingBounds.y;
			}
			if (this.maxOutput > zoneSettingBounds.y)
			{
				this.maxOutput = zoneSettingBounds.y;
			}
			else if (this.maxOutput < zoneSettingBounds.x)
			{
				this.maxOutput = zoneSettingBounds.x;
			}
		}

		public void DestinationReached()
		{
			this.currentValue = this._targetValue;
		}

		public void Update(float deltaTime)
		{
			this._timeElapsed = Mathf.Min(this._timeElapsed + deltaTime, this.damp);
			if (this.setting != ReverbZoneSetting.NONE)
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
				value = Mathf.Clamp01(S13ReverbControl.Round(value));
			}
			float num;
			num = Mathf.Clamp01(S13ReverbControl.Round(this.curve.Evaluate(value)));
			return (this.maxOutput - this.minOutput) * num + this.minOutput;
		}

		private Vector2 GetZoneSettingBounds(ReverbZoneSetting setting)
		{
			return setting switch
			{
				ReverbZoneSetting.Room => new Vector2(-10000f, 0f), 
				ReverbZoneSetting.RoomHF => new Vector2(-10000f, 0f), 
				ReverbZoneSetting.RoomLF => new Vector2(-10000f, 0f), 
				ReverbZoneSetting.DecayTime => new Vector2(0.1f, 20f), 
				ReverbZoneSetting.DecayHFRatio => new Vector2(0.1f, 2f), 
				ReverbZoneSetting.Reflections => new Vector2(-10000f, 1000f), 
				ReverbZoneSetting.ReflectionsDecay => new Vector2(0f, 0.3f), 
				ReverbZoneSetting.Reverb => new Vector2(-10000f, 2000f), 
				ReverbZoneSetting.ReverbDelay => new Vector2(0f, 0.1f), 
				ReverbZoneSetting.HFReference => new Vector2(1000f, 20000f), 
				ReverbZoneSetting.LFReference => new Vector2(20f, 1000f), 
				ReverbZoneSetting.Diffusion => new Vector2(0f, 100f), 
				ReverbZoneSetting.Density => new Vector2(0f, 100f), 
				_ => new Vector2(0f, 0f), 
			};
		}

		private float GetCurrentSettingValue(ReverbZoneSetting setting)
		{
			return setting switch
			{
				ReverbZoneSetting.NONE => 0f, 
				ReverbZoneSetting.Room => this.zone.room, 
				ReverbZoneSetting.RoomHF => this.zone.roomHF, 
				ReverbZoneSetting.RoomLF => this.zone.roomLF, 
				ReverbZoneSetting.DecayTime => this.zone.decayTime, 
				ReverbZoneSetting.DecayHFRatio => this.zone.decayHFRatio, 
				ReverbZoneSetting.Reflections => this.zone.reflections, 
				ReverbZoneSetting.ReflectionsDecay => this.zone.reflectionsDelay, 
				ReverbZoneSetting.Reverb => this.zone.reverb, 
				ReverbZoneSetting.ReverbDelay => this.zone.reverbDelay, 
				ReverbZoneSetting.HFReference => this.zone.HFReference, 
				ReverbZoneSetting.LFReference => this.zone.LFReference, 
				ReverbZoneSetting.Diffusion => this.zone.diffusion, 
				ReverbZoneSetting.Density => this.zone.density, 
				_ => 0f, 
			};
		}
	}

	private const string DEFAULT_ACCESSOR_ID = "";

	private const int PLACES_TO_ROUND = 4;

	[Tooltip("Unique Identifier to access this S13ReverbControl from S13Manager")]
	[S13LockableField]
	[SerializeField]
	private string accessorID = "";

	[Tooltip("The zone to control")]
	[SerializeField]
	private AudioReverbZone zone;

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
	private ZoneParameter[] zoneSettings = new ZoneParameter[1];

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
		ZoneParameter[] array;
		array = this.zoneSettings;
		foreach (ZoneParameter zoneParameter in array)
		{
			zoneParameter.Update(this.deltaTime);
			this.SetZoneValue(zoneParameter.setting, S13ReverbControl.Round(zoneParameter.CurrentValue));
		}
	}

	private void OnValidate()
	{
		if (this.minInput > this.maxInput)
		{
			this.minInput = this.maxInput;
		}
		for (int i = 0; i < this.zoneSettings.Length; i = unchecked(i + 1))
		{
			this.zoneSettings[i].OnValidate();
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
		target = S13ReverbControl.Round(this.Normalize01(Mathf.Clamp(nextValue, this.minInput, this.maxInput)));
		ZoneParameter[] array;
		array = this.zoneSettings;
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			array[i].SetTarget(target);
		}
	}

	private void SetZoneValue(ReverbZoneSetting setting, float value)
	{
		unchecked
		{
			switch (setting)
			{
			case ReverbZoneSetting.Room:
				this.zone.room = (int)value;
				break;
			case ReverbZoneSetting.RoomHF:
				this.zone.roomHF = (int)value;
				break;
			case ReverbZoneSetting.RoomLF:
				this.zone.roomLF = (int)value;
				break;
			case ReverbZoneSetting.DecayTime:
				this.zone.decayTime = value;
				break;
			case ReverbZoneSetting.DecayHFRatio:
				this.zone.decayHFRatio = value;
				break;
			case ReverbZoneSetting.Reflections:
				this.zone.reflections = (int)value;
				break;
			case ReverbZoneSetting.ReflectionsDecay:
				this.zone.reflectionsDelay = value;
				break;
			case ReverbZoneSetting.Reverb:
				this.zone.reverb = (int)value;
				break;
			case ReverbZoneSetting.ReverbDelay:
				this.zone.reverbDelay = value;
				break;
			case ReverbZoneSetting.HFReference:
				this.zone.HFReference = value;
				break;
			case ReverbZoneSetting.LFReference:
				this.zone.LFReference = value;
				break;
			case ReverbZoneSetting.Diffusion:
				this.zone.diffusion = value;
				break;
			case ReverbZoneSetting.Density:
				this.zone.density = value;
				break;
			case ReverbZoneSetting.NONE:
				break;
			}
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
		if (this.zone == null || this.zoneSettings == null || this.zoneSettings.Length == 0)
		{
			S13Debug.LogWarning(base.name + " has null mixer or parameters", this);
			base.enabled = false;
			return;
		}
		S13Manager.RegisterAccessorListener(this.accessorID, this.UpdateValue);
		for (int i = 0; i < this.zoneSettings.Length; i = unchecked(i + 1))
		{
			this.zoneSettings[i].Init(this.zone);
		}
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
