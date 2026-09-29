using System;
using UnityEngine;

public class GentSchematicContent : ActionEventContent<GentSchematicContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable GentSchematicContent_Default;
	}

	private GentSchematic m_GentSchematic;

	private bool m_IsComplete;

	protected override void OnInitialize()
	{
		this.m_GentSchematic = (GentSchematic)base.Connectable;
		if (!(this.m_GentSchematic.Data != null) || base.m_Properties == null || base.m_Properties.Length == 0)
		{
			return;
		}
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.ContainsKey(this.m_GentSchematic.Data.ID))
		{
			Properties properties;
			properties = base.m_Properties[0];
			properties.ActionEvent = this.CreateContent(properties.GentSchematicContent_Default);
			if (!(properties.ActionEvent == null))
			{
				properties.ActionEvent.transform.SetParent(base.transform);
				properties.ActionEvent.transform.localPosition = Vector3.zero;
				properties.ActionEvent.transform.localEulerAngles = Vector3.zero;
				properties.ActionEvent.transform.localScale = Vector3.one;
			}
		}
		else
		{
			this.m_IsComplete = true;
		}
	}

	protected override void OnInitialized()
	{
		if (!this.m_IsComplete)
		{
			base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
			base.Connectable.OnActivate += this.HandleConnectableOnActivate;
		}
	}

	private void HandleConnectableOnActivate(object sender, EventArgs e)
	{
		base.Connectable.OnActivate -= this.HandleConnectableOnActivate;
		base.Connectable.Dispose();
	}

	protected override void OnActivate()
	{
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_SCHEMATIC"));
		this.m_IsComplete = true;
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.ContainsKey(this.m_GentSchematic.Data.ID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.GentSchematicDirectory.Add(this.m_GentSchematic.Data.ID, DataObject<GentSchematicID, GentSchematicDataObject>.Create(this.m_GentSchematic.Data.ID));
		}
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
		this.m_GentSchematic = null;
		base.OnDisposed();
	}
}
