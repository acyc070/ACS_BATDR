using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class MenuItemButton : JMonoBehaviour
{
	[Header("TextMeshProUGUI")]
	[SerializeField]
	private Localize m_Label;

	[SerializeField]
	private TextMeshProUGUI m_LabelUGUI;

	[Header("Images")]
	[SerializeField]
	private GameObject m_Arrows;

	[Header("Button")]
	[SerializeField]
	private UIButton m_Button;

	private Action m_Callback;

	private bool m_IsClicked;

	public UIButton Button => this.m_Button;

	public string Label => this.m_Label.Term;

	public event EventHandler OnSelected;

	public event EventHandler OnClick;

	public void Init(UIMenuItemButtonDataVO vo)
	{
		if ((bool)this.m_Label)
		{
			this.m_Label.SetTerm(vo.Label);
		}
		if ((bool)this.m_LabelUGUI)
		{
			this.m_LabelUGUI.text = vo.Label;
		}
		this.m_Callback = vo.Callback;
		if ((bool)this.m_Arrows)
		{
			this.m_Arrows.SetActive(value: false);
		}
		this.AddListeners();
	}

	public void InitBasic()
	{
		this.m_Button.OnClick += this.HandleButtonBasicOnClick;
	}

	private void HandleButtonOnSelected(object sender, EventArgs e)
	{
		this.OnSelected.Send(this);
		if ((bool)this.m_Arrows)
		{
			this.m_Arrows.SetActive(value: true);
		}
	}

	private void HandleButtonOnDeselected(object sender, EventArgs e)
	{
		if ((bool)this.m_Arrows)
		{
			this.m_Arrows.SetActive(value: false);
		}
	}

	private void HandleButtonBasicOnClick(object sender, EventArgs e)
	{
		this.OnClick.Send(this);
	}

	private void HandleButtonOnClick(object sender, EventArgs e)
	{
		this.RemoveListeners();
		if ((bool)this.m_Arrows)
		{
			this.m_Arrows.SetActive(value: true);
		}
		this.m_IsClicked = true;
		this.OnClick.Send(this);
		if (this.m_Callback != null)
		{
			this.m_Callback();
		}
		this.m_Button.OnClick += this.HandleButtonOnClick;
	}

	public void Deselect()
	{
		if (this.m_IsClicked)
		{
			this.AddListeners();
			this.m_IsClicked = false;
		}
		if ((bool)this.m_Arrows)
		{
			this.m_Arrows.SetActive(value: false);
		}
	}

	private void AddListeners()
	{
		this.RemoveListeners();
		this.m_Button.OnClick += this.HandleButtonOnClick;
		this.AddSelectionListeners();
	}

	private void RemoveListeners()
	{
		this.m_Button.OnClick -= this.HandleButtonOnClick;
		this.RemoveSelectionListeners();
	}

	private void AddSelectionListeners()
	{
		if ((bool)this.m_Arrows)
		{
			this.m_Button.OnEnter += this.HandleButtonOnSelected;
			this.m_Button.OnExit += this.HandleButtonOnDeselected;
		}
	}

	private void RemoveSelectionListeners()
	{
		if ((bool)this.m_Arrows)
		{
			this.m_Button.OnEnter -= this.HandleButtonOnSelected;
			this.m_Button.OnExit -= this.HandleButtonOnDeselected;
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.OnSelected = null;
		this.OnClick = null;
		this.m_Callback = null;
		base.OnDisposed();
	}
}
