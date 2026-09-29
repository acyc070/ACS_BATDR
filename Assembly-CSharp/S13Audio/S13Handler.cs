using System;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public abstract class S13Handler : MonoBehaviour
{
	protected float lastPlayTime = float.NegativeInfinity;

	protected Dictionary<int, Coroutine> routines = new Dictionary<int, Coroutine>();

	public abstract bool isPlaying { get; }

	public bool isPaused { get; protected set; }

	protected float currentTime
	{
		get
		{
			if (!this.baseObject.ignoreTimeScale)
			{
				return Time.time;
			}
			return Time.realtimeSinceStartup;
		}
	}

	protected float deltaTime
	{
		get
		{
			if (!this.baseObject.ignoreTimeScale)
			{
				return Time.deltaTime;
			}
			return Time.unscaledDeltaTime;
		}
	}

	public S13Object baseObject { get; set; }

	public abstract int NumActiveVoices { get; }

	private bool CheckAvailability()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			S13Debug.LogWarning("Could not Play on " + base.gameObject.name + ", GameObject is not active in hierarchy", this);
			return false;
		}
		if (this.baseObject.chanceToPlay < 100f && global::UnityEngine.Random.Range(0f, 100f) >= this.baseObject.chanceToPlay)
		{
			return false;
		}
		if (this.lastPlayTime >= this.currentTime - this.baseObject.replayGate)
		{
			return false;
		}
		this.lastPlayTime = this.currentTime;
		return true;
	}

	public abstract void Initialize(S13Object targetObject);

	public void Play()
	{
		this.Play(null);
	}

	public void Play(Transform parent)
	{
		if (this.CheckAvailability())
		{
			this.PlayHandler(parent);
		}
	}

	public void Play(float duration)
	{
		this.Play(null, duration);
	}

	public void Play(Transform parent, float duration)
	{
		if (this.CheckAvailability())
		{
			this.PlayHandler(parent);
			base.StartCoroutine(S13CoroutineUtil.WaitForDuration(duration, this.baseObject.ignoreTimeScale, this.Stop));
		}
	}

	public void PlayDelayed(float delayTime)
	{
		this.PlayDelayed(null, delayTime);
	}

	public void PlayDelayed(Transform parent, float delayTime)
	{
		this.ManagedWaitForDuration(action: () =>
		{
			this.Play(parent);
		}, delayTime: delayTime, ignoreTimeScale: this.baseObject.ignoreTimeScale);
	}

	public void PlayDelayed(float delayTime, float duration)
	{
		this.PlayDelayed(null, delayTime, duration);
	}

	public void PlayDelayed(Transform parent, float delayTime, float duration)
	{
		this.ManagedWaitForDuration(action: () =>
		{
			this.Play(parent, duration);
		}, delayTime: delayTime, ignoreTimeScale: this.baseObject.ignoreTimeScale);
	}

	public void Stop()
	{
		this.Stop(ignoreFades: false);
	}

	public void Stop(bool ignoreFades)
	{
		foreach (KeyValuePair<int, Coroutine> routine in this.routines)
		{
			if (routine.Value != null)
			{
				base.StopCoroutine(routine.Value);
			}
		}
		this.routines.Clear();
		this.StopHandler(ignoreFades);
	}

	public void StopDelayed(float delayTime)
	{
		this.StopDelayed(delayTime, ignoreFades: false);
	}

	public void StopDelayed(float delayTime, bool ignoreFades)
	{
		base.StartCoroutine(S13CoroutineUtil.WaitForDuration(delayTime, this.baseObject.ignoreTimeScale, () =>
		{
			this.Stop(ignoreFades);
		}));
	}

	public void Shutdown()
	{
		this.ShutdownHandler();
	}

	public void Pause()
	{
		this.PauseHandler();
	}

	public void Resume()
	{
		this.ResumeHandler();
	}

	protected abstract void PlayHandler(Transform parent);

	protected abstract void StopHandler(bool ignoreFades);

	protected abstract void ShutdownHandler();

	protected abstract void PauseHandler();

	protected abstract void ResumeHandler();

	private void ManagedWaitForDuration(float delayTime, bool ignoreTimeScale, Action action)
	{
		int playActionHash;
		playActionHash = action.GetHashCode();
		Coroutine value;
		value = base.StartCoroutine(S13CoroutineUtil.WaitForDuration(delayTime, ignoreTimeScale, () =>
		{
			if (this.routines.ContainsKey(playActionHash))
			{
				action();
				this.routines.Remove(playActionHash);
			}
		}));
		this.routines.Add(playActionHash, value);
	}

	public override string ToString()
	{
		return base.name;
	}
}
public abstract class S13Handler<TObject, THandler> : S13Handler where TObject : S13Object<TObject, THandler> where THandler : S13Handler<TObject, THandler>
{
	public TObject targetObject
	{
		get
		{
			return base.baseObject as TObject;
		}
		set
		{
			base.baseObject = value;
		}
	}

	public virtual void Initialize(TObject targetObject)
	{
		if ((bool)targetObject)
		{
			this.targetObject = targetObject;
			S13Manager.OnGamePaused += this.GamePauseHandler;
		}
	}

	public sealed override void Initialize(S13Object targetObject)
	{
		this.Initialize(targetObject as TObject);
	}

	protected override void PlayHandler(Transform parent)
	{
	}

	protected override void StopHandler(bool ignoreFades)
	{
	}

	protected override void ShutdownHandler()
	{
		this.StopHandler(ignoreFades: true);
	}

	protected override void PauseHandler()
	{
		base.isPaused = true;
	}

	protected override void ResumeHandler()
	{
		base.isPaused = false;
	}

	private void GamePauseHandler(bool paused)
	{
		if (this == null)
		{
			S13Manager.OnGamePaused -= this.GamePauseHandler;
		}
		else if (this.targetObject.gamePauseReactive)
		{
			if (paused)
			{
				base.Pause();
			}
			else
			{
				base.Resume();
			}
		}
	}
}
