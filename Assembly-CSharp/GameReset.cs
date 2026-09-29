using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core;
using S13Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReset : JMonoBehaviour
{
	private List<GameObject> m_GameObjects = new List<GameObject>();

	public override void Awake()
	{
		if (GameManager.Instance.GameData != null && GameManager.Instance.GameData.CurrentSave != null)
		{
			GameManager.Instance.GameData.CurrentSave.PostSave();
		}
		GameManager.Instance.HideCursor();
		S13Manager.UnloadAllSoundBanks();
		this.m_GameObjects = Object.FindObjectsOfType<GameObject>().ToList();
		this.m_GameObjects.Remove(base.gameObject);
		foreach (GameObject gameObject in this.m_GameObjects)
		{
			if (!gameObject.GetComponent<DOTweenComponent>() && !gameObject.GetComponent<SteamManager>())
			{
				Object.Destroy(gameObject);
			}
		}
		GameManager.Instance.Clear();
		GameManager.Instance.isGameLoaded = true;
		GameManager.Instance.LockPause();
		Time.timeScale = 1f;
	}

	public override void Start()
	{
		base.StartCoroutine(this.GoToMainMenu());
	}

	private IEnumerator GoToMainMenu()
	{
		yield return new WaitForEndOfFrame();
		AsyncOperation async;
		async = SceneManager.LoadSceneAsync("InitializeGame");
		async.allowSceneActivation = false;
		while (async.progress < 0.9f)
		{
			yield return new WaitForEndOfFrame();
		}
		async.allowSceneActivation = true;
		base.Dispose();
	}

	protected override void OnDisposed()
	{
		if (this.m_GameObjects != null)
		{
			this.m_GameObjects.Clear();
			this.m_GameObjects = null;
		}
		base.OnDisposed();
	}
}
