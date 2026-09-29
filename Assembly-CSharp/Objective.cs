using System;

public abstract class Objective : JMonoBehaviour, ICommand
{
	public bool IsActive { get; protected set; }

	public bool IsComplete { get; protected set; }

	public event EventHandler OnComplete;

	private void Update()
	{
		if (this.IsActive && !GameManager.Instance.IsPaused && !base.IsDisposed)
		{
			if (this.IsComplete)
			{
				this.InternalUpdateComplete();
			}
			else
			{
				this.InternalUpdate();
			}
		}
	}

	protected virtual void InternalUpdate()
	{
	}

	protected virtual void InternalUpdateComplete()
	{
	}

	public void Initialize()
	{
		this.IsActive = true;
		this.InternalInitialize();
	}

	protected virtual void InternalInitialize()
	{
	}

	public void Enable()
	{
		this.InternalEnable();
	}

	protected virtual void InternalEnable()
	{
	}

	public void Disable()
	{
		this.InternalDisable();
	}

	protected virtual void InternalDisable()
	{
	}

	public void Inactive()
	{
		this.InternalInactive();
	}

	protected virtual void InternalInactive()
	{
	}

	public void ForceComplete()
	{
		this.IsComplete = true;
		this.InternalForceComplete();
	}

	protected virtual void InternalForceComplete()
	{
	}

	protected void SendOnComplete()
	{
		this.IsComplete = true;
		this.InternalComplete();
		this.OnComplete.Send(this);
	}

	protected virtual void InternalComplete()
	{
	}

	protected virtual void RemoveListeners()
	{
	}

	public sealed override void Start()
	{
	}

	public sealed override void Awake()
	{
	}

	public sealed override void OnEnable()
	{
	}

	public sealed override void OnDisable()
	{
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.OnComplete = null;
		base.OnDisposed();
	}
}
