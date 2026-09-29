using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AssetManager : JDisposable
{
	private Dictionary<int, Object> m_Assets = new Dictionary<int, Object>();

	private Dictionary<int, Object> m_AssetsNonResourced = new Dictionary<int, Object>();

	private HashSet<int> m_asyncLoadsInProgress = new HashSet<int>();

	public static AssetManager Create()
	{
		return new AssetManager();
	}

	private T InternalCreateAsset<T>(string assetKey) where T : Component
	{
		return Object.Instantiate(this.GetAsset<T>(assetKey));
	}

	private T InternalCreateAsset<T>(Object assetPrefab) where T : Component
	{
		return Object.Instantiate(this.GetAsset<T>((T)assetPrefab));
	}

	private async Task<T> InternalCreateAssetAsync<T>(string assetKey) where T : Component
	{
		return Object.Instantiate(await this.GetAssetAsync<T>(assetKey));
	}

	public T CreateAsset<T>(string assetKey, object data = null) where T : Component
	{
		T component;
		component = this.InternalCreateAsset<T>(assetKey).GetComponent<T>();
		if (component != null)
		{
			if (component is IData data2)
			{
				data2.Load(data);
				data2.Initialize();
			}
			else if (component is IUIElement iUIElement)
			{
				iUIElement.Initialize(data);
			}
			else if (component is IInitializer initializer)
			{
				initializer.Initialize(data);
			}
		}
		else
		{
			Debug.LogWarning("Asset [" + assetKey + "] does not contan component <" + typeof(T).ToString() + ">");
		}
		return component;
	}

	public async Task<T> CreateAssetAsync<T>(string assetKey, object data = null) where T : Component
	{
		T component;
		component = (await this.InternalCreateAssetAsync<T>(assetKey)).GetComponent<T>();
		if (component != null)
		{
			if (component is IData data2)
			{
				data2.Load(data);
				data2.Initialize();
			}
			else if (component is IUIElement iUIElement)
			{
				iUIElement.Initialize(data);
			}
			else if (component is IInitializer initializer)
			{
				initializer.Initialize(data);
			}
		}
		else
		{
			Debug.LogWarning("Asset [" + assetKey + "] does not contan component <" + typeof(T).ToString() + ">");
		}
		return component;
	}

	public T CreateAsset<T>(Object assetPrefab, object data = null) where T : Component
	{
		T component;
		component = this.InternalCreateAsset<T>(assetPrefab).GetComponent<T>();
		if (component != null)
		{
			if (component is IData data2)
			{
				data2.Load(data);
				data2.Initialize();
			}
			else if (component is IUIElement iUIElement)
			{
				iUIElement.Initialize(data);
			}
			else if (component is IInitializer initializer)
			{
				initializer.Initialize(data);
			}
		}
		else
		{
			Debug.LogWarning("Asset [" + assetPrefab.name + "] does not contan component <" + typeof(T).ToString() + ">");
		}
		return component;
	}

	public T GetAsset<T>(string assetKey) where T : Object
	{
		int hashCode;
		hashCode = assetKey.GetHashCode();
		if (!this.m_Assets.ContainsKey(hashCode))
		{
			this.m_Assets.Add(hashCode, Resources.Load<T>(assetKey));
		}
		return (T)this.m_Assets[hashCode];
	}

	public async Task<T> GetAssetAsync<T>(string assetKey) where T : Object
	{
		int hashID;
		hashID = assetKey.GetHashCode();
		if (!this.m_Assets.ContainsKey(hashID))
		{
			if (this.m_asyncLoadsInProgress.Contains(hashID))
			{
				Debug.LogError("Asset [" + assetKey + "] is already being loaded Asynchronously need to fix this");
				return null;
			}
			this.m_asyncLoadsInProgress.Add(hashID);
			ResourceRequest resourceRequest;
			resourceRequest = Resources.LoadAsync<T>(assetKey);
			while (!resourceRequest.isDone)
			{
				await Task.Yield();
			}
			if (resourceRequest.asset == null)
			{
				Debug.LogError("Failed to load " + assetKey);
				return null;
			}
			this.m_Assets.Add(hashID, resourceRequest.asset);
			this.m_asyncLoadsInProgress.Remove(hashID);
		}
		return (T)this.m_Assets[hashID];
	}

	public T GetAsset<T>(Object assetPrefab) where T : Object
	{
		int hashCode;
		hashCode = assetPrefab.GetHashCode();
		if (!this.m_AssetsNonResourced.ContainsKey(hashCode))
		{
			this.m_AssetsNonResourced.Add(hashCode, assetPrefab);
		}
		return (T)this.m_AssetsNonResourced[hashCode];
	}

	public void FreeAsset(string assetKey)
	{
		int hashCode;
		hashCode = assetKey.GetHashCode();
		this.m_Assets.Remove(hashCode);
	}

	public void FreeAsset(Object assetPrefab)
	{
		int hashCode;
		hashCode = assetPrefab.GetHashCode();
		this.m_AssetsNonResourced.Remove(hashCode);
	}

	protected override void OnDisposed()
	{
		if (this.m_Assets != null)
		{
			this.m_Assets.Clear();
			this.m_Assets = null;
		}
		if (this.m_AssetsNonResourced != null)
		{
			this.m_AssetsNonResourced.Clear();
			this.m_AssetsNonResourced = null;
		}
		base.OnDisposed();
	}
}
