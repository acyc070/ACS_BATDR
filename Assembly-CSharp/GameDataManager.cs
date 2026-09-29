using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameDataManager : JDisposable
{
	public delegate void OnSaveComplete(bool success);

	public delegate void OnLoadComplete(SaveData data);

	public delegate void OnDeleteComplete(bool success);

	public async void Save(OnSaveComplete onSaveComplete = null)
	{
		GameManager.Instance.ShowAsyncLoader();
		GameManager.Instance.bAsyncFileIOInProgress = true;
		bool success;
		success = await this.SaveInternal();
		onSaveComplete?.Invoke(success);
		GameManager.Instance.bAsyncFileIOInProgress = false;
		GameManager.Instance.CompleteAsyncLoader();
	}

	protected virtual async Task<bool> SaveInternal()
	{
		string saveFolder;
		saveFolder = this.GetSaveFolder();
		if (!this.CheckDirectory(saveFolder))
		{
			JDebug.Log("Unable to create the save game folder at: \"" + saveFolder + "\"", JDebug.JDebugType.SaveData);
			return false;
		}
		GameData gameData;
		gameData = GameManager.Instance.GameData;
		if (!(await this.AsyncWriteFile(filePath: Path.Combine(saveFolder, "data.game"), data: Encoding.ASCII.GetBytes(JsonUtility.ToJson(gameData)))))
		{
			return false;
		}
		if (!(await this.AsyncWriteFile(filePath: Path.Combine(saveFolder, gameData.CurrentSaveFileName + ".save"), data: Encoding.ASCII.GetBytes(JsonUtility.ToJson(gameData.CurrentSave)))))
		{
			return false;
		}
		string text;
		text = Path.Combine(this.GetImagesFolder(), gameData.CurrentSaveFileName + ".png");
		if (!this.CheckDirectory(text))
		{
			JDebug.Log("Unable to create the image folder at: \"" + text + "\"", JDebug.JDebugType.SaveData);
			return false;
		}
		return await this.AsyncWriteFile(GameManager.Instance.GameData.CurrentSave.ScreenCaptureTexture.EncodeToPNG(), text);
	}

	private async Task<bool> AsyncWriteFile(byte[] data, string filePath)
	{
		Task writeTask;
		writeTask = File.WriteAllBytesAsync(filePath, data);
		await writeTask;
		if (!writeTask.IsCompletedSuccessfully)
		{
			JDebug.LogWarning("Failed to save \"" + filePath.Replace("/", "\\") + "\"", JDebug.JDebugType.SaveData);
			if (writeTask.IsFaulted)
			{
				JDebug.LogWarning("Error: " + writeTask.Exception.Message, JDebug.JDebugType.SaveData);
			}
			return false;
		}
		JDebug.Log("Successfully saved \"" + filePath.Replace("/", "\\") + "\"", JDebug.JDebugType.SaveData);
		return true;
	}

	public async void LoadGameData()
	{
		GameManager.Instance.ShowAsyncLoader();
		GameManager.Instance.bAsyncFileIOInProgress = true;
		await this.LoadGameDataInternal();
		GameManager.Instance.bAsyncFileIOInProgress = false;
		GameManager.Instance.CompleteAsyncLoader();
	}

	protected virtual async Task LoadGameDataInternal()
	{
		if (GameManager.Instance.GameData != null)
		{
			JDebug.LogWarning("Attempting to load GameData after it has already been loaded once.", JDebug.JDebugType.SaveData);
		}
		GameData gameData;
		gameData = await this.AsyncReadFile<GameData>(Path.Combine(this.GetSaveFolder(), "data.game"));
		if (gameData != null)
		{
			gameData.SetCurrentSave(null);
			GameManager.Instance.GameData = gameData;
		}
	}

	public async void Load(string fileName, OnLoadComplete onLoadComplete)
	{
		GameManager.Instance.bAsyncFileIOInProgress = true;
		onLoadComplete(await this.LoadInternal(fileName));
		GameManager.Instance.bAsyncFileIOInProgress = false;
	}

	protected virtual async Task<SaveData> LoadInternal(string fileName)
	{
		return await this.AsyncReadFile<SaveData>(Path.Combine(this.GetSaveFolder(), fileName + ".save"));
	}

	protected virtual async Task<T> AsyncReadFile<T>(string filePath)
	{
		if (!this.CheckDirectory(filePath, canCreate: false))
		{
			return default;
		}
		if (!this.CheckFile(filePath))
		{
			return default;
		}
		Task<string> readTask;
		readTask = File.ReadAllTextAsync(filePath, Encoding.ASCII);
		await readTask;
		if (!readTask.IsCompletedSuccessfully)
		{
			JDebug.LogWarning("Failed to load \"" + filePath.Replace("/", "\\") + "\"", JDebug.JDebugType.SaveData);
			if (readTask.IsFaulted)
			{
				JDebug.LogWarning("Error: " + readTask.Exception.Message, JDebug.JDebugType.SaveData);
			}
			return default;
		}
		JDebug.Log("Successfully loaded \"" + filePath.Replace("/", "\\") + "\"", JDebug.JDebugType.SaveData);
		return JsonUtility.FromJson<T>(readTask.Result);
	}

	public async void Delete(string fileName, OnDeleteComplete onDeleteComplete = null)
	{
		GameManager.Instance.ShowAsyncLoader();
		GameManager.Instance.bAsyncFileIOInProgress = true;
		bool success;
		success = await this.DeleteInternal(fileName);
		onDeleteComplete?.Invoke(success);
		GameManager.Instance.bAsyncFileIOInProgress = false;
		GameManager.Instance.CompleteAsyncLoader();
	}

	protected virtual async Task<bool> DeleteInternal(string fileName)
	{
		string filePath;
		filePath = Path.Combine(this.GetSaveFolder(), fileName);
		if (!this.CheckDirectory(filePath))
		{
			return false;
		}
		if (!this.CheckFile(filePath))
		{
			return false;
		}
		Task deleteTask;
		deleteTask = Task.Run(() =>
		{
			File.Delete(filePath);
		});
		await deleteTask;
		if (!deleteTask.IsCompletedSuccessfully)
		{
			JDebug.LogWarning("Failed to delete \"" + filePath.Replace("/", "\\") + "\"", JDebug.JDebugType.SaveData);
			if (deleteTask.IsFaulted)
			{
				JDebug.LogWarning("Error: " + deleteTask.Exception.Message, JDebug.JDebugType.SaveData);
			}
			return false;
		}
		JDebug.Log("Successfully deleted \"" + filePath.Replace("/", "\\") + "\"", JDebug.JDebugType.SaveData);
		return true;
	}

	private string GetSaveFolder()
	{
		string text;
		text = Path.Combine(Application.persistentDataPath, "data");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	private string GetImagesFolder()
	{
		string text;
		text = Path.Combine(Application.persistentDataPath, "images");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	private bool CheckDirectory(string path, bool canCreate = true)
	{
		bool result;
		result = true;
		string directoryName;
		directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			if (canCreate)
			{
				Directory.CreateDirectory(directoryName);
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	private bool CheckFile(string path)
	{
		if (!File.Exists(path))
		{
			JDebug.Log("File does not exist", JDebug.JDebugType.SaveData);
			return false;
		}
		return true;
	}
}
