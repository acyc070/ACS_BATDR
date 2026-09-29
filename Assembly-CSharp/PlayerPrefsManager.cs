using UnityEngine;

public static class PlayerPrefsManager
{
	public static void Save(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
		PlayerPrefs.Save();
	}

	public static void Save(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
		PlayerPrefs.Save();
	}

	public static void Save(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
		PlayerPrefs.Save();
	}

	public static float GetFloat(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetFloat(key);
		}
		PlayerPrefs.SetFloat(key, 0f);
		PlayerPrefs.Save();
		return PlayerPrefs.GetFloat(key);
	}

	public static int GetInt(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetInt(key);
		}
		PlayerPrefs.SetInt(key, 0);
		PlayerPrefs.Save();
		return PlayerPrefs.GetInt(key);
	}

	public static bool GetBool(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			if (PlayerPrefs.GetInt(key) <= 0)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public static string GetString(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetString(key);
		}
		PlayerPrefs.SetString(key, "");
		PlayerPrefs.Save();
		return PlayerPrefs.GetString(key);
	}
}
