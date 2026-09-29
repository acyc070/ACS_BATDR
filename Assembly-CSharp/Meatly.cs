using System;
using System.Collections.Generic;
using UnityEngine;

public class Meatly : JMonoBehaviour
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("Meatly Identifier")]
	[SerializeField]
	protected MeatlyID m_MeatlyID;

	[Header("Meatly Content")]
	[SerializeField]
	private MeatlyContent m_Content;

	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Requirement Complete")]
	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	public MeatlyContent Content { get; private set; }

	public event EventHandler OnActivated;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.MeatlyDirectory.ContainsKey(this.m_MeatlyID))
		{
			this.Content = GameManager.Instance.AssetManager.CreateAsset<MeatlyContent>(this.m_Content);
			this.Content.transform.SetParent(base.transform);
			this.Content.transform.localPosition = Vector3.zero;
			this.Content.transform.localEulerAngles = Vector3.zero;
			this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
			this.Content.Activator.OnActivated += this.HandleActivatorOnActivated;
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			if (!this.CheckStatus())
			{
				GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
			}
		}
		else if (this.m_ActiveSetter != null)
		{
			this.m_ActiveSetter.SetActive(active: true);
		}
	}

	private void HandleActivatorOnActivated(object sender, EventArgs e)
	{
		JDebug.Log("Meatly :: HandleActivatorOnActivated", this, JDebug.JDebugType.Collectables);
		this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
		GameManager.Instance.GameData.CurrentSave.DataDirectories.MeatlyDirectory.Add(this.m_MeatlyID, DataObject<MeatlyID, MeatlyDataObject>.Create(this.m_MeatlyID));
		this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		this.Content.Director.OnComplete += this.HandleDirectorOnComplete;
		this.Content.Director.Play();
		Array values;
		values = Enum.GetValues(typeof(MeatlyID));
		List<MeatlyID> list;
		list = new List<MeatlyID>();
		for (int i = 0; i < values.Length; i = unchecked(i + 1))
		{
			object value;
			value = values.GetValue(i);
			if (!value.ToString().ToLower().Contains("template"))
			{
				list.Add((MeatlyID)value);
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.MeatlyDirectory.Count >= list.Count)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.FAMILIAR_FACES);
		}
		this.OnActivated.Send(this);
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		JDebug.Log("Meatly :: HandleDirectorOnComplete", this, JDebug.JDebugType.Collectables);
		this.RemoveListeners();
		this.OnComplete.Send(this);
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("Meatly :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Collectables);
		this.CheckStatus();
	}

	private bool CheckStatus()
	{
		bool flag;
		flag = true;
		if (this.m_Requirements != null)
		{
			flag = this.m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			if (this.Content != null)
			{
				this.Content.Activator.SetActive(active: true);
			}
			if (this.m_ActiveSetter != null)
			{
				this.m_ActiveSetter.SetActive(active: true);
			}
		}
		return flag;
	}

	private void RemoveListeners()
	{
		if (this.Content != null)
		{
			this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
			this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		}
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
