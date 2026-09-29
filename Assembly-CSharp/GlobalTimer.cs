using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimer : JMonoBehaviour
{
	private static GlobalTimer m_Instance;

	private List<Timer> m_Timers = new List<Timer>();

	public bool isPaused { get; private set; }

	protected GlobalTimer()
	{
	}

	public static Timer SetTimer(float time)
	{
		GlobalTimer.CheckInstance();
		return GlobalTimer.m_Instance.InternalSetTime(time);
	}

	public static Timer SetTimer(float time, Action onComplete)
	{
		GlobalTimer.CheckInstance();
		return GlobalTimer.m_Instance.InternalSetTime(time, onComplete);
	}

	private static void CheckInstance()
	{
		if (GlobalTimer.m_Instance == null)
		{
			GlobalTimer.m_Instance = new GameObject("[GlobalTimer]").AddComponent<GlobalTimer>();
			global::UnityEngine.Object.DontDestroyOnLoad(GlobalTimer.m_Instance);
		}
	}

	private Timer InternalSetTime(float time)
	{
		return this.ReturnTimer(new Timer(time));
	}

	private Timer InternalSetTime(float time, Action onComplete)
	{
		return this.ReturnTimer(new Timer(time, onComplete));
	}

	private Timer ReturnTimer(Timer timer)
	{
		this.m_Timers.Add(timer);
		return timer;
	}

	public static void StartAll()
	{
		GlobalTimer.m_Instance.isPaused = false;
	}

	public static void PauseAll()
	{
		GlobalTimer.m_Instance.isPaused = true;
	}

	public static void ClearAll()
	{
		unchecked
		{
			for (int num = GlobalTimer.m_Instance.m_Timers.Count - 1; num >= 0; num--)
			{
				Timer timer;
				timer = GlobalTimer.m_Instance.m_Timers[num];
				if (timer != null)
				{
					timer.Pause();
					timer.Dispose();
				}
				GlobalTimer.m_Instance.m_Timers.RemoveAt(num);
			}
			GlobalTimer.m_Instance.m_Timers?.Clear();
		}
	}

	private void Update()
	{
		if (base.IsDisposed || this.isPaused || GameManager.Instance.IsPaused)
		{
			return;
		}
		unchecked
		{
			for (int num = this.m_Timers.Count - 1; num >= 0; num--)
			{
				Timer timer;
				timer = this.m_Timers[num];
				if (timer == null)
				{
					this.m_Timers.RemoveAt(num);
				}
				else
				{
					timer?.Update();
					if (timer.isComplete)
					{
						this.m_Timers.RemoveAt(num);
						timer.Dispose();
					}
				}
			}
		}
	}

	protected override void OnDisposed()
	{
		GlobalTimer.m_Instance = null;
		this.m_Timers?.Clear();
		this.m_Timers = null;
		base.OnDisposed();
	}
}
