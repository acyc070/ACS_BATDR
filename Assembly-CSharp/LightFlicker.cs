using System;
using S13Audio;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : JMonoBehaviour
{
	private enum WaveType
	{
		SIN = 0,
		TRI = 1,
		SQR = 2,
		SAW = 3,
		INV = 4,
		NOISE = 5
	}

	[SerializeField]
	private bool m_IsOff;

	[Header("Flicker Settings")]
	[SerializeField]
	private WaveType m_WaveType;

	[SerializeField]
	private float m_Base;

	[SerializeField]
	private float m_Amplitude;

	[SerializeField]
	private float m_Phase;

	[SerializeField]
	private float m_Frequency;

	[Header("Emission - Primary Light Fixture")]
	[SerializeField]
	private LightFixture m_LightFixture;

	[Header("Emission - Additional Light Fixtures")]
	[SerializeField]
	private LightFixture[] m_ExtraLightFixtures;

	[Header("Audio Settings")]
	[SerializeField]
	private S13LocalAction m_OnMaxBrightnessAction;

	private bool m_AtMaxFlag;

	private Color m_OriginalColor;

	private float m_OrigintalIntensity;

	private float m_OriginalEmission = 1f;

	private bool m_IsConnected;

	public Color OriginalColor => this.m_OriginalColor;

	public Light Light { get; private set; }

	public bool IsOff => this.m_IsOff;

	public float WaveValue { get; private set; }

	public override void Awake()
	{
		this.Light = base.GetComponent<Light>();
		this.m_OriginalColor = this.Light.color;
		this.m_OrigintalIntensity = this.Light.intensity;
		if (this.m_LightFixture != null)
		{
			this.m_OriginalEmission = this.m_LightFixture.EmissionValue;
		}
	}

	private void Update()
	{
		if (!this.IsOff && !base.IsDisposed && !GameManager.Instance.IsPaused && !(this.Light == null) && !this.m_IsConnected && this.Light.enabled && this.Light.gameObject.activeInHierarchy)
		{
			this.UpdateLight(this.EvalWave());
		}
	}

	public void SetConnected(bool isConnected)
	{
		this.m_IsConnected = isConnected;
	}

	public void TurnOn(bool light = false)
	{
		if (light)
		{
			this.m_IsOff = true;
		}
		else
		{
			this.m_IsOff = false;
		}
		if (this.Light != null)
		{
			this.Light.color = this.m_OriginalColor;
			this.Light.intensity = this.m_OrigintalIntensity;
		}
		if (!(this.m_LightFixture != null))
		{
			return;
		}
		this.m_LightFixture.SetEmission(this.m_OriginalEmission);
		if (this.m_ExtraLightFixtures == null || this.m_ExtraLightFixtures.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.m_ExtraLightFixtures.Length; i = unchecked(i + 1))
		{
			LightFixture lightFixture;
			lightFixture = this.m_ExtraLightFixtures[i];
			if (lightFixture != null)
			{
				lightFixture.SetEmission(this.m_OriginalEmission);
			}
		}
	}

	public void TurnOff(bool light = false)
	{
		this.m_IsOff = true;
		unchecked
		{
			if (light)
			{
				this.Light.color = this.m_OriginalColor;
				this.Light.intensity = 0f;
				if (!(this.m_LightFixture != null))
				{
					return;
				}
				this.m_LightFixture.SetEmission(0f);
				if (this.m_ExtraLightFixtures == null || this.m_ExtraLightFixtures.Length == 0)
				{
					return;
				}
				for (int i = 0; i < this.m_ExtraLightFixtures.Length; i++)
				{
					LightFixture lightFixture;
					lightFixture = this.m_ExtraLightFixtures[i];
					if (lightFixture != null)
					{
						lightFixture.SetEmission(0f);
					}
				}
				return;
			}
			if (this.Light != null)
			{
				this.Light.color = this.m_OriginalColor;
				this.Light.intensity = this.m_OrigintalIntensity;
			}
			if (!(this.m_LightFixture != null))
			{
				return;
			}
			this.m_LightFixture.SetEmission(this.m_OriginalEmission);
			if (this.m_ExtraLightFixtures.Length == 0)
			{
				return;
			}
			for (int j = 0; j < this.m_ExtraLightFixtures.Length; j++)
			{
				LightFixture lightFixture2;
				lightFixture2 = this.m_ExtraLightFixtures[j];
				if (lightFixture2 != null)
				{
					lightFixture2.SetEmission(this.m_OriginalEmission);
				}
			}
		}
	}

	public void UpdateLight(float value)
	{
		if (value < 0f)
		{
			this.Light.color = this.m_OriginalColor * 0f;
		}
		else
		{
			this.Light.color = this.m_OriginalColor * value;
		}
		if (this.m_LightFixture != null)
		{
			this.m_LightFixture.SetEmission(value);
			if (this.m_ExtraLightFixtures != null && this.m_ExtraLightFixtures.Length != 0)
			{
				for (int i = 0; i < this.m_ExtraLightFixtures.Length; i = unchecked(i + 1))
				{
					LightFixture lightFixture;
					lightFixture = this.m_ExtraLightFixtures[i];
					if (lightFixture != null)
					{
						lightFixture.SetEmission(value);
					}
				}
			}
		}
		if (!this.m_AtMaxFlag && value >= 0.95f)
		{
			this.m_AtMaxFlag = true;
			this.m_OnMaxBrightnessAction.Execute();
		}
		else if (this.m_AtMaxFlag && value < 0.95f)
		{
			this.m_AtMaxFlag = false;
		}
	}

	private float EvalWave()
	{
		float num;
		num = (Time.time + this.m_Phase) * this.m_Frequency;
		float num2;
		num2 = 0f;
		num -= Mathf.Floor(num);
		return this.WaveValue = this.m_WaveType switch
		{
			WaveType.SIN => Mathf.Sin(num * 2f * MathF.PI), 
			WaveType.TRI => (!(num < 0.5f)) ? (-4f * num + 3f) : (4f * num - 1f), 
			WaveType.SQR => (!(num < 0.5f)) ? (-1f) : 1f, 
			WaveType.SAW => num, 
			WaveType.INV => 1f - num, 
			WaveType.NOISE => 1f - global::UnityEngine.Random.value * 2f, 
			_ => 1f, 
		} * this.m_Amplitude + this.m_Base;
	}

	protected override void OnDisposed()
	{
		this.Light = null;
		base.OnDisposed();
	}
}
