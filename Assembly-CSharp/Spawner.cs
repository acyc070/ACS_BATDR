using System;

public abstract class Spawner : JMonoBehaviour
{
	public event EventHandler OnSpawn;

	public abstract void Spawn();

	public abstract void Activate();

	public abstract void Deactivate();

	protected void SendOnSpawn()
	{
		this.OnSpawn.Send(this);
	}
}
