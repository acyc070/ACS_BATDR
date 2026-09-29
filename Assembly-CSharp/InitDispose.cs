using UnityEngine;

public class InitDispose : JMonoBehaviour
{
	[SerializeField]
	private bool m_OnStart;

	public override void Awake()
	{
		if (!base.IsDisposed && !this.m_OnStart)
		{
			base.Dispose();
		}
	}

	public override void Start()
	{
		if (!base.IsDisposed && this.m_OnStart)
		{
			base.Dispose();
		}
	}
}
