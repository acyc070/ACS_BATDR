using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

public abstract class S13HandlerPrimitive<TObject, THandler> : S13Handler<TObject, THandler>, IS13HandlerPrimitive where TObject : S13ObjectPrimitive<TObject, THandler> where THandler : S13HandlerPrimitive<TObject, THandler>
{
	protected class VoiceHandlerData
	{
		public enum State
		{
			Stopped = 0,
			Waiting = 1,
			FadingIn = 2,
			Playing = 3,
			FadingOut = 4
		}

		public float volume;

		public float startTime;

		public Coroutine coroutine;

		public bool sourcePaused;

		public State state { get; set; }

		public AudioSource voice { get; private set; }

		public S13PrimitiveVoiceHelper helper { get; private set; }

		public bool isPlaying => this.state != State.Stopped;

		public VoiceHandlerData(AudioSource voice)
		{
			this.voice = voice;
			this.helper = voice.GetOrAddComponent<S13PrimitiveVoiceHelper>();
		}
	}

	protected AudioSource[] voices;

	protected Dictionary<AudioSource, VoiceHandlerData> voiceHandlers = new Dictionary<AudioSource, VoiceHandlerData>();

	public override bool isPlaying => this.voices.Any((AudioSource x) => this.voiceHandlers[x].isPlaying);

	public override int NumActiveVoices => this.voices.Count((AudioSource x) => x.isPlaying);

	public AudioSource[] Voices => this.voices;

	private void OnDisable()
	{
		if (base.gameObject.activeInHierarchy)
		{
			return;
		}
		foreach (KeyValuePair<AudioSource, VoiceHandlerData> voiceHandler in this.voiceHandlers)
		{
			voiceHandler.Value.state = VoiceHandlerData.State.Stopped;
		}
	}

	public override void Initialize(TObject targetObject)
	{
		base.Initialize(targetObject);
		if (!targetObject)
		{
			return;
		}
		if (!targetObject.voiceTemplate || !targetObject.voiceTemplate.audioSource)
		{
			S13Debug.LogError("S13Object '" + targetObject.name + "' does not have a voice template assigned (or the template's prefab is null)!", targetObject);
			return;
		}
		this.voices = new AudioSource[targetObject.voiceCount];
		unchecked
		{
			for (int i = 0; i < targetObject.voiceCount; i++)
			{
				this.voices[i] = this.CreateVoice();
				this.voices[i].name = targetObject.name + " Voice " + (i + 1);
			}
		}
	}

	protected override void StopHandler(bool ignoreFades)
	{
		base.StopHandler(ignoreFades);
		AudioSource[] array;
		if (ignoreFades)
		{
			array = this.voices;
			foreach (AudioSource audioSource in array)
			{
				if (this.voiceHandlers[audioSource].state != VoiceHandlerData.State.Stopped)
				{
					this.StopVoice(audioSource);
				}
			}
			return;
		}
		array = this.voices;
		foreach (AudioSource audioSource2 in array)
		{
			switch (this.voiceHandlers[audioSource2].state)
			{
			case VoiceHandlerData.State.Waiting:
				this.StopVoice(audioSource2);
				break;
			case VoiceHandlerData.State.FadingIn:
			case VoiceHandlerData.State.Playing:
				this.voiceHandlers[audioSource2].state = VoiceHandlerData.State.FadingOut;
				break;
			}
		}
	}

	protected override void PauseHandler()
	{
		base.PauseHandler();
		AudioSource[] array;
		array = this.voices;
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			this.PauseVoice(array[i]);
		}
	}

	protected override void ResumeHandler()
	{
		base.ResumeHandler();
		AudioSource[] array;
		array = this.voices;
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			this.ResumeVoice(array[i]);
		}
	}

	public AudioSource CreateVoice()
	{
		AudioSource audioSource;
		audioSource = Object.Instantiate(base.targetObject.voiceTemplate.audioSource);
		audioSource.transform.SetParent(base.transform);
		if (base.targetObject.outputOverride != null)
		{
			audioSource.outputAudioMixerGroup = base.targetObject.outputOverride;
		}
		audioSource.playOnAwake = false;
		audioSource.loop = base.targetObject.loop;
		audioSource.priority = base.targetObject.priority;
		audioSource.spatialize = base.targetObject.spatialize;
		audioSource.bypassReverbZones = base.targetObject.bypassReverbZones;
		VoiceHandlerData voiceHandlerData;
		voiceHandlerData = new VoiceHandlerData(audioSource);
		voiceHandlerData.helper.followMode = base.targetObject.followMode;
		this.voiceHandlers[audioSource] = voiceHandlerData;
		return audioSource;
	}

	protected AudioSource GetValidVoice(Vector3 voicePosition)
	{
		AudioSource audioSource;
		audioSource = this.GetFreeVoice();
		if (!audioSource)
		{
			switch (base.targetObject.voiceMode)
			{
			case S13Object.VoiceMode.KillOld:
				audioSource = this.GetOldestVoice();
				this.StopVoice(audioSource);
				break;
			case S13Object.VoiceMode.UseClosest:
			{
				Vector3 vector;
				vector = S13Manager.GetAudioListener()?.transform?.position ?? Vector3.zero;
				audioSource = this.GetFurthestVoice(vector, Vector3.Distance(vector, voicePosition));
				if ((bool)audioSource)
				{
					this.StopVoice(audioSource);
				}
				break;
			}
			}
		}
		return audioSource;
	}

	protected AudioSource GetFreeVoice()
	{
		AudioSource[] array;
		array = this.voices;
		foreach (AudioSource audioSource in array)
		{
			if (this.voiceHandlers[audioSource].state == VoiceHandlerData.State.Stopped)
			{
				return audioSource;
			}
		}
		return null;
	}

	protected AudioSource GetOldestVoice()
	{
		float num;
		num = float.PositiveInfinity;
		AudioSource result;
		result = null;
		AudioSource[] array;
		array = this.voices;
		foreach (AudioSource audioSource in array)
		{
			if (this.voiceHandlers[audioSource].startTime < num)
			{
				num = this.voiceHandlers[audioSource].startTime;
				result = audioSource;
			}
		}
		return result;
	}

	protected AudioSource GetFurthestVoice(Vector3 listenerPosition, float minDistance)
	{
		float num;
		num = minDistance;
		AudioSource result;
		result = null;
		AudioSource[] array;
		array = this.voices;
		foreach (AudioSource audioSource in array)
		{
			float num2;
			num2 = Vector3.Distance(audioSource.transform.position, listenerPosition);
			if (num2 > num)
			{
				num = num2;
				result = audioSource;
			}
		}
		return result;
	}

	protected void SetVoiceParent(AudioSource voice, Transform parent, Vector3 offset)
	{
		if (parent == null)
		{
			parent = base.transform;
		}
		this.voiceHandlers[voice].helper.SetVoiceParent(parent, offset);
	}

	protected void VoiceClip(AudioClip clip, bool loop, Transform parent, Vector3? offsetPosition = null, AudioSource targetVoice = null)
	{
		if ((bool)clip)
		{
			Vector3 vector;
			vector = offsetPosition ?? base.targetObject.GetPositionOffset();
			Vector3 vector2;
			vector2 = ((parent != null) ? parent.position : Vector3.zero);
			if (!targetVoice)
			{
				targetVoice = this.GetValidVoice(vector2 + vector);
			}
			if ((bool)targetVoice)
			{
				this.SetVoiceParent(targetVoice, parent, vector);
				targetVoice.clip = clip;
				targetVoice.loop = loop;
				this.StartVoice(targetVoice);
			}
		}
	}

	protected void StartVoice(AudioSource voice)
	{
		VoiceHandlerData voiceHandlerData;
		voiceHandlerData = this.voiceHandlers[voice];
		if (voiceHandlerData.coroutine != null)
		{
			base.StopCoroutine(voiceHandlerData.coroutine);
		}
		voiceHandlerData.volume = base.targetObject.volumeRange.value;
		voiceHandlerData.startTime = base.currentTime;
		voiceHandlerData.coroutine = base.StartCoroutine(this.HandleVoice(voice));
	}

	protected void StopVoice(AudioSource voice)
	{
		VoiceHandlerData voiceHandlerData;
		voiceHandlerData = this.voiceHandlers[voice];
		voice.Stop();
		voiceHandlerData.state = VoiceHandlerData.State.Stopped;
		voiceHandlerData.sourcePaused = false;
		base.StopCoroutine(voiceHandlerData.coroutine);
		voiceHandlerData.coroutine = null;
		voiceHandlerData.helper.ClearVoiceParent();
	}

	protected void PauseVoice(AudioSource voice)
	{
		if (!(voice == null))
		{
			VoiceHandlerData voiceHandlerData;
			voiceHandlerData = this.voiceHandlers[voice];
			if (voiceHandlerData.isPlaying && voice.isPlaying)
			{
				voice.Pause();
				voiceHandlerData.sourcePaused = true;
			}
		}
	}

	protected void ResumeVoice(AudioSource voice)
	{
		VoiceHandlerData voiceHandlerData;
		voiceHandlerData = this.voiceHandlers[voice];
		if (voiceHandlerData.sourcePaused)
		{
			voiceHandlerData.sourcePaused = false;
			voice.UnPause();
		}
	}

	private IEnumerator HandleVoice(AudioSource voice)
	{
		VoiceHandlerData data;
		data = this.voiceHandlers[voice];
		data.state = VoiceHandlerData.State.Waiting;
		data.sourcePaused = false;
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
		voice.time = base.targetObject.startTime;
		voice.volume = 0f;
		voice.pitch = S13AudioUtil.SemitoneToPitch(base.targetObject.pitchRange.value);
		voice.Play();
		data.state = VoiceHandlerData.State.FadingIn;
		float fadeProgress;
		fadeProgress = 0f;
		bool playing;
		playing = true;
		while (playing)
		{
			if (base.isPaused)
			{
				yield return null;
				continue;
			}
			if (!voice.isPlaying)
			{
				break;
			}
			switch (data.state)
			{
			case VoiceHandlerData.State.FadingIn:
				fadeProgress = ((!(base.targetObject.fadeInTime > 0f)) ? 1f : Mathf.Clamp01(fadeProgress + base.deltaTime / base.targetObject.fadeInTime));
				if (fadeProgress >= 1f)
				{
					data.state = VoiceHandlerData.State.Playing;
				}
				break;
			case VoiceHandlerData.State.FadingOut:
				fadeProgress = ((!(base.targetObject.fadeOutTime > 0f)) ? 0f : Mathf.Clamp01(fadeProgress - base.deltaTime / base.targetObject.fadeOutTime));
				if (fadeProgress <= 0f)
				{
					playing = false;
				}
				break;
			default:
				playing = false;
				voice.volume = 0f;
				break;
			case VoiceHandlerData.State.Playing:
				break;
			}
			if (!playing)
			{
				break;
			}
			voice.volume = S13AudioUtil.DecibelToLinear(data.volume) * base.targetObject.fadeCurve.Evaluate(fadeProgress);
			yield return null;
		}
		voice.Stop();
		data.state = VoiceHandlerData.State.Stopped;
		data.coroutine = null;
		data.helper.ClearVoiceParent();
	}
}
