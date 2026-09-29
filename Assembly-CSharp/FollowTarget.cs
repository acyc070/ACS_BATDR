using UnityEngine;

public class FollowTarget : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Target;

	[SerializeField]
	private bool m_LockX;

	[SerializeField]
	private bool m_LockY;

	[SerializeField]
	private bool m_LockZ;

	[SerializeField]
	private bool m_IsFixed;

	private void Update()
	{
		if (!this.m_IsFixed && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			this.Follow();
		}
	}

	private void FixedUpdate()
	{
		if (this.m_IsFixed && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			this.Follow();
		}
	}

	private void Follow()
	{
		Vector3 position;
		position = this.m_Target.position;
		if (this.m_LockX)
		{
			position.x = base.transform.position.x;
		}
		if (this.m_LockY)
		{
			position.y = base.transform.position.y;
		}
		if (this.m_LockZ)
		{
			position.z = base.transform.position.z;
		}
		base.transform.position = position;
	}
}
