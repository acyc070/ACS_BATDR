using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActionEventContent<T> : ActionEventContent where T : ActionEventProperties
{
	protected UnityEvent EventActivated;

	protected UnityEvent EventDeactivated;

	[SerializeField]
	protected bool m_Connected;

	[SerializeField]
	protected T[] m_Properties;

	protected Dictionary<int, T> m_PropertiesDictionary = new Dictionary<int, T>();

	protected List<T> m_ActiveProperties = new List<T>();

	protected Sequencer m_Sequencer;

	public override bool IsConnected => this.m_Connected;

	public override bool IsPlaying
	{
		get
		{
			if (this.m_Sequencer != null)
			{
				return this.m_Sequencer.IsPlaying;
			}
			return false;
		}
	}

	public ActionEventController Connectable { get; private set; }

	public override void Initialize(object connected)
	{
		this.Connectable = (ActionEventController)connected;
		this.OnInitialize();
		for (int i = 0; i < this.m_Properties.Length; i = unchecked(i + 1))
		{
			T val;
			val = this.m_Properties[i];
			val.Initialize();
			if (val.Animator != null)
			{
				this.m_Sequencer = new Sequencer(isIndependent: false, UpdateType.Fixed).New();
			}
			if (val.ActionEvent != null)
			{
				int hashCode;
				hashCode = val.ActionEvent.GetHashCode();
				if (!this.m_PropertiesDictionary.ContainsKey(hashCode))
				{
					this.m_PropertiesDictionary.Add(hashCode, val);
				}
			}
		}
		this.RemoveListeners();
		this.AddListeners();
		this.Initialized();
	}

	protected virtual void OnInitialize()
	{
	}

	private void Initialized()
	{
		this.OnInitialized();
	}

	protected virtual void OnInitialized()
	{
	}

	protected void CombatDisable()
	{
		if (!base.IsInactive)
		{
			this.Disable();
		}
	}

	protected void CombatEnable()
	{
		if (base.IsInactive)
		{
			this.Enable();
		}
	}

	private void HandleOnInteract(object sender, EventArgs e)
	{
		if (this.CheckIsInactive())
		{
			return;
		}
		ActionEvent actionEvent;
		actionEvent = (ActionEvent)sender;
		if (!base.IsActivated && !this.m_Connected)
		{
			int hashCode;
			hashCode = actionEvent.GetHashCode();
			this.m_ActiveProperties.Clear();
			if (this.m_PropertiesDictionary.ContainsKey(hashCode))
			{
				this.m_ActiveProperties.Add(this.m_PropertiesDictionary[hashCode]);
			}
		}
		else
		{
			this.GetActiveProperties();
		}
		this.RemoveListeners();
		this.OnAction();
	}

	public override void Action()
	{
		if (!base.IsActivated)
		{
			this.Activate();
		}
		else
		{
			this.Deactivate();
		}
	}

	protected void OnAction()
	{
		if (!base.IsActivated)
		{
			if (!this.CheckIsInactive())
			{
				this.InternalActivate();
			}
		}
		else
		{
			this.Deactivate();
		}
	}

	public override void Activate()
	{
		if (!this.CheckIsInactive())
		{
			this.GetActiveProperties();
			this.InternalActivate();
		}
	}

	public sealed override void ForceActivate()
	{
		this.GetActiveProperties();
		this.OnForceActivate();
		this.InternalActivate();
	}

	protected virtual void OnForceActivate()
	{
	}

	private void InternalActivate()
	{
		this.OnActivate();
		if (!base.IsTooExpensive)
		{
			if ((bool)this.Connectable)
			{
				this.Connectable.SendOnActivate();
			}
			if (this.m_Sequencer != null)
			{
				this.m_Sequencer.OnComplete(this.ActivateComplete);
			}
		}
	}

	protected virtual void OnActivate()
	{
	}

	public sealed override void ForceActivateComplete()
	{
		this.OnForceActivateComplete();
	}

	protected virtual void OnForceActivateComplete()
	{
	}

	protected void ActivateComplete()
	{
		this.ActivateComplete(isForceComplete: false);
	}

	protected void ActivateComplete(bool isForceComplete)
	{
		this.OnActivateComplete();
		this.ResetActionEvents();
		this.RemoveListeners();
		this.AddListeners();
		base.IsActivated = true;
		this.EventActivated?.Invoke();
		if ((bool)this.Connectable && !isForceComplete)
		{
			this.Connectable.SendOnActivated();
		}
	}

	protected virtual void OnActivateComplete()
	{
	}

	public void AnimationComplete()
	{
		this.ActivateComplete();
	}

	public override void Deactivate()
	{
		if (!this.CheckIsInactive())
		{
			this.InternalDeactivate();
		}
	}

	public sealed override void ForceDeactivate()
	{
		this.OnForceDeactivate();
		this.InternalDeactivate();
	}

	protected virtual void OnForceDeactivate()
	{
	}

	private void InternalDeactivate()
	{
		this.OnDeactivate();
		if ((bool)this.Connectable)
		{
			this.Connectable.SendOnOnDeactivate();
		}
		if (this.m_Sequencer != null)
		{
			this.m_Sequencer.OnComplete(this.DeactivateComplete);
		}
	}

	protected virtual void OnDeactivate()
	{
	}

	public sealed override void ForceDeactivateComplete()
	{
		this.OnForceDeactivateComplete();
	}

	protected virtual void OnForceDeactivateComplete()
	{
	}

	protected void DeactivateComplete()
	{
		this.OnDeactivateComplete();
		this.ResetActionEvents();
		this.RemoveListeners();
		this.AddListeners();
		base.IsActivated = false;
		this.EventDeactivated?.Invoke();
		if ((bool)this.Connectable)
		{
			this.Connectable.SendOnOnDeactivated();
		}
	}

	protected virtual void OnDeactivateComplete()
	{
	}

	public sealed override void ForceInactive()
	{
		base.IsInactive = true;
	}

	public sealed override void ForceActive()
	{
		base.IsInactive = false;
		base.IsDisabled = false;
	}

	public sealed override void Disable()
	{
		base.IsInactive = true;
		base.IsDisabled = true;
		for (int i = 0; i < this.m_Properties.Length; i = unchecked(i + 1))
		{
			this.m_Properties[i].ActionEvent?.SetActive(active: false);
		}
		if ((bool)this.Connectable)
		{
			this.Connectable.SendOnDisabled();
		}
		this.InternalDisable();
	}

	protected virtual void InternalDisable()
	{
	}

	public sealed override void Enable()
	{
		base.IsInactive = false;
		base.IsDisabled = false;
		for (int i = 0; i < this.m_Properties.Length; i = unchecked(i + 1))
		{
			T val;
			val = this.m_Properties[i];
			if (val.HasActionEvent)
			{
				val.ActionEvent?.ResetAction();
			}
			val.ActionEvent?.SetActive(active: true);
		}
		if ((bool)this.Connectable)
		{
			this.Connectable.SendOnEnabled();
		}
		this.InternalEnable();
	}

	protected virtual void InternalEnable()
	{
	}

	protected bool CheckIsInactive()
	{
		if (base.IsInactive)
		{
			if (!base.IsActivated)
			{
				this.OnIsInactive();
				if ((bool)this.Connectable)
				{
					this.Connectable.SendOnInactive();
				}
			}
			return true;
		}
		return false;
	}

	public virtual void OnIsInactive()
	{
	}

	protected void DoAnimation(T properties, Vector3 position, Vector3 rotation, float duration, Ease ease, float delay = 0f)
	{
		if (properties.HasActivePosition)
		{
			this.m_Sequencer.Insert(delay, properties.Animator.DOLocalMove(position, duration).SetEase(ease));
		}
		if (properties.HasActiveRotation)
		{
			this.m_Sequencer.Insert(delay, properties.Animator.DOLocalRotate(rotation, duration).SetEase(ease));
		}
	}

	protected void ForceAnimationComplete(T properties, Vector3 position, Vector3 rotation)
	{
		if (properties.HasActivePosition)
		{
			properties.Animator.localPosition = position;
		}
		if (properties.HasActiveRotation)
		{
			properties.Animator.localEulerAngles = rotation;
		}
		this.ActivateComplete(isForceComplete: true);
	}

	protected void GetActiveProperties()
	{
		this.m_ActiveProperties.Clear();
		for (int i = 0; i < this.m_Properties.Length; i = unchecked(i + 1))
		{
			this.m_ActiveProperties.Add(this.m_Properties[i]);
		}
	}

	public override void ResetActionEvents()
	{
		for (int i = 0; i < this.m_Properties.Length; i = unchecked(i + 1))
		{
			T val;
			val = this.m_Properties[i];
			if (val.HasActionEvent)
			{
				val.ActionEvent.ResetAction();
			}
		}
	}

	protected void AddListeners()
	{
		for (int i = 0; i < this.m_Properties.Length; i = unchecked(i + 1))
		{
			T val;
			val = this.m_Properties[i];
			if (val.HasActionEvent)
			{
				val.ActionEvent.OnInteract += this.HandleOnInteract;
			}
		}
	}

	protected void RemoveListeners()
	{
		for (int i = 0; i < this.m_Properties.Length; i = unchecked(i + 1))
		{
			T val;
			val = this.m_Properties[i];
			if (val.HasActionEvent)
			{
				val.ActionEvent.OnInteract -= this.HandleOnInteract;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.EventActivated = null;
		this.EventDeactivated = null;
		this.Connectable = null;
		if (this.m_Sequencer != null)
		{
			this.m_Sequencer.Dispose();
			this.m_Sequencer = null;
		}
		if (this.m_PropertiesDictionary != null)
		{
			this.m_PropertiesDictionary.Clear();
			this.m_PropertiesDictionary = null;
		}
		if (this.m_ActiveProperties != null)
		{
			this.m_ActiveProperties.Clear();
			this.m_ActiveProperties = null;
		}
		base.OnDisposed();
	}
}
public abstract class ActionEventContent : JMonoBehaviour
{
	public bool IsDisabled { get; protected set; }

	public bool IsInactive { get; protected set; }

	public bool IsActivated { get; protected set; }

	public bool IsTooExpensive { get; protected set; }

	public abstract bool IsConnected { get; }

	public abstract bool IsPlaying { get; }

	public abstract void Initialize(object connected);

	public abstract void ResetActionEvents();

	public abstract void Action();

	public abstract void Activate();

	public abstract void ForceActivate();

	public abstract void ForceActivateComplete();

	public abstract void Deactivate();

	public abstract void ForceDeactivate();

	public abstract void ForceDeactivateComplete();

	public abstract void Enable();

	public abstract void Disable();

	public abstract void ForceInactive();

	public abstract void ForceActive();

	public void ResetInactive()
	{
		this.ResetActionEvents();
		this.Enable();
		this.ForceInactive();
	}
}
