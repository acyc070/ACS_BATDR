using System;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class MeatlyDirector : JMonoBehaviour
{
	[SerializeField]
	private MeatlyID m_MeatlyID;

	[SerializeField]
	private PlayableDirector m_PlayableDirector;

	private bool m_WasPlayingPause;

	public MeatlyID MeatlyID => this.m_MeatlyID;

	public PlayableDirector PlayableDirector => this.m_PlayableDirector;

	public bool IsPlaying { get; private set; }

	public bool IsSkipped { get; private set; }

	public bool IsComplete { get; private set; }

	public event EventHandler OnPause;

	public event EventHandler OnStopped;

	public event EventHandler OnSkipped;

	public event EventHandler OnStatusChange;

	public event EventHandler OnComplete;

	public override void Awake()
	{
		if (this.m_PlayableDirector == null)
		{
			this.m_PlayableDirector = base.GetComponent<PlayableDirector>();
		}
		this.m_PlayableDirector.playOnAwake = false;
	}

	private void Update()
	{
		if (this.IsComplete || base.IsDisposed)
		{
			return;
		}
		if (GameManager.Instance.IsPaused)
		{
			if (this.IsPlaying)
			{
				this.m_WasPlayingPause = true;
				this.IsPlaying = false;
				this.m_PlayableDirector.Pause();
			}
		}
		else if (this.m_WasPlayingPause)
		{
			this.m_WasPlayingPause = false;
			this.IsPlaying = true;
			this.m_PlayableDirector.Play();
		}
	}

	private void FixedUpdate()
	{
		if (this.IsPlaying && !this.IsComplete && !GameManager.Instance.IsPaused && !base.IsDisposed && (this.m_PlayableDirector.time >= this.m_PlayableDirector.duration || this.m_PlayableDirector.state != PlayState.Playing))
		{
			this.IsComplete = true;
			this.IsPlaying = false;
			this.ChangeStatus(CutsceneStatus.Complete);
		}
	}

	public void Play()
	{
		JDebug.Log("MeatlyDirector :: Play", this, JDebug.JDebugType.Cutscene);
		this.IsPlaying = true;
		this.m_PlayableDirector.Play();
		this.ChangeStatus(CutsceneStatus.Active);
	}

	public void Pause()
	{
		this.IsPlaying = false;
		this.IsSkipped = false;
		this.m_PlayableDirector.Pause();
		this.OnPause.Send(this);
	}

	public void Stop()
	{
		this.IsPlaying = false;
		this.IsSkipped = false;
		this.m_PlayableDirector.Stop();
		this.OnStopped.Send(this);
	}

	public void Skip()
	{
		this.IsSkipped = true;
		this.m_PlayableDirector.time = this.m_PlayableDirector.duration - 0.01;
		this.OnSkipped.Send(this);
	}

	public void ChangeStatus(CutsceneStatus status)
	{
		this.OnStatusChange.Send(status);
		if (status == CutsceneStatus.Complete)
		{
			this.OnComplete.Send(this);
		}
	}

	public void ResetDirector()
	{
		this.IsPlaying = false;
		this.IsSkipped = false;
		this.IsComplete = false;
		this.ChangeStatus(CutsceneStatus.None);
		this.m_PlayableDirector.Stop();
		this.m_PlayableDirector.time = 0.0;
	}

	protected override void OnDisposed()
	{
		this.OnPause = null;
		this.OnStopped = null;
		this.OnSkipped = null;
		this.OnStatusChange = null;
		this.OnComplete = null;
		this.m_PlayableDirector = null;
		base.OnDisposed();
	}
}
