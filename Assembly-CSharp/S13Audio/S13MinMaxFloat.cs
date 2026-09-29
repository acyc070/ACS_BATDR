using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public struct S13MinMaxFloat
{
	[SerializeField]
	private string overrideLabel;

	public float minValue;

	public float maxValue;

	public float minLimit;

	public float maxLimit;

	public float value => global::UnityEngine.Random.Range(this.minValue, this.maxValue);

	public void ClampValues()
	{
		this.minValue = Mathf.Clamp(this.minValue, this.minLimit, this.maxLimit);
		this.maxValue = Mathf.Clamp(this.maxValue, this.minValue, this.maxLimit);
	}

	public S13MinMaxFloat(float minLimit, float maxLimit)
		: this(null, minLimit, maxLimit)
	{
	}

	public S13MinMaxFloat(float minLimit, float maxLimit, float value)
		: this(null, minLimit, maxLimit, value)
	{
	}

	public S13MinMaxFloat(float minLimit, float maxLimit, float minValue, float maxValue)
		: this(null, minLimit, maxLimit, minValue, maxValue)
	{
	}

	public S13MinMaxFloat(string overrideLabel)
		: this(overrideLabel, 0f, 0f, 0f)
	{
	}

	public S13MinMaxFloat(string overrideLabel, float minLimit, float maxLimit)
		: this(overrideLabel, minLimit, maxLimit, 0f)
	{
	}

	public S13MinMaxFloat(string overrideLabel, float minLimit, float maxLimit, float value)
		: this(overrideLabel, minLimit, maxLimit, value, value)
	{
	}

	public S13MinMaxFloat(string overrideLabel, float minLimit, float maxLimit, float minValue, float maxValue)
	{
		this.overrideLabel = overrideLabel;
		this.minLimit = minLimit;
		this.maxLimit = maxLimit;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}
}
