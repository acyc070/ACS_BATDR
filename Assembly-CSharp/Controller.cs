using System;
using System.Collections;

public abstract class Controller : JMonoBehaviour, ICommandIEnumerator
{
	public bool IsActive { get; protected set; }

	public bool IsComplete { get; protected set; }

	public event EventHandler OnComplete;

	public IEnumerator Initialize()
	{
		JDebug.Log(base.name + ".Controller :: Initialize", this, JDebug.JDebugType.SectionController);
		this.IsActive = true;
		yield return this.InternalInitialize();
	}

	protected virtual IEnumerator InternalInitialize()
	{
		yield return null;
	}

	public void Activate()
	{
		JDebug.Log(base.name + ".Controller :: Activate", this, JDebug.JDebugType.SectionController);
		this.InternalActivate();
	}

	protected virtual void InternalActivate()
	{
	}

	public void Enable()
	{
		JDebug.Log(base.name + ".Controller :: Enable", this, JDebug.JDebugType.SectionController);
		this.InternalEnable();
	}

	protected virtual void InternalEnable()
	{
	}

	public void Disable()
	{
		JDebug.Log(base.name + ".Controller :: Disable", this, JDebug.JDebugType.SectionController);
		this.InternalDisable();
	}

	protected virtual void InternalDisable()
	{
	}

	public void Inactive()
	{
		JDebug.Log(base.name + ".Controller :: Inactive", this, JDebug.JDebugType.SectionController);
		this.InternalInactive();
	}

	protected virtual void InternalInactive()
	{
	}

	public void ForceComplete()
	{
		JDebug.Log(base.name + ".Controller :: ForceComplete", this, JDebug.JDebugType.SectionController);
		this.InternalForceComplete();
		this.SendOnComplete();
	}

	protected virtual void InternalForceComplete()
	{
	}

	protected void SendOnComplete()
	{
		if (!this.IsComplete)
		{
			this.IsComplete = true;
			this.OnComplete.Send(this);
		}
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
