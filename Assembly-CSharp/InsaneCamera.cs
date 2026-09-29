using UnityEngine;

public class InsaneCamera : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Target;

	public override void Awake()
	{
		base.transform.LookAt(this.m_Target);
	}

	private void LateUpdate()
	{
		Quaternion b;
		b = Quaternion.LookRotation(this.m_Target.position - base.transform.position);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, 1f * Time.deltaTime);
	}
}
