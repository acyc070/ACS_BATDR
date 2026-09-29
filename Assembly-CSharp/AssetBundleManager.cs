using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleManager : JDisposable
{
	public Dictionary<string, AssetBundle> AssetBundles { get; private set; }

	public AssetBundle LoadedAssetBundle { get; private set; }

	public bool HasAssetBundle { get; private set; }

	public event EventHandler OnLoaded;

	public AssetBundleManager()
	{
		Caching.ClearCache();
		this.AssetBundles = new Dictionary<string, AssetBundle>();
	}

	public IEnumerator GetAssetBundle(string assetBundleName)
	{
		this.HasAssetBundle = false;
		AssetBundleCreateRequest assetBundleRequest;
		assetBundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, assetBundleName));
		yield return assetBundleRequest;
		this.LoadedAssetBundle = assetBundleRequest.assetBundle;
		if (this.LoadedAssetBundle == null)
		{
			this.HasAssetBundle = false;
		}
		else if (!this.AssetBundles.ContainsKey(assetBundleName))
		{
			this.AssetBundles.Add(assetBundleName, this.LoadedAssetBundle);
			this.HasAssetBundle = true;
		}
		this.OnLoaded.Send(this);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
