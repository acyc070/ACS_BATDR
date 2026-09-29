using System;
using UnityEngine;

public class CutsceneActivator : JMonoBehaviour
{
	[SerializeField]
	private CutsceneActivateType m_ActivateType;

	[SerializeField]
	private ActionEvent m_Action;

	public CutsceneActivateType ActivateType => this.m_ActivateType;

	public ActionEvent Action => this.m_Action;

	public bool IsActive { get; private set; }

	public event EventHandler OnActivated;

	public override void Awake()
	{
		if (this.m_ActivateType == CutsceneActivateType.OnAwake)
		{
			this.InternalPlay();
		}
	}

	public override void Start()
	{
		if (this.m_ActivateType == CutsceneActivateType.OnStart)
		{
			this.InternalPlay();
		}
		else if (this.m_ActivateType == CutsceneActivateType.EventAction && this.m_Action != null)
		{
			this.m_Action.OnInteract += this.HandleCutsceneOnInteract;
		}
	}

	public void Play()
	{
		if (this.m_ActivateType == CutsceneActivateType.Callback)
		{
			this.InternalPlay();
		}
	}

	public void ForcePlay()
	{
		this.InternalPlay();
	}

	private void InternalPlay()
	{
		JDebug.Log("CutsceneActivator :: InternalPlay", this, JDebug.JDebugType.Cutscene);
		this.OnActivated.Send(this);
	}

	public void SetActive(bool active)
	{
		this.InternalSetActive(active);
	}

	private void InternalSetActive(bool active)
	{
		this.IsActive = active;
		this.m_Action?.SetActive(this.IsActive);
	}

	private void HandleCutsceneOnInteract(object sender, EventArgs e)
	{
		JDebug.Log("CutsceneActivator :: HandleCutsceneOnInteract", this, JDebug.JDebugType.Cutscene);
		if (this.IsActive)
		{
			this.m_Action.OnInteract -= this.HandleCutsceneOnInteract;
			this.m_Action.SetActive(active: false);
			this.InternalPlay();
		}
	}

	public void ResetInteraction()
	{
		if (this.m_Action != null)
		{
			this.m_Action.OnInteract -= this.HandleCutsceneOnInteract;
			this.m_Action.OnInteract += this.HandleCutsceneOnInteract;
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_Action != null)
		{
			this.m_Action.OnInteract -= this.HandleCutsceneOnInteract;
		}
		this.OnActivated = null;
		base.OnDisposed();
	}
}
