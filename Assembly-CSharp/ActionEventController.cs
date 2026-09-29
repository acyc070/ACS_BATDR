using System;
using UnityEngine;

[DefaultExecutionOrder(105)]
public abstract class ActionEventController<TContent, TData> : ActionEventController where TContent : ActionEventContent
{
	[Header("Data")]
	[SerializeField]
	protected TData m_Data;

	[Header("Initialize Settings")]
	[SerializeField]
	private bool m_InitializeOnAwake = true;

	public TData Data => this.m_Data;

	public bool InitializeOnAwake => this.m_InitializeOnAwake;

	protected sealed override ActionEventContent m_Content { get; set; }

	public override void Awake()
	{
		if (this.m_InitializeOnAwake)
		{
			this.Initialize();
		}
	}

	public override void Initialize()
	{
		if (this.m_Content == null)
		{
			this.m_Content = base.GetComponentInChildren<TContent>(includeInactive: true);
		}
		if (this.m_Content != null)
		{
			this.OnInitialize();
			this.m_Content.Initialize(this);
			this.OnInitialized();
		}
	}

	protected virtual void OnInitialize()
	{
	}

	protected virtual void OnInitialized()
	{
	}
}
public abstract class ActionEventController : JMonoBehaviour
{
	public Transform InteractableTransform => base.GetComponentInChildren<Interactable>()?.transform ?? base.transform;

	public ActionEventContent Content => this.m_Content;

	protected abstract ActionEventContent m_Content { get; set; }

	public event EventHandler OnActivate;

	public event EventHandler OnActivated;

	public event EventHandler OnDeactivate;

	public event EventHandler OnDeactivated;

	public event EventHandler OnInactive;

	public event EventHandler OnDisabled;

	public event EventHandler OnEnabled;

	public virtual void Initialize()
	{
	}

	public void SendOnActivate()
	{
		this.OnActivate.Send(this);
	}

	public void SendOnActivated()
	{
		this.OnActivated.Send(this);
	}

	public void SendOnOnDeactivate()
	{
		this.OnDeactivate.Send(this);
	}

	public void SendOnOnDeactivated()
	{
		this.OnDeactivated.Send(this);
	}

	public void SendOnInactive()
	{
		this.OnInactive.Send(this);
	}

	public void SendOnDisabled()
	{
		this.OnDisabled.Send(this);
	}

	public void SendOnEnabled()
	{
		this.OnEnabled.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnActivate = null;
		this.OnActivated = null;
		this.OnDeactivate = null;
		this.OnDeactivated = null;
		this.OnInactive = null;
		this.OnDisabled = null;
		this.OnEnabled = null;
		base.OnDisposed();
	}
}
