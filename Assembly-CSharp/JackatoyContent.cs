using System;
using UnityEngine;

public class JackatoyContent : ActionEventContent<JackatoyContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable Content;
	}

	private Jackatoy m_Jackatoy;

	private bool m_IsComplete;

	protected override void OnInitialize()
	{
		this.m_Jackatoy = (Jackatoy)base.Connectable;
		if (!(this.m_Jackatoy.Data != null) || base.m_Properties == null || base.m_Properties.Length == 0)
		{
			return;
		}
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.JackatoyDirectory.ContainsKey(this.m_Jackatoy.Data.ID))
		{
			Properties properties;
			properties = base.m_Properties[0];
			properties.ActionEvent = this.CreateContent(properties.Content);
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
		this.m_IsComplete = true;
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.JackatoyDirectory.ContainsKey(this.m_Jackatoy.Data.ID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.JackatoyDirectory.Add(this.m_Jackatoy.Data.ID, DataObject<JackatoyID, JackatoyDataObject>.Create(this.m_Jackatoy.Data.ID));
		}
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.PLAYTHING);
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
		this.m_Jackatoy = null;
		base.OnDisposed();
	}
}
