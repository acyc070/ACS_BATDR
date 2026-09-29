using System;
using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementButtonLabel : MenuElementButton
{
	[Header("Button Label")]
	[SerializeField]
	protected TextMeshProUGUI m_Label;

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

	[Header("Controller")]
	[SerializeField]
	private RectTransform m_ControllerButton;

	[SerializeField]
	private Image m_ControllerButtonIcon;

	[SerializeField]
	private Image m_ControllerButtonInput;

	protected Sequencer m_Sequencer;

	public TextMeshProUGUI Label => this.m_Label;

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

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_Label.text = (_data as UIElementButtonDataVO).Label;
		this.m_Label.alignment = (_data as UIElementButtonDataVO).Alignment;
		this.m_Label.color = this.InactiveColor;
		this.m_Label.fontSize = this.m_ExitSize;
		this.m_Sequencer = new Sequencer(isIndependent: true);
		if (this.m_ControllerButton != null)
		{
			this.m_ControllerButton.gameObject.SetActive(value: false);
			if (_data is UIElementButtonDataVO uIElementButtonDataVO && GameManager.Instance.HasController && uIElementButtonDataVO.InputControlType != InputControlType.None)
			{
				Vector2 sizeDelta;
				sizeDelta = this.Label.rectTransform.sizeDelta;
				sizeDelta.x = this.Label.GetPreferredValues().x;
				this.Label.rectTransform.sizeDelta = sizeDelta;
				base.m_Button.GetComponent<RectTransform>().sizeDelta = sizeDelta;
				Vector2 zero;
				zero = Vector2.zero;
				zero.x -= base.m_Button.GetComponent<RectTransform>().sizeDelta.x / 2f + this.m_ControllerButton.sizeDelta.x / 2f;
				this.m_ControllerButton.anchoredPosition = zero;
				this.m_ControllerButtonIcon.color = this.InactiveColor;
				this.m_ControllerButtonInput.color = this.ActiveColor;
				this.m_ControllerButtonInput.sprite = GameManager.Instance.ControllerInput.GetSprite(uIElementButtonDataVO.InputControlType);
				this.m_ControllerButton.gameObject.SetActive(value: true);
			}
		}
	}

	public override void ForceOnEnter()
	{
		this.m_Label.color = this.ActiveColor;
		this.m_Label.fontSize = this.m_EnterSize;
	}

	public override void ForceOnExit()
	{
		this.m_Label.color = this.InactiveColor;
		this.m_Label.fontSize = this.m_ExitSize;
	}

	protected override void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.m_Sequencer?.CreateSequence(this.m_Label.DOFontSize(this.m_ExitSize, 0.1f), this.m_Label.DOColor(this.InactiveColor, 0.1f));
	}

	protected override void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.m_Sequencer?.CreateSequence(this.m_Label.DOFontSize(this.m_EnterSize, 0.1f), this.m_Label.DOColor(this.ActiveColor, 0.1f));
	}

	protected override void OnDisposed()
	{
		this.m_Sequencer?.Dispose();
		this.m_Sequencer = null;
		base.OnDisposed();
	}
}
