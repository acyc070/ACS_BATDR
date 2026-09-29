using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIElementGameMenuButton : UIElement
{
	[Header("Game Menu Type")]
	[SerializeField]
	private GameMenuType m_GameMenuType;

	[Header("Button")]
	[SerializeField]
	private UIButton m_Button;

	[Header("Visuals")]
	[SerializeField]
	private Image m_Icon;

	[Header("Color Settings")]
	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	[SerializeField]
	protected Color m_ActiveColorReal;

	[SerializeField]
	protected Color m_InactiveColorReal;

	private Sequencer m_Sequencer;

	public GameMenuType GameMenuType => this.m_GameMenuType;

	public UIButton Button => this.m_Button;

	public Image Icon => this.m_Icon;

	protected Color ActiveColor
	{
		get
		{
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
			if (!GameManager.Instance.IsRealWorld)
			{
				return this.m_InactiveColor;
			}
			return this.m_InactiveColorReal;
		}
	}

	public event EventHandler OnEnter;

	public event EventHandler OnClick;

	public event EventHandler OnSelected;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_Sequencer = new Sequencer(isIndependent: true);
		this.m_Icon.color = this.InactiveColor;
		this.m_Button.OnEnter += this.HandleButtonOnEnter;
		this.m_Button.OnExit += this.HandleButtonOnExit;
		this.m_Button.OnClick += this.HandleButtonOnClick;
	}

	public void ForceOnEnter()
	{
		this.m_Sequencer?.CreateSequence(this.m_Icon.DOColor(this.ActiveColor, 0.1f), this.m_Icon.rectTransform.DOScale(1.05f, 0.1f));
		if (!this.m_Button.selected)
		{
			this.OnEnter.Send(this);
		}
		else
		{
			this.OnSelected.Send(this);
		}
	}

	public void ForceOnExit()
	{
		this.m_Sequencer?.CreateSequence(this.m_Icon.DOColor(this.InactiveColor, 0.1f), this.m_Icon.rectTransform.DOScale(1f, 0.1f));
	}

	protected virtual void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.ForceOnEnter();
	}

	protected virtual void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.ForceOnExit();
	}

	private void HandleButtonOnClick(object sender, EventArgs e)
	{
		this.OnClick.Send(this);
	}

	private void RemoveListeners()
	{
		this.m_Button.OnEnter -= this.HandleButtonOnEnter;
		this.m_Button.OnExit -= this.HandleButtonOnExit;
		this.m_Button.OnClick -= this.HandleButtonOnClick;
	}

	protected override void OnDisposed()
	{
		this.OnEnter = null;
		this.OnClick = null;
		this.OnSelected = null;
		this.m_Sequencer?.Dispose();
		this.m_Sequencer = null;
		this.RemoveListeners();
		base.OnDisposed();
	}
}
