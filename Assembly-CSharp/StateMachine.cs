using System;
using System.Collections.Generic;

public abstract class StateMachine<TActor, TIdentifier> : JMonoBehaviour where TIdentifier : Enum
{
	private const string STATE_STRING = "State";

	private Dictionary<TIdentifier, State<TActor, TIdentifier>> m_ActiveStates = new Dictionary<TIdentifier, State<TActor, TIdentifier>>();

	private bool m_IsActive;

	protected bool m_GizmosSelected;

	protected State<TActor, TIdentifier> m_State { get; private set; }

	protected TIdentifier m_PreviousState { get; private set; }

	protected TIdentifier m_NextState { get; private set; }

	protected bool IsActive
	{
		get
		{
			if (this.m_IsActive)
			{
				return this.m_State != null;
			}
			return false;
		}
	}

	protected virtual bool UseAwake => true;

	public bool IsInitialized { get; private set; }

	public TIdentifier CurrentState { get; private set; }

	public bool Disabled { get; private set; }

	public bool GizmosSelected => this.m_GizmosSelected;

	public StateMachine()
	{
		this.SetActive(active: true);
	}

	protected void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	public sealed override void Awake()
	{
		if (this.UseAwake)
		{
			this.Initialize();
		}
	}

	public void Initialize()
	{
		if (!this.IsInitialized)
		{
			this.InternalInitialize();
			if (!this.UseAwake)
			{
				this.InitializeOnComplete();
				this.IsInitialized = true;
			}
		}
	}

	protected virtual void InternalInitialize()
	{
	}

	public sealed override void Start()
	{
		if (this.UseAwake)
		{
			this.InitializeOnComplete();
		}
	}

	private void InitializeOnComplete()
	{
		if (!this.IsInitialized)
		{
			this.InternalInitializeOnComplete();
		}
		if (this.UseAwake)
		{
			this.IsInitialized = true;
		}
	}

	protected virtual void InternalInitializeOnComplete()
	{
	}

	protected virtual void Update()
	{
		this.GetActions(() =>
		{
			this.m_State?.Update();
		});
	}

	protected virtual void FixedUpdate()
	{
		this.GetActions(() =>
		{
			this.m_State?.FixedUpdate();
		});
	}

	protected virtual void LateUpdate()
	{
		this.GetActions(() =>
		{
			this.m_State?.LateUpdate();
		});
	}

	protected void GetActions(params Action[] actions)
	{
		if (!base.IsDisposed && base.enabled && base.gameObject.activeSelf && this.m_IsActive && !GameManager.Instance.IsPaused && actions != null)
		{
			for (int i = 0; i < actions.Length; i = unchecked(i + 1))
			{
				actions[i]?.Invoke();
			}
		}
	}

	public void SetDisable(bool disable)
	{
		this.Disabled = disable;
	}

	public void SetState(TIdentifier state)
	{
		if (this.m_State == null || !this.m_State.ID.Equals(state))
		{
			string typeName;
			typeName = typeof(TIdentifier).ToString().RemoveSymbols().Replace("State", "") + "State" + state.ToString();
			if (!this.m_ActiveStates.ContainsKey(state))
			{
				this.m_ActiveStates.Add(state, (State<TActor, TIdentifier>)Activator.CreateInstance(Type.GetType(typeName), this, state));
			}
			if (this.m_State != null)
			{
				this.m_PreviousState = this.m_State.ID;
			}
			this.CurrentState = state;
			this.m_State?.OnStateExit();
			this.m_State = this.m_ActiveStates[state];
			this.m_State?.OnStateEnter();
		}
	}

	public void SetPreviousState()
	{
		if (this.m_State != null && this.m_PreviousState != null && !this.m_PreviousState.Equals(this.m_State.ID))
		{
			TIdentifier iD;
			iD = this.m_State.ID;
			this.SetState(this.m_PreviousState);
			this.m_PreviousState = iD;
		}
	}

	public void SetPreviousState(TIdentifier state)
	{
		this.m_PreviousState = state;
	}

	public void SetNextState()
	{
		if (!this.m_NextState.Equals(this.m_State.ID))
		{
			this.SetState(this.m_NextState);
			this.m_NextState = this.CurrentState;
		}
	}

	public void SetNextState(TIdentifier state)
	{
		this.m_NextState = state;
	}

	protected override void OnDisposed()
	{
		this.m_State = null;
		this.m_ActiveStates?.Clear();
		this.m_ActiveStates = null;
		base.OnDisposed();
	}
}
