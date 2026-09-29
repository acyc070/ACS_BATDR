using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Memory : JMonoBehaviour
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("Memory Identifier")]
	[SerializeField]
	protected MemoryID m_MemoryID;

	[Header("Memory Content")]
	[SerializeField]
	private MemoryContent m_Content;

	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	public SectionID SectionID => this.m_SectionID;

	public MemoryID MemoryID => this.m_MemoryID;

	public MemoryContent Content { get; private set; }

	public event EventHandler OnActivated;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.ContainsKey(this.m_MemoryID))
		{
			this.Content = GameManager.Instance.AssetManager.CreateAsset<MemoryContent>(this.m_Content);
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
	}

	private void HandleActivatorOnActivated(object sender, EventArgs e)
	{
		JDebug.Log("Memory :: HandleActivatorOnActivated", this, JDebug.JDebugType.Collectables);
		this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.ContainsKey(this.m_MemoryID))
		{
			GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.Add(this.m_MemoryID, DataObject<MemoryID, MemoryDataObject>.Create(this.m_MemoryID));
		}
		this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		this.Content.Director.OnComplete += this.HandleDirectorOnComplete;
		this.InternalPreparePlay();
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowNotificationBox(TextUtility.GetKey("NOTIFICATION_MEMORY"));
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.Count < 2)
		{
			GameManager.Instance.ClearNotificationText();
			string text;
			text = " <color=#FEC97D>";
			string text2;
			text2 = "<color=#FFFFFF> ";
			string text3;
			text3 = TextUtility.GetKey("NOTIFICATION_MEMORY_MENU").ToUpper();
			string text4;
			text4 = TextUtility.GetKey("NOTIFICATION_MENU_01").ToUpper();
			string text5;
			text5 = TextUtility.GetKey("NOTIFICATION_MENU_02").ToUpper();
			string text6;
			text6 = TextUtility.GetKey("NOTIFICATION_MENU_03").ToUpper();
			string text7;
			text7 = TextUtility.GetKey("NOTIFICATION_MENU_04").ToUpper();
			string text8;
			text8 = text + text3 + text2 + text4 + text + text5 + text2 + text6 + text + text7 + " ";
			GameManager.Instance.ShowNotificationText(text8);
		}
		this.DirectorOnComplete();
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("Memory :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Collectables);
		this.CheckStatus();
	}

	private void InternalPreparePlay()
	{
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.Player.HideFirstPersonArms();
		GameManager.Instance.Player.CancelMovement();
		GameManager.Instance.Player.ResetAnimation();
		GameManager.Instance.Player.ResetRotation();
		GameManager.Instance.Player.SetState(State.Player.Cutscene);
		Vector3 vector;
		vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
		Sequence sequence;
		sequence = DOTween.Sequence();
		sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(this.Content.StartLocation.position + vector, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(this.Content.StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
		sequence.OnComplete(this.InternalPreparePlayOnComplete);
	}

	private void InternalPreparePlayOnComplete()
	{
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		this.Content.Director.Play();
		Array values;
		values = Enum.GetValues(typeof(MemoryID));
		List<MemoryID> list;
		list = new List<MemoryID>();
		for (int i = 0; i < values.Length; i = unchecked(i + 1))
		{
			object value;
			value = values.GetValue(i);
			if (!value.ToString().ToLower().Contains("template"))
			{
				list.Add((MemoryID)value);
			}
		}
		if (GameManager.Instance.GameData.CurrentSave.DataDirectories.MemoryDirectory.Count >= list.Count)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.SELF_DISCOVERY);
		}
		this.OnActivated.Send(this);
	}

	private void DirectorOnComplete()
	{
		this.RemoveListeners();
		GameManager.Instance.ShowCrosshair();
		this.OnComplete.Send(this);
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
				this.Content.Initialize();
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
