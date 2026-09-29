public abstract class AbilityState<TActor, TIdentifier> : State<TActor, TIdentifier>
{
	protected AbilityState(TActor actor, TIdentifier id)
		: base(actor)
	{
		base.ID = id;
	}

	protected abstract bool CheckReady();

	protected abstract bool CheckUse();

	protected abstract void ClearCheck();

	public abstract bool Use();

	public abstract void Reset();

	public sealed override void OnStateEnter()
	{
		this.InternalOnStateEnter();
	}

	protected abstract void InternalOnStateEnter();

	public sealed override void OnStateExit()
	{
		this.ClearCheck();
		this.InternalOnStateExit();
	}

	protected abstract void InternalOnStateExit();

	public sealed override void Update()
	{
		base.GetActions(this.InternalUpdate, this.InternalUpdateInput);
	}

	protected abstract void InternalUpdate();

	protected abstract void InternalUpdateInput();

	public sealed override void FixedUpdate()
	{
		base.GetActions(this.CheckFixedUpdate);
	}

	private void CheckFixedUpdate()
	{
		if (this.CheckReady())
		{
			this.InternalFixedUpdate();
		}
	}

	protected abstract void InternalFixedUpdate();

	public sealed override void LateUpdate()
	{
		base.GetActions(this.InternalLateUpdate);
	}

	protected abstract void InternalLateUpdate();

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
