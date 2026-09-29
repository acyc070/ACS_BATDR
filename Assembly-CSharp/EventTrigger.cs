using UnityEngine;

public class EventTrigger : ActionEvent
{
	[Header("Trigger Layer")]
	[SerializeField]
	[TagSelector]
	private string m_TriggerTag = "Player";

	public Collider Collider { get; protected set; }

	public Collider[] Colliders { get; protected set; }

	public override void Awake()
	{
		base.gameObject.layer = LayerMask.NameToLayer("EventTrigger");
		this.InternalGetCollider();
	}

	protected void InternalGetCollider()
	{
		this.Collider = base.GetComponent<Collider>();
		if (this.Collider != null && !this.Collider.isTrigger)
		{
			this.Collider.isTrigger = true;
		}
		this.Colliders = base.GetComponentsInChildren<Collider>();
		if (this.Colliders == null)
		{
			return;
		}
		for (int i = 0; i < this.Colliders.Length; i = unchecked(i + 1))
		{
			if (!this.Colliders[i].isTrigger)
			{
				this.Colliders[i].isTrigger = true;
			}
		}
	}

	public override void SetActive(bool active)
	{
		if (this.Collider != null)
		{
			this.Collider.enabled = (base.m_IsActive = active);
		}
		if (this.Colliders != null)
		{
			for (int i = 0; i < this.Colliders.Length; i = unchecked(i + 1))
			{
				this.Colliders[i].enabled = (base.m_IsActive = active);
			}
		}
		this.InternalSetActive(active);
	}

	protected virtual void InternalSetActive(bool active)
	{
	}

	private void OnTriggerEnter(Collider col)
	{
		if (this.CanTrigger(_isTriggered: false, col) && this.InternalEnterCheck(col))
		{
			base.ActivateAction(true, () =>
			{
				this.OnInternalEnter(col);
			}, () =>
			{
				base.SendOnEnter(this);
				base.SendOnInteract(this);
			});
			if (base.m_IsSingleAction)
			{
				base.Clear();
			}
		}
	}

	protected virtual bool InternalEnterCheck(Collider col)
	{
		return true;
	}

	protected virtual void OnInternalEnter(Collider col)
	{
	}

	private void OnTriggerExit(Collider col)
	{
		if (this.CanTrigger(_isTriggered: true, col) && this.InternalExitCheck(col) && !base.m_IsSingleAction)
		{
			base.ActivateAction(false, () =>
			{
				this.OnInternalExit(col);
			}, () =>
			{
				base.SendOnExit(this);
			});
		}
	}

	protected virtual bool InternalExitCheck(Collider col)
	{
		return true;
	}

	protected virtual void OnInternalExit(Collider col)
	{
	}

	protected virtual bool CanTrigger(bool _isTriggered, Collider col)
	{
		if (base.m_IsActive && base.IsActioned == _isTriggered && this.CheckTag(col.tag))
		{
			return this.InternalCanTrigger(col);
		}
		return false;
	}

	protected virtual bool CheckTag(string tag)
	{
		return tag == this.m_TriggerTag;
	}

	protected virtual bool InternalCanTrigger(Collider col)
	{
		return true;
	}

	protected override void OnDisposed()
	{
		this.Collider = null;
		this.Colliders = null;
		base.OnDisposed();
	}
}
