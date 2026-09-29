using System;
using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementButtonLabel : UIElementButton
{
	[Header("Button Label")]
	[SerializeField]
	protected UIElementLabel m_Label;

	[Header("Select Image")]
	[SerializeField]
	protected Image m_SelectImage;

	[Header("Label Settings")]
	[SerializeField]
	protected float m_EnterSize = 100f;

	[SerializeField]
	protected float m_ExitSize = 76f;

	[Space]
	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	[SerializeField]
	protected Color m_ActiveColorReal;

	[SerializeField]
	protected Color m_InactiveColorReal;

	[SerializeField]
	protected Color m_DisabledColor;

	[Header("Controller")]
	[SerializeField]
	private RectTransform m_ControllerButton;

	[SerializeField]
	private Image m_ControllerButtonIcon;

	[SerializeField]
	private Image m_ControllerButtonInput;

	private Sequencer m_Sequencer;

	protected bool m_IsDisabled;

	public string Label => this.m_Label.Label.text;

	public TextMeshProUGUI LabelUI => this.m_Label.Label;

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

	protected Color DisabledColor
	{
		get
		{
			if (GameManager.Instance.IsRealWorld)
			{
				Color inactiveColorReal;
				inactiveColorReal = this.m_InactiveColorReal;
				inactiveColorReal.a = 0.25f;
				return inactiveColorReal;
			}
			return this.m_DisabledColor;
		}
	}

	protected virtual bool m_ForceTextSize => true;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		if (this.m_ControllerButton != null)
		{
			this.m_ControllerButton.gameObject.SetActive(value: false);
		}
		this.m_Sequencer = new Sequencer(isIndependent: true);
		if (_data is UIElementButtonDataVO uIElementButtonDataVO)
		{
			if (uIElementButtonDataVO.LabelDataVO != null)
			{
				this.m_Label.Label.text = uIElementButtonDataVO.LabelDataVO.Label;
				this.m_Label.Label.color = uIElementButtonDataVO.LabelDataVO.Color;
				this.m_Label.Label.fontSizeMax = uIElementButtonDataVO.LabelDataVO.FontSize;
			}
			else
			{
				this.m_Label.Label.text = uIElementButtonDataVO.Label;
				this.m_Label.Label.color = this.InactiveColor;
				this.m_Label.Label.fontSizeMax = this.m_ExitSize;
			}
		}
		else if (_data is UIElementTitleMenuButtonDataVO uIElementTitleMenuButtonDataVO)
		{
			this.m_Label.Label.text = uIElementTitleMenuButtonDataVO.Label;
			this.m_Label.Label.color = this.InactiveColor;
			this.m_Label.Label.fontSizeMax = this.m_ExitSize;
		}
		else if (_data is UIElementGameMenuSettingsButtonDataVO uIElementGameMenuSettingsButtonDataVO)
		{
			this.m_Label.Label.text = uIElementGameMenuSettingsButtonDataVO.Label;
			this.m_Label.Label.color = this.InactiveColor;
			this.m_Label.Label.fontSizeMax = this.m_ExitSize;
			this.m_Label.Label.fontSize = this.m_ExitSize;
			this.m_Label.Label.alignment = TextAlignmentOptions.MidlineLeft;
		}
		else if (_data is UIElementGameMenuMenuButtonDataVO uIElementGameMenuMenuButtonDataVO)
		{
			this.m_Label.Label.text = uIElementGameMenuMenuButtonDataVO.Label;
			this.m_Label.Label.color = this.InactiveColor;
			this.m_Label.Label.fontSizeMax = this.m_ExitSize;
			this.m_Label.Label.fontSize = this.m_ExitSize;
			this.m_Label.Label.alignment = TextAlignmentOptions.MidlineLeft;
		}
		if ((bool)this.m_SelectImage)
		{
			this.m_SelectImage.enabled = false;
			this.m_SelectImage.color = new Color(this.m_SelectImage.color.r, this.m_SelectImage.color.g, this.m_SelectImage.color.b, 0f);
		}
		if (this.m_ForceTextSize)
		{
			Vector2 sizeDelta;
			sizeDelta = this.m_Label.Label.rectTransform.sizeDelta;
			sizeDelta.x = this.m_Label.Label.GetPreferredValues().x;
			this.m_Label.Label.rectTransform.sizeDelta = sizeDelta;
			base.m_Button.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		}
		if (_data is UIElementButtonDataVO uIElementButtonDataVO2 && GameManager.Instance.HasController && this.m_ControllerButton != null && uIElementButtonDataVO2.InputControlType != InputControlType.None)
		{
			Vector2 zero;
			zero = Vector2.zero;
			zero.x -= base.m_Button.GetComponent<RectTransform>().sizeDelta.x / 2f + this.m_ControllerButton.sizeDelta.x / 2f;
			this.m_ControllerButton.anchoredPosition = zero;
			this.m_ControllerButtonIcon.color = this.InactiveColor;
			this.m_ControllerButtonInput.color = this.ActiveColor;
			this.m_ControllerButtonInput.sprite = GameManager.Instance.ControllerInput.GetSprite(uIElementButtonDataVO2.InputControlType);
			this.m_ControllerButton.gameObject.SetActive(value: true);
		}
	}

	public void SetDisabled(bool disabled)
	{
		this.m_IsDisabled = disabled;
		if (this.m_IsDisabled)
		{
			this.m_Label.Label.color = this.DisabledColor;
		}
	}

	public override void ForceOnEnter()
	{
		if (!this.m_IsDisabled)
		{
			this.m_Label.Label.color = this.ActiveColor;
			this.m_Label.Label.fontSize = this.m_EnterSize;
			if ((bool)this.m_SelectImage)
			{
				this.m_SelectImage.enabled = true;
				this.m_SelectImage.color = new Color(this.m_SelectImage.color.r, this.m_SelectImage.color.g, this.m_SelectImage.color.b, 1f);
			}
			GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0], isIndependentUpdate: true);
		}
		else
		{
			this.m_Label.Label.color = this.DisabledColor;
			this.m_Label.Label.fontSize = this.m_EnterSize;
		}
		base.SendOnEnter();
	}

	public override void ForceOnExit()
	{
		if (!this.m_IsDisabled)
		{
			this.m_Label.Label.color = this.InactiveColor;
			this.m_Label.Label.fontSize = this.m_ExitSize;
			if ((bool)this.m_SelectImage)
			{
				this.m_SelectImage.enabled = true;
				this.m_SelectImage.color = new Color(this.m_SelectImage.color.r, this.m_SelectImage.color.g, this.m_SelectImage.color.b, 0f);
			}
		}
		else
		{
			this.m_Label.Label.color = this.DisabledColor;
			this.m_Label.Label.fontSize = this.m_ExitSize;
		}
		base.SendOnExit();
	}

	public override void ForceOnClik()
	{
		base.SendOnClick();
	}

	protected override void InternalHandleButtonOnExit(object sender, EventArgs e)
	{
		if (!this.m_IsDisabled)
		{
			this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOFontSize(this.m_ExitSize, 0.1f), this.m_Label.Label.DOColor(this.InactiveColor, 0.1f));
			if ((bool)this.m_SelectImage)
			{
				this.m_Sequencer?.Insert(0f, this.m_SelectImage.DOFade(0f, 0.1f).OnComplete(() =>
				{
					this.m_SelectImage.enabled = true;
				}));
			}
		}
		else
		{
			this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOFontSize(this.m_ExitSize, 0.1f), this.m_Label.Label.DOColor(this.DisabledColor, 0.1f));
		}
	}

	protected override void InternalHandleButtonOnEnter(object sender, EventArgs e)
	{
		if (!this.m_IsDisabled)
		{
			this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOFontSize(this.m_EnterSize, 0.1f), this.m_Label.Label.DOColor(this.ActiveColor, 0.1f));
			if ((bool)this.m_SelectImage)
			{
				this.m_SelectImage.enabled = true;
				this.m_Sequencer?.Insert(0f, this.m_SelectImage.DOFade(1f, 0.1f));
			}
		}
		else
		{
			this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOFontSize(this.m_EnterSize, 0.1f), this.m_Label.Label.DOColor(this.DisabledColor, 0.1f));
		}
	}

	public void ResetListeners()
	{
		this.RemoveListeners();
		this.AddListeners();
	}

	protected override void OnDisposed()
	{
		this.m_Sequencer?.Dispose();
		this.m_Sequencer = null;
		base.OnDisposed();
	}
}
