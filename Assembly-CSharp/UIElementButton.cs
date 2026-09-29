using System;
using UnityEngine;

public class UIElementButton : UIElement
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

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		if (_data is UIElementButtonDataVO uIElementButtonDataVO)
		{
			this.Callback = uIElementButtonDataVO.Callback;
			this.InvokeCallback = uIElementButtonDataVO.InvokeCallback;
		}
		this.AddListeners();
	}

	public virtual void ForceOnEnter()
	{
	}

	public virtual void ForceOnExit()
	{
	}

	public virtual void ForceOnClik()
	{
	}

	protected void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.InternalHandleButtonOnEnter(sender, e);
		this.OnEnter.Send(this);
	}

	protected virtual void InternalHandleButtonOnEnter(object sender, EventArgs e)
	{
	}

	protected void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.InternalHandleButtonOnExit(sender, e);
		this.OnExit.Send(this);
	}

	protected virtual void InternalHandleButtonOnExit(object sender, EventArgs e)
	{
	}

	protected virtual void HandleButtonOnClick(object sender, EventArgs e)
	{
		this.RemoveListeners();
		this.Callback?.Invoke();
		this.OnClick.Send(this);
	}

	protected void SendOnEnter()
	{
		this.OnEnter.Send(this);
	}

	protected void SendOnExit()
	{
		this.OnExit.Send(this);
	}

	protected void SendOnClick()
	{
		this.OnClick.Send(this);
	}

	public void ResetButton()
	{
		this.InteralResetButton();
	}

	protected virtual void InteralResetButton()
	{
	}

	protected virtual void AddListeners()
	{
		this.m_Button.OnEnter += this.HandleButtonOnEnter;
		this.m_Button.OnExit += this.HandleButtonOnExit;
		this.m_Button.OnClick += this.HandleButtonOnClick;
	}

	protected virtual void RemoveListeners()
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
		this.Callback = null;
		base.OnDisposed();
	}
}
