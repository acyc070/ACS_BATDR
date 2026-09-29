using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class StrikerNest : ButcherGangSpawner
{
	[Header("Trigger")]
	[SerializeField]
	private EventTrigger m_EventTrigger;

	[Header("Locations")]
	[SerializeField]
	private Transform m_EnterLocation;

	[SerializeField]
	private Transform m_ExitLocation;

	[Header("Effects")]
	[SerializeField]
	private GameObject m_Particles;

	[Header("Enemy Animation Clips")]
	[SerializeField]
	private AnimationClipGroup m_EnemySpawnClip;

	[SerializeField]
	private AnimationClipGroup m_EnemyReturnClip;

	[Header("Prefabs")]
	[SerializeField]
	private Enemy m_Prefab;

	private Enemy m_Enemy;

	private Sequence m_ActiveSequence;

	private Vector3 m_InitialEnterPosition;

	private Vector3 m_InitialExitPosition;

	private float m_ActiveTimer;

	private float m_ActiveTimerLimit = 7.5f;

	private float m_BrokenPathTimer;

	private float m_BrokenPathTimerLimit = 0.75f;

	public override void Start()
	{
		this.m_InitialEnterPosition = this.m_EnterLocation.position;
		this.m_InitialExitPosition = this.m_ExitLocation.position;
		Vector3 position;
		position = base.transform.position;
		position.y -= 2f;
		Vector3 end;
		end = position;
		end.y -= 50f;
		if (Physics.Linecast(position, end, out var hitInfo, -1, QueryTriggerInteraction.Ignore))
		{
			Vector3 position2;
			position2 = this.m_EnterLocation.position;
			position2.y = hitInfo.point.y;
			this.m_EnterLocation.position = position2;
			Vector3 position3;
			position3 = this.m_ExitLocation.position;
			position3.y = hitInfo.point.y;
			this.m_ExitLocation.position = position3;
			Vector3 position4;
			position4 = this.m_EventTrigger.transform.position;
			position4.y = hitInfo.point.y + 5f;
			this.m_EventTrigger.transform.position = position4;
		}
	}

	private void Update()
	{
		if (!base.IsSpawned || this.m_Enemy == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.m_ActiveTimer >= this.m_ActiveTimerLimit || GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			this.Return();
			return;
		}
		this.m_ActiveTimer += Time.deltaTime;
		if (!(this.m_Enemy.Target == GameManager.Instance.Player.transform))
		{
			return;
		}
		if (this.m_Enemy.Agent.Agent.pathStatus == NavMeshPathStatus.PathInvalid || this.m_Enemy.Agent.Agent.pathStatus == NavMeshPathStatus.PathPartial)
		{
			if (this.m_BrokenPathTimer >= this.m_BrokenPathTimerLimit)
			{
				this.Return();
			}
			else
			{
				this.m_BrokenPathTimer += Time.deltaTime;
			}
		}
		else
		{
			this.m_BrokenPathTimer = 0f;
		}
	}

	protected override void Enable()
	{
		this.m_Particles.SetActive(value: true);
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.OnEnter += this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.SetActive(active: true);
		this.m_EventTrigger.ResetAction();
		base.SendOnEnabled();
	}

	protected override void Disable()
	{
		this.m_Particles.SetActive(value: false);
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.SetActive(active: false);
		base.SendOnDisabled();
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.SetActive(active: false);
		this.m_Particles.SetActive(value: false);
		this.m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(this.m_Prefab);
		this.m_Enemy.SetSection(base.gameObject.GetComponentInParent<Section>().SectionID);
		Vector3 position;
		position = GameManager.Instance.Player.transform.position;
		position.y = this.m_EnterLocation.transform.position.y;
		Vector3 normalized;
		normalized = (position - this.m_EnterLocation.transform.position).normalized;
		this.m_Enemy.transform.position = this.m_EnterLocation.position;
		this.m_Enemy.transform.rotation = Quaternion.LookRotation(normalized);
		this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		this.m_Enemy.OnInitializeOnComplete += this.HandleEnemyOnInitializeOnComplete;
		this.ResetActiveSequence();
		this.m_ActiveSequence.InsertCallback(0.2f, () =>
		{
			CameraEffects.Damage();
			Vector3 position2;
			position2 = base.transform.position;
			position2.y = GameManager.Instance.Player.transform.position.y;
			GameManager.Instance.Player.PlayerMovement.SetPreviouslyGrounded(active: true);
			GameManager.Instance.Player.AddForce((GameManager.Instance.Player.transform.position - position2).normalized * 25f);
			GameManager.Instance.Player.PlayerMovement.SetPreviouslyGrounded(active: true);
		});
		base.SendOnSpawned();
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		this.m_Enemy.SetState(State.Character.Cutscene);
		this.m_Enemy.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		base.IsSpawned = true;
		this.m_Enemy.Controller.enabled = false;
		this.m_Enemy.transform.position = this.m_EnterLocation.position;
		this.m_Enemy.Content.transform.position = this.m_InitialEnterPosition;
		this.m_ActiveSequence.Insert(0.3f, this.m_Enemy.Content.transform.DOMove(this.m_EnterLocation.position, 0.2f).SetEase(Ease.Linear));
		this.m_EnemySpawnClip.Initialize();
		this.m_Enemy.Content.UpdateClipOverrides(this.m_EnemySpawnClip.AnimationClip);
		this.m_Enemy.Content.SetAnimationTrigger("InteractInstant");
		this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleSpawnedEnemyOnAnimationComplete;
	}

	private void HandleSpawnedEnemyOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
		this.m_Enemy.Content.transform.localPosition = Vector3.zero;
		this.m_Enemy.transform.position = this.m_EnterLocation.position;
		this.m_Enemy.SetTarget(GameManager.Instance.Player.transform);
		this.m_Enemy.SetState(State.Character.Follow);
		this.m_Enemy.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		this.m_Enemy.Agent.ResetAgent();
		this.m_Enemy.Controller.enabled = true;
	}

	private void Return()
	{
		base.IsSpawned = false;
		this.m_ActiveTimer = 0f;
		this.m_Enemy.Controller.enabled = false;
		this.m_Enemy.ForceStop();
		this.m_Enemy.SetDisableVision(active: true);
		this.m_Enemy.SetNode(this.m_ExitLocation.GetComponent<CharacterNode>());
		this.m_Enemy.SetTarget(this.m_ExitLocation);
		this.m_Enemy.SetState(State.Character.Flee);
		this.m_Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
		this.m_Enemy.OnNodeReached += this.HandleEnemyOnNodeReached;
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		this.m_Enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
		this.m_Enemy.SetState(State.Character.Cutscene);
		this.m_Enemy.Agent.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		this.m_Enemy.Controller.enabled = false;
		this.ResetActiveSequence();
		this.m_ActiveSequence.Insert(0.3f, this.m_Enemy.transform.DOMove(this.m_EnterLocation.position, 0.25f));
		this.m_ActiveSequence.Insert(0.3f, this.m_Enemy.Content.transform.DOMove(this.m_InitialExitPosition, 0.25f));
		this.m_EnemyReturnClip.Initialize();
		this.m_Enemy.Content.UpdateClipOverrides(this.m_EnemyReturnClip.AnimationClip);
		this.m_Enemy.Content.SetAnimationTrigger("InteractInstant");
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyReturnOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleEnemyReturnOnAnimationComplete;
		base.StartCoroutine(this.ClearEnemyDelay());
	}

	private IEnumerator ClearEnemyDelay()
	{
		yield return new WaitForSeconds(5f);
		yield return new WaitForEndOfFrame();
		if (!base.IsSpawned)
		{
			this.ClearEnemy();
			base.SendOnReturned();
		}
	}

	private void HandleEnemyReturnOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleEnemyReturnOnAnimationComplete;
		this.ClearEnemy();
		base.SendOnReturned();
		base.StopCoroutine(this.ClearEnemyDelay());
	}

	private void ClearEnemy()
	{
		if (this.m_Enemy != null)
		{
			this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
			this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
			this.m_Enemy.OnAnimationComplete -= this.HandleEnemyReturnOnAnimationComplete;
			this.m_Enemy.Dispose();
			this.m_Enemy = null;
		}
	}

	private void ResetActiveSequence()
	{
		this.KillActiveSequence();
		this.m_ActiveSequence = DOTween.Sequence();
	}

	private void KillActiveSequence()
	{
		if (this.m_ActiveSequence != null)
		{
			this.m_ActiveSequence.Kill();
			this.m_ActiveSequence = null;
		}
	}

	public override void Cancel()
	{
		this.KillActiveSequence();
		this.ClearEnemy();
		base.SendOnReturned();
	}

	protected override void OnDisposed()
	{
		this.ClearEnemy();
		this.KillActiveSequence();
		base.OnDisposed();
	}
}
