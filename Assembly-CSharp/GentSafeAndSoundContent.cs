using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GentSafeAndSoundContent : ActionEventContent<GentSafeAndSoundContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public GameObject Lighting;

		public GameObject Blocker;

		public float TweenDuration = 0.4f;

		public Ease TweenEase = Ease.OutSine;
	}

	[Header("Shelves")]
	[SerializeField]
	private BoxCollider m_TopShelf;

	[SerializeField]
	private BoxCollider m_BottomShelf;

	[Header("Loot Containers")]
	[SerializeField]
	private Transform[] m_LootContainers;

	private int m_LootCount;

	private List<ActionEventController> m_Loot = new List<ActionEventController>();

	private Transform m_ActiveContainer;

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) > 15f)
		{
			if (base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards >= 1)
			{
				this.Enable();
			}
			else if (base.IsActivated)
			{
				this.ForceClose();
			}
		}
		if (!base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards < 1)
		{
			this.Disable();
		}
	}

	protected override void InternalEnable()
	{
		if (base.IsActivated)
		{
			base.Connectable.SendOnOnDeactivate();
		}
		base.IsActivated = false;
		if (base.m_Sequencer != null)
		{
			base.m_Sequencer.New();
		}
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			this.ResetContent(base.m_Properties[i]);
		}
	}

	protected override void InternalDisable()
	{
		for (int i = 0; i < base.m_Properties.Length; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_Properties[i];
			if (properties.Lighting != null)
			{
				properties.Lighting.SetActive(value: false);
			}
			if (properties.Blocker != null)
			{
				properties.Blocker.SetActive(value: false);
			}
		}
	}

	protected override void OnActivate()
	{
		this.RemoveLootListeners();
		this.ResetLoot();
		bool flag;
		flag = true;
		Collider[] array;
		array = Physics.OverlapBox(this.m_TopShelf.bounds.center, this.m_TopShelf.bounds.extents);
		unchecked
		{
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform;
				transform = array[i].transform;
				if (transform.GetComponentInParent<AudioLogContent>() != null || transform.GetComponentInParent<IllusionContent>() != null || transform.GetComponentInParent<GentSchematicContent>() != null)
				{
					flag = false;
				}
				if (!flag)
				{
					break;
				}
			}
			bool flag2;
			flag2 = true;
			Collider[] array2;
			array2 = Physics.OverlapBox(this.m_BottomShelf.bounds.center, this.m_BottomShelf.bounds.extents);
			for (int j = 0; j < array2.Length; j++)
			{
				Transform transform2;
				transform2 = array2[j].transform;
				if (transform2.GetComponentInParent<AudioLogContent>() != null || transform2.GetComponentInParent<IllusionContent>() != null || transform2.GetComponentInParent<GentSchematicContent>() != null)
				{
					flag2 = false;
				}
				if (!flag2)
				{
					break;
				}
			}
			if (this.m_ActiveContainer == null)
			{
				this.m_ActiveContainer = this.m_LootContainers[global::UnityEngine.Random.Range(0, this.m_LootContainers.Length)];
			}
			else
			{
				Transform transform3;
				transform3 = this.m_ActiveContainer;
				while (transform3 == this.m_ActiveContainer)
				{
					transform3 = this.m_LootContainers[global::UnityEngine.Random.Range(0, this.m_LootContainers.Length)];
				}
				this.m_ActiveContainer = transform3;
			}
			ActionEventController[] componentsInChildren;
			componentsInChildren = this.m_ActiveContainer.GetComponentsInChildren<ActionEventController>(includeInactive: true);
			foreach (ActionEventController actionEventController in componentsInChildren)
			{
				actionEventController.gameObject.SetActive(value: false);
				if ((flag || !this.m_TopShelf.bounds.Contains(actionEventController.transform.position)) && (flag2 || !this.m_BottomShelf.bounds.Contains(actionEventController.transform.position)) && global::UnityEngine.Random.value < 0.45f)
				{
					this.GenerateLoot(actionEventController);
				}
			}
			this.m_ActiveContainer.gameObject.SetActive(value: true);
			base.IsTooExpensive = false;
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_SPENT") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_CARD"), "Icon/Collectables/Small/UIIcon_Card", 1);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveCard();
			this.Disable();
			if (base.m_Sequencer != null)
			{
				base.m_Sequencer.New();
			}
			for (int l = 0; l < base.m_ActiveProperties.Count; l++)
			{
				this.CheckAnimation(base.m_ActiveProperties[l]);
			}
		}
	}

	private void GenerateLoot(ActionEventController lootPrefab)
	{
		ActionEventController actionEventController;
		actionEventController = GameManager.Instance.AssetManager.CreateAsset<ActionEventController>(lootPrefab);
		unchecked
		{
			if (!this.m_Loot.Contains(actionEventController))
			{
				this.m_Loot.Add(actionEventController);
				actionEventController.transform.SetParent(this.m_ActiveContainer);
				actionEventController.transform.position = lootPrefab.transform.position;
				actionEventController.transform.eulerAngles = lootPrefab.transform.eulerAngles;
				actionEventController.Initialize();
				if (actionEventController.Content is FoodContent foodContent)
				{
					foodContent.InitializeContent();
				}
				else if (actionEventController.Content is SlugContent slugContent)
				{
					slugContent.InitializeContent();
				}
				else if (actionEventController.Content is GentPartsContent gentPartsContent)
				{
					gentPartsContent.InitializeContent();
				}
				else if (actionEventController.Content is GentBatteryContent gentBatteryContent)
				{
					gentBatteryContent.InitializeContent();
				}
				else if (actionEventController.Content is GentBatteryCasingContent gentBatteryCasingContent)
				{
					gentBatteryCasingContent.InitializeContent();
				}
				else
				{
					if (!(actionEventController.Content is GentToolkitContent gentToolkitContent))
					{
						actionEventController.Dispose();
						return;
					}
					gentToolkitContent.InitializeContent();
				}
				this.m_LootCount++;
				actionEventController.gameObject.SetActive(value: true);
				actionEventController.OnActivate -= this.HandleLootOnActivate;
				actionEventController.OnActivate += this.HandleLootOnActivate;
			}
			else
			{
				actionEventController.Dispose();
			}
		}
	}

	protected override void OnActivateComplete()
	{
		for (int i = 0; i < base.m_ActiveProperties.Count; i = unchecked(i + 1))
		{
			Properties properties;
			properties = base.m_ActiveProperties[i];
			if (properties.Lighting != null)
			{
				properties.Lighting.SetActive(value: false);
			}
			if (properties.Blocker != null)
			{
				properties.Blocker.SetActive(value: false);
			}
			if (properties.ActionEvent != null)
			{
				properties.ActionEvent.gameObject.SetActive(value: false);
			}
		}
	}

	private void ResetContent(Properties properties)
	{
		base.DoAnimation(properties, properties.OriginPosition, properties.OriginRotation, properties.TweenDuration, properties.TweenEase);
		if (properties.Lighting != null)
		{
			properties.Lighting.SetActive(value: true);
		}
		if (properties.Blocker != null)
		{
			properties.Blocker.SetActive(value: true);
		}
		if (properties.ActionEvent != null)
		{
			properties.ActionEvent.gameObject.SetActive(value: true);
		}
	}

	private void CheckAnimation(Properties properties)
	{
		base.DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, properties.TweenDuration, properties.TweenEase);
	}

	private void ForceClose()
	{
		if (base.IsActivated)
		{
			base.Connectable.SendOnOnDeactivate();
		}
		base.IsActivated = false;
		if (base.m_Sequencer != null)
		{
			base.m_Sequencer.New();
			base.m_Sequencer.OnComplete(this.ForceCloseOnComplete);
		}
		unchecked
		{
			for (int i = 0; i < base.m_Properties.Length; i++)
			{
				Properties properties;
				properties = base.m_Properties[i];
				base.DoAnimation(properties, properties.OriginPosition, properties.OriginRotation, properties.TweenDuration, properties.TweenEase);
				if (properties.Blocker != null)
				{
					properties.Blocker.SetActive(value: true);
				}
				if (properties.ActionEvent != null)
				{
					properties.ActionEvent.gameObject.SetActive(value: true);
				}
			}
			for (int num = this.m_Loot.Count - 1; num >= 0; num--)
			{
				this.m_Loot[num].Dispose();
			}
			this.ClearLoot();
		}
	}

	private void ForceCloseOnComplete()
	{
		if (base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Cards >= 1)
		{
			this.Enable();
		}
	}

	private void HandleLootOnActivate(object sender, EventArgs e)
	{
		(sender as ActionEventController).OnActivate -= this.HandleLootOnActivate;
		unchecked
		{
			this.m_LootCount--;
			if (this.m_LootCount <= 0)
			{
				this.m_LootCount = 0;
				this.ForceClose();
			}
		}
	}

	private void ResetLoot()
	{
		this.ClearLoot();
		this.m_Loot = new List<ActionEventController>();
	}

	private void ClearLoot()
	{
		if (this.m_Loot != null)
		{
			this.m_Loot.Clear();
			this.m_Loot = null;
		}
		for (int i = 0; i < this.m_LootContainers.Length; i = unchecked(i + 1))
		{
			this.m_LootContainers[i].gameObject.SetActive(value: false);
		}
	}

	private void RemoveLootListeners()
	{
		if (this.m_Loot != null)
		{
			for (int i = 0; i < this.m_Loot.Count; i = unchecked(i + 1))
			{
				this.m_Loot[i].OnActivate -= this.HandleLootOnActivate;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveLootListeners();
		this.ClearLoot();
		base.OnDisposed();
	}
}
