using System;
using DG.Tweening;
using UnityEngine;

public class InkDemonTest : JMonoBehaviour
{
	[Header("Cutscene")]
	[SerializeField]
	private CutsceneContent m_Cutscene;

	[Header("Event Triggers")]
	[SerializeField]
	private EventTrigger m_SafeEventTrigger;

	[SerializeField]
	private EventTrigger m_EventTrigger;

	[Header("Lost One")]
	[SerializeField]
	private Animator m_LostOneAnimator;

	[Header("Ink Demon")]
	[SerializeField]
	private Animator m_Animator;

	[Header("Player")]
	[SerializeField]
	private AnimationClip m_AudreyDeathClip;

	public event EventHandler OnDeath;

	public override void Start()
	{
		this.m_EventTrigger.OnEnter += this.HandleEventTriggerOnEnter;
		if (this.m_SafeEventTrigger != null)
		{
			this.m_SafeEventTrigger.OnEnter += this.HandleSafeEventTriggerOnEnter;
		}
	}

	private void HandleSafeEventTriggerOnEnter(object sender, EventArgs e)
	{
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_SafeEventTrigger.OnEnter -= this.HandleSafeEventTriggerOnEnter;
		this.m_Cutscene.Director.Stop();
		this.m_Cutscene.Dispose();
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_Cutscene.Director.Stop();
		if (this.m_LostOneAnimator != null)
		{
			this.m_LostOneAnimator.SetTrigger("Death");
		}
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.Player.ForceAbilitiesCancel();
		}
		GameManager.Instance.HideTeleport();
		Vector3 position;
		position = GameManager.Instance.Player.transform.position;
		position.y = this.m_Animator.transform.position.y;
		Vector3 normalized;
		normalized = (position - this.m_Animator.transform.position).normalized;
		this.m_Animator.transform.eulerAngles = Quaternion.LookRotation(normalized).eulerAngles;
		GameManager.Instance.Player.transform.DOMove(this.m_Animator.transform.position + normalized * 12f, 0.25f);
		GameManager.Instance.Player.transform.DORotate(Quaternion.LookRotation(-normalized).eulerAngles, 0.25f);
		this.Death();
	}

	private void Death()
	{
		this.OnDeath.Send(this);
		GameManager.Instance.LockPause();
		this.m_Animator.SetTrigger("Death");
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		this.m_AudreyDeathClip.name = "Interact";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_AudreyDeathClip);
		GameManager.Instance.Player.EnterInteraction("Interact");
	}

	public void Respawn()
	{
		GameManager.Instance.ShowGameOver(GameOverType.InkDemon);
	}

	protected override void OnDisposed()
	{
		this.OnDeath = null;
		if (this.m_EventTrigger != null)
		{
			this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		}
		if (this.m_SafeEventTrigger != null)
		{
			this.m_SafeEventTrigger.OnEnter -= this.HandleSafeEventTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
