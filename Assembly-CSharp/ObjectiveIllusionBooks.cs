public class ObjectiveIllusionBooks : Objective
{
	private bool m_CanCheck;

	protected override void InternalInitialize()
	{
		this.m_CanCheck = true;
	}

	private void Update()
	{
		if (this.m_CanCheck && !base.IsComplete && !base.IsDisposed && !GameManager.Instance.IsPaused && GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.Count >= 24)
		{
			this.m_CanCheck = false;
			base.SendOnComplete();
		}
	}
}
