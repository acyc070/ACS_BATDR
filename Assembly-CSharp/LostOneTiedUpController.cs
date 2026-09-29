using System;
using UnityEngine;

public class LostOneTiedUpController : JMonoBehaviour
{
	[SerializeField]
	private LostOneTiedUp[] m_LostOnes;

	public override void Start()
	{
		this.RemoveListeners();
		this.AddListeners();
	}

	private void HandleLostOneOnInteract(object sender, EventArgs e)
	{
		LostOneTiedUp lostOneTiedUp;
		lostOneTiedUp = sender as LostOneTiedUp;
		for (int i = 0; i < this.m_LostOnes.Length; i = unchecked(i + 1))
		{
			LostOneTiedUp lostOneTiedUp2;
			lostOneTiedUp2 = this.m_LostOnes[i];
			if (lostOneTiedUp2 != lostOneTiedUp)
			{
				lostOneTiedUp2.Disable();
			}
		}
	}

	private void HandleLostOneOnComplete(object sender, EventArgs e)
	{
		LostOneTiedUp lostOneTiedUp;
		lostOneTiedUp = sender as LostOneTiedUp;
		for (int i = 0; i < this.m_LostOnes.Length; i = unchecked(i + 1))
		{
			LostOneTiedUp lostOneTiedUp2;
			lostOneTiedUp2 = this.m_LostOnes[i];
			if (lostOneTiedUp2 != lostOneTiedUp)
			{
				lostOneTiedUp2.Enable();
			}
		}
	}

	private void AddListeners()
	{
		for (int i = 0; i < this.m_LostOnes.Length; i = unchecked(i + 1))
		{
			LostOneTiedUp obj;
			obj = this.m_LostOnes[i];
			obj.OnInteract += this.HandleLostOneOnInteract;
			obj.OnComplete += this.HandleLostOneOnComplete;
		}
	}

	private void RemoveListeners()
	{
		for (int i = 0; i < this.m_LostOnes.Length; i = unchecked(i + 1))
		{
			LostOneTiedUp obj;
			obj = this.m_LostOnes[i];
			obj.OnInteract -= this.HandleLostOneOnInteract;
			obj.OnComplete -= this.HandleLostOneOnComplete;
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
