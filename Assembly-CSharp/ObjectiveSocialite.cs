public class ObjectiveSocialite : Objective
{
	protected override void InternalInitialize()
	{
		GameManager.Instance.GameData.CurrentSave.Difficulty.UpdateSocialite();
	}
}
