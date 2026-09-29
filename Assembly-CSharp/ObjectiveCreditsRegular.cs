using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveCreditsRegular : Objective
{
	private Sequence m_Sequence;

	protected override void InternalInitialize()
	{
		JDebug.Log("ObjectiveCreditsRegular :: InternalInitialize", this, JDebug.JDebugType.UI);
		base.transform.SetParent(null);
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		GameManager.Instance.LockPause();
		this.ResetSequence();
		this.m_Sequence.InsertCallback(3f, () =>
		{
		});
		this.m_Sequence.InsertCallback(5f, () =>
		{
			GameManager.Instance.ClearPlayer();
			SceneManager.sceneLoaded += this.HandleGameSceneLoaded;
			SceneManager.LoadScene("Empty");
		});
	}

	private void HandleGameSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		JDebug.Log("ObjectiveCreditsRegular :: HandleGameSceneLoaded", this, JDebug.JDebugType.UI);
		SceneManager.sceneLoaded -= this.HandleGameSceneLoaded;
		this.ResetSequence();
		this.m_Sequence.InsertCallback(2f, () =>
		{
			AudioListener.volume = 1f;
			GameManager.Instance.UIManager.Show<UICredits>("UI/Views/UICredits", "VIEW").OnPlayOutComplete += this.HandleUICreditsOnPlayOutComplete;
		});
	}

	private void HandleUICreditsOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("ObjectiveCreditsRegular :: HandleUICreditsOnPlayOutComplete", this, JDebug.JDebugType.UI);
		(sender as UICredits).OnPlayOutComplete -= this.HandleUICreditsOnPlayOutComplete;
		SceneManager.LoadScene("Reset");
		base.Dispose();
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		SceneManager.sceneLoaded -= this.HandleGameSceneLoaded;
		this.KillSequence();
		base.OnDisposed();
	}
}
