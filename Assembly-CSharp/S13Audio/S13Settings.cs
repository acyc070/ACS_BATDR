using UnityEngine;

namespace S13Audio;

public static class S13Settings
{
	private const string EDITOR_PREFIX_KEY = "Studio13/S13Settings";

	private static string json_savedSettings;

	private static string json_savedProjectSettings;

	private static StampSettingsData curSettings;

	private static StampProjectSettingsData curProjectSettings;

	public static bool Debug_AreaSoundUI;

	private static string stampProjectSettingsKey => "Studio13/S13Settings" + Application.productName;

	public static StampSettingsData SavedSettings
	{
		get
		{
			if (S13Settings.curSettings == null && S13Settings.curSettings == null)
			{
				S13Settings.curSettings = new StampSettingsData();
				S13Settings.json_savedSettings = JsonUtility.ToJson(S13Settings.curSettings);
			}
			return S13Settings.curSettings;
		}
	}

	public static StampProjectSettingsData SavedProjectSettings
	{
		get
		{
			if (S13Settings.curProjectSettings == null && S13Settings.curProjectSettings == null)
			{
				S13Settings.curProjectSettings = new StampProjectSettingsData();
				S13Settings.json_savedProjectSettings = JsonUtility.ToJson(S13Settings.curProjectSettings);
			}
			return S13Settings.curProjectSettings;
		}
	}

	public static S13Debug.LogType EnabledLogs => S13Settings.SavedSettings._enabledLogs;

	public static bool Debug_EnableLogTypePrefix => S13Settings.SavedSettings._logTypePrefix;

	public static bool Debug_EnableFramePrefix => S13Settings.SavedSettings._frameCount;

	public static int Debug_TextSize => S13Settings.SavedSettings._textSize;

	public static bool Color_AreaSoundColliders => S13Settings.SavedSettings._areaSoundColors;

	public static Color Color_UnselectedAreaCollider => S13Settings.SavedSettings._unselectedAreaSoundColor;

	public static Color Color_SimpleAreaSoundCollider => S13Settings.SavedSettings._simpleAreaSoundColor;

	public static Color Color_PortalAreaSoundCollider => S13Settings.SavedSettings._portalAreaSoundColor;

	public static bool AudioDevMode => S13Settings.SavedProjectSettings._audioDevMode;

	public static bool AudreyPointLight => S13Settings.curProjectSettings._audreyPointLight;

	public static void UpdateSettings(StampSettingsData newSettings, StampProjectSettingsData newProjectSettings)
	{
		string value;
		value = JsonUtility.ToJson(newSettings);
		if (!S13Settings.json_savedSettings.Equals(value))
		{
			Debug.Log("S13Settings changes saved");
			S13Settings.curSettings = newSettings;
			S13Settings.json_savedSettings = value;
		}
		string value2;
		value2 = JsonUtility.ToJson(newProjectSettings);
		if (!S13Settings.json_savedProjectSettings.Equals(value2))
		{
			Debug.Log("S13ProjectSettings changes saved");
			S13Settings.curProjectSettings = newProjectSettings;
			S13Settings.json_savedProjectSettings = value2;
		}
	}

	[RuntimeInitializeOnLoadMethod]
	private static void OnLoadProject()
	{
		S13Settings.curSettings = S13Settings.SavedSettings;
		S13Settings.curProjectSettings = S13Settings.SavedProjectSettings;
	}

	public static void ResetSettings()
	{
		S13Settings.UpdateSettings(null, null);
	}

	static S13Settings()
	{
		S13Settings.Debug_AreaSoundUI = S13Settings.SavedSettings._areaSoundUI;
	}
}
