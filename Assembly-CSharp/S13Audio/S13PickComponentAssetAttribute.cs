using UnityEngine;

namespace S13Audio;

public class S13PickComponentAssetAttribute : PropertyAttribute
{
	public string searchInFolders;

	public S13PickComponentAssetAttribute(string searchInFolders)
	{
		this.searchInFolders = searchInFolders;
	}
}
