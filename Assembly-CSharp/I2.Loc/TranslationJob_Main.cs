using System.Collections.Generic;

namespace I2.Loc;

public class TranslationJob_Main : TranslationJob
{
	private TranslationJob_WEB mWeb;

	private TranslationJob_POST mPost;

	private TranslationJob_GET mGet;

	private Dictionary<string, TranslationQuery> _requests;

	private GoogleTranslation.fnOnTranslationReady _OnTranslationReady;

	public string mErrorMessage;

	public TranslationJob_Main(Dictionary<string, TranslationQuery> requests, GoogleTranslation.fnOnTranslationReady OnTranslationReady)
	{
		this._requests = requests;
		this._OnTranslationReady = OnTranslationReady;
		this.mPost = new TranslationJob_POST(requests, OnTranslationReady);
	}

	public override eJobState GetState()
	{
		if (this.mWeb != null)
		{
			switch (this.mWeb.GetState())
			{
			case eJobState.Running:
				return eJobState.Running;
			case eJobState.Succeeded:
				base.mJobState = eJobState.Succeeded;
				break;
			case eJobState.Failed:
				this.mWeb.Dispose();
				this.mWeb = null;
				this.mPost = new TranslationJob_POST(this._requests, this._OnTranslationReady);
				break;
			}
		}
		if (this.mPost != null)
		{
			switch (this.mPost.GetState())
			{
			case eJobState.Running:
				return eJobState.Running;
			case eJobState.Succeeded:
				base.mJobState = eJobState.Succeeded;
				break;
			case eJobState.Failed:
				this.mPost.Dispose();
				this.mPost = null;
				this.mGet = new TranslationJob_GET(this._requests, this._OnTranslationReady);
				break;
			}
		}
		if (this.mGet != null)
		{
			switch (this.mGet.GetState())
			{
			case eJobState.Running:
				return eJobState.Running;
			case eJobState.Succeeded:
				base.mJobState = eJobState.Succeeded;
				break;
			case eJobState.Failed:
				this.mErrorMessage = this.mGet.mErrorMessage;
				if (this._OnTranslationReady != null)
				{
					this._OnTranslationReady(this._requests, this.mErrorMessage);
				}
				this.mGet.Dispose();
				this.mGet = null;
				break;
			}
		}
		return base.mJobState;
	}

	public override void Dispose()
	{
		if (this.mPost != null)
		{
			this.mPost.Dispose();
		}
		if (this.mGet != null)
		{
			this.mGet.Dispose();
		}
		this.mPost = null;
		this.mGet = null;
	}
}
