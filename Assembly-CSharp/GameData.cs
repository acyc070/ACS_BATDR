using System;
using System.IO;
using UnityEngine;

[Serializable]
public class GameData
{
	[SerializeField]
	private int m_CurrentSaveFileID = -1;

	[SerializeField]
	private string m_CurrentSaveFileName = "";

	[SerializeField]
	private string m_CurrentSaveFileTitle = "";

	[SerializeField]
	private SaveFileDirectory m_SaveFileDirectory = new SaveFileDirectory();

	[SerializeField]
	private bool m_Credits;

	public int CurrentSaveFileID => this.m_CurrentSaveFileID;

	public string CurrentSaveFileName => this.m_CurrentSaveFileName;

	public string CurrentSaveFileTitle => this.m_CurrentSaveFileTitle;

	public SaveFileDirectory SaveFileDirectory => this.m_SaveFileDirectory;

	public bool Credits => this.m_Credits;

	public SaveData CurrentSave { get; private set; }

	public SaveFileData CurrentSaveFile { get; private set; }

	public void SetCredits(bool has)
	{
		this.m_Credits = has;
	}

	private SaveData UpdateCurrentSave(SaveData saveData)
	{
		return this.CurrentSave = saveData;
	}

	public void SetCurrentSave(SaveData saveData)
	{
		this.CurrentSave = ((saveData == null) ? null : new SaveData(saveData.ID, saveData));
	}

	public void Update()
	{
		this.CurrentSave.Update();
		this.CurrentSaveFile.Update(GameManager.Instance.GameData.CurrentSave);
	}

	public void GetSave(int id)
	{
		for (int i = 0; i < this.m_SaveFileDirectory.Count; i = unchecked(i + 1))
		{
			SaveFileData saveFileData;
			saveFileData = (SaveFileData)this.m_SaveFileDirectory.GetValue(i);
			if (saveFileData.ID != id)
			{
				continue;
			}
			GameManager.Instance.GameDataManager.Load(saveFileData.Name, (SaveData saveData) =>
			{
				if (saveData != null)
				{
					this.m_CurrentSaveFileID = saveFileData.ID;
					this.m_CurrentSaveFileName = saveFileData.Name;
					this.UpdateCurrentSave(new SaveData(saveData.ID, saveData));
				}
			});
			break;
		}
	}

	public SaveFileData NewAutoSaveFile(string fileName, string fileTitle)
	{
		SaveFileData saveFileData;
		saveFileData = new SaveFileData(0, fileName, fileTitle);
		if (this.m_SaveFileDirectory.Count > 0)
		{
			this.m_SaveFileDirectory.ChangeValue(0, saveFileData);
		}
		else
		{
			this.m_SaveFileDirectory.Add(0, saveFileData);
		}
		this.CurrentSaveFile = saveFileData;
		this.m_CurrentSaveFileID = 0;
		this.m_CurrentSaveFileName = saveFileData.Name;
		return saveFileData;
	}

	public SaveFileData NewSaveFile(string fileName, string fileTitle)
	{
		int count;
		count = this.m_SaveFileDirectory.Count;
		SaveFileData saveFileData;
		saveFileData = new SaveFileData(count, fileName + "_" + count, fileTitle);
		this.m_SaveFileDirectory.Add(count, saveFileData);
		this.CurrentSaveFile = saveFileData;
		this.m_CurrentSaveFileID = saveFileData.ID;
		this.m_CurrentSaveFileName = saveFileData.Name;
		return saveFileData;
	}

	public bool GetSaveFile(string fileName, out SaveFileData saveFileData)
	{
		bool result;
		result = false;
		saveFileData = null;
		for (int i = 0; i < this.m_SaveFileDirectory.Count; i = unchecked(i + 1))
		{
			SaveFileData saveFileData2;
			saveFileData2 = (SaveFileData)this.m_SaveFileDirectory.GetValue(i);
			if (saveFileData2.Name == fileName)
			{
				saveFileData = saveFileData2;
				this.CurrentSaveFile = saveFileData;
				this.m_CurrentSaveFileID = saveFileData.ID;
				this.m_CurrentSaveFileName = saveFileData.Name;
				result = true;
				break;
			}
		}
		return result;
	}

	public SaveData NewSave(int id, DifficultyLevel difficulty)
	{
		return this.UpdateCurrentSave(new SaveData(id, difficulty));
	}

	public SaveData NewSave(int id, SaveData copy)
	{
		return this.UpdateCurrentSave(new SaveData(id, copy));
	}

	public void TriggerCredits()
	{
		this.m_Credits = true;
	}

	public void AutoSave(bool disableNotification = false, Action callback = null)
	{
		SaveFileData saveFileData;
		saveFileData = GameManager.Instance.GameData.NewAutoSaveFile("autosave", "Saved Data");
		GameManager.Instance.GameData.NewSave(saveFileData.ID, GameManager.Instance.GameData.CurrentSave);
		GameManager.Instance.ShowAsyncLoader();
		Path.Combine(Path.Combine(Application.persistentDataPath, "images"), "autosave.png");
		this.InternalAutoSave(saveFileData.Name, disableNotification, callback);
	}

	private void InternalAutoSave(string fileName, bool disableNotification, Action callback = null)
	{
		JDebug.Log("[SAVING] - Overwriting " + fileName, null, JDebug.JDebugType.SaveData);
		this.CurrentSave.PrepareSave(() =>
		{
			this.FinalizeSave(disableNotification, callback);
		}, isAutosave: true);
	}

	private void FinalizeSave(bool disableNotification, Action callback)
	{
		this.Update();
		GameManager.Instance.GameDataManager.Save((bool success) =>
		{
			this.CurrentSave.PostSave();
			GameManager.Instance.CompleteAsyncLoader();
			if (!disableNotification)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_AUTOSAVE"));
			}
			callback?.Invoke();
		});
	}
}
