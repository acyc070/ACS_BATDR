using System;
using System.Collections.Generic;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

[DefaultExecutionOrder(-100)]
public class S13Manager : MonoBehaviour
{
	private class HandlerData
	{
		public S13Handler handler;

		public Dictionary<S13SoundBank, int> referencedSoundBanks = new Dictionary<S13SoundBank, int>();
	}

	public class AccessorRegistrar : Dictionary<string, Action<float>>
	{
	}

	public class SwitchRegistrar : Dictionary<IS13Switchable, Action<S13ScriptableSwitch>>
	{
	}

	private static S13Manager _Instance;

	private static bool Quitting;

	private static bool ManagerLoaded;

	[Tooltip("The set of sound banks to load automatically.")]
	[SerializeField]
	private S13SoundBank[] defaultSoundBanks = new S13SoundBank[0];

	[SerializeField]
	private bool SetDontDestroyOnLoad = true;

	[SerializeField]
	private bool enableRuntimeSoundbank;

	[Tooltip("This Action runs once the S13Manager is fully loaded")]
	[SerializeField]
	private S13GlobalActionGroup managerIsReadyCommand;

	[Header("S13AudioDevActions")]
	[Tooltip("These actions are only run if S13Settings::AudioDevMode is enabled, and will never run in builds")]
	[SerializeField]
	private S13GlobalActionGroup[] audioDevCommands = new S13GlobalActionGroup[0];

	[Header("State Transitions")]
	[SerializeField]
	[Tooltip("The current S13ScriptableState of the game, as Set by S13Manager.SetScriptableState()")]
	private S13ScriptableState currentScriptableState;

	[SerializeField]
	[Reorderable]
	[Tooltip("List of transitions between game states and corresponding actions to run")]
	private S13StateTransitionList stateTransitionList;

	private Dictionary<S13ScriptableControlGroup, S13ControlGroupHandler> controlGroups = new Dictionary<S13ScriptableControlGroup, S13ControlGroupHandler>();

	private Transform _controlGroupHolder;

	private Dictionary<S13Object, HandlerData> handlers = new Dictionary<S13Object, HandlerData>();

	private Transform _handlerHolder;

	private S13SoundBank _runtimeSoundBank;

	private AudioListener _audioListener;

	private AccessorRegistrar accessorDelegates = new AccessorRegistrar();

	private SwitchRegistrar switchDelegates = new SwitchRegistrar();

	private bool _gamePaused;

	private S13ScriptableState previousState;

	public static S13Manager Instance
	{
		get
		{
			if (S13Manager.Quitting)
			{
				return null;
			}
			if (!S13Manager._Instance)
			{
				S13Manager._Instance = global::UnityEngine.Object.FindObjectOfType<S13Manager>();
				if (!S13Manager._Instance)
				{
					Debug.LogError("S13Manager does not exist within scene. One must be added in this or previously loaded scene for stamp to function");
				}
			}
			return S13Manager._Instance;
		}
	}

	public static bool IsQuitting => S13Manager.Quitting;

	public static bool IsManagerLoaded => S13Manager.ManagerLoaded;

	private Transform controlGroupHolder
	{
		get
		{
			if (!this._controlGroupHolder)
			{
				this._controlGroupHolder = new GameObject("ControlGroups").transform;
				this._controlGroupHolder.SetParent(base.transform);
			}
			return this._controlGroupHolder;
		}
	}

	private Transform handlerHolder
	{
		get
		{
			if (!this._handlerHolder)
			{
				this._handlerHolder = new GameObject("Handlers").transform;
				this._handlerHolder.SetParent(base.transform);
			}
			return this._handlerHolder;
		}
	}

	private S13SoundBank runtimeSoundBank
	{
		get
		{
			if (!this._runtimeSoundBank)
			{
				this._runtimeSoundBank = ScriptableObject.CreateInstance<S13SoundBank>();
				this._runtimeSoundBank.name = "RuntimeSoundBank";
			}
			return this._runtimeSoundBank;
		}
	}

	private S13Handler this[S13Object obj]
	{
		get
		{
			if (!obj)
			{
				return null;
			}
			if (!this.handlers.TryGetValue(obj, out var value))
			{
				if (this.enableRuntimeSoundbank && (bool)this.runtimeSoundBank)
				{
					S13Debug.LogWarning("S13Object '" + obj.name + "' has not been loaded from a SoundBank. Performing runtime load");
					return this.LoadObject(this.runtimeSoundBank, obj);
				}
				S13Debug.LogError("S13Object '" + obj.name + "' has not been loaded!");
				return null;
			}
			return value.handler;
		}
	}

	private AudioListener audioListener
	{
		get
		{
			if (!this._audioListener)
			{
				AudioListener audioListener;
				audioListener = global::UnityEngine.Object.FindObjectOfType<AudioListener>();
				if (audioListener == null)
				{
					S13Debug.LogError("Could not find instance of AudioListener in scene.");
					return null;
				}
				this._audioListener = audioListener;
			}
			return this._audioListener;
		}
	}

	public static event Action<bool> OnGamePaused;

	public static event Action OnS13ManagerLoad;

	private void Awake()
	{
		if ((bool)S13Manager._Instance && S13Manager._Instance != this)
		{
			Debug.LogError("Multiple S13Mangers in scene");
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		S13Manager._Instance = this;
		if (this.SetDontDestroyOnLoad)
		{
			global::UnityEngine.Object.DontDestroyOnLoad(this);
		}
		S13SoundBank[] array;
		array = this.defaultSoundBanks;
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			S13Manager.LoadSoundBank(array[i]);
		}
		S13Manager.ManagerLoaded = true;
		S13Manager.OnS13ManagerLoad?.Invoke();
		if (this.managerIsReadyCommand != null)
		{
			this.managerIsReadyCommand.Execute();
		}
	}

	private void Start()
	{
	}

	private void OnApplicationQuit()
	{
		S13Manager.Quitting = true;
	}

	public static bool IsGamePaused()
	{
		if (S13Manager.Quitting)
		{
			return false;
		}
		if (!S13Manager._Instance)
		{
			return false;
		}
		return S13Manager.Instance._gamePaused;
	}

	public static bool IsObjectPlaying(S13Object obj)
	{
		if (S13Manager.Quitting)
		{
			return false;
		}
		if (!S13Manager._Instance)
		{
			return false;
		}
		return S13Manager.Instance._IsObjectPlaying(obj);
	}

	public static bool IsObjectLoaded(S13Object obj)
	{
		if (S13Manager.Quitting)
		{
			return false;
		}
		if (!S13Manager._Instance)
		{
			return false;
		}
		return S13Manager.Instance._IsObjectLoaded(obj);
	}

	public static void LoadSoundBank(S13SoundBank soundBank)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._LoadSoundBank(soundBank);
		}
	}

	public static void UnloadSoundBank(S13SoundBank soundBank)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._UnloadSoundBank(soundBank);
		}
	}

	public static void UnloadAllSoundBanks()
	{
		if (!S13Manager.Quitting)
		{
			S13SoundBank[] array;
			array = S13Manager.Instance.defaultSoundBanks;
			foreach (S13SoundBank soundBank in array)
			{
				S13Manager.Instance._UnloadSoundBank(soundBank);
			}
		}
	}

	public static void UnloadSoundBankDelayed(S13SoundBank soundBank, float delayTime)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._UnloadSoundBankDelayed(soundBank, delayTime);
		}
	}

	public static void StopSoundBank(S13SoundBank soundBank, bool ignoreFades = false)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._StopSoundBank(soundBank, ignoreFades);
		}
	}

	public static void Play(S13Object obj)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._Play(obj);
		}
	}

	public static void Play(S13Object obj, Transform parent)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._Play(obj, parent);
		}
	}

	public static void Play(S13Object obj, float duration)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._Play(obj, duration);
		}
	}

	public static void Play(S13Object obj, Transform parent, float duration)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._Play(obj, parent, duration);
		}
	}

	public static void PlayDelayed(S13Object obj, float delayTime)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._PlayDelayed(obj, delayTime);
		}
	}

	public static void PlayDelayed(S13Object obj, Transform parent, float delayTime)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._PlayDelayed(obj, parent, delayTime);
		}
	}

	public static void PlayDelayed(S13Object obj, float delayTime, float duration)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._PlayDelayed(obj, delayTime, duration);
		}
	}

	public static void PlayDelayed(S13Object obj, Transform parent, float delayTime, float duration)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._PlayDelayed(obj, parent, delayTime, duration);
		}
	}

	public static void Stop(S13Object obj)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._Stop(obj);
		}
	}

	public static void Stop(S13Object obj, bool ignoreFades)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._Stop(obj, ignoreFades);
		}
	}

	public static void StopDelayed(S13Object obj, float delayTime)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._StopDelayed(obj, delayTime);
		}
	}

	public static void StopDelayed(S13Object obj, float delayTime, bool ignoreFades)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._StopDelayed(obj, delayTime, ignoreFades);
		}
	}

	public static void StopAll(bool ignoreFades)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._StopAll(ignoreFades);
		}
	}

	public static void Shutdown(S13Object obj)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._Shutdown(obj);
		}
	}

	public static void Pause(S13Object obj)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._Pause(obj);
		}
	}

	public static void Resume(S13Object obj)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._Resume(obj);
		}
	}

	public static void PauseGame()
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._PauseGame();
		}
	}

	public static void ResumeGame()
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._ResumeGame();
		}
	}

	public static void RegisterSwitch(IS13Switchable switchable, Action<S13ScriptableSwitch> listener)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._RegisterSwitch(switchable, listener);
		}
	}

	public static void DeRegisterSwitch(IS13Switchable switchable, Action<S13ScriptableSwitch> listener)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._DeRegisterSwitch(switchable, listener);
		}
	}

	public static void SetSwitch(IS13Switchable switchable, S13ScriptableSwitch scriptableSwitchValue)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._SetSwitch(switchable, scriptableSwitchValue);
		}
	}

	public static void SetSwitch(S13ObjectSwitch objectSwitch, S13ScriptableSwitch scriptableSwitchValue)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._SetSwitch(objectSwitch, scriptableSwitchValue);
		}
	}

	public static void SetSwitch(S13ScriptableSwitch scriptableSwitchValue)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._SetSwitch(scriptableSwitchValue);
		}
	}

	public static AudioListener GetAudioListener()
	{
		if (S13Manager.Quitting)
		{
			return null;
		}
		return S13Manager.Instance._GetAudioListener();
	}

	public static S13Handler GetGlobalHandler(S13Object obj)
	{
		if (S13Manager.Quitting)
		{
			return null;
		}
		return S13Manager.Instance._GetGlobalHandler(obj);
	}

	public static void RegisterAccessorListener(string accessorID, Action<float> listener)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._RegisterAccessorListener(accessorID, listener);
		}
	}

	public static void DeRegisterAccessorListener(string accessorID, Action<float> listener)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._DeRegisterAccessorListener(accessorID, listener);
		}
	}

	public static void SetAccessor(string accessorID, float value)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._SetAccessor(accessorID, value);
		}
	}

	public static S13ControlGroupHandler GetControlGroupHandler(S13ScriptableControlGroup controlGroup)
	{
		if (S13Manager.Quitting)
		{
			return null;
		}
		return S13Manager.Instance._GetControlGroupHandler(controlGroup);
	}

	public static void SetScriptableState(S13ScriptableState newState)
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._SetScriptableState(newState);
		}
	}

	public static void SetScriptableStateToPrevious()
	{
		if (!S13Manager.Quitting)
		{
			S13Manager.Instance._SetScriptableState(S13Manager.Instance.previousState);
		}
	}

	private bool _IsObjectPlaying(S13Object obj)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if (!s13Handler)
		{
			return false;
		}
		return s13Handler.isPlaying;
	}

	private void _LoadSoundBank(S13SoundBank soundBank)
	{
		if (!soundBank)
		{
			return;
		}
		foreach (S13Object item in soundBank)
		{
			this.LoadObject(soundBank, item);
		}
	}

	private void _UnloadSoundBank(S13SoundBank soundBank)
	{
		if (!soundBank)
		{
			return;
		}
		foreach (S13Object item in soundBank)
		{
			this.UnloadObject(soundBank, item);
		}
	}

	private void _UnloadSoundBankDelayed(S13SoundBank soundBank, float delayTime)
	{
		base.StartCoroutine(S13CoroutineUtil.WaitForDuration(delayTime, ignoreTimeScale: true, () =>
		{
			this._UnloadSoundBank(soundBank);
		}));
	}

	private void _StopSoundBank(S13SoundBank soundBank, bool ignoreFades)
	{
		if (!soundBank)
		{
			return;
		}
		foreach (S13Object item in soundBank)
		{
			HandlerData value;
			if (item == null)
			{
				S13Debug.LogWarning("StopSoundBank: Hit null S13Object in soundbank " + soundBank.name, item);
			}
			else if (this.handlers.TryGetValue(item, out value) && value.handler.isPlaying)
			{
				value.handler.Stop(ignoreFades);
			}
		}
	}

	private S13Handler LoadObject(S13SoundBank soundBank, S13Object obj)
	{
		if (soundBank == null || obj == null)
		{
			S13Debug.LogWarning("Attempted to load null S13Obj or soundbank");
			return null;
		}
		bool flag;
		flag = false;
		if (!this.handlers.TryGetValue(obj, out var value))
		{
			value = new HandlerData();
			this.handlers[obj] = value;
			S13Handler s13Handler;
			s13Handler = obj.CreateHandler();
			s13Handler.transform.SetParent(this.handlerHolder);
			value.handler = s13Handler;
			flag = true;
		}
		this.AddObject(soundBank, value);
		if (value.handler is IS13HandlerCompound iS13HandlerCompound)
		{
			foreach (S13Object dependency in obj.GetDependencies())
			{
				if (dependency == null)
				{
					S13Debug.LogWarning("LoadObject: S13Object " + obj.name + " has null dependency in soundbank " + soundBank.name, obj);
					continue;
				}
				S13Handler subHandler;
				subHandler = this.LoadObject(soundBank, dependency);
				if (flag)
				{
					iS13HandlerCompound.RegisterSubHandler(subHandler);
				}
			}
		}
		return value.handler;
	}

	private void UnloadObject(S13SoundBank soundBank, S13Object obj)
	{
		if (!this.handlers.TryGetValue(obj, out var value))
		{
			return;
		}
		if (!value.referencedSoundBanks.ContainsKey(soundBank))
		{
			S13Debug.LogWarning(soundBank.name + " is not referenced by " + value.handler.name + ", aborting unload of " + value.handler.name, soundBank, S13Debug.LogType.Soundbank);
			return;
		}
		if (this.RemoveObject(soundBank, value) <= 0 && value.referencedSoundBanks.Count <= 0)
		{
			S13Debug.Log(value.handler.name + " has no remaining references and is being unloaded.", soundBank, S13Debug.LogType.Soundbank);
			this.handlers.Remove(obj);
			global::UnityEngine.Object.Destroy(value.handler.gameObject);
		}
		foreach (S13Object dependency in obj.GetDependencies())
		{
			if (dependency == null)
			{
				S13Debug.LogWarning("UnloadObject: S13Object " + obj.name + " has null dependency in soundbank " + soundBank.name, obj);
			}
			else
			{
				this.UnloadObject(soundBank, dependency);
			}
		}
	}

	private int AddObject(S13SoundBank soundBank, HandlerData data)
	{
		unchecked
		{
			if (data.referencedSoundBanks.ContainsKey(soundBank))
			{
				data.referencedSoundBanks[soundBank]++;
			}
			else
			{
				data.referencedSoundBanks.Add(soundBank, 1);
			}
			S13Debug.Log($"{soundBank.name} is now referenced by {data.handler.name} {data.referencedSoundBanks[soundBank]} times.", base.gameObject, S13Debug.LogType.Soundbank);
			return data.referencedSoundBanks[soundBank];
		}
	}

	private int RemoveObject(S13SoundBank soundBank, HandlerData data)
	{
		if (!data.referencedSoundBanks.ContainsKey(soundBank))
		{
			S13Debug.LogWarning(soundBank.name + " is not referenced by " + data.handler.name + ", it may be being removed in error.", soundBank, S13Debug.LogType.Soundbank);
			return 0;
		}
		unchecked
		{
			if (data.referencedSoundBanks[soundBank] > 1)
			{
				data.referencedSoundBanks[soundBank]--;
				S13Debug.Log($"{soundBank.name} is now referenced by {data.handler.name} {data.referencedSoundBanks[soundBank]} times.", soundBank, S13Debug.LogType.Soundbank);
				return data.referencedSoundBanks[soundBank];
			}
			S13Debug.Log($"{soundBank.name} is now referenced by {data.handler.name} {data.referencedSoundBanks[soundBank] - 1} times and is being removed as a dependency.", soundBank, S13Debug.LogType.Soundbank);
			data.referencedSoundBanks.Remove(soundBank);
			return 0;
		}
	}

	private void _Play(S13Object obj)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.Play();
		}
	}

	private void _Play(S13Object obj, Transform parent)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.Play(parent);
		}
	}

	private void _Play(S13Object obj, float duration)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.Play(duration);
		}
	}

	private void _Play(S13Object obj, Transform parent, float duration)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.Play(parent, duration);
		}
	}

	private void _PlayDelayed(S13Object obj, float delayTime)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.PlayDelayed(delayTime);
		}
	}

	private void _PlayDelayed(S13Object obj, Transform parent, float delayTime)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.PlayDelayed(parent, delayTime);
		}
	}

	private void _PlayDelayed(S13Object obj, float delayTime, float duration)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.PlayDelayed(delayTime, duration);
		}
	}

	private void _PlayDelayed(S13Object obj, Transform parent, float delayTime, float duration)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.PlayDelayed(parent, delayTime, duration);
		}
	}

	private void _Stop(S13Object obj)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.Stop();
		}
	}

	private void _Stop(S13Object obj, bool ignoreFades)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.Stop(ignoreFades);
		}
	}

	private void _StopDelayed(S13Object obj, float delayTime)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.StopDelayed(delayTime);
		}
	}

	private void _StopDelayed(S13Object obj, float delayTime, bool ignoreFades)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.StopDelayed(delayTime, ignoreFades);
		}
	}

	private void _Shutdown(S13Object obj)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.Shutdown();
		}
	}

	private void _Pause(S13Object obj)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.Pause();
		}
	}

	private void _Resume(S13Object obj)
	{
		S13Handler s13Handler;
		s13Handler = this[obj];
		if ((bool)s13Handler)
		{
			s13Handler.Resume();
		}
	}

	private void _StopAll(bool ignoreFades)
	{
		foreach (KeyValuePair<S13Object, HandlerData> handler in this.handlers)
		{
			if (handler.Value.handler.isPlaying)
			{
				handler.Value.handler.Stop(ignoreFades);
			}
		}
	}

	private void _PauseGame()
	{
		this._gamePaused = true;
		S13Manager.OnGamePaused?.Invoke(obj: true);
	}

	private void _ResumeGame()
	{
		this._gamePaused = false;
		S13Manager.OnGamePaused?.Invoke(obj: false);
	}

	private void _RegisterAccessorListener(string accessorID, Action<float> listener)
	{
		if (this.accessorDelegates.ContainsKey(accessorID))
		{
			if (Enumerable.Contains(this.accessorDelegates[accessorID].GetInvocationList(), listener))
			{
				return;
			}
			AccessorRegistrar accessorRegistrar;
			accessorRegistrar = this.accessorDelegates;
			accessorRegistrar[accessorID] = (Action<float>)Delegate.Combine(accessorRegistrar[accessorID], listener);
		}
		else
		{
			this.accessorDelegates.Add(accessorID, listener);
		}
		S13Debug.Log("Registered" + $" to {accessorID} | Total: {this.accessorDelegates[accessorID].GetInvocationList().Length}", S13Debug.LogType.Accessor);
	}

	private void _DeRegisterAccessorListener(string accessorID, Action<float> listener)
	{
		string text;
		text = "listener";
		if (!this.accessorDelegates.ContainsKey(accessorID))
		{
			S13Debug.LogWarning("Attempted to remove " + text + " from accessorID \"" + accessorID + "\" despite no registered entry");
		}
		else
		{
			int num;
			num = this.accessorDelegates[accessorID].GetInvocationList().Length;
			if (num == 1)
			{
				this.accessorDelegates.Remove(accessorID);
			}
			else
			{
				AccessorRegistrar accessorRegistrar;
				accessorRegistrar = this.accessorDelegates;
				accessorRegistrar[accessorID] = (Action<float>)Delegate.Remove(accessorRegistrar[accessorID], listener);
			}
			S13Debug.Log($"De-Registered {text} from {accessorID}| Remaining {num = unchecked(num - 1)}", S13Debug.LogType.Accessor);
		}
	}

	private void _SetAccessor(string accessorID, float value)
	{
		if (float.IsNaN(value))
		{
			S13Debug.LogWarning("Received a NaN value, these should be filtered out");
		}
		else if (!this.accessorDelegates.ContainsKey(accessorID))
		{
			S13Debug.LogWarning("Accessor with accessorID =\"" + accessorID + "\" not registered");
		}
		else
		{
			this.accessorDelegates[accessorID](value);
		}
	}

	private void _RegisterSwitch(IS13Switchable switchable, Action<S13ScriptableSwitch> listener)
	{
		if (this.switchDelegates.ContainsKey(switchable))
		{
			SwitchRegistrar switchRegistrar;
			switchRegistrar = this.switchDelegates;
			switchRegistrar[switchable] = (Action<S13ScriptableSwitch>)Delegate.Combine(switchRegistrar[switchable], listener);
		}
		else
		{
			this.switchDelegates.Add(switchable, listener);
		}
		S13Debug.Log("Registered Switch" + $" to {switchable} | Total: {this.switchDelegates[switchable].GetInvocationList().Length}", S13Debug.LogType.Switch);
	}

	private void _DeRegisterSwitch(IS13Switchable switchable, Action<S13ScriptableSwitch> listener)
	{
		string arg;
		arg = "listener";
		if (!this.switchDelegates.ContainsKey(switchable))
		{
			S13Debug.LogWarning($"Attempted to remove Switch {arg} from \"{switchable}\" despite no registered entry");
			return;
		}
		int num;
		num = this.switchDelegates[switchable].GetInvocationList().Length;
		if (num == 1)
		{
			this.switchDelegates.Remove(switchable);
		}
		else
		{
			SwitchRegistrar switchRegistrar;
			switchRegistrar = this.switchDelegates;
			switchRegistrar[switchable] = (Action<S13ScriptableSwitch>)Delegate.Remove(switchRegistrar[switchable], listener);
		}
		S13Debug.Log($"De-Registered Switch {arg} from {switchable}| Remaining {num = unchecked(num - 1)}", S13Debug.LogType.Switch);
	}

	private void _SetSwitch(S13ObjectSwitch objectSwitch, S13ScriptableSwitch value)
	{
		if (objectSwitch == null)
		{
			S13Debug.LogWarning("Received null value in switch");
		}
		else if (this._GetGlobalHandler(objectSwitch) is IS13Switchable switchable)
		{
			this._SetSwitch(switchable, value);
		}
		else
		{
			S13Debug.Log(" Could not find handler for " + objectSwitch.name + " ", objectSwitch, S13Debug.LogType.Switch);
		}
	}

	private void _SetSwitch(IS13Switchable switchable, S13ScriptableSwitch value)
	{
		if (!this.switchDelegates.ContainsKey(switchable))
		{
			S13Debug.LogWarning($"Switch {switchable} not registered");
		}
		else
		{
			this.switchDelegates[switchable](value);
		}
	}

	private void _SetSwitch(S13ScriptableSwitch value)
	{
		Action<S13ScriptableSwitch>[] array;
		array = (from x in this.switchDelegates
			where x.Key.UsesScriptableSwitch(value)
			select x.Value).ToArray();
		for (int num = 0; num < array.Length; num = unchecked(num + 1))
		{
			array[num](value);
		}
	}

	private S13Handler _GetGlobalHandler(S13Object obj)
	{
		return this[obj];
	}

	private AudioListener _GetAudioListener()
	{
		return this.audioListener;
	}

	private bool _IsObjectLoaded(S13Object obj)
	{
		HandlerData value;
		if ((bool)obj)
		{
			return this.handlers.TryGetValue(obj, out value);
		}
		return false;
	}

	private S13ControlGroupHandler _GetControlGroupHandler(S13ScriptableControlGroup controlGroup)
	{
		if (this.controlGroups.ContainsKey(controlGroup))
		{
			return this.controlGroups[controlGroup];
		}
		GameObject gameObject;
		gameObject = new GameObject(controlGroup.name);
		gameObject.transform.SetParent(this.controlGroupHolder);
		S13ControlGroupHandler s13ControlGroupHandler;
		s13ControlGroupHandler = gameObject.AddComponent<S13ControlGroupHandler>();
		s13ControlGroupHandler.reference = controlGroup;
		this.controlGroups.Add(controlGroup, s13ControlGroupHandler);
		return s13ControlGroupHandler;
	}

	private void _SetScriptableState(S13ScriptableState newState)
	{
		S13StateTransition[] array;
		array = this.stateTransitionList.Where((S13StateTransition x) => x.from == this.currentScriptableState && x.to == newState).ToArray();
		S13Debug.Log("Setting current state from " + ((this.currentScriptableState != null) ? this.currentScriptableState.name : "None") + " to " + ((newState != null) ? newState.name : "None") + ". " + $"Performing {array.Length} Transitions");
		this.previousState = this.currentScriptableState;
		this.currentScriptableState = newState;
		if (array.Length != 0)
		{
			S13StateTransition[] array2;
			array2 = array;
			for (int num = 0; num < array2.Length; num = unchecked(num + 1))
			{
				array2[num].actions.Execute();
			}
		}
	}

	private void _SetGameStateToPrevious()
	{
		this._SetScriptableState(this.previousState);
	}
}
