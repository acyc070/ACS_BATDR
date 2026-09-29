using UnityEngine;

public class ObjectiveDisposable : Objective
{
	[SerializeField]
	private DisposableObject m_DisposableObject;

	protected override void InternalInitialize()
	{
		this.m_DisposableObject.Dispose();
		base.SendOnComplete();
	}

	protected override void InternalForceComplete()
	{
		this.m_DisposableObject.Dispose();
	}
}
