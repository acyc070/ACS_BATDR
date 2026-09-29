using System;
using UnityEngine;

[Serializable]
public abstract class ActionEventProperties
{
	protected internal Vector3 m_OriginPosition;

	protected internal Vector3 m_OriginRotation;

	public string Name = "New Properties";

	public SwitchType SwitchType;

	public Transform Container;

	public Transform Animator;

	public Transform ActiveLocation;

	public Animator AnimatorController;

	public string AnimationTriggerActivate = "Activate";

	public string AnimationTriggerDeactivate = "Deactivate";

	public string PlayerInteractionTriggerEnter = "SwitchInteractEnter";

	public string PlayerInteractionTriggerExit = "SwitchInteractExit";

	public Transform AnimationLocation;

	public ActionEvent ActionEvent;

	public Vector3 OriginPosition => this.m_OriginPosition;

	public Vector3 OriginRotation => this.m_OriginRotation;

	public bool HasActionEvent => this.ActionEvent != null;

	public bool HasActivePosition
	{
		get
		{
			if (this.ActiveLocation != null)
			{
				return this.ActiveLocation.localPosition != this.m_OriginPosition;
			}
			return false;
		}
	}

	public bool HasActiveRotation
	{
		get
		{
			if (this.ActiveLocation != null)
			{
				return this.ActiveLocation.localEulerAngles != this.m_OriginRotation;
			}
			return false;
		}
	}

	public void Initialize()
	{
		this.OnInitialize();
		if (this.Container != null)
		{
			this.m_OriginPosition = this.Container.localPosition;
			this.m_OriginRotation = this.Container.localEulerAngles;
		}
	}

	public virtual void OnInitialize()
	{
	}
}
