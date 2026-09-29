using UnityEngine;

public class LightFixture : JMonoBehaviour
{
	private const string POWER = "_Power";

	[SerializeField]
	private MeshRenderer m_LightRenderer;

	[SerializeField]
	private SkinnedMeshRenderer m_SkinnedLightRenderer;

	public MeshRenderer Renderer => this.m_LightRenderer;

	public SkinnedMeshRenderer SkinnedRenderer => this.m_SkinnedLightRenderer;

	public float InitialEmissionValue { get; private set; }

	public float EmissionValue
	{
		get
		{
			if (this.m_LightRenderer != null)
			{
				if (!this.HasEmission)
				{
					return 1f;
				}
				return this.m_LightRenderer.material.GetFloat("_Power");
			}
			if (this.m_SkinnedLightRenderer != null)
			{
				if (!this.HasEmission)
				{
					return 1f;
				}
				return this.m_SkinnedLightRenderer.material.GetFloat("_Power");
			}
			return 1f;
		}
	}

	public bool HasEmission
	{
		get
		{
			if (this.m_LightRenderer != null)
			{
				if (!Application.isPlaying)
				{
					if (this.m_LightRenderer != null && this.m_LightRenderer.sharedMaterial != null)
					{
						return this.m_LightRenderer.sharedMaterial.HasProperty("_Power");
					}
					return false;
				}
				if (this.m_LightRenderer != null && this.m_LightRenderer.material != null)
				{
					return this.m_LightRenderer.material.HasProperty("_Power");
				}
				return false;
			}
			if (this.m_SkinnedLightRenderer != null)
			{
				if (!Application.isPlaying)
				{
					if (this.m_SkinnedLightRenderer != null && this.m_SkinnedLightRenderer.sharedMaterial != null)
					{
						return this.m_SkinnedLightRenderer.sharedMaterial.HasProperty("_Power");
					}
					return false;
				}
				if (this.m_SkinnedLightRenderer != null && this.m_SkinnedLightRenderer.material != null)
				{
					return this.m_SkinnedLightRenderer.material.HasProperty("_Power");
				}
				return false;
			}
			return false;
		}
	}

	public override void Start()
	{
		this.InitialEmissionValue = this.EmissionValue;
	}

	public void SetEmission(float value)
	{
		if (!this.HasEmission)
		{
			return;
		}
		if (value < 0f)
		{
			value = 0f;
		}
		else if (value > 1f)
		{
			value = 1f;
		}
		if (this.m_LightRenderer != null)
		{
			if (!Application.isPlaying)
			{
				this.m_LightRenderer.sharedMaterial?.SetFloat("_Power", value);
			}
			else
			{
				this.m_LightRenderer.material?.SetFloat("_Power", value);
			}
		}
		else if (this.m_SkinnedLightRenderer != null)
		{
			if (!Application.isPlaying)
			{
				this.m_SkinnedLightRenderer.sharedMaterial?.SetFloat("_Power", value);
			}
			else
			{
				this.m_SkinnedLightRenderer.material?.SetFloat("_Power", value);
			}
		}
	}
}
