using System;
using DG.Tweening;
using UnityEngine;

public class HambushController : JMonoBehaviour
{
	[SerializeField]
	private Hittable[] m_Hittables;

	private int m_HitCounter;

	public event EventHandler OnComplete;

	public void Initialize()
	{
		for (int i = 0; i < this.m_Hittables.Length; i = unchecked(i + 1))
		{
			Hittable obj;
			obj = this.m_Hittables[i];
			obj.OnHit -= this.HandleHittableObjectOnHit;
			obj.OnHit += this.HandleHittableObjectOnHit;
		}
	}

	private void HandleHittableObjectOnHit(object sender, EventArgs e)
	{
		(sender as Hittable).OnHit -= this.HandleHittableObjectOnHit;
		unchecked
		{
			this.m_HitCounter++;
			if (this.m_HitCounter >= this.m_Hittables.Length)
			{
				this.OnComplete.Send(this);
				DOTween.Sequence().InsertCallback(10f, () =>
				{
					GameManager.Instance.ShowSubtitles(TextUtility.GetKey("Audrey_Outakes_01"), 5f, isTrimmed: true);
				});
			}
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < this.m_Hittables.Length; i = unchecked(i + 1))
		{
			this.m_Hittables[i].OnHit -= this.HandleHittableObjectOnHit;
		}
		base.OnDisposed();
	}
}
