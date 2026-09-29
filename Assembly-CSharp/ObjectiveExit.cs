using System;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveExit : Objective
{
	[SerializeField]
	private Interactable m_Interactalbe;

	private UIPrompt m_MenuExitPrompt;

	protected override void InternalInitialize()
	{
		this.m_Interactalbe.OnInteract += this.HandleInteractableOnInteract;
		base.SendOnComplete();
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.m_Interactalbe.OnInteract -= this.HandleInteractableOnInteract;
		GameManager.Instance.SoftPause();
		GameManager.Instance.ShowCursor();
		if (this.m_MenuExitPrompt != null)
		{
			this.m_MenuExitPrompt.Dispose();
			this.m_MenuExitPrompt = null;
		}
		this.m_MenuExitPrompt = GameManager.Instance.UIManager.Show<UIPrompt>("UI/GameMenuPrompt/UIPrompt", "GAME MENU PROMPT", new UIPromptDataVO(TextUtility.GetKey("MENU_SETTINGS_MAIN_MENU"), TextUtility.GetKey("MENU_SETTINGS_MAIN_MENU_HEADDER"), new UIElementButtonDataVO("UI/GameMenu/MenuElementButtonControllerLabel", TextUtility.GetKey("MENU_YES"), () =>
		{
			SceneManager.LoadScene("Reset");
		}, "", TextAlignmentOptions.Midline, InputControlType.Action1), new UIElementButtonDataVO("UI/GameMenu/MenuElementButtonControllerLabel", TextUtility.GetKey("MENU_NO"), this.PlayOutMenuExit, "", TextAlignmentOptions.Midline, InputControlType.Action2)));
	}

	private void PlayOutMenuExit()
	{
		GameManager.Instance.SoftUnpause();
		GameManager.Instance.HideCursor();
		this.m_Interactalbe.OnInteract += this.HandleInteractableOnInteract;
		this.m_Interactalbe.ResetAction();
		if (this.m_MenuExitPrompt != null)
		{
			this.m_MenuExitPrompt.PlayOut();
		}
	}
}
