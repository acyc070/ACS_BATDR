using System;
using System.Collections.Generic;
using UnityEngine;

public class MemoContent : ActionEventContent<MemoContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable MemoContent_LostOne;

		public Interactable MemoContent_JDS;

		public Interactable MemoContent_Gent;
	}

	private Memo m_Memo;

	private UIMemoDataVO m_DataVO;

	protected override void OnInitialize()
	{
		this.m_Memo = (Memo)base.Connectable;
		if (this.m_Memo.Data != null && base.m_Properties != null && base.m_Properties.Length != 0 && !GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.ContainsKey(this.m_Memo.Data.ID))
		{
			Properties properties;
			properties = base.m_Properties[0];
			properties.ActionEvent = this.GenerateMemo(properties);
			if (!(properties.ActionEvent == null))
			{
				properties.ActionEvent.transform.SetParent(base.transform);
				properties.ActionEvent.transform.localPosition = Vector3.zero;
				properties.ActionEvent.transform.localEulerAngles = Vector3.zero;
				properties.ActionEvent.transform.localScale = Vector3.one;
			}
		}
	}

	private Interactable GenerateMemo(Properties properties)
	{
		return this.m_Memo.Data.MemoType switch
		{
			MemoType.LostOne => this.CreateMemo(properties.MemoContent_LostOne), 
			MemoType.JDS => this.CreateMemo(properties.MemoContent_JDS), 
			MemoType.GENT => this.CreateMemo(properties.MemoContent_Gent), 
			_ => null, 
		};
	}

	private Interactable CreateMemo(Interactable interactable)
	{
		return GameManager.Instance.AssetManager.CreateAsset<Interactable>(interactable);
	}

	protected override void OnActivate()
	{
		GameManager.Instance.ShowInteraction(TextUtility.GetKey(InteractionType.INTERACTION_TAKE.ToString()));
		this.m_DataVO = new UIMemoDataVO(this.m_Memo, this.m_Memo.Data.MemoType, TextUtility.GetKey(this.m_Memo.Data.ID.ToString()));
		GameManager.Instance.ShowMemo(this.m_DataVO);
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.ContainsKey(this.m_Memo.Data.ID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.Add(this.m_Memo.Data.ID, DataObject<MemoID, MemoDataObject>.Create(this.m_Memo.Data.ID));
		}
		Array values;
		values = Enum.GetValues(typeof(MemoID));
		List<MemoID> list;
		list = new List<MemoID>();
		for (int i = 0; i < values.Length; i = unchecked(i + 1))
		{
			object value;
			value = values.GetValue(i);
			if (!value.ToString().ToLower().Contains("template"))
			{
				list.Add((MemoID)value);
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoDirectory.Count >= list.Count)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.WRITTEN_IN_INK);
		}
	}

	protected override void OnDisposed()
	{
		this.m_Memo = null;
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
