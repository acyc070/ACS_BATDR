using System;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio;

public class S13AudioHandler : MonoBehaviour, IS13Controllable
{
	private enum S13AudioHandlerScope
	{
		Global = 0,
		Local = 1
	}

	[Serializable]
	private class ControlGroupList : ReorderableArray<ControlGroupEntry>
	{
	}

	[Serializable]
	private class ControlGroupEntry : IS13HorizontalPropertyHolder
	{
		public enum ControlGroupMode
		{
			CheckAndRegister = 0,
			JustCheck = 1
		}

		public ControlGroupMode mode;

		public S13ScriptableControlGroup controlGroup;

		public bool RequestPlay(S13AudioHandler audioHandler)
		{
			bool shouldRegister;
			shouldRegister = this.mode == ControlGroupMode.CheckAndRegister;
			if (this.controlGroup == null)
			{
				return true;
			}
			return this.controlGroup.RequestPlay(audioHandler, shouldRegister);
		}
	}

	private enum EnableDisableOptions
	{
		None = 0,
		PlayOnEnable = 1,
		StopOnDisable = 2,
		PlayOnDisable = 4,
		PlayOnEnableStopOnDisable = 3
	}

	private static class S13HandlerCreatorUtility
	{
		private const int MAX_LOCAL_HANDLER_DEPTH = 10;

		public static S13Handler GenerateHandler(S13Object targetObj, Transform parent)
		{
			return S13HandlerCreatorUtility.GenerateHandler(targetObj, parent, 0);
		}

		private static S13Handler GenerateHandler(S13Object targetObj, Transform parent, int depth)
		{
			if (targetObj == null)
			{
				S13Debug.LogError($"Null S13Object encountered. Please assign one. Parent: {parent}", parent);
				return null;
			}
			if (depth > 10)
			{
				S13Handler component;
				component = parent.GetComponent<S13Handler>();
				S13Debug.LogError("Hit MAX_LOCAL_HANDLER_DEPTH. This error is to prevent a crash. Check " + component.name + " for cycle.", component.baseObject);
				return null;
			}
			S13Handler s13Handler;
			s13Handler = targetObj.CreateHandler();
			s13Handler.transform.SetParent(parent);
			s13Handler.transform.localPosition = Vector3.zero;
			if (s13Handler is IS13HandlerCompound iS13HandlerCompound && targetObj is IS13ObjectCompound { ForceGlobalSubHandlers: var forceGlobalSubHandlers })
			{
				foreach (S13Object dependency in targetObj.GetDependencies())
				{
					S13Handler s13Handler2;
					s13Handler2 = (forceGlobalSubHandlers ? S13Manager.GetGlobalHandler(dependency) : S13HandlerCreatorUtility.GenerateHandler(dependency, s13Handler.transform, depth = unchecked(depth + 1)));
					if (!(s13Handler2 == null))
					{
						iS13HandlerCompound.RegisterSubHandler(s13Handler2);
					}
				}
			}
			return s13Handler;
		}
	}

	[Tooltip("The sound object to load and control.")]
	[SerializeField]
	private S13Object obj;

	[SerializeField]
	private S13AudioHandlerScope scope;

	[Tooltip("Should this S13AudioHandler do anything OnEnable or OnDisable")]
	[FormerlySerializedAs("onEnableOrDisableOptions")]
	[SerializeField]
	private EnableDisableOptions quickPlay;

	[SerializeField]
	[Reorderable]
	private ControlGroupList controlGroups = new ControlGroupList();

	private S13Handler _localHandler;

	private bool hasRunStart;

	private S13Handler handler
	{
		get
		{
			if (this.scope == S13AudioHandlerScope.Global)
			{
				return S13Manager.GetGlobalHandler(this.obj);
			}
			if (!this._localHandler)
			{
				this.CreateHandler();
			}
			return this._localHandler;
		}
	}

	private void Start()
	{
		if (this.scope == S13AudioHandlerScope.Local && !this._localHandler)
		{
			this.CreateHandler();
		}
		this.hasRunStart = true;
		this.TryQuickPlayOnEnable();
	}

	private void OnEnable()
	{
		this.TryQuickPlayOnEnable();
	}

	private void OnDisable()
	{
		this.TryQuickPlayOnDisable();
	}

	private void CreateHandler()
	{
		if (this.obj == null)
		{
			S13Debug.LogWarning("No S13Object set on AudioHandler " + base.name + ". Please assign one");
		}
		else
		{
			this._localHandler = S13HandlerCreatorUtility.GenerateHandler(this.obj, base.transform);
		}
	}

	private void TryQuickPlayOnEnable()
	{
		if (this.hasRunStart && this.quickPlay.HasFlag(EnableDisableOptions.PlayOnEnable))
		{
			S13Debug.Log("Play On Enable " + base.name + ". Playing: " + this.handler.name, this);
			this.Play();
		}
	}

	private void TryQuickPlayOnDisable()
	{
		if (!this.hasRunStart || S13Manager.IsQuitting)
		{
			return;
		}
		if (this.quickPlay.HasFlag(EnableDisableOptions.StopOnDisable))
		{
			S13Debug.Log("Stop On Disable " + base.name + ". Stopping: " + this.handler.name, this);
			this.Stop();
		}
		else if (this.quickPlay.HasFlag(EnableDisableOptions.PlayOnDisable))
		{
			S13Debug.Log("Play On Disable " + base.name + ". Playing: " + this.handler.name, this);
			if (this.scope == S13AudioHandlerScope.Local && this.obj is IS13ObjectCompound { ForceGlobalSubHandlers: false })
			{
				S13Debug.LogWarning("Attempting to PlayOnDisable a local S13Handler with ForceGlobalSubHandlers: False. This may not succeed");
			}
			this.Play();
		}
	}

	private bool RequestControlGroupPlay()
	{
		if (this.controlGroups.Length == 0)
		{
			return true;
		}
		if (this.GetHandler() != null)
		{
			return this.controlGroups.All((ControlGroupEntry controlGroup) => controlGroup.RequestPlay(this));
		}
		return false;
	}

	public void Play()
	{
		if (!base.enabled)
		{
			S13Debug.Log("Could not Play on " + base.gameObject.name + ", S13AudioHandler is not enabled");
		}
		else if (this.RequestControlGroupPlay())
		{
			if (this.scope == S13AudioHandlerScope.Global)
			{
				S13Manager.Play(this.obj, base.transform);
			}
			else
			{
				this.handler?.Play(base.transform);
			}
		}
	}

	public void Play(float duration)
	{
		if (!base.enabled)
		{
			S13Debug.Log("Could not Play on " + base.gameObject.name + ", S13AudioHandler is not enabled");
		}
		else if (this.RequestControlGroupPlay())
		{
			if (this.scope == S13AudioHandlerScope.Global)
			{
				S13Manager.Play(this.obj, base.transform, duration);
			}
			else
			{
				this.handler?.Play(base.transform, duration);
			}
		}
	}

	public void PlayDelayed(float delayTime)
	{
		if (!base.enabled)
		{
			S13Debug.Log("Could not Play on " + base.gameObject.name + ", S13AudioHandler is not enabled");
		}
		else if (this.RequestControlGroupPlay())
		{
			if (this.scope == S13AudioHandlerScope.Global)
			{
				S13Manager.PlayDelayed(this.obj, base.transform, delayTime);
			}
			else
			{
				this.handler?.PlayDelayed(base.transform, delayTime);
			}
		}
	}

	public void PlayDelayed(float delayTime, float duration)
	{
		if (!base.enabled)
		{
			S13Debug.Log("Could not Play on " + base.gameObject.name + ", S13AudioHandler is not enabled");
		}
		else if (this.RequestControlGroupPlay())
		{
			if (this.scope == S13AudioHandlerScope.Global)
			{
				S13Manager.PlayDelayed(this.obj, base.transform, delayTime, duration);
			}
			else
			{
				this.handler?.PlayDelayed(base.transform, delayTime, duration);
			}
		}
	}

	public void Stop()
	{
		if (this.scope == S13AudioHandlerScope.Global)
		{
			S13Manager.Stop(this.obj);
		}
		else
		{
			this.handler?.Stop();
		}
	}

	public void Stop(bool ignoreFades)
	{
		if (this.scope == S13AudioHandlerScope.Global)
		{
			S13Manager.Stop(this.obj, ignoreFades);
		}
		else
		{
			this.handler?.Stop(ignoreFades);
		}
	}

	public void StopDelayed(float delayTime)
	{
		if (this.scope == S13AudioHandlerScope.Global)
		{
			S13Manager.StopDelayed(this.obj, delayTime);
		}
		else
		{
			this.handler?.StopDelayed(delayTime);
		}
	}

	public void StopDelayed(float delayTime, bool ignoreFades)
	{
		if (this.scope == S13AudioHandlerScope.Global)
		{
			S13Manager.StopDelayed(this.obj, delayTime, ignoreFades);
		}
		else
		{
			this.handler?.StopDelayed(delayTime, ignoreFades);
		}
	}

	public void Shutdown()
	{
		if (this.scope == S13AudioHandlerScope.Global)
		{
			S13Manager.Shutdown(this.obj);
		}
		else
		{
			this.handler?.Shutdown();
		}
	}

	public void Pause()
	{
		if (this.scope == S13AudioHandlerScope.Global)
		{
			S13Manager.Pause(this.obj);
		}
		else
		{
			this.handler?.Pause();
		}
	}

	public void Resume()
	{
		if (this.scope == S13AudioHandlerScope.Global)
		{
			S13Manager.Resume(this.obj);
		}
		else
		{
			this.handler?.Resume();
		}
	}

	public S13Handler GetHandler()
	{
		return this.handler;
	}

	public bool IsSetLocal()
	{
		return this.scope == S13AudioHandlerScope.Local;
	}

	public S13Object GetObject()
	{
		return this.obj;
	}
}
