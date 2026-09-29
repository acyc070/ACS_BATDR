using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

public class S13SimpleAreaSound : MonoBehaviour, IS13AreaSound
{
	[Tooltip("The transform that should be moved toward the target.")]
	public Transform follower;

	[Tooltip("Defines the set of colliders wherein the follower may venture.")]
	public List<Collider> triggers = new List<Collider>();

	private Vector3 _targetPosition = Vector3.zero;

	private bool _targetWithinTrigger;

	public Vector3 targetPosition
	{
		get
		{
			return this._targetPosition;
		}
		set
		{
			this._targetPosition = value;
			this.UpdateBoundedPosition();
		}
	}

	public bool targetWithinTrigger
	{
		get
		{
			return this._targetWithinTrigger;
		}
		set
		{
			this._targetWithinTrigger = value;
		}
	}

	public event Action onAreaEnter;

	public event Action onAreaExit;

	public void GetContainedTriggers()
	{
		this.triggers = (from x in base.GetComponentsInChildren<Collider>()
			where x.isTrigger
			select x).ToList();
	}

	private void UpdateBoundedPosition()
	{
		if (!this.follower || this.triggers == null || this.triggers.Count <= 0)
		{
			return;
		}
		float num;
		num = float.PositiveInfinity;
		float num2;
		num2 = float.PositiveInfinity;
		Vector3 positiveInfinity;
		positiveInfinity = Vector3.positiveInfinity;
		Vector3 position;
		position = Vector3.positiveInfinity;
		foreach (Collider trigger in this.triggers)
		{
			positiveInfinity = trigger.ClosestPoint(this.targetPosition);
			num = Vector3.Distance(this.targetPosition, positiveInfinity);
			if (num <= float.Epsilon)
			{
				this.follower.position = this.targetPosition;
				if (!this.targetWithinTrigger)
				{
					S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " Entered", base.gameObject, S13Debug.LogType.AreaSound);
					if (S13Settings.Debug_AreaSoundUI)
					{
						S13Debug.DisplayCanvasText("Area_" + base.transform.parent.name, "");
					}
					this.onAreaEnter?.Invoke();
					this.targetWithinTrigger = true;
				}
				return;
			}
			if (num < num2)
			{
				num2 = num;
				position = positiveInfinity;
			}
		}
		this.follower.position = position;
		if (this.targetWithinTrigger)
		{
			S13Debug.Log(base.transform.parent?.name + " " + base.gameObject.name + " Exited", base.gameObject, S13Debug.LogType.AreaSound);
			if (S13Settings.Debug_AreaSoundUI)
			{
				S13Debug.HideCanvasText("Area_" + base.transform.parent.name);
			}
			this.onAreaExit?.Invoke();
			this.targetWithinTrigger = false;
		}
	}
}
