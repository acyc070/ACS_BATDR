using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace I2.Loc;

public class TranslationJob_GET : TranslationJob_WWW
{
	private Dictionary<string, TranslationQuery> _requests;

	private GoogleTranslation.fnOnTranslationReady _OnTranslationReady;

	private List<string> mQueries;

	public string mErrorMessage;

	public TranslationJob_GET(Dictionary<string, TranslationQuery> requests, GoogleTranslation.fnOnTranslationReady OnTranslationReady)
	{
		this._requests = requests;
		this._OnTranslationReady = OnTranslationReady;
		this.mQueries = GoogleTranslation.ConvertTranslationRequest(requests, encodeGET: true);
		this.GetState();
	}

	private void ExecuteNextQuery()
	{
		if (this.mQueries.Count == 0)
		{
			base.mJobState = eJobState.Succeeded;
			return;
		}
		int index;
		index = unchecked(this.mQueries.Count - 1);
		string arg;
		arg = this.mQueries[index];
		this.mQueries.RemoveAt(index);
		base.www = UnityWebRequest.Get($"{LocalizationManager.GetWebServiceURL()}?action=Translate&list={arg}");
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
			this.ExecuteNextQuery();
		}
		return base.mJobState;
	}

	public void ProcessResult(byte[] bytes, string errorMsg)
	{
		if (string.IsNullOrEmpty(errorMsg))
		{
			errorMsg = GoogleTranslation.ParseTranslationResult(Encoding.UTF8.GetString(bytes, 0, bytes.Length), this._requests);
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
}
