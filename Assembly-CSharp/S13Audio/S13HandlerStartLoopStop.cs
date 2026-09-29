using System.Collections;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerStartLoopStop : S13HandlerCompound<S13ObjectStartLoopStop, S13HandlerStartLoopStop>
{
	private int isPlayingSemaphore;

	private Transform targetParent;

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

	private S13Handler startSegmentHandler
	{
		get
		{
			if (base.subHandlers.Count <= 0)
			{
				return null;
			}
			return base.subHandlers[0];
		}
	}

	private S13Handler loopSegmentHandler
	{
		get
		{
			if (base.subHandlers.Count <= 1)
			{
				return null;
			}
			return base.subHandlers[1];
		}
	}

	private S13Handler stopSegmentHandler
	{
		get
		{
			if (base.subHandlers.Count <= 2)
			{
				return null;
			}
			return base.subHandlers[2];
		}
	}

	protected override void PlayHandler(Transform parent)
	{
		base.PlayHandler(parent);
		this.targetParent = parent;
		base.StartCoroutine(this.PlayRoutine());
	}

	protected override void StopHandler(bool ignoreFades)
	{
		base.StopHandler(ignoreFades);
		base.StopAllCoroutines();
		this.isPlayingSemaphore = 0;
		if (base.targetObject.cutStart && (bool)this.startSegmentHandler)
		{
			this.startSegmentHandler.Stop(ignoreFades);
		}
		if ((bool)this.loopSegmentHandler)
		{
			this.loopSegmentHandler.Stop(ignoreFades);
		}
		if ((bool)this.stopSegmentHandler)
		{
			this.stopSegmentHandler.Play(this.targetParent);
		}
	}

	protected override void ShutdownHandler()
	{
		base.StopAllCoroutines();
		this.isPlayingSemaphore = 0;
		if ((bool)this.startSegmentHandler)
		{
			this.startSegmentHandler.Shutdown();
		}
		if ((bool)this.loopSegmentHandler)
		{
			this.loopSegmentHandler.Shutdown();
		}
	}

	protected override void PauseHandler()
	{
		base.PauseHandler();
		if ((bool)this.startSegmentHandler)
		{
			this.startSegmentHandler.Pause();
		}
		if ((bool)this.loopSegmentHandler)
		{
			this.loopSegmentHandler.Pause();
		}
		if ((bool)this.stopSegmentHandler)
		{
			this.stopSegmentHandler.Pause();
		}
	}

	protected override void ResumeHandler()
	{
		base.ResumeHandler();
		if ((bool)this.startSegmentHandler)
		{
			this.startSegmentHandler.Resume();
		}
		if ((bool)this.loopSegmentHandler)
		{
			this.loopSegmentHandler.Resume();
		}
		if ((bool)this.stopSegmentHandler)
		{
			this.stopSegmentHandler.Resume();
		}
	}

	private IEnumerator PlayRoutine()
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
			if (base.targetObject.cutStop && (bool)this.stopSegmentHandler)
			{
				this.stopSegmentHandler.Stop(ignoreFades: false);
			}
			if ((bool)this.startSegmentHandler)
			{
				this.startSegmentHandler.Play(this.targetParent);
			}
			if (!this.loopSegmentHandler)
			{
				yield break;
			}
			waitTime = base.targetObject.loopDelay;
			while (waitTime > 0f)
			{
				yield return null;
				if (!base.isPaused)
				{
					waitTime -= base.deltaTime;
				}
			}
			this.loopSegmentHandler.Play(this.targetParent);
		}
	}
}
