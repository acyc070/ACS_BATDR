using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;
using UnityEngine.AI;

public class SeasonalSkullController : JMonoBehaviour
{
	[SerializeField]
	private SeasonalCauldron m_Cauldron;

	[SerializeField]
	private Transform m_LootLocation;

	[SerializeField]
	private Transform m_NodeContainer;

	[SerializeField]
	private Transform m_SpawnContainer;

	[SerializeField]
	private S13AudioHandler m_AudioHandler_Music;

	private bool m_IsActive;

	private int m_EnemyLimit;

	private int m_EnemyCount;

	private List<Enemy> m_Enemies = new List<Enemy>();

	public event EventHandler OnLoot;

	public override void Start()
	{
		this.m_Cauldron.Content.Disable();
		this.m_Cauldron.OnActivate += this.HandleCauldronOnActivated;
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		GameManager.Instance.Player.OnDeath += this.HandlePlayerOnDeath;
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		if (this.m_AudioHandler_Music.GetObject().isPlaying)
		{
			this.m_AudioHandler_Music.Stop();
		}
		this.m_EnemyCount = 0;
		if (this.m_Enemies != null)
		{
			for (int i = 0; i < this.m_Enemies.Count; i = unchecked(i + 1))
			{
				this.m_Enemies[i].Dispose();
			}
			this.m_Enemies.Clear();
		}
	}

	private void Update()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (!this.m_IsActive)
		{
			if (SeasonalSkullCheck.SkullCount >= 6)
			{
				this.m_IsActive = true;
				this.m_Cauldron.Content.Enable();
				this.m_Cauldron.EnableParticles();
			}
		}
		else if (SeasonalSkullCheck.SkullCount < 6)
		{
			this.m_IsActive = false;
			this.m_Cauldron.Content.Disable();
			this.m_Cauldron.DisableParticles();
		}
	}

	private void HandleCauldronOnActivated(object sender, EventArgs e)
	{
		this.m_IsActive = false;
		this.m_Cauldron.Content.Disable();
		this.m_Cauldron.DisableParticles();
		this.m_Cauldron.Deposit();
		SeasonalSkullCheck.Reset();
		Section componentInParent;
		componentInParent = base.GetComponentInParent<Section>();
		CharacterNode[] componentsInChildren;
		componentsInChildren = this.m_SpawnContainer.GetComponentsInChildren<CharacterNode>();
		this.m_EnemyLimit = componentsInChildren.Length;
		foreach (CharacterNode characterNode in componentsInChildren)
		{
			if (!(characterNode == this.m_SpawnContainer))
			{
				Enemy enemy;
				enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>("Enemies/Enemy_LostOne_Seasonal_Halloween_PumpkinHead");
				enemy.SetSection(componentInParent.SectionID);
				enemy.SetNode(characterNode);
				if (!this.m_Enemies.Contains(enemy))
				{
					this.m_Enemies.Add(enemy);
				}
				enemy.transform.SetParent(base.transform);
				enemy.transform.position = characterNode.transform.position;
				enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
				enemy.OnInitializeOnComplete += this.HandleEnemyOnInitializeOnComplete;
				enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
				enemy.OnCharacterDeath += this.HandleEnemyOnCharacterDeath;
			}
		}
		this.m_AudioHandler_Music.Play();
		SeasonalHalloweenAchievement.Mask();
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		Enemy obj;
		obj = sender as Enemy;
		obj.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		obj.SetTarget(GameManager.Instance.Player.transform);
		obj.SetState(State.Character.Follow);
		obj.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		obj.Agent.ResetAgent();
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		(sender as Enemy).OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
		unchecked
		{
			this.m_EnemyCount++;
			if (this.m_EnemyCount >= this.m_EnemyLimit)
			{
				this.m_EnemyCount = 0;
				if (this.m_Enemies != null)
				{
					this.m_Enemies.Clear();
				}
				Section componentInParent;
				componentInParent = base.GetComponentInParent<Section>();
				SeasonalEnemy seasonalEnemy;
				seasonalEnemy = GameManager.Instance.AssetManager.CreateAsset<SeasonalEnemy>("Enemies/Enemy_LostOne_Seasonal_Halloween");
				seasonalEnemy.SetSection(componentInParent.SectionID);
				seasonalEnemy.SetNodes(this.m_NodeContainer.GetComponentsInChildren<CharacterInteractionNode>());
				seasonalEnemy.transform.SetParent(base.transform);
				seasonalEnemy.transform.eulerAngles = GameManager.Instance.Player.transform.eulerAngles;
				seasonalEnemy.transform.position = GameManager.Instance.Player.transform.position - GameManager.Instance.Player.transform.forward * 7f;
				seasonalEnemy.OnInitializeOnComplete -= this.HandleSeasonalEnemyOnInitializeOnComplete;
				seasonalEnemy.OnInitializeOnComplete += this.HandleSeasonalEnemyOnInitializeOnComplete;
				seasonalEnemy.OnCharacterDeath -= this.HandleSeasonalEnemyOnCharacterDeath;
				seasonalEnemy.OnCharacterDeath += this.HandleSeasonalEnemyOnCharacterDeath;
				this.m_AudioHandler_Music.Stop();
			}
		}
	}

	private void HandleSeasonalEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		SeasonalEnemy obj;
		obj = sender as SeasonalEnemy;
		obj.OnInitializeOnComplete -= this.HandleSeasonalEnemyOnInitializeOnComplete;
		obj.SetState(State.Character.Seasonal);
	}

	private void HandleSeasonalEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		SeasonalEnemy obj;
		obj = sender as SeasonalEnemy;
		obj.OnCharacterDeath -= this.HandleSeasonalEnemyOnCharacterDeath;
		obj.OnAnimationComplete -= this.HandleSeasonalEnemyDeathOnAnimationComplete;
		obj.OnAnimationComplete += this.HandleSeasonalEnemyDeathOnAnimationComplete;
	}

	private void HandleSeasonalEnemyDeathOnAnimationComplete(object sender, EventArgs e)
	{
		SeasonalEnemy seasonalEnemy;
		seasonalEnemy = sender as SeasonalEnemy;
		seasonalEnemy.OnAnimationComplete -= this.HandleSeasonalEnemyDeathOnAnimationComplete;
		Collider[] colliders;
		colliders = seasonalEnemy.Content.GetComponentsInChildren<Collider>();
		Sequence sequence;
		sequence = DOTween.Sequence();
		Vector3 position;
		position = seasonalEnemy.transform.position;
		Vector3 position2;
		position2 = position + seasonalEnemy.transform.forward * 2f + -seasonalEnemy.transform.right * 2f;
		this.m_LootLocation.position = position2;
		float num;
		num = 0f;
		unchecked
		{
			for (int i = 0; i < 20; i++)
			{
				Slug slug;
				slug = GameManager.Instance.AssetManager.CreateAsset<Slug>("Slugs/Slug_Many");
				(slug.Content as SlugContent).InitializeContent();
				slug.Content.Disable();
				slug.transform.position = position2;
				slug.transform.localScale = Vector3.zero;
				Vector3 endValue;
				endValue = position + global::UnityEngine.Random.insideUnitSphere * 2f;
				endValue.y = position2.y;
				sequence.Insert(num + 0f, slug.transform.DOScale(1f, 0.25f).SetEase(Ease.Linear));
				sequence.Insert(num + 0f, slug.transform.DOMoveY(position2.y + 7f, 0.25f).SetEase(Ease.Linear));
				sequence.Insert(num + 0.25f, slug.transform.DOMove(endValue, 0.25f).SetEase(Ease.Linear));
				sequence.InsertCallback(num, () =>
				{
					this.OnLoot.Send(this);
				});
				sequence.InsertCallback(num + 0.5f, slug.Content.Enable);
				num += 0.2f;
			}
			sequence.OnComplete(() =>
			{
				for (int j = 0; j < colliders.Length; j++)
				{
					colliders[j].enabled = true;
				}
			});
			seasonalEnemy.Dispose();
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_Enemies != null)
		{
			this.m_Enemies.Clear();
			this.m_Enemies = null;
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		}
		base.OnDisposed();
	}
}
