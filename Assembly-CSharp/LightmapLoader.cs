using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class LightmapLoader : JMonoBehaviour
{
	[Serializable]
	private class SphericalHarmonics
	{
		public float[] coefficients = new float[27];
	}

	[Serializable]
	private class RendererInfo
	{
		public Renderer renderer;

		public int lightmapIndex;

		public Vector4 lightmapOffsetScale;
	}

	[Serializable]
	private class LightingScenarioData
	{
		public LightmapsMode lightmapsMode;

		public RendererInfo[] rendererInfos;

		public Texture2D[] lightmaps;

		public Texture2D[] lightmapsDir;

		public Texture2D[] lightmapsShadow;

		public SphericalHarmonics[] lightProbes;
	}

	[SerializeField]
	private string m_ResourceFolder = "Section_LightingData";

	[SerializeField]
	private LightingScenarioData m_LightingScenariosData;

	[SerializeField]
	private bool m_LoadOnAwake = true;

	private string m_JsonFileName = "lightmapconfig.txt";

	private string m_FinalFilePath;

	public string ResourceFolder => this.m_ResourceFolder;

	public override void Awake()
	{
		if (this.m_LoadOnAwake)
		{
			this.Load();
		}
	}

	public void Load()
	{
		this.m_LightingScenariosData = this.LoadJsonData();
		LightmapData[] array;
		array = new LightmapData[this.m_LightingScenariosData.lightmaps.Length];
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			array[i] = new LightmapData();
			array[i].lightmapColor = Resources.Load<Texture2D>(this.m_ResourceFolder + "/" + this.m_LightingScenariosData.lightmaps[i].name);
			if (this.m_LightingScenariosData.lightmapsMode != LightmapsMode.NonDirectional)
			{
				array[i].lightmapDir = Resources.Load<Texture2D>(this.m_ResourceFolder + "/" + this.m_LightingScenariosData.lightmapsDir[i].name);
				if (i < this.m_LightingScenariosData.lightmapsShadow.Length && this.m_LightingScenariosData.lightmapsShadow[i] != null)
				{
					array[i].shadowMask = Resources.Load<Texture2D>(this.m_ResourceFolder + "/" + this.m_LightingScenariosData.lightmapsShadow[i].name);
				}
			}
		}
		this.LoadLightProbes();
		this.ApplyRendererInfo(this.m_LightingScenariosData.rendererInfos);
		LightmapSettings.lightmaps = array;
	}

	private void LoadLightProbes()
	{
		SphericalHarmonicsL2[] array;
		array = new SphericalHarmonicsL2[this.m_LightingScenariosData.lightProbes.Length];
		unchecked
		{
			for (int i = 0; i < this.m_LightingScenariosData.lightProbes.Length; i++)
			{
				SphericalHarmonicsL2 sphericalHarmonicsL;
				sphericalHarmonicsL = default;
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 9; k++)
					{
						sphericalHarmonicsL[j, k] = this.m_LightingScenariosData.lightProbes[i].coefficients[j * 9 + k];
					}
				}
				array[i] = sphericalHarmonicsL;
			}
			try
			{
				LightmapSettings.lightProbes.bakedProbes = array;
			}
			catch
			{
				Debug.LogWarning("Warning, error when trying to load lightprobes..");
			}
		}
	}

	private void ApplyRendererInfo(RendererInfo[] infos)
	{
		try
		{
			for (int i = 0; i < infos.Length; i = unchecked(i + 1))
			{
				RendererInfo rendererInfo;
				rendererInfo = infos[i];
				if (!(rendererInfo.renderer == null))
				{
					rendererInfo.renderer.lightmapIndex = infos[i].lightmapIndex;
					if (!rendererInfo.renderer.isPartOfStaticBatch)
					{
						rendererInfo.renderer.lightmapScaleOffset = infos[i].lightmapOffsetScale;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error in ApplyRendererInfo:" + ex.GetType().ToString());
		}
	}

	private LightingScenarioData LoadJsonData()
	{
		this.m_FinalFilePath = this.GetResourcesDirectory(this.m_ResourceFolder) + this.m_JsonFileName;
		return this.m_LightingScenariosData = JsonUtility.FromJson<LightingScenarioData>(this.GetJsonFile(this.m_FinalFilePath));
	}

	public string GetResourcesDirectory(string dir)
	{
		return Application.dataPath + "/Resources/LightingData" + dir + "/";
	}

	public bool CheckResourcesDirectoryExists(string dir)
	{
		return Directory.Exists(this.GetResourcesDirectory(dir));
	}

	private void CreateResourcesDirectory(string dir)
	{
		if (!this.CheckResourcesDirectoryExists(this.m_ResourceFolder))
		{
			Directory.CreateDirectory(this.GetResourcesDirectory(dir));
		}
	}

	private string GetJsonFile(string f)
	{
		if (!File.Exists(f))
		{
			return "";
		}
		return File.ReadAllText(f);
	}
}
