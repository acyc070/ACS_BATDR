using System;

public class ButcherGangSpawner : JMonoBehaviour
{
	public bool IsActive { get; private set; }

	public bool IsSpawned { get; protected set; }

	public event EventHandler OnSpawned;

	public event EventHandler OnReturned;

	public event EventHandler OnEnabled;

	public event EventHandler OnDisabled;

	public void SetActive(bool active)
	{
		this.IsActive = active;
		if (this.IsActive)
		{
			this.Enable();
		}
		else
		{
			this.Disable();
		}
	}

	protected virtual void Enable()
	{
	}

	protected virtual void Disable()
	{
	}

	public virtual void Cancel()
	{
	}

	protected void SendOnSpawned()
	{
		this.OnSpawned.Send(this);
	}

	protected void SendOnReturned()
	{
		this.OnReturned.Send(this);
	}

	protected void SendOnEnabled()
	{
		this.OnEnabled.Send(this);
	}

	protected void SendOnDisabled()
	{
		this.OnDisabled.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnSpawned = null;
		this.OnReturned = null;
		this.OnEnabled = null;
		this.OnDisabled = null;
		base.OnDisposed();
	}
}
