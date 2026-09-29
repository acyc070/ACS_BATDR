using DG.Tweening;
using S13Audio;
using UnityEngine;

public class Lurker : Hittable
{
	[SerializeField]
	private CharacterContent m_CharacterContent;

	[SerializeField]
	private AnimationClip m_LurkerDeathClip;

	[SerializeField]
	private AnimationClip m_AudreyDeathClip;

	[SerializeField]
	private Transform m_DeathLocation;

	[SerializeField]
	private S13AudioHandler m_DeathAudio;

	[SerializeField]
	private Material m_DeathMaterial;

	private int m_HitCount;

	private int m_HitCountMax = 3;

	protected override bool InternalHit(RaycastHit hit)
	{
		GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Ink", 5f).GetComponent<Impact>().Initialize(hit, isPooled: true);
		if (GameManager.Instance.Player != null && GameManager.Instance.Player.CurrentWeapon != null && GameManager.Instance.Player.CurrentWeapon.IsCharged)
		{
			GentPipeChargeEffects componentInChildren;
			componentInChildren = GameManager.Instance.Player.CurrentWeapon.Weapon.GetComponentInChildren<GentPipeChargeEffects>();
			if (componentInChildren != null)
			{
				componentInChildren.Hit();
				GameManager.Instance.PoolingManager.GetFromPool("Impacts/Impact_Electric", 5f).GetComponent<Impact>().Initialize(hit, isPooled: true);
			}
		}
		unchecked
		{
			this.m_HitCount++;
			if (this.m_HitCount >= this.m_HitCountMax)
			{
				this.Death();
			}
			return true;
		}
	}

	private void Death()
	{
		AnimationCycler component;
		component = base.gameObject.GetComponent<AnimationCycler>();
		if (component != null)
		{
			component.Disable();
		}
		this.m_DeathAudio.Play();
		GameManager.Instance.Player.SetDisable(disable: true);
		GameManager.Instance.UIManager.Clear();
		this.m_LurkerDeathClip.name = "Death";
		this.m_CharacterContent.UpdateClipOverrides(this.m_LurkerDeathClip);
		this.m_CharacterContent.Animator.SetTrigger("Death");
		GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
		this.m_AudreyDeathClip.name = "Interact";
		GameManager.Instance.Player.UpdatePlayerContentClipOverrides(this.m_AudreyDeathClip);
		GameManager.Instance.Player.EnterInteraction("Interact");
		GameManager.Instance.Player.transform.DOMove(this.m_DeathLocation.position, 0.4f).SetEase(Ease.Linear);
		GameManager.Instance.Player.transform.DORotate(this.m_DeathLocation.eulerAngles, 0.4f).SetEase(Ease.Linear);
		SkinnedMeshRenderer[] componentsInChildren;
		componentsInChildren = this.m_CharacterContent.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			componentsInChildren[i].material = this.m_DeathMaterial;
		}
	}

	public void GameOver()
	{
		GameManager.Instance.ShowGameOver(GameOverType.Lurker);
	}
}
