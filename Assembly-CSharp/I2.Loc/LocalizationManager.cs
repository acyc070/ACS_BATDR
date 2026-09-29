using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace I2.Loc;

public static class LocalizationManager
{
	public delegate object _GetParam(string param);

	public delegate void OnLocalizeCallback();

	private static string mCurrentLanguage;

	private static string mLanguageCode;

	private static CultureInfo mCurrentCulture;

	private static bool mChangeCultureInfo;

	public static bool IsRight2Left;

	public static bool HasJoinedWords;

	public static List<ILocalizationParamsManager> ParamManagers;

	private static string[] LanguagesRTL;

	public static List<LanguageSourceData> Sources;

	public static string[] GlobalSources;

	private static string mCurrentDeviceLanguage;

	public static List<ILocalizeTargetDescriptor> mLocalizeTargets;

	private static bool mLocalizeIsScheduled;

	private static bool mLocalizeIsScheduledWithForcedValue;

	public static bool HighlightLocalizedTargets;

	public static string CurrentLanguage
	{
		get
		{
			LocalizationManager.InitializeIfNeeded();
			return LocalizationManager.mCurrentLanguage;
		}
		set
		{
			LocalizationManager.InitializeIfNeeded();
			string supportedLanguage;
			supportedLanguage = LocalizationManager.GetSupportedLanguage(value);
			if (!string.IsNullOrEmpty(supportedLanguage) && LocalizationManager.mCurrentLanguage != supportedLanguage)
			{
				LocalizationManager.SetLanguageAndCode(supportedLanguage, LocalizationManager.GetLanguageCode(supportedLanguage));
			}
		}
	}

	public static string CurrentLanguageCode
	{
		get
		{
			LocalizationManager.InitializeIfNeeded();
			return LocalizationManager.mLanguageCode;
		}
		set
		{
			LocalizationManager.InitializeIfNeeded();
			if (LocalizationManager.mLanguageCode != value)
			{
				string languageFromCode;
				languageFromCode = LocalizationManager.GetLanguageFromCode(value);
				if (!string.IsNullOrEmpty(languageFromCode))
				{
					LocalizationManager.SetLanguageAndCode(languageFromCode, value);
				}
			}
		}
	}

	public static string CurrentRegion
	{
		get
		{
			string currentLanguage;
			currentLanguage = LocalizationManager.CurrentLanguage;
			int num;
			num = currentLanguage.IndexOfAny("/\\".ToCharArray());
			unchecked
			{
				if (num > 0)
				{
					return currentLanguage.Substring(num + 1);
				}
				num = currentLanguage.IndexOfAny("[(".ToCharArray());
				int num2;
				num2 = currentLanguage.LastIndexOfAny("])".ToCharArray());
				if (num > 0 && num != num2)
				{
					return currentLanguage.Substring(num + 1, num2 - num - 1);
				}
				return string.Empty;
			}
		}
		set
		{
			string text;
			text = LocalizationManager.CurrentLanguage;
			int num;
			num = text.IndexOfAny("/\\".ToCharArray());
			if (num > 0)
			{
				LocalizationManager.CurrentLanguage = text.Substring(unchecked(num + 1)) + value;
				return;
			}
			num = text.IndexOfAny("[(".ToCharArray());
			int num2;
			num2 = text.LastIndexOfAny("])".ToCharArray());
			if (num > 0 && num != num2)
			{
				text = text.Substring(num);
			}
			LocalizationManager.CurrentLanguage = text + "(" + value + ")";
		}
	}

	public static string CurrentRegionCode
	{
		get
		{
			string currentLanguageCode;
			currentLanguageCode = LocalizationManager.CurrentLanguageCode;
			int num;
			num = currentLanguageCode.IndexOfAny(" -_/\\".ToCharArray());
			if (num >= 0)
			{
				return currentLanguageCode.Substring(unchecked(num + 1));
			}
			return string.Empty;
		}
		set
		{
			string text;
			text = LocalizationManager.CurrentLanguageCode;
			int num;
			num = text.IndexOfAny(" -_/\\".ToCharArray());
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			LocalizationManager.CurrentLanguageCode = text + "-" + value;
		}
	}

	public static CultureInfo CurrentCulture => LocalizationManager.mCurrentCulture;

	public static event OnLocalizeCallback OnLocalizeEvent;

	public static void InitializeIfNeeded()
	{
		if (string.IsNullOrEmpty(LocalizationManager.mCurrentLanguage) || LocalizationManager.Sources.Count == 0)
		{
			LocalizationManager.AutoLoadGlobalParamManagers();
			LocalizationManager.UpdateSources();
			LocalizationManager.SelectStartupLanguage();
		}
	}

	public static string GetVersion()
	{
		return "2.8.13 f2";
	}

	public static int GetRequiredWebServiceVersion()
	{
		return 5;
	}

	public static string GetWebServiceURL(LanguageSourceData source = null)
	{
		if (source != null && !string.IsNullOrEmpty(source.Google_WebServiceURL))
		{
			return source.Google_WebServiceURL;
		}
		LocalizationManager.InitializeIfNeeded();
		for (int i = 0; i < LocalizationManager.Sources.Count; i = unchecked(i + 1))
		{
			if (LocalizationManager.Sources[i] != null && !string.IsNullOrEmpty(LocalizationManager.Sources[i].Google_WebServiceURL))
			{
				return LocalizationManager.Sources[i].Google_WebServiceURL;
			}
		}
		return string.Empty;
	}

	public static void SetLanguageAndCode(string LanguageName, string LanguageCode, bool RememberLanguage = true, bool Force = false)
	{
		if ((LocalizationManager.mCurrentLanguage != LanguageName || LocalizationManager.mLanguageCode != LanguageCode) | Force)
		{
			if (RememberLanguage)
			{
				PersistentStorage.SetSetting_String("I2 Language", LanguageName);
			}
			LocalizationManager.mCurrentLanguage = LanguageName;
			LocalizationManager.mLanguageCode = LanguageCode;
			LocalizationManager.mCurrentCulture = LocalizationManager.CreateCultureForCode(LanguageCode);
			if (LocalizationManager.mChangeCultureInfo)
			{
				LocalizationManager.SetCurrentCultureInfo();
			}
			LocalizationManager.IsRight2Left = LocalizationManager.IsRTL(LocalizationManager.mLanguageCode);
			LocalizationManager.HasJoinedWords = GoogleLanguages.LanguageCode_HasJoinedWord(LocalizationManager.mLanguageCode);
			LocalizationManager.LocalizeAll(Force);
		}
	}

	private static CultureInfo CreateCultureForCode(string code)
	{
		try
		{
			return CultureInfo.CreateSpecificCulture(code);
		}
		catch (Exception)
		{
			return CultureInfo.InvariantCulture;
		}
	}

	public static void EnableChangingCultureInfo(bool bEnable)
	{
		if (!LocalizationManager.mChangeCultureInfo & bEnable)
		{
			LocalizationManager.SetCurrentCultureInfo();
		}
		LocalizationManager.mChangeCultureInfo = bEnable;
	}

	private static void SetCurrentCultureInfo()
	{
		Thread.CurrentThread.CurrentCulture = LocalizationManager.mCurrentCulture;
	}

	private static void SelectStartupLanguage()
	{
		if (LocalizationManager.Sources.Count == 0)
		{
			return;
		}
		string setting_String;
		setting_String = PersistentStorage.GetSetting_String("I2 Language", string.Empty);
		string currentDeviceLanguage;
		currentDeviceLanguage = LocalizationManager.GetCurrentDeviceLanguage();
		if (!string.IsNullOrEmpty(setting_String) && LocalizationManager.HasLanguage(setting_String, AllowDiscartingRegion: true, Initialize: false))
		{
			LocalizationManager.SetLanguageAndCode(setting_String, LocalizationManager.GetLanguageCode(setting_String));
			return;
		}
		if (!LocalizationManager.Sources[0].IgnoreDeviceLanguage)
		{
			string supportedLanguage;
			supportedLanguage = LocalizationManager.GetSupportedLanguage(currentDeviceLanguage, ignoreDisabled: true);
			if (!string.IsNullOrEmpty(supportedLanguage))
			{
				LocalizationManager.SetLanguageAndCode(supportedLanguage, LocalizationManager.GetLanguageCode(supportedLanguage), RememberLanguage: false);
				return;
			}
		}
		int i;
		i = 0;
		unchecked
		{
			for (int count = LocalizationManager.Sources.Count; i < count; i++)
			{
				if (LocalizationManager.Sources[i].mLanguages.Count <= 0)
				{
					continue;
				}
				for (int j = 0; j < LocalizationManager.Sources[i].mLanguages.Count; j++)
				{
					if (LocalizationManager.Sources[i].mLanguages[j].IsEnabled())
					{
						LocalizationManager.SetLanguageAndCode(LocalizationManager.Sources[i].mLanguages[j].Name, LocalizationManager.Sources[i].mLanguages[j].Code, RememberLanguage: false);
						return;
					}
				}
			}
		}
	}

	public static bool HasLanguage(string Language, bool AllowDiscartingRegion = true, bool Initialize = true, bool SkipDisabled = true)
	{
		if (Initialize)
		{
			LocalizationManager.InitializeIfNeeded();
		}
		int i;
		i = 0;
		unchecked
		{
			for (int count = LocalizationManager.Sources.Count; i < count; i++)
			{
				if (LocalizationManager.Sources[i].GetLanguageIndex(Language, AllowDiscartingRegion: false, SkipDisabled) >= 0)
				{
					return true;
				}
			}
			if (AllowDiscartingRegion)
			{
				int j;
				j = 0;
				for (int count2 = LocalizationManager.Sources.Count; j < count2; j++)
				{
					if (LocalizationManager.Sources[j].GetLanguageIndex(Language, AllowDiscartingRegion: true, SkipDisabled) >= 0)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public static string GetSupportedLanguage(string Language, bool ignoreDisabled = false)
	{
		string languageCode;
		languageCode = GoogleLanguages.GetLanguageCode(Language);
		unchecked
		{
			if (!string.IsNullOrEmpty(languageCode))
			{
				int i;
				i = 0;
				for (int count = LocalizationManager.Sources.Count; i < count; i++)
				{
					int languageIndexFromCode;
					languageIndexFromCode = LocalizationManager.Sources[i].GetLanguageIndexFromCode(languageCode, exactMatch: true, ignoreDisabled);
					if (languageIndexFromCode >= 0)
					{
						return LocalizationManager.Sources[i].mLanguages[languageIndexFromCode].Name;
					}
				}
				int j;
				j = 0;
				for (int count2 = LocalizationManager.Sources.Count; j < count2; j++)
				{
					int languageIndexFromCode2;
					languageIndexFromCode2 = LocalizationManager.Sources[j].GetLanguageIndexFromCode(languageCode, exactMatch: false, ignoreDisabled);
					if (languageIndexFromCode2 >= 0)
					{
						return LocalizationManager.Sources[j].mLanguages[languageIndexFromCode2].Name;
					}
				}
			}
			int k;
			k = 0;
			for (int count3 = LocalizationManager.Sources.Count; k < count3; k++)
			{
				int languageIndex;
				languageIndex = LocalizationManager.Sources[k].GetLanguageIndex(Language, AllowDiscartingRegion: false, ignoreDisabled);
				if (languageIndex >= 0)
				{
					return LocalizationManager.Sources[k].mLanguages[languageIndex].Name;
				}
			}
			int l;
			l = 0;
			for (int count4 = LocalizationManager.Sources.Count; l < count4; l++)
			{
				int languageIndex2;
				languageIndex2 = LocalizationManager.Sources[l].GetLanguageIndex(Language, AllowDiscartingRegion: true, ignoreDisabled);
				if (languageIndex2 >= 0)
				{
					return LocalizationManager.Sources[l].mLanguages[languageIndex2].Name;
				}
			}
			return string.Empty;
		}
	}

	public static string GetLanguageCode(string Language)
	{
		if (LocalizationManager.Sources.Count == 0)
		{
			LocalizationManager.UpdateSources();
		}
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			int languageIndex;
			languageIndex = LocalizationManager.Sources[i].GetLanguageIndex(Language);
			if (languageIndex >= 0)
			{
				return LocalizationManager.Sources[i].mLanguages[languageIndex].Code;
			}
		}
		return string.Empty;
	}

	public static string GetLanguageFromCode(string Code, bool exactMatch = true)
	{
		if (LocalizationManager.Sources.Count == 0)
		{
			LocalizationManager.UpdateSources();
		}
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			int languageIndexFromCode;
			languageIndexFromCode = LocalizationManager.Sources[i].GetLanguageIndexFromCode(Code, exactMatch);
			if (languageIndexFromCode >= 0)
			{
				return LocalizationManager.Sources[i].mLanguages[languageIndexFromCode].Name;
			}
		}
		return string.Empty;
	}

	public static List<string> GetAllLanguages(bool SkipDisabled = true)
	{
		if (LocalizationManager.Sources.Count == 0)
		{
			LocalizationManager.UpdateSources();
		}
		List<string> Languages;
		Languages = new List<string>();
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			Languages.AddRange(from x in LocalizationManager.Sources[i].GetLanguages(SkipDisabled)
				where !Languages.Contains(x)
				select x);
		}
		return Languages;
	}

	public static List<string> GetAllLanguagesCode(bool allowRegions = true, bool SkipDisabled = true)
	{
		List<string> Languages;
		Languages = new List<string>();
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			Languages.AddRange(from x in LocalizationManager.Sources[i].GetLanguagesCode(allowRegions, SkipDisabled)
				where !Languages.Contains(x)
				select x);
		}
		return Languages;
	}

	public static bool IsLanguageEnabled(string Language)
	{
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			if (!LocalizationManager.Sources[i].IsLanguageEnabled(Language))
			{
				return false;
			}
		}
		return true;
	}

	private static void LoadCurrentLanguage()
	{
		for (int i = 0; i < LocalizationManager.Sources.Count; i = unchecked(i + 1))
		{
			int languageIndex;
			languageIndex = LocalizationManager.Sources[i].GetLanguageIndex(LocalizationManager.mCurrentLanguage, AllowDiscartingRegion: true, SkipDisabled: false);
			LocalizationManager.Sources[i].LoadLanguage(languageIndex, UnloadOtherLanguages: true, useFallback: true, onlyCurrentSpecialization: true, forceLoad: false);
		}
	}

	public static void PreviewLanguage(string NewLanguage)
	{
		LocalizationManager.mCurrentLanguage = NewLanguage;
		LocalizationManager.mLanguageCode = LocalizationManager.GetLanguageCode(LocalizationManager.mCurrentLanguage);
		LocalizationManager.IsRight2Left = LocalizationManager.IsRTL(LocalizationManager.mLanguageCode);
		LocalizationManager.HasJoinedWords = GoogleLanguages.LanguageCode_HasJoinedWord(LocalizationManager.mLanguageCode);
	}

	public static void AutoLoadGlobalParamManagers()
	{
		LocalizationParamsManager[] array;
		array = global::UnityEngine.Object.FindObjectsOfType<LocalizationParamsManager>();
		foreach (LocalizationParamsManager localizationParamsManager in array)
		{
			if (localizationParamsManager._IsGlobalManager && !LocalizationManager.ParamManagers.Contains(localizationParamsManager))
			{
				Debug.Log(localizationParamsManager);
				LocalizationManager.ParamManagers.Add(localizationParamsManager);
			}
		}
	}

	public static void ApplyLocalizationParams(ref string translation, bool allowLocalizedParameters = true)
	{
		LocalizationManager.ApplyLocalizationParams(ref translation, (string p) => LocalizationManager.GetLocalizationParam(p, null), allowLocalizedParameters);
	}

	public static void ApplyLocalizationParams(ref string translation, GameObject root, bool allowLocalizedParameters = true)
	{
		LocalizationManager.ApplyLocalizationParams(ref translation, (string p) => LocalizationManager.GetLocalizationParam(p, root), allowLocalizedParameters);
	}

	public static void ApplyLocalizationParams(ref string translation, Dictionary<string, object> parameters, bool allowLocalizedParameters = true)
	{
		LocalizationManager.ApplyLocalizationParams(ref translation, (string p) =>
		{
			object value;
			value = null;
			return parameters.TryGetValue(p, out value) ? value : null;
		}, allowLocalizedParameters);
	}

	public static void ApplyLocalizationParams(ref string translation, _GetParam getParam, bool allowLocalizedParameters = true)
	{
		if (translation == null)
		{
			return;
		}
		string text;
		text = null;
		int num;
		num = 0;
		int length;
		length = translation.Length;
		int num2;
		num2 = 0;
		unchecked
		{
			while (num2 >= 0 && num2 < translation.Length)
			{
				int num3;
				num3 = translation.IndexOf("{[", num2);
				if (num3 < 0)
				{
					break;
				}
				int num4;
				num4 = translation.IndexOf("]}", num3);
				if (num4 < 0)
				{
					break;
				}
				int num5;
				num5 = translation.IndexOf("{[", num3 + 1);
				if (num5 > 0 && num5 < num4)
				{
					num2 = num5;
					continue;
				}
				int num6;
				num6 = ((translation[num3 + 2] == '#') ? 3 : 2);
				string text2;
				text2 = (string)getParam(translation.Substring(num3 + num6, num4 - num3 - num6));
				if ((text2 != null) & allowLocalizedParameters)
				{
					TermData termData;
					termData = LocalizationManager.GetTermData(text2, out var source);
					if (termData != null)
					{
						int languageIndex;
						languageIndex = source.GetLanguageIndex(LocalizationManager.CurrentLanguage);
						if (languageIndex >= 0)
						{
							text2 = termData.GetTranslation(languageIndex);
						}
					}
					string oldValue;
					oldValue = translation.Substring(num3, num4 - num3 + 2);
					translation = translation.Replace(oldValue, text2);
					int result;
					result = 0;
					if (int.TryParse(text2, out result))
					{
						text = GoogleLanguages.GetPluralType(LocalizationManager.CurrentLanguageCode, result).ToString();
					}
					num2 = num3 + text2.Length;
				}
				else
				{
					num2 = num4 + 2;
				}
			}
			if (text != null)
			{
				string text3;
				text3 = "[i2p_" + text + "]";
				num = translation.IndexOf(text3, StringComparison.OrdinalIgnoreCase);
				num = ((num >= 0) ? (num + text3.Length) : 0);
				length = translation.IndexOf("[i2p_", num + 1, StringComparison.OrdinalIgnoreCase);
				if (length < 0)
				{
					length = translation.Length;
				}
				translation = translation.Substring(num, length - num);
			}
		}
	}

	internal static string GetLocalizationParam(string ParamName, GameObject root)
	{
		string text;
		text = null;
		unchecked
		{
			if ((bool)root)
			{
				MonoBehaviour[] components;
				components = root.GetComponents<MonoBehaviour>();
				int i;
				i = 0;
				for (int num = components.Length; i < num; i++)
				{
					if (components[i] is ILocalizationParamsManager localizationParamsManager && components[i].enabled)
					{
						text = localizationParamsManager.GetParameterValue(ParamName);
						if (text != null)
						{
							return text;
						}
					}
				}
			}
			int j;
			j = 0;
			for (int count = LocalizationManager.ParamManagers.Count; j < count; j++)
			{
				text = LocalizationManager.ParamManagers[j].GetParameterValue(ParamName);
				if (text != null)
				{
					return text;
				}
			}
			return null;
		}
	}

	private static string GetPluralType(MatchCollection matches, string langCode, _GetParam getParam)
	{
		int i;
		i = 0;
		unchecked
		{
			for (int count = matches.Count; i < count; i++)
			{
				Match match;
				match = matches[i];
				string text;
				text = (string)getParam(match.Groups[match.Groups.Count - 1].Value);
				if (text != null)
				{
					int result;
					result = 0;
					if (int.TryParse(text, out result))
					{
						return GoogleLanguages.GetPluralType(langCode, result).ToString();
					}
				}
			}
			return null;
		}
	}

	public static string ApplyRTLfix(string line)
	{
		return LocalizationManager.ApplyRTLfix(line, 0, ignoreNumbers: true);
	}

	public static string ApplyRTLfix(string line, int maxCharacters, bool ignoreNumbers)
	{
		if (string.IsNullOrEmpty(line))
		{
			return line;
		}
		char c;
		c = line[0];
		if (c == '!' || c == '.' || c == '?')
		{
			line = line.Substring(1) + c;
		}
		int tagStart;
		tagStart = -1;
		int num;
		num = 0;
		int num2;
		num2 = 40000;
		num = 0;
		List<string> list;
		list = new List<string>();
		unchecked
		{
			while (I2Utils.FindNextTag(line, num, out tagStart, out num))
			{
				string text;
				text = "@@" + (char)(num2 + list.Count) + "@@";
				list.Add(line.Substring(tagStart, num - tagStart + 1));
				line = line.Substring(0, tagStart) + text + line.Substring(num + 1);
				num = tagStart + 5;
			}
			line = line.Replace("\r\n", "\n");
			line = I2Utils.SplitLine(line, maxCharacters);
			line = RTLFixer.Fix(line, showTashkeel: true, !ignoreNumbers);
			for (int i = 0; i < list.Count; i++)
			{
				int length;
				length = line.Length;
				for (int j = 0; j < length; j++)
				{
					if (line[j] == '@' && line[j + 1] == '@' && line[j + 2] >= num2 && line[j + 3] == '@' && line[j + 4] == '@')
					{
						int num3;
						num3 = line[j + 2] - num2;
						num3 = ((num3 % 2 != 0) ? (num3 - 1) : (num3 + 1));
						if (num3 >= list.Count)
						{
							num3 = list.Count - 1;
						}
						line = line.Substring(0, j) + list[num3] + line.Substring(j + 5);
						break;
					}
				}
			}
			return line;
		}
	}

	public static string FixRTL_IfNeeded(string text, int maxCharacters = 0, bool ignoreNumber = false)
	{
		if (LocalizationManager.IsRight2Left)
		{
			return LocalizationManager.ApplyRTLfix(text, maxCharacters, ignoreNumber);
		}
		return text;
	}

	public static bool IsRTL(string Code)
	{
		return Array.IndexOf(LocalizationManager.LanguagesRTL, Code) >= 0;
	}

	public static bool UpdateSources()
	{
		LocalizationManager.UnregisterDeletededSources();
		LocalizationManager.RegisterSourceInResources();
		LocalizationManager.RegisterSceneSources();
		return LocalizationManager.Sources.Count > 0;
	}

	private static void UnregisterDeletededSources()
	{
		unchecked
		{
			for (int num = LocalizationManager.Sources.Count - 1; num >= 0; num--)
			{
				if (LocalizationManager.Sources[num] == null)
				{
					LocalizationManager.RemoveSource(LocalizationManager.Sources[num]);
				}
			}
		}
	}

	private static void RegisterSceneSources()
	{
		LanguageSource[] array;
		array = (LanguageSource[])Resources.FindObjectsOfTypeAll(typeof(LanguageSource));
		foreach (LanguageSource languageSource in array)
		{
			if (!LocalizationManager.Sources.Contains(languageSource.mSource))
			{
				if (languageSource.mSource.owner == null)
				{
					languageSource.mSource.owner = languageSource;
				}
				LocalizationManager.AddSource(languageSource.mSource);
			}
		}
	}

	private static void RegisterSourceInResources()
	{
		string[] globalSources;
		globalSources = LocalizationManager.GlobalSources;
		foreach (string name in globalSources)
		{
			LanguageSourceAsset asset;
			asset = ResourceManager.pInstance.GetAsset<LanguageSourceAsset>(name);
			if ((bool)asset && !LocalizationManager.Sources.Contains(asset.mSource))
			{
				if (!asset.mSource.mIsGlobalSource)
				{
					asset.mSource.mIsGlobalSource = true;
				}
				asset.mSource.owner = asset;
				LocalizationManager.AddSource(asset.mSource);
			}
		}
	}

	internal static void AddSource(LanguageSourceData Source)
	{
		if (LocalizationManager.Sources.Contains(Source))
		{
			return;
		}
		LocalizationManager.Sources.Add(Source);
		if (Source.HasGoogleSpreadsheet() && Source.GoogleUpdateFrequency != LanguageSourceData.eGoogleUpdateFrequency.Never)
		{
			Source.Import_Google_FromCache();
			bool justCheck;
			justCheck = false;
			if (Source.GoogleUpdateDelay > 0f)
			{
				CoroutineManager.Start(LocalizationManager.Delayed_Import_Google(Source, Source.GoogleUpdateDelay, justCheck));
			}
			else
			{
				Source.Import_Google(ForceUpdate: false, justCheck);
			}
		}
		for (int i = 0; i < Source.mLanguages.Count(); i = unchecked(i + 1))
		{
			Source.mLanguages[i].SetLoaded(loaded: true);
		}
		if (Source.mDictionary.Count == 0)
		{
			Source.UpdateDictionary(force: true);
		}
	}

	private static IEnumerator Delayed_Import_Google(LanguageSourceData source, float delay, bool justCheck)
	{
		yield return new WaitForSeconds(delay);
		source?.Import_Google(ForceUpdate: false, justCheck);
	}

	internal static void RemoveSource(LanguageSourceData Source)
	{
		LocalizationManager.Sources.Remove(Source);
	}

	public static bool IsGlobalSource(string SourceName)
	{
		return Array.IndexOf(LocalizationManager.GlobalSources, SourceName) >= 0;
	}

	public static LanguageSourceData GetSourceContaining(string term, bool fallbackToFirst = true)
	{
		if (!string.IsNullOrEmpty(term))
		{
			int i;
			i = 0;
			for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
			{
				if (LocalizationManager.Sources[i].GetTermData(term) != null)
				{
					return LocalizationManager.Sources[i];
				}
			}
		}
		if (!fallbackToFirst || LocalizationManager.Sources.Count <= 0)
		{
			return null;
		}
		return LocalizationManager.Sources[0];
	}

	public static global::UnityEngine.Object FindAsset(string value)
	{
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			global::UnityEngine.Object obj;
			obj = LocalizationManager.Sources[i].FindAsset(value);
			if ((bool)obj)
			{
				return obj;
			}
		}
		return null;
	}

	public static void ApplyDownloadedDataFromGoogle()
	{
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			LocalizationManager.Sources[i].ApplyDownloadedDataFromGoogle();
		}
	}

	public static string GetCurrentDeviceLanguage(bool force = false)
	{
		if (force || string.IsNullOrEmpty(LocalizationManager.mCurrentDeviceLanguage))
		{
			LocalizationManager.DetectDeviceLanguage();
		}
		return LocalizationManager.mCurrentDeviceLanguage;
	}

	private static void DetectDeviceLanguage()
	{
		LocalizationManager.mCurrentDeviceLanguage = Application.systemLanguage.ToString();
		if (LocalizationManager.mCurrentDeviceLanguage == "ChineseSimplified")
		{
			LocalizationManager.mCurrentDeviceLanguage = "Chinese (Simplified)";
		}
		if (LocalizationManager.mCurrentDeviceLanguage == "ChineseTraditional")
		{
			LocalizationManager.mCurrentDeviceLanguage = "Chinese (Traditional)";
		}
	}

	public static void RegisterTarget(ILocalizeTargetDescriptor desc)
	{
		if (LocalizationManager.mLocalizeTargets.FindIndex((ILocalizeTargetDescriptor x) => x.Name == desc.Name) != -1)
		{
			return;
		}
		for (int num = 0; num < LocalizationManager.mLocalizeTargets.Count; num = unchecked(num + 1))
		{
			if (LocalizationManager.mLocalizeTargets[num].Priority > desc.Priority)
			{
				LocalizationManager.mLocalizeTargets.Insert(num, desc);
				return;
			}
		}
		LocalizationManager.mLocalizeTargets.Add(desc);
	}

	public static string GetTranslation(string Term, bool FixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
	{
		string Translation;
		Translation = null;
		LocalizationManager.TryGetTranslation(Term, out Translation, FixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage);
		return Translation;
	}

	public static string GetTermTranslation(string Term, bool FixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
	{
		return LocalizationManager.GetTranslation(Term, FixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage);
	}

	public static bool TryGetTranslation(string Term, out string Translation, bool FixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
	{
		Translation = null;
		if (string.IsNullOrEmpty(Term))
		{
			return false;
		}
		LocalizationManager.InitializeIfNeeded();
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			if (LocalizationManager.Sources[i].TryGetTranslation(Term, out Translation, overrideLanguage))
			{
				if (applyParameters)
				{
					LocalizationManager.ApplyLocalizationParams(ref Translation, localParametersRoot);
				}
				if (LocalizationManager.IsRight2Left & FixForRTL)
				{
					Translation = LocalizationManager.ApplyRTLfix(Translation, maxLineLengthForRTL, ignoreRTLnumbers);
				}
				return true;
			}
		}
		return false;
	}

	public static T GetTranslatedObject<T>(string AssetName, Localize optionalLocComp = null) where T : global::UnityEngine.Object
	{
		if (optionalLocComp != null)
		{
			return optionalLocComp.FindTranslatedObject<T>(AssetName);
		}
		T val;
		val = LocalizationManager.FindAsset(AssetName) as T;
		if ((bool)val)
		{
			return val;
		}
		return ResourceManager.pInstance.GetAsset<T>(AssetName);
	}

	public static T GetTranslatedObjectByTermName<T>(string Term, Localize optionalLocComp = null) where T : global::UnityEngine.Object
	{
		return LocalizationManager.GetTranslatedObject<T>(LocalizationManager.GetTranslation(Term, FixForRTL: false));
	}

	public static string GetAppName(string languageCode)
	{
		if (!string.IsNullOrEmpty(languageCode))
		{
			for (int i = 0; i < LocalizationManager.Sources.Count; i = unchecked(i + 1))
			{
				if (string.IsNullOrEmpty(LocalizationManager.Sources[i].mTerm_AppName))
				{
					continue;
				}
				int languageIndexFromCode;
				languageIndexFromCode = LocalizationManager.Sources[i].GetLanguageIndexFromCode(languageCode, exactMatch: false);
				if (languageIndexFromCode < 0)
				{
					continue;
				}
				TermData termData;
				termData = LocalizationManager.Sources[i].GetTermData(LocalizationManager.Sources[i].mTerm_AppName);
				if (termData != null)
				{
					string translation;
					translation = termData.GetTranslation(languageIndexFromCode);
					if (!string.IsNullOrEmpty(translation))
					{
						return translation;
					}
				}
			}
		}
		return Application.productName;
	}

	public static void LocalizeAll(bool Force = false)
	{
		LocalizationManager.LoadCurrentLanguage();
		if (!Application.isPlaying)
		{
			LocalizationManager.DoLocalizeAll(Force);
			return;
		}
		LocalizationManager.mLocalizeIsScheduledWithForcedValue |= Force;
		if (!LocalizationManager.mLocalizeIsScheduled)
		{
			CoroutineManager.Start(LocalizationManager.Coroutine_LocalizeAll());
		}
	}

	private static IEnumerator Coroutine_LocalizeAll()
	{
		LocalizationManager.mLocalizeIsScheduled = true;
		yield return null;
		LocalizationManager.mLocalizeIsScheduled = false;
		bool force;
		force = LocalizationManager.mLocalizeIsScheduledWithForcedValue;
		LocalizationManager.mLocalizeIsScheduledWithForcedValue = false;
		LocalizationManager.DoLocalizeAll(force);
	}

	private static void DoLocalizeAll(bool Force = false)
	{
		Localize[] array;
		array = (Localize[])Resources.FindObjectsOfTypeAll(typeof(Localize));
		int i;
		i = 0;
		for (int num = array.Length; i < num; i = unchecked(i + 1))
		{
			array[i].OnLocalize(Force);
		}
		if (LocalizationManager.OnLocalizeEvent != null)
		{
			LocalizationManager.OnLocalizeEvent();
		}
	}

	public static List<string> GetCategories()
	{
		List<string> list;
		list = new List<string>();
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			LocalizationManager.Sources[i].GetCategories(OnlyMainCategory: false, list);
		}
		return list;
	}

	public static List<string> GetTermsList(string Category = null)
	{
		if (LocalizationManager.Sources.Count == 0)
		{
			LocalizationManager.UpdateSources();
		}
		if (LocalizationManager.Sources.Count == 1)
		{
			return LocalizationManager.Sources[0].GetTermsList(Category);
		}
		HashSet<string> hashSet;
		hashSet = new HashSet<string>();
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			hashSet.UnionWith(LocalizationManager.Sources[i].GetTermsList(Category));
		}
		return new List<string>(hashSet);
	}

	public static TermData GetTermData(string term)
	{
		LocalizationManager.InitializeIfNeeded();
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			TermData termData;
			termData = LocalizationManager.Sources[i].GetTermData(term);
			if (termData != null)
			{
				return termData;
			}
		}
		return null;
	}

	public static TermData GetTermData(string term, out LanguageSourceData source)
	{
		LocalizationManager.InitializeIfNeeded();
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			TermData termData;
			termData = LocalizationManager.Sources[i].GetTermData(term);
			if (termData != null)
			{
				source = LocalizationManager.Sources[i];
				return termData;
			}
		}
		source = null;
		return null;
	}

	static LocalizationManager()
	{
		LocalizationManager.mChangeCultureInfo = false;
		LocalizationManager.IsRight2Left = false;
		LocalizationManager.HasJoinedWords = false;
		LocalizationManager.ParamManagers = new List<ILocalizationParamsManager>();
		LocalizationManager.LanguagesRTL = new string[21]
		{
			"ar-DZ", "ar", "ar-BH", "ar-EG", "ar-IQ", "ar-JO", "ar-KW", "ar-LB", "ar-LY", "ar-MA",
			"ar-OM", "ar-QA", "ar-SA", "ar-SY", "ar-TN", "ar-AE", "ar-YE", "fa", "he", "ur",
			"ji"
		};
		LocalizationManager.Sources = new List<LanguageSourceData>();
		LocalizationManager.GlobalSources = new string[1] { "I2Languages" };
		LocalizationManager.mLocalizeTargets = new List<ILocalizeTargetDescriptor>();
		LocalizationManager.mLocalizeIsScheduled = false;
		LocalizationManager.mLocalizeIsScheduledWithForcedValue = false;
		LocalizationManager.HighlightLocalizedTargets = false;
	}
}
