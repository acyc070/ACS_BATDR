using System;
using UnityEngine;

public class Peek : JMonoBehaviour
{
	[DisplayWithoutEdit]
	[SerializeField]
	private int m_ID;

	[SerializeField]
	private InteractableAnimationEnter[] m_Interactions;

	private bool m_IsInactive;

	public int ID => this.m_ID;

	public int CurrentIndex { get; private set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public void SetID(int id)
	{
		this.m_ID = id;
	}

	public void Initialize()
	{
		for (int i = 0; i < this.m_Interactions.Length; i = unchecked(i + 1))
		{
			InteractableAnimationEnter obj;
			obj = this.m_Interactions[i];
			obj.OnInteract -= this.HandleInteractionOnInteract;
			obj.OnInteract += this.HandleInteractionOnInteract;
			obj.OnInteractionComplete -= this.HandleInteractionOnInteractionComplete;
			obj.OnInteractionComplete += this.HandleInteractionOnInteractionComplete;
			obj.OnInteractionExit -= this.HandleInteractionOnInteractionExit;
			obj.OnInteractionExit += this.HandleInteractionOnInteractionExit;
		}
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		bool flag;
		flag = GameManager.Instance.Player.CombatStatus == CombatStatus.Combat;
		for (int i = 0; i < this.m_Interactions.Length; i = unchecked(i + 1))
		{
			InteractableAnimationEnter interactableAnimationEnter;
			interactableAnimationEnter = this.m_Interactions[i];
			if (flag)
			{
				if (!this.m_IsInactive)
				{
					if (interactableAnimationEnter.IsInteracted)
					{
						interactableAnimationEnter.OnInteractionExit -= this.HandleInteractionCombatOnInteractionExit;
						interactableAnimationEnter.OnInteractionExit += this.HandleInteractionCombatOnInteractionExit;
						interactableAnimationEnter.Exit();
						this.OnExit.Send(this);
					}
					if (interactableAnimationEnter.IsActive)
					{
						interactableAnimationEnter.SetActive(active: false);
					}
				}
			}
			else if (this.m_IsInactive && !interactableAnimationEnter.IsActive)
			{
				interactableAnimationEnter.SetActive(active: true);
			}
		}
		this.m_IsInactive = flag;
	}

	public void ForceEnter(PeekDataObject dataObject)
	{
		this.CurrentIndex = dataObject.Index;
		this.m_Interactions[this.CurrentIndex].ForceEnter(dataObject.RotationX, dataObject.RotationY);
	}

	private void HandleInteractionOnInteractionComplete(object sender, EventArgs e)
	{
		InteractableAnimationEnter interactableAnimationEnter;
		interactableAnimationEnter = sender as InteractableAnimationEnter;
		for (int i = 0; i < this.m_Interactions.Length; i = unchecked(i + 1))
		{
			if (interactableAnimationEnter == this.m_Interactions[i])
			{
				this.CurrentIndex = i;
				break;
			}
		}
		this.OnEnter.Send(this);
	}

	private void HandleInteractionOnInteractionExit(object sender, EventArgs e)
	{
		this.OnExit.Send(this);
	}

	private void HandleInteractionCombatOnInteractionExit(object sender, EventArgs e)
	{
		InteractableAnimationEnter obj;
		obj = sender as InteractableAnimationEnter;
		obj.OnInteractionExit -= this.HandleInteractionCombatOnInteractionExit;
		obj.SetActive(active: false);
	}

	private void HandleInteractionOnInteract(object sender, EventArgs e)
	{
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.DisableTeleport();
		}
	}

	private void RemoveListeners()
	{
		for (int i = 0; i < this.m_Interactions.Length; i = unchecked(i + 1))
		{
			InteractableAnimationEnter obj;
			obj = this.m_Interactions[i];
			obj.OnInteract -= this.HandleInteractionOnInteract;
			obj.OnInteractionComplete -= this.HandleInteractionOnInteractionComplete;
			obj.OnInteractionExit -= this.HandleInteractionOnInteractionExit;
			obj.OnInteractionExit -= this.HandleInteractionCombatOnInteractionExit;
		}
	}

	protected override void OnDisposed()
	{
		this.OnExit = null;
		this.OnEnter = null;
		this.RemoveListeners();
		base.OnDisposed();
	}
}
