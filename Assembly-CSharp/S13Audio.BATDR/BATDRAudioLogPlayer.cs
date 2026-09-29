using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio.BATDR;

public class BATDRAudioLogPlayer : MonoBehaviour
{
	private static BATDRAudioLogPlayer _Instance;

	[SerializeField]
	[FormerlySerializedAs("startAction")]
	private S13LocalAction tapeSFXStartAction;

	[SerializeField]
	[FormerlySerializedAs("stopAction")]
	private S13LocalAction tapeSFXStopAction;

	[SerializeField]
	[Tooltip("Delay's the playing of the actual audiolog clip by n seconds when using tape SFX")]
	private float tapeSFXAudiologDelay = 0.5f;

	[Header("Settings")]
	[SerializeField]
	[Tooltip("S13ObjectDynamic so that we can play incoming AudioClips")]
	private S13ObjectDynamic audioLogPlayerObject;

	[FormerlySerializedAs("stopCallbackDelay")]
	[SerializeField]
	[Tooltip("Delay's the closing of the AudioLog UI for n seconds")]
	private float stopUICallbackDelay = 0.5f;

	[SerializeField]
	[Tooltip("Delay's the playing of a new clip after an interrupt by n seconds. This is ignored if enableTapeSFX is false")]
	private float interruptPlayDelay = 0.75f;

	[SerializeField]
	[Tooltip("Should tape SFX be played? Also effects delays. This value can change at runtime")]
	private bool enableTapeSFX;

	public static BATDRAudioLogPlayer Instance
	{
		get
		{
			if (!BATDRAudioLogPlayer._Instance)
			{
				BATDRAudioLogPlayer._Instance = global::UnityEngine.Object.FindObjectOfType<BATDRAudioLogPlayer>();
				if (!BATDRAudioLogPlayer._Instance)
				{
					Debug.LogError("BATDRAudioLogPlayer does not exist within scene. One must be added in this or previously loaded scene for stamp to function");
				}
			}
			return BATDRAudioLogPlayer._Instance;
		}
		set
		{
			BATDRAudioLogPlayer._Instance = value;
		}
	}

	public static void Play(AudioClip clip)
	{
		if (BATDRAudioLogPlayer.Instance.audioLogPlayerObject.isPlaying)
		{
			BATDRAudioLogPlayer.Instance.InterruptHandler(clip);
		}
		else
		{
			BATDRAudioLogPlayer.Instance.PlayHandler(clip);
		}
	}

	public static void Stop()
	{
		BATDRAudioLogPlayer.Instance.StopHandler();
	}

	public static void ForceStop()
	{
		if (BATDRAudioLogPlayer.Instance != null && BATDRAudioLogPlayer.Instance.audioLogPlayerObject.isPlaying)
		{
			BATDRAudioLogPlayer.Instance.audioLogPlayerObject.Stop();
			BATDRAudioLogPlayer.Instance.tapeSFXStopAction.Execute();
		}
	}

	public static void Stop(Action callback)
	{
		if (BATDRAudioLogPlayer.Instance == null)
		{
			callback?.Invoke();
		}
		else
		{
			BATDRAudioLogPlayer.Instance.StartCoroutine(BATDRAudioLogPlayer.Instance.StopAudioLogCoroutine(BATDRAudioLogPlayer.Instance.stopUICallbackDelay, callback));
		}
	}

	public static void EnableTapeSFX(bool enabledTapeSFX)
	{
		BATDRAudioLogPlayer.Instance.enableTapeSFX = enabledTapeSFX;
	}

	private void InterruptHandler(AudioClip clip)
	{
		if (this.enableTapeSFX)
		{
			base.StartCoroutine(this.StopAudioLogCoroutine(this.interruptPlayDelay, () =>
			{
				this.PlayHandler(clip);
			}));
		}
		else
		{
			base.StartCoroutine(this.StopAudioLogCoroutine(0f, () =>
			{
				this.PlayHandler(clip);
			}));
		}
	}

	private void PlayHandler(AudioClip clip)
	{
		S13HandlerDynamic s13HandlerDynamic;
		s13HandlerDynamic = S13Manager.GetGlobalHandler(this.audioLogPlayerObject) as S13HandlerDynamic;
		if (s13HandlerDynamic != null)
		{
			s13HandlerDynamic.SetNextClip(clip);
		}
		if (this.enableTapeSFX)
		{
			if (s13HandlerDynamic != null)
			{
				base.StartCoroutine(S13CoroutineUtil.WaitForDuration(this.tapeSFXAudiologDelay, ignoreTimeScale: true, s13HandlerDynamic.Play));
			}
			if (this.tapeSFXStartAction.IsExecutable)
			{
				this.tapeSFXStartAction.Execute();
			}
		}
		else if (!this.enableTapeSFX && s13HandlerDynamic != null)
		{
			s13HandlerDynamic.Play();
		}
	}

	private IEnumerator StopAudioLogCoroutine(float callbackDelay, Action callback)
	{
		this.StopHandler();
		yield return new WaitForSecondsRealtime(callbackDelay);
		callback?.Invoke();
	}

	private void StopHandler()
	{
		this.audioLogPlayerObject.Stop();
		if (this.enableTapeSFX && this.tapeSFXStopAction.IsExecutable)
		{
			this.tapeSFXStopAction.Execute();
		}
	}
}
