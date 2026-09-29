using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public struct S13MinMaxInt
{
	[SerializeField]
	private string overrideLabel;

	public int minValue;

	public int maxValue;

	public int minLimit;

	public int maxLimit;

	public int value => global::UnityEngine.Random.Range(this.minValue, unchecked(this.maxValue + 1));

	public void ClampValues()
	{
		this.minValue = Mathf.Clamp(this.minValue, this.minLimit, this.maxLimit);
		this.maxValue = Mathf.Clamp(this.maxValue, this.minValue, this.maxLimit);
	}

	public S13MinMaxInt(int minLimit, int maxLimit)
		: this(null, minLimit, maxLimit)
	{
	}

	public S13MinMaxInt(int minLimit, int maxLimit, int value)
		: this(null, minLimit, maxLimit, value)
	{
	}

	public S13MinMaxInt(int minLimit, int maxLimit, int minValue, int maxValue)
		: this(null, minLimit, maxLimit, minValue, maxValue)
	{
	}

	public S13MinMaxInt(string overrideLabel)
		: this(overrideLabel, 0, 0, 0)
	{
	}

	public S13MinMaxInt(string overrideLabel, int minLimit, int maxLimit)
		: this(overrideLabel, minLimit, maxLimit, 0)
	{
	}

	public S13MinMaxInt(string overrideLabel, int minLimit, int maxLimit, int value)
		: this(overrideLabel, minLimit, maxLimit, value, value)
	{
	}

	public S13MinMaxInt(string overrideLabel, int minLimit, int maxLimit, int minValue, int maxValue)
	{
		this.overrideLabel = overrideLabel;
		this.minLimit = minLimit;
		this.maxLimit = maxLimit;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}
}
