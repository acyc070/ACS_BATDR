using UnityEngine;

public class InteractableInactive : Interactable
{
	[Header("Initialize Requirements")]
	[SerializeField]
	private bool m_UseInitialize;

	[SerializeField]
	private bool m_IgnoreCombatState;

	[Header("Visual Options")]
	[SerializeField]
	protected bool m_IsReal;

	[Header("Input Options")]
	[SerializeField]
	private InteractionType m_InteractionType;

	[Header("Connected Interactable")]
	[SerializeField]
	private Interactable m_Connected;

	private Collider m_InactiveCollider;

	private Collider m_ActiveCollider;

	private bool m_IsInitialized;

	public override void Awake()
	{
		this.m_InactiveCollider = base.gameObject.GetComponent<Collider>();
		this.m_ActiveCollider = this.m_Connected.GetComponent<Collider>();
	}

	public void InitializeInactive()
	{
		this.m_IsInitialized = true;
	}

	public void DisableInitialize()
	{
		this.m_IsInitialized = false;
	}

	private void Update()
	{
		if (this.m_Connected == null || this.m_InactiveCollider == null || this.m_ActiveCollider == null || base.IsDisposed || GameManager.Instance.IsPaused || (this.m_UseInitialize && !this.m_IsInitialized))
		{
			return;
		}
		if (this.m_Connected.IsActive)
		{
			if (base.IsActive)
			{
				this.SetActive(active: false);
				this.m_InactiveCollider.enabled = false;
			}
			this.m_ActiveCollider.enabled = true;
		}
		else
		{
			if (!base.IsActive)
			{
				this.SetActive(active: true);
				this.m_InactiveCollider.enabled = true;
			}
			this.m_ActiveCollider.enabled = false;
		}
	}

	protected override void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null)
	{
		string key;
		key = "";
		if (this.m_InteractionType != InteractionType.NOT_SET_UP)
		{
			key = this.m_InteractionType.ToString();
		}
		if (!this.m_IgnoreCombatState && GameManager.Instance.Player.CombatStatus == CombatStatus.Combat)
		{
			key = InteractionType.INTERACTION_INVALID_COMBAT.ToString();
		}
		string text;
		text = TextUtility.GetKey(key);
		if (this.m_IsReal)
		{
			text = text.ToUpper();
		}
		GameManager.Instance.ShowInteractInvalid(text, this.m_IsReal);
	}

	protected override void OnInternalExit(Vector3 origin, RaycastHit hit, object sender = null)
	{
		GameManager.Instance.HideInteractInvalid();
	}

	protected override bool InternalInteractCheck(Vector3 origin, RaycastHit hit, object sender = null)
	{
		return false;
	}

	protected override void OnDisposed()
	{
		this.m_InactiveCollider = null;
		this.m_ActiveCollider = null;
		base.OnDisposed();
	}
}
