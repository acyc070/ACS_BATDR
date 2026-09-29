using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Compound/Playlist", order = 0)]
public class S13ObjectPlaylist : S13ObjectCompound<S13ObjectPlaylist, S13HandlerPlaylist>, IEnumerable<S13Object>, IEnumerable
{
	[Serializable]
	private class PlaylistItemList : ReorderableArray<PlaylistItem>
	{
	}

	[Serializable]
	[S13HorizontalPropertyHolder(forceExpand = true)]
	private class PlaylistItem : IS13HorizontalPropertyHolder
	{
		public S13Object obj;

		public S13CompoundInterruptMode interruptMode;
	}

	[SerializeField]
	[Reorderable(surrogateType = new Type[] { typeof(S13Object) }, surrogateProperty = new string[] { "obj" })]
	[Tooltip("Items to be played in order.")]
	private PlaylistItemList playlistItems;

	public bool resetPlaylistOnStop;

	public bool ShouldInterupt(int index)
	{
		if (index < this.playlistItems.Count)
		{
			return this.playlistItems[index].interruptMode == S13CompoundInterruptMode.Stop;
		}
		return false;
	}

	public override IEnumerable<S13Object> GetDependencies()
	{
		return this.playlistItems.Select((PlaylistItem x) => x.obj);
	}

	public IEnumerator<S13Object> GetEnumerator()
	{
		foreach (S13Object item in this.playlistItems.Select((PlaylistItem x) => x.obj))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (PlaylistItem item in this.playlistItems.Where((PlaylistItem x) => x.obj))
		{
			yield return item;
		}
	}
}
