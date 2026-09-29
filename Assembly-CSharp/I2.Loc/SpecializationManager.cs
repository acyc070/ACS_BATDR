using System.Collections.Generic;

namespace I2.Loc;

public class SpecializationManager : BaseSpecializationManager
{
	public static SpecializationManager Singleton;

	private SpecializationManager()
	{
		this.InitializeSpecializations();
	}

	public static string GetSpecializedText(string text, string specialization = null)
	{
		int num;
		num = text.IndexOf("[i2s_");
		if (num < 0)
		{
			return text;
		}
		if (string.IsNullOrEmpty(specialization))
		{
			specialization = SpecializationManager.Singleton.GetCurrentSpecialization();
		}
		unchecked
		{
			while (!string.IsNullOrEmpty(specialization) && specialization != "Any")
			{
				string text2;
				text2 = "[i2s_" + specialization + "]";
				int num2;
				num2 = text.IndexOf(text2);
				if (num2 < 0)
				{
					specialization = SpecializationManager.Singleton.GetFallbackSpecialization(specialization);
					continue;
				}
				num2 += text2.Length;
				int num3;
				num3 = text.IndexOf("[i2s_", num2);
				if (num3 < 0)
				{
					num3 = text.Length;
				}
				return text.Substring(num2, num3 - num2);
			}
			return text.Substring(0, num);
		}
	}

	public static string SetSpecializedText(string text, string newText, string specialization)
	{
		if (string.IsNullOrEmpty(specialization))
		{
			specialization = "Any";
		}
		if ((text == null || !text.Contains("[i2s_")) && specialization == "Any")
		{
			return newText;
		}
		Dictionary<string, string> specializations;
		specializations = SpecializationManager.GetSpecializations(text);
		specializations[specialization] = newText;
		return SpecializationManager.SetSpecializedText(specializations);
	}

	public static string SetSpecializedText(Dictionary<string, string> specializations)
	{
		if (!specializations.TryGetValue("Any", out var value))
		{
			value = string.Empty;
		}
		foreach (KeyValuePair<string, string> specialization in specializations)
		{
			if (specialization.Key != "Any" && !string.IsNullOrEmpty(specialization.Value))
			{
				value = value + "[i2s_" + specialization.Key + "]" + specialization.Value;
			}
		}
		return value;
	}

	public static Dictionary<string, string> GetSpecializations(string text, Dictionary<string, string> buffer = null)
	{
		if (buffer == null)
		{
			buffer = new Dictionary<string, string>();
		}
		else
		{
			buffer.Clear();
		}
		if (text == null)
		{
			buffer["Any"] = "";
			return buffer;
		}
		int num;
		num = 0;
		int num2;
		num2 = text.IndexOf("[i2s_");
		if (num2 < 0)
		{
			num2 = text.Length;
		}
		buffer["Any"] = text.Substring(0, num2);
		unchecked
		{
			for (num = num2; num < text.Length; num = num2)
			{
				num += "[i2s_".Length;
				int num3;
				num3 = text.IndexOf(']', num);
				if (num3 < 0)
				{
					break;
				}
				string key;
				key = text.Substring(num, num3 - num);
				num = num3 + 1;
				num2 = text.IndexOf("[i2s_", num);
				if (num2 < 0)
				{
					num2 = text.Length;
				}
				buffer[key] = text.Substring(num, num2 - num);
			}
			return buffer;
		}
	}

	public static void AppendSpecializations(string text, List<string> list = null)
	{
		if (text == null)
		{
			return;
		}
		if (list == null)
		{
			list = new List<string>();
		}
		if (!list.Contains("Any"))
		{
			list.Add("Any");
		}
		int num;
		num = 0;
		unchecked
		{
			while (num < text.Length)
			{
				num = text.IndexOf("[i2s_", num);
				if (num >= 0)
				{
					num += "[i2s_".Length;
					int num2;
					num2 = text.IndexOf(']', num);
					if (num2 >= 0)
					{
						string item;
						item = text.Substring(num, num2 - num);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
						continue;
					}
					break;
				}
				break;
			}
		}
	}

	static SpecializationManager()
	{
		SpecializationManager.Singleton = new SpecializationManager();
	}
}
