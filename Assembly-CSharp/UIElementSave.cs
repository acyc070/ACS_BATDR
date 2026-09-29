using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementSave : UIElementButtonLabel
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

	private UIElementSaveDataVO m_Data;

	private Sequencer m_Sequencer;

	public string DateTimeLabel => this.m_DateTimeLabel.text;

	public string ObjectiveLabel => this.m_ObjectiveLabel.text;

	protected override bool m_ForceTextSize => false;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_Data = (UIElementSaveDataVO)_data;
		base.m_Label.Label.text = this.m_Data.Label;
		this.m_DateTimeLabel.text = this.m_Data.DateTimeLabel;
		this.m_ObjectiveLabel.text = this.m_Data.MessageLabel;
		if (this.m_Data.Image != null)
		{
			this.m_SaveImage.texture = this.m_Data.Image;
			this.m_SaveImage.enabled = true;
			this.m_DefaultImage.enabled = false;
		}
		this.m_Sequencer = new Sequencer(isIndependent: true);
		base.Button.OnEnter += base.HandleButtonOnEnter;
		base.Button.OnExit += base.HandleButtonOnExit;
	}

	protected override void InternalHandleButtonOnEnter(object sender, EventArgs e)
	{
		this.ForceOnEnter();
	}

	protected override void InternalHandleButtonOnExit(object sender, EventArgs e)
	{
		this.ForceOnExit();
	}

	public override void ForceOnEnter()
	{
		this.m_Sequencer?.CreateSequence(base.m_Label.Label.DOColor(base.m_ActiveColor, 0.1f), this.m_DateTimeLabel.DOColor(base.m_ActiveColor, 0.1f), this.m_ObjectiveLabel.DOColor(base.m_ActiveColor, 0.1f), this.m_FrameImage.DOColor(base.m_ActiveColor, 0.1f));
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0]);
	}

	public override void ForceOnExit()
	{
		this.m_Sequencer?.CreateSequence(base.m_Label.Label.DOColor(base.m_InactiveColor, 0.1f), this.m_DateTimeLabel.DOColor(base.m_InactiveColor, 0.1f), this.m_ObjectiveLabel.DOColor(base.m_InactiveColor, 0.1f), this.m_FrameImage.DOColor(base.m_InactiveColor, 0.1f));
	}

	protected override void OnDisposed()
	{
		this.m_Sequencer?.Kill();
		this.m_Sequencer = null;
		base.OnDisposed();
	}
}
