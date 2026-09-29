public class DifficultyDisabler : JMonoBehaviour
{
	public override void Start()
	{
		if (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty == DifficultyLevel.Hard || GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty == DifficultyLevel.Impossible)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
