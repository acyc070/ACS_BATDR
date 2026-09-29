using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13AudioReference : IS13Controllable
{
	[SerializeField]
	private S13Object obj;

	[SerializeField]
	private S13AudioHandler handler;

	public IS13Controllable Reference
	{
		get
		{
			if ((bool)this.obj)
			{
				return this.obj;
			}
			if ((bool)this.handler)
			{
				return this.handler;
			}
			return null;
		}
	}

	public string name => this.ToString();

	public void Play()
	{
		if ((bool)this.obj)
		{
			S13Manager.Play(this.obj);
		}
		else if ((bool)this.handler)
		{
			this.handler.Play();
		}
	}

	public void Play(float duration)
	{
		if ((bool)this.obj)
		{
			S13Manager.Play(this.obj, duration);
		}
		else if ((bool)this.handler)
		{
			this.handler.Play(duration);
		}
	}

	public void PlayDelayed(float delayTime)
	{
		if ((bool)this.obj)
		{
			S13Manager.PlayDelayed(this.obj, delayTime);
		}
		else if ((bool)this.handler)
		{
			this.handler.PlayDelayed(delayTime);
		}
	}

	public void PlayDelayed(float delayTime, float duration)
	{
		if ((bool)this.obj)
		{
			S13Manager.PlayDelayed(this.obj, delayTime, duration);
		}
		else if ((bool)this.handler)
		{
			this.handler.PlayDelayed(delayTime, duration);
		}
	}

	public void Stop()
	{
		if ((bool)this.obj)
		{
			S13Manager.Stop(this.obj);
		}
		else if ((bool)this.handler)
		{
			this.handler.Stop();
		}
	}

	public void Stop(bool ignoreFades)
	{
		if ((bool)this.obj)
		{
			S13Manager.Stop(this.obj, ignoreFades);
		}
		else if ((bool)this.handler)
		{
			this.handler.Stop(ignoreFades);
		}
	}

	public void StopDelayed(float delayTime)
	{
		if ((bool)this.obj)
		{
			S13Manager.StopDelayed(this.obj, delayTime);
		}
		else if ((bool)this.handler)
		{
			this.handler.StopDelayed(delayTime);
		}
	}

	public void StopDelayed(float delayTime, bool ignoreFades)
	{
		if ((bool)this.obj)
		{
			S13Manager.StopDelayed(this.obj, delayTime, ignoreFades);
		}
		else if ((bool)this.handler)
		{
			this.handler.StopDelayed(delayTime, ignoreFades);
		}
	}

	public void Shutdown()
	{
		if ((bool)this.obj)
		{
			S13Manager.Shutdown(this.obj);
		}
		else if ((bool)this.handler)
		{
			this.handler.Shutdown();
		}
	}

	public void Pause()
	{
		if ((bool)this.obj)
		{
			S13Manager.Pause(this.obj);
		}
		else if ((bool)this.handler)
		{
			this.handler.Pause();
		}
	}

	public void Resume()
	{
		if ((bool)this.obj)
		{
			S13Manager.Resume(this.obj);
		}
		else if ((bool)this.handler)
		{
			this.handler.Resume();
		}
	}

	public override string ToString()
	{
		if ((bool)this.obj)
		{
			return this.obj.name;
		}
		if ((bool)this.handler)
		{
			return this.handler.name;
		}
		return "Empty Audio Reference";
	}
}
