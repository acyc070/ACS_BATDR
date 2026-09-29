using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIElementToggle : UIElement
{
	[Header("UI Type")]
	[SerializeField]
	private bool m_IsTitle;

	[Header("Toggle")]
	[SerializeField]
	protected UIToggle m_Toggle;

	[SerializeField]
	protected UIElementLabel m_Label;

	[Header("Toggle Images")]
	[SerializeField]
	private Image m_ToggleBorder;

	[SerializeField]
	private Image m_ToggleBG;

	[SerializeField]
	private Image m_ToggleCheckmark;

	[Header("Toggle BG Color")]
	[SerializeField]
	private Color m_ToggleBGColor;

	[SerializeField]
	private Color m_ToggleBGColorReal;

	[Header("Select Settings")]
	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	[SerializeField]
	protected Color m_ActiveColorReal;

	[SerializeField]
	protected Color m_InactiveColorReal;

	private Sequencer m_Sequencer;

	private UIElementToggleDataVO m_DataVO;

	public UIToggle Toggle => this.m_Toggle;

	public UIElementLabel Label => this.m_Label;

	protected Color ActiveColor
	{
		get
		{
			if (this.m_IsTitle)
			{
				return this.m_ActiveColor;
			}
			if (!GameManager.Instance.IsRealWorld)
			{
				return this.m_ActiveColor;
			}
			return this.m_ActiveColorReal;
		}
	}

	protected Color InactiveColor
	{
		get
		{
			if (this.m_IsTitle)
			{
				return this.m_InactiveColor;
			}
			if (!GameManager.Instance.IsRealWorld)
			{
				return this.m_InactiveColor;
			}
			return this.m_InactiveColorReal;
		}
	}

	public Action Callback { get; protected set; }

	public string InvokeCallback { get; protected set; }

	public bool IsSelected { get; private set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnClick;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_Sequencer = new Sequencer(isIndependent: true);
		this.m_DataVO = (UIElementToggleDataVO)_data;
		this.m_Label.Label.text = this.m_DataVO.Label;
		this.m_Label.Label.color = this.InactiveColor;
		this.m_ToggleBorder.color = this.InactiveColor;
		if (!this.m_IsTitle && this.m_ToggleBG != null)
		{
			this.m_ToggleBG.color = (GameManager.Instance.IsRealWorld ? this.m_ToggleBGColorReal : this.m_ToggleBGColor);
		}
		this.m_ToggleCheckmark.color = this.InactiveColor;
		this.RemoveListeners();
		this.AddListeners();
		this.m_Toggle.isOn = GameManager.Instance.PlayerSettings.GetBool(this.m_DataVO.PlayerPrefs);
	}

	public virtual void ForceOnEnter()
	{
		this.IsSelected = true;
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOColor(this.ActiveColor, 0.1f), this.m_ToggleBorder.DOColor(this.ActiveColor, 0.1f));
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0]);
	}

	public virtual void ForceOnExit()
	{
		this.IsSelected = false;
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOColor(this.InactiveColor, 0.1f), this.m_ToggleBorder.DOColor(this.InactiveColor, 0.1f));
	}

	protected virtual void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.ForceOnEnter();
		this.OnEnter.Send(this);
	}

	protected virtual void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.ForceOnExit();
		this.OnExit.Send(this);
	}

	protected virtual void HandleButtonOnClick(object sender, EventArgs e)
	{
		this.OnClick.Send(this);
	}

	private void ToggleOnChangedValue(bool value)
	{
		GameManager.Instance.PlayerSettings.SetBool(this.m_DataVO.PlayerPrefs, value);
	}

	public void ChangeValue()
	{
		this.m_Toggle.isOn = !this.m_Toggle.isOn;
	}

	public virtual void AddListeners()
	{
		this.m_Toggle.OnEnter += this.HandleButtonOnEnter;
		this.m_Toggle.OnExit += this.HandleButtonOnExit;
		this.m_Toggle.OnClick += this.HandleButtonOnClick;
		this.m_Toggle.onValueChanged.AddListener(this.ToggleOnChangedValue);
	}

	public virtual void RemoveListeners()
	{
		this.m_Toggle.OnEnter -= this.HandleButtonOnEnter;
		this.m_Toggle.OnExit -= this.HandleButtonOnExit;
		this.m_Toggle.OnClick -= this.HandleButtonOnClick;
		this.m_Toggle.onValueChanged.RemoveListener(this.ToggleOnChangedValue);
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
