using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementUpgrade : JMonoBehaviour, IUIElement
{
	[SerializeField]
	private RectTransform m_UpgradeLevelsElementContainer;

	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLabel;

	[SerializeField]
	private Image m_Icon;

	[Header("Selection")]
	[SerializeField]
	private GameObject m_SelectedContainer;

	[SerializeField]
	private Image m_SelectedCircle;

	[SerializeField]
	private Color m_SelectedCircleColor;

	[SerializeField]
	private Color m_HeaderLabelColor;

	[SerializeField]
	private Color m_HeaderLabelSelectedColor;

	[SerializeField]
	private UIButton m_Button;

	private UIElementUpgradeDataVO m_DataVO;

	private List<UIElementUpgradeLevel> m_UpgradeLevels = new List<UIElementUpgradeLevel>();

	public string PrefabKey => "UI/Elements/UIElementUpgrade";

	public UpgradeType UpgradeType { get; private set; }

	public bool IsSelected { get; private set; }

	public bool IsFullydUpgraded { get; private set; }

	public event EventHandler OnSelected;

	public void Initialize(object data)
	{
		this.m_DataVO = (UIElementUpgradeDataVO)data;
		this.UpgradeType = this.m_DataVO.UpgradeType;
		bool isFullydUpgraded;
		isFullydUpgraded = true;
		for (int i = 0; i < this.m_DataVO.Levels.Count; i = unchecked(i + 1))
		{
			UIElementUpgradeLevel uIElementUpgradeLevel;
			uIElementUpgradeLevel = GameManager.Instance.AssetManager.CreateAsset<UIElementUpgradeLevel>("UI/Elements/UIElementUpgradeLevel", this.m_DataVO.Levels[i]);
			uIElementUpgradeLevel.transform.SetParent(this.m_UpgradeLevelsElementContainer);
			if (!this.m_UpgradeLevels.Contains(uIElementUpgradeLevel))
			{
				this.m_UpgradeLevels.Add(uIElementUpgradeLevel);
				if (!uIElementUpgradeLevel.Data.IsActive)
				{
					isFullydUpgraded = false;
				}
			}
		}
		this.IsFullydUpgraded = isFullydUpgraded;
		this.m_HeaderLabel.text = this.m_DataVO.Header;
		this.m_DescriptionLabel.text = this.m_DataVO.Description;
		this.m_Icon.sprite = this.m_DataVO.Icon;
		this.m_SelectedContainer.SetActive(value: false);
		this.m_Icon.color = this.m_HeaderLabelColor;
		this.Exit();
		if (!this.IsFullydUpgraded)
		{
			this.AddListeners();
		}
		if (this.m_DataVO.Levels.Count <= 0)
		{
			Color headerLabelColor;
			headerLabelColor = this.m_HeaderLabelColor;
			headerLabelColor.a = 0.2f;
			this.m_Icon.color = headerLabelColor;
			this.m_SelectedCircle.enabled = false;
		}
	}

	public void Initialized()
	{
	}

	public void Select()
	{
		if (this.IsFullydUpgraded)
		{
			return;
		}
		this.RemoveListeners();
		this.Enter();
		this.OnSelected.Send(this);
		this.IsSelected = true;
		this.m_SelectedContainer.SetActive(value: true);
		this.m_Icon.color = this.m_HeaderLabelSelectedColor;
		for (int i = 0; i < this.m_UpgradeLevels.Count; i = unchecked(i + 1))
		{
			UIElementUpgradeLevel uIElementUpgradeLevel;
			uIElementUpgradeLevel = this.m_UpgradeLevels[i];
			if (!uIElementUpgradeLevel.Data.IsActive)
			{
				uIElementUpgradeLevel.Select();
				break;
			}
		}
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0], isIndependentUpdate: true);
	}

	public void Deselect()
	{
		if (this.IsFullydUpgraded)
		{
			return;
		}
		this.IsSelected = false;
		this.m_SelectedContainer.SetActive(value: false);
		this.m_Icon.color = this.m_HeaderLabelColor;
		for (int i = 0; i < this.m_UpgradeLevels.Count; i = unchecked(i + 1))
		{
			UIElementUpgradeLevel uIElementUpgradeLevel;
			uIElementUpgradeLevel = this.m_UpgradeLevels[i];
			if (uIElementUpgradeLevel.IsSelected)
			{
				uIElementUpgradeLevel.Deselect();
			}
		}
		this.Exit();
		this.AddListeners();
	}

	public void Enter()
	{
		if (!this.IsSelected)
		{
			this.m_SelectedCircle.color = this.m_HeaderLabelSelectedColor;
			this.m_HeaderLabel.color = this.m_HeaderLabelSelectedColor;
		}
	}

	public void Exit()
	{
		if (!this.IsSelected)
		{
			this.m_SelectedCircle.color = this.m_SelectedCircleColor;
			this.m_HeaderLabel.color = this.m_HeaderLabelColor;
		}
	}

	private void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.Enter();
	}

	private void HandleButtonOnClick(object sender, EventArgs e)
	{
		this.Select();
	}

	private void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.Exit();
	}

	private void AddListeners()
	{
		this.m_Button.OnEnter += this.HandleButtonOnEnter;
		this.m_Button.OnClick += this.HandleButtonOnClick;
		this.m_Button.OnExit += this.HandleButtonOnExit;
	}

	private void RemoveListeners()
	{
		this.m_Button.OnEnter -= this.HandleButtonOnEnter;
		this.m_Button.OnClick -= this.HandleButtonOnClick;
		this.m_Button.OnExit -= this.HandleButtonOnExit;
	}

	protected override void OnDisposed()
	{
		this.OnSelected = null;
		this.RemoveListeners();
		this.m_DataVO = null;
		if (this.m_UpgradeLevels != null)
		{
			this.m_UpgradeLevels.Clear();
			this.m_UpgradeLevels = null;
		}
		base.OnDisposed();
	}
}
