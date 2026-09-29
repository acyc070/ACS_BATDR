using System.Collections;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerContainer : S13HandlerCompound<S13ObjectContainer, S13HandlerContainer>
{
	private int isPlayingSemaphore;

	public override bool isPlaying
	{
		get
		{
			if (this.isPlayingSemaphore <= 0)
			{
				return base.isPlaying;
			}
			return true;
		}
	}

	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		base.StartCoroutine(this.PlayRoutine(parent));
	}

	protected override void StopHandler(bool ignoreFades)
	{
		base.StopHandler(ignoreFades);
		base.StopAllCoroutines();
		this.isPlayingSemaphore = 0;
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
		base.ResumeHandler();
		foreach (S13Handler subHandler in base.subHandlers)
		{
			subHandler.Resume();
		}
	}

	private IEnumerator PlayRoutine(Transform parent)
	{
		unchecked
		{
			this.isPlayingSemaphore++;
			float waitTime;
			waitTime = base.targetObject.startDelay;
			while (waitTime > 0f)
			{
				yield return null;
				if (!base.isPaused)
				{
					waitTime -= base.deltaTime;
				}
			}
			this.isPlayingSemaphore--;
			foreach (S13Handler subHandler in base.subHandlers)
			{
				subHandler.Play(parent);
			}
		}
	}
}
