using System;
using UnityEngine;

namespace I2.Loc;

public interface IResourceManager_Bundles
{
	global::UnityEngine.Object LoadFromBundle(string path, Type assetType);
}
