using UnityEngine;
using UnityEngine.AI;

public class CharacterAgent : JDisposable
{
	protected Vector3 m_CurrentTarget;

	private float m_SampleDistance = 25f;

	private bool isLinking;

	public Character Character { get; private set; }

	public NavMeshAgent Agent { get; private set; }

	public Vector3 CurrentTarget => this.m_CurrentTarget;

	public CharacterAgent(Character character)
	{
		this.Character = character;
		this.Agent = this.Character.GetComponent<NavMeshAgent>();
	}

	public bool MoveTo(Vector3 desiredTarget)
	{
		if (this.QueryNavmesh(desiredTarget, ref this.m_CurrentTarget) && this.Agent.pathStatus != NavMeshPathStatus.PathInvalid && !this.Agent.isOnOffMeshLink)
		{
			this.isLinking = false;
			NavMeshPath navMeshPath;
			return this.GetPath(this.m_CurrentTarget, out navMeshPath);
		}
		if (!this.isLinking && this.Agent.isOnOffMeshLink)
		{
			this.Agent.speed = 0.001f;
			this.isLinking = true;
		}
		return false;
	}

	public bool IsTargetReached()
	{
		if ((this.Agent.isOnNavMesh || this.Agent.isOnOffMeshLink) && !this.Agent.pathPending)
		{
			return this.Agent.remainingDistance <= this.Agent.stoppingDistance;
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

	public bool GetPath(Vector3 destination, out NavMeshPath navMeshPath)
	{
		bool result;
		result = false;
		navMeshPath = new NavMeshPath();
		unchecked
		{
			if (NavMesh.CalculatePath(this.Character.transform.position, destination, -1, navMeshPath))
			{
				this.Agent.SetPath(navMeshPath);
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
		if (this.Agent.isOnNavMesh || this.Agent.isOnOffMeshLink)
		{
			this.Agent.ResetPath();
			this.Agent.CompleteOffMeshLink();
		}
	}

	public void ResetAgent()
	{
		if (this.Agent != null)
		{
			this.Agent.enabled = false;
			this.Agent.enabled = true;
		}
	}

	public void SetSampleDistance(float value)
	{
		this.m_SampleDistance = value;
	}

	protected override void OnDisposed()
	{
		this.Agent = null;
		this.Character = null;
		base.OnDisposed();
	}
}
