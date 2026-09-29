namespace I2.Loc;

public static class PersistentStorage
{
	public enum eFileType
	{
		Raw = 0,
		Persistent = 1,
		Temporal = 2,
		Streaming = 3
	}

	private static I2CustomPersistentStorage mStorage;

	public static void SetSetting_String(string key, string value)
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		PersistentStorage.mStorage.SetSetting_String(key, value);
	}

	public static string GetSetting_String(string key, string defaultValue)
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		return PersistentStorage.mStorage.GetSetting_String(key, defaultValue);
	}

	public static void DeleteSetting(string key)
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		PersistentStorage.mStorage.DeleteSetting(key);
	}

	public static bool HasSetting(string key)
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		return PersistentStorage.mStorage.HasSetting(key);
	}

	public static void ForceSaveSettings()
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		PersistentStorage.mStorage.ForceSaveSettings();
	}

	public static bool CanAccessFiles()
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		return PersistentStorage.mStorage.CanAccessFiles();
	}

	public static bool SaveFile(eFileType fileType, string fileName, string data, bool logExceptions = true)
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		return PersistentStorage.mStorage.SaveFile(fileType, fileName, data, logExceptions);
	}

	public static string LoadFile(eFileType fileType, string fileName, bool logExceptions = true)
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		return PersistentStorage.mStorage.LoadFile(fileType, fileName, logExceptions);
	}

	public static bool DeleteFile(eFileType fileType, string fileName, bool logExceptions = true)
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		return PersistentStorage.mStorage.DeleteFile(fileType, fileName, logExceptions);
	}

	public static bool HasFile(eFileType fileType, string fileName, bool logExceptions = true)
	{
		if (PersistentStorage.mStorage == null)
		{
			PersistentStorage.mStorage = new I2CustomPersistentStorage();
		}
		return PersistentStorage.mStorage.HasFile(fileType, fileName, logExceptions);
	}
}
