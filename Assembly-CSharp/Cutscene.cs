using System;
using DG.Tweening;
using UnityEngine;

public class Cutscene : DataMonoBehaviour<CutsceneID, CutsceneDataObject>
{
	[Header("Section Identifier")]
	[SerializeField]
	protected SectionID m_SectionID;

	[Header("Cutscene Identifier")]
	[SerializeField]
	protected CutsceneID m_CutsceneID;

	[Header("Cutscene Content")]
	[SerializeField]
	private CutsceneContent m_Content;

	[Header("Cutscene Complete Content")]
	[SerializeField]
	private GameObject m_CompleteContent;

	[Header("Options")]
	[SerializeField]
	private bool m_ResetOnComplete;

	[SerializeField]
	private bool m_KillAudioLogs = true;

	public SectionID SectionID => this.m_SectionID;

	protected override CutsceneID m_ID => this.m_CutsceneID;

	public CutsceneID CutsceneID => this.m_CutsceneID;

	public bool ResetOnComplete => this.m_ResetOnComplete;

	public bool KillAudioLogs => this.m_KillAudioLogs;

	public CutsceneContent Content { get; private set; }

	public event EventHandler OnActivated;

	public event EventHandler OnUnityEvent;

	public event EventHandler OnComplete;

	protected override void InternalInitialize()
	{
		if (!(this.m_Content != null))
		{
			return;
		}
		CutsceneDataObject cutsceneDataObject;
		cutsceneDataObject = (CutsceneDataObject)GameManager.Instance.GameData.CurrentSave.GetData<CutsceneID, CutsceneDataObject>(this.m_SectionID, this.m_CutsceneID);
		if (cutsceneDataObject != null)
		{
			if (cutsceneDataObject.Status != CutsceneStatus.Complete || this.m_ResetOnComplete)
			{
				this.AddContent();
			}
			else
			{
				this.AddCompleteContent();
			}
		}
		else
		{
			cutsceneDataObject = DataObject<CutsceneID, CutsceneDataObject>.Create(this.m_CutsceneID);
			cutsceneDataObject.SetStatus(CutsceneStatus.None);
			GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, cutsceneDataObject);
			this.AddContent();
		}
	}

	public void AddContent()
	{
		this.Content = GameManager.Instance.AssetManager.CreateAsset<CutsceneContent>(this.m_Content);
		this.Content.transform.SetParent(base.transform);
		this.Content.transform.localPosition = Vector3.zero;
		this.Content.transform.localEulerAngles = Vector3.zero;
		this.Content.OnForceComplete -= this.HandleCutsceneContentOnForceComplete;
		this.Content.OnForceComplete += this.HandleCutsceneContentOnForceComplete;
		this.Content.Activator.OnActivated -= this.HandleCutsceneActivatorOnActivated;
		this.Content.Activator.OnActivated += this.HandleCutsceneActivatorOnActivated;
	}

	private void HandleCutsceneContentOnForceComplete(object sender, EventArgs e)
	{
		this.Content.OnForceComplete -= this.HandleCutsceneContentOnForceComplete;
		this.ForceComplete();
	}

	public void AddCompleteContent()
	{
		if (this.m_CompleteContent != null)
		{
			CutsceneComplete cutsceneComplete;
			cutsceneComplete = GameManager.Instance.AssetManager.CreateAsset<CutsceneComplete>(this.m_CompleteContent.GetComponent<CutsceneComplete>());
			cutsceneComplete.transform.SetParent(base.transform);
			cutsceneComplete.transform.localPosition = Vector3.zero;
			cutsceneComplete.transform.localEulerAngles = Vector3.zero;
			cutsceneComplete.Initialize();
		}
	}

	private void InternalPreparePlay()
	{
		if (this.m_KillAudioLogs)
		{
			GameManager.Instance.KillAudioLog();
		}
		GameManager.Instance.HideCrosshair();
		if (GameManager.Instance.BeastBendy == null)
		{
			GameManager.Instance.Player.HideFirstPersonArms();
			GameManager.Instance.Player.CancelMovement();
			GameManager.Instance.Player.ResetAnimation();
			GameManager.Instance.Player.ResetRotation();
			GameManager.Instance.Player.SetState(State.Player.Cutscene);
		}
		Vector3 vector;
		vector = ((GameManager.Instance.BeastBendy == null) ? (Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth) : Vector3.zero);
		Sequence sequence;
		sequence = DOTween.Sequence();
		Transform transform;
		transform = null;
		transform = this.Content.StartLocation;
		if (transform != null)
		{
			if (GameManager.Instance.BeastBendy == null)
			{
				sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(transform.position + vector, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
				sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(transform.eulerAngles, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
			}
			else
			{
				sequence.Insert(0f, GameManager.Instance.BeastBendy.transform.DOMove(transform.position, 0.5f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
				sequence.Insert(0f, GameManager.Instance.BeastBendy.transform.DORotate(transform.eulerAngles, 0.5f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
			}
		}
		sequence.OnComplete(this.InternalPreparePlayOnComplete);
	}

	private void InternalPreparePlayOnComplete()
	{
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		this.InternalPlay();
	}

	private void InternalPlay()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Interaction.ResetInteraction();
		}
		this.Content.Director.Play();
		this.OnActivated.Send(this);
	}

	public void ForcePlay(double timeline)
	{
		this.Content.Activator.OnActivated -= this.HandleCutsceneActivatorOnActivated;
		if (!this.Content.CompleteOnPlay)
		{
			this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
			this.Content.Director.OnComplete += this.HandleDirectorOnComplete;
		}
		else
		{
			this.DirectorOnComplete();
		}
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Interaction.ResetInteraction();
		}
		this.Content.Director.Play();
		this.Content.Director.PlayableDirector.time = timeline;
		this.OnActivated.Send(this);
	}

	public void ForceComplete()
	{
		if (!this.m_ResetOnComplete && this.Content != null)
		{
			this.Content.Activator.OnActivated -= this.HandleCutsceneActivatorOnActivated;
			this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		}
		this.SendOnComplete();
	}

	private void HandleCutsceneActivatorOnActivated(object sender, EventArgs e)
	{
		JDebug.Log("Cutscene :: HandleCutsceneActivatorOnActivated", this, JDebug.JDebugType.Cutscene);
		this.Content.Activator.OnActivated -= this.HandleCutsceneActivatorOnActivated;
		if (!this.Content.CompleteOnPlay)
		{
			this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
			this.Content.Director.OnComplete += this.HandleDirectorOnComplete;
		}
		else
		{
			this.DirectorOnComplete();
		}
		if (this.Content.StartLocation != null)
		{
			this.InternalPreparePlay();
		}
		else
		{
			this.InternalPlay();
		}
	}

	private void HandleDirectorOnComplete(object sender, EventArgs e)
	{
		this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		this.DirectorOnComplete();
	}

	private void DirectorOnComplete()
	{
		if (this.m_ResetOnComplete)
		{
			this.Content.Director.ResetDirector();
			this.Content.Activator.Action?.ResetAction();
			this.SetActive(active: true);
			this.Content.Activator.OnActivated -= this.HandleCutsceneActivatorOnActivated;
			this.Content.Activator.OnActivated += this.HandleCutsceneActivatorOnActivated;
			this.Content.Activator.ResetInteraction();
		}
		this.SendOnComplete();
	}

	protected void SendOnComplete()
	{
		this.OnComplete.Send(this);
		base.Load(null);
	}

	public void SetActive(bool active)
	{
		if (this.Content != null)
		{
			this.Content.Activator.SetActive(active);
		}
	}

	public void Play()
	{
		if (this.Content != null)
		{
			this.Content.Activator.Play();
		}
	}

	public void SendOnUnityEvent()
	{
		this.OnUnityEvent.Send(this);
	}

	protected override void OnDisposed()
	{
		if (this.Content != null)
		{
			this.Content.OnForceComplete -= this.HandleCutsceneContentOnForceComplete;
			this.Content.Activator.OnActivated -= this.HandleCutsceneActivatorOnActivated;
			this.Content.Director.OnComplete -= this.HandleDirectorOnComplete;
		}
		base.OnDisposed();
	}
}
