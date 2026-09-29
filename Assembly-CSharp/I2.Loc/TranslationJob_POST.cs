using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace I2.Loc;

public class TranslationJob_POST : TranslationJob_WWW
{
	private Dictionary<string, TranslationQuery> _requests;

	private GoogleTranslation.fnOnTranslationReady _OnTranslationReady;

	public TranslationJob_POST(Dictionary<string, TranslationQuery> requests, GoogleTranslation.fnOnTranslationReady OnTranslationReady)
	{
		this._requests = requests;
		this._OnTranslationReady = OnTranslationReady;
		List<string> list;
		list = GoogleTranslation.ConvertTranslationRequest(requests, encodeGET: false);
		WWWForm wWWForm;
		wWWForm = new WWWForm();
		wWWForm.AddField("action", "Translate");
		wWWForm.AddField("list", list[0]);
		base.www = UnityWebRequest.Post(LocalizationManager.GetWebServiceURL(), wWWForm);
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
		return base.mJobState;
	}

	public void ProcessResult(byte[] bytes, string errorMsg)
	{
		if (!string.IsNullOrEmpty(errorMsg))
		{
			base.mJobState = eJobState.Failed;
			return;
		}
		errorMsg = GoogleTranslation.ParseTranslationResult(Encoding.UTF8.GetString(bytes, 0, bytes.Length), this._requests);
		if (this._OnTranslationReady != null)
		{
			this._OnTranslationReady(this._requests, errorMsg);
		}
		base.mJobState = eJobState.Succeeded;
	}
}
