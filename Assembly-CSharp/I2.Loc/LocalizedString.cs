using System;

namespace I2.Loc;

[Serializable]
public struct LocalizedString
{
	public string mTerm;

	public bool mRTL_IgnoreArabicFix;

	public int mRTL_MaxLineLength;

	public bool mRTL_ConvertNumbers;

	public bool m_DontLocalizeParameters;

	public static implicit operator string(LocalizedString s)
	{
		return s.ToString();
	}

	public static implicit operator LocalizedString(string term)
	{
		return new LocalizedString
		{
			mTerm = term
		};
	}

	public LocalizedString(LocalizedString str)
	{
		this.mTerm = str.mTerm;
		this.mRTL_IgnoreArabicFix = str.mRTL_IgnoreArabicFix;
		this.mRTL_MaxLineLength = str.mRTL_MaxLineLength;
		this.mRTL_ConvertNumbers = str.mRTL_ConvertNumbers;
		this.m_DontLocalizeParameters = str.m_DontLocalizeParameters;
	}

	public override string ToString()
	{
		string translation;
		translation = LocalizationManager.GetTranslation(this.mTerm, !this.mRTL_IgnoreArabicFix, this.mRTL_MaxLineLength, !this.mRTL_ConvertNumbers, applyParameters: true);
		LocalizationManager.ApplyLocalizationParams(ref translation, !this.m_DontLocalizeParameters);
		return translation;
	}
}
