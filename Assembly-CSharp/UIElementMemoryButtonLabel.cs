using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIElementMemoryButtonLabel : UIElementButton
{
	[Header("Button Label")]
	[SerializeField]
	protected UIElementLabel m_Label;

	[Header("Label Settings")]
	[SerializeField]
	protected float m_EnterSize = 100f;

	[SerializeField]
	protected float m_ExitSize = 76f;

	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	[SerializeField]
	protected Material m_FontMaterialEnter;

	[SerializeField]
	protected Material m_FontMaterialExit;

	[SerializeField]
	protected Image m_SelectImage;

	private Sequencer m_Sequencer;

	private UIElementMemoryDataVO m_DataVO;

	public string Label => this.m_Label.Label.text;

	public UIElementMemoryDataVO DataVO => this.m_DataVO;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_DataVO = (UIElementMemoryDataVO)_data;
		this.m_Sequencer = new Sequencer(isIndependent: true);
		if (this.m_DataVO.Button.LabelDataVO != null)
		{
			this.m_Label.Label.text = this.m_DataVO.Button.LabelDataVO.Label;
			this.m_Label.Label.color = this.m_DataVO.Button.LabelDataVO.Color;
			this.m_Label.Label.fontSizeMax = this.m_DataVO.Button.LabelDataVO.FontSize;
		}
		else
		{
			this.m_Label.Label.text = this.m_DataVO.Button.Label;
			this.m_Label.Label.color = this.m_InactiveColor;
			this.m_Label.Label.fontSizeMax = this.m_ExitSize;
		}
		this.SelectImageOff();
	}

	public override void ForceOnEnter()
	{
		this.m_Label.Label.color = this.m_ActiveColor;
		this.m_Label.Label.fontSize = this.m_EnterSize;
		if ((bool)this.m_FontMaterialEnter)
		{
			this.m_Label.Label.fontSharedMaterial = this.m_FontMaterialEnter;
		}
		if ((bool)this.m_SelectImage)
		{
			this.m_SelectImage.enabled = true;
			this.m_SelectImage.color = new Color(this.m_SelectImage.color.r, this.m_SelectImage.color.g, this.m_SelectImage.color.b, 1f);
		}
	}

	public override void ForceOnExit()
	{
		this.m_Label.Label.color = this.m_InactiveColor;
		this.m_Label.Label.fontSize = this.m_ExitSize;
		if ((bool)this.m_FontMaterialExit)
		{
			this.m_Label.Label.fontSharedMaterial = this.m_FontMaterialExit;
		}
		this.SelectImageOff();
	}

	private void SelectImageOff()
	{
		if ((bool)this.m_SelectImage)
		{
			this.m_SelectImage.enabled = false;
			this.m_SelectImage.color = new Color(this.m_SelectImage.color.r, this.m_SelectImage.color.g, this.m_SelectImage.color.b, 0f);
		}
	}

	protected override void InternalHandleButtonOnExit(object sender, EventArgs e)
	{
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOFontSize(this.m_ExitSize, 0.1f), this.m_Label.Label.DOColor(this.m_InactiveColor, 0.1f));
		if ((bool)this.m_FontMaterialExit)
		{
			this.m_Label.Label.fontSharedMaterial = this.m_FontMaterialExit;
		}
	}

	protected override void InternalHandleButtonOnEnter(object sender, EventArgs e)
	{
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOFontSize(this.m_EnterSize, 0.1f), this.m_Label.Label.DOColor(this.m_ActiveColor, 0.1f));
		if ((bool)this.m_FontMaterialEnter)
		{
			this.m_Label.Label.fontSharedMaterial = this.m_FontMaterialEnter;
		}
	}

	protected override void InteralResetButton()
	{
		this.ForceOnExit();
		this.ResetListeners();
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
