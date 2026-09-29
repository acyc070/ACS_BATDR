using System;
using UnityEngine;

public class Timer : JDisposable
{
	private Action m_OnComplete;

	private float m_SetTime;

	public float time { get; private set; }

	public bool isPaused { get; private set; }

	public bool isComplete { get; private set; }

	public event EventHandler OnComplete;

	public Timer(float _time)
	{
		this.time = _time;
	}

	public Timer(float _time, Action onComplete)
	{
		this.m_OnComplete = onComplete;
		this.m_SetTime = _time;
		this.time = this.m_SetTime;
	}

	public void Start()
	{
		this.isPaused = false;
	}

	public void Stop()
	{
		this.isComplete = true;
	}

	public void Pause()
	{
		this.isPaused = true;
	}

	public void Reset()
	{
		this.time = this.m_SetTime;
	}

	public void Update()
	{
		if (!base.IsDisposed && !this.isComplete && !this.isPaused)
		{
			this.time -= Time.deltaTime;
			if (this.time <= 0f)
			{
				this.m_OnComplete?.Invoke();
				this.OnComplete.Send(this);
				this.isComplete = true;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		this.m_OnComplete = null;
		base.OnDisposed();
	}
}
