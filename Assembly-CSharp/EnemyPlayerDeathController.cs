using System;
using DG.Tweening;
using UnityEngine;

public class EnemyPlayerDeathController : JMonoBehaviour
{
	[SerializeField]
	private Character m_Target;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private AnimationClip m_TargetDeathAnimation;

	[SerializeField]
	private AnimationClip m_PlayerDeathAnimation;

	public void Activate()
	{
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnRespawn += this.HandlePlayerOnRespawn;
		if (this.m_TargetDeathAnimation != null)
		{
			GameManager.Instance.Player.OnDeath += this.HandlePlayerOnDeath;
			GameManager.Instance.Player.SetDeathSequence(active: true);
		}
	}

	public void Deactivate()
	{
		GameManager.Instance.Player.OnRespawn -= this.HandlePlayerOnRespawn;
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		if (this.m_TargetDeathAnimation != null)
		{
			GameManager.Instance.Player.SetDeathSequence(active: false);
			this.m_Target.OnAnimationComplete -= this.HandleTargetOnAnimationComplete;
		}
	}

	private void HandlePlayerOnRespawn(object sender, EventArgs e)
	{
		if (this.m_Target != null && this.m_StartLocation != null)
		{
			this.m_Target.SetState(State.Character.Cutscene);
			this.m_Target.SetTarget(null);
			this.m_Target.transform.position = this.m_StartLocation.position;
			this.m_Target.transform.eulerAngles = this.m_StartLocation.eulerAngles;
			this.m_Target.CancelPath();
			this.m_Target.Content.Animator.SetMovementState(0f, smooth: false);
			this.m_Target.Content.Animator.SetMovementSpeed(0f, smooth: false);
			this.m_Target.ClearAnimationTriggers();
			this.m_Target.Content.Animator.enabled = false;
			this.m_Target.Content.Animator.enabled = true;
			DOTween.Sequence().InsertCallback(4f, () =>
			{
				this.m_Target.SetTarget(GameManager.Instance.Player.transform);
				this.m_Target.SetState(State.Character.Follow);
				this.m_Target.Agent.ResetAgent();
			});
		}
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		this.m_TargetDeathAnimation.name = "Cutscene";
		this.m_Target.Content.UpdateClipOverrides(this.m_TargetDeathAnimation);
		this.m_Target.Content.SetAnimationTrigger("Cutscene");
		this.m_PlayerDeathAnimation.name = "Cutscene";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_PlayerDeathAnimation);
		GameManager.Instance.Player.Death();
		CameraEffects.Damage();
		GameManager.Instance.Player.transform.DORotate(this.m_Target.transform.eulerAngles + new Vector3(0f, 180f, 0f), 0.3f).SetEase(Ease.InOutSine);
		GameManager.Instance.Player.transform.DOMove(this.m_Target.transform.position + this.m_Target.transform.forward * 5f, 0.3f).SetEase(Ease.InOutSine);
		this.m_Target.OnAnimationComplete -= this.HandleTargetOnAnimationComplete;
		this.m_Target.OnAnimationComplete += this.HandleTargetOnAnimationComplete;
	}

	private void HandleTargetOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Target.OnAnimationComplete -= this.HandleTargetOnAnimationComplete;
		GameManager.Instance.Player.ForceRespawn(0.3f);
	}
}
