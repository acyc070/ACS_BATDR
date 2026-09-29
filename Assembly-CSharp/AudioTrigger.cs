using System;
using UnityEngine;

public class AudioTrigger : JMonoBehaviour
{
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private float m_TimerMin = 900f;

	[SerializeField]
	private float m_TimerMax = 900f;

	private float m_Timer;

	private float m_TimerLimit;

	public bool IsEntered { get; private set; }

	public event EventHandler OnTriggered;

	public override void Start()
	{
		this.m_TimerLimit = global::UnityEngine.Random.Range(this.m_TimerMin, this.m_TimerMax);
		this.RemoveListeners();
		this.AddListeners();
	}

	private void Update()
	{
		if (this.IsEntered && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			if (this.m_Timer >= this.m_TimerLimit)
			{
				this.m_Timer = 0f;
				this.OnTriggered.Send(this);
			}
			else
			{
				this.m_Timer += Time.deltaTime;
			}
		}
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		this.IsEntered = true;
	}

	private void HandleEventTriggerOnExit(object sender, EventArgs e)
	{
		this.IsEntered = false;
		this.m_Timer = 0f;
	}

	private void AddListeners()
	{
		this.m_EventTrigger.OnEnter += this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.OnExit += this.HandleEventTriggerOnExit;
	}

	private void RemoveListeners()
	{
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.OnExit -= this.HandleEventTriggerOnExit;
	}

	protected override void OnDisposed()
	{
		this.OnTriggered = null;
		this.RemoveListeners();
		base.OnDisposed();
	}
}
