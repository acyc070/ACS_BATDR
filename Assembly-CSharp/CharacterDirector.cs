using System;
using System.Collections;
using DG.Tweening;
using S13Audio;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-59)]
public class CharacterDirector : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ID;

	[Header("Director Settings")]
	[SerializeField]
	private bool m_OnStart = true;

	[SerializeField]
	private bool m_SmoothLocationNode;

	[Header("Pathing")]
	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private CharacterPatrolPath m_PatrolPath;

	[SerializeField]
	private Transform m_SpecialPath;

	[Header("Audio")]
	[SerializeField]
	private S13AudioHandler m_S13AudioHandler;

	[Header("Prefab")]
	[SerializeField]
	private Character m_Character;

	private SectionID m_SectionID;

	private Enemy m_Enemy;

	private CharacterInteractionNode m_CurrentNode;

	private int m_CurrentPatrolPathPoint;

	private bool m_HasSpecialPath;

	private bool m_IsInitialized;

	private bool m_IsYoYoing;

	private float m_BrokenPathTimer;

	private float m_BrokenPathTimerLimit = 3f;

	public bool OnStart => this.m_OnStart;

	public bool IsInitialized => this.m_IsInitialized;

	public event EventHandler OnNodeReached;

	public void SetSectionID(SectionID sectionID)
	{
		this.m_SectionID = sectionID;
	}

	public void Initialize()
	{
		if (!this.m_IsInitialized)
		{
			base.StartCoroutine(this.InternalInitialize());
		}
	}

	private IEnumerator InternalInitialize()
	{
		this.m_IsInitialized = true;
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID);
		if (sectionDataObject != null)
		{
			if ((CharacterDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CharacterDirectorDataObject>(sectionDataObject.ID, this.m_ID) == null)
			{
				this.m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(this.m_Character);
				this.m_Enemy.transform.SetParent(base.transform);
				this.m_Enemy.transform.position = this.m_StartLocation.position;
				this.m_Enemy.transform.eulerAngles = this.m_StartLocation.eulerAngles;
				this.m_Enemy.SetTarget(this.m_PatrolPath.Path[this.m_CurrentPatrolPathPoint]);
				this.m_Enemy.Initialize();
				CharacterDirectorDataObject characterDirectorDataObject;
				characterDirectorDataObject = DataObject<int, CharacterDirectorDataObject>.Create(this.m_ID);
				characterDirectorDataObject.SetEnemyType(this.m_Enemy.EnemyType);
				if (this.m_Enemy.Content is LostOneCharacterContent lostOneCharacterContent)
				{
					characterDirectorDataObject.SetLostOneType(lostOneCharacterContent.LostOneType);
				}
				characterDirectorDataObject.SetTransform(this.m_Enemy.transform);
				characterDirectorDataObject.SetPathIndex(this.m_CurrentPatrolPathPoint);
				characterDirectorDataObject.SetSpecialPath(this.m_HasSpecialPath);
				sectionDataObject.CharacterDirectorData.Add(this.m_ID, characterDirectorDataObject);
			}
			else
			{
				yield return this.InternalLoadData();
			}
		}
		yield return null;
	}

	public void LoadData()
	{
		base.StartCoroutine(this.InternalLoadData());
	}

	private IEnumerator InternalLoadData()
	{
		this.m_IsInitialized = true;
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID);
		if (sectionDataObject != null)
		{
			CharacterDirectorDataObject characterDirectorDataObject;
			characterDirectorDataObject = (CharacterDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CharacterDirectorDataObject>(sectionDataObject.ID, this.m_ID);
			if (characterDirectorDataObject != null)
			{
				this.m_CurrentPatrolPathPoint = characterDirectorDataObject.CurrentPathIndex;
				this.m_HasSpecialPath = characterDirectorDataObject.HasSpecialPath;
				this.m_Enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(PrefabCheck.GetEnemy(characterDirectorDataObject.EnemyType));
				this.m_Enemy.transform.SetParent(base.transform);
				if (characterDirectorDataObject.EnemyType == EnemyType.LostOne || characterDirectorDataObject.EnemyType == EnemyType.LostOneColor)
				{
					CharacterContent characterContent;
					characterContent = GameManager.Instance.AssetManager.CreateAsset<CharacterContent>(PrefabCheck.GetCharacter(characterDirectorDataObject.EnemyType, characterDirectorDataObject.LostOneType));
					characterContent.transform.SetParent(this.m_Enemy.transform);
					characterContent.transform.localPosition = Vector3.zero;
					characterContent.transform.localEulerAngles = Vector3.zero;
					characterContent.transform.localScale = Vector3.one;
				}
				this.m_Enemy.transform.position = characterDirectorDataObject.Transform.Position;
				this.m_Enemy.transform.rotation = characterDirectorDataObject.Transform.Rotation;
				if (this.m_HasSpecialPath && this.m_SpecialPath != null)
				{
					this.m_Enemy.SetTarget(this.m_SpecialPath);
				}
				else
				{
					this.m_Enemy.SetTarget(this.m_PatrolPath.Path[characterDirectorDataObject.CurrentPathIndex]);
				}
				this.m_Enemy.Initialize();
			}
		}
		yield return null;
	}

	public void UnloadData()
	{
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID);
		if (sectionDataObject == null)
		{
			return;
		}
		CharacterDirectorDataObject characterDirectorDataObject;
		characterDirectorDataObject = (CharacterDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CharacterDirectorDataObject>(sectionDataObject.ID, this.m_ID);
		if (characterDirectorDataObject != null)
		{
			sectionDataObject.CharacterDirectorData.Remove(characterDirectorDataObject);
			if (this.m_Enemy != null)
			{
				this.m_Enemy.OnAnimationComplete -= this.HandleAnimationComplete;
				this.m_Enemy.Dispose();
				this.m_Enemy = null;
			}
		}
	}

	public void UpdateData()
	{
		if (!(this.m_Enemy != null))
		{
			return;
		}
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID);
		if (sectionDataObject == null)
		{
			return;
		}
		CharacterDirectorDataObject characterDirectorDataObject;
		characterDirectorDataObject = (CharacterDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CharacterDirectorDataObject>(sectionDataObject.ID, this.m_ID);
		if (characterDirectorDataObject != null)
		{
			characterDirectorDataObject.SetEnemyType(this.m_Enemy.EnemyType);
			if (this.m_Enemy.Content is LostOneCharacterContent lostOneCharacterContent)
			{
				characterDirectorDataObject.SetLostOneType(lostOneCharacterContent.LostOneType);
			}
			if (GameManager.Instance.Player != null)
			{
				characterDirectorDataObject.SetCombatState(this.m_Enemy.Target == GameManager.Instance.Player.transform);
			}
			characterDirectorDataObject.SetTransform(this.m_Enemy.transform);
			characterDirectorDataObject.SetPathIndex(this.m_CurrentPatrolPathPoint);
			characterDirectorDataObject.SetSpecialPath(this.m_HasSpecialPath);
		}
	}

	public void InitializeNoData()
	{
		this.m_IsInitialized = true;
		this.m_Enemy = this.m_Character as Enemy;
		this.m_Enemy.SetTarget(this.m_PatrolPath.Path[0]);
		this.m_Enemy.Initialize();
	}

	private void Update()
	{
		if (!this.m_IsInitialized || this.m_Enemy == null || this.m_Enemy.CurrentState == State.Character.Death || this.m_PatrolPath == null)
		{
			return;
		}
		if (this.m_Enemy.Target == null && this.m_Enemy.CurrentNode == null && this.m_Enemy.CurrentState == State.Character.Flee)
		{
			this.m_Enemy.SetTarget(this.m_PatrolPath.Path[this.m_CurrentPatrolPathPoint]);
			this.m_Enemy.SetState(State.Character.Patrol);
		}
		else
		{
			if (this.m_Enemy.Target == null)
			{
				return;
			}
			if (this.m_Enemy.Target != GameManager.Instance.Player.transform)
			{
				if (!(Vector3.Distance(this.m_Enemy.transform.position, this.m_Enemy.Target.position) <= this.m_Enemy.Agent.Agent.stoppingDistance + 0.1f))
				{
					return;
				}
				this.OnNodeReached.Send(this);
				this.m_CurrentNode = this.m_Enemy.Target.GetComponent<CharacterInteractionNode>();
				if (this.m_CurrentNode != null)
				{
					if (this.m_CurrentNode.NodeType == CharacterNodeType.Location)
					{
						this.m_Enemy.SetTarget(null);
						this.m_Enemy.transform.DOKill();
						this.m_Enemy.transform.DOMove(this.m_CurrentNode.transform.position, 0.5f).SetEase(Ease.InOutSine);
						this.m_Enemy.transform.DORotate(this.m_CurrentNode.transform.eulerAngles, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
						{
							this.m_CurrentNode = null;
							this.m_Enemy.CancelPath();
							this.m_Enemy.ForceStop();
							if (this.m_SmoothLocationNode)
							{
								float currentMovementState;
								currentMovementState = this.m_Enemy.Content.Animator.GetFloat("MovementState");
								DOTween.To(() => currentMovementState, (float x) =>
								{
									currentMovementState = x;
								}, 0f, 0.35f).OnUpdate(() =>
								{
									this.m_Enemy.Content.Animator.SetMovementState(currentMovementState, smooth: false);
								});
								float currentCombatState;
								currentCombatState = this.m_Enemy.Content.Animator.GetFloat("CombatState");
								DOTween.To(() => currentCombatState, (float x) =>
								{
									currentCombatState = x;
								}, 0f, 0.35f).OnUpdate(() =>
								{
									this.m_Enemy.Content.Animator.SetCombatState(currentCombatState, smooth: false);
								});
							}
							else
							{
								this.m_Enemy.Content.Animator.SetMovementState(0f, smooth: false);
								this.m_Enemy.Content.Animator.SetCombatState(0f, smooth: false);
							}
						});
					}
					else if (this.m_CurrentNode.NodeType == CharacterNodeType.Interaction)
					{
						if (this.m_Enemy.CurrentState != State.Character.Patrol)
						{
							this.m_Enemy.SetState(State.Character.Patrol);
						}
						this.Interaction("Interact", "Interact");
					}
					else if (this.m_CurrentNode.NodeType == CharacterNodeType.InteractionLoop)
					{
						if (this.m_Enemy.CurrentState != State.Character.Patrol)
						{
							this.m_Enemy.SetState(State.Character.Patrol);
						}
						this.Interaction("SpecialAnimation01", "SpecialAnimationEnter", "SpecialAnimation02");
					}
				}
				else
				{
					if (this.m_Enemy.CurrentState != State.Character.Patrol)
					{
						this.m_Enemy.SetState(State.Character.Patrol);
					}
					this.SetNextPatrolPoint();
					if (!this.m_PatrolPath.IsSinglePath || this.m_CurrentPatrolPathPoint < this.m_PatrolPath.PathLength)
					{
						this.m_Enemy.SetTarget(this.m_PatrolPath.Path[this.m_CurrentPatrolPathPoint]);
					}
				}
				return;
			}
			if (Vector3.Distance(this.m_Enemy.transform.position, this.m_Enemy.Target.position) > this.m_Enemy.CharacterVision.SightRange || GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
			{
				this.LoseTarget();
				return;
			}
			if (this.m_Enemy.Agent.Agent.pathStatus == NavMeshPathStatus.PathPartial || this.m_Enemy.Agent.Agent.pathStatus == NavMeshPathStatus.PathInvalid)
			{
				if (this.m_BrokenPathTimer >= this.m_BrokenPathTimerLimit)
				{
					this.LoseTarget();
					return;
				}
				this.m_BrokenPathTimer += Time.deltaTime;
			}
			else
			{
				this.m_BrokenPathTimer = 0f;
			}
			this.m_Enemy.OnAnimationComplete -= this.HandleAnimationComplete;
		}
	}

	public void TriggerSpecialPath()
	{
		if (!this.m_HasSpecialPath && this.m_Enemy != null && this.m_SpecialPath != null)
		{
			if (this.m_Enemy.CurrentState != State.Character.Patrol)
			{
				this.m_Enemy.SetState(State.Character.Patrol);
			}
			this.m_Enemy.SetTarget(this.m_SpecialPath);
			this.m_HasSpecialPath = true;
			if (this.m_S13AudioHandler != null)
			{
				this.m_S13AudioHandler.transform.position = this.m_Enemy.transform.position;
				this.m_S13AudioHandler.Play();
			}
		}
	}

	private void LoseTarget()
	{
		this.m_HasSpecialPath = false;
		this.m_Enemy.OnAnimationComplete -= this.HandleAnimationComplete;
		this.m_CurrentNode = null;
		this.m_Enemy.SetTarget(this.m_PatrolPath.Path[this.m_CurrentPatrolPathPoint]);
		this.m_Enemy.SetState(State.Character.Flee);
	}

	private void Interaction(string clipName, string triggerName, string loopClipName = "")
	{
		this.m_Enemy.SetTarget(null);
		this.m_Enemy.transform.DOMove(this.m_CurrentNode.transform.position, 0.75f).SetEase(Ease.InOutSine);
		this.m_Enemy.transform.DORotate(this.m_CurrentNode.transform.eulerAngles, 0.75f).SetEase(Ease.InOutSine).OnComplete(() =>
		{
			this.m_Enemy.CancelPath();
			this.m_Enemy.ForceStop();
			this.m_Enemy.Content.Animator.SetCombatState(0f, smooth: false);
			this.m_CurrentNode.Interactable.Trigger();
			this.m_CurrentNode.InteractionClip.name = clipName;
			if (loopClipName != "" && this.m_CurrentNode.InteractionLoopClip != null)
			{
				this.m_CurrentNode.InteractionLoopClip.name = loopClipName;
			}
			if (this.m_CurrentNode.InteractionLoopClip != null)
			{
				this.m_Enemy.Content.UpdateClipOverrides(this.m_CurrentNode.InteractionClip, this.m_CurrentNode.InteractionLoopClip);
			}
			else
			{
				this.m_Enemy.Content.UpdateClipOverrides(this.m_CurrentNode.InteractionClip);
			}
			this.m_Enemy.Content.SetAnimationTrigger(triggerName);
			this.m_CurrentNode.SendOnInteract();
		});
		this.m_Enemy.OnAnimationComplete -= this.HandleAnimationComplete;
		this.m_Enemy.OnAnimationComplete += this.HandleAnimationComplete;
	}

	private void HandleAnimationComplete(object sender, EventArgs e)
	{
		this.m_Enemy.OnAnimationComplete -= this.HandleAnimationComplete;
		if (this.m_CurrentNode != null)
		{
			if (this.m_CurrentNode.EndLocation != null)
			{
				this.m_Enemy.transform.position = this.m_CurrentNode.EndLocation.position;
				this.m_Enemy.transform.eulerAngles = this.m_CurrentNode.EndLocation.eulerAngles;
			}
			this.m_CurrentNode = null;
		}
		if (this.m_Enemy.CurrentState != State.Character.Patrol)
		{
			this.m_Enemy.SetState(State.Character.Patrol);
		}
		if (this.m_HasSpecialPath)
		{
			this.m_HasSpecialPath = false;
		}
		this.SetNextPatrolPoint();
		if (!this.m_PatrolPath.IsSinglePath || this.m_CurrentPatrolPathPoint < this.m_PatrolPath.PathLength)
		{
			this.m_Enemy.SetTarget(this.m_PatrolPath.Path[this.m_CurrentPatrolPathPoint]);
		}
	}

	private void SetNextPatrolPoint()
	{
		unchecked
		{
			if (!this.m_PatrolPath.IsYoYo)
			{
				this.m_CurrentPatrolPathPoint++;
				if (!this.m_PatrolPath.IsSinglePath && this.m_CurrentPatrolPathPoint >= this.m_PatrolPath.PathLength)
				{
					this.m_CurrentPatrolPathPoint = 0;
				}
				else if (this.m_PatrolPath.IsSinglePath && this.m_CurrentPatrolPathPoint >= this.m_PatrolPath.PathLength)
				{
					this.m_CurrentPatrolPathPoint = this.m_PatrolPath.PathLength - 1;
				}
			}
			else if (!this.m_IsYoYoing)
			{
				this.m_CurrentPatrolPathPoint++;
				if (this.m_CurrentPatrolPathPoint >= this.m_PatrolPath.PathLength)
				{
					this.m_CurrentPatrolPathPoint = this.m_PatrolPath.PathLength - 2;
					this.m_IsYoYoing = true;
				}
			}
			else
			{
				this.m_CurrentPatrolPathPoint--;
				if (this.m_CurrentPatrolPathPoint <= 0)
				{
					this.m_CurrentPatrolPathPoint = 0;
					this.m_IsYoYoing = false;
				}
			}
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_Enemy != null)
		{
			this.m_Enemy.OnAnimationComplete -= this.HandleAnimationComplete;
			this.m_Enemy = null;
		}
		this.m_CurrentNode = null;
		this.OnNodeReached = null;
		base.OnDisposed();
	}
}
