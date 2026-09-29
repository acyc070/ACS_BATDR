using UnityEngine;

public class LockBoneTransform : JMonoBehaviour
{
	[SerializeField]
	private Vector3 m_LockedRotation;

	private void LateUpdate()
	{
		if (!base.IsDisposed)
		{
			base.transform.localEulerAngles = this.m_LockedRotation;
		}
	}
}
