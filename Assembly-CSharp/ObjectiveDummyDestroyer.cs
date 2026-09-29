public class ObjectiveDummyDestroyer : Objective
{
	protected override void InternalInitialize()
	{
		if (GameManager.Instance.DummyManager != null)
		{
			GameManager.Instance.DummyManager.KillAll();
		}
		base.SendOnComplete();
	}
}
