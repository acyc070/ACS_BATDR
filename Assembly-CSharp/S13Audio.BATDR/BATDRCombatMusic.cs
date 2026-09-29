using System;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio.BATDR;

[RequireComponent(typeof(S13AccessorToParameters))]
public class BATDRCombatMusic : MonoBehaviour
{
	[Serializable]
	internal struct EnemyContent
	{
		public bool isEmpty;

		public EnemyType type;

		public S13Object combatMusic;

		public S13Object discoveredStinger;

		public S13Object defeatEnemiesStinger;

		public S13Object hideFromEnemiesStinger;
	}

	[Header("Combat Settings")]
	[Tooltip("First element is designated as the default generic content")]
	[SerializeField]
	private List<EnemyContent> _enemyContent = new List<EnemyContent>();

	[Header("Actions")]
	[SerializeField]
	[Tooltip("Triggers when alert music starts")]
	private S13LocalAction alertAction;

	[SerializeField]
	[Tooltip("Triggers when all enemies are defeated")]
	private S13LocalAction defeatedEnemiesAction;

	[SerializeField]
	[Tooltip("Triggers when hidden from all enemies")]
	private S13LocalAction hiddenFromEnemiesAction;

	[Header("Danger Settings")]
	[SerializeField]
	private S13Object dangerMusic;

	[SerializeField]
	[Tooltip("Triggers when danger music starts")]
	private S13LocalAction startDangerAction;

	[SerializeField]
	[Tooltip("Triggers when danger music stops")]
	private S13LocalAction stopDangerAction;

	[Header("Parameters")]
	[SerializeField]
	private float dangerDistanceThreshold = 35f;

	private S13AccessorToParameters dangerVolume;

	[Header("Debug")]
	[Tooltip("For Testing purposes only")]
	[SerializeField]
	private float currentDistance;

	private bool IsPlaying
	{
		get
		{
			foreach (EnemyContent item in this._enemyContent)
			{
				if (item.combatMusic.isPlaying)
				{
					return true;
				}
			}
			return false;
		}
	}

	private bool DangerWithinThreshold => this.currentDistance >= this.dangerDistanceThreshold;

	private void Awake()
	{
		this.dangerVolume = base.GetComponent<S13AccessorToParameters>();
	}

	private void OnDisable()
	{
		foreach (EnemyContent item in this._enemyContent)
		{
			item.combatMusic.Stop();
		}
		this.dangerVolume.UpdateValue(float.PositiveInfinity);
	}

	public void OnAlert(EnemyType enemyType = EnemyType.Generic)
	{
		if (!base.enabled)
		{
			return;
		}
		this.StopDanger();
		this.alertAction.Execute();
		EnemyContent playingContentType;
		playingContentType = this.GetPlayingContentType();
		if (playingContentType.hideFromEnemiesStinger.isPlaying)
		{
			playingContentType.hideFromEnemiesStinger.Stop(ignoreFades: true);
		}
		if (playingContentType.defeatEnemiesStinger.isPlaying)
		{
			playingContentType.defeatEnemiesStinger.Stop(ignoreFades: true);
		}
		EnemyContent matchingContentType;
		matchingContentType = this.GetMatchingContentType(enemyType);
		if (!matchingContentType.isEmpty)
		{
			if (matchingContentType.discoveredStinger.isPlaying)
			{
				matchingContentType.discoveredStinger.Stop(ignoreFades: true);
			}
			matchingContentType.discoveredStinger.Play();
			matchingContentType.combatMusic.Play();
		}
	}

	public void OnDefeatedEnemies()
	{
		if (base.enabled && this.IsPlaying)
		{
			this.StopCombat();
			this.GetPlayingContentType().defeatEnemiesStinger.Play();
			this.defeatedEnemiesAction.Execute();
		}
	}

	public void OnHiddenFromEnemies()
	{
		if (base.enabled && this.IsPlaying)
		{
			this.StopCombat();
			this.GetPlayingContentType().hideFromEnemiesStinger.Play();
			this.hiddenFromEnemiesAction.Execute();
		}
	}

	public void StopCombat()
	{
		if (!base.enabled)
		{
			return;
		}
		foreach (EnemyContent item in this._enemyContent)
		{
			item.combatMusic.Stop();
		}
		this.UpdateEnemyDistance(this.currentDistance);
	}

	public void UpdateEnemyDistance(float distanceToClosestEnemy)
	{
		if (base.enabled && !this.IsPlaying)
		{
			this.currentDistance = distanceToClosestEnemy;
			float nextValue;
			if (this.DangerWithinThreshold)
			{
				nextValue = 1f;
				this.StopDanger();
			}
			else
			{
				nextValue = Mathf.Clamp01(this.currentDistance / this.dangerDistanceThreshold);
				this.PlayDanger();
			}
			this.dangerVolume.UpdateValue(nextValue);
		}
	}

	private EnemyContent GetMatchingContentType(EnemyType enemyType)
	{
		foreach (EnemyContent item in this._enemyContent)
		{
			if (enemyType == item.type)
			{
				return item;
			}
		}
		return new EnemyContent
		{
			isEmpty = true
		};
	}

	private EnemyContent GetPlayingContentType()
	{
		foreach (EnemyContent item in this._enemyContent)
		{
			if (item.combatMusic.isPlaying)
			{
				return item;
			}
		}
		return this._enemyContent[0];
	}

	private void PlayDanger()
	{
		if (!this.dangerMusic.isPlaying)
		{
			this.dangerMusic.Play();
			this.startDangerAction.Execute();
		}
	}

	public void StopDanger()
	{
		if (this.dangerMusic.isPlaying)
		{
			this.dangerMusic.Stop();
			this.stopDangerAction.Execute();
		}
	}
}
