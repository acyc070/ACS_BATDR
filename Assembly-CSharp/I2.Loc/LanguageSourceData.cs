using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace I2.Loc;

[Serializable]
[ExecuteInEditMode]
public class LanguageSourceData
{
	public enum MissingTranslationAction
	{
		Empty = 0,
		Fallback = 1,
		ShowWarning = 2,
		ShowTerm = 3
	}

	public enum eAllowUnloadLanguages
	{
		Never = 0,
		OnlyInDevice = 1,
		EditorAndDevice = 2
	}

	public enum eGoogleUpdateFrequency
	{
		Always = 0,
		Never = 1,
		Daily = 2,
		Weekly = 3,
		Monthly = 4,
		OnlyOnce = 5,
		EveryOtherDay = 6
	}

	public enum eGoogleUpdateSynchronization
	{
		Manual = 0,
		OnSceneLoaded = 1,
		AsSoonAsDownloaded = 2
	}

	[NonSerialized]
	public ILanguageSource owner;

	public bool UserAgreesToHaveItOnTheScene;

	public bool UserAgreesToHaveItInsideThePluginsFolder;

	public bool GoogleLiveSyncIsUptoDate = true;

	[NonSerialized]
	public bool mIsGlobalSource;

	public List<TermData> mTerms = new List<TermData>();

	public bool CaseInsensitiveTerms;

	[NonSerialized]
	public Dictionary<string, TermData> mDictionary = new Dictionary<string, TermData>(StringComparer.Ordinal);

	public MissingTranslationAction OnMissingTranslation = MissingTranslationAction.Fallback;

	public string mTerm_AppName;

	public List<LanguageData> mLanguages = new List<LanguageData>();

	public bool IgnoreDeviceLanguage;

	public eAllowUnloadLanguages _AllowUnloadingLanguages;

	public string Google_WebServiceURL;

	public string Google_SpreadsheetKey;

	public string Google_SpreadsheetName;

	public string Google_LastUpdatedVersion;

	public eGoogleUpdateFrequency GoogleUpdateFrequency = eGoogleUpdateFrequency.Weekly;

	public eGoogleUpdateFrequency GoogleInEditorCheckFrequency = eGoogleUpdateFrequency.Daily;

	public eGoogleUpdateSynchronization GoogleUpdateSynchronization = eGoogleUpdateSynchronization.OnSceneLoaded;

	public float GoogleUpdateDelay;

	public List<global::UnityEngine.Object> Assets = new List<global::UnityEngine.Object>();

	[NonSerialized]
	public Dictionary<string, global::UnityEngine.Object> mAssetDictionary = new Dictionary<string, global::UnityEngine.Object>(StringComparer.Ordinal);

	private string mDelayedGoogleData;

	public static string EmptyCategory;

	public static char[] CategorySeparators;

	public global::UnityEngine.Object ownerObject => this.owner as global::UnityEngine.Object;

	public event LanguageSource.fnOnSourceUpdated Event_OnSourceUpdateFromGoogle;

	public void Awake()
	{
		LocalizationManager.AddSource(this);
		this.UpdateDictionary();
		this.UpdateAssetDictionary();
		LocalizationManager.LocalizeAll(Force: true);
	}

	public void OnDestroy()
	{
		LocalizationManager.RemoveSource(this);
	}

	public bool IsEqualTo(LanguageSourceData Source)
	{
		if (Source.mLanguages.Count != this.mLanguages.Count)
		{
			return false;
		}
		int i;
		i = 0;
		unchecked
		{
			for (int count = this.mLanguages.Count; i < count; i++)
			{
				if (Source.GetLanguageIndex(this.mLanguages[i].Name) < 0)
				{
					return false;
				}
			}
			if (Source.mTerms.Count != this.mTerms.Count)
			{
				return false;
			}
			for (int j = 0; j < this.mTerms.Count; j++)
			{
				if (Source.GetTermData(this.mTerms[j].Term) == null)
				{
					return false;
				}
			}
			return true;
		}
	}

	internal bool ManagerHasASimilarSource()
	{
		int i;
		i = 0;
		for (int count = LocalizationManager.Sources.Count; i < count; i = unchecked(i + 1))
		{
			LanguageSourceData languageSourceData;
			languageSourceData = LocalizationManager.Sources[i];
			if (languageSourceData != null && languageSourceData.IsEqualTo(this) && languageSourceData != this)
			{
				return true;
			}
		}
		return false;
	}

	public void ClearAllData()
	{
		this.mTerms.Clear();
		this.mLanguages.Clear();
		this.mDictionary.Clear();
		this.mAssetDictionary.Clear();
	}

	public bool IsGlobalSource()
	{
		return this.mIsGlobalSource;
	}

	public void Editor_SetDirty()
	{
	}

	public void UpdateAssetDictionary()
	{
		this.Assets.RemoveAll((global::UnityEngine.Object x) => x == null);
		this.mAssetDictionary = (from o in this.Assets.Distinct()
			group o by o.name).ToDictionary((IGrouping<string, global::UnityEngine.Object> g) => g.Key, (IGrouping<string, global::UnityEngine.Object> g) => g.First());
	}

	public global::UnityEngine.Object FindAsset(string Name)
	{
		if (this.Assets != null)
		{
			if (this.mAssetDictionary == null || this.mAssetDictionary.Count != this.Assets.Count)
			{
				this.UpdateAssetDictionary();
			}
			if (this.mAssetDictionary.TryGetValue(Name, out var value))
			{
				return value;
			}
		}
		return null;
	}

	public bool HasAsset(global::UnityEngine.Object Obj)
	{
		return this.Assets.Contains(Obj);
	}

	public void AddAsset(global::UnityEngine.Object Obj)
	{
		if (!this.Assets.Contains(Obj))
		{
			this.Assets.Add(Obj);
			this.UpdateAssetDictionary();
		}
	}

	public string Export_I2CSV(string Category, char Separator = ',', bool specializationsAsRows = true)
	{
		StringBuilder stringBuilder;
		stringBuilder = new StringBuilder();
		stringBuilder.Append("Key[*]Type[*]Desc");
		foreach (LanguageData mLanguage in this.mLanguages)
		{
			stringBuilder.Append("[*]");
			if (!mLanguage.IsEnabled())
			{
				stringBuilder.Append('$');
			}
			stringBuilder.Append(GoogleLanguages.GetCodedLanguage(mLanguage.Name, mLanguage.Code));
		}
		stringBuilder.Append("[ln]");
		this.mTerms.Sort((TermData a, TermData b) => string.CompareOrdinal(a.Term, b.Term));
		int count;
		count = this.mLanguages.Count;
		bool flag;
		flag = true;
		unchecked
		{
			foreach (TermData mTerm in this.mTerms)
			{
				string term;
				if (string.IsNullOrEmpty(Category) || (Category == LanguageSourceData.EmptyCategory && mTerm.Term.IndexOfAny(LanguageSourceData.CategorySeparators) < 0))
				{
					term = mTerm.Term;
				}
				else
				{
					if (!mTerm.Term.StartsWith(Category + "/") || !(Category != mTerm.Term))
					{
						continue;
					}
					term = mTerm.Term.Substring(Category.Length + 1);
				}
				if (!flag)
				{
					stringBuilder.Append("[ln]");
				}
				flag = false;
				if (!specializationsAsRows)
				{
					LanguageSourceData.AppendI2Term(stringBuilder, count, term, mTerm, Separator, null);
					continue;
				}
				List<string> allSpecializations;
				allSpecializations = mTerm.GetAllSpecializations();
				for (int num = 0; num < allSpecializations.Count; num++)
				{
					if (num != 0)
					{
						stringBuilder.Append("[ln]");
					}
					LanguageSourceData.AppendI2Term(stringBuilder, count, term, mTerm, Separator, allSpecializations[num]);
				}
			}
			return stringBuilder.ToString();
		}
	}

	private static void AppendI2Term(StringBuilder Builder, int nLanguages, string Term, TermData termData, char Separator, string forceSpecialization)
	{
		LanguageSourceData.AppendI2Text(Builder, Term);
		if (!string.IsNullOrEmpty(forceSpecialization) && forceSpecialization != "Any")
		{
			Builder.Append("[");
			Builder.Append(forceSpecialization);
			Builder.Append("]");
		}
		Builder.Append("[*]");
		Builder.Append(termData.TermType.ToString());
		Builder.Append("[*]");
		Builder.Append(termData.Description);
		for (int i = 0; i < Mathf.Min(nLanguages, termData.Languages.Length); i = unchecked(i + 1))
		{
			Builder.Append("[*]");
			string text;
			text = termData.Languages[i];
			if (!string.IsNullOrEmpty(forceSpecialization))
			{
				text = termData.GetTranslation(i, forceSpecialization);
			}
			LanguageSourceData.AppendI2Text(Builder, text);
		}
	}

	private static void AppendI2Text(StringBuilder Builder, string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			if (text.StartsWith("'") || text.StartsWith("="))
			{
				Builder.Append('\'');
			}
			Builder.Append(text);
		}
	}

	private string Export_Language_to_Cache(int langIndex, bool fillTermWithFallback)
	{
		if (!this.mLanguages[langIndex].IsLoaded())
		{
			return null;
		}
		StringBuilder stringBuilder;
		stringBuilder = new StringBuilder();
		for (int i = 0; i < this.mTerms.Count; i = unchecked(i + 1))
		{
			if (i > 0)
			{
				stringBuilder.Append("[i2t]");
			}
			TermData termData;
			termData = this.mTerms[i];
			stringBuilder.Append(termData.Term);
			stringBuilder.Append("=");
			string Translation;
			Translation = termData.Languages[langIndex];
			if (this.OnMissingTranslation == MissingTranslationAction.Fallback && string.IsNullOrEmpty(Translation) && this.TryGetFallbackTranslation(termData, out Translation, langIndex, null, skipDisabled: true))
			{
				stringBuilder.Append("[i2fb]");
				if (fillTermWithFallback)
				{
					termData.Languages[langIndex] = Translation;
				}
			}
			if (!string.IsNullOrEmpty(Translation))
			{
				stringBuilder.Append(Translation);
			}
		}
		return stringBuilder.ToString();
	}

	public string Export_CSV(string Category, char Separator = ',', bool specializationsAsRows = true)
	{
		StringBuilder stringBuilder;
		stringBuilder = new StringBuilder();
		int count;
		count = this.mLanguages.Count;
		stringBuilder.AppendFormat("Key{0}Type{0}Desc", Separator);
		foreach (LanguageData mLanguage in this.mLanguages)
		{
			stringBuilder.Append(Separator);
			if (!mLanguage.IsEnabled())
			{
				stringBuilder.Append('$');
			}
			LanguageSourceData.AppendString(stringBuilder, GoogleLanguages.GetCodedLanguage(mLanguage.Name, mLanguage.Code), Separator);
		}
		stringBuilder.Append("\n");
		this.mTerms.Sort((TermData a, TermData b) => string.CompareOrdinal(a.Term, b.Term));
		foreach (TermData mTerm in this.mTerms)
		{
			string term;
			if (string.IsNullOrEmpty(Category) || (Category == LanguageSourceData.EmptyCategory && mTerm.Term.IndexOfAny(LanguageSourceData.CategorySeparators) < 0))
			{
				term = mTerm.Term;
			}
			else
			{
				if (!mTerm.Term.StartsWith(Category + "/") || !(Category != mTerm.Term))
				{
					continue;
				}
				term = mTerm.Term.Substring(unchecked(Category.Length + 1));
			}
			if (specializationsAsRows)
			{
				foreach (string allSpecialization in mTerm.GetAllSpecializations())
				{
					LanguageSourceData.AppendTerm(stringBuilder, count, term, mTerm, allSpecialization, Separator);
				}
			}
			else
			{
				LanguageSourceData.AppendTerm(stringBuilder, count, term, mTerm, null, Separator);
			}
		}
		return stringBuilder.ToString();
	}

	private static void AppendTerm(StringBuilder Builder, int nLanguages, string Term, TermData termData, string specialization, char Separator)
	{
		LanguageSourceData.AppendString(Builder, Term, Separator);
		if (!string.IsNullOrEmpty(specialization) && specialization != "Any")
		{
			Builder.AppendFormat("[{0}]", specialization);
		}
		Builder.Append(Separator);
		Builder.Append(termData.TermType.ToString());
		Builder.Append(Separator);
		LanguageSourceData.AppendString(Builder, termData.Description, Separator);
		for (int i = 0; i < Mathf.Min(nLanguages, termData.Languages.Length); i = unchecked(i + 1))
		{
			Builder.Append(Separator);
			string text;
			text = termData.Languages[i];
			if (!string.IsNullOrEmpty(specialization))
			{
				text = termData.GetTranslation(i, specialization);
			}
			LanguageSourceData.AppendTranslation(Builder, text, Separator, null);
		}
		Builder.Append("\n");
	}

	private static void AppendString(StringBuilder Builder, string Text, char Separator)
	{
		if (!string.IsNullOrEmpty(Text))
		{
			Text = Text.Replace("\\n", "\n");
			if (Text.IndexOfAny((Separator + "\n\"").ToCharArray()) >= 0)
			{
				Text = Text.Replace("\"", "\"\"");
				Builder.AppendFormat("\"{0}\"", Text);
			}
			else
			{
				Builder.Append(Text);
			}
		}
	}

	private static void AppendTranslation(StringBuilder Builder, string Text, char Separator, string tags)
	{
		if (!string.IsNullOrEmpty(Text))
		{
			Text = Text.Replace("\\n", "\n");
			if (Text.IndexOfAny((Separator + "\n\"").ToCharArray()) >= 0)
			{
				Text = Text.Replace("\"", "\"\"");
				Builder.AppendFormat("\"{0}{1}\"", tags, Text);
			}
			else
			{
				Builder.Append(tags);
				Builder.Append(Text);
			}
		}
	}

	public UnityWebRequest Export_Google_CreateWWWcall(eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace)
	{
		string value;
		value = this.Export_Google_CreateData();
		WWWForm wWWForm;
		wWWForm = new WWWForm();
		wWWForm.AddField("key", this.Google_SpreadsheetKey);
		wWWForm.AddField("action", "SetLanguageSource");
		wWWForm.AddField("data", value);
		wWWForm.AddField("updateMode", UpdateMode.ToString());
		UnityWebRequest unityWebRequest;
		unityWebRequest = UnityWebRequest.Post(LocalizationManager.GetWebServiceURL(this), wWWForm);
		I2Utils.SendWebRequest(unityWebRequest);
		return unityWebRequest;
	}

	private string Export_Google_CreateData()
	{
		List<string> categories;
		categories = this.GetCategories(OnlyMainCategory: true);
		StringBuilder stringBuilder;
		stringBuilder = new StringBuilder();
		bool flag;
		flag = true;
		foreach (string item in categories)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append("<I2Loc>");
			}
			string value;
			value = this.Export_I2CSV(item);
			stringBuilder.Append(item);
			stringBuilder.Append("<I2Loc>");
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	public string Import_CSV(string Category, string CSVstring, eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace, char Separator = ',')
	{
		return this.Import_CSV(Category, LocalizationReader.ReadCSV(CSVstring, Separator), UpdateMode);
	}

	public string Import_I2CSV(string Category, string I2CSVstring, eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace)
	{
		return this.Import_CSV(Category, LocalizationReader.ReadI2CSV(I2CSVstring), UpdateMode);
	}

	public string Import_CSV(string Category, List<string[]> CSV, eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace)
	{
		string[] array;
		array = CSV[0];
		int num;
		num = 1;
		int num2;
		num2 = -1;
		int num3;
		num3 = -1;
		string[] texts;
		texts = new string[1] { "Key" };
		string[] texts2;
		texts2 = new string[1] { "Type" };
		string[] texts3;
		texts3 = new string[2] { "Desc", "Description" };
		unchecked
		{
			if (array.Length > 1 && this.ArrayContains(array[0], texts))
			{
				if (UpdateMode == eSpreadsheetUpdateMode.Replace)
				{
					this.ClearAllData();
				}
				if (array.Length > 2)
				{
					if (this.ArrayContains(array[1], texts2))
					{
						num2 = 1;
						num = 2;
					}
					if (this.ArrayContains(array[1], texts3))
					{
						num3 = 1;
						num = 2;
					}
				}
				if (array.Length > 3)
				{
					if (this.ArrayContains(array[2], texts2))
					{
						num2 = 2;
						num = 3;
					}
					if (this.ArrayContains(array[2], texts3))
					{
						num3 = 2;
						num = 3;
					}
				}
				int num4;
				num4 = Mathf.Max(array.Length - num, 0);
				int[] array2;
				array2 = new int[num4];
				for (int i = 0; i < num4; i++)
				{
					if (string.IsNullOrEmpty(array[i + num]))
					{
						array2[i] = -1;
						continue;
					}
					string text;
					text = array[i + num];
					bool flag;
					flag = true;
					if (text.StartsWith("$"))
					{
						flag = false;
						text = text.Substring(1);
					}
					GoogleLanguages.UnPackCodeFromLanguageName(text, out var Language, out var code);
					int num5;
					num5 = -1;
					num5 = (string.IsNullOrEmpty(code) ? this.GetLanguageIndex(Language, AllowDiscartingRegion: true, SkipDisabled: false) : this.GetLanguageIndexFromCode(code));
					if (num5 < 0)
					{
						LanguageData languageData;
						languageData = new LanguageData();
						languageData.Name = Language;
						languageData.Code = code;
						languageData.Flags = (byte)(0u | ((!flag) ? 1u : 0u));
						this.mLanguages.Add(languageData);
						num5 = this.mLanguages.Count - 1;
					}
					array2[i] = num5;
				}
				num4 = this.mLanguages.Count;
				int j;
				j = 0;
				for (int count = this.mTerms.Count; j < count; j++)
				{
					TermData termData;
					termData = this.mTerms[j];
					if (termData.Languages.Length < num4)
					{
						Array.Resize(ref termData.Languages, num4);
						Array.Resize(ref termData.Flags, num4);
					}
				}
				int k;
				k = 1;
				for (int count2 = CSV.Count; k < count2; k++)
				{
					array = CSV[k];
					string Term;
					Term = (string.IsNullOrEmpty(Category) ? array[0] : (Category + "/" + array[0]));
					string text2;
					text2 = null;
					if (Term.EndsWith("]"))
					{
						int num6;
						num6 = Term.LastIndexOf('[');
						if (num6 > 0)
						{
							text2 = Term.Substring(num6 + 1, Term.Length - num6 - 2);
							if (text2 == "touch")
							{
								text2 = "Touch";
							}
							Term = Term.Remove(num6);
						}
					}
					LanguageSourceData.ValidateFullTerm(ref Term);
					if (string.IsNullOrEmpty(Term))
					{
						continue;
					}
					TermData termData2;
					termData2 = this.GetTermData(Term);
					if (termData2 == null)
					{
						termData2 = new TermData();
						termData2.Term = Term;
						termData2.Languages = new string[this.mLanguages.Count];
						termData2.Flags = new byte[this.mLanguages.Count];
						for (int l = 0; l < this.mLanguages.Count; l++)
						{
							termData2.Languages[l] = string.Empty;
						}
						this.mTerms.Add(termData2);
						this.mDictionary.Add(Term, termData2);
					}
					else if (UpdateMode == eSpreadsheetUpdateMode.AddNewTerms)
					{
						continue;
					}
					if (num2 > 0)
					{
						termData2.TermType = LanguageSourceData.GetTermType(array[num2]);
					}
					if (num3 > 0)
					{
						termData2.Description = array[num3];
					}
					for (int m = 0; m < array2.Length && m < array.Length - num; m++)
					{
						if (string.IsNullOrEmpty(array[m + num]))
						{
							continue;
						}
						int num7;
						num7 = array2[m];
						if (num7 >= 0)
						{
							string text3;
							text3 = array[m + num];
							if (text3 == "-")
							{
								text3 = string.Empty;
							}
							else if (text3 == "")
							{
								text3 = null;
							}
							termData2.SetTranslation(num7, text3, text2);
						}
					}
				}
				if (Application.isPlaying)
				{
					this.SaveLanguages(this.HasUnloadedLanguages());
				}
				return string.Empty;
			}
			return "Bad Spreadsheet Format.\nFirst columns should be 'Key', 'Type' and 'Desc'";
		}
	}

	private bool ArrayContains(string MainText, params string[] texts)
	{
		int i;
		i = 0;
		for (int num = texts.Length; i < num; i = unchecked(i + 1))
		{
			if (MainText.IndexOf(texts[i], StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	public static eTermType GetTermType(string type)
	{
		int i;
		i = 0;
		unchecked
		{
			for (int num = 10; i <= num; i++)
			{
				if (string.Equals(((eTermType)i/*cast due to constrained. prefix*/).ToString(), type, StringComparison.OrdinalIgnoreCase))
				{
					return (eTermType)i;
				}
			}
			return eTermType.Text;
		}
	}

	private void Import_Language_from_Cache(int langIndex, string langData, bool useFallback, bool onlyCurrentSpecialization)
	{
		int num;
		num = 0;
		unchecked
		{
			while (num < langData.Length)
			{
				int num2;
				num2 = langData.IndexOf("[i2t]", num);
				if (num2 < 0)
				{
					num2 = langData.Length;
				}
				int num3;
				num3 = langData.IndexOf("=", num);
				if (num3 >= num2)
				{
					break;
				}
				string term;
				term = langData.Substring(num, num3 - num);
				num = num3 + 1;
				TermData termData;
				termData = this.GetTermData(term);
				if (termData != null)
				{
					string text;
					text = null;
					if (num != num2)
					{
						text = langData.Substring(num, num2 - num);
						if (text.StartsWith("[i2fb]"))
						{
							text = (useFallback ? text.Substring(6) : null);
						}
						if (onlyCurrentSpecialization && text != null)
						{
							text = SpecializationManager.GetSpecializedText(text);
						}
					}
					termData.Languages[langIndex] = text;
				}
				num = num2 + 5;
			}
		}
	}

	public static void FreeUnusedLanguages()
	{
		LanguageSourceData languageSourceData;
		languageSourceData = LocalizationManager.Sources[0];
		int languageIndex;
		languageIndex = languageSourceData.GetLanguageIndex(LocalizationManager.CurrentLanguage);
		unchecked
		{
			for (int i = 0; i < languageSourceData.mTerms.Count; i++)
			{
				TermData termData;
				termData = languageSourceData.mTerms[i];
				for (int j = 0; j < termData.Languages.Length; j++)
				{
					if (j != languageIndex)
					{
						termData.Languages[j] = null;
					}
				}
			}
		}
	}

	public void Import_Google_FromCache()
	{
		if (this.GoogleUpdateFrequency == eGoogleUpdateFrequency.Never || !I2Utils.IsPlaying())
		{
			return;
		}
		string sourcePlayerPrefName;
		sourcePlayerPrefName = this.GetSourcePlayerPrefName();
		string text;
		text = PersistentStorage.LoadFile(PersistentStorage.eFileType.Persistent, "I2Source_" + sourcePlayerPrefName + ".loc", logExceptions: false);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (text.StartsWith("[i2e]", StringComparison.Ordinal))
		{
			text = StringObfucator.Decode(text.Substring(5, unchecked(text.Length - 5)));
		}
		bool flag;
		flag = false;
		string text2;
		text2 = this.Google_LastUpdatedVersion;
		if (PersistentStorage.HasSetting("I2SourceVersion_" + sourcePlayerPrefName))
		{
			text2 = PersistentStorage.GetSetting_String("I2SourceVersion_" + sourcePlayerPrefName, this.Google_LastUpdatedVersion);
			flag = this.IsNewerVersion(this.Google_LastUpdatedVersion, text2);
		}
		if (!flag)
		{
			PersistentStorage.DeleteFile(PersistentStorage.eFileType.Persistent, "I2Source_" + sourcePlayerPrefName + ".loc", logExceptions: false);
			PersistentStorage.DeleteSetting("I2SourceVersion_" + sourcePlayerPrefName);
			return;
		}
		if (text2.Length > 19)
		{
			text2 = string.Empty;
		}
		this.Google_LastUpdatedVersion = text2;
		this.Import_Google_Result(text, eSpreadsheetUpdateMode.Replace);
	}

	private bool IsNewerVersion(string currentVersion, string newVersion)
	{
		if (string.IsNullOrEmpty(newVersion))
		{
			return false;
		}
		if (string.IsNullOrEmpty(currentVersion))
		{
			return true;
		}
		if (!long.TryParse(newVersion, out var result) || !long.TryParse(currentVersion, out var result2))
		{
			return true;
		}
		return result > result2;
	}

	public void Import_Google(bool ForceUpdate, bool justCheck)
	{
		if ((!ForceUpdate && this.GoogleUpdateFrequency == eGoogleUpdateFrequency.Never) || !I2Utils.IsPlaying())
		{
			return;
		}
		eGoogleUpdateFrequency googleUpdateFrequency;
		googleUpdateFrequency = this.GoogleUpdateFrequency;
		string sourcePlayerPrefName;
		sourcePlayerPrefName = this.GetSourcePlayerPrefName();
		if (!ForceUpdate && googleUpdateFrequency != eGoogleUpdateFrequency.Always)
		{
			string setting_String;
			setting_String = PersistentStorage.GetSetting_String("LastGoogleUpdate_" + sourcePlayerPrefName, "");
			try
			{
				if (DateTime.TryParse(setting_String, out var result))
				{
					double totalDays;
					totalDays = (DateTime.Now - result).TotalDays;
					switch (googleUpdateFrequency)
					{
					case eGoogleUpdateFrequency.Daily:
						if (totalDays < 1.0)
						{
							return;
						}
						break;
					case eGoogleUpdateFrequency.Weekly:
						if (totalDays < 8.0)
						{
							return;
						}
						break;
					case eGoogleUpdateFrequency.Monthly:
						if (totalDays < 31.0)
						{
							return;
						}
						break;
					case eGoogleUpdateFrequency.OnlyOnce:
						return;
					case eGoogleUpdateFrequency.EveryOtherDay:
						if (totalDays < 2.0)
						{
							return;
						}
						break;
					}
				}
			}
			catch (Exception)
			{
			}
		}
		PersistentStorage.SetSetting_String("LastGoogleUpdate_" + sourcePlayerPrefName, DateTime.Now.ToString());
		CoroutineManager.Start(this.Import_Google_Coroutine(justCheck));
	}

	private string GetSourcePlayerPrefName()
	{
		if (this.owner == null)
		{
			return null;
		}
		string text;
		text = (this.owner as global::UnityEngine.Object).name;
		if (!string.IsNullOrEmpty(this.Google_SpreadsheetKey))
		{
			text += this.Google_SpreadsheetKey;
		}
		if (Array.IndexOf(LocalizationManager.GlobalSources, (this.owner as global::UnityEngine.Object).name) >= 0)
		{
			return text;
		}
		return SceneManager.GetActiveScene().name + "_" + text;
	}

	private IEnumerator Import_Google_Coroutine(bool JustCheck)
	{
		UnityWebRequest www;
		www = this.Import_Google_CreateWWWcall(ForceUpdate: false, JustCheck);
		if (www == null)
		{
			yield break;
		}
		while (!www.isDone)
		{
			yield return null;
		}
		if (string.IsNullOrEmpty(www.error))
		{
			byte[] data;
			data = www.downloadHandler.data;
			string text;
			text = Encoding.UTF8.GetString(data, 0, data.Length);
			bool flag;
			flag = string.IsNullOrEmpty(text) || text == "\"\"";
			if (JustCheck)
			{
				if (!flag)
				{
					Debug.LogWarning("Spreadsheet is not up-to-date and Google Live Synchronization is enabled\nWhen playing in the device the Spreadsheet will be downloaded and translations may not behave as what you see in the editor.\nTo fix this, Import or Export replace to Google");
					this.GoogleLiveSyncIsUptoDate = false;
				}
				yield break;
			}
			if (!flag)
			{
				this.mDelayedGoogleData = text;
				switch (this.GoogleUpdateSynchronization)
				{
				case eGoogleUpdateSynchronization.AsSoonAsDownloaded:
					this.ApplyDownloadedDataFromGoogle();
					break;
				case eGoogleUpdateSynchronization.OnSceneLoaded:
					SceneManager.sceneLoaded += this.ApplyDownloadedDataOnSceneLoaded;
					break;
				}
				yield break;
			}
		}
		if (this.Event_OnSourceUpdateFromGoogle != null)
		{
			this.Event_OnSourceUpdateFromGoogle(this, ReceivedNewData: false, www.error);
		}
		Debug.Log("Language Source was up-to-date with Google Spreadsheet");
	}

	private void ApplyDownloadedDataOnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SceneManager.sceneLoaded -= this.ApplyDownloadedDataOnSceneLoaded;
		this.ApplyDownloadedDataFromGoogle();
	}

	public void ApplyDownloadedDataFromGoogle()
	{
		if (string.IsNullOrEmpty(this.mDelayedGoogleData))
		{
			return;
		}
		if (string.IsNullOrEmpty(this.Import_Google_Result(this.mDelayedGoogleData, eSpreadsheetUpdateMode.Replace, saveInPlayerPrefs: true)))
		{
			if (this.Event_OnSourceUpdateFromGoogle != null)
			{
				this.Event_OnSourceUpdateFromGoogle(this, ReceivedNewData: true, "");
			}
			LocalizationManager.LocalizeAll(Force: true);
			Debug.Log("Done Google Sync");
		}
		else
		{
			if (this.Event_OnSourceUpdateFromGoogle != null)
			{
				this.Event_OnSourceUpdateFromGoogle(this, ReceivedNewData: false, "");
			}
			Debug.Log("Done Google Sync: source was up-to-date");
		}
	}

	public UnityWebRequest Import_Google_CreateWWWcall(bool ForceUpdate, bool justCheck)
	{
		if (!this.HasGoogleSpreadsheet())
		{
			return null;
		}
		string text;
		text = PersistentStorage.GetSetting_String("I2SourceVersion_" + this.GetSourcePlayerPrefName(), this.Google_LastUpdatedVersion);
		if (text.Length > 19)
		{
			text = string.Empty;
		}
		if (this.IsNewerVersion(text, this.Google_LastUpdatedVersion))
		{
			this.Google_LastUpdatedVersion = text;
		}
		UnityWebRequest unityWebRequest;
		unityWebRequest = UnityWebRequest.Get(string.Format("{0}?key={1}&action=GetLanguageSource&version={2}", LocalizationManager.GetWebServiceURL(this), this.Google_SpreadsheetKey, ForceUpdate ? "0" : this.Google_LastUpdatedVersion));
		I2Utils.SendWebRequest(unityWebRequest);
		return unityWebRequest;
	}

	public bool HasGoogleSpreadsheet()
	{
		if (!string.IsNullOrEmpty(this.Google_WebServiceURL) && !string.IsNullOrEmpty(this.Google_SpreadsheetKey))
		{
			return !string.IsNullOrEmpty(LocalizationManager.GetWebServiceURL(this));
		}
		return false;
	}

	public string Import_Google_Result(string JsonString, eSpreadsheetUpdateMode UpdateMode, bool saveInPlayerPrefs = false)
	{
		unchecked
		{
			try
			{
				string empty;
				empty = string.Empty;
				if (string.IsNullOrEmpty(JsonString) || JsonString == "\"\"")
				{
					return empty;
				}
				int num;
				num = JsonString.IndexOf("version=", StringComparison.Ordinal);
				int num2;
				num2 = JsonString.IndexOf("script_version=", StringComparison.Ordinal);
				if (num < 0 || num2 < 0)
				{
					return "Invalid Response from Google, Most likely the WebService needs to be updated";
				}
				num += "version=".Length;
				num2 += "script_version=".Length;
				string text;
				text = JsonString.Substring(num, JsonString.IndexOf(",", num, StringComparison.Ordinal) - num);
				int num3;
				num3 = int.Parse(JsonString.Substring(num2, JsonString.IndexOf(",", num2, StringComparison.Ordinal) - num2));
				if (text.Length > 19)
				{
					text = string.Empty;
				}
				if (num3 != LocalizationManager.GetRequiredWebServiceVersion())
				{
					return "The current Google WebService is not supported.\nPlease, delete the WebService from the Google Drive and Install the latest version.";
				}
				if (saveInPlayerPrefs && !this.IsNewerVersion(this.Google_LastUpdatedVersion, text))
				{
					return "LanguageSource is up-to-date";
				}
				if (saveInPlayerPrefs)
				{
					string sourcePlayerPrefName;
					sourcePlayerPrefName = this.GetSourcePlayerPrefName();
					PersistentStorage.SaveFile(PersistentStorage.eFileType.Persistent, "I2Source_" + sourcePlayerPrefName + ".loc", "[i2e]" + StringObfucator.Encode(JsonString));
					PersistentStorage.SetSetting_String("I2SourceVersion_" + sourcePlayerPrefName, text);
					PersistentStorage.ForceSaveSettings();
				}
				this.Google_LastUpdatedVersion = text;
				if (UpdateMode == eSpreadsheetUpdateMode.Replace)
				{
					this.ClearAllData();
				}
				int num4;
				num4 = JsonString.IndexOf("[i2category]", StringComparison.Ordinal);
				while (num4 > 0)
				{
					num4 += "[i2category]".Length;
					int num5;
					num5 = JsonString.IndexOf("[/i2category]", num4, StringComparison.Ordinal);
					string category;
					category = JsonString.Substring(num4, num5 - num4);
					num5 += "[/i2category]".Length;
					int num6;
					num6 = JsonString.IndexOf("[/i2csv]", num5, StringComparison.Ordinal);
					string i2CSVstring;
					i2CSVstring = JsonString.Substring(num5, num6 - num5);
					num4 = JsonString.IndexOf("[i2category]", num6, StringComparison.Ordinal);
					this.Import_I2CSV(category, i2CSVstring, UpdateMode);
					if (UpdateMode == eSpreadsheetUpdateMode.Replace)
					{
						UpdateMode = eSpreadsheetUpdateMode.Merge;
					}
				}
				this.GoogleLiveSyncIsUptoDate = true;
				if (I2Utils.IsPlaying())
				{
					this.SaveLanguages(unloadAll: true);
				}
				if (!string.IsNullOrEmpty(empty))
				{
					this.Editor_SetDirty();
				}
				return empty;
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex);
				return ex.ToString();
			}
		}
	}

	public int GetLanguageIndex(string language, bool AllowDiscartingRegion = true, bool SkipDisabled = true)
	{
		int i;
		i = 0;
		unchecked
		{
			for (int count = this.mLanguages.Count; i < count; i++)
			{
				if ((!SkipDisabled || this.mLanguages[i].IsEnabled()) && string.Compare(this.mLanguages[i].Name, language, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			if (AllowDiscartingRegion)
			{
				int num;
				num = -1;
				int num2;
				num2 = 0;
				int j;
				j = 0;
				for (int count2 = this.mLanguages.Count; j < count2; j++)
				{
					if (!SkipDisabled || this.mLanguages[j].IsEnabled())
					{
						int commonWordInLanguageNames;
						commonWordInLanguageNames = LanguageSourceData.GetCommonWordInLanguageNames(this.mLanguages[j].Name, language);
						if (commonWordInLanguageNames > num2)
						{
							num2 = commonWordInLanguageNames;
							num = j;
						}
					}
				}
				if (num >= 0)
				{
					return num;
				}
			}
			return -1;
		}
	}

	public LanguageData GetLanguageData(string language, bool AllowDiscartingRegion = true)
	{
		int languageIndex;
		languageIndex = this.GetLanguageIndex(language, AllowDiscartingRegion, SkipDisabled: false);
		if (languageIndex >= 0)
		{
			return this.mLanguages[languageIndex];
		}
		return null;
	}

	public bool IsCurrentLanguage(int languageIndex)
	{
		return LocalizationManager.CurrentLanguage == this.mLanguages[languageIndex].Name;
	}

	public int GetLanguageIndexFromCode(string Code, bool exactMatch = true, bool ignoreDisabled = false)
	{
		int i;
		i = 0;
		unchecked
		{
			for (int count = this.mLanguages.Count; i < count; i++)
			{
				if ((!ignoreDisabled || this.mLanguages[i].IsEnabled()) && string.Compare(this.mLanguages[i].Code, Code, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			if (!exactMatch)
			{
				int j;
				j = 0;
				for (int count2 = this.mLanguages.Count; j < count2; j++)
				{
					if ((!ignoreDisabled || this.mLanguages[j].IsEnabled()) && string.Compare(this.mLanguages[j].Code, 0, Code, 0, 2, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return j;
					}
				}
			}
			return -1;
		}
	}

	public static int GetCommonWordInLanguageNames(string Language1, string Language2)
	{
		if (string.IsNullOrEmpty(Language1) || string.IsNullOrEmpty(Language2))
		{
			return 0;
		}
		char[] separator;
		separator = "( )-/\\".ToCharArray();
		string[] array;
		array = Language1.ToLower().Split(separator);
		string[] array2;
		array2 = Language2.ToLower().Split(separator);
		int num;
		num = 0;
		string[] array3;
		array3 = array;
		unchecked
		{
			foreach (string value in array3)
			{
				if (!string.IsNullOrEmpty(value) && Enumerable.Contains(array2, value))
				{
					num++;
				}
			}
			array3 = array2;
			foreach (string value2 in array3)
			{
				if (!string.IsNullOrEmpty(value2) && Enumerable.Contains(array, value2))
				{
					num++;
				}
			}
			return num;
		}
	}

	public static bool AreTheSameLanguage(string Language1, string Language2)
	{
		Language1 = LanguageSourceData.GetLanguageWithoutRegion(Language1);
		Language2 = LanguageSourceData.GetLanguageWithoutRegion(Language2);
		return string.Compare(Language1, Language2, StringComparison.OrdinalIgnoreCase) == 0;
	}

	public static string GetLanguageWithoutRegion(string Language)
	{
		int num;
		num = Language.IndexOfAny("(/\\[,{".ToCharArray());
		if (num < 0)
		{
			return Language;
		}
		return Language.Substring(0, num).Trim();
	}

	public void AddLanguage(string LanguageName)
	{
		this.AddLanguage(LanguageName, GoogleLanguages.GetLanguageCode(LanguageName));
	}

	public void AddLanguage(string LanguageName, string LanguageCode)
	{
		if (this.GetLanguageIndex(LanguageName, AllowDiscartingRegion: false) < 0)
		{
			LanguageData languageData;
			languageData = new LanguageData();
			languageData.Name = LanguageName;
			languageData.Code = LanguageCode;
			this.mLanguages.Add(languageData);
			int count;
			count = this.mLanguages.Count;
			int i;
			i = 0;
			for (int count2 = this.mTerms.Count; i < count2; i = unchecked(i + 1))
			{
				Array.Resize(ref this.mTerms[i].Languages, count);
				Array.Resize(ref this.mTerms[i].Flags, count);
			}
			this.Editor_SetDirty();
		}
	}

	public void RemoveLanguage(string LanguageName)
	{
		int languageIndex;
		languageIndex = this.GetLanguageIndex(LanguageName, AllowDiscartingRegion: false, SkipDisabled: false);
		if (languageIndex < 0)
		{
			return;
		}
		int count;
		count = this.mLanguages.Count;
		int i;
		i = 0;
		unchecked
		{
			for (int count2 = this.mTerms.Count; i < count2; i++)
			{
				for (int j = languageIndex + 1; j < count; j++)
				{
					this.mTerms[i].Languages[j - 1] = this.mTerms[i].Languages[j];
					this.mTerms[i].Flags[j - 1] = this.mTerms[i].Flags[j];
				}
				Array.Resize(ref this.mTerms[i].Languages, count - 1);
				Array.Resize(ref this.mTerms[i].Flags, count - 1);
			}
			this.mLanguages.RemoveAt(languageIndex);
			this.Editor_SetDirty();
		}
	}

	public List<string> GetLanguages(bool skipDisabled = true)
	{
		List<string> list;
		list = new List<string>();
		int i;
		i = 0;
		for (int count = this.mLanguages.Count; i < count; i = unchecked(i + 1))
		{
			if (!skipDisabled || this.mLanguages[i].IsEnabled())
			{
				list.Add(this.mLanguages[i].Name);
			}
		}
		return list;
	}

	public List<string> GetLanguagesCode(bool allowRegions = true, bool skipDisabled = true)
	{
		List<string> list;
		list = new List<string>();
		int i;
		i = 0;
		for (int count = this.mLanguages.Count; i < count; i = unchecked(i + 1))
		{
			if (!skipDisabled || this.mLanguages[i].IsEnabled())
			{
				string text;
				text = this.mLanguages[i].Code;
				if (!allowRegions && text != null && text.Length > 2)
				{
					text = text.Substring(0, 2);
				}
				if (!string.IsNullOrEmpty(text) && !list.Contains(text))
				{
					list.Add(text);
				}
			}
		}
		return list;
	}

	public bool IsLanguageEnabled(string Language)
	{
		int languageIndex;
		languageIndex = this.GetLanguageIndex(Language, AllowDiscartingRegion: false);
		if (languageIndex >= 0)
		{
			return this.mLanguages[languageIndex].IsEnabled();
		}
		return false;
	}

	public void EnableLanguage(string Language, bool bEnabled)
	{
		int languageIndex;
		languageIndex = this.GetLanguageIndex(Language, AllowDiscartingRegion: false, SkipDisabled: false);
		if (languageIndex >= 0)
		{
			this.mLanguages[languageIndex].SetEnabled(bEnabled);
		}
	}

	public bool AllowUnloadingLanguages()
	{
		return this._AllowUnloadingLanguages != eAllowUnloadLanguages.Never;
	}

	private string GetSavedLanguageFileName(int languageIndex)
	{
		if (languageIndex < 0)
		{
			return null;
		}
		return "LangSource_" + this.GetSourcePlayerPrefName() + "_" + this.mLanguages[languageIndex].Name + ".loc";
	}

	public void LoadLanguage(int languageIndex, bool UnloadOtherLanguages, bool useFallback, bool onlyCurrentSpecialization, bool forceLoad)
	{
		if (!this.AllowUnloadingLanguages() || !PersistentStorage.CanAccessFiles())
		{
			return;
		}
		if (languageIndex >= 0 && (forceLoad || !this.mLanguages[languageIndex].IsLoaded()))
		{
			string text;
			text = PersistentStorage.LoadFile(PersistentStorage.eFileType.Temporal, this.GetSavedLanguageFileName(languageIndex), logExceptions: false);
			if (!string.IsNullOrEmpty(text))
			{
				this.Import_Language_from_Cache(languageIndex, text, useFallback, onlyCurrentSpecialization);
				this.mLanguages[languageIndex].SetLoaded(loaded: true);
			}
		}
		if (!UnloadOtherLanguages || !I2Utils.IsPlaying())
		{
			return;
		}
		for (int i = 0; i < this.mLanguages.Count; i = unchecked(i + 1))
		{
			if (i != languageIndex)
			{
				this.UnloadLanguage(i);
			}
		}
	}

	public void LoadAllLanguages(bool forceLoad = false)
	{
		for (int i = 0; i < this.mLanguages.Count; i = unchecked(i + 1))
		{
			this.LoadLanguage(i, UnloadOtherLanguages: false, useFallback: false, onlyCurrentSpecialization: false, forceLoad);
		}
	}

	public void UnloadLanguage(int languageIndex)
	{
		if (!this.AllowUnloadingLanguages() || !PersistentStorage.CanAccessFiles() || !I2Utils.IsPlaying() || !this.mLanguages[languageIndex].IsLoaded() || !this.mLanguages[languageIndex].CanBeUnloaded() || this.IsCurrentLanguage(languageIndex) || !PersistentStorage.HasFile(PersistentStorage.eFileType.Temporal, this.GetSavedLanguageFileName(languageIndex)))
		{
			return;
		}
		foreach (TermData mTerm in this.mTerms)
		{
			mTerm.Languages[languageIndex] = null;
		}
		this.mLanguages[languageIndex].SetLoaded(loaded: false);
	}

	public void SaveLanguages(bool unloadAll, PersistentStorage.eFileType fileLocation = PersistentStorage.eFileType.Temporal)
	{
		if (!this.AllowUnloadingLanguages() || !PersistentStorage.CanAccessFiles())
		{
			return;
		}
		unchecked
		{
			for (int i = 0; i < this.mLanguages.Count; i++)
			{
				string text;
				text = this.Export_Language_to_Cache(i, this.IsCurrentLanguage(i));
				if (!string.IsNullOrEmpty(text))
				{
					PersistentStorage.SaveFile(PersistentStorage.eFileType.Temporal, this.GetSavedLanguageFileName(i), text);
				}
			}
			if (!unloadAll)
			{
				return;
			}
			for (int j = 0; j < this.mLanguages.Count; j++)
			{
				if (unloadAll && !this.IsCurrentLanguage(j))
				{
					this.UnloadLanguage(j);
				}
			}
		}
	}

	public bool HasUnloadedLanguages()
	{
		for (int i = 0; i < this.mLanguages.Count; i = unchecked(i + 1))
		{
			if (!this.mLanguages[i].IsLoaded())
			{
				return true;
			}
		}
		return false;
	}

	public List<string> GetCategories(bool OnlyMainCategory = false, List<string> Categories = null)
	{
		if (Categories == null)
		{
			Categories = new List<string>();
		}
		foreach (TermData mTerm in this.mTerms)
		{
			string categoryFromFullTerm;
			categoryFromFullTerm = LanguageSourceData.GetCategoryFromFullTerm(mTerm.Term, OnlyMainCategory);
			if (!Categories.Contains(categoryFromFullTerm))
			{
				Categories.Add(categoryFromFullTerm);
			}
		}
		Categories.Sort();
		return Categories;
	}

	public static string GetKeyFromFullTerm(string FullTerm, bool OnlyMainCategory = false)
	{
		int num;
		num = (OnlyMainCategory ? FullTerm.IndexOfAny(LanguageSourceData.CategorySeparators) : FullTerm.LastIndexOfAny(LanguageSourceData.CategorySeparators));
		if (num >= 0)
		{
			return FullTerm.Substring(unchecked(num + 1));
		}
		return FullTerm;
	}

	public static string GetCategoryFromFullTerm(string FullTerm, bool OnlyMainCategory = false)
	{
		int num;
		num = (OnlyMainCategory ? FullTerm.IndexOfAny(LanguageSourceData.CategorySeparators) : FullTerm.LastIndexOfAny(LanguageSourceData.CategorySeparators));
		if (num >= 0)
		{
			return FullTerm.Substring(0, num);
		}
		return LanguageSourceData.EmptyCategory;
	}

	public static void DeserializeFullTerm(string FullTerm, out string Key, out string Category, bool OnlyMainCategory = false)
	{
		int num;
		num = (OnlyMainCategory ? FullTerm.IndexOfAny(LanguageSourceData.CategorySeparators) : FullTerm.LastIndexOfAny(LanguageSourceData.CategorySeparators));
		if (num < 0)
		{
			Category = LanguageSourceData.EmptyCategory;
			Key = FullTerm;
		}
		else
		{
			Category = FullTerm.Substring(0, num);
			Key = FullTerm.Substring(unchecked(num + 1));
		}
	}

	public void UpdateDictionary(bool force = false)
	{
		if (force || this.mDictionary == null || this.mDictionary.Count != this.mTerms.Count)
		{
			StringComparer stringComparer;
			stringComparer = (this.CaseInsensitiveTerms ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
			if (this.mDictionary.Comparer != stringComparer)
			{
				this.mDictionary = new Dictionary<string, TermData>(stringComparer);
			}
			else
			{
				this.mDictionary.Clear();
			}
			int i;
			i = 0;
			for (int count = this.mTerms.Count; i < count; i = unchecked(i + 1))
			{
				TermData termData;
				termData = this.mTerms[i];
				LanguageSourceData.ValidateFullTerm(ref termData.Term);
				this.mDictionary[termData.Term] = this.mTerms[i];
				this.mTerms[i].Validate();
			}
			if (I2Utils.IsPlaying())
			{
				this.SaveLanguages(unloadAll: true);
			}
		}
	}

	public string GetTranslation(string term, string overrideLanguage = null, string overrideSpecialization = null, bool skipDisabled = false, bool allowCategoryMistmatch = false)
	{
		if (this.TryGetTranslation(term, out var Translation, overrideLanguage, overrideSpecialization, skipDisabled, allowCategoryMistmatch))
		{
			return Translation;
		}
		return string.Empty;
	}

	public bool TryGetTranslation(string term, out string Translation, string overrideLanguage = null, string overrideSpecialization = null, bool skipDisabled = false, bool allowCategoryMistmatch = false)
	{
		int languageIndex;
		languageIndex = this.GetLanguageIndex((overrideLanguage == null) ? LocalizationManager.CurrentLanguage : overrideLanguage, AllowDiscartingRegion: true, SkipDisabled: false);
		if (languageIndex >= 0 && (!skipDisabled || this.mLanguages[languageIndex].IsEnabled()))
		{
			TermData termData;
			termData = this.GetTermData(term, allowCategoryMistmatch);
			if (termData != null)
			{
				Translation = termData.GetTranslation(languageIndex, overrideSpecialization, editMode: true);
				if (Translation == "---")
				{
					Translation = string.Empty;
					return true;
				}
				if (!string.IsNullOrEmpty(Translation))
				{
					return true;
				}
				Translation = null;
			}
			if (this.OnMissingTranslation == MissingTranslationAction.ShowWarning)
			{
				Translation = $"<!-Missing Translation [{term}]-!>";
				return true;
			}
			if (this.OnMissingTranslation == MissingTranslationAction.Fallback && termData != null)
			{
				return this.TryGetFallbackTranslation(termData, out Translation, languageIndex, overrideSpecialization, skipDisabled);
			}
			if (this.OnMissingTranslation == MissingTranslationAction.Empty)
			{
				Translation = string.Empty;
				return true;
			}
			if (this.OnMissingTranslation == MissingTranslationAction.ShowTerm)
			{
				Translation = term;
				return true;
			}
		}
		Translation = null;
		return false;
	}

	private bool TryGetFallbackTranslation(TermData termData, out string Translation, int langIndex, string overrideSpecialization = null, bool skipDisabled = false)
	{
		string text;
		text = this.mLanguages[langIndex].Code;
		unchecked
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (text.Contains('-'))
				{
					text = text.Substring(0, text.IndexOf('-'));
				}
				for (int i = 0; i < this.mLanguages.Count; i++)
				{
					if (i != langIndex && this.mLanguages[i].Code.StartsWith(text) && (!skipDisabled || this.mLanguages[i].IsEnabled()))
					{
						Translation = termData.GetTranslation(i, overrideSpecialization, editMode: true);
						if (!string.IsNullOrEmpty(Translation))
						{
							return true;
						}
					}
				}
			}
			for (int j = 0; j < this.mLanguages.Count; j++)
			{
				if (j != langIndex && (!skipDisabled || this.mLanguages[j].IsEnabled()) && (text == null || !this.mLanguages[j].Code.StartsWith(text)))
				{
					Translation = termData.GetTranslation(j, overrideSpecialization, editMode: true);
					if (!string.IsNullOrEmpty(Translation))
					{
						return true;
					}
				}
			}
			Translation = null;
			return false;
		}
	}

	public TermData AddTerm(string term)
	{
		return this.AddTerm(term, eTermType.Text);
	}

	public TermData GetTermData(string term, bool allowCategoryMistmatch = false)
	{
		if (string.IsNullOrEmpty(term))
		{
			return null;
		}
		if (this.mDictionary.Count == 0)
		{
			this.UpdateDictionary();
		}
		if (this.mDictionary.TryGetValue(term, out var value))
		{
			return value;
		}
		TermData termData;
		termData = null;
		if (allowCategoryMistmatch)
		{
			string keyFromFullTerm;
			keyFromFullTerm = LanguageSourceData.GetKeyFromFullTerm(term);
			foreach (KeyValuePair<string, TermData> item in this.mDictionary)
			{
				if (item.Value.IsTerm(keyFromFullTerm, allowCategoryMistmatch: true))
				{
					if (termData != null)
					{
						return null;
					}
					termData = item.Value;
				}
			}
		}
		return termData;
	}

	public bool ContainsTerm(string term)
	{
		return this.GetTermData(term) != null;
	}

	public List<string> GetTermsList(string Category = null)
	{
		if (this.mDictionary.Count != this.mTerms.Count)
		{
			this.UpdateDictionary();
		}
		if (string.IsNullOrEmpty(Category))
		{
			return new List<string>(this.mDictionary.Keys);
		}
		List<string> list;
		list = new List<string>();
		for (int i = 0; i < this.mTerms.Count; i = unchecked(i + 1))
		{
			TermData termData;
			termData = this.mTerms[i];
			if (LanguageSourceData.GetCategoryFromFullTerm(termData.Term) == Category)
			{
				list.Add(termData.Term);
			}
		}
		return list;
	}

	public TermData AddTerm(string NewTerm, eTermType termType, bool SaveSource = true)
	{
		LanguageSourceData.ValidateFullTerm(ref NewTerm);
		NewTerm = NewTerm.Trim();
		if (this.mLanguages.Count == 0)
		{
			this.AddLanguage("English", "en");
		}
		TermData termData;
		termData = this.GetTermData(NewTerm);
		if (termData == null)
		{
			termData = new TermData();
			termData.Term = NewTerm;
			termData.TermType = termType;
			termData.Languages = new string[this.mLanguages.Count];
			termData.Flags = new byte[this.mLanguages.Count];
			this.mTerms.Add(termData);
			this.mDictionary.Add(NewTerm, termData);
		}
		return termData;
	}

	public void RemoveTerm(string term)
	{
		int i;
		i = 0;
		for (int count = this.mTerms.Count; i < count; i = unchecked(i + 1))
		{
			if (this.mTerms[i].Term == term)
			{
				this.mTerms.RemoveAt(i);
				this.mDictionary.Remove(term);
				break;
			}
		}
	}

	public static void ValidateFullTerm(ref string Term)
	{
		Term = Term.Replace('\\', '/');
		Term = Term.Trim();
		if (Term.StartsWith(LanguageSourceData.EmptyCategory, StringComparison.Ordinal) && Term.Length > LanguageSourceData.EmptyCategory.Length && Term[LanguageSourceData.EmptyCategory.Length] == '/')
		{
			Term = Term.Substring(unchecked(LanguageSourceData.EmptyCategory.Length + 1));
		}
		Term = I2Utils.GetValidTermName(Term, allowCategory: true);
	}

	static LanguageSourceData()
	{
		LanguageSourceData.EmptyCategory = "Default";
		LanguageSourceData.CategorySeparators = "/\\".ToCharArray();
	}
}
