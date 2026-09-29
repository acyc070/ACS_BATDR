using System;
using UnityEngine;

public class GentBatteryContent : ActionEventContent<GentBatteryContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
	}

	private GentBattery m_GentBattery;

	protected override void OnInitialize()
	{
		this.m_GentBattery = (GentBattery)base.Connectable;
	}

	public void InitializeContent()
	{
		base.RemoveListeners();
		if (this.m_GentBattery.Data != null && base.m_Properties != null && base.m_Properties.Length != 0)
		{
			Properties properties;
			properties = base.m_Properties[0];
			properties.ActionEvent = this.CreateContent(this.m_GentBattery.Data.Interactable);
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
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_COLLECTED") + " " + TextUtility.GetKey("NOTIFICATION_LOOT_GENT_BATTERY"), "Icon/Collectables/Small/UIIcon_Battery", 1);
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.AddBattery();
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
		this.m_GentBattery = null;
		base.OnDisposed();
	}
}
