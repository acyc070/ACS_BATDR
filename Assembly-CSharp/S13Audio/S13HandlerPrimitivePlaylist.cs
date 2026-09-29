using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerPrimitivePlaylist : S13HandlerPrimitiveMultiClip<S13ObjectPrimitivePlaylist, S13HandlerPrimitivePlaylist>
{
	private int _curIndex;

	private int _totalPlays;

	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		unchecked
		{
			if (this._totalPlays > 0)
			{
				this._curIndex = ++this._curIndex % base.targetObject.playlistClips.Length;
			}
			base.VoiceClip(base.targetObject.playlistClips[this._curIndex], base.targetObject.loop, parent);
			this._totalPlays++;
		}
	}

	protected override void StopHandler(bool ignoreFades)
	{
		base.StopHandler(ignoreFades);
		if (base.targetObject.resetPlaylistOnStop)
		{
			this._curIndex = 0;
			this._totalPlays = 0;
		}
	}
}
