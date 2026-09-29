using UnityEngine;

public class ClampRotation : JMonoBehaviour
{
	private float m_ClampX = 20f;

	private float m_ClampZ = 20f;

	private Quaternion m_LastRotation;

	public override void Start()
	{
		this.m_LastRotation = base.transform.localRotation;
	}

	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.identity;
		Vector3 localEulerAngles;
		localEulerAngles = base.transform.localEulerAngles;
		if (localEulerAngles.x > this.m_ClampX && localEulerAngles.x < 180f)
		{
			localEulerAngles.x = this.m_ClampX;
		}
		else if (localEulerAngles.x > this.m_ClampX && localEulerAngles.x < 360f - this.m_ClampX)
		{
			localEulerAngles.x = 360f - this.m_ClampX;
		}
		if (localEulerAngles.z > this.m_ClampZ && localEulerAngles.z < 180f)
		{
			localEulerAngles.z = this.m_ClampZ;
		}
		else if (localEulerAngles.z > this.m_ClampZ && localEulerAngles.z < 360f - this.m_ClampZ)
		{
			localEulerAngles.z = 360f - this.m_ClampZ;
		}
		base.transform.localRotation = Quaternion.Slerp(this.m_LastRotation, Quaternion.Euler(localEulerAngles), 1.5f * Time.deltaTime);
		this.m_LastRotation = base.transform.localRotation;
	}
}
