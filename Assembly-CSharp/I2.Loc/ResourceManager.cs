using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace I2.Loc;

public class ResourceManager : MonoBehaviour
{
	private static ResourceManager mInstance;

	public List<IResourceManager_Bundles> mBundleManagers = new List<IResourceManager_Bundles>();

	public global::UnityEngine.Object[] Assets;

	private readonly Dictionary<string, global::UnityEngine.Object> mResourcesCache = new Dictionary<string, global::UnityEngine.Object>(StringComparer.Ordinal);

	public static ResourceManager pInstance
	{
		get
		{
			bool flag;
			flag = ResourceManager.mInstance == null;
			if (ResourceManager.mInstance == null)
			{
				ResourceManager.mInstance = (ResourceManager)global::UnityEngine.Object.FindObjectOfType(typeof(ResourceManager));
			}
			if (ResourceManager.mInstance == null)
			{
				GameObject gameObject;
				gameObject = new GameObject("I2ResourceManager", typeof(ResourceManager));
				gameObject.hideFlags |= HideFlags.HideAndDontSave;
				ResourceManager.mInstance = gameObject.GetComponent<ResourceManager>();
				SceneManager.sceneLoaded += ResourceManager.MyOnLevelWasLoaded;
			}
			if (flag && Application.isPlaying)
			{
				global::UnityEngine.Object.DontDestroyOnLoad(ResourceManager.mInstance.gameObject);
			}
			return ResourceManager.mInstance;
		}
	}

	public static void MyOnLevelWasLoaded(Scene scene, LoadSceneMode mode)
	{
		ResourceManager.pInstance.CleanResourceCache();
		LocalizationManager.UpdateSources();
	}

	public T GetAsset<T>(string Name) where T : global::UnityEngine.Object
	{
		T val;
		val = this.FindAsset(Name) as T;
		if (val != null)
		{
			return val;
		}
		return this.LoadFromResources<T>(Name);
	}

	private global::UnityEngine.Object FindAsset(string Name)
	{
		if (this.Assets != null)
		{
			int i;
			i = 0;
			for (int num = this.Assets.Length; i < num; i = unchecked(i + 1))
			{
				if (this.Assets[i] != null && this.Assets[i].name == Name)
				{
					return this.Assets[i];
				}
			}
		}
		return null;
	}

	public bool HasAsset(global::UnityEngine.Object Obj)
	{
		if (this.Assets == null)
		{
			return false;
		}
		return Array.IndexOf(this.Assets, Obj) >= 0;
	}

	public T LoadFromResources<T>(string Path) where T : global::UnityEngine.Object
	{
		unchecked
		{
			try
			{
				if (string.IsNullOrEmpty(Path))
				{
					return null;
				}
				if (this.mResourcesCache.TryGetValue(Path, out var value) && value != null)
				{
					return value as T;
				}
				T val;
				val = null;
				if (Path.EndsWith("]", StringComparison.OrdinalIgnoreCase))
				{
					int num;
					num = Path.LastIndexOf("[", StringComparison.OrdinalIgnoreCase);
					string value2;
					value2 = Path.Substring(num + 1, Path.Length - num - 2);
					Path = Path.Substring(0, num);
					T[] array;
					array = Resources.LoadAll<T>(Path);
					int i;
					i = 0;
					for (int num2 = array.Length; i < num2; i++)
					{
						if (array[i].name.Equals(value2))
						{
							val = array[i];
							break;
						}
					}
				}
				else
				{
					val = Resources.Load(Path, typeof(T)) as T;
				}
				if (val == null)
				{
					val = this.LoadFromBundle<T>(Path);
				}
				if (val != null)
				{
					this.mResourcesCache[Path] = val;
				}
				return val;
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Unable to load {0} '{1}'\nERROR: {2}", typeof(T), Path, ex.ToString());
				return null;
			}
		}
	}

	public T LoadFromBundle<T>(string path) where T : global::UnityEngine.Object
	{
		int i;
		i = 0;
		for (int count = this.mBundleManagers.Count; i < count; i = unchecked(i + 1))
		{
			if (this.mBundleManagers[i] != null)
			{
				T val;
				val = this.mBundleManagers[i].LoadFromBundle(path, typeof(T)) as T;
				if (val != null)
				{
					return val;
				}
			}
		}
		return null;
	}

	public void CleanResourceCache()
	{
		this.mResourcesCache.Clear();
		Resources.UnloadUnusedAssets();
		base.CancelInvoke();
	}
}
