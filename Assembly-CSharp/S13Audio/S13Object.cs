using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public abstract class S13Object : ScriptableObject, IS13Controllable
{
	public enum VoiceMode
	{
		KillOld = 0,
		IgnoreNew = 1,
		UseClosest = 2
	}

	public enum FollowMode
	{
		Update = 0,
		Static = 1
	}

	protected static HashSet<S13Object> pausedList;

	[TextArea(1, 50)]
	public string note;

	[S13URL]
	public string ticketURL;

	[S13Foldout("Advanced Settings", "S13Object")]
	public bool gamePauseReactive = true;

	[S13Foldout("Advanced Settings", "S13Object")]
	public bool ignoreTimeScale = true;

	[S13Slider("Start Delay (s)", 0f, 5f)]
	[S13Foldout("Advanced Settings", "S13Object")]
	public float startDelay;

	[S13Foldout("Advanced Settings", "S13Object")]
	[S13Slider("Replay Gate (s)", 0f, 60f)]
	public float replayGate;

	[S13Foldout("Advanced Settings", "S13Object")]
	[S13Slider("Chance to Play (%)", 0f, 100f)]
	public float chanceToPlay = 100f;

	public bool isPlaying => S13Manager.IsObjectPlaying(this);

	public bool isPaused => S13Object.pausedList.Contains(this);

	public abstract S13Handler CreateHandler();

	public abstract IEnumerable<S13Object> GetDependencies();

	public void Play()
	{
		S13Manager.Play(this);
	}

	public void Play(Transform parent)
	{
		S13Manager.Play(this, parent);
	}

	public void Play(float duration)
	{
		S13Manager.Play(this, duration);
	}

	public void Play(Transform parent, float duration)
	{
		S13Manager.Play(this, parent, duration);
	}

	public void PlayDelayed(float delayTime)
	{
		S13Manager.PlayDelayed(this, delayTime);
	}

	public void PlayDelayed(Transform parent, float delayTime)
	{
		S13Manager.PlayDelayed(this, parent, delayTime);
	}

	public void PlayDelayed(float delayTime, float duration)
	{
		S13Manager.PlayDelayed(this, delayTime, duration);
	}

	public void PlayDelayed(Transform parent, float delayTime, float duration)
	{
		S13Manager.PlayDelayed(this, parent, delayTime, duration);
	}

	public void Stop()
	{
		S13Manager.Stop(this);
	}

	public void Stop(bool ignoreFades)
	{
		S13Manager.Stop(this, ignoreFades);
	}

	public void StopDelayed(float delayTime)
	{
		S13Manager.StopDelayed(this, delayTime);
	}

	public void StopDelayed(float delayTime, bool ignoreFades)
	{
		S13Manager.StopDelayed(this, delayTime, ignoreFades);
	}

	public void Shutdown()
	{
		S13Manager.Shutdown(this);
	}

	public void Pause()
	{
		if (!this.isPaused)
		{
			S13Manager.Pause(this);
			S13Object.pausedList.Add(this);
		}
	}

	public void Resume()
	{
		if (this.isPaused)
		{
			S13Manager.Resume(this);
			S13Object.pausedList.Remove(this);
		}
	}

	static S13Object()
	{
		S13Object.pausedList = new HashSet<S13Object>();
	}
}
public abstract class S13Object<TObject, THandler> : S13Object where TObject : S13Object<TObject, THandler> where THandler : S13Handler<TObject, THandler>
{
	public sealed override S13Handler CreateHandler()
	{
		THandler val;
		val = new GameObject(base.name).AddComponent<THandler>();
		val.Initialize(this);
		return val;
	}
}
