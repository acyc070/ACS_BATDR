using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIElementSelector : UIElement
{
	[Header("UI Type")]
	[SerializeField]
	private bool m_IsTitle;

	[Header("Slection Left")]
	[SerializeField]
	protected UIButton m_LeftButton;

	[SerializeField]
	private Image m_LeftIcon;

	[Header("Slection Right")]
	[SerializeField]
	protected UIButton m_RightButton;

	[SerializeField]
	private Image m_RightIcon;

	[Header("Slection Label")]
	[SerializeField]
	protected UIElementLabel m_SelectionLabel;

	[Header("Label")]
	[SerializeField]
	protected UIElementLabel m_Label;

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

	private UIElementSelectorDataVO m_DataVO;

	private int m_SelectedIndex;

	private string[] m_Selection;

	public UIButton LeftButton => this.m_LeftButton;

	public UIButton RightButton => this.m_RightButton;

	public UIElementLabel SelectionLabel => this.m_SelectionLabel;

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
		this.m_DataVO = (UIElementSelectorDataVO)_data;
		this.m_Label.Label.text = this.m_DataVO.Label;
		this.m_Label.Label.color = this.InactiveColor;
		this.m_SelectionLabel.Label.color = this.InactiveColor;
		this.m_LeftIcon.color = this.InactiveColor;
		this.m_RightIcon.color = this.InactiveColor;
		this.m_Selection = this.m_DataVO.Selection;
		this.m_SelectedIndex = GameManager.Instance.PlayerSettings.GetInt(this.m_DataVO.PlayerPrefs);
		if (this.m_SelectedIndex >= unchecked(this.m_Selection.Length - 1))
		{
			this.m_RightButton.gameObject.SetActive(value: false);
		}
		else if (this.m_SelectedIndex <= 0)
		{
			this.LeftButton.gameObject.SetActive(value: false);
		}
		this.SetSelection(isInitialize: true);
		this.RemoveListeners();
		this.AddListeners();
	}

	public virtual void ForceOnEnter()
	{
		this.IsSelected = true;
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOColor(this.ActiveColor, 0.1f), this.m_SelectionLabel.Label.DOColor(this.ActiveColor, 0.1f));
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0]);
	}

	public virtual void ForceOnExit()
	{
		this.IsSelected = false;
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOColor(this.InactiveColor, 0.1f), this.m_SelectionLabel.Label.DOColor(this.InactiveColor, 0.1f));
	}

	protected virtual void HandleLeftButtonOnEnter(object sender, EventArgs e)
	{
		this.ForceOnEnter();
		this.m_LeftIcon.DOColor(this.ActiveColor, 0.1f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		this.OnEnter.Send(this);
	}

	protected virtual void HandleLeftButtonOnExit(object sender, EventArgs e)
	{
		this.ForceOnExit();
		this.m_LeftIcon.DOColor(this.InactiveColor, 0.1f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		this.OnExit.Send(this);
	}

	protected virtual void HandleRightButtonOnEnter(object sender, EventArgs e)
	{
		this.ForceOnEnter();
		this.m_RightIcon.DOColor(this.ActiveColor, 0.1f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		this.OnEnter.Send(this);
	}

	protected virtual void HandleRightButtonOnExit(object sender, EventArgs e)
	{
		this.ForceOnExit();
		this.m_RightIcon.DOColor(this.InactiveColor, 0.1f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		this.OnExit.Send(this);
	}

	private void HandleLeftButtonOnClick(object sender, EventArgs e)
	{
		this.m_LeftIcon.DOKill();
		this.m_LeftIcon.color = this.InactiveColor;
		this.m_LeftIcon.DOColor(this.ActiveColor, 0.15f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		this.Reduce();
	}

	private void HandleRightButtonOnClick(object sender, EventArgs e)
	{
		this.m_RightIcon.DOKill();
		this.m_RightIcon.color = this.InactiveColor;
		this.m_RightIcon.DOColor(this.ActiveColor, 0.15f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
		this.Increase();
	}

	public void Increase(bool isController = false)
	{
		unchecked
		{
			if (this.m_SelectedIndex < this.m_Selection.Length - 1)
			{
				this.m_LeftButton.gameObject.SetActive(value: true);
				if (isController)
				{
					this.m_RightIcon.DOKill();
					this.m_RightIcon.color = this.ActiveColor;
					this.m_RightIcon.DOColor(this.InactiveColor, 0.15f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
				}
				this.m_SelectedIndex++;
				this.SetSelection();
				if (this.m_SelectedIndex >= this.m_Selection.Length - 1)
				{
					this.m_RightButton.gameObject.SetActive(value: false);
				}
			}
		}
	}

	public void Reduce(bool isController = false)
	{
		unchecked
		{
			if (this.m_SelectedIndex > 0)
			{
				this.RightButton.gameObject.SetActive(value: true);
				if (isController)
				{
					this.m_LeftIcon.DOKill();
					this.m_LeftIcon.color = this.ActiveColor;
					this.m_LeftIcon.DOColor(this.InactiveColor, 0.15f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
				}
				this.m_SelectedIndex--;
				this.SetSelection();
				if (this.m_SelectedIndex <= 0)
				{
					this.LeftButton.gameObject.SetActive(value: false);
				}
			}
		}
	}

	private void SetSelection(bool isInitialize = false)
	{
		if (!isInitialize)
		{
			GameManager.Instance.PlayerSettings.SetInt(this.m_DataVO.PlayerPrefs, this.m_SelectedIndex);
		}
		if (this.m_DataVO.PlayerPrefs == "RESOLUTION_CURRENT")
		{
			this.m_SelectionLabel.Label.text = this.m_Selection[this.m_SelectedIndex];
		}
		else
		{
			this.m_SelectionLabel.Label.text = TextUtility.GetKey(this.m_Selection[this.m_SelectedIndex]).ToUpper();
		}
	}

	public virtual void AddListeners()
	{
		this.m_LeftButton.OnEnter += this.HandleLeftButtonOnEnter;
		this.m_LeftButton.OnExit += this.HandleLeftButtonOnExit;
		this.m_LeftButton.OnClick += this.HandleLeftButtonOnClick;
		this.m_RightButton.OnEnter += this.HandleRightButtonOnEnter;
		this.m_RightButton.OnExit += this.HandleRightButtonOnExit;
		this.m_RightButton.OnClick += this.HandleRightButtonOnClick;
	}

	public virtual void RemoveListeners()
	{
		this.m_LeftButton.OnEnter -= this.HandleLeftButtonOnEnter;
		this.m_LeftButton.OnExit -= this.HandleLeftButtonOnExit;
		this.m_LeftButton.OnClick -= this.HandleLeftButtonOnClick;
		this.m_RightButton.OnEnter -= this.HandleRightButtonOnEnter;
		this.m_RightButton.OnExit -= this.HandleRightButtonOnExit;
		this.m_RightButton.OnClick -= this.HandleRightButtonOnClick;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.OnEnter = null;
		this.OnExit = null;
		this.OnClick = null;
		this.Callback = null;
		this.m_DataVO = null;
		this.m_Selection = null;
		this.m_LeftIcon.DOKill();
		this.m_RightIcon.DOKill();
		base.OnDisposed();
	}
}
