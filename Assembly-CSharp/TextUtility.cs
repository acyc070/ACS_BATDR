using I2.Loc;

public static class TextUtility
{
	public static string GetKey(string key)
	{
		TermData termData;
		termData = LocalizationManager.GetSourceContaining(key).GetTermData(key);
		string result;
		result = "CANNOT FIND TERM: " + key;
		if (termData != null)
		{
			result = termData.Languages[GameManager.Instance.PlayerSettings.Language];
		}
		return result;
	}

	public static string GetKey(string key, int language)
	{
		TermData termData;
		termData = LocalizationManager.GetSourceContaining(key).GetTermData(key);
		string result;
		result = "CANNOT FIND TERM: " + key;
		if (termData != null)
		{
			result = termData.Languages[language];
		}
		return result;
	}
}
