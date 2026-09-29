using System;
using System.Collections;

public class JCoroutine : JMonoBehaviour
{
	private Action m_Callback;

	public event EventHandler OnComplete;

	public void StartCoroutine(IEnumerator coroutine, Action callback)
	{
		this.m_Callback = callback;
		base.StartCoroutine(this.Coroutine(coroutine));
	}

	private IEnumerator Coroutine(IEnumerator coroutine)
	{
		yield return coroutine;
		this.m_Callback?.Invoke();
		this.OnComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		this.m_Callback = null;
		base.OnDisposed();
	}
}
