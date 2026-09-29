using UnityEngine;

public class LightFlickerConnector : JMonoBehaviour
{
	[Header("Primary Light Flicker Controls")]
	[SerializeField]
	private LightFlicker m_PrimaryLight;

	[Header("Additional Light Flickers (Copy Primary)")]
	[SerializeField]
	private LightFlicker[] m_AdditionalLights;

	public override void Start()
	{
		if (this.m_AdditionalLights != null)
		{
			for (int i = 0; i < this.m_AdditionalLights.Length; i = unchecked(i + 1))
			{
				this.m_AdditionalLights[i].SetConnected(isConnected: true);
			}
		}
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && !(this.m_PrimaryLight == null) && !this.m_PrimaryLight.IsOff && this.m_AdditionalLights != null)
		{
			for (int i = 0; i < this.m_AdditionalLights.Length; i = unchecked(i + 1))
			{
				this.m_AdditionalLights[i].UpdateLight(this.m_PrimaryLight.WaveValue);
			}
		}
	}
}
