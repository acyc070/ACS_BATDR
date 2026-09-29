public abstract class WeaponState<T> : State<T, object>
{
	protected WeaponData Data { get; private set; }

	public abstract bool IsCharged { get; }

	public abstract bool IsOvercharged { get; }

	protected WeaponState(T actor, WeaponData data)
		: base(actor)
	{
		this.Data = data;
	}

	protected abstract bool CheckUse();

	public abstract bool Use();

	public abstract void Reset();

	public sealed override void OnStateEnter()
	{
		this.InternalOnStateEnter();
	}

	protected abstract void InternalOnStateEnter();

	public sealed override void OnStateExit()
	{
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
		base.GetActions(this.InternalFixedUpdate);
	}

	protected abstract void InternalFixedUpdate();

	public sealed override void LateUpdate()
	{
		base.GetActions(this.InternalLateUpdate);
	}

	protected abstract void InternalLateUpdate();

	protected override void OnDisposed()
	{
		this.Data = null;
		base.OnDisposed();
	}
}
