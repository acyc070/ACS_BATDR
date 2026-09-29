using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableEnemy : Interactable
{
	private SectionID m_SectionID;

	private EnemyType m_EnemyType;

	private LostOneType m_LostOneType;

	private SkinnedMeshRenderer[] m_SkinnedMeshRenderers;

	private MeshRenderer[] m_MeshRenderers;

	private GameObject m_Model;

	private int m_LootableEnemyID;

	private float m_FoodChance = 0.2f;

	private float m_KitChance = 0.4f;

	private float m_BatteryChance = 0.6f;

	private float m_PartsChance = 0.8f;

	private float m_SelfDestructTimer;

	private float m_SelfDestructTimerLimit = 60f;

	private bool m_IsLooted;

	private bool m_IsColor;

	public EnemyType EnemyType => this.m_EnemyType;

	public LostOneType LostOneType => this.m_LostOneType;

	public void Initialize(GameObject model, SectionID sectionID, EnemyType enemyType, LostOneType lostOneType = LostOneType.NONE, bool isColor = false, Transform parent = null)
	{
		this.m_Model = model;
		this.m_SectionID = sectionID;
		this.m_EnemyType = enemyType;
		this.m_LostOneType = lostOneType;
		this.m_IsColor = isColor;
		this.m_Model.transform.SetParent(base.transform);
		if (parent == null)
		{
			Section section;
			section = GameManager.Instance.SectionManager.GetSection(this.m_SectionID);
			if (section != null)
			{
				SectionController[] sectionControllers;
				sectionControllers = section.SectionControllers;
				for (int i = 0; i < sectionControllers.Length; i = unchecked(i + 1))
				{
					if (sectionControllers[i] is SectionEnemyController sectionEnemyController)
					{
						sectionEnemyController.AddLootableEnemy(this);
						base.transform.SetParent(sectionEnemyController.transform);
						break;
					}
				}
			}
		}
		else
		{
			base.transform.SetParent(parent);
		}
		base.StartCoroutine(this.StartLootable());
	}

	public void AddData()
	{
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID);
		this.m_LootableEnemyID = base.gameObject.GetInstanceID();
		unchecked
		{
			while (sectionDataObject.LootableEnemyData.ContainsKey(this.m_LootableEnemyID))
			{
				this.m_LootableEnemyID += Random.Range(-100, 100);
			}
			LootableEnemyDataObject lootableEnemyDataObject;
			lootableEnemyDataObject = DataObject<int, LootableEnemyDataObject>.Create(this.m_LootableEnemyID);
			lootableEnemyDataObject.SetEnemyType(this.m_EnemyType);
			lootableEnemyDataObject.SetLostOneType(this.m_LostOneType);
			lootableEnemyDataObject.SetGameObjectData(this.m_Model);
			sectionDataObject.LootableEnemyData.Add(this.m_LootableEnemyID, lootableEnemyDataObject);
		}
	}

	public void UpdateData()
	{
		if (!this.m_IsLooted)
		{
			((LootableEnemyDataObject)((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID)).LootableEnemyData.GetValue(this.m_LootableEnemyID))?.SetGameObjectData(this.m_Model);
		}
	}

	public void SetData(int id)
	{
		this.m_LootableEnemyID = id;
		LootableEnemyDataObject lootableEnemyDataObject;
		lootableEnemyDataObject = (LootableEnemyDataObject)((SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID)).LootableEnemyData.GetValue(this.m_LootableEnemyID);
		if (lootableEnemyDataObject == null)
		{
			return;
		}
		Transform[] componentsInChildren;
		componentsInChildren = this.m_Model.GetComponentsInChildren<Transform>(includeInactive: true);
		List<Transform> list;
		list = new List<Transform>();
		int num;
		num = 0;
		Transform[] array;
		array = componentsInChildren;
		unchecked
		{
			foreach (Transform transform in array)
			{
				if (num > 0)
				{
					if (transform.GetComponent<SeasonalHat>() != null || transform.GetComponent<SeasonalHatContainer>() != null || transform.GetComponent<SeasonalHatLoader>() != null)
					{
						continue;
					}
					list.Add(transform);
				}
				num++;
			}
			for (int j = 0; j < list.Count; j++)
			{
				Transform transform2;
				transform2 = list[j];
				GameObjectDataExtensions.SetData(gameObjectData: lootableEnemyDataObject.GameObjectDatas[j], gameObject: transform2.gameObject);
			}
		}
	}

	private IEnumerator StartLootable()
	{
		this.m_SkinnedMeshRenderers = base.GetComponentsInChildren<SkinnedMeshRenderer>();
		this.m_MeshRenderers = base.GetComponentsInChildren<MeshRenderer>();
		this.SetActive(active: false);
		yield return new WaitForSeconds(1f);
		yield return new WaitForEndOfFrame();
		this.SetActive(active: true);
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && !this.m_IsLooted && this.m_LostOneType != LostOneType.Male_Amok && this.m_EnemyType != EnemyType.KingWidow && this.m_EnemyType != EnemyType.ShipAhoy)
		{
			if (this.m_SelfDestructTimer >= this.m_SelfDestructTimerLimit)
			{
				this.SelfDestruct();
			}
			else
			{
				this.m_SelfDestructTimer += Time.deltaTime;
			}
		}
	}

	public void SelfDestruct()
	{
		RaycastHit hit;
		hit = default;
		if (this.m_Model != null)
		{
			Collider componentInChildren;
			componentInChildren = this.m_Model.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				hit.point = componentInChildren.transform.position;
			}
			else
			{
				hit.point = this.m_Model.transform.position;
			}
		}
		this.Loot(hit, isSelfDestruct: true);
	}

	protected override void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null)
	{
		unchecked
		{
			if (this.m_SkinnedMeshRenderers != null)
			{
				for (int i = 0; i < this.m_SkinnedMeshRenderers.Length; i++)
				{
					SkinnedMeshRenderer skinnedMeshRenderer;
					skinnedMeshRenderer = this.m_SkinnedMeshRenderers[i];
					if (!(skinnedMeshRenderer != null) || skinnedMeshRenderer.materials == null || skinnedMeshRenderer.materials.Length == 0)
					{
						continue;
					}
					Material[] materials;
					materials = skinnedMeshRenderer.materials;
					foreach (Material material in materials)
					{
						if (material.HasFloat("_Gradient"))
						{
							material.SetFloat("_Gradient", 1f);
						}
						if (material.HasFloat("_PostHitGlow"))
						{
							material.SetFloat("_PostHitGlow", 2f);
						}
					}
				}
			}
			if (this.m_MeshRenderers != null)
			{
				for (int k = 0; k < this.m_MeshRenderers.Length; k++)
				{
					MeshRenderer meshRenderer;
					meshRenderer = this.m_MeshRenderers[k];
					if (!(meshRenderer != null) || meshRenderer.materials == null || meshRenderer.materials.Length == 0)
					{
						continue;
					}
					Material[] materials;
					materials = meshRenderer.materials;
					foreach (Material material2 in materials)
					{
						if (material2.HasFloat("_Gradient"))
						{
							material2.SetFloat("_Gradient", 1f);
						}
						if (material2.HasFloat("_PostHitGlow"))
						{
							material2.SetFloat("_PostHitGlow", 2f);
						}
					}
				}
			}
			GameManager.Instance.ShowInteraction(TextUtility.GetKey("NOTIFICATION_LOOT"));
		}
	}

	protected override void OnInternalExit(Vector3 origin, RaycastHit hit, object sender = null)
	{
		unchecked
		{
			if (this.m_SkinnedMeshRenderers != null)
			{
				for (int i = 0; i < this.m_SkinnedMeshRenderers.Length; i++)
				{
					SkinnedMeshRenderer skinnedMeshRenderer;
					skinnedMeshRenderer = this.m_SkinnedMeshRenderers[i];
					if (!(skinnedMeshRenderer != null) || skinnedMeshRenderer.materials == null || skinnedMeshRenderer.materials.Length == 0)
					{
						continue;
					}
					Material[] materials;
					materials = skinnedMeshRenderer.materials;
					foreach (Material material in materials)
					{
						if (material.HasFloat("_Gradient"))
						{
							material.SetFloat("_Gradient", 0.1f);
						}
						if (material.HasFloat("_PostHitGlow"))
						{
							material.SetFloat("_PostHitGlow", 0.3f);
						}
					}
				}
			}
			if (this.m_MeshRenderers != null)
			{
				for (int k = 0; k < this.m_MeshRenderers.Length; k++)
				{
					MeshRenderer meshRenderer;
					meshRenderer = this.m_MeshRenderers[k];
					if (!(meshRenderer != null) || meshRenderer.materials == null || meshRenderer.materials.Length == 0)
					{
						continue;
					}
					Material[] materials;
					materials = meshRenderer.materials;
					foreach (Material material2 in materials)
					{
						if (material2.HasFloat("_Gradient"))
						{
							material2.SetFloat("_Gradient", 0.1f);
						}
						if (material2.HasFloat("_PostHitGlow"))
						{
							material2.SetFloat("_PostHitGlow", 0.3f);
						}
					}
				}
			}
			GameManager.Instance.HideInteraction();
		}
	}

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		this.Loot(hit, isSelfDestruct: false);
	}

	private void Loot(RaycastHit hit, bool isSelfDestruct)
	{
		if (!isSelfDestruct)
		{
			GameManager.Instance.HideInteraction();
			GameManager.Instance.Player.Interaction.ResetInteraction();
		}
		string prefab;
		prefab = "Effects/Effects_Looted_Enemy";
		string prefab2;
		prefab2 = "Effects/Effects_Hit_Ink";
		if (this.m_IsColor)
		{
			prefab = "Effects/Effects_Looted_Enemy_Color";
			prefab2 = "Effects/Effects_Hit_Ink_Color";
		}
		GameManager.Instance.PoolingManager.GetFromPool(prefab, 10f).GetComponent<LootedEnemy>().Initialize(hit.point);
		GameManager.Instance.PoolingManager.GetFromPool(prefab2, 6f).transform.position = hit.point;
		if (!isSelfDestruct)
		{
			this.GetLoot();
		}
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID);
		LootableEnemyDataObject lootableEnemyDataObject;
		lootableEnemyDataObject = (LootableEnemyDataObject)sectionDataObject.LootableEnemyData.GetValue(this.m_LootableEnemyID);
		if (lootableEnemyDataObject != null)
		{
			sectionDataObject.LootableEnemyData.Remove(lootableEnemyDataObject);
		}
		this.m_IsLooted = true;
		if (isSelfDestruct)
		{
			base.Dispose();
		}
	}

	private void GetLoot()
	{
		if (this.m_LostOneType == LostOneType.Male_Amok)
		{
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_EAT") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_BACON_SOUP"), "Icon/Collectables/Small/UIIcon_Food", 1);
			GameManager.Instance.Player.Heal(unchecked((int)UpgradeCheck.GetHealth()));
			int amount;
			amount = 20;
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_SLUGS"), "Icon/Collectables/Small/UIIcon_Slug", amount, 0.15f);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddSlugs(amount);
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_TOOLKIT"), "Icon/Collectables/Small/UIIcon_Toolkit", 1, 0.3f);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddToolkit();
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_PARTS"), "Icon/Collectables/Small/UIIcon_Parts", 2, 0.45f);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddPart();
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddPart();
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_BATTERY"), "Icon/Collectables/Small/UIIcon_Battery", 1, 0.6f);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddBattery();
		}
		else if (this.m_EnemyType == EnemyType.LostOneSeasonalWinter)
		{
			if (!SeasonalCheck.HasHat)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_SEASONAL_WINTER_HAT"));
				GameObject gameObject;
				gameObject = (GameObject)Object.Instantiate(Resources.Load("Hats/Prefab_Seasonal_Winter_Hat_01_Audrey"));
				Transform transform;
				transform = GameManager.Instance.Player.transform.FindDeepChild("WinterHat");
				if (transform != null)
				{
					gameObject.transform.SetParent(transform);
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localEulerAngles = Vector3.zero;
				}
				SeasonalCheck.HasHat = true;
			}
			else
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_SEASONAL_PRESENT"));
				PresentCheck.Collect();
			}
		}
		else if (this.m_EnemyType == EnemyType.LostOneSeasonalHalloween)
		{
			if (!SeasonalCheck.HasHat)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_SEASONAL_HALLOWEEN_HAT"));
				GameObject gameObject2;
				gameObject2 = (GameObject)Object.Instantiate(Resources.Load("Hats/Prefab_Seasonal_Halloween_Hat_01_Audrey"));
				Transform transform2;
				transform2 = GameManager.Instance.Player.transform.FindDeepChild("HalloweenHat");
				if (transform2 != null)
				{
					gameObject2.transform.SetParent(transform2);
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localEulerAngles = Vector3.zero;
				}
				SeasonalCheck.HasHat = true;
			}
		}
		else if (Random.value <= LootCheck.GetChance())
		{
			float value;
			value = Random.value;
			if (value <= this.m_FoodChance)
			{
				string[] array;
				array = new string[3] { "NOTIFICATION_LOOT_BENDY_BAR", "NOTIFICATION_LOOT_SANDWHICH", "NOTIFICATION_LOOT_DONUT" };
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_EAT") + " " + TextUtility.GetKey(array[Random.Range(0, array.Length)]), "Icon/Collectables/Small/UIIcon_Food", 1);
				GameManager.Instance.Player.Heal(1);
				this.CheckSlugs(value, 0.15f);
			}
			else if (value <= this.m_KitChance)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_TOOLKIT"), "Icon/Collectables/Small/UIIcon_Toolkit", 1);
				GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddToolkit();
				this.CheckSlugs(value, 0.15f);
			}
			else if (value <= this.m_BatteryChance)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_BATTERY"), "Icon/Collectables/Small/UIIcon_Battery", 1);
				GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddBattery();
				this.CheckSlugs(value, 0.15f);
			}
			else if (value <= this.m_PartsChance)
			{
				GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_PARTS"), "Icon/Collectables/Small/UIIcon_Parts", 1);
				GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddPart();
				this.CheckSlugs(value, 0.15f);
			}
			else
			{
				this.GetSlugs();
			}
		}
	}

	private void CheckSlugs(float percent, float delay)
	{
		if (percent < 0.1f)
		{
			this.GetSlugs(delay);
		}
	}

	private void GetSlugs(float delay = 0f)
	{
		int num;
		num = Random.Range(1, 6);
		string key;
		key = TextUtility.GetKey("NOTIFICATION_LOOT_SLUG");
		if (num > 1)
		{
			key = TextUtility.GetKey("NOTIFICATION_LOOT_SLUGS");
		}
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + key, "Icon/Collectables/Small/UIIcon_Slug", num, delay);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddSlugs(num);
	}

	protected override void OnDisposed()
	{
		this.m_SkinnedMeshRenderers = null;
		this.m_MeshRenderers = null;
		this.m_Model = null;
		base.OnDisposed();
	}
}
