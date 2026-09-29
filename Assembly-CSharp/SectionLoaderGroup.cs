using System;

[Serializable]
public class SectionLoaderGroup
{
	public SectionID ID;

	public EventTrigger Loader;

	public EventTrigger Unloader;

	public Requirements Requirements;

	public event EventHandler OnLoad;

	public event EventHandler OnUnload;

	public void Initialize()
	{
		this.RemoveListeners();
		if (!this.CheckStatus())
		{
			if (this.Loader != null)
			{
				this.Loader.SetActive(active: false);
			}
			if (this.Unloader != null)
			{
				this.Unloader.SetActive(active: false);
			}
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
		else
		{
			this.Enable();
		}
	}

	private void Enable()
	{
		this.EnableLoader();
		this.EnableUnloader();
	}

	private void EnableLoader()
	{
		if (this.Loader != null)
		{
			this.Loader.OnEnter -= this.HandleLoaderOnEnter;
			this.Loader.OnEnter += this.HandleLoaderOnEnter;
			this.Loader.SetActive(active: true);
		}
	}

	private void EnableUnloader()
	{
		if (this.Unloader != null)
		{
			this.Unloader.OnEnter -= this.HandleUnloaderOnEnter;
			this.Unloader.OnEnter += this.HandleUnloaderOnEnter;
			this.Unloader.SetActive(active: true);
		}
	}

	private void HandleLoaderOnEnter(object sender, EventArgs e)
	{
		this.OnLoad.Send(this);
	}

	private void HandleUnloaderOnEnter(object sender, EventArgs e)
	{
		this.OnUnload.Send(this);
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("SectionLoaderGroup :: HandleOnObjectiveComplete", null, JDebug.JDebugType.Objectives);
		if (this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			this.RemoveListeners();
			this.Enable();
		}
	}

	private bool CheckStatus()
	{
		bool result;
		result = true;
		if (this.Requirements != null)
		{
			result = this.Requirements.IsComplete();
		}
		return result;
	}

	public void RemoveListeners()
	{
		if (this.Loader != null)
		{
			this.Loader.OnEnter -= this.HandleLoaderOnEnter;
		}
		if (this.Unloader != null)
		{
			this.Unloader.OnEnter -= this.HandleUnloaderOnEnter;
		}
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
	}

	public void Clear()
	{
		this.OnLoad = null;
		this.OnUnload = null;
		this.RemoveListeners();
	}
}
