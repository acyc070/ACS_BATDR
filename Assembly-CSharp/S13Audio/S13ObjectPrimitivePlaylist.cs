using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Sounds/Primitive/Playlist", order = 2)]
public class S13ObjectPrimitivePlaylist : S13ObjectPrimitiveMultiClip<S13ObjectPrimitivePlaylist, S13HandlerPrimitivePlaylist>, IEnumerable<AudioClip>, IEnumerable
{
	public AudioClip[] playlistClips;

	public bool resetPlaylistOnStop;

	public IEnumerator<AudioClip> GetEnumerator()
	{
		foreach (AudioClip item in this.playlistClips.Where((AudioClip x) => x))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (AudioClip item in this.playlistClips.Where((AudioClip x) => x))
		{
			yield return item;
		}
	}
}
