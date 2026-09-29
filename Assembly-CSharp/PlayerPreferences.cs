using System.Collections.Generic;
using I2.Loc;
using S13Audio.BATDR;
using UnityEngine;

public class PlayerPreferences : JDisposable
{
	private string[] m_Languages;

	private string[] m_ResolutionList;

	private List<Resolution> m_Resolutions;

	private string[] m_Qualites;

	private string[] m_VSyncs;

	public float Volume
	{
		get
		{
			return PlayerPrefsManager.GetFloat("VOLUME");
		}
		set
		{
			float num;
			num = value;
			if (num <= 0f)
			{
				num = 0.0001f;
			}
			PlayerPrefsManager.Save("VOLUME", num);
			S13MixerReference.MasterMixer.SetFloat("Master", Mathf.Log10(num) * 20f);
		}
	}

	public float MusicVolume
	{
		get
		{
			return PlayerPrefsManager.GetFloat("MUSIC_VOLUME");
		}
		set
		{
			float num;
			num = value;
			if (num <= 0f)
			{
				num = 0.0001f;
			}
			PlayerPrefsManager.Save("MUSIC_VOLUME", num);
			S13MixerReference.MasterMixer.SetFloat("Music_Slider", Mathf.Log10(num) * 20f);
		}
	}

	public float SFXVolume
	{
		get
		{
			return PlayerPrefsManager.GetFloat("SFX_VOLUME");
		}
		set
		{
			float num;
			num = value;
			if (num <= 0f)
			{
				num = 0.0001f;
			}
			PlayerPrefsManager.Save("SFX_VOLUME", num);
			S13MixerReference.MasterMixer.SetFloat("Effects_Slider", Mathf.Log10(num) * 20f);
		}
	}

	public float DialogueVolume
	{
		get
		{
			return PlayerPrefsManager.GetFloat("DIALOGUE_VOLUME");
		}
		set
		{
			float num;
			num = value;
			if (num <= 0f)
			{
				num = 0.0001f;
			}
			PlayerPrefsManager.Save("DIALOGUE_VOLUME", num);
			S13MixerReference.MasterMixer.SetFloat("Voice_Slider", Mathf.Log10(num) * 20f);
		}
	}

	public bool Subtitles
	{
		get
		{
			return PlayerPrefsManager.GetBool("SUBTITLES");
		}
		set
		{
			PlayerPrefsManager.Save("SUBTITLES", value ? 1 : 0);
			if (GameManager.Instance.Player != null && !value)
			{
				GameManager.Instance.ClearSubtitles();
			}
		}
	}

	public int Language
	{
		get
		{
			return PlayerPrefsManager.GetInt("LANGUAGE");
		}
		set
		{
			PlayerPrefsManager.Save("LANGUAGE", value);
			LocalizationManager.CurrentLanguageCode = this.Languages[value];
		}
	}

	public string[] Languages
	{
		get
		{
			if (this.m_Languages == null)
			{
				this.m_Languages = new string[10] { "MENU_OPTIONS_AUDIO_LANGUAGE_ENGLISH", "MENU_OPTIONS_AUDIO_LANGUAGE_FRENCH", "MENU_OPTIONS_AUDIO_LANGUAGE_ITALIAN", "MENU_OPTIONS_AUDIO_LANGUAGE_GERMAN", "MENU_OPTIONS_AUDIO_LANGUAGE_SPANISH", "MENU_OPTIONS_AUDIO_LANGUAGE_JAPANESE", "MENU_OPTIONS_AUDIO_LANGUAGE_RUSSIAN", "MENU_OPTIONS_AUDIO_LANGUAGE_UKRANIAN", "MENU_OPTIONS_AUDIO_LANGUAGE_BRAZILIAN_PORTUGUESE", "MENU_OPTIONS_AUDIO_LANGUAGE_SIMPLIFIED_CHINESE" };
			}
			return this.m_Languages;
		}
	}

	public int CurrentResolution
	{
		get
		{
			return PlayerPrefsManager.GetInt("RESOLUTION_CURRENT");
		}
		set
		{
			PlayerPrefsManager.Save("RESOLUTION_CURRENT", value);
			Resolution resolution;
			resolution = this.Resolutions[value];
			this.ResolutionWidth = resolution.width;
			this.ResolutionHeight = resolution.height;
			Screen.SetResolution(this.ResolutionWidth, this.ResolutionHeight, this.Fullscreen);
		}
	}

	public int ResolutionWidth
	{
		get
		{
			return PlayerPrefsManager.GetInt("RESOLUTION_WIDTH");
		}
		set
		{
			PlayerPrefsManager.Save("RESOLUTION_WIDTH", value);
		}
	}

	public int ResolutionHeight
	{
		get
		{
			return PlayerPrefsManager.GetInt("RESOLUTION_HEIGHT");
		}
		set
		{
			PlayerPrefsManager.Save("RESOLUTION_HEIGHT", value);
		}
	}

	public string[] ResolutionsList
	{
		get
		{
			if (this.m_ResolutionList == null || this.m_ResolutionList.Length == 0)
			{
				List<string> list;
				list = new List<string>();
				foreach (Resolution resolution in this.Resolutions)
				{
					string item;
					item = resolution.width + "x" + resolution.height;
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
				this.m_ResolutionList = list.ToArray();
			}
			return this.m_ResolutionList;
		}
	}

	public List<Resolution> Resolutions
	{
		get
		{
			unchecked
			{
				if (this.m_Resolutions == null || this.m_Resolutions.Count <= 0)
				{
					this.m_Resolutions = new List<Resolution>();
					Resolution[] resolutions;
					resolutions = Screen.resolutions;
					List<string> list;
					list = new List<string>();
					float num;
					num = 1f;
					float num2;
					num2 = 1f;
					Resolution[] array;
					array = resolutions;
					for (int i = 0; i < array.Length; i++)
					{
						Resolution item;
						item = array[i];
						float num3;
						num3 = item.width / item.height;
						if (num3 != num && num3 != num2)
						{
							continue;
						}
						float num4;
						num4 = item.width;
						float num5;
						num5 = item.height;
						string item2;
						item2 = num4 + "x" + num5;
						if (!list.Contains(item2))
						{
							list.Add(item2);
							if (!this.m_Resolutions.Contains(item))
							{
								this.m_Resolutions.Add(item);
							}
						}
					}
				}
				return this.m_Resolutions;
			}
		}
	}

	public int Quality
	{
		get
		{
			return PlayerPrefsManager.GetInt("QUALITY");
		}
		set
		{
			PlayerPrefsManager.Save("QUALITY", value);
			QualitySettings.SetQualityLevel(value, applyExpensiveChanges: true);
			QualitySettings.vSyncCount = this.GetVSync(this.VSync);
		}
	}

	public string[] Qualites
	{
		get
		{
			if (this.m_Qualites == null)
			{
				this.m_Qualites = new string[3] { "MENU_OPTIONS_GRAPHICS_QUALITY_LOW", "MENU_OPTIONS_GRAPHICS_QUALITY_MEDIUM", "MENU_OPTIONS_GRAPHICS_QUALITY_HIGH" };
			}
			return this.m_Qualites;
		}
	}

	public float Brightness
	{
		get
		{
			return PlayerPrefsManager.GetFloat("BRIGHTNESS");
		}
		set
		{
			PlayerPrefsManager.Save("BRIGHTNESS", value);
			if (GameManager.Instance.GameCamera != null)
			{
				GameManager.Instance.GameCamera.Brightness = value;
			}
		}
	}

	public bool Fullscreen
	{
		get
		{
			return PlayerPrefsManager.GetBool("FULLSCREEN");
		}
		set
		{
			PlayerPrefsManager.Save("FULLSCREEN", value ? 1 : 0);
			Screen.fullScreen = value;
		}
	}

	public bool DoF
	{
		get
		{
			return PlayerPrefsManager.GetBool("DOF");
		}
		set
		{
			PlayerPrefsManager.Save("DOF", value ? 1 : 0);
			if (GameManager.Instance.GameCamera != null)
			{
				GameManager.Instance.GameCamera.DoF = value;
			}
		}
	}

	public bool Bloom
	{
		get
		{
			return PlayerPrefsManager.GetBool("BLOOM");
		}
		set
		{
			PlayerPrefsManager.Save("BLOOM", value ? 1 : 0);
			if (GameManager.Instance.GameCamera != null)
			{
				GameManager.Instance.GameCamera.Bloom = value;
			}
		}
	}

	public bool AmbientOcclusion
	{
		get
		{
			return PlayerPrefsManager.GetBool("AMBIENT_OCCLUSION");
		}
		set
		{
			PlayerPrefsManager.Save("AMBIENT_OCCLUSION", value ? 1 : 0);
			if (GameManager.Instance.GameCamera != null)
			{
				GameManager.Instance.GameCamera.AmbientOcclusion = value;
			}
		}
	}

	public bool MotionBlur
	{
		get
		{
			return PlayerPrefsManager.GetBool("MOTION_BLUR");
		}
		set
		{
			PlayerPrefsManager.Save("MOTION_BLUR", value ? 1 : 0);
			if (GameManager.Instance.GameCamera != null)
			{
				GameManager.Instance.GameCamera.MotionBlur = value;
			}
		}
	}

	public bool AA
	{
		get
		{
			return PlayerPrefsManager.GetBool("ANTI_ALIASING");
		}
		set
		{
			PlayerPrefsManager.Save("ANTI_ALIASING", value ? 1 : 0);
			if (GameManager.Instance.GameCamera != null)
			{
				GameManager.Instance.GameCamera.AA = value;
			}
		}
	}

	public int VSync
	{
		get
		{
			return PlayerPrefsManager.GetInt("V_SYNC");
		}
		set
		{
			PlayerPrefsManager.Save("V_SYNC", value);
			if ((QualitySettings.vSyncCount = this.GetVSync(value)) == 0)
			{
				Application.targetFrameRate = -1;
			}
			else
			{
				Application.targetFrameRate = ProjectSettings.Instance.TargetFrameRate;
			}
		}
	}

	public string[] VSyncs
	{
		get
		{
			if (this.m_VSyncs == null)
			{
				this.m_VSyncs = new string[3] { "MENU_OPTIONS_GRAPHICS_VSYNC_OFF", "MENU_OPTIONS_GRAPHICS_VSYNC_LOW", "MENU_OPTIONS_GRAPHICS_VSYNC_HIGH" };
			}
			return this.m_VSyncs;
		}
	}

	public float Sensitivity
	{
		get
		{
			return PlayerPrefsManager.GetFloat("SENSITIVITY");
		}
		set
		{
			PlayerPrefsManager.Save("SENSITIVITY", value);
			PlayerInput.Sensitivity = value;
		}
	}

	public bool Inverted
	{
		get
		{
			return PlayerPrefsManager.GetBool("INVERTED");
		}
		set
		{
			PlayerPrefsManager.Save("INVERTED", value ? 1 : 0);
			PlayerInput.IsInverted = value;
		}
	}

	public bool Crosshair
	{
		get
		{
			return PlayerPrefsManager.GetBool("CROSSHAIR");
		}
		set
		{
			PlayerPrefsManager.Save("CROSSHAIR", value ? 1 : 0);
			if (GameManager.Instance.Player != null)
			{
				GameManager.Instance.SetCrosshair(value);
			}
		}
	}

	public bool ViewSwaying
	{
		get
		{
			return PlayerPrefsManager.GetBool("VIEW_SWAYING");
		}
		set
		{
			PlayerPrefsManager.Save("VIEW_SWAYING", value ? 1 : 0);
			if (GameManager.Instance.Player != null)
			{
				CameraMovements cameraMovement;
				cameraMovement = GameManager.Instance.Player.CameraMovement;
				if (cameraMovement != null)
				{
					cameraMovement.SetActive(value);
				}
			}
		}
	}

	public bool SmoothCamera
	{
		get
		{
			return PlayerPrefsManager.GetBool("SMOOTH_CAMERA");
		}
		set
		{
			PlayerPrefsManager.Save("SMOOTH_CAMERA", value ? 1 : 0);
		}
	}

	public float Rumble
	{
		get
		{
			return PlayerPrefsManager.GetFloat("RUMBLE");
		}
		set
		{
			PlayerPrefsManager.Save("RUMBLE", value);
		}
	}

	public bool Seasonal
	{
		get
		{
			return PlayerPrefsManager.GetBool("SEASONAL");
		}
		set
		{
			bool flag;
			flag = value;
			if (!SeasonalCheck.IsSeasonal())
			{
				flag = false;
			}
			PlayerPrefsManager.Save("SEASONAL", flag ? 1 : 0);
		}
	}

	private int GetVSync(int vsync)
	{
		return vsync switch
		{
			0 => 0, 
			1 => 2, 
			_ => 1, 
		};
	}

	public static PlayerPreferences Create()
	{
		PlayerPreferences playerPreferences;
		playerPreferences = new PlayerPreferences();
		if (!PlayerPrefs.HasKey("v1.0.2.0250"))
		{
			JDebug.Log("PlayerPreferences :: Resetting Player Prefs", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefs.DeleteAll();
			PlayerPrefsManager.Save("v1.0.2.0250", 1);
		}
		if (!PlayerPrefs.HasKey("VOLUME"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Volume", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("VOLUME", 1f);
		}
		else
		{
			playerPreferences.SetFloat("VOLUME", playerPreferences.GetFloat("VOLUME"));
		}
		if (!PlayerPrefs.HasKey("MUSIC_VOLUME"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Music Volume", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("MUSIC_VOLUME", 0.9f);
		}
		else
		{
			playerPreferences.SetFloat("MUSIC_VOLUME", playerPreferences.GetFloat("MUSIC_VOLUME"));
		}
		if (!PlayerPrefs.HasKey("SFX_VOLUME"))
		{
			JDebug.Log("PlayerPreferences :: Initializing SFX Volume", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("SFX_VOLUME", 0.9f);
		}
		else
		{
			playerPreferences.SetFloat("SFX_VOLUME", playerPreferences.GetFloat("SFX_VOLUME"));
		}
		if (!PlayerPrefs.HasKey("DIALOGUE_VOLUME"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Dialogue Volume", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("DIALOGUE_VOLUME", 0.9f);
		}
		else
		{
			playerPreferences.SetFloat("DIALOGUE_VOLUME", playerPreferences.GetFloat("DIALOGUE_VOLUME"));
		}
		if (!PlayerPrefs.HasKey("SUBTITLES"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Subtitles", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("SUBTITLES", 1);
		}
		if (!PlayerPrefs.HasKey("LANGUAGE"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Language", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("LANGUAGE", 0);
			LocalizationManager.CurrentLanguageCode = playerPreferences.Languages[0];
		}
		if (!PlayerPrefs.HasKey("QUALITY"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Quality", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("QUALITY", 2);
		}
		if (!PlayerPrefs.HasKey("BRIGHTNESS"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Brightness", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("BRIGHTNESS", 0.5f);
		}
		if (!PlayerPrefs.HasKey("DOF"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Depth of Field", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("DOF", 1);
		}
		if (!PlayerPrefs.HasKey("BLOOM"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Bloom Lighting", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("BLOOM", 1);
		}
		if (!PlayerPrefs.HasKey("AMBIENT_OCCLUSION"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Ambient Occlusion", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("AMBIENT_OCCLUSION", 1);
		}
		if (!PlayerPrefs.HasKey("MOTION_BLUR"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Motion Blur", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("MOTION_BLUR", 1);
		}
		if (!PlayerPrefs.HasKey("ANTI_ALIASING"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Anti Aliasing", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("ANTI_ALIASING", 1);
		}
		if (!PlayerPrefs.HasKey("V_SYNC"))
		{
			JDebug.Log("PlayerPreferences :: Initializing V-Sync", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("V_SYNC", 2);
		}
		if (!PlayerPrefs.HasKey("SENSITIVITY"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Look Sensitivity", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("SENSITIVITY", 0.4f);
		}
		else
		{
			playerPreferences.SetFloat("SENSITIVITY", playerPreferences.GetFloat("SENSITIVITY"));
		}
		if (!PlayerPrefs.HasKey("INVERTED"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Inverted", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("INVERTED", 0);
		}
		else
		{
			playerPreferences.SetBool("INVERTED", playerPreferences.GetBool("INVERTED"));
		}
		if (!PlayerPrefs.HasKey("CROSSHAIR"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Crosshair", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("CROSSHAIR", 1);
		}
		if (!PlayerPrefs.HasKey("VIEW_SWAYING"))
		{
			JDebug.Log("PlayerPreferences :: Initializing View Swaying", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("VIEW_SWAYING", 1);
		}
		if (!PlayerPrefs.HasKey("SMOOTH_CAMERA"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Smooth Camera", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("SMOOTH_CAMERA", 1);
		}
		if (!PlayerPrefs.HasKey("RUMBLE"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Rumble", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("RUMBLE", 0.5f);
		}
		if (!PlayerPrefs.HasKey("SEASONAL"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Seasonal", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("SEASONAL", 0);
		}
		else if (!SeasonalCheck.IsSeasonal())
		{
			playerPreferences.SetBool("SEASONAL", value: false);
		}
		playerPreferences.InitializeQuality();
		playerPreferences.InitializeResolution();
		LocalizationManager.CurrentLanguageCode = playerPreferences.Languages[playerPreferences.Language];
		return playerPreferences;
	}

	private void InitializeQuality()
	{
		QualitySettings.SetQualityLevel(this.Quality, applyExpensiveChanges: true);
		QualitySettings.vSyncCount = this.GetVSync(this.VSync);
	}

	private void InitializeResolution()
	{
		if (!PlayerPrefs.HasKey("FULLSCREEN"))
		{
			JDebug.Log("PlayerPreferences :: Initializing Fullscreen", JDebug.JDebugType.PlayerPrefs);
			PlayerPrefsManager.Save("FULLSCREEN", 1);
		}
		unchecked
		{
			if (!PlayerPrefs.HasKey("RESOLUTION_WIDTH"))
			{
				JDebug.Log("PlayerPreferences :: Initializing Resolution Width", JDebug.JDebugType.PlayerPrefs);
				PlayerPrefsManager.Save("RESOLUTION_WIDTH", this.Resolutions[this.Resolutions.Count - 1].width);
			}
			if (!PlayerPrefs.HasKey("RESOLUTION_HEIGHT"))
			{
				JDebug.Log("PlayerPreferences :: Initializing Resolution Height", JDebug.JDebugType.PlayerPrefs);
				PlayerPrefsManager.Save("RESOLUTION_HEIGHT", this.Resolutions[this.Resolutions.Count - 1].height);
			}
			if (!PlayerPrefs.HasKey("RESOLUTION_CURRENT"))
			{
				JDebug.Log("PlayerPreferences :: Initializing Resolution Current", JDebug.JDebugType.PlayerPrefs);
				PlayerPrefsManager.Save("RESOLUTION_CURRENT", this.Resolutions.Count - 1);
			}
			this.Fullscreen = PlayerPrefsManager.GetBool("FULLSCREEN");
			int width;
			width = PlayerPrefsManager.GetInt("RESOLUTION_WIDTH");
			int height;
			height = PlayerPrefsManager.GetInt("RESOLUTION_HEIGHT");
			Screen.SetResolution(width, height, this.Fullscreen);
			JDebug.Log("PlayerPreferences :: Fullscreen: " + this.Fullscreen + "\nResolution :" + width + " x " + height + "\nResolution Index: " + this.CurrentResolution, JDebug.JDebugType.PlayerPrefs);
		}
	}

	public float GetFloat(string key)
	{
		float result;
		result = 0f;
		switch (key)
		{
		case "VOLUME":
			result = this.Volume;
			break;
		case "SFX_VOLUME":
			result = this.SFXVolume;
			break;
		case "MUSIC_VOLUME":
			result = this.MusicVolume;
			break;
		case "DIALOGUE_VOLUME":
			result = this.DialogueVolume;
			break;
		case "BRIGHTNESS":
			result = this.Brightness;
			break;
		case "SENSITIVITY":
			result = this.Sensitivity;
			break;
		case "RUMBLE":
			result = this.Rumble;
			break;
		}
		return result;
	}

	public int GetInt(string key)
	{
		int result;
		result = 0;
		switch (key)
		{
		case "LANGUAGE":
			result = this.Language;
			break;
		case "QUALITY":
			result = this.Quality;
			break;
		case "RESOLUTION_CURRENT":
			result = this.CurrentResolution;
			break;
		case "V_SYNC":
			result = this.VSync;
			break;
		}
		return result;
	}

	public bool GetBool(string key)
	{
		bool result;
		result = false;
		switch (key)
		{
		case "SUBTITLES":
			result = this.Subtitles;
			break;
		case "FULLSCREEN":
			result = this.Fullscreen;
			break;
		case "DOF":
			result = this.DoF;
			break;
		case "BLOOM":
			result = this.Bloom;
			break;
		case "AMBIENT_OCCLUSION":
			result = this.AmbientOcclusion;
			break;
		case "MOTION_BLUR":
			result = this.MotionBlur;
			break;
		case "VIEW_SWAYING":
			result = this.ViewSwaying;
			break;
		case "SMOOTH_CAMERA":
			result = this.SmoothCamera;
			break;
		case "ANTI_ALIASING":
			result = this.AA;
			break;
		case "INVERTED":
			result = this.Inverted;
			break;
		case "CROSSHAIR":
			result = this.Crosshair;
			break;
		case "SEASONAL":
			result = this.Seasonal;
			break;
		}
		return result;
	}

	public void SetFloat(string key, float value)
	{
		switch (key)
		{
		case "VOLUME":
			this.Volume = value;
			break;
		case "SFX_VOLUME":
			this.SFXVolume = value;
			break;
		case "MUSIC_VOLUME":
			this.MusicVolume = value;
			break;
		case "DIALOGUE_VOLUME":
			this.DialogueVolume = value;
			break;
		case "BRIGHTNESS":
			this.Brightness = value;
			break;
		case "SENSITIVITY":
			this.Sensitivity = value;
			break;
		case "RUMBLE":
			this.Rumble = value;
			break;
		}
	}

	public void SetInt(string key, int value)
	{
		switch (key)
		{
		case "LANGUAGE":
			this.Language = value;
			break;
		case "QUALITY":
			this.Quality = value;
			break;
		case "RESOLUTION_CURRENT":
			this.CurrentResolution = value;
			break;
		case "V_SYNC":
			this.VSync = value;
			break;
		}
	}

	public void SetBool(string key, bool value)
	{
		switch (key)
		{
		case "SUBTITLES":
			this.Subtitles = value;
			break;
		case "FULLSCREEN":
			this.Fullscreen = value;
			break;
		case "DOF":
			this.DoF = value;
			break;
		case "BLOOM":
			this.Bloom = value;
			break;
		case "AMBIENT_OCCLUSION":
			this.AmbientOcclusion = value;
			break;
		case "MOTION_BLUR":
			this.MotionBlur = value;
			break;
		case "VIEW_SWAYING":
			this.ViewSwaying = value;
			break;
		case "SMOOTH_CAMERA":
			this.SmoothCamera = value;
			break;
		case "ANTI_ALIASING":
			this.AA = value;
			break;
		case "INVERTED":
			this.Inverted = value;
			break;
		case "CROSSHAIR":
			this.Crosshair = value;
			break;
		case "SEASONAL":
			this.Seasonal = value;
			break;
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_Resolutions != null)
		{
			this.m_Resolutions.Clear();
			this.m_Resolutions = null;
		}
		base.OnDisposed();
	}
}
