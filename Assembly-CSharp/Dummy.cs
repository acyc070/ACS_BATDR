using System;
using UnityEngine;
using UnityEngine.AI;

public class Dummy : JMonoBehaviour, IHittable
{
	[Header("Content")]
	[SerializeField]
	private CharacterContent m_Content;

	[Header("Nav Mesh Agent")]
	[SerializeField]
	private NavMeshAgent m_Agent;

	[SerializeField]
	private Collider m_Collider;

	[Header("Movement")]
	[SerializeField]
	private float m_StopDistance;

	[Header("Other")]
	[SerializeField]
	private bool m_IsImmune;

	[SerializeField]
	private bool m_IsCompanion;

	protected Transform m_Target;

	protected Vector3 m_CurrentTarget;

	private float m_SampleDistance = 25f;

	private bool m_IsDead;

	private bool m_IsAttacking;

	private float m_AttackType;

	private float m_AttackCooldown;

	private float m_AttackCooldownLimit = 0.75f;

	public CharacterContent Content => this.m_Content;

	public NavMeshAgent Agent => this.m_Agent;

	public Transform Target => this.m_Target;

	public Vector3 CurrentTarget => this.m_CurrentTarget;

	public bool IsBroken { get; private set; }

	public bool IsPlayerBreakable { get; private set; }

	public event EventHandler OnDeath;

	public override void Start()
	{
		this.m_Content.GenericAnimationEvents.SetReciever(this);
		if (!this.m_IsCompanion)
		{
			if (GameManager.Instance.BeastBendy != null)
			{
				this.SetTarget(GameManager.Instance.BeastBendy.transform);
			}
		}
		else if (GameManager.Instance.DummyManager != null)
		{
			Dummy closest;
			closest = GameManager.Instance.DummyManager.GetClosest(base.transform);
			if (closest != null)
			{
				this.SetTarget(closest.transform);
			}
		}
		this.ResetAgent();
	}

	private void FixedUpdate()
	{
		if (this.m_IsDead || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.m_Target == null && GameManager.Instance.DummyManager != null)
		{
			Dummy closest;
			closest = GameManager.Instance.DummyManager.GetClosest(base.transform);
			if (closest != null)
			{
				this.SetTarget(closest.transform);
				closest.SetTarget(base.transform);
				Dummy closest2;
				closest2 = GameManager.Instance.DummyManager.GetClosest(closest.transform);
				if (closest2 != null)
				{
					closest2.SetTarget(base.transform);
					closest2 = GameManager.Instance.DummyManager.GetClosest(GameManager.Instance.BeastBendy.transform);
					if (closest2 != null)
					{
						closest2.SetTarget(base.transform);
					}
				}
			}
		}
		if (this.m_Target == null)
		{
			this.Stop();
			return;
		}
		float num;
		num = Vector3.Distance(base.transform.position, this.m_Target.position);
		if (!this.m_IsCompanion)
		{
			this.m_Agent.avoidancePriority = ((num > 99f) ? 99 : unchecked((int)num));
		}
		if (num >= this.m_StopDistance)
		{
			Collider[] array;
			array = Physics.OverlapSphere(base.transform.position + Vector3.up * 4f + base.transform.forward * 2f, 0.5f);
			bool flag;
			flag = false;
			Collider[] array2;
			array2 = array;
			foreach (Collider collider in array2)
			{
				if (!(collider.transform == base.transform) && collider.gameObject.layer == LayerMask.NameToLayer("AI"))
				{
					flag = true;
					break;
				}
			}
			if (!this.m_IsCompanion & flag)
			{
				if (num < 20f)
				{
					this.Stop();
					this.Attack();
				}
				else if (this.MoveTo(this.m_Target.position))
				{
					this.Move();
				}
				else
				{
					this.Stop();
				}
			}
			else if (this.MoveTo(this.m_Target.position))
			{
				this.Move();
			}
			else
			{
				this.Stop();
			}
		}
		else
		{
			this.Stop();
			this.Attack();
		}
	}

	public void SetTarget(Transform target)
	{
		this.m_Target = target;
	}

	public void SetStopDistance(float distance)
	{
		this.m_StopDistance = distance;
	}

	private void Move()
	{
		this.m_Content.Animator.SetMovementState(2f);
		this.m_Content.Animator.SetMovementSpeed(2f);
	}

	private void Stop()
	{
		this.m_Content.Animator.SetMovementState(0f);
		this.m_Content.Animator.SetMovementSpeed(0f);
		this.CancelPath();
	}

	private void Attack()
	{
		if (!this.m_IsAttacking)
		{
			this.LookAtTarget();
		}
		this.m_AttackCooldown += Time.deltaTime;
		if (this.m_AttackCooldown >= this.m_AttackCooldownLimit && !this.m_IsAttacking)
		{
			this.m_IsAttacking = true;
			this.m_Content.Animator.Attack(this.m_AttackType);
			this.m_AttackType++;
			if (this.m_AttackType >= 3f)
			{
				this.m_AttackType = 0f;
			}
		}
	}

	public void OnAttack()
	{
		Vector3 vector;
		vector = base.transform.position + Vector3.up * (this.m_IsCompanion ? 2f : 4f);
		float num;
		num = (this.m_IsCompanion ? 20 : 10);
		float radius;
		radius = 0.2f;
		Vector3 position;
		position = vector + base.transform.forward * (num / 2f);
		string layerName;
		layerName = (this.m_IsCompanion ? "AI" : "Player");
		string layerName2;
		layerName2 = (this.m_IsCompanion ? "Player" : "AI");
		QueryTriggerInteraction queryTriggerInteraction;
		queryTriggerInteraction = ((!this.m_IsCompanion) ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
		Collider[] array;
		array = new Collider[1];
		if (Physics.OverlapSphereNonAlloc(position, num / 2f, array, 1 << LayerMask.NameToLayer(layerName), queryTriggerInteraction) <= 0)
		{
			return;
		}
		Vector3 position2;
		position2 = array[0].transform.position;
		position2.y = vector.y;
		if (!Physics.SphereCast(vector, radius, (position2 - vector).normalized, out var hitInfo, num, ~(1 << LayerMask.NameToLayer(layerName2)), queryTriggerInteraction))
		{
			return;
		}
		if (this.m_IsCompanion)
		{
			Dummy component;
			component = hitInfo.transform.GetComponent<Dummy>();
			if (component != null)
			{
				component.Hit(hitInfo);
			}
		}
		else
		{
			CameraEffects.ShakeRotation(0.25f, 0.75f, 10, 90f, fadeOut: false);
		}
	}

	public void SetPreviousState()
	{
		this.m_Content.Animator.SetMovementState(0f);
		this.m_Content.Animator.SetMovementSpeed(0f);
		this.m_AttackCooldown = 0f;
		this.m_IsAttacking = false;
	}

	private void LookAtTarget()
	{
		Quaternion b;
		b = Quaternion.LookRotation((this.m_Target.position - base.transform.position).normalized);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 5f);
	}

	public void ForceKil()
	{
		if (!this.m_IsImmune)
		{
			this.m_IsDead = true;
			this.m_Agent.enabled = false;
			this.m_Collider.enabled = false;
			this.m_Content.gameObject.SetActive(value: false);
			this.OnDeath.Send(this);
		}
	}

	public void Hit(RaycastHit hit)
	{
		if (!this.m_IsImmune)
		{
			Transform transform;
			transform = GameManager.Instance.PoolingManager.GetFromPool("Effects/Effects_Dummy_Death").transform;
			transform.position = base.transform.position;
			transform.SetParent(null);
			this.m_IsDead = true;
			this.OnDeath.Send(this);
			this.m_Agent.enabled = false;
			this.m_Collider.enabled = false;
			this.m_Content.gameObject.SetActive(value: false);
		}
	}

	public bool MoveTo(Vector3 desiredTarget)
	{
		if (this.QueryNavmesh(desiredTarget, ref this.m_CurrentTarget) && this.m_Agent.pathStatus != NavMeshPathStatus.PathInvalid && !this.m_Agent.isOnOffMeshLink)
		{
			return this.GetPath(this.m_CurrentTarget);
		}
		return false;
	}

	public bool IsTargetReached()
	{
		if ((this.m_Agent.isOnNavMesh || this.m_Agent.isOnOffMeshLink) && !this.m_Agent.pathPending)
		{
			return this.m_Agent.remainingDistance <= this.m_Agent.stoppingDistance;
		}
		return false;
	}

	public bool QueryNavmesh(Vector3 desiredTarget, ref Vector3 actualTarget)
	{
		if (NavMesh.SamplePosition(desiredTarget, out var hit, this.m_SampleDistance, -1))
		{
			actualTarget = hit.position;
			return true;
		}
		return false;
	}

	public bool GetPath(Vector3 destination)
	{
		bool result;
		result = false;
		NavMeshPath navMeshPath;
		navMeshPath = new NavMeshPath();
		unchecked
		{
			if (NavMesh.CalculatePath(base.transform.position, destination, -1, navMeshPath))
			{
				this.m_Agent.SetPath(navMeshPath);
				for (int i = 0; i < navMeshPath.corners.Length; i++)
				{
					if (i < navMeshPath.corners.Length - 1)
					{
						Debug.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
					}
				}
				result = true;
			}
			return result;
		}
	}

	public void CancelPath()
	{
		if (this.m_Agent.isOnNavMesh || this.m_Agent.isOnOffMeshLink)
		{
			this.m_Agent.ResetPath();
			this.m_Agent.CompleteOffMeshLink();
		}
	}

	public void ResetAgent()
	{
		if (this.m_Agent != null)
		{
			this.m_Agent.enabled = false;
			this.m_Agent.enabled = true;
		}
	}

	public void SendOnDeath()
	{
		this.OnDeath.Send(this);
	}
}
