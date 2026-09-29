using System;

public abstract class JDisposable : IDisposable
{
	public bool IsDisposed { get; private set; }

	protected virtual void OnDisposed()
	{
	}

	public void Dispose()
	{
		if (!this.IsDisposed)
		{
			this.OnDisposed();
			this.IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	~JDisposable()
	{
		this.Dispose();
	}
}
