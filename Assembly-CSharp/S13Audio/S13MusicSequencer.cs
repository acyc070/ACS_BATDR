using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;
using UnityEngine.Audio;

namespace S13Audio;

public class S13MusicSequencer : MonoBehaviour
{
	private enum FadeDirection
	{
		IN = 0,
		OUT = 1
	}

	private class Stinger
	{
		private AudioSource _stingerSource;

		private AudioClip[] _stingers;

		public Transform ParentTransform
		{
			set
			{
				this._stingerSource.gameObject.transform.parent = value;
			}
		}

		internal Stinger()
		{
			this._stingerSource = new GameObject("Stinger Source").AddComponent<AudioSource>();
			this._stingerSource.volume = 1f;
		}

		internal void Play(double startTime)
		{
			if (!this._stingerSource.isPlaying)
			{
				AudioClip audioClip;
				audioClip = this._stingers[global::UnityEngine.Random.Range(0, this._stingers.Length)];
				if (audioClip == null)
				{
					S13Debug.LogWarning("Null found when searching for next stinger. Stinger.Play() cancelled.", this._stingerSource, S13Debug.LogType.Sequencer);
					return;
				}
				this._stingerSource.clip = audioClip;
				S13Debug.Log($"MusicSequencer scheduling {this._stingerSource.clip.name} in {startTime}.", this._stingerSource, S13Debug.LogType.Sequencer);
				this._stingerSource.PlayScheduled(startTime);
			}
		}

		internal void Load(AudioClip[] stingersToLoad, AudioMixerGroup mixerGroup)
		{
			this._stingers = stingersToLoad;
			this._stingerSource.outputAudioMixerGroup = mixerGroup;
		}
	}

	private enum PlayMode
	{
		Loop = 0,
		Linear = 1,
		Random = 2
	}

	private enum TransitionType
	{
		Immediate = 0,
		NextBeat = 1,
		NextBar = 2,
		EndOfClip = 3
	}

	[Serializable]
	internal class S13MusicStemList : ReorderableArray<S13ObjectStemGroup>
	{
	}

	private const float MIN_TRANSITION_TIME = 0.2f;

	[Tooltip("The stem groups which to play")]
	[Reorderable]
	[SerializeField]
	private S13MusicStemList stemGroupList = new S13MusicStemList();

	[Tooltip("Loop: Iterate list on repeat, Linear: Play through the stems once, Random: Continually play random stems")]
	[SerializeField]
	private PlayMode playMode;

	[Tooltip("Curve for fading between transitions. Fade-ins use this curve left to right, fade-outs use the curve right to left ")]
	[S13CurveRange(0f, 0f, 1f, 1f)]
	[SerializeField]
	private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[Tooltip("How long a fade should take to complete. Because we cross-fade, this is the total fade time")]
	[SerializeField]
	private float fadeDuration = 0.5f;

	[Tooltip("Should all transitions be faded, or only the first?")]
	[SerializeField]
	private bool fadeAllTransitions = true;

	[Tooltip("Should the audio sources be set to loop? This assists in preventing sounds from dropping out")]
	[SerializeField]
	private bool shouldLoopAudioSources;

	private Stinger _stinger;

	private Queue<S13ObjectStemGroup> _stemQueue = new Queue<S13ObjectStemGroup>();

	private List<AudioSource> _freeAudioSources = new List<AudioSource>();

	private List<AudioSource> _activeAudioSources = new List<AudioSource>();

	private double _nextStartTime = double.PositiveInfinity;

	private float _tempo = 100f;

	private int _beatsPerBar = 4;

	private int _meter = 4;

	private S13ObjectStemGroup _lastStemGroup;

	private double BeatLength => 60.0 / (double)this._tempo;

	private double BarLength => this.BeatLength * (double)this._beatsPerBar * (double)((float)this._beatsPerBar / (float)this._meter);

	private void Awake()
	{
		if (!this.CheckStemsValid(out var message))
		{
			S13Debug.LogWarning(message + ", disabling " + base.name, this, S13Debug.LogType.Sequencer);
			base.enabled = false;
			return;
		}
		int num;
		num = 0;
		foreach (S13ObjectStemGroup stemGroup in this.stemGroupList)
		{
			if (stemGroup.stems.Length > num)
			{
				num = stemGroup.stems.Length;
			}
		}
		unchecked
		{
			num *= 2;
			for (int i = 0; i < num; i++)
			{
				AudioSource audioSource;
				audioSource = new GameObject("Music Source " + i).AddComponent<AudioSource>();
				audioSource.transform.parent = base.transform;
				audioSource.loop = this.shouldLoopAudioSources;
				audioSource.volume = 0f;
				audioSource.playOnAwake = false;
				this._freeAudioSources.Add(audioSource);
			}
			this._stinger = new Stinger
			{
				ParentTransform = base.transform
			};
		}
	}

	private void Update()
	{
		if (AudioSettings.dspTime > this._nextStartTime)
		{
			this.ScheduleTransition(TransitionType.EndOfClip);
		}
	}

	private void OnDisable()
	{
		if (this._activeAudioSources.Count <= 0)
		{
			return;
		}
		foreach (AudioSource item in this._activeAudioSources.ToList())
		{
			this.ResetSource(item);
		}
		this._nextStartTime = double.PositiveInfinity;
	}

	private void ScheduleTransition(TransitionType transitionType, bool firstTime = false)
	{
		if (!firstTime && this._activeAudioSources.Count < 1)
		{
			S13Debug.Log("Cannot schedule a transition while the sequencer is stopped.", this, S13Debug.LogType.Sequencer);
			return;
		}
		if (this._stemQueue.Count < 1 || this._stemQueue.Peek() == null)
		{
			if (this.playMode == PlayMode.Linear)
			{
				S13Debug.Log("No more valid items available in queue", this, S13Debug.LogType.Sequencer);
				this.Stop();
				return;
			}
			this.LoadQueue();
		}
		if (this._freeAudioSources.Count < 1 || this._stemQueue.Peek().stems.Length > this._freeAudioSources.Count)
		{
			S13Debug.LogWarning("Not enough free audio sources available for transition.", this, S13Debug.LogType.Sequencer);
			return;
		}
		S13ObjectStemGroup s13ObjectStemGroup;
		s13ObjectStemGroup = this._stemQueue.Dequeue();
		double nextStartTime;
		nextStartTime = this.GetNextStartTime(transitionType);
		AudioSource[] freeSources;
		freeSources = this.GetFreeSources(s13ObjectStemGroup.stems.Length);
		if (freeSources == null)
		{
			S13Debug.LogWarning("No free audio sources available. A stemGroup was passed in that exceeded the number of available sources, or an error has occurred and audio sources are not being cleared after use.", this, S13Debug.LogType.Sequencer);
			return;
		}
		unchecked
		{
			if (this._activeAudioSources.Count > 0)
			{
				AudioSource[] array;
				array = this._activeAudioSources.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					_ = array[i];
					if (this.fadeAllTransitions)
					{
						base.StartCoroutine(this.ScheduleFade(array[i], nextStartTime, FadeDirection.OUT));
					}
					else
					{
						base.StartCoroutine(this.ScheduleResetSource(array[i], nextStartTime));
					}
				}
			}
			for (int j = 0; j < s13ObjectStemGroup.stems.Length; j++)
			{
				freeSources[j].clip = s13ObjectStemGroup.stems[j].clip;
				freeSources[j].outputAudioMixerGroup = s13ObjectStemGroup.stems[j].mixer;
				freeSources[j].loop = s13ObjectStemGroup.loop;
				if (firstTime || this.fadeAllTransitions)
				{
					base.StartCoroutine(this.ScheduleFade(freeSources[j], nextStartTime, FadeDirection.IN));
				}
				else
				{
					S13Debug.Log($"Scheduling {freeSources[j].clip.name} in {this.GetDspTimeUntil(nextStartTime)} Seconds", this, S13Debug.LogType.Sequencer);
					freeSources[j].volume = 1f;
					freeSources[j].PlayScheduled(nextStartTime);
				}
				this._freeAudioSources.Remove(freeSources[j]);
				this._activeAudioSources.Add(freeSources[j]);
			}
			this._stinger.Load(s13ObjectStemGroup.stingers, s13ObjectStemGroup.stingerMixerGroup);
			this._tempo = s13ObjectStemGroup.tempo;
			this._beatsPerBar = s13ObjectStemGroup.beatsPerBar;
			this._meter = s13ObjectStemGroup.meter;
			this._lastStemGroup = s13ObjectStemGroup;
			if (s13ObjectStemGroup.loop)
			{
				this._nextStartTime = double.PositiveInfinity;
				S13Debug.Log(" " + s13ObjectStemGroup.name + " is set to loop.", this, S13Debug.LogType.Sequencer);
			}
			else
			{
				this._nextStartTime = AudioSettings.dspTime + S13MusicSequencer.GetClipDuration(freeSources[0].clip) + 0.20000000298023224 - (double)this.fadeDuration;
				S13Debug.Log($"Next start time for {freeSources[0].clip.name}: {this._nextStartTime:0.000} | In {this.GetDspTimeUntil(this._nextStartTime)} Seconds", this, S13Debug.LogType.Sequencer);
			}
		}
	}

	private double GetNextStartTime(TransitionType type)
	{
		AudioSource activeSource;
		activeSource = this.GetActiveSource();
		if (activeSource == null)
		{
			type = TransitionType.Immediate;
		}
		double timeElapsed;
		timeElapsed = S13MusicSequencer.GetTimeElapsed(activeSource);
		switch (type)
		{
		case TransitionType.Immediate:
			return AudioSettings.dspTime + 0.20000000298023224;
		case TransitionType.NextBeat:
		{
			double num2;
			num2 = this.BeatLength - timeElapsed % this.BeatLength;
			return AudioSettings.dspTime + num2;
		}
		case TransitionType.NextBar:
		{
			double num;
			num = this.BarLength - timeElapsed % this.BarLength;
			return AudioSettings.dspTime + num;
		}
		case TransitionType.EndOfClip:
			return AudioSettings.dspTime + (S13MusicSequencer.GetClipDuration(this.GetActiveSource().clip) - timeElapsed) + 0.20000000298023224 - (double)this.fadeDuration;
		default:
			throw new ArgumentOutOfRangeException("type", type, null);
		}
	}

	private AudioSource[] GetFreeSources(int numberRequested)
	{
		if (this._freeAudioSources.Count < numberRequested)
		{
			S13Debug.LogWarning($"Can't provide {numberRequested} free audio sources.", this);
			return null;
		}
		AudioSource[] array;
		array = new AudioSource[numberRequested];
		int num;
		num = 0;
		unchecked
		{
			for (int i = 0; i < numberRequested; i++)
			{
				array[num] = this._freeAudioSources[i];
				num++;
			}
			return array;
		}
	}

	private IEnumerator ScheduleFade(AudioSource source, double startFadeTime, FadeDirection fade)
	{
		S13Debug.Log($"Fade {fade} scheduled to start at {startFadeTime:0.000}. Current dspTime is: {AudioSettings.dspTime:0.000} | Delay: {this.GetDspTimeUntil(startFadeTime)}", S13Debug.LogType.Sequencer);
		while (AudioSettings.dspTime < startFadeTime)
		{
			yield return null;
		}
		float currentTime;
		currentTime = 0f;
		if (fade == FadeDirection.IN)
		{
			source.volume = 0f;
			source.Play();
		}
		S13Debug.Log($"Fade {fade} starting for {source.name} with clip {source.clip.name}!", S13Debug.LogType.Sequencer);
		if (this.fadeDuration <= 0f && fade == FadeDirection.IN)
		{
			source.volume = this.fadeCurve.Evaluate(1f);
		}
		while (currentTime < this.fadeDuration)
		{
			currentTime += Time.deltaTime;
			float num;
			num = currentTime / this.fadeDuration;
			if (fade == FadeDirection.OUT)
			{
				num = 1f - num;
			}
			source.volume = this.fadeCurve.Evaluate(num);
			yield return null;
		}
		if (fade == FadeDirection.OUT)
		{
			this.ResetSource(source);
		}
	}

	private IEnumerator ScheduleResetSource(AudioSource source, double resetTime)
	{
		S13Debug.Log($"Reset audio source scheduled to start at {resetTime:0.000}. Current dspTime is: {AudioSettings.dspTime:0.000} | Delay: {this.GetDspTimeUntil(resetTime)}", S13Debug.LogType.Sequencer);
		while (AudioSettings.dspTime < resetTime)
		{
			yield return null;
		}
		this.ResetSource(source);
	}

	private void ResetSource(AudioSource source)
	{
		source.Stop();
		source.volume = 0f;
		if (this._activeAudioSources.Contains(source))
		{
			this._activeAudioSources.Remove(source);
		}
		if (!this._freeAudioSources.Contains(source))
		{
			this._freeAudioSources.Add(source);
		}
		source.clip = null;
		source.outputAudioMixerGroup = null;
	}

	private AudioSource GetActiveSource()
	{
		return this._activeAudioSources.FirstOrDefault((AudioSource t) => t.isPlaying);
	}

	private static double GetClipDuration(AudioClip clip)
	{
		return (double)clip.samples / (double)clip.frequency;
	}

	private static double GetTimeElapsed(AudioSource source)
	{
		if (source == null)
		{
			return 0.0;
		}
		return (double)source.timeSamples / (double)source.clip.frequency;
	}

	private void LoadQueue()
	{
		if (this.stemGroupList.Count < 1)
		{
			S13Debug.LogWarning("S13MusicSequencer::LoadQueue() No stems available to load", this);
			return;
		}
		foreach (S13ObjectStemGroup stemGroup in this.stemGroupList)
		{
			if (stemGroup.stems == null)
			{
				S13Debug.LogWarning("S13MusicSequencer::LoadQueue() stem group " + stemGroup.name + " stems array is null, this will cause errors", stemGroup);
			}
		}
		switch (this.playMode)
		{
		case PlayMode.Loop:
		case PlayMode.Linear:
		{
			foreach (S13ObjectStemGroup stemGroup2 in this.stemGroupList)
			{
				this._stemQueue.Enqueue(stemGroup2);
				S13Debug.Log(stemGroup2.name + " added to queue", this, S13Debug.LogType.Sequencer);
			}
			break;
		}
		case PlayMode.Random:
		{
			List<S13ObjectStemGroup> list;
			list = new List<S13ObjectStemGroup>(this.stemGroupList);
			if (list.Count > 1)
			{
				list.Remove(this._lastStemGroup);
			}
			S13ObjectStemGroup s13ObjectStemGroup;
			s13ObjectStemGroup = list[global::UnityEngine.Random.Range(0, list.Count)];
			this._stemQueue.Enqueue(s13ObjectStemGroup);
			S13Debug.Log(s13ObjectStemGroup.name + " added to queue", this, S13Debug.LogType.Sequencer);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void PlayStinger(TransitionType transitionType)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			S13Debug.LogWarning("Attempted to call PlayStringer on a sequencer that is not active, ignoring", this, S13Debug.LogType.Sequencer);
			return;
		}
		if (this._activeAudioSources.Count < 1)
		{
			S13Debug.LogWarning("MusicSequencer can't schedule a stinger when the music system is stopped.");
			return;
		}
		double nextStartTime;
		nextStartTime = this.GetNextStartTime(transitionType);
		this._stinger.Play(nextStartTime);
	}

	private bool CheckStemsValid(out string message)
	{
		if (this.stemGroupList.Length < 1)
		{
			message = "StemGroupList Invalid: No stem groups";
			return false;
		}
		foreach (S13ObjectStemGroup stemGroup in this.stemGroupList)
		{
			if (stemGroup == null)
			{
				message = "StemGroupList Invalid: StemGroupList contains null stem group";
				return false;
			}
			if (stemGroup.stems == null || stemGroup.stems.Length < 1)
			{
				message = "StemGroup Invalid: " + stemGroup.name + " has no stems";
				return false;
			}
			if (stemGroup.stems.Any((S13ObjectStemGroup.Stem stem) => stem.clip == null))
			{
				message = "StemGroup Invalid: " + stemGroup.name + " contains stem with null clip";
				return false;
			}
		}
		message = "Stems valid";
		return true;
	}

	private double GetDspTimeUntil(double targetDSP)
	{
		return targetDSP - AudioSettings.dspTime;
	}

	public void Play()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			S13Debug.LogWarning("Attempted to call Play on a sequencer that is not active, ignoring", this, S13Debug.LogType.Sequencer);
			return;
		}
		if (this._activeAudioSources.Count > 0)
		{
			S13Debug.Log("Music Sequence is already playing.", this, S13Debug.LogType.Sequencer);
			return;
		}
		this.LoadQueue();
		this.ScheduleTransition(TransitionType.Immediate, firstTime: true);
	}

	public void Stop()
	{
		double num;
		num = AudioSettings.dspTime + 0.20000000298023224;
		AudioSource[] array;
		array = this._activeAudioSources.ToArray();
		foreach (AudioSource source in array)
		{
			if (this.fadeAllTransitions && this.fadeDuration > 0f)
			{
				base.StartCoroutine(this.ScheduleFade(source, num, FadeDirection.OUT));
			}
			else
			{
				base.StartCoroutine(this.ScheduleResetSource(source, num));
			}
		}
		this._nextStartTime = double.PositiveInfinity;
	}

	public void TransitionImmediate()
	{
		this.ScheduleTransition(TransitionType.Immediate);
	}

	public void TransitionNextBar()
	{
		this.ScheduleTransition(TransitionType.NextBar);
	}

	public void TransitionNextBeat()
	{
		this.ScheduleTransition(TransitionType.NextBeat);
	}

	public void PlayStingerNextBeat()
	{
		this.PlayStinger(TransitionType.NextBeat);
	}

	public void PlayStingerNextBar()
	{
		this.PlayStinger(TransitionType.NextBar);
	}

	public bool CheckStemsValid()
	{
		string message;
		return this.CheckStemsValid(out message);
	}
}
