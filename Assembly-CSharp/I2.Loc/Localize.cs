using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace I2.Loc;

[AddComponentMenu("I2/Localization/I2 Localize")]
public class Localize : MonoBehaviour
{
	public enum TermModification
	{
		DontModify = 0,
		ToUpper = 1,
		ToLower = 2,
		ToUpperFirst = 3,
		ToTitle = 4
	}

	public string mTerm = string.Empty;

	public string mTermSecondary = string.Empty;

	[NonSerialized]
	public string FinalTerm;

	[NonSerialized]
	public string FinalSecondaryTerm;

	public TermModification PrimaryTermModifier;

	public TermModification SecondaryTermModifier;

	public string TermPrefix;

	public string TermSuffix;

	public bool LocalizeOnAwake = true;

	private string LastLocalizedLanguage;

	public bool IgnoreRTL;

	public int MaxCharactersInRTL;

	public bool IgnoreNumbersInRTL = true;

	public bool CorrectAlignmentForRTL = true;

	public bool AddSpacesToJoinedLanguages;

	public bool AllowLocalizedParameters = true;

	public List<global::UnityEngine.Object> TranslatedObjects = new List<global::UnityEngine.Object>();

	[NonSerialized]
	public Dictionary<string, global::UnityEngine.Object> mAssetDictionary = new Dictionary<string, global::UnityEngine.Object>(StringComparer.Ordinal);

	public UnityEvent LocalizeEvent = new UnityEvent();

	public static string MainTranslation;

	public static string SecondaryTranslation;

	public static string CallBackTerm;

	public static string CallBackSecondaryTerm;

	public static Localize CurrentLocalizeComponent;

	public bool AlwaysForceLocalize;

	[SerializeField]
	public EventCallback LocalizeCallBack = new EventCallback();

	public bool mGUI_ShowReferences;

	public bool mGUI_ShowTems = true;

	public bool mGUI_ShowCallback;

	public ILocalizeTarget mLocalizeTarget;

	public string mLocalizeTargetName;

	public string Term
	{
		get
		{
			return this.mTerm;
		}
		set
		{
			this.SetTerm(value);
		}
	}

	public string SecondaryTerm
	{
		get
		{
			return this.mTermSecondary;
		}
		set
		{
			this.SetTerm(null, value);
		}
	}

	private void Awake()
	{
		this.UpdateAssetDictionary();
		this.FindTarget();
		if (this.LocalizeOnAwake)
		{
			this.OnLocalize();
		}
	}

	private void OnEnable()
	{
		this.OnLocalize();
	}

	public bool HasCallback()
	{
		if (this.LocalizeCallBack.HasCallback())
		{
			return true;
		}
		return this.LocalizeEvent.GetPersistentEventCount() > 0;
	}

	public void OnLocalize(bool Force = false)
	{
		if ((!Force && (!base.enabled || base.gameObject == null || !base.gameObject.activeInHierarchy)) || string.IsNullOrEmpty(LocalizationManager.CurrentLanguage) || (!this.AlwaysForceLocalize && !Force && !this.HasCallback() && this.LastLocalizedLanguage == LocalizationManager.CurrentLanguage))
		{
			return;
		}
		this.LastLocalizedLanguage = LocalizationManager.CurrentLanguage;
		if (string.IsNullOrEmpty(this.FinalTerm) || string.IsNullOrEmpty(this.FinalSecondaryTerm))
		{
			this.GetFinalTerms(out this.FinalTerm, out this.FinalSecondaryTerm);
		}
		bool flag;
		flag = I2Utils.IsPlaying() && this.HasCallback();
		if (!flag && string.IsNullOrEmpty(this.FinalTerm) && string.IsNullOrEmpty(this.FinalSecondaryTerm))
		{
			return;
		}
		Localize.CallBackTerm = this.FinalTerm;
		Localize.CallBackSecondaryTerm = this.FinalSecondaryTerm;
		Localize.MainTranslation = ((string.IsNullOrEmpty(this.FinalTerm) || this.FinalTerm == "-") ? null : LocalizationManager.GetTranslation(this.FinalTerm, FixForRTL: false));
		Localize.SecondaryTranslation = ((string.IsNullOrEmpty(this.FinalSecondaryTerm) || this.FinalSecondaryTerm == "-") ? null : LocalizationManager.GetTranslation(this.FinalSecondaryTerm, FixForRTL: false));
		if (!flag && string.IsNullOrEmpty(this.FinalTerm) && string.IsNullOrEmpty(Localize.SecondaryTranslation))
		{
			return;
		}
		Localize.CurrentLocalizeComponent = this;
		this.LocalizeCallBack.Execute(this);
		this.LocalizeEvent.Invoke();
		LocalizationManager.ApplyLocalizationParams(ref Localize.MainTranslation, base.gameObject, this.AllowLocalizedParameters);
		if (!this.FindTarget())
		{
			return;
		}
		bool flag2;
		flag2 = LocalizationManager.IsRight2Left && !this.IgnoreRTL;
		if (Localize.MainTranslation != null)
		{
			switch (this.PrimaryTermModifier)
			{
			case TermModification.ToUpper:
				Localize.MainTranslation = Localize.MainTranslation.ToUpper();
				break;
			case TermModification.ToLower:
				Localize.MainTranslation = Localize.MainTranslation.ToLower();
				break;
			case TermModification.ToUpperFirst:
				Localize.MainTranslation = GoogleTranslation.UppercaseFirst(Localize.MainTranslation);
				break;
			case TermModification.ToTitle:
				Localize.MainTranslation = GoogleTranslation.TitleCase(Localize.MainTranslation);
				break;
			}
			if (!string.IsNullOrEmpty(this.TermPrefix))
			{
				Localize.MainTranslation = (flag2 ? (Localize.MainTranslation + this.TermPrefix) : (this.TermPrefix + Localize.MainTranslation));
			}
			if (!string.IsNullOrEmpty(this.TermSuffix))
			{
				Localize.MainTranslation = (flag2 ? (this.TermSuffix + Localize.MainTranslation) : (Localize.MainTranslation + this.TermSuffix));
			}
			if (this.AddSpacesToJoinedLanguages && LocalizationManager.HasJoinedWords && !string.IsNullOrEmpty(Localize.MainTranslation))
			{
				StringBuilder stringBuilder;
				stringBuilder = new StringBuilder();
				stringBuilder.Append(Localize.MainTranslation[0]);
				int i;
				i = 1;
				for (int length = Localize.MainTranslation.Length; i < length; i = unchecked(i + 1))
				{
					stringBuilder.Append(' ');
					stringBuilder.Append(Localize.MainTranslation[i]);
				}
				Localize.MainTranslation = stringBuilder.ToString();
			}
			if (flag2 && this.mLocalizeTarget.AllowMainTermToBeRTL() && !string.IsNullOrEmpty(Localize.MainTranslation))
			{
				Localize.MainTranslation = LocalizationManager.ApplyRTLfix(Localize.MainTranslation, this.MaxCharactersInRTL, this.IgnoreNumbersInRTL);
			}
		}
		if (Localize.SecondaryTranslation != null)
		{
			switch (this.SecondaryTermModifier)
			{
			case TermModification.ToUpper:
				Localize.SecondaryTranslation = Localize.SecondaryTranslation.ToUpper();
				break;
			case TermModification.ToLower:
				Localize.SecondaryTranslation = Localize.SecondaryTranslation.ToLower();
				break;
			case TermModification.ToUpperFirst:
				Localize.SecondaryTranslation = GoogleTranslation.UppercaseFirst(Localize.SecondaryTranslation);
				break;
			case TermModification.ToTitle:
				Localize.SecondaryTranslation = GoogleTranslation.TitleCase(Localize.SecondaryTranslation);
				break;
			}
			if (flag2 && this.mLocalizeTarget.AllowSecondTermToBeRTL() && !string.IsNullOrEmpty(Localize.SecondaryTranslation))
			{
				Localize.SecondaryTranslation = LocalizationManager.ApplyRTLfix(Localize.SecondaryTranslation);
			}
		}
		if (LocalizationManager.HighlightLocalizedTargets)
		{
			Localize.MainTranslation = "LOC:" + this.FinalTerm;
		}
		this.mLocalizeTarget.DoLocalize(this, Localize.MainTranslation, Localize.SecondaryTranslation);
		Localize.CurrentLocalizeComponent = null;
	}

	public bool FindTarget()
	{
		if (this.mLocalizeTarget != null && this.mLocalizeTarget.IsValid(this))
		{
			return true;
		}
		if (this.mLocalizeTarget != null)
		{
			global::UnityEngine.Object.DestroyImmediate(this.mLocalizeTarget);
			this.mLocalizeTarget = null;
			this.mLocalizeTargetName = null;
		}
		if (!string.IsNullOrEmpty(this.mLocalizeTargetName))
		{
			foreach (ILocalizeTargetDescriptor mLocalizeTarget in LocalizationManager.mLocalizeTargets)
			{
				if (this.mLocalizeTargetName == mLocalizeTarget.GetTargetType().ToString())
				{
					if (mLocalizeTarget.CanLocalize(this))
					{
						this.mLocalizeTarget = mLocalizeTarget.CreateTarget(this);
					}
					if (this.mLocalizeTarget != null)
					{
						return true;
					}
				}
			}
		}
		foreach (ILocalizeTargetDescriptor mLocalizeTarget2 in LocalizationManager.mLocalizeTargets)
		{
			if (mLocalizeTarget2.CanLocalize(this))
			{
				this.mLocalizeTarget = mLocalizeTarget2.CreateTarget(this);
				this.mLocalizeTargetName = mLocalizeTarget2.GetTargetType().ToString();
				if (this.mLocalizeTarget != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void GetFinalTerms(out string primaryTerm, out string secondaryTerm)
	{
		primaryTerm = string.Empty;
		secondaryTerm = string.Empty;
		if (this.FindTarget())
		{
			if (this.mLocalizeTarget != null)
			{
				this.mLocalizeTarget.GetFinalTerms(this, this.mTerm, this.mTermSecondary, out primaryTerm, out secondaryTerm);
				primaryTerm = I2Utils.GetValidTermName(primaryTerm);
			}
			if (!string.IsNullOrEmpty(this.mTerm))
			{
				primaryTerm = this.mTerm;
			}
			if (!string.IsNullOrEmpty(this.mTermSecondary))
			{
				secondaryTerm = this.mTermSecondary;
			}
			if (primaryTerm != null)
			{
				primaryTerm = primaryTerm.Trim();
			}
			if (secondaryTerm != null)
			{
				secondaryTerm = secondaryTerm.Trim();
			}
		}
	}

	public string GetMainTargetsText()
	{
		string primaryTerm;
		primaryTerm = null;
		string secondaryTerm;
		secondaryTerm = null;
		if (this.mLocalizeTarget != null)
		{
			this.mLocalizeTarget.GetFinalTerms(this, null, null, out primaryTerm, out secondaryTerm);
		}
		if (!string.IsNullOrEmpty(primaryTerm))
		{
			return primaryTerm;
		}
		return this.mTerm;
	}

	public void SetFinalTerms(string Main, string Secondary, out string primaryTerm, out string secondaryTerm, bool RemoveNonASCII)
	{
		primaryTerm = (RemoveNonASCII ? I2Utils.GetValidTermName(Main) : Main);
		secondaryTerm = Secondary;
	}

	public void SetTerm(string primary)
	{
		if (!string.IsNullOrEmpty(primary))
		{
			this.FinalTerm = (this.mTerm = primary);
		}
		this.OnLocalize(Force: true);
	}

	public void SetTerm(string primary, string secondary)
	{
		if (!string.IsNullOrEmpty(primary))
		{
			this.FinalTerm = (this.mTerm = primary);
		}
		this.FinalSecondaryTerm = (this.mTermSecondary = secondary);
		this.OnLocalize(Force: true);
	}

	internal T GetSecondaryTranslatedObj<T>(ref string mainTranslation, ref string secondaryTranslation) where T : global::UnityEngine.Object
	{
		this.DeserializeTranslation(mainTranslation, out var value, out var secondary);
		T val;
		val = null;
		if (!string.IsNullOrEmpty(secondary))
		{
			val = this.GetObject<T>(secondary);
			if (val != null)
			{
				mainTranslation = value;
				secondaryTranslation = secondary;
			}
		}
		if (val == null)
		{
			val = this.GetObject<T>(secondaryTranslation);
		}
		return val;
	}

	public void UpdateAssetDictionary()
	{
		this.TranslatedObjects.RemoveAll((global::UnityEngine.Object x) => x == null);
		this.mAssetDictionary = (from o in this.TranslatedObjects.Distinct()
			group o by o.name).ToDictionary((IGrouping<string, global::UnityEngine.Object> g) => g.Key, (IGrouping<string, global::UnityEngine.Object> g) => g.First());
	}

	internal T GetObject<T>(string Translation) where T : global::UnityEngine.Object
	{
		if (string.IsNullOrEmpty(Translation))
		{
			return null;
		}
		return this.GetTranslatedObject<T>(Translation);
	}

	private T GetTranslatedObject<T>(string Translation) where T : global::UnityEngine.Object
	{
		return this.FindTranslatedObject<T>(Translation);
	}

	private void DeserializeTranslation(string translation, out string value, out string secondary)
	{
		unchecked
		{
			if (!string.IsNullOrEmpty(translation) && translation.Length > 1 && translation[0] == '[')
			{
				int num;
				num = translation.IndexOf(']');
				if (num > 0)
				{
					secondary = translation.Substring(1, num - 1);
					value = translation.Substring(num + 1);
					return;
				}
			}
			value = translation;
			secondary = string.Empty;
		}
	}

	public T FindTranslatedObject<T>(string value) where T : global::UnityEngine.Object
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		if (this.mAssetDictionary == null || this.mAssetDictionary.Count != this.TranslatedObjects.Count)
		{
			this.UpdateAssetDictionary();
		}
		foreach (KeyValuePair<string, global::UnityEngine.Object> item in this.mAssetDictionary)
		{
			if (item.Value is T && value.EndsWith(item.Key, StringComparison.OrdinalIgnoreCase) && string.Compare(value, item.Key, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return (T)item.Value;
			}
		}
		T val;
		val = LocalizationManager.FindAsset(value) as T;
		if ((bool)val)
		{
			return val;
		}
		return ResourceManager.pInstance.GetAsset<T>(value);
	}

	public bool HasTranslatedObject(global::UnityEngine.Object Obj)
	{
		if (this.TranslatedObjects.Contains(Obj))
		{
			return true;
		}
		return ResourceManager.pInstance.HasAsset(Obj);
	}

	public void AddTranslatedObject(global::UnityEngine.Object Obj)
	{
		if (!this.TranslatedObjects.Contains(Obj))
		{
			this.TranslatedObjects.Add(Obj);
			this.UpdateAssetDictionary();
		}
	}

	public void SetGlobalLanguage(string Language)
	{
		LocalizationManager.CurrentLanguage = Language;
	}
}
