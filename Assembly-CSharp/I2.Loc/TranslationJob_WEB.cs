using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace I2.Loc;

public class TranslationJob_WEB : TranslationJob_WWW
{
	private Dictionary<string, TranslationQuery> _requests;

	private GoogleTranslation.fnOnTranslationReady _OnTranslationReady;

	public string mErrorMessage;

	private string mCurrentBatch_ToLanguageCode;

	private string mCurrentBatch_FromLanguageCode;

	private List<string> mCurrentBatch_Text;

	private List<KeyValuePair<string, string>> mQueries;

	public TranslationJob_WEB(Dictionary<string, TranslationQuery> requests, GoogleTranslation.fnOnTranslationReady OnTranslationReady)
	{
		this._requests = requests;
		this._OnTranslationReady = OnTranslationReady;
		this.FindAllQueries();
		this.ExecuteNextBatch();
	}

	private void FindAllQueries()
	{
		this.mQueries = new List<KeyValuePair<string, string>>();
		foreach (KeyValuePair<string, TranslationQuery> request in this._requests)
		{
			string[] targetLanguagesCode;
			targetLanguagesCode = request.Value.TargetLanguagesCode;
			foreach (string text in targetLanguagesCode)
			{
				this.mQueries.Add(new KeyValuePair<string, string>(request.Value.OrigText, request.Value.LanguageCode + ":" + text));
			}
		}
		this.mQueries.Sort((KeyValuePair<string, string> a, KeyValuePair<string, string> b) => a.Value.CompareTo(b.Value));
	}

	private void ExecuteNextBatch()
	{
		if (this.mQueries.Count == 0)
		{
			base.mJobState = eJobState.Succeeded;
			return;
		}
		this.mCurrentBatch_Text = new List<string>();
		string text;
		text = null;
		int num;
		num = 200;
		StringBuilder stringBuilder;
		stringBuilder = new StringBuilder();
		int i;
		for (i = 0; i < this.mQueries.Count; i = unchecked(i + 1))
		{
			string key;
			key = this.mQueries[i].Key;
			string value;
			value = this.mQueries[i].Value;
			if (text == null || value == text)
			{
				if (i != 0)
				{
					stringBuilder.Append("|||");
				}
				stringBuilder.Append(key);
				this.mCurrentBatch_Text.Add(key);
				text = value;
			}
			if (stringBuilder.Length > num)
			{
				break;
			}
		}
		this.mQueries.RemoveRange(0, i);
		string[] array;
		array = text.Split(':');
		this.mCurrentBatch_FromLanguageCode = array[0];
		this.mCurrentBatch_ToLanguageCode = array[1];
		string text2;
		text2 = $"http://www.google.com/translate_t?hl=en&vi=c&ie=UTF8&oe=UTF8&submit=Translate&langpair={this.mCurrentBatch_FromLanguageCode}|{this.mCurrentBatch_ToLanguageCode}&text={Uri.EscapeUriString(stringBuilder.ToString())}";
		Debug.Log(text2);
		base.www = UnityWebRequest.Get(text2);
		I2Utils.SendWebRequest(base.www);
	}

	public override eJobState GetState()
	{
		if (base.www != null && base.www.isDone)
		{
			this.ProcessResult(base.www.downloadHandler.data, base.www.error);
			base.www.Dispose();
			base.www = null;
		}
		if (base.www == null)
		{
			this.ExecuteNextBatch();
		}
		return base.mJobState;
	}

	public void ProcessResult(byte[] bytes, string errorMsg)
	{
		if (string.IsNullOrEmpty(errorMsg))
		{
			Debug.Log(this.ParseTranslationResult(Encoding.UTF8.GetString(bytes, 0, bytes.Length), "aab"));
			if (string.IsNullOrEmpty(errorMsg))
			{
				if (this._OnTranslationReady != null)
				{
					this._OnTranslationReady(this._requests, null);
				}
				return;
			}
		}
		base.mJobState = eJobState.Failed;
		this.mErrorMessage = errorMsg;
	}

	private string ParseTranslationResult(string html, string OriginalText)
	{
		unchecked
		{
			try
			{
				int num;
				num = html.IndexOf("TRANSLATED_TEXT='") + "TRANSLATED_TEXT='".Length;
				string text;
				text = Regex.Replace(Regex.Replace(html.Substring(num, html.IndexOf("';var", num) - num), "\\\\x([a-fA-F0-9]{2})", (Match match) => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value, NumberStyles.HexNumber))), "&#(\\d+);", (Match match) => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value))).Replace("<br>", "\n");
				if (OriginalText.ToUpper() == OriginalText)
				{
					text = text.ToUpper();
				}
				else if (GoogleTranslation.UppercaseFirst(OriginalText) == OriginalText)
				{
					text = GoogleTranslation.UppercaseFirst(text);
				}
				else if (GoogleTranslation.TitleCase(OriginalText) == OriginalText)
				{
					text = GoogleTranslation.TitleCase(text);
				}
				return text;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				return string.Empty;
			}
		}
	}
}
