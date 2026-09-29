using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class SaveData
{
	[SerializeField]
	private int m_ID;

	[SerializeField]
	private TimeData m_TimeData = new TimeData();

	[SerializeField]
	private DifficultyData m_Difficulty;

	[SerializeField]
	private PlayerDataDirectory m_PlayerData = new PlayerDataDirectory();

	[SerializeField]
	private DataDirectories m_DataDirectories = new DataDirectories();

	[SerializeField]
	private int m_CursorState = -1;

	public int ID => this.m_ID;

	public TimeData TimeData => this.m_TimeData;

	public DifficultyData Difficulty => this.m_Difficulty;

	public PlayerDataDirectory PlayerData => this.m_PlayerData;

	public DataDirectories DataDirectories => this.m_DataDirectories;

	public int CursorState => this.m_CursorState;

	public Texture2D ScreenCaptureTexture { get; private set; }

	public void SetCursorState(int cursorState)
	{
		this.m_CursorState = cursorState;
	}

	public SaveData(int id, DifficultyLevel difficulty)
	{
		this.m_ID = id;
		this.m_Difficulty = new DifficultyData(difficulty);
	}

	public SaveData(int id, SaveData copy)
	{
		this.m_ID = id;
		this.m_TimeData = copy.TimeData;
		this.m_Difficulty = copy.Difficulty;
		this.m_DataDirectories = copy.DataDirectories;
		this.m_PlayerData = copy.PlayerData;
		this.m_CursorState = copy.CursorState;
		this.ScreenCaptureTexture = copy.ScreenCaptureTexture;
	}

	public void PrepareSave(Action callback = null, bool isAutosave = false)
	{
		JCoroutine jCoroutine;
		jCoroutine = new GameObject("JCoroutine - PrepareSave").AddComponent<JCoroutine>();
		global::UnityEngine.Object.DontDestroyOnLoad(jCoroutine);
		jCoroutine.OnComplete -= this.HandleGetCameraImageCoroutineOnComplete;
		jCoroutine.OnComplete += this.HandleGetCameraImageCoroutineOnComplete;
		jCoroutine.StartCoroutine(this.CameraImage(callback, isAutosave), null);
	}

	private IEnumerator CameraImage(Action callback, bool isAutosave)
	{
		yield return new WaitForEndOfFrame();
		if (GameManager.Instance.GameCamera != null)
		{
			if (!isAutosave)
			{
				RenderTexture active;
				active = RenderTexture.active;
				RenderTexture.active = GameManager.Instance.GameCamera.Camera.targetTexture;
				GameManager.Instance.GameCamera.Camera.Render();
				this.ScreenCaptureTexture = new Texture2D(Screen.width, Screen.height);
				this.ScreenCaptureTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
				this.ScreenCaptureTexture.Apply();
				RenderTexture.active = active;
				TextureScale.Bilinear(this.ScreenCaptureTexture, 540, 300);
			}
			else
			{
				this.ScreenCaptureTexture = GameManager.Instance.AssetManager.GetAsset<SpriteData>("Autosave/AutosaveSpriteData").Sprite.texture;
			}
		}
		callback?.Invoke();
	}

	private void HandleGetCameraImageCoroutineOnComplete(object sender, EventArgs e)
	{
		JCoroutine obj;
		obj = sender as JCoroutine;
		obj.OnComplete -= this.HandleGetCameraImageCoroutineOnComplete;
		obj.Dispose();
	}

	public void PostSave()
	{
		this.ScreenCaptureTexture = null;
	}

	public void Update()
	{
		this.m_TimeData.Update();
		if (GameManager.Instance.Player != null)
		{
			this.m_PlayerData.Transform.Update();
			this.m_PlayerData.Statistics.SetHealth(unchecked((int)GameManager.Instance.Player.Health));
			this.m_PlayerData.Statistics.SetCrouched(GameManager.Instance.Player.PlayerMovement.IsCrouched);
		}
		Section[] allSections;
		allSections = GameManager.Instance.SectionManager.GetAllSections();
		foreach (Section section in allSections)
		{
			if (section == null || section.SectionControllers == null)
			{
				continue;
			}
			SectionController[] sectionControllers;
			sectionControllers = section.SectionControllers;
			foreach (SectionController sectionController in sectionControllers)
			{
				if (!(sectionController == null))
				{
					sectionController.SaveData();
				}
			}
		}
		this.m_DataDirectories.SectionDirectory.Update();
	}

	public object Load(object key, object value)
	{
		if (value == null || value == null)
		{
			JDebug.Log("Cannot save [key: " + key?.ToString() + "] | [value: " + value?.ToString() + "] | because one is null", JDebug.JDebugType.SaveData);
			return null;
		}
		string text;
		text = key.GetType().Name + value.GetType().Name;
		if (this.m_DataDirectories.GetData(text, out var data))
		{
			return data.GetValue(key);
		}
		JDebug.Log("Cannot save [key] | [value] | because [directory: " + text + "] does not exist", JDebug.JDebugType.SaveData);
		return null;
	}

	public bool Save(object key, object value)
	{
		bool result;
		result = false;
		if (value == null || value == null)
		{
			JDebug.Log("Cannot save [key: " + key?.ToString() + "] | [value: " + value?.ToString() + "] | because one is null", JDebug.JDebugType.SaveData);
			return false;
		}
		string text;
		text = key.GetType().Name + value.GetType().Name;
		if (this.m_DataDirectories.GetData(text, out var data))
		{
			if (data.ChangeValue(key, value))
			{
				result = true;
				JDebug.Log("Succesfully saved: [key: " + key?.ToString() + "] | [value: " + value?.ToString() + "]", JDebug.JDebugType.SaveData);
			}
			else
			{
				JDebug.Log("Failed to save: [key: " + key?.ToString() + "] | [value: " + value?.ToString() + "]", JDebug.JDebugType.SaveData);
			}
		}
		else
		{
			JDebug.Log("Cannot save [key] | [value] | because [directory: " + text + "] does not exist", JDebug.JDebugType.SaveData);
		}
		return result;
	}

	public bool AddData<Key, Value>(SectionID sectionID, DataObject<Key, Value> value) where Key : IConvertible where Value : IDataObject<Key>, new()
	{
		bool result;
		result = false;
		if (((SectionDataObject)this.m_DataDirectories.SectionDirectory.GetValue(sectionID)).GetGroup(value.GetType().ToString(), out var group))
		{
			result = group.Add(value.ID, value);
		}
		return result;
	}

	public DataObject<Key, Value> GetData<Key, Value>(SectionID sectionID, Key key) where Key : IConvertible where Value : IDataObject<Key>, new()
	{
		object obj;
		obj = null;
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)this.m_DataDirectories.SectionDirectory.GetValue(sectionID);
		if (sectionDataObject != null && sectionDataObject.GetGroup(typeof(Value).Name, out var group))
		{
			object value;
			value = group.GetValue(key);
			if (value != null)
			{
				obj = value;
			}
		}
		if (obj != null)
		{
			return (DataObject<Key, Value>)obj;
		}
		return null;
	}
}
