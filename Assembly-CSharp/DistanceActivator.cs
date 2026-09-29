using UnityEngine;

public class DistanceActivator : JMonoBehaviour
{
	[SerializeField]
	private GameObject[] m_GameObjects;

	private void Update()
	{
		if (GameManager.Instance.GameCamera == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		GameObject[] gameObjects;
		gameObjects = this.m_GameObjects;
		foreach (GameObject gameObject in gameObjects)
		{
			if (Vector3.Distance(GameManager.Instance.GameCamera.transform.position, gameObject.transform.position) < 20f)
			{
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(value: true);
				}
			}
			else if (gameObject.activeSelf)
			{
				gameObject.SetActive(value: false);
			}
		}
	}
}
