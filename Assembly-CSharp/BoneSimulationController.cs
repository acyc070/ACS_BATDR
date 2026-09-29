using UnityEngine;

public class BoneSimulationController : JMonoBehaviour
{
	[SerializeField]
	private BoneFollow[] m_BoneSimulations;

	private bool m_IsActive = true;

	private void Update()
	{
		if (!(GameManager.Instance.GameCamera == null) && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			Transform transform;
			transform = GameManager.Instance.GameCamera.transform;
			float minDistance;
			minDistance = Vector3.Distance(transform.position, base.transform.position);
			if (Vector3.Angle(to: (base.transform.position - transform.position).normalized, from: transform.forward) < 90f)
			{
				this.CheckDistance(minDistance, 60f);
			}
			else
			{
				this.CheckDistance(minDistance, 10f);
			}
		}
	}

	private void CheckDistance(float minDistance, float maxDistance)
	{
		if (minDistance > maxDistance)
		{
			this.Disable();
		}
		else
		{
			this.Enable();
		}
	}

	private void Enable()
	{
		if (!this.m_IsActive)
		{
			for (int i = 0; i < this.m_BoneSimulations.Length; i = unchecked(i + 1))
			{
				this.m_BoneSimulations[i].Enable();
			}
			this.m_IsActive = true;
		}
	}

	private void Disable()
	{
		if (this.m_IsActive)
		{
			this.m_IsActive = false;
			for (int i = 0; i < this.m_BoneSimulations.Length; i = unchecked(i + 1))
			{
				this.m_BoneSimulations[i].Disable();
			}
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_BoneSimulations != null)
		{
			for (int i = 0; i < this.m_BoneSimulations.Length; i = unchecked(i + 1))
			{
				this.m_BoneSimulations[i].ClearSimulations();
			}
		}
		base.OnDisposed();
	}
}
