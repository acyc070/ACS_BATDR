using System;
using DG.Tweening;
using UnityEngine;

public class PresentController : JMonoBehaviour
{
	[SerializeField]
	private Transform m_LootLocation;

	[SerializeField]
	private Transform m_NodeContainer;

	private Present[] m_Presents;

	public event EventHandler OnLoot;

	public override void Start()
	{
		this.m_Presents = base.GetComponentsInChildren<Present>(includeInactive: true);
		if (this.m_Presents == null)
		{
			return;
		}
		Present[] presents;
		presents = this.m_Presents;
		foreach (Present present in presents)
		{
			if (present != null)
			{
				present.OnActivate -= this.HandlePresentOnActivate;
				present.OnActivate += this.HandlePresentOnActivate;
			}
		}
	}

	private void HandlePresentOnActivate(object sender, EventArgs e)
	{
		Present present;
		present = sender as Present;
		if (present != null)
		{
			present.OnActivate -= this.HandlePresentOnActivate;
		}
		if (PresentCheck.PresentCount >= 20)
		{
			Section componentInParent;
			componentInParent = base.GetComponentInParent<Section>();
			SeasonalEnemy seasonalEnemy;
			seasonalEnemy = GameManager.Instance.AssetManager.CreateAsset<SeasonalEnemy>("Enemies/Enemy_LostOne_Seasonal_Winter");
			seasonalEnemy.SetSection(componentInParent.SectionID);
			seasonalEnemy.SetNodes(this.m_NodeContainer.GetComponentsInChildren<CharacterInteractionNode>());
			seasonalEnemy.transform.SetParent(base.transform);
			seasonalEnemy.transform.eulerAngles = GameManager.Instance.Player.transform.eulerAngles;
			seasonalEnemy.transform.position = GameManager.Instance.Player.transform.position - GameManager.Instance.Player.transform.forward * 7f;
			seasonalEnemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
			seasonalEnemy.OnInitializeOnComplete += this.HandleEnemyOnInitializeOnComplete;
			seasonalEnemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
			seasonalEnemy.OnCharacterDeath += this.HandleEnemyOnCharacterDeath;
			PresentCheck.Reset();
		}
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		SeasonalEnemy obj;
		obj = sender as SeasonalEnemy;
		obj.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		obj.SetState(State.Character.Seasonal);
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		SeasonalEnemy obj;
		obj = sender as SeasonalEnemy;
		obj.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
		obj.OnAnimationComplete -= this.HandleEnemyDeathOnAnimationComplete;
		obj.OnAnimationComplete += this.HandleEnemyDeathOnAnimationComplete;
	}

	private void HandleEnemyDeathOnAnimationComplete(object sender, EventArgs e)
	{
		SeasonalEnemy seasonalEnemy;
		seasonalEnemy = sender as SeasonalEnemy;
		seasonalEnemy.OnAnimationComplete -= this.HandleEnemyDeathOnAnimationComplete;
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
		if (this.m_Presents != null)
		{
			Present[] presents;
			presents = this.m_Presents;
			foreach (Present present in presents)
			{
				if (present != null)
				{
					present.OnActivate -= this.HandlePresentOnActivate;
				}
			}
		}
		base.OnDisposed();
	}
}
