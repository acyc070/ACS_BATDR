using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class FisherCloset : ButcherGangSpawner
{
	[Header("Closet")]
	[SerializeField]
	private InteractableHide m_Closet;

	[SerializeField]
	private Transform m_Door;

	[SerializeField]
	private AnimatorOverrider m_DoorAnimator;

	[SerializeField]
	private AnimationClipGroup m_DoorSpawnClip;

	[SerializeField]
	private AnimationClipGroup m_DoorReturnClip;

	[Header("Interactable")]
	[SerializeField]
	private Interactable m_Interactable;

	[Header("Locations")]
	[SerializeField]
	private Transform m_EnterLocation;

	[SerializeField]
	private Transform m_ExitLocation;

	[Header("ActiveSetter")]
	[SerializeField]
	private ActiveSetter m_ActiveSetter;

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

	private float m_ActiveTimer;

	private float m_ActiveTimerLimit = 8f;

	private float m_BrokenPathTimer;

	private float m_BrokenPathTimerLimit = 1.5f;

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
		this.m_ActiveSetter.SetActive(active: true);
		this.m_DoorAnimator.enabled = false;
		this.DODoorAnimation();
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
		this.m_Interactable.SetActive(active: true);
		this.m_Interactable.ResetAction();
		base.SendOnEnabled();
	}

	protected override void Disable()
	{
		this.m_ActiveSetter.SetActive(active: false);
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_Interactable.SetActive(active: false);
		base.SendOnDisabled();
	}

	private void DODoorAnimation()
	{
		if (!this.m_DoorAnimator.enabled)
		{
			this.ResetActiveSequence();
			this.m_ActiveSequence.Insert(2f, this.m_Door.DOPunchRotation(new Vector3(0f, 1f, 0f), 0.4f).SetEase(Ease.Linear));
			this.m_ActiveSequence.OnComplete(this.DODoorAnimation);
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_Interactable.SetActive(active: false);
		this.m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(this.m_Prefab);
		this.m_Enemy.SetSection(base.gameObject.GetComponentInParent<Section>().SectionID);
		this.m_Enemy.transform.position = this.m_EnterLocation.position;
		this.m_Enemy.transform.rotation = this.m_EnterLocation.rotation;
		this.m_Enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		this.m_Enemy.OnInitializeOnComplete += this.HandleEnemyOnInitializeOnComplete;
		this.KillActiveSequence();
		this.m_Door.localEulerAngles = Vector3.zero;
		CameraEffects.Damage();
		Vector3 position;
		position = base.transform.position;
		position.y = GameManager.Instance.Player.transform.position.y;
		GameManager.Instance.Player.PlayerMovement.SetPreviouslyGrounded(active: true);
		GameManager.Instance.Player.AddForce((GameManager.Instance.Player.transform.position - position).normalized * 30f);
		GameManager.Instance.Player.PlayerMovement.SetPreviouslyGrounded(active: true);
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
		this.m_Enemy.transform.rotation = this.m_EnterLocation.rotation;
		this.m_DoorAnimator.enabled = true;
		this.m_DoorSpawnClip.Initialize();
		this.m_DoorAnimator.UpdateClipOverrides(this.m_DoorSpawnClip);
		this.m_DoorAnimator.Animator.SetTrigger("InteractInstant");
		this.m_EnemySpawnClip.Initialize();
		this.m_Enemy.Content.UpdateClipOverrides(this.m_EnemySpawnClip.AnimationClip);
		this.m_Enemy.Content.SetAnimationTrigger("InteractInstant");
		this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleSpawnedEnemyOnAnimationComplete;
	}

	private void HandleSpawnedEnemyOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleSpawnedEnemyOnAnimationComplete;
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
		this.m_ActiveSequence.Insert(0f, this.m_Enemy.transform.DOMove(this.m_ExitLocation.position, 0.1f).SetEase(Ease.Linear));
		this.m_ActiveSequence.Insert(0f, this.m_Enemy.transform.DORotate(this.m_ExitLocation.eulerAngles, 0.1f).SetEase(Ease.Linear));
		this.m_DoorReturnClip.Initialize();
		this.m_DoorAnimator.UpdateClipOverrides(this.m_DoorReturnClip);
		this.m_DoorAnimator.Animator.SetTrigger("InteractInstant");
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
		this.m_Door.localEulerAngles = Vector3.zero;
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
