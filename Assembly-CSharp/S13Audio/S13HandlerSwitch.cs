using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13HandlerSwitch : S13HandlerCompound<S13ObjectSwitch, S13HandlerSwitch>, IS13Switchable
{
	private int isPlayingSemaphore;

	private Dictionary<S13ScriptableSwitch, S13Object> _unmatchedQueue;

	private Dictionary<S13ScriptableSwitch, S13Handler> switchDictionary = new Dictionary<S13ScriptableSwitch, S13Handler>();

	private Transform _lastParent;

	private S13ScriptableSwitch _curScriptableSwitch;

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

	private S13Handler CurrentHandler
	{
		get
		{
			if (!(this.CurrentScriptableSwitch != null))
			{
				return null;
			}
			return this.switchDictionary[this.CurrentScriptableSwitch];
		}
	}

	public S13ScriptableSwitch CurrentScriptableSwitch
	{
		get
		{
			if (!this._curScriptableSwitch)
			{
				this._curScriptableSwitch = base.targetObject.GetDefaultState();
				if (!this.UsesScriptableSwitch(this._curScriptableSwitch))
				{
					this._curScriptableSwitch = null;
				}
			}
			return this._curScriptableSwitch;
		}
	}

	private void Awake()
	{
		S13Manager.RegisterSwitch(this, this.SetScriptableSwitch);
	}

	private void OnDestroy()
	{
		S13Manager.DeRegisterSwitch(this, this.SetScriptableSwitch);
	}

	public override void Initialize(S13ObjectSwitch targetObject)
	{
		base.Initialize(targetObject);
		this._unmatchedQueue = targetObject.GetEntries();
	}

	public override void RegisterSubHandler(S13Handler subHandler)
	{
		base.RegisterSubHandler(subHandler);
		if (this._unmatchedQueue.ContainsValue(subHandler.baseObject))
		{
			KeyValuePair<S13ScriptableSwitch, S13Object> keyValuePair;
			keyValuePair = this._unmatchedQueue.FirstOrDefault((KeyValuePair<S13ScriptableSwitch, S13Object> x) => x.Value == subHandler.baseObject);
			this._unmatchedQueue.Remove(keyValuePair.Key);
			this.switchDictionary.Add(keyValuePair.Key, subHandler);
		}
		else
		{
			S13Debug.LogWarning("Could not Register SubHandler " + subHandler.name + " on S13HandlerSwitch " + base.name, this);
		}
	}

	protected override void PlayHandler(Transform parent)
	{
		S13Handler currentHandler;
		currentHandler = this.CurrentHandler;
		if (!currentHandler)
		{
			S13Debug.LogWarning($"Attempted to play S13HandlerSwitch {this} pointing to null value. Was one of the switch states unloaded?");
			return;
		}
		base.PlayHandler(parent);
		this._lastParent = parent;
		base.StartCoroutine(this.PlayRoutine(parent, currentHandler));
	}

	protected override void StopHandler(bool ignoreFades)
	{
		if (!this.CurrentHandler)
		{
			return;
		}
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
		S13Handler currentHandler;
		currentHandler = this.CurrentHandler;
		if ((bool)currentHandler)
		{
			base.PauseHandler();
			currentHandler.Pause();
		}
	}

	protected override void ResumeHandler()
	{
		S13Handler currentHandler;
		currentHandler = this.CurrentHandler;
		if ((bool)currentHandler)
		{
			base.ResumeHandler();
			currentHandler.Resume();
		}
	}

	private IEnumerator PlayRoutine(Transform parent, S13Handler playTarget)
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
			playTarget.Play(parent);
		}
	}

	public bool UsesScriptableSwitch(S13ScriptableSwitch scriptableSwitch)
	{
		if (base.targetObject.UsesScriptableSwitch(scriptableSwitch))
		{
			return this.switchDictionary.ContainsKey(scriptableSwitch);
		}
		return false;
	}

	public void SetScriptableSwitch(S13ScriptableSwitch scriptableSwitch)
	{
		if (!this.UsesScriptableSwitch(scriptableSwitch))
		{
			S13Debug.LogWarning("Attempted to set " + base.name + " to switch " + scriptableSwitch.name + ", which this switch does not use or is not loaded!");
			return;
		}
		S13Handler currentHandler;
		currentHandler = this.CurrentHandler;
		_ = this.switchDictionary[scriptableSwitch];
		if (!(this._curScriptableSwitch == scriptableSwitch))
		{
			S13Debug.Log(base.name + " set to " + scriptableSwitch.name, S13Debug.LogType.Switch);
			if (base.targetObject.ShouldInterupt(this._curScriptableSwitch) && currentHandler.isPlaying)
			{
				S13Debug.Log(base.name + " interrupting " + currentHandler.name, S13Debug.LogType.Switch);
				base.StopAllCoroutines();
				currentHandler.Stop();
				this.isPlayingSemaphore = 0;
			}
			this._curScriptableSwitch = scriptableSwitch;
			if (base.targetObject.PlayOnSwitch)
			{
				S13Debug.Log(base.name + " PlayOnSwitch " + this.CurrentHandler.name, S13Debug.LogType.Switch);
				this.PlayHandler(this._lastParent);
			}
		}
	}
}
