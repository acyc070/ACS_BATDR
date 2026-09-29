using System;
using System.Collections.Generic;
using UnityEngine;

namespace I2.Loc;

[Serializable]
public class TermData
{
	public string Term = string.Empty;

	public eTermType TermType;

	[NonSerialized]
	public string Description;

	public string[] Languages = new string[0];

	public byte[] Flags = new byte[0];

	[SerializeField]
	private string[] Languages_Touch;

	public string GetTranslation(int idx, string specialization = null, bool editMode = false)
	{
		string text;
		text = this.Languages[idx];
		if (text != null)
		{
			text = SpecializationManager.GetSpecializedText(text, specialization);
			if (!editMode)
			{
				text = text.Replace("[i2nt]", "").Replace("[/i2nt]", "");
			}
		}
		return text;
	}

	public void SetTranslation(int idx, string translation, string specialization = null)
	{
		this.Languages[idx] = SpecializationManager.SetSpecializedText(this.Languages[idx], translation, specialization);
	}

	public void RemoveSpecialization(string specialization)
	{
		for (int i = 0; i < this.Languages.Length; i = unchecked(i + 1))
		{
			this.RemoveSpecialization(i, specialization);
		}
	}

	public void RemoveSpecialization(int idx, string specialization)
	{
		string text;
		text = this.Languages[idx];
		if (!(specialization == "Any") && text.Contains("[i2s_" + specialization + "]"))
		{
			Dictionary<string, string> specializations;
			specializations = SpecializationManager.GetSpecializations(text);
			specializations.Remove(specialization);
			this.Languages[idx] = SpecializationManager.SetSpecializedText(specializations);
		}
	}

	public bool IsAutoTranslated(int idx, bool IsTouch)
	{
		return (this.Flags[idx] & 2) > 0;
	}

	public void Validate()
	{
		int num;
		num = Mathf.Max(this.Languages.Length, this.Flags.Length);
		if (this.Languages.Length != num)
		{
			Array.Resize(ref this.Languages, num);
		}
		if (this.Flags.Length != num)
		{
			Array.Resize(ref this.Flags, num);
		}
		if (this.Languages_Touch == null)
		{
			return;
		}
		for (int i = 0; i < Mathf.Min(this.Languages_Touch.Length, num); i = unchecked(i + 1))
		{
			if (string.IsNullOrEmpty(this.Languages[i]) && !string.IsNullOrEmpty(this.Languages_Touch[i]))
			{
				this.Languages[i] = this.Languages_Touch[i];
				this.Languages_Touch[i] = null;
			}
		}
		this.Languages_Touch = null;
	}

	public bool IsTerm(string name, bool allowCategoryMistmatch)
	{
		if (!allowCategoryMistmatch)
		{
			return name == this.Term;
		}
		return name == LanguageSourceData.GetKeyFromFullTerm(this.Term);
	}

	public bool HasSpecializations()
	{
		for (int i = 0; i < this.Languages.Length; i = unchecked(i + 1))
		{
			if (!string.IsNullOrEmpty(this.Languages[i]) && this.Languages[i].Contains("[i2s_"))
			{
				return true;
			}
		}
		return false;
	}

	public List<string> GetAllSpecializations()
	{
		List<string> list;
		list = new List<string>();
		for (int i = 0; i < this.Languages.Length; i = unchecked(i + 1))
		{
			SpecializationManager.AppendSpecializations(this.Languages[i], list);
		}
		return list;
	}
}
