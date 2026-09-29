using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerPlaylist : S13HandlerCompound<S13ObjectPlaylist, S13HandlerPlaylist>
{
	private int _curIndex;

	private int _totalPlays;

	protected override void PlayHandler(Transform parent)
	{
		unchecked
		{
			if (this._totalPlays > 0)
			{
				if (base.targetObject.ShouldInterupt(this._curIndex))
				{
					base.subHandlers[this._curIndex].Stop();
				}
				this._curIndex = ++this._curIndex % base.subHandlers.Count;
			}
			base.subHandlers[this._curIndex].Play(parent);
			this._totalPlays++;
		}
	}

	protected override void StopHandler(bool ignoreFades)
	{
		if (base.targetObject.resetPlaylistOnStop)
		{
			this._curIndex = 0;
			this._totalPlays = 0;
		}
		foreach (S13Handler subHandler in base.subHandlers)
		{
			subHandler.Stop(ignoreFades);
		}
	}

	protected override void PauseHandler()
	{
		base.PauseHandler();
		foreach (S13Handler subHandler in base.subHandlers)
		{
			subHandler.Pause();
		}
	}

	protected override void ResumeHandler()
	{
		foreach (S13Handler subHandler in base.subHandlers)
		{
			subHandler.Resume();
		}
	}
}
