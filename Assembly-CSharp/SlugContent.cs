using System;
using UnityEngine;

public class SlugContent : ActionEventContent<SlugContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private Slug m_Slug;

	protected override void OnInitialize()
	{
		this.m_Slug = (Slug)base.Connectable;
	}

	public void InitializeContent()
	{
		base.RemoveListeners();
		if (this.m_Slug.Data != null && base.m_Properties != null && base.m_Properties.Length != 0)
		{
			Properties properties;
			properties = base.m_Properties[0];
			properties.ActionEvent = this.CreateContent(this.m_Slug.Data.Interactable);
			if (properties.ActionEvent == null)
			{
				return;
			}
			properties.ActionEvent.transform.SetParent(base.transform);
			properties.ActionEvent.transform.localPosition = Vector3.zero;
			properties.ActionEvent.transform.localEulerAngles = Vector3.zero;
			properties.ActionEvent.transform.localScale = Vector3.one;
		}
		base.AddListeners();
		base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		base.Connectable.OnActivate += this.HandleConnectableOnActivate;
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
		base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		base.Connectable.Dispose();
	}

	protected override void OnActivate()
	{
		int value;
		value = this.m_Slug.Data.Value;
		string key;
		key = TextUtility.GetKey("NOTIFICATION_LOOT_SLUG");
		if (value > 1)
		{
			key = TextUtility.GetKey("NOTIFICATION_LOOT_SLUGS");
		}
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + key, "Icon/Collectables/Small/UIIcon_Slug", value);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddSlugs(value);
	}

	protected override void OnForceActivateComplete()
	{
		if (base.Connectable != null)
		{
			base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		}
		base.RemoveListeners();
		base.m_ActiveProperties.Clear();
		foreach (Properties value in base.m_PropertiesDictionary.Values)
		{
			base.m_ActiveProperties.Add(value);
		}
		this.Disable();
		base.IsActivated = true;
	}

	private Interactable CreateContent(Interactable interactable)
	{
		return GameManager.Instance.AssetManager.CreateAsset<Interactable>(interactable);
	}

	protected override void OnDisposed()
	{
		if (base.Connectable != null)
		{
			base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		}
		this.m_Slug = null;
		base.OnDisposed();
	}
}
