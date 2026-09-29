using UnityEngine;

public class ObjectivePlayerSpawn : Objective
{
	[SerializeField]
	private Player m_Player;

	[SerializeField]
	private bool m_SetZone;

	[SerializeField]
	private string m_Zone;

	[SerializeField]
	private bool m_SetLocation;

	protected override void InternalInitialize()
	{
		if (this.m_Player != null)
		{
			if (GameManager.Instance.Player != null)
			{
				GameManager.Instance.Player.Dispose();
				GameManager.Instance.Player = null;
			}
			Object.DontDestroyOnLoad(GameManager.Instance.Player = GameManager.Instance.AssetManager.CreateAsset<Player>(this.m_Player));
			GameManager.Instance.Player.transform.position = new Vector3(0f, 2400f, 0f);
			GameManager.Instance.Player.Initialize();
			GameManager.Instance.Player.gameObject.SetActive(value: false);
			if (this.m_SetZone)
			{
				GameManager.Instance.Player.SetZone(this.m_Zone);
			}
			if (this.m_SetLocation)
			{
				GameManager.Instance.Player.transform.position = base.transform.position;
				GameManager.Instance.Player.transform.eulerAngles = base.transform.eulerAngles;
				GameManager.Instance.Player.gameObject.SetActive(value: true);
			}
		}
		else if (GameManager.Instance.Player != null && this.m_SetZone)
		{
			GameManager.Instance.Player.SetZone(this.m_Zone);
		}
		base.SendOnComplete();
	}
}
