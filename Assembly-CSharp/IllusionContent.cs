using System;
using System.Collections.Generic;
using UnityEngine;

public class IllusionContent : ActionEventContent<IllusionContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable IllusionContent_Default;
	}

	private Illusion m_Illusion;

	private bool m_IsComplete;

	protected override void OnInitialize()
	{
		this.m_Illusion = (Illusion)base.Connectable;
		if (!(this.m_Illusion.Data != null) || base.m_Properties == null || base.m_Properties.Length == 0)
		{
			return;
		}
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.ContainsKey(this.m_Illusion.Data.ID))
		{
			Properties properties;
			properties = base.m_Properties[0];
			properties.ActionEvent = this.CreateContent(properties.IllusionContent_Default);
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
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.ContainsKey(this.m_Illusion.Data.ID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.Add(this.m_Illusion.Data.ID, DataObject<IllusionID, IllusionDataObject>.Create(this.m_Illusion.Data.ID));
		}
		Array values;
		values = Enum.GetValues(typeof(IllusionID));
		List<IllusionID> list;
		list = new List<IllusionID>();
		for (int i = 0; i < values.Length; i = unchecked(i + 1))
		{
			object value;
			value = values.GetValue(i);
			if (!value.ToString().ToLower().Contains("template"))
			{
				list.Add((IllusionID)value);
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.IllusionDirectory.Count >= list.Count)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_INSANE_READER);
		}
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[7]);
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
		this.m_Illusion = null;
		base.OnDisposed();
	}
}
