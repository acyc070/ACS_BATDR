using System.Collections;
using UnityEngine;

public class ObjectiveAutoSave : Objective
{
	[SerializeField]
	private bool m_DisableNotification;

	[SerializeField]
	private bool m_WaitToComplete;

	private bool CantSave
	{
		get
		{
			if (!GameManager.Instance.IsPaused)
			{
				return !GameManager.Instance.Player.gameObject.activeInHierarchy;
			}
			return true;
		}
	}

	protected override void InternalInitialize()
	{
		base.StartCoroutine(this.AutoSave());
		if (!this.m_WaitToComplete)
		{
			base.SendOnComplete();
		}
	}

	private IEnumerator AutoSave()
	{
		while (this.CantSave)
		{
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(0.2f);
		while (this.CantSave)
		{
			yield return new WaitForEndOfFrame();
		}
		if (!base.IsDisposed)
		{
			if (this.m_WaitToComplete)
			{
				GameManager.Instance.GameData.AutoSave(this.m_DisableNotification, base.SendOnComplete);
			}
			else
			{
				GameManager.Instance.GameData.AutoSave(this.m_DisableNotification);
			}
		}
	}
}
