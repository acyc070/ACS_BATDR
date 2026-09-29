using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementSaveSlot : MenuElementButtonLabel
{
	[Header("Save Slot Settings")]
	[SerializeField]
	protected TextMeshProUGUI m_NewSaveGameLabel;

	[SerializeField]
	private Image[] m_Borders;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_NewSaveGameLabel.text = TextUtility.GetKey("MENU_SETTINGS_SAVE_GAME_NEW");
		this.m_NewSaveGameLabel.color = base.InactiveColor;
		for (int i = 0; i < this.m_Borders.Length; i = unchecked(i + 1))
		{
			this.m_Borders[i].color = base.InactiveColor;
		}
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
		base.m_Sequencer?.RestartSequence();
		base.m_Sequencer?.Insert(0f, base.m_Label.DOColor(base.ActiveColor, 0.1f));
		base.m_Sequencer?.Insert(0f, this.m_NewSaveGameLabel.DOColor(base.ActiveColor, 0.1f));
		for (int i = 0; i < this.m_Borders.Length; i = unchecked(i + 1))
		{
			Image target;
			target = this.m_Borders[i];
			base.m_Sequencer?.Insert(0f, target.DOColor(base.ActiveColor, 0.1f));
		}
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0]);
	}

	public override void ForceOnExit()
	{
		base.m_Sequencer?.RestartSequence();
		base.m_Sequencer?.Insert(0f, base.m_Label.DOColor(base.InactiveColor, 0.1f));
		base.m_Sequencer?.Insert(0f, this.m_NewSaveGameLabel.DOColor(base.InactiveColor, 0.1f));
		for (int i = 0; i < this.m_Borders.Length; i = unchecked(i + 1))
		{
			Image target;
			target = this.m_Borders[i];
			base.m_Sequencer?.Insert(0f, target.DOColor(base.InactiveColor, 0.1f));
		}
	}
}
