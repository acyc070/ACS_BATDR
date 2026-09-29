using System;

[Serializable]
public class SectionSoftLoaderGroup
{
	public SectionID ID;

	public EventTrigger Loader;

	public EventTrigger Unloader;

	public event EventHandler OnLoad;

	public event EventHandler OnUnload;

	public void Initialize()
	{
		this.RemoveListeners();
		this.Enable();
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
	}

	public void Clear()
	{
		this.OnLoad = null;
		this.OnUnload = null;
		this.RemoveListeners();
	}
}
