using System;
using DG.Tweening;
using UnityEngine;

public class SubwayCar : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Cutscene Parents")]
	[SerializeField]
	private Transform m_CutsceneParent;

	[SerializeField]
	private Transform m_Cutscenes;

	[Header("Section Controllers")]
	[SerializeField]
	private Cutscene m_CutsceneStationArrival;

	[SerializeField]
	private Door m_Door;

	[SerializeField]
	private Door m_DoorCar;

	[Header("Subway")]
	[SerializeField]
	private Transform m_SubwayCarParent;

	[SerializeField]
	private Transform m_Subway;

	[SerializeField]
	private Transform m_SubwayStart;

	[SerializeField]
	private Transform m_SubwayEnd;

	[SerializeField]
	private Transform m_SubwayLeave;

	[Header("Tunnel")]
	[SerializeField]
	private Transform m_Tunnel;

	[SerializeField]
	private Transform m_TunnelStart;

	[SerializeField]
	private Transform m_TunnelEnd;

	[Header("DOTween Animations")]
	[SerializeField]
	private DOTweenAnimation[] m_Handles;

	private Sequence m_TunnelSequence;

	private Sequence m_Sequence;

	public event EventHandler OnLeave;

	public override void Start()
	{
		if (!this.CheckStatus())
		{
			for (int i = 0; i < this.m_Handles.Length; i = unchecked(i + 1))
			{
				this.m_Handles[i].DOPlay();
			}
			this.m_Cutscenes.SetParent(this.m_CutsceneParent);
			Cutscene[] componentsInChildren;
			componentsInChildren = this.m_Cutscenes.GetComponentsInChildren<Cutscene>(includeInactive: true);
			foreach (Cutscene cutscene in componentsInChildren)
			{
				if (cutscene != null)
				{
					cutscene.Initialize();
				}
			}
			GameManager.Instance.Player.transform.SetParent(this.m_SubwayCarParent);
			this.m_CutsceneStationArrival.OnActivated -= this.HandleRetreatInitializeOnInteract;
			this.m_CutsceneStationArrival.OnActivated += this.HandleRetreatInitializeOnInteract;
			this.Loop();
		}
		else
		{
			base.Dispose();
		}
	}

	private bool CheckStatus()
	{
		bool result;
		result = true;
		if (this.m_Requirements != null)
		{
			result = this.m_Requirements.IsComplete();
		}
		return result;
	}

	private void Loop()
	{
		CameraEffects.ShakeRotation(1.5f, 0.5f, 10, 90f, fadeOut: false);
		this.ResetTunnelSequence();
		this.m_TunnelSequence.InsertCallback(0f, () =>
		{
			this.m_Tunnel.position = this.m_TunnelStart.position;
		});
		this.m_TunnelSequence.Insert(0f, this.m_Tunnel.DOMove(this.m_TunnelEnd.position, 1.5f).SetEase(Ease.Linear));
		this.m_TunnelSequence.OnComplete(this.Loop);
	}

	private void HandleRetreatInitializeOnInteract(object sender, EventArgs e)
	{
		this.m_CutsceneStationArrival.OnActivated -= this.HandleRetreatInitializeOnInteract;
		float num;
		num = 0.5f;
		this.ResetSequence();
		this.m_Sequence.InsertCallback(num, () =>
		{
			this.m_Subway.position = this.m_SubwayStart.position;
			this.KillTunnelSequence();
		});
		this.m_Sequence.Insert(num, this.m_Subway.DOMove(this.m_SubwayEnd.position, 4f).SetEase(Ease.OutQuad));
		this.m_Sequence.InsertCallback(num + 4f, this.RetreatInitializeOnComplete);
	}

	private void RetreatInitializeOnComplete()
	{
		for (int i = 0; i < this.m_Handles.Length; i = unchecked(i + 1))
		{
			this.m_Handles[i].DOKill();
		}
		this.m_DoorCar.Data.SetStatus(DoorStatus.Closed);
		this.m_DoorCar.Content.ForceDeactivateComplete();
		this.m_DoorCar.Content.Enable();
		this.m_Door.Content.ForceActivate();
		CameraEffects.ShakeRotation(0.5f, 5f);
		GameManager.Instance.Player.transform.SetParent(null);
		global::UnityEngine.Object.DontDestroyOnLoad(GameManager.Instance.Player.gameObject);
	}

	public void Leave()
	{
		if (GameManager.Instance.Player.transform.parent != null)
		{
			GameManager.Instance.Player.transform.SetParent(null);
			global::UnityEngine.Object.DontDestroyOnLoad(GameManager.Instance.Player.gameObject);
		}
		this.m_Door.Content.ForceDeactivate();
		this.ResetSequence();
		this.m_Sequence.Insert(1f, this.m_Subway.DOMove(this.m_SubwayLeave.position, 4f).SetEase(Ease.InSine));
		this.m_Sequence.OnComplete(this.Dispose);
		this.OnLeave.Send(this);
	}

	private void ResetTunnelSequence()
	{
		this.KillTunnelSequence();
		this.m_TunnelSequence = DOTween.Sequence();
	}

	private void KillTunnelSequence()
	{
		if (this.m_TunnelSequence != null)
		{
			this.m_TunnelSequence.Kill();
			this.m_TunnelSequence = null;
		}
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.OnLeave = null;
		this.KillTunnelSequence();
		this.KillSequence();
		for (int i = 0; i < this.m_Handles.Length; i = unchecked(i + 1))
		{
			this.m_Handles[i].DOKill();
		}
		if (this.m_CutsceneStationArrival != null)
		{
			this.m_CutsceneStationArrival.OnActivated -= this.HandleRetreatInitializeOnInteract;
		}
		base.OnDisposed();
	}
}
