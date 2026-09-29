using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveMainTitle : Objective
{
	protected override void InternalInitialize()
	{
		GameManager.Instance.LockPause();
		SceneManager.LoadScene("Empty");
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Dispose();
			GameManager.Instance.Player = null;
		}
		GameManager.Instance.ClearInteraction();
		GameManager.Instance.ClearInteractInvalid();
		GameManager.Instance.ClearNotificationText();
		GameManager.Instance.ClearNotificationBox();
		GameManager.Instance.ClearCrosshair();
		GameManager.Instance.ClearAudioLog();
		GameManager.Instance.ClearCutsceneBars();
		GameManager.Instance.ClearObjective();
		GameManager.Instance.ClearCrosshair();
		GameManager.Instance.ClearSubtitles();
		GameManager.Instance.ClearTeleport();
		GameManager.Instance.SectionManager?.Clear();
		GameManager.Instance.RespawnManager?.Clear();
		GameManager.Instance.InkDemonManager?.SetActive(active: false);
		GameManager.Instance.GameData.CurrentSave.SetCursorState(414);
		GameManager.Instance.UIManager.Show<UIMainTitle>("UI/Videos/UIMainTitle", "VIEW").OnPlayOutComplete += this.HandleUIMainTitleOnPlayOutComplete;
	}

	private void HandleUIMainTitleOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("ObjectiveMainTitle :: HandleUIMainTitleOnPlayOutComplete", this, JDebug.JDebugType.UI);
		(sender as UIMainTitle).OnPlayOutComplete -= this.HandleUIMainTitleOnPlayOutComplete;
		SceneManager.sceneLoaded += this.HandleGameSceneLoaded;
		SceneManager.LoadScene("Game");
	}

	private void HandleGameSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		JDebug.Log("ObjectiveMainTitle :: HandleGameSceneLoaded", this, JDebug.JDebugType.UI);
		SceneManager.sceneLoaded -= this.HandleGameSceneLoaded;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Dispose();
			GameManager.Instance.Player = null;
		}
		GameManager.Instance.Player = GameManager.Instance.AssetManager.CreateAsset<Player>("Characters/Player/Player_Audrey");
		GameManager.Instance.Player.transform.position = new Vector3(0f, 2400f, 0f);
		GameManager.Instance.Player.Initialize();
		GameManager.Instance.Player.gameObject.SetActive(value: false);
		GameManager.Instance.SectionManager.OnLoaded += this.HandleSectionOnLoaded;
		GameManager.Instance.ShowAsyncLoader();
		GameManager.Instance.SectionManager.LoadSectionAsync(SectionID.Section_S105_WelcomeHome);
	}

	private void HandleSectionOnLoaded(object sender, EventArgs e)
	{
		GameManager.Instance.SectionManager.OnLoaded -= this.HandleSectionOnLoaded;
		GameManager.Instance.CompleteAsyncLoader();
		GameManager.Instance.UnlockPause();
		base.Dispose();
	}
}
