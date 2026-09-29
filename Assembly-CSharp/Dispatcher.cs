using System;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : JMonoBehaviour
{
	private List<Action> m_Pending = new List<Action>();

	private static Dispatcher m_Instance;

	public static Dispatcher Instance => Dispatcher.Initialize();

	public static Dispatcher Initialize()
	{
		if (Dispatcher.m_Instance == null)
		{
			Dispatcher.m_Instance = new GameObject("[Dispatcher]").AddComponent<Dispatcher>();
			global::UnityEngine.Object.DontDestroyOnLoad(Dispatcher.m_Instance);
		}
		return Dispatcher.m_Instance;
	}

	public void Invoke(Action action)
	{
		lock (this.m_Pending)
		{
			this.m_Pending.Add(action);
		}
	}

	private void Update()
	{
		this.InvokePending();
	}

	private void InvokePending()
	{
		lock (this.m_Pending)
		{
			foreach (Action item in this.m_Pending)
			{
				item();
			}
			this.m_Pending.Clear();
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_Pending != null)
		{
			this.m_Pending.Clear();
			this.m_Pending = null;
		}
		base.OnDisposed();
	}
}
