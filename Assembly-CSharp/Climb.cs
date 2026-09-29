using System;
using UnityEngine;

public class Climb : JMonoBehaviour
{
	[SerializeField]
	private InteractableAnimationBase m_Interactable;

	public bool IsClimbing { get; private set; }

	public event EventHandler OnInteract;

	public event EventHandler OnComplete;

	public override void Awake()
	{
		this.RemoveListeners();
		this.AddListeners();
	}

	public void Inactive()
	{
		ActiveSetter[] componentsInChildren;
		componentsInChildren = base.transform.GetComponentsInChildren<ActiveSetter>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].SetActive(active: true);
		}
		this.Disable();
	}

	public void Enable()
	{
		this.RemoveListeners();
		this.AddListeners();
		this.m_Interactable.SetActive(active: true);
		ActiveSetter[] componentsInChildren;
		componentsInChildren = base.transform.GetComponentsInChildren<ActiveSetter>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].SetActive(active: true);
		}
	}

	public void Disable()
	{
		this.RemoveListeners();
		this.m_Interactable.SetActive(active: false);
	}

	public void ForceActivateComplete()
	{
		this.Enable();
	}

	private void HandleClimbOnInteract(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hide);
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.ClearEnemies();
		this.IsClimbing = true;
		this.OnInteract.Send(this);
	}

	private void HandleClimbOnComplete(object sender, EventArgs e)
	{
		this.OnComplete.Send(this);
		this.IsClimbing = false;
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.EnableTeleport();
		}
		GameManager.Instance.Player.SetCollision(active: true);
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.None);
	}

	private void AddListeners()
	{
		if (this.m_Interactable != null)
		{
			this.m_Interactable.OnInteract += this.HandleClimbOnInteract;
			this.m_Interactable.OnInteractionComplete += this.HandleClimbOnComplete;
		}
	}

	protected void RemoveListeners()
	{
		if (this.m_Interactable != null)
		{
			this.m_Interactable.OnInteract -= this.HandleClimbOnInteract;
			this.m_Interactable.OnInteractionComplete -= this.HandleClimbOnComplete;
		}
	}

	protected override void OnDisposed()
	{
		this.OnInteract = null;
		this.OnComplete = null;
		this.RemoveListeners();
		base.OnDisposed();
	}
}
