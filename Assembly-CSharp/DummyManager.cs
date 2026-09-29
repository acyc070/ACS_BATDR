using System;
using System.Collections.Generic;
using UnityEngine;

public class DummyManager : JDisposable
{
	public const int ACTIVE_DUMMY_LIMIT = 20;

	public List<Dummy> Dummies = new List<Dummy>();

	public bool CanSpawn => this.Dummies.Count < 20;

	public static DummyManager Create()
	{
		return new DummyManager();
	}

	public void AddDummy(Dummy dummy)
	{
		if (!this.Dummies.Contains(dummy))
		{
			dummy.OnDeath -= this.HandleDummyOnDeath;
			dummy.OnDeath += this.HandleDummyOnDeath;
			this.Dummies.Add(dummy);
		}
	}

	private void HandleDummyOnDeath(object sender, EventArgs e)
	{
		Dummy dummy;
		dummy = sender as Dummy;
		dummy.OnDeath -= this.HandleDummyOnDeath;
		this.RemoveDummy(dummy);
		dummy.Dispose();
	}

	public void RemoveDummy(Dummy dummy)
	{
		if (this.Dummies.Contains(dummy))
		{
			this.Dummies.Remove(dummy);
		}
	}

	public void RemoveDummiesOf(Transform transform)
	{
		unchecked
		{
			for (int num = this.Dummies.Count - 1; num >= 0; num--)
			{
				Dummy dummy;
				dummy = this.Dummies[num];
				if (dummy.Target == transform)
				{
					dummy.ForceKil();
				}
			}
		}
	}

	public Dummy GetClosest(Transform transform)
	{
		Dummy result;
		result = null;
		float num;
		num = float.PositiveInfinity;
		Vector3 position;
		position = transform.position;
		foreach (Dummy dummy in this.Dummies)
		{
			if (!(dummy == null) && !(dummy.transform == transform) && !dummy.IsDisposed)
			{
				float num2;
				num2 = Vector3.Distance(dummy.transform.position, position);
				if (num2 < num)
				{
					result = dummy;
					num = num2;
				}
			}
		}
		return result;
	}

	public void KillAll()
	{
		unchecked
		{
			for (int num = this.Dummies.Count - 1; num >= 0; num--)
			{
				this.Dummies[num].ForceKil();
			}
		}
	}

	protected override void OnDisposed()
	{
		if (this.Dummies != null)
		{
			this.Dummies.Clear();
			this.Dummies = null;
		}
		base.OnDisposed();
	}
}
