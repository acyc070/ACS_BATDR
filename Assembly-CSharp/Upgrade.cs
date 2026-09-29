using System;
using DG.Tweening;
using UnityEngine;

public class Upgrade : JMonoBehaviour
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("Upgrade Identifier")]
	[SerializeField]
	protected UpgradeID m_UpgradeID;

	[Header("Upgrade Content")]
	[SerializeField]
	private UpgradeContent m_Content;

	[Header("Upgrade Complete")]
	[SerializeField]
	private GameObject m_CompleteContent;

	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	public SectionID SectionID => this.m_SectionID;

	public UpgradeID UpgradeID => this.m_UpgradeID;

	public UpgradeContent Content { get; private set; }

	public event EventHandler OnActivated;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		if (!GameManager.Instance.GameData.CurrentSave.DataDirectories.UpgradeDirectory.ContainsKey(this.m_UpgradeID))
		{
			this.AddContent();
		}
		else
		{
			this.AddCompleteContent();
		}
	}

	private void AddContent()
	{
		this.Content = GameManager.Instance.AssetManager.CreateAsset<UpgradeContent>(this.m_Content);
		this.Content.transform.SetParent(base.transform);
		this.Content.transform.localPosition = Vector3.zero;
		this.Content.transform.localEulerAngles = Vector3.zero;
		this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
		this.Content.Activator.OnActivated += this.HandleActivatorOnActivated;
		this.SetActive(active: true);
	}

	private void AddCompleteContent()
	{
		if (this.m_CompleteContent != null)
		{
			CutsceneComplete cutsceneComplete;
			cutsceneComplete = GameManager.Instance.AssetManager.CreateAsset<CutsceneComplete>(this.m_CompleteContent.GetComponent<CutsceneComplete>());
			cutsceneComplete.transform.SetParent(base.transform);
			cutsceneComplete.transform.localPosition = Vector3.zero;
			cutsceneComplete.transform.localEulerAngles = Vector3.zero;
		}
	}

	public void Play()
	{
		this.Content.Activator.Play();
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
		this.OnActivated.Send(this);
	}

	private void DirectorOnComplete()
	{
		this.RemoveListeners();
		GameManager.Instance.ShowCrosshair();
		this.OnComplete.Send(this);
	}

	public void ForceComplete()
	{
		this.DirectorOnComplete();
	}

	public void SetActive(bool active)
	{
		this.Content.Activator.SetActive(active);
	}

	private void HandleActivatorOnActivated(object sender, EventArgs e)
	{
		Debug.Log("Upgrade :: HandleActivatorOnActivated", this);
		this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
		GameManager.Instance.GameData.CurrentSave.DataDirectories.UpgradeDirectory.Add(this.m_UpgradeID, DataObject<UpgradeID, UpgradeDataObject>.Create(this.m_UpgradeID));
		this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		this.Content.Director.OnComplete += this.HandleDirectorOnComplete;
		this.InternalPreparePlay();
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowUpgrades();
		this.DirectorOnComplete();
	}

	private void RemoveListeners()
	{
		if (this.Content != null)
		{
			this.Content.Activator.OnActivated -= this.HandleActivatorOnActivated;
			this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
