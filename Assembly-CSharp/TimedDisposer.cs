using UnityEngine;

public class TimedDisposer : JMonoBehaviour
{
	[SerializeField]
	private float m_TimeDelay;

	public override void OnEnable()
	{
		base.OnEnable();
		base.Invoke("Dispose", this.m_TimeDelay);
	}
}
