using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SubwayCarTravel : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Event Trigger")]
	[SerializeField]
	private EventTrigger m_SubwayCarEnter;

	[SerializeField]
	private EventTrigger m_SubwayCarDeath;

	[Header("Section Controllers")]
	[SerializeField]
	private Door m_Door;

	[SerializeField]
	private Door m_DoorCar;

	[Header("Subway")]
	[SerializeField]
	private EventTrigger m_SubwayEventTrigger;

	[SerializeField]
	private Transform m_SubwayCarParent;

	[SerializeField]
	private Transform m_SubwayCarPlayerParent;

	[SerializeField]
	private Transform m_Subway;

	[SerializeField]
	private Transform m_SubwayStart;

	[SerializeField]
	private Transform m_SubwayEnd;

	[SerializeField]
	private Transform m_SubwayLeave;

	[SerializeField]
	private Transform m_SubwayTravel;

	[Header("Tunnel")]
	[SerializeField]
	private Transform m_Tunnel;

	[SerializeField]
	private Transform m_TunnelStart;

	[SerializeField]
	private Transform m_TunnelEnd;

	[SerializeField]
	private GameObject m_TunnelCollider;

	[Header("City")]
	[SerializeField]
	private EventTrigger m_CityEnter;

	[SerializeField]
	private Transform m_CityStart;

	[SerializeField]
	private Transform m_CityEnd;

	[SerializeField]
	private Transform m_CityLeave;

	[Header("Section")]
	[SerializeField]
	private SectionID m_CurrentSectionID;

	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private SectionID[] m_UninitializedSections;

	[Header("DOTween Animations")]
	[SerializeField]
	private DOTweenAnimation[] m_Handles;

	private Section m_Section;

	private Sequence m_TunnelSequence;

	private Sequence m_Sequence;

	private bool m_IsActive;

	private bool m_HasArrived;

	private bool m_IsTraveled;

	private bool m_CanReturn;

	private int m_LoopIndex;

	private int m_LoopMax = 10;

	private float m_Timer;

	private const float m_TimerMax = 15f;

	private const float m_TimerReturnMax = 30f;

	public event EventHandler OnArrive;

	public event EventHandler OnLeave;

	public event EventHandler OnTravelStart;

	public event EventHandler OnTravelEnd;

	public event EventHandler OnArriveVO;

	public event EventHandler OnLeaveVO;

	public event EventHandler OnHorn;

	public override void Start()
	{
		this.m_Section = base.GetComponentInParent<Section>(includeInactive: true);
		this.m_Subway.gameObject.SetActive(value: false);
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	private void Update()
	{
		if (!(this.m_Section == null) && this.m_Section.IsInitialized && this.m_Section.IsActive && this.m_IsActive && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			float num;
			num = (this.m_CanReturn ? 30f : 15f);
			if (!this.m_HasArrived && this.m_Timer >= num)
			{
				this.Arrive();
			}
			else if (this.m_HasArrived && this.m_CanReturn && this.m_Timer >= 15f)
			{
				this.Return();
			}
			else
			{
				this.m_Timer += Time.deltaTime;
			}
		}
	}

	private void SendOnHorn()
	{
		this.OnHorn.Send(this);
	}

	private void SendOnArrive()
	{
		this.OnArrive.Send(this);
	}

	private void Return()
	{
		if (!(GameManager.Instance.Player != null) || !(Vector3.Distance(GameManager.Instance.Player.transform.position, this.m_Door.transform.position) < 10f))
		{
			this.m_IsActive = false;
			this.m_HasArrived = false;
			this.m_Timer = 0f;
			this.m_SubwayCarEnter.SetActive(active: false);
			this.m_SubwayCarEnter.OnEnter -= this.HandleSubwayCarEnterOnEnter;
			this.m_SubwayCarDeath.OnEnter -= this.HandleSubwayCarDeathOnEnter;
			this.m_SubwayCarDeath.OnEnter += this.HandleSubwayCarDeathOnEnter;
			this.m_SubwayCarDeath.SetActive(active: true);
			this.m_Door.Content.ForceDeactivate();
			this.ResetSequence();
			this.m_Sequence.Insert(1f, this.m_Subway.DOMove(this.m_SubwayLeave.position, 3f).SetEase(Ease.InSine).SetUpdate(UpdateType.Fixed));
			this.m_Sequence.OnComplete(this.ReturnComplete);
			this.OnLeave.Send(this);
		}
	}

	private void ReturnComplete()
	{
		this.m_SubwayCarDeath.OnEnter -= this.HandleSubwayCarDeathOnEnter;
		this.m_Subway.gameObject.SetActive(value: false);
		this.m_IsActive = true;
	}

	private void Arrive()
	{
		this.m_IsActive = false;
		this.m_Subway.gameObject.SetActive(value: true);
		this.m_SubwayCarEnter.SetActive(active: false);
		this.m_SubwayCarDeath.OnEnter -= this.HandleSubwayCarDeathOnEnter;
		this.m_SubwayCarDeath.OnEnter += this.HandleSubwayCarDeathOnEnter;
		this.m_SubwayCarDeath.SetActive(active: true);
		float num;
		num = 5f;
		this.ResetSequence();
		this.m_Sequence.InsertCallback(num, this.SendOnHorn);
		num += 3f;
		this.m_Sequence.InsertCallback(num, this.SendOnArrive);
		num += 2.5f;
		this.m_Sequence.InsertCallback(num, this.EnableSubwayCar);
		this.m_Sequence.Insert(num, this.m_Subway.DOMove(this.m_SubwayEnd.position, 4f).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Fixed));
		this.m_Sequence.InsertCallback(num + 4f, this.RetreatInitializeOnComplete);
	}

	private void EnableSubwayCar()
	{
		this.m_Subway.position = this.m_SubwayStart.position;
		this.KillTunnelSequence();
	}

	private void HandleSubwayCarDeathOnEnter(object sender, EventArgs e)
	{
		this.m_SubwayCarDeath.OnEnter -= this.HandleSubwayCarDeathOnEnter;
		GameManager.Instance.Player.ForceDeath();
		this.OnHorn.Send(this);
	}

	private void RetreatInitializeOnComplete()
	{
		this.m_DoorCar.Data.SetStatus(DoorStatus.Closed);
		this.m_DoorCar.Content.ForceDeactivateComplete();
		this.m_DoorCar.Content.Enable();
		this.m_Door.Content.ForceActivate();
		this.m_SubwayCarDeath.SetActive(active: false);
		this.m_SubwayCarDeath.OnEnter -= this.HandleSubwayCarDeathOnEnter;
		this.m_SubwayCarEnter.OnEnter -= this.HandleSubwayCarEnterOnEnter;
		this.m_SubwayCarEnter.OnEnter += this.HandleSubwayCarEnterOnEnter;
		this.m_SubwayCarEnter.SetActive(active: true);
		this.m_Timer = 0f;
		this.m_IsActive = true;
		this.m_HasArrived = true;
		this.m_CanReturn = true;
	}

	private void HandleSubwayCarEnterOnEnter(object sender, EventArgs e)
	{
		this.m_SubwayCarEnter.OnEnter -= this.HandleSubwayCarEnterOnEnter;
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.Subway);
		this.m_IsActive = false;
		this.m_CanReturn = false;
		this.m_Timer = 0f;
		GameManager.Instance.Player.ForceAbilitiesCancel();
		GameManager.Instance.Player.ForceAbilitiesCooldown();
		GameManager.Instance.Player.SetState(State.Player.Default);
		GameManager.Instance.Player.LockAbilities();
		GameManager.Instance.DisableTeleport();
		if ((bool)this.m_SubwayCarPlayerParent)
		{
			GameManager.Instance.Player.transform.SetParent(this.m_SubwayCarPlayerParent);
		}
		else
		{
			GameManager.Instance.Player.transform.SetParent(this.m_SubwayCarParent);
		}
		if (this.m_TunnelCollider != null)
		{
			this.m_TunnelCollider.SetActive(value: false);
		}
		this.m_Door.Content.ForceDeactivate();
		this.m_Door.OnDeactivated -= this.HandleSubwayCarOnLeave;
		this.m_Door.OnDeactivated += this.HandleSubwayCarOnLeave;
	}

	private void HandleSubwayCarOnLeave(object sender, EventArgs e)
	{
		this.m_Door.OnDeactivated -= this.HandleSubwayCarOnLeave;
		CameraEffects.ShakeRotation(2f, 0.5f, 10, 90f, fadeOut: false);
		this.ResetSequence();
		this.m_Sequence.Insert(1f, this.m_Subway.DOMove(this.m_SubwayLeave.position, 2f).SetEase(Ease.InSine));
		this.m_Sequence.InsertCallback(1.25f, this.HandlesPlay);
		this.m_Sequence.OnComplete(this.Travel);
		this.OnLeaveVO.Send(this);
	}

	private void HandlesPlay()
	{
		for (int i = 0; i < this.m_Handles.Length; i = unchecked(i + 1))
		{
			Transform target;
			target = this.m_Handles[i].transform;
			target.DOKill();
			target.DOShakeRotation(0.25f, new Vector3(10f, 0f, 0f), 10, 90f, fadeOut: false).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
		}
	}

	private void HandlesStop()
	{
		for (int i = 0; i < this.m_Handles.Length; i = unchecked(i + 1))
		{
			this.m_Handles[i].transform.DOKill();
		}
	}

	private void Travel()
	{
		this.m_Subway.position = this.m_SubwayTravel.position;
		this.Loop();
		this.OnTravelStart.Send(this);
		base.StartCoroutine(this.LoadSections());
	}

	private IEnumerator LoadSections()
	{
		SectionID[] uninitializedSections;
		uninitializedSections = this.m_UninitializedSections;
		foreach (SectionID sectionID in uninitializedSections)
		{
			Task uninitializedTask;
			uninitializedTask = GameManager.Instance.SectionManager.LoadSectionAsync(sectionID, initialize: false);
			while (!uninitializedTask.IsCompleted)
			{
				yield return null;
			}
		}
		Task task;
		task = GameManager.Instance.SectionManager.LoadSectionAsync(this.m_SectionID);
		while (!task.IsCompleted)
		{
			yield return null;
		}
		this.Traveled();
	}

	private void Traveled()
	{
		this.m_IsTraveled = true;
	}

	private void Loop()
	{
		CameraEffects.ShakeRotation(1.5f, 0.5f, 10, 90f, fadeOut: false);
		this.ResetTunnelSequence();
		if (this.m_LoopIndex >= this.m_LoopMax)
		{
			if (this.m_IsTraveled)
			{
				this.m_Subway.position = this.m_CityStart.position;
				this.m_Subway.eulerAngles = this.m_CityStart.eulerAngles;
				this.ResetSequence();
				this.OnTravelEnd.Send(this);
				this.OnArriveVO.Send(this);
				this.m_Sequence.Insert(0f, this.m_Subway.DOMove(this.m_CityEnd.position, 4f).SetEase(Ease.OutSine));
				this.m_Sequence.OnComplete(this.TraveledComplete);
			}
			else
			{
				this.LoopTravel();
			}
		}
		else
		{
			this.LoopTravel();
		}
	}

	private void TraveledComplete()
	{
		this.HandlesStop();
		CameraEffects.ShakeRotation(0.5f, 5f);
		GameManager.Instance.Player.UnlockAbilities();
		GameManager.Instance.EnableTeleport();
		GameManager.Instance.Player.transform.SetParent(null);
		global::UnityEngine.Object.DontDestroyOnLoad(GameManager.Instance.Player.gameObject);
		GameManager.Instance.Player.ResetRotation();
		this.m_Door.Content.ForceActivate();
		this.m_CityEnter.OnEnter -= this.HandleCityOnEnter;
		this.m_CityEnter.OnEnter += this.HandleCityOnEnter;
		this.m_CityEnter.SetActive(active: true);
	}

	private void HandleCityOnEnter(object sender, EventArgs e)
	{
		this.m_CityEnter.OnEnter -= this.HandleCityOnEnter;
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
		this.Leave();
	}

	private void LoopTravel()
	{
		unchecked
		{
			this.m_LoopIndex++;
			this.m_TunnelSequence.InsertCallback(0f, () =>
			{
				this.m_Tunnel.position = this.m_TunnelStart.position;
			});
			this.m_TunnelSequence.Insert(0f, this.m_Tunnel.DOMove(this.m_TunnelEnd.position, 1.5f).SetEase(Ease.Linear));
			this.m_TunnelSequence.OnComplete(this.Loop);
		}
	}

	public void Leave()
	{
		this.m_Door.Content.ForceDeactivate();
		this.ResetSequence();
		this.m_Sequence.Insert(1f, this.m_Subway.DOMove(this.m_CityLeave.position, 3f).SetEase(Ease.InSine).SetUpdate(UpdateType.Fixed));
		this.m_Sequence.OnComplete(this.LeaveComplete);
		this.m_SubwayCarEnter.SetActive(active: false);
		this.m_SubwayCarDeath.OnEnter -= this.HandleSubwayCarDeathOnEnter;
		this.m_SubwayCarDeath.OnEnter += this.HandleSubwayCarDeathOnEnter;
		this.m_SubwayCarDeath.SetActive(active: true);
		this.OnLeave.Send(this);
	}

	private void LeaveComplete()
	{
		GameManager.Instance.SectionManager.Remove(this.m_CurrentSectionID);
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
			this.m_SubwayEventTrigger.OnEnter -= this.HandleSubwayEventTriggerOnEnter;
			this.m_SubwayEventTrigger.OnEnter += this.HandleSubwayEventTriggerOnEnter;
			this.m_SubwayEventTrigger.SetActive(active: true);
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("SubwayCarTravel :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		this.CheckStatus();
	}

	private void HandleSubwayEventTriggerOnEnter(object sender, EventArgs e)
	{
		this.m_SubwayEventTrigger.OnEnter -= this.HandleSubwayEventTriggerOnEnter;
		this.m_IsActive = true;
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

	private void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		this.m_SubwayEventTrigger.OnEnter -= this.HandleSubwayEventTriggerOnEnter;
		this.m_SubwayCarEnter.OnEnter -= this.HandleSubwayCarEnterOnEnter;
		this.m_Door.OnDeactivated -= this.HandleSubwayCarOnLeave;
		this.m_SubwayCarDeath.OnEnter -= this.HandleSubwayCarDeathOnEnter;
	}

	protected override void OnDisposed()
	{
		this.OnArrive = null;
		this.OnLeave = null;
		this.OnTravelStart = null;
		this.OnTravelEnd = null;
		this.OnArriveVO = null;
		this.OnLeaveVO = null;
		this.OnHorn = null;
		this.m_Section = null;
		this.RemoveListeners();
		this.KillTunnelSequence();
		this.KillSequence();
		base.OnDisposed();
	}
}
