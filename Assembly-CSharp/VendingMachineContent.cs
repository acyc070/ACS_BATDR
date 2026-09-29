using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class VendingMachineContent : ActionEventContent<VendingMachineContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Transform Drawer;

		public GameObject Blocker;

		public float TweenDuration = 0.4f;

		public Ease TweenEase = Ease.OutSine;

		public LightFixture LightFixture;
	}

	private bool m_IsActivated;

	private int m_FoodCount;

	private List<Food> m_Foods = new List<Food>();

	private void Update()
	{
		if (!this.m_IsActivated && !(GameManager.Instance.Player == null) && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			if (base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs >= 5)
			{
				this.Enable();
			}
			else if (!base.IsInactive && GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs < 5)
			{
				this.Disable();
				this.DisableLighting();
			}
		}
	}

	protected override void InternalEnable()
	{
		this.m_IsActivated = false;
		this.EnableLighting();
	}

	private void EnableLighting()
	{
		if (base.m_Properties.Length != 0)
		{
			Properties properties;
			properties = base.m_Properties[0];
			if (properties.LightFixture != null)
			{
				properties.LightFixture.SetEmission(1f);
			}
		}
	}

	private void DisableLighting()
	{
		if (base.m_Properties.Length != 0)
		{
			Properties properties;
			properties = base.m_Properties[0];
			if (properties.LightFixture != null)
			{
				properties.LightFixture.SetEmission(0f);
			}
		}
	}

	protected override void OnActivate()
	{
		this.m_IsActivated = true;
		Properties properties;
		properties = base.m_Properties[0];
		this.RemoveFoodListeners();
		this.ResetFoods();
		Food[] componentsInChildren;
		componentsInChildren = base.gameObject.GetComponentsInChildren<Food>(includeInactive: true);
		int num;
		num = global::UnityEngine.Random.Range(0, componentsInChildren.Length);
		unchecked
		{
			int num2;
			num2 = num + 1;
			if (num2 >= componentsInChildren.Length)
			{
				num2 = 0;
			}
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Food food;
				food = componentsInChildren[i];
				food.gameObject.SetActive(value: false);
				if (i == num)
				{
					this.GenerateFood(properties, food);
				}
				else if (i == num2 && GameManager.Instance.Player.Health < 7.5f)
				{
					this.GenerateFood(properties, food);
				}
				else if (GameManager.Instance.Player.Health < 5f && global::UnityEngine.Random.value < 0.5f)
				{
					this.GenerateFood(properties, food);
				}
			}
			GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_LOOT_SPENT") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_SLUGS"), "Icon/Collectables/Small/UIIcon_Slug", 5);
			GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.RemoveSlugs(5);
			this.Disable();
			if (base.m_Sequencer != null)
			{
				base.m_Sequencer.New();
			}
			base.DoAnimation(properties, properties.ActiveLocation.localPosition, properties.ActiveLocation.localEulerAngles, properties.TweenDuration, properties.TweenEase, 0.5f);
		}
	}

	private void GenerateFood(Properties properties, Food foodPrefab)
	{
		Food food;
		food = GameManager.Instance.AssetManager.CreateAsset<Food>(foodPrefab);
		unchecked
		{
			if (!this.m_Foods.Contains(food))
			{
				this.m_Foods.Add(food);
				food.transform.SetParent(properties.Drawer);
				food.transform.position = foodPrefab.transform.position;
				food.transform.eulerAngles = foodPrefab.transform.eulerAngles;
				food.Initialize();
				(food.Content as FoodContent).InitializeContent();
				this.m_FoodCount++;
				food.gameObject.SetActive(value: true);
				food.OnActivate -= this.HandleFoodOnActivate;
				food.OnActivate += this.HandleFoodOnActivate;
			}
			else
			{
				food.Dispose();
			}
		}
	}

	protected override void OnActivateComplete()
	{
		if (base.m_Properties.Length != 0)
		{
			Properties properties;
			properties = base.m_Properties[0];
			if (properties.Blocker != null)
			{
				properties.Blocker.SetActive(value: false);
			}
		}
	}

	protected override void OnDeactivate()
	{
		Properties properties;
		properties = base.m_Properties[0];
		base.DoAnimation(properties, Vector3.zero, Vector3.zero, properties.TweenDuration, properties.TweenEase);
		if (properties.Blocker != null)
		{
			properties.Blocker.SetActive(value: true);
		}
	}

	protected override void OnDeactivateComplete()
	{
		this.m_IsActivated = false;
		this.ResetActionEvents();
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.Slugs < 5)
		{
			this.DisableLighting();
		}
	}

	private void HandleFoodOnActivate(object sender, EventArgs e)
	{
		(sender as Food).OnActivate -= this.HandleFoodOnActivate;
		unchecked
		{
			this.m_FoodCount--;
			if (this.m_FoodCount <= 0)
			{
				_ = base.m_Properties[0];
				this.m_FoodCount = 0;
				if (base.m_Sequencer != null)
				{
					base.m_Sequencer.New();
				}
				this.ForceDeactivate();
			}
		}
	}

	private void ResetFoods()
	{
		this.ClearFoods();
		this.m_Foods = new List<Food>();
	}

	private void ClearFoods()
	{
		if (this.m_Foods != null)
		{
			this.m_Foods.Clear();
			this.m_Foods = null;
		}
	}

	private void RemoveFoodListeners()
	{
		if (this.m_Foods != null)
		{
			for (int i = 0; i < this.m_Foods.Count; i = unchecked(i + 1))
			{
				this.m_Foods[i].OnActivate -= this.HandleFoodOnActivate;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveFoodListeners();
		this.ClearFoods();
		base.OnDisposed();
	}
}
