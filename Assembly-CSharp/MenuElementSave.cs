using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementSave : MenuElementButtonLabel
{
	[Header("Save Labels")]
	[SerializeField]
	private TextMeshProUGUI m_DateTimeLabel;

	[SerializeField]
	private TextMeshProUGUI m_ObjectiveLabel;

	[Header("Save Images")]
	[SerializeField]
	private RawImage m_SaveImage;

	[SerializeField]
	private Image m_FrameImage;

	[SerializeField]
	private Image m_DefaultImage;

	[Header("Frame")]
	[SerializeField]
	private Image m_SlotFrameImage;

	private UIElementSaveDataVO m_Data;

	public string DateTimeLabel => this.m_DateTimeLabel.text;

	public string ObjectiveLabel => this.m_ObjectiveLabel.text;

	public UIElementSaveDataVO Data => this.m_Data;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_Data = (UIElementSaveDataVO)_data;
		base.m_Label.text = this.m_Data.Label;
		base.m_Label.color = base.InactiveColor;
		this.m_DateTimeLabel.text = this.m_Data.DateTimeLabel;
		this.m_DateTimeLabel.color = base.InactiveColor;
		this.m_ObjectiveLabel.text = this.m_Data.MessageLabel;
		this.m_ObjectiveLabel.color = base.InactiveColor;
		this.m_FrameImage.color = base.InactiveColor;
		Color inactiveColor;
		inactiveColor = base.InactiveColor;
		inactiveColor.a = 0f;
		this.m_SlotFrameImage.color = inactiveColor;
		if (this.m_Data.Image != null)
		{
			this.m_SaveImage.texture = this.m_Data.Image;
			this.m_SaveImage.enabled = true;
			this.m_DefaultImage.enabled = false;
		}
		base.m_Sequencer = new Sequencer(isIndependent: true);
	}

	protected override void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.ForceOnEnter();
	}

	protected override void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.ForceOnExit();
	}

	public override void ForceOnEnter()
	{
		base.m_Sequencer?.CreateSequence(base.m_Label.DOColor(base.ActiveColor, 0.1f), this.m_DateTimeLabel.DOColor(base.ActiveColor, 0.1f), this.m_ObjectiveLabel.DOColor(base.ActiveColor, 0.1f), this.m_FrameImage.DOColor(base.ActiveColor, 0.1f), this.m_SlotFrameImage.DOColor(base.ActiveColor, 0.1f));
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0]);
	}

	public override void ForceOnExit()
	{
		Color inactiveColor;
		inactiveColor = base.InactiveColor;
		inactiveColor.a = 0f;
		base.m_Sequencer?.CreateSequence(base.m_Label.DOColor(base.InactiveColor, 0.1f), this.m_DateTimeLabel.DOColor(base.InactiveColor, 0.1f), this.m_ObjectiveLabel.DOColor(base.InactiveColor, 0.1f), this.m_FrameImage.DOColor(base.InactiveColor, 0.1f), this.m_SlotFrameImage.DOColor(inactiveColor, 0.1f));
	}

	protected override void OnDisposed()
	{
		this.m_Data = null;
		base.OnDisposed();
	}
}
