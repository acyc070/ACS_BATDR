using System;
using UnityEngine;

public class MenuElementButton : UIElement
{
	[Header("Button")]
	[SerializeField]
	protected UIButton m_Button;

	public UIButton Button => this.m_Button;

	public Action Callback { get; protected set; }

	public string InvokeCallback { get; protected set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnClick;

	public event EventHandler OnCallbackInvoked;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.Callback = (_data as UIElementButtonDataVO).Callback;
		this.InvokeCallback = (_data as UIElementButtonDataVO).InvokeCallback;
		this.AddListeners();
	}

	public virtual void ForceOnEnter()
	{
	}

	public virtual void ForceOnExit()
	{
	}

	protected virtual void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.OnEnter.Send(this);
	}

	protected virtual void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.OnExit.Send(this);
	}

	protected virtual void HandleButtonOnClick(object sender, EventArgs e)
	{
		this.RemoveListeners();
		this.GetCallback();
		this.OnClick.Send(this);
	}

	public void GetCallback()
	{
		if (this.Callback != null)
		{
			this.Callback();
			this.OnCallbackInvoked.Send(this);
		}
	}

	public virtual void AddListeners()
	{
		this.m_Button.OnEnter += this.HandleButtonOnEnter;
		this.m_Button.OnExit += this.HandleButtonOnExit;
		this.m_Button.OnClick += this.HandleButtonOnClick;
	}

	public virtual void RemoveListeners()
	{
		this.m_Button.OnEnter -= this.HandleButtonOnEnter;
		this.m_Button.OnExit -= this.HandleButtonOnExit;
		this.m_Button.OnClick -= this.HandleButtonOnClick;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.OnEnter = null;
		this.OnExit = null;
		this.OnClick = null;
		this.OnCallbackInvoked = null;
		this.Callback = null;
		base.OnDisposed();
	}
}
